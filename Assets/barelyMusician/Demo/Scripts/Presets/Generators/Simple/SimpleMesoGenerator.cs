﻿using UnityEngine;
using System.Collections;

namespace BarelyAPI
{
    public class SimpleMesoGenerator : MesoGenerator
    {
        public SimpleMesoGenerator(SequencerState sequencerState)
            : base(sequencerState)
        {
        }

        protected override void generateProgression(SectionType section, ref int[] progression)
        {
            switch (section)
            {
                case SectionType.INTRO:
                    progression[0] = 1;
                    progression[1] = 1;
                    progression[2] = 1;
                    progression[3] = 1;
                    break;
                case SectionType.VERSE:
                    progression[0] = 1;
                    progression[1] = 4;
                    progression[2] = 2;
                    progression[3] = 5;
                    break;
                case SectionType.PRE_CHORUS:
                    progression[0] = 1;
                    progression[1] = 4;
                    progression[2] = 5;
                    progression[3] = 8;
                    break;
                case SectionType.CHORUS:
                    progression[0] = 1;
                    progression[1] = 4;
                    progression[2] = 5;
                    progression[3] = 1;
                    break;
                case SectionType.BRIDGE:
                    progression[0] = 1;
                    progression[1] = 4;
                    progression[2] = 5;
                    progression[3] = 4;
                    break;
                case SectionType.OUTRO:
                    progression[0] = 1;
                    progression[1] = 5;
                    progression[2] = 1;
                    progression[3] = 1;
                    break;
            }
        }
    }
}