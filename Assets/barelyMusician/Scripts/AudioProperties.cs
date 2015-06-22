// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

// Main configuration class.
public static class AudioProperties {
  // System sampling rate.
  public static int SampleRate {
    get { return sampleRate; }
  }
    
  // System sampling interval (1 / sampleRate).
  public static float Interval {
    get { return interval; }
  }
    
  public static void Initialize () {
    AudioConfiguration config = AudioSettings.GetConfiguration();
    sampleRate = config.sampleRate;
    interval = 1.0f / sampleRate;
  }
    
  // System configuration.
  private static int sampleRate = 48000;
  private static float interval = 1.0f / sampleRate;
}
  
} // namespace BarelyAPI