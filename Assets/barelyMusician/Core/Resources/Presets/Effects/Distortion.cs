// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace BarelyAPI
{
    public class Distortion : AudioEffect
    {
        // Distortion level
        private float level, levelApplied;
        public float Level
        {
            get { return level; }
            set { level = value; }
        }

        public Distortion(float distortionLevel)
        {
            level = levelApplied = distortionLevel;
        }

        public Distortion()
            : this(4.0f)
        {
        }

        public override void Apply(TimbreProperties timbreProperties)
        {
            levelApplied = Mathf.Max(1.0f, (0.1f * timbreProperties.Brightness + 0.9f * timbreProperties.Tense) * level);
        }

        public override void ProcessBlock(ref float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                data[i] = Mathf.Clamp(data[i] * levelApplied, -1.0f, 1.0f) / levelApplied;

                if (channels == 2)
                    data[i + 1] = data[i];
            }
        }
    }
}