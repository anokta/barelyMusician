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
    public class Performer
    {
        bool active;
        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        // Line generator
        MicroGenerator microGenerator;
        public MicroGenerator MicroGenerator
        {
            get { return microGenerator; }
            set { microGenerator = value; }
        }

        // Instrument
        Instrument instrument;
        public Instrument Instrument
        {
            get { return instrument; }
            set { instrument = value; initialOnset = instrument.Attack; }
        }

        // Score (note list per bar)
        Dictionary<int, List<Note>[]> score;
        List<NoteMeta> currentBar;


        float initialOnset;

        public Performer(Instrument instrument, MicroGenerator microGenerator)
        {
            Instrument = instrument;
            MicroGenerator = microGenerator;

            active = true;

            Reset();
        }

        public void Reset()
        {
            score = new Dictionary<int, List<Note>[]>();

            instrument.StopAllNotes();

            microGenerator.Restart();
        }

        public void GenerateBar(SectionType section, int index, int harmonic)
        {
            currentBar = microGenerator.GetLine(section, index, harmonic);
        }

        public void AddBeat(Sequencer sequencer, Conductor conductor)
        {
            if (active)
            {
                foreach (NoteMeta noteMeta in currentBar)
                {
                    if (Mathf.FloorToInt(noteMeta.Offset * sequencer.BeatCount) == sequencer.CurrentBeat)
                    {
                        NoteMeta meta = conductor.TransformNote(noteMeta);

                        float start = sequencer.CurrentSection * sequencer.BarCount + sequencer.CurrentBar + meta.Offset;
                        float end = start + meta.Duration;

                        addNote(new Note(meta.Index, meta.Loudness), start, sequencer.BarLength);
                        addNote(new Note(meta.Index, 0.0f), end, sequencer.BarLength);
                    }
                }
            }
        }

        public void Play(int bar, int pulse)
        {
            List<Note>[] currentBar;
            if (score.TryGetValue(bar, out currentBar) && currentBar[pulse] != null)
            {
                foreach (Note note in currentBar[pulse])
                {
                    instrument.PlayNote(note);
                }
            }
        }

        void addNote(Note note, float onset, int barLength)
        {
            int pulse = Mathf.RoundToInt(onset * barLength);
            int bar = pulse / barLength;
            pulse %= barLength;

            List<Note>[] currentBar = null;
            if (!score.TryGetValue(bar, out currentBar))
                score[bar] = currentBar = new List<Note>[barLength];
            if (currentBar[pulse] == null)
                currentBar[pulse] = new List<Note>();

            currentBar[pulse].Add(note);
        }
    }
}