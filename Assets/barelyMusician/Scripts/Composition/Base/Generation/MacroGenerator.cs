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
    public class MacroGenerator
    {
        // Sequence generator callback function.
        public delegate void GenerateSequence(ref string sequence);

        GenerateSequence generateSequenceCallback;
        public GenerateSequence GenerateSequenceCallback
        {
            set { generateSequenceCallback = value; }
        }

        // Sequence of sections (musical form).
        string sectionSequence;
        public int SequenceLength
        {
            get { return sectionSequence.Length; }
        }

        // Should the sequence loop?
        bool loop;
        public bool Loop
        {
            get { return loop; }
            set { loop = value; }
        }

        public MacroGenerator(int length, bool loop = false)
        {
            sectionSequence = SectionType.None.ToString();
            sectionSequence.PadRight(length, sectionSequence[0]);
            Loop = loop;

            Restart();
        }

        // Returns the section corresponding to the |index|.
        public SectionType GetSection(int index)
        {
            if (index >= sectionSequence.Length)
            {
                if (loop)
                {
                    index %= sectionSequence.Length;
                }
                else
                {
                    return SectionType.End;
                }
            }

            SectionType currentSection = (SectionType)sectionSequence[index];
            if (currentSection == SectionType.None)
            {
                generateSequenceCallback(ref sectionSequence);
                currentSection = (SectionType)sectionSequence[index];
            }

            return currentSection;
        }

        // Flushes the generator.
        public void Restart()
        {
            int length = SequenceLength;
            sectionSequence = SectionType.None.ToString();
            sectionSequence.PadRight(length, sectionSequence[0]);
        }
    }

    public enum SectionType
    {
        Intro = 'I', Verse = 'V', PreChorus = 'P', Chorus = 'C', Bridge = 'B', Outro = 'O', End = '.', None = ' '
    }
}