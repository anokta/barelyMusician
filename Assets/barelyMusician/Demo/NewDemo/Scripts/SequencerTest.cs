using UnityEngine;
using System.Collections;
using BarelyAPI;

public class SequencerTest : MonoBehaviour {
  Sequencer sequencer;

  // Use this for initialization
  void Start () {
    sequencer = GetComponent<Sequencer>();
    sequencer.OnNextBeat += OnNextBeat;
  }

  void OnNextBeat (Sequencer s) {
    Debug.Log(s.CurrentBeat);
  }
}
