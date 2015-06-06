// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI
{
    public class MesoGenerator
    {
        // Progression generator callback function.
        public delegate void GenerateProgression(SectionType section, ref int[] progression);

        GenerateProgression generateProgressionCallback;
        public GenerateProgression GenerateProgressionCallback
        {
            set { generateProgressionCallback = value; }
        }

        Sequencer sequencer;
        protected int ProgressionLength
        {
            get { return sequencer.BarCount; }
        }
        Dictionary<SectionType, int[]> progressions;

        public MesoGenerator(Sequencer sequencer)
        {
            this.sequencer = sequencer;
            this.progressions = new Dictionary<SectionType, int[]>();
        }

        public int GetHarmonic(SectionType section, int index)
        {
            int[] progression = null;
            if (!progressions.TryGetValue(section, out progression))
            {
                progression = progressions[section] = new int[ProgressionLength];
                generateProgressionCallback(section, ref progression);
            }

            return progression[index];
        }

        public void Restart()
        {
            progressions.Clear();
        }
    }
}