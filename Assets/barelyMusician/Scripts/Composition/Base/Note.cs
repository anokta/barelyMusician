// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

public class Note {
  // Pitch (Hz)
  float frequency;

  public float Pitch {
    get { return frequency; }
  }

  // Index (default = A4)
  float index;

  public float Index {
    get { return index; }
    set { index = value;
      frequency = Mathf.Pow(2, index / 12) * 440.0f; }
  }

  // Loudness (0. - 1.)
  float loudness;

  public float Loudness {
    get { return loudness; }
    set { loudness = value; }
  }
  //public int Velocity
  //{
  //    get { return (int)(loudness * 127); }
  //    set { loudness = value / 127.0f; }
  //}

  public bool IsNoteOn {
    get { return loudness > 0.0f; }
  }

  public Note(float index, float loudness = 1.0f) {
    Index = index;

    Loudness = loudness;
  }
}

public enum NoteIndex {
  A4 = 0,
  ASharp4 = 1,
  B4 = 2,
  C4 = 3,
  CSharp4 = 4,
  D4 = 5,
  DSharp4 = 6,
  E4 = 7,
  F4 = 8,
  FSharp4 = 9,
  G4 = 10,
  GSharp4 = 11
}
  
} // namespace BarelyAPI