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
    public static class AudioProperties
    {
        // System sampling rate.
        private static int sampleRate;
        public static int SampleRate
        {
            get { return sampleRate; }
        }

        // System sampling interval (1 / SampleRate).
        private static float interval;
        public static float Interval
        {
            get { return interval; }
        }

        static AudioProperties()
        {
            AudioConfiguration config = AudioSettings.GetConfiguration();
            sampleRate = config.sampleRate;
            interval = 1.0f / sampleRate;
        }
    }
}