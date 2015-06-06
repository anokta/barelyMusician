// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;

namespace BarelyAPI
{
    public class Envelope : UGen
    {
        // Attack (ms).
        float attack;
        public float Attack
        {
            get { return 1.0f / attack; }
            set { attack = 1.0f / value; }
        }

        // Decay (ms).
        float decay;
        public float Decay
        {
            get { return 1.0f / decay; }
            set { decay = 1.0f / value; }
        }

        // Sustain (0. - 1.).
        float sustain;
        public float Sustain
        {
            get { return sustain; }
            set { sustain = value; }
        }

        // Release (ms).
        float release;
        public float Release
        {
            get { return 1.0f / release; }
            set { release = 1.0f / value; }
        }
        float releaseOutput;

        // Envelope state.
        enum EnvelopeState { ATTACK, DECAY, SUSTAIN, RELEASE, OFF };
        EnvelopeState state;
        EnvelopeState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == EnvelopeState.RELEASE)
                    releaseOutput = output;

                phase = 0.0f;
                state = value;
            }
        }

        public Envelope(float attack, float decay, float sustain, float release)
        {
            Attack = attack;
            Decay = decay;
            Sustain = sustain;
            Release = release;
        }

        public override void Reset()
        {
            base.Reset();

            State = EnvelopeState.OFF;
        }

        public override float Next()
        {
            switch (state)
            {
                case EnvelopeState.OFF:
                    break;

                case EnvelopeState.ATTACK:
                    phase += AudioProperties.Interval * attack;
                    output = Mathf.Lerp(0.0f, 1.0f, phase);
                    if (phase >= 1.0f) State = EnvelopeState.DECAY;
                    break;

                case EnvelopeState.DECAY:
                    phase += AudioProperties.Interval * decay;
                    output = Mathf.Lerp(1.0f, sustain, phase);
                    if (phase >= 1.0f) State = EnvelopeState.SUSTAIN;
                    break;

                case EnvelopeState.SUSTAIN:
                    output = sustain;
                    break;

                case EnvelopeState.RELEASE:
                    phase += AudioProperties.Interval * release;
                    output = Mathf.Lerp(releaseOutput, 0.0f, phase);
                    if (phase >= 1.0) State = EnvelopeState.OFF;
                    break;
            }

            return output;
        }

        public void Start()
        {
            State = EnvelopeState.ATTACK;
        }

        public void Stop()
        {
            if (State != EnvelopeState.OFF)
                State = EnvelopeState.RELEASE;
        }
    }
}