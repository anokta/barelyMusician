// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------
using UnityEngine;
using System.Collections;

namespace BarelyAPI {
  
// Step sequencer that sends signals in each section, bar, beat and pulse 
// respectively.
[AddComponentMenu("BarelyAPI/Sequencer")]
public class Sequencer : MonoBehaviour {
  // Sequencer event dispatcher.
  public delegate void SequencerEvent(Sequencer sequencer);
    
  // Sequencer callbacks per each audio event. 
  public event SequencerEvent OnNextSection, OnNextBar, OnNextBeat, OnNextPulse;
    
  // Beats per minute.
  public int tempo;
    
  // Bars per section.
  public int barCount = 4;
    
  // Beats per bar.
  public int beatCount = 4;
    
  // Clock frequency per bar.
  public int pulseCount = 32;
    
  // Note length type.
  public NoteType noteType = NoteType.QuarterNote;
    
  // Current section.
  public int CurrentSection {
    get { return currentSection; }
    set { currentSection = value; }
  }
    
  // Current bar of the section.
  public int CurrentBar {
    get { return currentBar; }
    set { currentBar = value % barCount; }
  }
    
  // Current beat of the bar.
  public int CurrentBeat {
    get { return currentBeat; }
    set { currentBeat = value % beatCount; }
  }
    
  // Current pulse of the bar.
  public int CurrentPulse {
    get { return currentPulse; }
    set { currentPulse = value % BarLength; }
  }
    
  // Section length in pulses.
  public int SectionLength {
    get { return barCount * BarLength; }
  }
    
  // Bar length in pulses.
  public int BarLength {
    get { return beatCount * BeatLength; }
  }
    
  // Beat length in pulses.
  public int BeatLength {
    get { return pulseCount / (int)noteType; }
  }
    
  private float PulseInterval {
    get { return 240.0f * AudioProperties.SampleRate / pulseCount / tempo; }
  }
    
  // Source that provides the audio callback.
  private AudioSource audioSource = null;
    
  // Current state of the sequencer.
  private int currentSection = -1;
  private int currentBar = -1;
  private int currentBeat = -1;
  private int currentPulse = -1;
    
  // Granular counter to determine the current state.
  private float phasor = 0;
    
  void Awake () {
    audioSource = gameObject.AddComponent<AudioSource>();
    audioSource.hideFlags = HideFlags.HideInInspector;
    audioSource.spatialBlend = 0.0f;
    Stop();
  }
    
  public void Start () { 
    if (audioSource.isPlaying) {
      Stop();
    }
    audioSource.Play();
  }
    
  public void Pause () {
    audioSource.Pause();
  }
    
  public void Stop () { 
    audioSource.Stop();
      
    currentSection = -1;
    currentBar = -1;
    currentBeat = -1;
    currentPulse = -1;
      
    phasor = PulseInterval;
  }
    
  void OnAudioFilterRead (float[] data, int channels) {
    for (int i = 0; i < data.Length; i += channels) {
      ++phasor;
      if (phasor >= PulseInterval) {
        ++CurrentPulse;
        if (CurrentPulse % BeatLength == 0) {
          ++CurrentBeat;
          if (CurrentBeat == 0) {
            ++CurrentBar;
            if (CurrentBar == 0) {
              ++CurrentSection;
              TriggerNextSection();
            }
            TriggerNextBar();
          }
          TriggerNextBeat();
        }
        TriggerNextPulse();
        phasor -= PulseInterval;
      }
    }
  }
    
  // Section callback function.
  void TriggerNextSection () {
    if (OnNextSection != null) {
      OnNextSection(this);
    }
  }
    
  // Bar callback function.
  void TriggerNextBar () {
    if (OnNextBar != null) {
      OnNextBar(this);
    }
  }
    
  // Beat callback function.
  void TriggerNextBeat () {
    if (OnNextBeat != null) {
      OnNextBeat(this);
    }
  }
    
  // Pulse callback function.
  void TriggerNextPulse () {  
    if (OnNextPulse != null) {
      OnNextPulse(this);
    }
  }
    
  // Common note length types.
  public enum NoteType {
    WholeNote = 1,
    HalfNote = 2,
    QuarterNote = 4,
    EightNote = 8,
    SixteenthNote = 16
  }
}
  
}  // namespace BarelyAPI