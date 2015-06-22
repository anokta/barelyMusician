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

// TODO(anokta): Is this class necessary at all now? Maybe merge the functionality to Conductor.
public class ModeGenerator {
  // Scale generator callback function.
  public delegate void GenerateScale(float stress);

  GenerateScale generateScaleCallback;

  public GenerateScale GenerateScaleCallback {
    set { generateScaleCallback = value; }
  }

  public const int Octave = 12;

  static float[] majorScale = { 0, 2, 4, 5, 7, 9, 11 };

  static float[] harmonicMinorScale = { 0, 2, 3, 5, 7, 8, 11 };

  static float[] naturalMinorScale = { 0, 2, 3, 5, 7, 8, 10 };

  static Dictionary<MusicalScale, float[]> Scales = new Dictionary<MusicalScale, float[]>() {
      { MusicalScale.Major, majorScale },
      { MusicalScale.HarmonicMinor, harmonicMinorScale },
      { MusicalScale.NaturalMinor, naturalMinorScale } 
    };

  float[] currentScale;

  public int ScaleLength {
    get { return currentScale.Length; }
  }

  public float GetNoteOffset (float index) {
    float octaveOffset = Mathf.Floor(index / currentScale.Length);
    float scaleOffset = index - octaveOffset * currentScale.Length;
    int scaleIndex = Mathf.FloorToInt(scaleOffset);

    float noteOffset = currentScale[scaleIndex] + octaveOffset * Octave;
    if (scaleOffset - scaleIndex > 0.0f) {
      noteOffset += 
          (scaleOffset - scaleIndex) * (currentScale[(scaleIndex + 1) % currentScale.Length] +
          ((scaleIndex + 1) / currentScale.Length) * Octave - currentScale[scaleIndex]);
    }

    return noteOffset;
  }

  public void SetMode (float stress) {
    generateScaleCallback(stress);
  }

  public void SetScale (MusicalScale scaleType, MusicalMode modeType = MusicalMode.Ionian) {
    float[] scale = Scales[scaleType];
    int offset = (int)modeType;

    currentScale = new float[scale.Length];

    for (int i = 0; i < currentScale.Length; ++i) {
      currentScale[i] = 
          scale[(i + offset) % scale.Length] + ((i + offset) / currentScale.Length) * Octave;
    }
  }
}

public enum MusicalScale {
  Major = 0,
  HarmonicMinor = 1,
  NaturalMinor = 2
}

public enum MusicalMode {
  Ionian = 0,
  Dorian = 1,
  Phrygian = 2,
  Lydian = 3,
  Mixolydian = 4,
  Aeolian = 5,
  Locrian = 6
}
  
} // namespace BarelyAPI