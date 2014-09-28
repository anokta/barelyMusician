// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All right reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace BarelyAPI
{
    public class PerformerWindow : EditorWindow
    {
        public Musician musician;

        public string performerName;
        public int microGeneratorType;
        public InstrumentMeta instrumentMeta;
        public int editIndex;

        bool advanced, effects;
        bool sampleListFoldout = true;

        void OnEnable()
        {
            editIndex = -1;
            //ShowAsDropDown(position, new Vector2(position.width, position.height));
        }

        void OnGUI()
        {
            performerName = EditorGUILayout.TextField(new GUIContent("Name", "String identifier of the performer."), performerName);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            microGeneratorType = EditorGUILayout.Popup("Generator", microGeneratorType, GeneratorFactory.MicroGeneratorTypes);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            drawInstrument();
            
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Cancel"))
            {
                DestroyImmediate(instrumentMeta);
                Close();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("OK"))
            {
                musician.RegisterPerformer(performerName, instrumentMeta, microGeneratorType, editIndex);

                Close();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }

        void drawInstrument()
        {
            EditorGUILayout.LabelField("Instrument");
            EditorGUI.indentLevel++;
            instrumentMeta.Volume = EditorGUILayout.Slider("Volume", instrumentMeta.Volume, 0.0f, 1.0f);
            EditorGUILayout.Space();
            instrumentMeta.Type = EditorGUILayout.Popup("Type", instrumentMeta.Type, InstrumentFactory.InstrumentTypes);
            EditorGUILayout.Space();
            switch (InstrumentFactory.InstrumentTypes[instrumentMeta.Type])
            {
                case "PercussiveInstrument":
                    if (instrumentMeta.Samples == null) instrumentMeta.Samples = new AudioClip[4];

                    sampleListFoldout = EditorGUILayout.Foldout(sampleListFoldout, "Samples");
                    if (sampleListFoldout)
                    {
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < instrumentMeta.Samples.Length; ++i)
                            instrumentMeta.Samples[i] = (AudioClip)EditorGUILayout.ObjectField(((DRUM_KIT)i).ToString(), instrumentMeta.Samples[i], typeof(AudioClip), false);
                        EditorGUI.indentLevel--;
                    }

                    instrumentMeta.Sustained = EditorGUILayout.Toggle("Sustained", instrumentMeta.Sustained);

                    break;

                case "SamplerInstrument":
                    instrumentMeta.Sample = (AudioClip)EditorGUILayout.ObjectField("Sample", instrumentMeta.Sample, typeof(AudioClip), false);
                    instrumentMeta.RootIndex = (int)(NoteIndex)EditorGUILayout.EnumPopup("Root Note", (NoteIndex)instrumentMeta.RootIndex);
                    instrumentMeta.Sustained = EditorGUILayout.Toggle("Loop", instrumentMeta.Sustained);
                    EditorGUILayout.Space();

                    advanced = EditorGUILayout.Foldout(advanced, "Advanced");
                    if (advanced)
                    {
                        instrumentMeta.VoiceCount = EditorGUILayout.IntSlider("Voice Count", instrumentMeta.VoiceCount, 1, 16);

                        EditorGUILayout.BeginHorizontal();
                        instrumentMeta.Attack = EditorGUILayout.FloatField("Attack", instrumentMeta.Attack);
                        instrumentMeta.Decay = EditorGUILayout.FloatField("Decay", instrumentMeta.Decay);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        instrumentMeta.Sustain = EditorGUILayout.FloatField("Sustain", instrumentMeta.Sustain);
                        instrumentMeta.Release = EditorGUILayout.FloatField("Release", instrumentMeta.Release);
                        EditorGUILayout.EndHorizontal();
                    }
                    break;

                case "SynthInstrument":
                    instrumentMeta.OscType = (OscillatorType)EditorGUILayout.EnumPopup("Oscillator Type", instrumentMeta.OscType);

                    EditorGUILayout.Space();

                    advanced = EditorGUILayout.Foldout(advanced, "Advanced");
                    if (advanced)
                    {
                        instrumentMeta.VoiceCount = EditorGUILayout.IntSlider("Voice Count", instrumentMeta.VoiceCount, 1, 32);

                        EditorGUILayout.BeginHorizontal();
                        instrumentMeta.Attack = EditorGUILayout.FloatField("Attack", instrumentMeta.Attack);
                        instrumentMeta.Decay = EditorGUILayout.FloatField("Decay", instrumentMeta.Decay);
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        instrumentMeta.Sustain = EditorGUILayout.FloatField("Sustain", instrumentMeta.Sustain);
                        instrumentMeta.Release = EditorGUILayout.FloatField("Release", instrumentMeta.Release);
                        EditorGUILayout.EndHorizontal();
                    }
                    break;
            }
            EditorGUILayout.Space();
            drawEffects();
            EditorGUI.indentLevel--;
        }

        void drawEffects()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Effects");
            GUILayout.FlexibleSpace();
            if ((instrumentMeta.effects == null || instrumentMeta.effects.Count < 4) &&  GUILayout.Button("Add Effect"))
            {
                if (instrumentMeta.effects == null) instrumentMeta.effects = new System.Collections.Generic.List<int>();
                instrumentMeta.effects.Add(instrumentMeta.effects.Count > 0 ? instrumentMeta.effects[instrumentMeta.effects.Count - 1] : 0);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;
            if (instrumentMeta.effects != null)
            {
                for (int i = 0; i < instrumentMeta.effects.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    instrumentMeta.effects[i] = EditorGUILayout.Popup(instrumentMeta.effects[i], InstrumentFactory.EffectTypes);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Delete"))
                    {
                        instrumentMeta.effects.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}