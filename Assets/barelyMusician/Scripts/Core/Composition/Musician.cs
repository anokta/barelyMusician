﻿using UnityEngine;
using System.Collections;

namespace BarelyAPI
{
    public class Musician : MonoBehaviour
    {
        [SerializeField]
        [Range(72, 220)]
        public int initialTempo;

        [SerializeField]
        [Range(1, 8)]
        public int barsPerSection;

        [SerializeField]
        [Range(1, 16)]
        public int beatsPerBar;

        public NoteIndex fundamentalKey;

        // Arousal (Passive - Active)
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


        #region TEST_ZONE
        public AudioClip sample;
        public AudioClip[] drumKit;

        Instrument[] instruments;
        #endregion TEST_ZONE

        AudioSource audioSource;

        Sequencer sequencer;

        Ensemble ensemble;
        Conductor conductor;

        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.panLevel = 0.0f;
            audioSource.Stop();
        }

        // Use this for initialization
        void Start()
        {
            sequencer = new Sequencer(initialTempo, barsPerSection, beatsPerBar);
            conductor = new Conductor((float)fundamentalKey, new SimpleModeGenerator());

            ensemble = new Ensemble(new SimpleMacroGenerator(true), new SimpleMesoGenerator(sequencer.State), conductor);
            ensemble.Register(sequencer);

            #region TEST_ZONE
            instruments = new Instrument[3];
            instruments[0] = new SamplerInstrument(sample, new Envelope(0.0f, 0.0f, 1.0f, 0.25f));
            instruments[1] = new SynthInstrument(OscillatorType.SAW, new Envelope(0.25f, 0.5f, 1.0f, 0.25f), -5.0f);
            instruments[2] = new PercussiveInstrument(drumKit, -4.0f);

            ensemble.AddProducer("Piano", new Producer(instruments[0], new SimpleMicroGenerator(sequencer.State)));
            ensemble.AddProducer("Synth", new Producer(instruments[1], new CA1DMicroGenerator(sequencer.State)));
            ensemble.AddProducer("Drums", new Producer(instruments[2], new DrumsMicroGenerator(sequencer.State)));
            #endregion TEST_ZONE
        }

        void Update()
        {
            if (Mathf.Abs(energy - energyTarget) < 0.01f * energyInterpolationSpeed)
                energy = energyTarget;
            else
                energy = Mathf.Lerp(energy, energyTarget, energyInterpolationSpeed);

            if (Mathf.Abs(stress - stressTarget) < 0.01f * stressInterpolationSpeed)
                stress = stressTarget;
            else
                stress = Mathf.Lerp(stress, stressTarget, stressInterpolationSpeed);
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            sequencer.Update(data.Length / channels);

            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] = ensemble.GetNextOutput();

                // If stereo, copy the mono data to each channel
                if (channels == 2) data[i + 1] = data[i];
            }
        }

        public void Play()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void Pause()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
            }
        }

        public void Stop()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            ensemble.Stop();
            sequencer.Reset();
        }

        public bool IsPlaying()
        {
            return audioSource.isPlaying;
        }

        public void SetMood(Mood moodType, float smoothness = 0.0f)
        {
            switch (moodType)
            {
                case Mood.HAPPY:
                    SetMood(0.5f, 0.0f, smoothness);
                    break;
                case Mood.TENDER:
                    SetMood(0.0f, 0.0f, smoothness);
                    break;
                case Mood.EXCITING:
                    SetMood(1.0f, 0.0f, smoothness);
                    break;
                case Mood.SAD:
                    SetMood(0.25f, 0.75f, smoothness);
                    break;
                case Mood.DEPRESSED:
                    SetMood(0.0f, 1.0f, smoothness);
                    break;
                case Mood.ANGRY:
                    SetMood(1.0f, 1.0f, smoothness);
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
            energyTarget = energy;
            energyInterpolationSpeed = (smoothness == 0.0f) ? 1.0f : Time.deltaTime / (smoothness * smoothness);
        }

        public void SetStress(float stress, float smoothness = 0.0f)
        {
            stressTarget = stress;
            stressInterpolationSpeed = (smoothness == 0.0f) ? 1.0f : Time.deltaTime / (smoothness * smoothness);
        }
    }

    public enum Mood { HAPPY, TENDER, EXCITING, SAD, DEPRESSED, ANGRY }
}