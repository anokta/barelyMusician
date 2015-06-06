// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace BarelyAPI {

  public static class AudioProperties {
    // System sampling rate.
    public static int SampleRate;
    
    // System sampling interval (1 / SampleRate).
    public static float Interval;

    static AudioProperties() {
      AudioConfiguration config = AudioSettings.GetConfiguration();
      SampleRate = config.sampleRate;
      Interval = 1.0f / SampleRate;
    }
  }

}