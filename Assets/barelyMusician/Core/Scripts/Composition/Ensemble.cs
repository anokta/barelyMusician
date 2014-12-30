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
    public class Ensemble : MonoBehaviour
    {
        public Performer[] performers;

        MacroGenerator macro;

        SectionType currentSection;
        public SectionType CurrentSection
        {
            get { return currentSection; }
            set { currentSection = value; }
        }

        MesoGenerator meso;

        Conductor conductor;

        void Awake()
        {
            currentSection = SectionType.NONE;
        }

        public void Stop()
        {
            foreach (Performer performer in performers)
            {
                performer.Reset();
            }

            currentSection = SectionType.NONE;
        }
        
        public void Reset()
        {
            Stop();

            macro.Restart();
            meso.Restart();
        }

        void OnNextSection(Sequencer sequencer)
        {
            currentSection = macro.GetSection(sequencer.CurrentSection);
        }

        void OnNextBar(Sequencer sequencer)
        {
            if (currentSection != SectionType.END)
            {
                foreach (Performer performer in performers)
                {
                    performer.GenerateBar(currentSection, sequencer.CurrentBar, meso.GetHarmonic(currentSection, sequencer.CurrentBar));
                }
            }
        }

        void OnNextBeat(Sequencer sequencer)
        {
            foreach (Performer performer in performers)
            {
                performer.AddBeat(sequencer, conductor);
            }
        }

        void OnNextPulse(Sequencer sequencer)
        {
            int bar = sequencer.CurrentSection * sequencer.BarCount + sequencer.CurrentBar;

            foreach (Performer performer in performers)
            {
                performer.Play(bar, sequencer.CurrentPulse);
            }
        }
    }
}