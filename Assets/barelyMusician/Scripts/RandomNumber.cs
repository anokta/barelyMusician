// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using System;

namespace BarelyAPI {

public static class RandomNumber {
  static Random rand = new Random();

  public static float NextFloat (float min = 0.0f, float max = 1.0f) {
    return min + (max - min) * (float)(rand.NextDouble());
  }

  public static int NextInt (int min = 0, int max = 2) {
    return rand.Next(min, max);
  }

  public static float NextNormal (float mean = 0.0f, float deviation = 1.0f) {
    // using Box-Muller transform
    float standart = (float)(Math.Sqrt(-2.0 * Math.Log(rand.NextDouble())) * 
                             Math.Sin(2.0 * Math.PI * rand.NextDouble()));
    return mean + deviation * standart;
  }
}
  
} // namespace BarelyAPI