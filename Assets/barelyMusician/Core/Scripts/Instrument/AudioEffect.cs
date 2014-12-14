// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace BarelyAPI
{
    public abstract class AudioEffect
    {
        protected bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        protected AudioEffect()
        {
            enabled = true;
        }

        public abstract void Apply(TimbreProperties timbreProperties);

        public abstract void ProcessBlock(ref float[] data, int channels);
    }
}