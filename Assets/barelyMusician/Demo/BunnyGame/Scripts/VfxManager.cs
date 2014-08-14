﻿using UnityEngine;
using System.Collections;
using BarelyAPI;

public class VfxManager : MonoBehaviour
{
    Musician musician;

    public Texture2D pixel;

    Color currentBackgroundColor, targetBackgroundColor;
    Color currentStrokeColor, targetStrokeColor;
    Color barColor;

    float currentBeat;
    float beatCount;

    // Use this for initialization
    void Start()
    {
        musician = FindObjectOfType<Musician>();
        musician.Sequencer.AddSectionListener(OnNextSection);
        musician.Sequencer.AddBarListener(OnNextBar);
        musician.Sequencer.AddBeatListener(OnNextBeat);
        musician.Sequencer.AddPulseListener(OnNextPulse);

        currentStrokeColor = targetStrokeColor = Color.red;
        currentBackgroundColor = targetBackgroundColor = Color.white;
        currentBeat = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBackgroundColor != targetBackgroundColor)
            currentBackgroundColor = Color.Lerp(currentBackgroundColor, targetBackgroundColor, 4.0f * Time.deltaTime);
        if (currentStrokeColor != targetStrokeColor)
            currentStrokeColor = Color.Lerp(currentStrokeColor, targetStrokeColor, 2.0f * Time.deltaTime);

        Camera.main.backgroundColor = currentBackgroundColor;
    }

    void OnGUI()
    {
        GUI.depth = 100;

        GUI.color = currentBackgroundColor;
        //GUI.DrawTexture(new Rect(0.0f, 0.0f, Screen.width, Screen.height), pixel);

        GUI.color = currentStrokeColor;
        if (currentBeat >= 0)
            GUI.DrawTexture(new Rect(Screen.width * currentBeat / beatCount, 0.0f, Screen.width / beatCount, Screen.height), pixel);
    }

    void OnNextSection(Sequencer sequencer)
    {
        targetBackgroundColor = new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
    }

    void OnNextBar(Sequencer sequencer)
    {
        barColor = new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
    }

    void OnNextBeat(Sequencer sequencer)
    {
        beatCount = sequencer.BeatCount;
        currentBeat = sequencer.CurrentBeat;

        currentStrokeColor = barColor;
        targetStrokeColor = Color.clear;
    }

    void OnNextPulse(Sequencer sequencer)
    {
        //currentBeat += 1.0f / state.BeatLength;
    }
}