// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

public class SamplerInstrument : MelodicInstrument {
  [SerializeField]
  protected AudioClip
    sample;
  [SerializeField]
  protected bool
    sustained;

  public bool Sustained {
    get { return sustained; }
    set {
      sustained = value;

      foreach (Voice voice in voices) {
        ((Sampler)voice.Ugen).Loop = sustained;
      }
    }
  }

  [SerializeField]
  protected NoteIndex
    rootNote;

  protected override void Awake () {
    base.Awake();

    for (int i = 0; i < voiceCount; ++i) {
      voices.Add(new Voice(new Sampler(sample, sustained, new Note((int)rootNote).Pitch),
                    new Envelope(attack, decay, sustain, release)));
    }
  }
}
  
} // namespace BarelyAPI
