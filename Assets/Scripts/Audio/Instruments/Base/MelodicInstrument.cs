﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class MelodicInstrument : Instrument
{
    [SerializeField]
    [Range(1, 16)]
    public int voiceCount = 1;

    List<int> activeList;

    protected override void Awake()
    {
        base.Awake();

        activeList = new List<int>();
    }

    // TODO: Optimization + refactoring needed!
    public override void AddNote(Note note)
    {
        if (activeList.Count < voiceCount)
        {
            // is free?
            for (int i = 0; i < voiceCount; ++i)
            {
                if (audibles[i].IsFree())
                {
                    activeList.Add(i);
                    audibles[i].Pitch = note.Pitch;
                    audibles[i].NoteOn();

                    return;
                }
            }
            // or is noteoff?
            for (int i = 0; i < voiceCount; ++i)
            {
                if (activeList.IndexOf(i) == -1)
                {
                    activeList.Add(i);
                    audibles[i].Pitch = note.Pitch;
                    audibles[i].NoteOn();

                    return;
                }
            }
        }

        // otherwise, replace
        activeList.Add(activeList[0]);
        activeList.RemoveAt(0);

        int last = activeList[activeList.Count - 1];
        audibles[last].Pitch = note.Pitch;
        audibles[last].NoteOn();
    }

    public override void RemoveNote(Note note)
    {
        for (int i = 0; i < activeList.Count; ++i)
        {
            if (audibles[activeList[i]].Pitch == note.Pitch)
            {
                audibles[activeList[i]].NoteOff();
                activeList.RemoveAt(i);
            }
        }
    }

    public override void RemoveAllNotes()
    {
        activeList.Clear();

        base.RemoveAllNotes();
    }
}