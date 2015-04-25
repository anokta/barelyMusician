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
    public class DefaultMacroGenerator : MacroGenerator
    {
        public DefaultMacroGenerator(int length, bool looping = false)
            : base(length, looping)
        {
        }

        protected override void generateSequence(int length)
        {
            sectionSequence += (char)SectionType.INTRO;
            sectionSequence += (char)SectionType.VERSE;
            sectionSequence += (char)SectionType.VERSE;
            sectionSequence += (char)SectionType.VERSE;
            sectionSequence += (char)SectionType.PRE_CHORUS;
            sectionSequence += (char)SectionType.CHORUS;
            sectionSequence += (char)SectionType.CHORUS;
            sectionSequence += (char)SectionType.VERSE;
            sectionSequence += (char)SectionType.PRE_CHORUS;
            sectionSequence += (char)SectionType.CHORUS;
            sectionSequence += (char)SectionType.BRIDGE;
            sectionSequence += (char)SectionType.PRE_CHORUS;
            sectionSequence += (char)SectionType.CHORUS;
            sectionSequence += (char)SectionType.CHORUS;
            sectionSequence += (char)SectionType.OUTRO;
        }
    }
}