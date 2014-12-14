// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using BarelyAPI;

public class NoiseMaker : MonoBehaviour
{
    public Texture2D pixel;
    public int size;

    Musician musician;

    float[,] noises;

    void Start()
    {
        musician = FindObjectOfType<Musician>();
        musician.Sequencer.AddBeatListener(OnNextBeat);

        noises = new float[Screen.width / size + 1, Screen.height / size + 1];
    }

    void OnGUI()
    {
        if (musician.IsPlaying)
        {
            GUI.depth = -100;

            Color color = Color.black;

            for (int x = 0; x < noises.GetLength(0); ++x)
            {
                for (int y = 0; y < noises.GetLength(1); ++y)
                {
                    color.a = noises[x, y];
                    GUI.color = color;

                    GUI.DrawTexture(new Rect(x * size, y * size, size, size), pixel);
                }
            }
        }
    }

    void OnNextBeat(Sequencer sequencer)
    {
        for (int x = 0; x < noises.GetLength(0); ++x)
        {
            for (int y = 0; y < noises.GetLength(1); ++y)
            {
                noises[x, y] = RandomNumber.NextFloat(musician.Stress * 0.25f, musician.Stress * 0.25f + musician.Energy * 0.15f + 0.1f);
            }
        }
    }
}