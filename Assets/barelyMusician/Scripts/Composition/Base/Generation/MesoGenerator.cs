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
    public abstract class MesoGenerator
    {
        Sequencer sequencer;
        protected int ProgressionLength
        {
            get { return sequencer.BarCount; }
        }

        Dictionary<SectionType, int[]> progressions;
       
        protected MesoGenerator(Sequencer sequencer)
        {
            this.sequencer = sequencer;

            Restart();
        }

        public int GetHarmonic(SectionType section, int index)
        {
            int[] progression = null;
            if (!progressions.TryGetValue(section, out progression))
            {
                progression = progressions[section] = new int[ProgressionLength];
                for (int i = 0; i < progression.Length; ++i) progression[i] = 1;
                generateProgression(section, ref progression);

                // Log the progression
                //string log = section + " Progression:";
                //for (int i = 0; i < progression.Length; ++i) log += " " + progression[i];
                //Debug.Log(log);
            }

            return progression[index] - 1;
        }

        public virtual void Restart()
        {
            progressions = new Dictionary<SectionType, int[]>();
        }

        protected abstract void generateProgression(SectionType section, ref int[] progression);
    }
}