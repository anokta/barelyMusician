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

public abstract class MelodicInstrument : Instrument {
  [SerializeField]
  protected int
    voiceCount;
  LinkedList<Voice> activeList, freeList;

  protected override void noteOn (Note note) {
    Voice voice;

    if (freeList.Count > 0) { // are any voices free?
      voice = freeList.First.Value;

      freeList.RemoveFirst();
      activeList.AddLast(voice);
    } else { // if not, steal the first used one
      voice = activeList.First.Value;
    }

    voice.Pitch = note.Pitch;
    voice.Gain = note.Loudness;
    voice.Start();
  }

  protected override void noteOff (Note note) {
    foreach (Voice voice in activeList) {
      if (voice.Pitch == note.Pitch) {
        voice.Stop();
        activeList.Remove(voice);
        freeList.AddLast(voice);

        break;
      }
    }
  }

  public override void StopAllNotes () {
    base.StopAllNotes();

    activeList = new LinkedList<Voice>();
    freeList = new LinkedList<Voice>(voices);
  }
}
  
} // namespace BarelyAPI