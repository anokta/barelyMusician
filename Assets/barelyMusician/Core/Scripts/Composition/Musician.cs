// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI
{
    [AddComponentMenu("BarelyAPI/Musician")]
    [RequireComponent(typeof(Conductor))]
    [RequireComponent(typeof(Sequencer))]
    public class Musician : MonoBehaviour
    {
        // Master volume
        [SerializeField]
        float masterVolume = 1.0f;
        public float MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = value; }
        }


        [SerializeField]
        bool playOnAwake;
        public bool PlayOnAwake
        {
            get { return playOnAwake; }
            set { playOnAwake = value; }
        }

        // Tempo (BPM)
        [SerializeField]
        int initialTempo = 120;
        public int Tempo
        {
            get { return initialTempo; }
            set
            {
                initialTempo = value;
                sequencer.Tempo = (int)(initialTempo * conductor.TempoMultiplier);
            }
        }

        public Ensemble ensemble;

        Conductor conductor;
        Sequencer sequencer;

        void Awake()
        {
            conductor = GetComponent<Conductor>();
            sequencer = GetComponent<Sequencer>();

            ensemble.Register(sequencer);

            if (playOnAwake)
                Play();
        }

        public void Play()
        {
            sequencer.Start();
        }

        public void Pause()
        {
            sequencer.Pause();
        }

        public void Stop()
        {
            ensemble.Reset();
            sequencer.Stop();
        }

    }
}