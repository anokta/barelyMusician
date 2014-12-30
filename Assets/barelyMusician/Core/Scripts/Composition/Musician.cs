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
        public Ensemble ensemble;
    
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

        public Mood Mood;

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
        public bool IsPlaying
        {
            get { return sequencer.audio.isPlaying; }
        }


        [SerializeField]
        bool playOnAwake;
        public bool PlayOnAwake
        {
            get { return playOnAwake; }
            set { playOnAwake = value; }
        }
        
        Conductor conductor;

        void Awake()
        {
            Init();

            sequencer = GetComponent<Sequencer>();

            Energy = energyTarget = energy;
            Stress = stressTarget = stress;

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

        public void Init()
        {
            //if (sequencer == null) sequencer = new Sequencer(initialTempo, barsPerSection, beatsPerBar);
            //if (conductor == null) conductor = new Conductor((float)rootNote);

            //if (ensemble == null)
            //{
            //    MacroGenerator macro = GeneratorFactory.CreateMacroGenerator(MacroGeneratorTypeIndex, sequencer.MinuteToSections(songDuration), true);
            //    MesoGenerator meso = GeneratorFactory.CreateMesoGenerator(MesoGeneratorTypeIndex, sequencer);

            //    ensemble = new Ensemble(macro, meso, conductor);
            //    ensemble.Register(sequencer);

            //    // performers
            //    if (PerformerNames == null) PerformerNames = new List<string>();
            //    if (Instruments == null) Instruments = new List<InstrumentMeta>();
            //    if (MicroGeneratorTypes == null) MicroGeneratorTypes = new List<int>();

            //    for (int i = 0; i < PerformerNames.Count; ++i)
            //    {
            //        string name = PerformerNames[i];

            //        Instrument instrument = InstrumentFactory.CreateInstrument(Instruments[i]);
            //        MicroGenerator micro = GeneratorFactory.CreateMicroGenerator(MicroGeneratorTypes[i], sequencer);
            //        Performer performer = new Performer(instrument, micro);
            //        performer.Active = Instruments[i].Active;
            //        ensemble.AddPerformer(name, performer);
            //    }
            //}
        }

        public void Play()
        {
            sequencer.Start();
        }

        public void Pause()
        {
            sequencer.Pause();
        }

        public void Stop()
        {
            ensemble.Reset();
            sequencer.Stop();
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