// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

public class NoteMeta {
  public float Index;
  public float Offset;
  public float Duration;
  public float Loudness;

  public NoteMeta(float index, float offset, float duration, float loudness = 1.0f) {
    Index = index;
    Offset = offset;
    Duration = duration;
    Loudness = loudness;
  }
}

} // namespace BarelyAPI