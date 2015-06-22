// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BarelyAPI {

[AddComponentMenu("BarelyAPI/Ensemble")]
[RequireComponent(typeof(Conductor))]
[RequireComponent(typeof(Sequencer))]
public class Ensemble : MonoBehaviour {
  // Master volume
  [SerializeField]
  [Range(0.0f, 1.0f)]
  float
    masterVolume = 1.0f;

  public float MasterVolume {
    get { return masterVolume; }
    set { masterVolume = value; }
  }

  // Play on awake
  [SerializeField]
  bool
    playOnAwake;

  public bool PlayOnAwake {
    get { return playOnAwake; }
    set { playOnAwake = value; }
  }

  Performer[] performers;
  MacroGenerator macro;
  MesoGenerator meso;
  Conductor conductor;
  Sequencer sequencer;
  SectionType currentSection;

  void Awake () {
    AudioProperties.Initialize();

    conductor = GetComponent<Conductor>();

    sequencer = GetComponent<Sequencer>();
    sequencer.OnNextSection += OnNextSection;
    sequencer.OnNextBar += OnNextBar;
    sequencer.OnNextBeat += OnNextBeat;
    sequencer.OnNextPulse += OnNextPulse;

    currentSection = SectionType.None;

    macro = new MacroGenerator(32, true);
    meso = new MesoGenerator(sequencer);

    // TODO(anokta): Move these outside?
    macro.GenerateSequenceCallback = delegate(ref string sequence) {
      sequence.Replace(' ', 'I');
    };
    meso.GenerateProgressionCallback = delegate(SectionType type, ref int[] progression) {
      progression.Initialize();
    };

    performers = GetComponentsInChildren<Performer>();
  }

  void Start () {
    if (playOnAwake) {
      Play();
    }
  }

  public void Play () {
    sequencer.Start();
  }

  public void Pause () {
    sequencer.Pause();
  }

  public void Stop () {
    sequencer.Stop();

    foreach (Performer performer in performers) {
      performer.Reset();
    }

    currentSection = SectionType.None;

    macro.Restart();
    meso.Restart();
  }

  void OnNextSection (Sequencer sequencer) {
    currentSection = macro.GetSection(sequencer.CurrentSection);
  }

  void OnNextBar (Sequencer sequencer) {
    if (currentSection != SectionType.End) {
      foreach (Performer performer in performers) {
        performer.GenerateBar(currentSection, sequencer.CurrentBar, 
                              meso.GetHarmonic(currentSection, sequencer.CurrentBar));
      }
    }
  }

  void OnNextBeat (Sequencer sequencer) {
    foreach (Performer performer in performers) {
      performer.AddBeat(sequencer, conductor);
    }
  }

  void OnNextPulse (Sequencer sequencer) {
    int bar = sequencer.CurrentSection * sequencer.barCount + sequencer.CurrentBar;

    foreach (Performer performer in performers) {
      performer.Play(bar, sequencer.CurrentPulse);
    }
  }
}
  
} // namespace BarelyAPI