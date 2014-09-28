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
        public static int SAMPLE_RATE = AudioSettings.outputSampleRate = 44100;
        public static float INTERVAL = 1.0f / SAMPLE_RATE;
    }
}