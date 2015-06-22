// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI {

public class PercussiveInstrument : Instrument {
  [SerializeField]
  protected AudioClip[]
    samples;
  [SerializeField]
  protected bool
    sustained;

  public bool Sustained {
    get { return sustained; }
    set {
      sustained = value;

      foreach (Voice voice in voices) {
        voice.Envelope.Release = sustained ? 0.0f : ((Sampler)voice.Ugen).SampleLength;
      }
    }
  }

  [SerializeField]
  protected NoteIndex
    rootNote;

  protected override void Awake () {
    base.Awake();

    for (int i = 0; i < samples.Length; ++i) {
      voices.Add(new Voice(
          new Sampler(samples[i], false, new Note((int)rootNote).Pitch), 
          new Envelope(Instrument.MinOnset, 0.0f, 1.0f, (sustained || samples[i] == null) ? 
                       0.0f : (samples[i].length / samples[i].channels))));
    }
  }

  // TODO(anokta): Note structure should be restructured!
  protected override void noteOn (Note note) {
    int index = (int)(note.Index - (int)rootNote) / 12;
    if (index >= 0 && index < voices.Count) {
      voices[index].Gain = note.Loudness;
      voices[index].Start();
    }
  }

  protected override void noteOff (Note note) {
    if (Sustained) {
      int index = (int)(note.Index - (int)rootNote);
      if (index >= 0 && index < voices.Count) {
        voices[index].Stop();
      }
    }
  }
    
  public enum DRUM_KIT {
    Kick = 0,
    Snare = 1,
    Hihat = 2,
    Cymbal = 3 
  }
}

} // namespace BarelyAPI