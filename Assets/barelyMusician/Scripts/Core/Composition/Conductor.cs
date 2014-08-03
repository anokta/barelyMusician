﻿using UnityEngine;
using System.Collections;
using System;

namespace BarelyAPI
{
    public class Conductor
    {
        // Key note
        float fundamentalKey;
        public float Key
        {
            get { return fundamentalKey; }
            set { fundamentalKey = value; }
        }

        // Tempo
        float tempoMult;
        public float TempoMultiplier
        {
            get { return tempoMult; }
            set { tempoMult = 0.875f + 0.25f * value; }
        }

        // Musical mode
        ModeGenerator mode;
        public float Mode
        {
            set { mode.GenerateScale(value); }
        }

        // Articulation
        float articulationMult;
        public float ArticulationMultiplier
        {
            get { return articulationMult; }
            set { articulationMult = 0.5f + value; }
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
            set { articulationVariance = 0.2f * value; }
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
            set { pitchHeight = 4.0f * value - 2.0f; }
        }

        // Instrument timbre properties
        TimbreProperties timbreProperties;
        public TimbreProperties TimbreProperties
        {
            get { return timbreProperties; }
        }

        public Conductor(float key, ModeGenerator modeGenerator)
        {
            fundamentalKey = key;

            mode = modeGenerator;

            timbreProperties = new TimbreProperties();
        }

        public void SetParameters(float energy, float stress)
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

            timbreProperties.Set(energy, stress);
        }

        public NoteMeta TransformNote(NoteMeta meta)
        {
            float index = getNote(Mathf.RoundToInt(harmonicCurve) != 0 ? Mathf.RoundToInt(harmonicCurve) * meta.Index : meta.Index + Mathf.RoundToInt(pitchHeight) / 2 * ModeGenerator.SCALE_LENGTH);
            float offset = meta.Offset;
            float duration = Mathf.Max(0.0f, RandomNumber.NextNormal(meta.Duration * articulationMult, meta.Duration * articulationMult * articulationVariance));
            float loudness = Mathf.Max(0.0f, RandomNumber.NextNormal(meta.Loudness * loudnessMult, meta.Loudness * loudnessMult * loudnessVariance));

            return new NoteMeta(index, offset, duration, loudness);
        }

        float getNote(float index)
        {
            return fundamentalKey + mode.GetNoteOffset(index);
        }
    }
}