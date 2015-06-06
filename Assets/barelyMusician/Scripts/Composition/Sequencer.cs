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
    [AddComponentMenu("BarelyAPI/Sequencer")]
    public class Sequencer : MonoBehaviour
    {
        // Event dispatcher
        public delegate void SequencerEvent(Sequencer sequencer);
        event SequencerEvent OnNextSection, OnNextBar, OnNextBeat, OnNextPulse;

        // Beats per minute
        public int Tempo = 120;

        // Bars per section
        public int BarCount = 4;

        // Beats per bar
        public int BeatCount = 4;

        // Clock frequency per bar
        public int PulseCount = 32;

        // Note type (quarter, eigth etc.)
        public NoteType NoteType = NoteType.QUARTER_NOTE;

        // Current state
        int currentSection;
        public int CurrentSection
        {
            get { return currentSection; }
            set { currentSection = value; }
        }

        int currentBar;
        public int CurrentBar
        {
            get { return currentBar; }
            set { currentBar = value % BarCount; }
        }

        int currentBeat;
        public int CurrentBeat
        {
            get { return currentBeat; }
            set { currentBeat = value % BeatCount; }
        }

        int currentPulse;
        public int CurrentPulse
        {
            get { return currentPulse; }
            set { currentPulse = value % BarLength; }
        }

        // Section length (in pulses)
        public int SectionLength
        {
            get { return BarCount * BarLength; }
        }

        // Bar length (in pulses)
        public int BarLength
        {
            get { return BeatCount * BeatLength; }
        }

        // Beat length (in pulses)
        public int BeatLength
        {
            get { return PulseCount / (int)NoteType; }
        }

        public int MinuteToSections(float minutes)
        {
            return Mathf.RoundToInt((minutes * Tempo * (int)NoteType / 4.0f) /
                (BarCount * BeatCount));
        }

        float pulseInterval
        {
            get { return 240.0f * AudioProperties.SampleRate / PulseCount / Tempo; }
        }

        AudioSource audioSource;
        float phasor;

        void Awake()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.spatialBlend = 0.0f;

            Stop();
        }

        public void Play()
        {
            audioSource.Play();
        }

        public void Pause()
        {
            audioSource.Pause();
        }

        public void Stop()
        {
            audioSource.Stop();

            currentSection = -1;
            currentBar = -1;
            currentBeat = -1;
            currentPulse = -1;

            phasor = pulseInterval;
        }

        // Event registration
        public void AddSectionListener(SequencerEvent sectionEvent)
        {
            OnNextSection += sectionEvent;
        }

        public void AddBarListener(SequencerEvent barEvent)
        {
            OnNextBar += barEvent;
        }

        public void AddBeatListener(SequencerEvent beatEvent)
        {
            OnNextBeat += beatEvent;
        }

        public void AddPulseListener(SequencerEvent pulseEvent)
        {
            OnNextPulse += pulseEvent;
        }

        public void RemoveSectionListener(SequencerEvent sectionEvent)
        {
            OnNextSection -= sectionEvent;
        }

        public void RemoveBarListener(SequencerEvent barEvent)
        {
            OnNextBar -= barEvent;
        }

        public void RemoveBeatListener(SequencerEvent beatEvent)
        {
            OnNextBeat -= beatEvent;
        }

        public void RemovePulseListener(SequencerEvent pulseEvent)
        {
            OnNextPulse -= pulseEvent;
        }

        // Audio callback
        void OnAudioFilterRead(float[] data, int channels)
        {
            for (int i = 0; i < data.Length; i += channels)
            {
                if (phasor++ >= pulseInterval)
                {
                    CurrentPulse++;

                    if (CurrentPulse % BeatLength == 0)
                    {
                        CurrentBeat++;

                        if (CurrentBeat == 0)
                        {
                            CurrentBar++;

                            if (CurrentBar == 0)
                            {
                                CurrentSection++;

                                triggerNextSection();
                            }

                            triggerNextBar();
                        }

                        triggerNextBeat();
                    }

                    triggerNextPulse();

                    phasor -= pulseInterval;
                }
            }
        }

        // Event callback functions
        void triggerNextSection()
        {
            if (OnNextSection != null)
            {
                OnNextSection(this);
            }
        }

        void triggerNextBar()
        {
            if (OnNextBar != null)
            {
                OnNextBar(this);
            }
        }

        void triggerNextBeat()
        {
            if (OnNextBeat != null)
            {
                OnNextBeat(this);
            }
        }

        void triggerNextPulse()
        {
            if (OnNextPulse != null)
            {
                OnNextPulse(this);
            }
        }
    }

    public enum NoteType
    {
        WHOLE_NOTE = 1, HALF_NOTE = 2, QUARTER_NOTE = 4, EIGHTH_NOTE = 8, SIXTEENTH_NOTE = 16
    }
}