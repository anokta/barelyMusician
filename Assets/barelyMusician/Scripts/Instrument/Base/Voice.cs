// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {

public class Voice {
  // Output level
  float gain;

  public float Gain {
    set { gain = value; }
  }

  // Frequency
  public float Pitch {
    get { return ugen.Frequency; }
    set { ugen.Frequency = value; }
  }

  // Envelope
  Envelope envelope;

  public Envelope Envelope {
    get { return envelope; }
  }

  // Sound generator
  UGen ugen;

  public UGen Ugen {
    get { return ugen; }
  }

  public Voice(UGen soundGenerator, Envelope soundEnvelope) {
    ugen = soundGenerator;
    envelope = soundEnvelope;
  }

  public void Start () {
    ugen.Reset();

    envelope.Start();
  }

  public void Stop () {
    envelope.Stop();
  }

  public void StopImmediately () {
    envelope.Reset();

    ugen.Reset();
  }

  public float ProcessNext () {
    return gain * envelope.Next() * ugen.Next();
  }
}

} // namespace BarelyAPI