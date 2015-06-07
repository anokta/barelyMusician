// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace BarelyAPI
{
    [AddComponentMenu("BarelyAPI/Conductor")]
    public class Conductor : MonoBehaviour
    {
        // Key note
        [SerializeField]
        float fundamentalKey = (float)NoteIndex.C4;
        public float Key
        {
            get { return fundamentalKey; }
            set { fundamentalKey = value; }
        }

        // Tempo (BPM)
        [SerializeField]
        int initialTempo = 120;
        public int Tempo
        {
            get { return initialTempo; }
            set
            {
                initialTempo = value;
                sequencer.Tempo = (int)(initialTempo * TempoMultiplier);
            }
        }

        public Mood Mood;

        // Arousal (Passive - Active)
        [SerializeField]
        [Range(0.0f, 1.0f)]
        float energy = 0.5f;
        float energyTarget, energyInterpolationSpeed;
        public float Energy
        {
            get { return energy; }
            set
            {
                energy = value;

                SetParameters(energy, stress);
                sequencer.Tempo = (int)(initialTempo * TempoMultiplier);
            }
        }
        // Valence (Happy - Sad) 
        [SerializeField]
        [Range(0.0f, 1.0f)]
        float stress = 0.5f;
        float stressTarget, stressInterpolationSpeed;
        public float Stress
        {
            get { return stress; }
            set
            {
                stress = value;

                SetParameters(energy, stress);
            }
        }

        // Tempo
        float tempoMult;
        public float TempoMultiplier
        {
            get { return tempoMult; }
            set { tempoMult = 0.85f + 0.3f * value; }
        }

        // Musical mode
        ModeGenerator mode;
        public float Mode
        {
            set { mode.SetMode(value); }
        }

        // Articulation
        float articulationMult;
        public float ArticulationMultiplier
        {
            get { return articulationMult; }
            set { articulationMult = 0.25f + 1.75f * value; }
        }

        // Loudness
        float loudnessMult;
        public float LoudnessMultiplier
        {
            get { return loudnessMult; }
            set { loudnessMult = 0.4f + 0.6f * value; }
        }

        // Articulation variance
        float articulationVariance;
        public float ArticulationVariance
        {
            get { return articulationVariance; }
            set { articulationVariance = 0.15f * value; }
        }

        // Loudness Variance
        float loudnessVariance;
        public float LoudnessVariance
        {
            get { return loudnessVariance; }
            set { loudnessVariance = 0.25f * value; }
        }

        // Harmonic pitch curve
        float harmonicCurve;
        public float HarmonicCurve
        {
            get { return harmonicCurve; }
            set { harmonicCurve = 2.0f * value - 1.0f; }
        }

        // Note pitch height
        float pitchHeight;
        public float PitchHeight
        {
            get { return pitchHeight; }
            set { pitchHeight = 3.0f * value - 2.0f; }
        }

        Sequencer sequencer;

        void Awake()
        {
            sequencer = GetComponent<Sequencer>();

            mode = new ModeGenerator();
            mode.GenerateScaleCallback = delegate(float stressValue)
            {
                if (stressValue < 0.25f)
                {
                    mode.SetScale(MusicalScale.Major, MusicalMode.Ionian);
                }
                else if (stressValue < 0.5f)
                {
                    mode.SetScale(MusicalScale.NaturalMinor, MusicalMode.Ionian);
                }
                else
                {
                    mode.SetScale(MusicalScale.HarmonicMinor, MusicalMode.Ionian);
                }
            };

            Energy = energyTarget = energy;
            Stress = stressTarget = stress;
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

        void SetParameters(float energy, float stress)
        {
            TempoMultiplier = energy;
            ArticulationMultiplier = 1.0f - energy;
            LoudnessMultiplier = energy;
            ArticulationVariance = energy;

            //harmonicComplexity = stress;
            Mode = stress;

            LoudnessVariance = (energy + stress) / 2.0f;
            PitchHeight = energy * 0.25f + (1.0f - stress) * 0.75f;
            HarmonicCurve = (stress > 0.5f) ? (0.75f * (1.0f - stress) + 0.25f * (1.0f - energy)) : 1.0f;
        }

        public NoteMeta TransformNote(NoteMeta meta)
        {
            float index = getNote(
                (Mathf.RoundToInt(harmonicCurve) != 0 ? Mathf.RoundToInt(harmonicCurve) * meta.Index : meta.Index) +
                Mathf.RoundToInt(pitchHeight) / 2 * mode.ScaleLength);
            float offset = meta.Offset;
            float duration = Mathf.Max(0.0f, RandomNumber.NextNormal(
                meta.Duration * articulationMult, meta.Duration * articulationMult * articulationVariance));
            float loudness = Mathf.Max(0.0f, Mathf.Min(1.0f, RandomNumber.NextNormal(
                meta.Loudness * loudnessMult, meta.Loudness * loudnessMult * loudnessVariance)));

            return new NoteMeta(index, offset, duration, loudness);
        }

        float getNote(float index)
        {
            return fundamentalKey + mode.GetNoteOffset(index);
        }
    }

    public enum Mood { Neutral, Happy, Tender, Exciting, Sad, Depressed, Angry, Custom }
}