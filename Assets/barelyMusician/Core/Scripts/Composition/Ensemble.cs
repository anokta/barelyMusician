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
    [AddComponentMenu("BarelyAPI/Ensemble")]
    public class Ensemble : MonoBehaviour
    {
        Performer[] performers;

        MacroGenerator macro;
        MesoGenerator meso;

        SectionType currentSection;

        Conductor conductor;

        void Awake()
        {
            currentSection = SectionType.NONE;

            performers = GetComponentsInChildren<Performer>();
        }

        public void Register(Sequencer sequencer)
        {
            sequencer.AddSectionListener(OnNextSection);
            sequencer.AddBarListener(OnNextBar);
            sequencer.AddBeatListener(OnNextBeat);
            sequencer.AddPulseListener(OnNextPulse);
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