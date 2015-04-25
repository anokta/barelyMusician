// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI
{
    public abstract class ModeGenerator
    {
        public const int OCTAVE = 12;
        public const int SCALE_LENGTH = 7;

        static float[] majorScale = { 0, 2, 4, 5, 7, 9, 11 };
        static float[] harmonicMinorScale = { 0, 2, 3, 5, 7, 8, 11 };
        static float[] naturalMinorScale = { 0, 2, 3, 5, 7, 8, 10 };
        static Dictionary<MusicalScale, float[]> Scales = new Dictionary<MusicalScale, float[]>()
        {
            { MusicalScale.MAJOR, majorScale },
            { MusicalScale.HARMONIC_MINOR, harmonicMinorScale },
            { MusicalScale.NATURAL_MINOR, naturalMinorScale }
        };

        float[] currentScale;
        public int ScaleLength
        {
            get { return currentScale.Length; }
        }

        protected ModeGenerator(float stress = 0.0f)
        {
            GenerateScale(stress);
        }

        public float GetNoteOffset(float index)
        {
            float octaveOffset = Mathf.Floor(index / currentScale.Length);
            float scaleOffset = index - octaveOffset * currentScale.Length;
            int scaleIndex = Mathf.FloorToInt(scaleOffset);

            float noteOffset = currentScale[scaleIndex] + octaveOffset * OCTAVE;
            if (scaleOffset - scaleIndex > 0.0f)
                noteOffset += (scaleOffset - scaleIndex) * (currentScale[(scaleIndex + 1) % currentScale.Length] + ((scaleIndex + 1) / currentScale.Length) * OCTAVE - currentScale[scaleIndex]);

            return noteOffset;
        }

        protected void setScale(MusicalScale scaleType, MusicalMode modeType = MusicalMode.IONIAN)
        {
            float[] scale = Scales[scaleType];
            int offset = (int)modeType;

            currentScale = new float[scale.Length];

            for (int i = 0; i < currentScale.Length; ++i)
            {
                currentScale[i] = scale[(i + offset) % scale.Length] + ((i + offset) / currentScale.Length) * OCTAVE;
            }
        }

        public abstract void GenerateScale(float stress);
    }

    public enum MusicalScale
    {
        MAJOR = 0, HARMONIC_MINOR = 1, NATURAL_MINOR = 2
    }
    public enum MusicalMode
    {
        IONIAN = 0, DORIAN = 1, PHRYGIAN = 2, LYDIAN = 3, MIXOLYDIAN = 4, AEOLIAN = 5, LOCRIAN = 6
    };
}