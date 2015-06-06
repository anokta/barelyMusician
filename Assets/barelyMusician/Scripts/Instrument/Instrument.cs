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
    public abstract class Instrument : MonoBehaviour
    {
        public static float MIN_ONSET = 0.01f;

        // Instrument Voices
        protected List<Voice> voices;

        // Envelope properties
        [SerializeField]
        protected float attack, decay, sustain, release;
        public float Attack
        {
            get { return attack; }
            set
            {
                attack = Mathf.Max(MIN_ONSET, value);

                if (voices != null)
                    foreach (Voice voice in voices)
                    {
                        voice.Envelope.Attack = attack;
                    }
            }
        }
        public float Decay
        {
            get { return decay; }
            set
            {
                decay = value;

                if (voices != null)
                    foreach (Voice voice in voices)
                    {
                        voice.Envelope.Decay = decay;
                    }
            }
        }
        public float Sustain
        {
            get { return sustain; }
            set
            {
                sustain = value;

                if (voices != null)
                    foreach (Voice voice in voices)
                    {
                        voice.Envelope.Sustain = sustain;
                    }
            }
        }
        public float Release
        {
            get { return release; }
            set
            {
                release = value;

                if (voices != null)
                    foreach (Voice voice in voices)
                    {
                        voice.Envelope.Release = release;
                    }
            }
        }

        [SerializeField]
        protected float volume;
        public float Volume
        {
            get { return volume; }
            set { volume = value; }
        }

        protected AudioSource audioSource;

        protected virtual void Awake()
        {
            voices = new List<Voice>();

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.hideFlags = HideFlags.HideInInspector;
            audioSource.spatialBlend = 0.0f;
            audioSource.Play();
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            float output;

            for (int i = 0; i < data.Length; i += channels)
            {
                output = 0.0f;

                foreach (Voice voice in voices)
                {
                    output += voice.ProcessNext();
                }
                data[i] = Mathf.Clamp(output * volume, -1.0f, 1.0f);

                // If stereo, copy the mono data to each channel
                if (channels == 2) data[i + 1] = data[i];
            }
        }

        public virtual void PlayNote(Note note)
        {
            if (note.IsNoteOn)
                noteOn(note);
            else
                noteOff(note);
        }

        public virtual void StopAllNotes()
        {
            foreach (Voice voice in voices)
            {
                voice.StopImmediately();
            }
        }

        protected abstract void noteOn(Note note);
        protected abstract void noteOff(Note note);
    }
}