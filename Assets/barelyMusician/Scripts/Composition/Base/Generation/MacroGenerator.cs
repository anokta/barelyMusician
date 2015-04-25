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
    public abstract class MacroGenerator
    {
        protected string sectionSequence;
        public int SequenceLength
        {
            get { return sectionSequence.Length; }
            set { targetLength = value; }
        }

        int targetLength;
        bool loop;

        public MacroGenerator(int length, bool looping = false)
        {
            targetLength = length;
            loop = looping;

            Restart();
        }

        public SectionType GetSection(int index)
        {
            if (sectionSequence.Length == 0)
            {
                generateSequence(targetLength);

                // Log the sequence
                //Debug.Log("Sequence: " + sectionSequence);
            }

            if (index >= sectionSequence.Length)
            {
                if (loop) index %= sectionSequence.Length;
                else return SectionType.END;
            }

            return (SectionType)sectionSequence[index];
        }

        public void Restart()
        {
            sectionSequence = "";
        }

        protected abstract void generateSequence(int length);
    }

    public enum SectionType { INTRO = 'I', VERSE = 'V', PRE_CHORUS = 'P', CHORUS = 'C', BRIDGE = 'B', OUTRO = 'O', END = '.', NONE = ' ' } 
}