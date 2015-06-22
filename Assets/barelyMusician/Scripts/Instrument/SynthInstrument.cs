// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

public class SynthInstrument : MelodicInstrument {
  [SerializeField]
  protected OscillatorType
    oscType;

  public OscillatorType OscType {
    get { return oscType; }
    set {
      oscType = value;

      foreach (Voice voice in voices) {
        ((Oscillator)voice.Ugen).Type = oscType;
      }
    }
  }

  protected override void Awake () {
    base.Awake();

    for (int i = 0; i < voiceCount; ++i) {
      voices.Add(new Voice(new Oscillator(oscType), new Envelope(attack, decay, sustain, release)));
    }
  }
}
  
} // namespace BarelyAPI