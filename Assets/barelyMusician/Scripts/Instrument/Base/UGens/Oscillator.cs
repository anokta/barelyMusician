// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System;

namespace BarelyAPI {

public class Oscillator : UGen {
  // Wave type.
  OscillatorType type;

  public OscillatorType Type {
    get {
      return type;
    }
    set {
      type = value;

      switch (type) {
      case OscillatorType.Sine:
        oscFunc = sine;
        break;
      case OscillatorType.Cosine:
        oscFunc = cos;
        break;
      case OscillatorType.Saw:
        oscFunc = saw;
        break;
      case OscillatorType.Square:
        oscFunc = square;
        break;
      case OscillatorType.Pulse:
        oscFunc = pulse;
        break;
      case OscillatorType.Triangle:
        oscFunc = triangle;
        break;
      case OscillatorType.Noise:
        oscFunc = noise;
        break;
      }
    }
  }

  // Pulse duty (0. - 1.).
  float duty;

  public float PulseDuty {
    get { return duty; }
    set { duty = value; }
  }

  Func<float> oscFunc;
  static float TwoPi = 2.0f * Mathf.PI;
  static System.Random rand = new System.Random();

  public Oscillator(OscillatorType type, float duty = 0.5f) {
    Type = type;
    PulseDuty = duty;
  }

  public override float Next () {
    return oscFunc();
  }

  float sine () {
    output = Mathf.Sin(phase * TwoPi);

    if (phase >= 1.0f) {
      phase -= 1.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float cos () {
    output = Mathf.Cos(phase * TwoPi);

    if (phase >= 1.0f) {
      phase -= 1.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float saw () {
    output = phase;

    if (phase >= 1.0f) {
      phase -= 2.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float square () {
    if (phase < 0.5f) {
      output = -1.0f;
    }
    if (phase > 0.5f) {
      output = 1.0f;
    }

    if (phase >= 1.0f) {
      phase -= 1.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float pulse () {
    if (duty < 0.0f) {
      duty = 0.0f;
    }
    if (duty > 1.0f) {
      duty = 1.0f;
    }
    if (phase < duty) {
      output = -1.0f;
    }
    if (phase > duty) {
      output = 1.0f;
    }

    if (phase >= 1.0f) {
      phase -= 1.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float triangle () {
    if (phase <= 0.5f) {
      output = (phase - 0.25f) * 4.0f;
    } else {
      output = ((1.0f - phase) - 0.25f) * 4.0f;
    }

    if (phase >= 1.0f) {
      phase -= 1.0f;
    }
    phase += frequency * AudioProperties.Interval;

    return output;
  }

  float noise () {
    output = 2.0f * (float)rand.NextDouble() - 1.0f;

    return output;
  }
}

public enum OscillatorType {
  Sine,
  Cosine,
  Saw,
  Square,
  Pulse,
  Triangle,
  Noise
}
  
} // namespace BarelyAPI