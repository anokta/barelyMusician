// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;

namespace BarelyAPI {

public class Envelope : UGen {
  // Attack (ms).
  float attack;

  public float Attack {
    get { return 1.0f / attack; }
    set { attack = 1.0f / value; }
  }

  // Decay (ms).
  float decay;

  public float Decay {
    get { return 1.0f / decay; }
    set { decay = 1.0f / value; }
  }

  // Sustain (0. - 1.).
  float sustain;

  public float Sustain {
    get { return sustain; }
    set { sustain = value; }
  }

  // Release (ms).
  float release;

  public float Release {
    get { return 1.0f / release; }
    set { release = 1.0f / value; }
  }

  float releaseOutput;

  // Envelope state.
  enum EnvelopeState {
    Attack,
    Decay,
    Sustain,
    Release,
    Off }
  ;
  EnvelopeState state;

  EnvelopeState State {
    get {
      return state;
    }
    set {
      if (value == EnvelopeState.Release) {
        releaseOutput = output;
      }

      phase = 0.0f;
      state = value;
    }
  }

  public Envelope(float attack, float decay, float sustain, float release) {
    Attack = attack;
    Decay = decay;
    Sustain = sustain;
    Release = release;
  }

  public override void Reset () {
    base.Reset();

    State = EnvelopeState.Off;
  }

  public override float Next () {
    switch (state) {
    case EnvelopeState.Off:
      break;

    case EnvelopeState.Attack:
      phase += AudioProperties.Interval * attack;
      output = Mathf.Lerp(0.0f, 1.0f, phase);
      if (phase >= 1.0f) {
        State = EnvelopeState.Decay;
      }
      break;

    case EnvelopeState.Decay:
      phase += AudioProperties.Interval * decay;
      output = Mathf.Lerp(1.0f, sustain, phase);
      if (phase >= 1.0f) {
        State = EnvelopeState.Sustain;
      }
      break;

    case EnvelopeState.Sustain:
      output = sustain;
      break;

    case EnvelopeState.Release:
      phase += AudioProperties.Interval * release;
      output = Mathf.Lerp(releaseOutput, 0.0f, phase);
      if (phase >= 1.0) {
        State = EnvelopeState.Off;
      }
      break;
    }

    return output;
  }

  public void Start () {
    State = EnvelopeState.Attack;
  }

  public void Stop () {
    if (State != EnvelopeState.Off) {
      State = EnvelopeState.Release;
    }
  }
}
  
} // namespace BarelyAPI