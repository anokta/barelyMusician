// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI
{
    [AddComponentMenu("BarelyAPI/Musician")]
    public class Musician : MonoBehaviour
    {
        public int MacroGeneratorTypeIndex = 0;
        public int MesoGeneratorTypeIndex = 0;

        public Mood Mood;
        public bool performerFoldout;

        public List<string> PerformerNames;
        public List<InstrumentMeta> Instruments;
        public List<int> MicroGeneratorTypes;

        // Tempo (BPM)
        [SerializeField]
        int initialTempo = 120;
        public int Tempo
        {
            get { return initialTempo; }
            set
            {
                initialTempo = value;
                sequencer.Tempo = (int)(initialTempo * conductor.TempoMultiplier);
            }
        }

        // Song duration in minutes
        [SerializeField]
        float songDuration = 1.0f;
        public float SongDuration
        {
            get { return songDuration; }
            set { songDuration = value; ensemble.SongDurationInSections = sequencer.MinuteToSections(songDuration); }
        }

        // Bars per section
        [SerializeField]
        int barsPerSection = 4;
        public int BarsPerSection
        {
            get { return barsPerSection; }
            set { barsPerSection = value; sequencer.BarCount = barsPerSection; }
        }

        // Beats per bar
        [SerializeField]
        public int beatsPerBar = 4;
        public int BeatsPerBar
        {
            get { return beatsPerBar; }
            set { beatsPerBar = value; sequencer.BeatCount = beatsPerBar; }
        }

        // Fundamental key of the song
        [SerializeField]
        NoteIndex rootNote = NoteIndex.C4;
        public NoteIndex RootNote
        {
            get { return rootNote; }
            set { rootNote = value; conductor.Key = (float)rootNote; }
        }

        // Master volume
        [SerializeField]
        float masterVolume = 1.0f;
        public float MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = value; }
        }

        [SerializeField]
        bool playOnAwake;
        public bool PlayOnAwake
        {
            get { return playOnAwake; }
            set { playOnAwake = value; }
        }

        // Arousal (Passive - Active)
        [SerializeField]
        float energy = 0.5f;
        float energyTarget, energyInterpolationSpeed;
        public float Energy
        {
            get { return energy; }
            set
            {
                energy = value;

                conductor.SetParameters(energy, stress);
                sequencer.Tempo = (int)(initialTempo * conductor.TempoMultiplier);
            }
        }
        // Valence (Happy - Sad) 
        [SerializeField]
        float stress = 0.5f;
        float stressTarget, stressInterpolationSpeed;
        public float Stress
        {
            get { return stress; }
            set
            {
                stress = value;

                conductor.SetParameters(energy, stress);
            }
        }

        Sequencer sequencer;
        public Sequencer Sequencer
        {
            get { return sequencer; }
        }

        Ensemble ensemble;
        public Ensemble Ensemble
        {
            get { return ensemble; }
        }

        Conductor conductor;

        AudioSource audioSource;
        public bool IsPlaying
        {
            get { return (audioSource == null) ? false : audioSource.isPlaying; }
        }
        bool paused;
        public bool IsPaused
        {
            get { return paused; }
        }

        void Awake()
        {
            Init();

            Energy = energyTarget = energy;
            Stress = stressTarget = stress;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.panLevel = 0.0f;
            audioSource.Stop();

            if (playOnAwake)
                Play();
        }

        void Update()
        {
            if (energy != energyTarget)
            {
                if (Mathf.Abs(energy - energyTarget) < 0.01f * energyInterpolationSpeed * Time.deltaTime)
                    Energy = energyTarget;
                else
                    Energy = Mathf.Lerp(energy, energyTarget, energyInterpolationSpeed * Time.deltaTime);
            }
            if (stress != stressTarget)
            {
                if (Mathf.Abs(stress - stressTarget) < 0.01f * stressInterpolationSpeed * Time.deltaTime)
                    Stress = stressTarget;
                else
                    Stress = Mathf.Lerp(stress, stressTarget, stressInterpolationSpeed * Time.deltaTime);
            }
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            sequencer.Update(data.Length / channels);

            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] = Mathf.Clamp(masterVolume * ensemble.GetOutput(), -1.0f, 1.0f);

                // If stereo, copy the mono data to each channel
                if (channels == 2) data[i + 1] = data[i];
            }
        }

        public void Init()
        {
            if (sequencer == null) sequencer = new Sequencer(initialTempo, barsPerSection, beatsPerBar);
            if (conductor == null) conductor = new Conductor((float)rootNote);

            if (ensemble == null)
            {
                MacroGenerator macro = GeneratorFactory.CreateMacroGenerator(MacroGeneratorTypeIndex, sequencer.MinuteToSections(songDuration), true);
                MesoGenerator meso = GeneratorFactory.CreateMesoGenerator(MesoGeneratorTypeIndex, sequencer);

                ensemble = new Ensemble(macro, meso, conductor);
                ensemble.Register(sequencer);

                // performers
                if (PerformerNames == null) PerformerNames = new List<string>();
                if (Instruments == null) Instruments = new List<InstrumentMeta>();
                if (MicroGeneratorTypes == null) MicroGeneratorTypes = new List<int>();

                for (int i = 0; i < PerformerNames.Count; ++i)
                {
                    string name = PerformerNames[i];

                    Instrument instrument = InstrumentFactory.CreateInstrument(Instruments[i]);
                    MicroGenerator micro = GeneratorFactory.CreateMicroGenerator(MicroGeneratorTypes[i], sequencer);
                    Performer performer = new Performer(instrument, micro);
                    performer.Active = Instruments[i].Active;
                    ensemble.AddPerformer(name, performer);
                }
            }
        }

        public void RegisterPerformer(string performerName, InstrumentMeta instrumentMeta, int microGeneratorTypeIndex, int editIndex = -1)
        {
            if (editIndex >= 0)
            {
                if (ensemble != null) ensemble.RemovePerfomer(PerformerNames[editIndex]);

                PerformerNames[editIndex] = performerName;
                DestroyImmediate(Instruments[editIndex]);
                Instruments[editIndex] = instrumentMeta;
                MicroGeneratorTypes[editIndex] = microGeneratorTypeIndex;
            }
            else
            {
                PerformerNames.Add(performerName);
                Instruments.Add(instrumentMeta);
                MicroGeneratorTypes.Add(microGeneratorTypeIndex);
            }

            if (ensemble != null)
            {
                Instrument instrument = InstrumentFactory.CreateInstrument(instrumentMeta);
                MicroGenerator micro = GeneratorFactory.CreateMicroGenerator(microGeneratorTypeIndex, sequencer);

                Performer performer = new Performer(instrument, micro);
                performer.Active = instrumentMeta.Active;
                ensemble.AddPerformer(performerName, performer);
            }
        }

        public void DeregisterPerformer(string performerName)
        {
            int index = PerformerNames.IndexOf(performerName);
            if (index > -1)
            {
                PerformerNames.RemoveAt(index);
                InstrumentMeta meta = Instruments[index];
                Instruments.RemoveAt(index);
                DestroyImmediate(meta);
                MicroGeneratorTypes.RemoveAt(index);

                if (ensemble != null)
                    ensemble.RemovePerfomer(performerName);
            }
        }

        public void Play()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                paused = false;
            }
        }

        public void Pause()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                paused = true;
            }
        }

        public void Stop()
        {
            audioSource.Stop();
            paused = false;

            ensemble.Reset();
            sequencer.Reset();
        }

        public void SetMood(Mood moodType, float smoothness = 0.0f)
        {
            Mood = moodType;
            switch (Mood)
            {
                case Mood.Happy:
                    SetMood(0.5f, 0.0f, smoothness);
                    break;
                case Mood.Tender:
                    SetMood(0.0f, 0.0f, smoothness);
                    break;
                case Mood.Exciting:
                    SetMood(1.0f, 0.0f, smoothness);
                    break;
                case Mood.Sad:
                    SetMood(0.25f, 0.75f, smoothness);
                    break;
                case Mood.Depressed:
                    SetMood(0.0f, 1.0f, smoothness);
                    break;
                case Mood.Angry:
                    SetMood(1.0f, 1.0f, smoothness);
                    break;
                case Mood.Custom:
                    break;
                default:
                    SetMood(0.5f, 0.5f, smoothness);
                    break;
            }
        }

        public void SetMood(float energy, float stress, float smoothness = 0.0f)
        {
            SetEnergy(energy, smoothness);
            SetStress(stress, smoothness);
        }

        public void SetEnergy(float energy, float smoothness = 0.0f)
        {
            energyTarget = Math.Max(-1.0f, Math.Min(1.0f, energy));
            energyInterpolationSpeed = (smoothness == 0.0f) ? 1.0f : 1.0f / (smoothness * smoothness);

            if (smoothness == 0.0f & Energy != energy) Energy = energyTarget;
        }

        public void SetStress(float stress, float smoothness = 0.0f)
        {
            stressTarget = Math.Max(-1.0f, Math.Min(1.0f, stress));
            stressInterpolationSpeed = (smoothness == 0.0f) ? 1.0f : 1.0f / (smoothness * smoothness);

            if (smoothness == 0.0f & Stress != stress) Stress = stressTarget;
        }
    }

    public enum Mood { Neutral, Happy, Tender, Exciting, Sad, Depressed, Angry, Custom }
}