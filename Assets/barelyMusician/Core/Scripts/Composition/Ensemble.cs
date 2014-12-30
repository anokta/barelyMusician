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
    public class Ensemble
    {
        Dictionary<string, Performer> performers;
        public int PerformersCount
        {
            get { if (performers != null) return performers.Count; return 0; }
        }

        MacroGenerator macro;
        public int SongDurationInSections
        {
            get { return macro.SequenceLength; }
            set { macro.SequenceLength = value; }
        }

        SectionType currentSection;
        public SectionType CurrentSection
        {
            get { return currentSection; }
            set { currentSection = value; }
        }

        MesoGenerator meso;

        Conductor conductor;

        public Ensemble(MacroGenerator sequenceGenerator, MesoGenerator sectionGenerator, Conductor conductor)
        {
            performers = new Dictionary<string, Performer>();

            macro = sequenceGenerator;
            meso = sectionGenerator;

            currentSection = SectionType.NONE;

            this.conductor = conductor;
        }

        public void Register(Sequencer sequencer)
        {
            sequencer.AddSectionListener(OnNextSection);
            sequencer.AddBarListener(OnNextBar);
            sequencer.AddBeatListener(OnNextBeat);
            sequencer.AddPulseListener(OnNextPulse);
        }

        public void AddPerformer(string name, Performer performer)
        {
            performers.Add(name, performer);
        }

        public void RemovePerfomer(string name)
        {
            performers.Remove(name);
        }

        public Performer GetPerformer(string name)
        {
            Performer performer = null;
            if (performers.TryGetValue(name, out performer))
            {
                return performer;
            }

            return null;
        }

        public void Stop()
        {
            foreach (Performer performer in performers.Values)
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

        public bool IsPerformerActive(string name)
        {
            Performer performer = null;
            if (performers.TryGetValue(name, out performer))
            {
                return performer.Active;
            }

            return false;
        }

        public void TogglePeformer(string name, bool active)
        {
            Performer performer = null;
            if (performers.TryGetValue(name, out performer))
            {
                performer.Active = active;
            }
        }

        void OnNextSection(Sequencer sequencer)
        {
            currentSection = macro.GetSection(sequencer.CurrentSection);
        }

        void OnNextBar(Sequencer sequencer)
        {
            if (currentSection != SectionType.END)
            {
                foreach (Performer performer in performers.Values)
                {
                    performer.GenerateBar(currentSection, sequencer.CurrentBar, meso.GetHarmonic(currentSection, sequencer.CurrentBar));
                }
            }
        }

        void OnNextBeat(Sequencer sequencer)
        {
            foreach (Performer performer in performers.Values)
            {
                performer.AddBeat(sequencer, conductor);
            }
        }

        void OnNextPulse(Sequencer sequencer)
        {
            int bar = sequencer.CurrentSection * sequencer.BarCount + sequencer.CurrentBar;

            foreach (Performer performer in performers.Values)
            {
                performer.Play(bar, sequencer.CurrentPulse);
            }
        }
    }
}