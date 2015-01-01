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
    [AddComponentMenu("BarelyAPI/Performer")]
    public abstract class Performer : MonoBehaviour
    {
        Instrument instrument;

        Dictionary<SectionType, List<NoteMeta>[]> lines;
        Sequencer sequencer;

        // Score (note list per bar)
        Dictionary<int, List<Note>[]> score;
        List<NoteMeta> currentBar;


        void Awake()
        {
            sequencer = GetComponentInParent<Sequencer>();

            instrument = GetComponent<Instrument>();

            Reset();
        }

        public void Reset()
        {
            lines = new Dictionary<SectionType, List<NoteMeta>[]>();
            score = new Dictionary<int, List<Note>[]>();

            instrument.StopAllNotes();
        }

        public void GenerateBar(SectionType section, int index, int harmonic)
        {
            List<NoteMeta>[] lineSection = null;
            if (!lines.TryGetValue(section, out lineSection))
                lines[section] = lineSection = new List<NoteMeta>[sequencer.BarCount];

            if (lineSection[index] == null)
            {
                lineSection[index] = new List<NoteMeta>();
                generateLine(section, index, harmonic, ref lineSection[index]);
            }

            currentBar = lineSection[index];
        }

        protected abstract void generateLine(SectionType section, int bar, int harmonic, ref List<NoteMeta> line);

        public void AddBeat(Sequencer sequencer, Conductor conductor)
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