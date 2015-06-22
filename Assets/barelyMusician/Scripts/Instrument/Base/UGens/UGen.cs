// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

namespace BarelyAPI {

// Unit generator interface.
public abstract class UGen {
  // Frequency (Hz).
  protected float frequency;

  public float Frequency {
    get { return frequency; }
    set { frequency = value; }
  }

  // Internal clock.
  protected float phase;

  // Final output.
  protected float output;

  protected UGen() {
    Reset();
  }

  // Compute next sample.
  public abstract float Next ();

  // Reset the generator.
  public virtual void Reset () {
    phase = 0.0f;
    output = 0.0f;
  }
}
  
} // namespace BarelyAPI