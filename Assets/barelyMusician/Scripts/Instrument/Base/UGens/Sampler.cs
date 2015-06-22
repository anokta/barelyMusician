// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;

namespace BarelyAPI {

public class Sampler : UGen {
  // Sample raw data.
  float[] sampleData;
  float samplingRatio;

  public AudioClip Sample {
    set {
      if (value == null) {
        sampleData = new float[1];
        samplingRatio = 0.0f;

        return;
      }

      sampleData = new float[value.samples];
      value.GetData(sampleData, 0);

      samplingRatio = value.frequency / AudioProperties.SampleRate;
    }
  }

  public int SampleLength {
    get { return sampleData.Length; }
  }

  float rootFrequency;

  public float RootFrequency {
    get { return rootFrequency; }
    set { rootFrequency = value; }
  }

  // Should loop?
  bool loop;

  public bool Loop {
    get { return loop; }
    set { loop = value; }
  }

  public Sampler(AudioClip sample, bool loop = false, float rootFrequency = 440.0f) {
    Sample = sample;

    RootFrequency = Frequency = rootFrequency;

    Loop = loop;

    Reset();
  }

  public override float Next () {
    output = sampleData[(int)(phase)];

    phase += (frequency / rootFrequency) * samplingRatio;
    if (phase >= sampleData.Length) {
      if (loop) {
        phase -= sampleData.Length;
      } else {
        return 0.0f;
      }
    }

    return output;
  }
}

} // namespace BarelyAPI