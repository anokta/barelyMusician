//// ----------------------------------------------------------------------
////   Adaptive music composition engine implementation for interactive systems.
////
////     Copyright 2014 Alper Gungormusler. All right reserved.
////
//// ------------------------------------------------------------------------

//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;

//namespace BarelyAPI
//{
//    [CustomEditor(typeof(Musician))]
//    public class MusicianEditor : Editor
//    {
//        Musician musician;

//        PerformerWindow performerWindow;

//        int selection;

//        void OnEnable()
//        {
//            musician = (Musician)target;
//            musician.Init();

//            if (performerWindow != null)
//                performerWindow.musician = musician;
//        }

//        public override void OnInspectorGUI()
//        {
//            //DrawDefaultInspector();

//            // Volume
//            musician.MasterVolume = EditorGUILayout.Slider(new GUIContent("Volume", "Master volume."), musician.MasterVolume, 0.0f, 1.0f);
//            musician.PlayOnAwake = EditorGUILayout.Toggle("Play On Awake", musician.PlayOnAwake);

//            EditorGUILayout.Space();

//            // Sequencer properties
//            GUILayout.Box("Sequencer");
//            EditorGUILayout.Space();
//            musician.Tempo = EditorGUILayout.IntSlider(new GUIContent("Tempo", "BPM."), musician.Tempo, 88, 220);

//            if (musician.IsPlaying) GUI.enabled = false;
//            musician.SongDuration = EditorGUILayout.Slider(new GUIContent("Song Duration", "Song duration in minutes."), musician.SongDuration, 1.0f, 10.0f);
//            musician.BarsPerSection = EditorGUILayout.IntSlider(new GUIContent("Bars Per Section", "Number of bars per each section."), musician.BarsPerSection, 1, 12);
//            musician.BeatsPerBar = EditorGUILayout.IntSlider(new GUIContent("Beats Per Bar", "Number of beats per each bar."), musician.BeatsPerBar, 1, 16);
//            if (musician.IsPlaying) GUI.enabled = true;

//            EditorGUILayout.Space();

//            // Key
//            musician.RootNote = (NoteIndex)EditorGUILayout.EnumPopup(new GUIContent("Root Note", "Fundamental key of the song."), musician.RootNote);

//            EditorGUILayout.Space();

//            // Ensemble
//            GUILayout.Box("Ensemble");
//            EditorGUILayout.Space();

//            if (musician.Ensemble.CurrentSection != SectionType.NONE) GUI.enabled = false;
//            musician.MacroGeneratorTypeIndex = EditorGUILayout.Popup("Macro Generator", musician.MacroGeneratorTypeIndex, GeneratorFactory.MacroGeneratorTypes);
//            musician.MesoGeneratorTypeIndex = EditorGUILayout.Popup("Meso Generator", musician.MesoGeneratorTypeIndex, GeneratorFactory.MesoGeneratorTypes);
//            // = EditorGUILayout.TextField(new GUIContent("Macro Generator", "For the musical form (sequence of sections)."), musician.MacroGeneratorType);
//            //musician.MesoGeneratorType = EditorGUILayout.TextField(new GUIContent("Meso Generator", "For harmonic progressions (sequence of bars per section)."), musician.MesoGeneratorType);

//            EditorGUILayout.Space();

//            // Performers
//            musician.performerFoldout = EditorGUILayout.Foldout(musician.performerFoldout, "Performers");
//            if (musician.performerFoldout)
//            {
//                EditorGUI.indentLevel++;
//                for (int i = 0; i < musician.PerformerNames.Count; ++i)
//                {
//                    if (!musician.Instruments[i].Active) GUI.enabled = false;

//                    EditorGUILayout.BeginHorizontal();
//                    EditorGUILayout.LabelField(musician.PerformerNames[i]);
//                    GUILayout.FlexibleSpace();
//                    GUI.enabled = true;

//                    bool active = GUILayout.Toggle(musician.Instruments[i].Active, "");
//                    if (musician.Instruments[i].Active != active)
//                    {
//                        musician.Instruments[i].Active = active;
//                        musician.Ensemble.TogglePeformer(musician.PerformerNames[i], musician.Instruments[i].Active);
//                    }
//                    if (musician.Ensemble.CurrentSection != SectionType.NONE) GUI.enabled = false; 
//                    if (GUILayout.Button("Edit"))
//                    {
//                        performerWindow = (PerformerWindow)EditorWindow.GetWindow(typeof(PerformerWindow), true, "Performer");
//                        performerWindow.performerName = musician.PerformerNames[i];
//                        performerWindow.instrumentMeta = InstrumentMeta.Instantiate(musician.Instruments[i]) as InstrumentMeta;
//                        performerWindow.microGeneratorType = musician.MicroGeneratorTypes[i];
//                        performerWindow.editIndex = i;
//                        performerWindow.musician = musician;
//                        break;
//                    }
//                    if (GUILayout.Button("Delete"))
//                    {
//                        musician.DeregisterPerformer(musician.PerformerNames[i]);
//                        break;
//                    }
//                    if (!active) GUI.enabled = false;
//                    EditorGUILayout.EndHorizontal();

//                    EditorGUI.indentLevel++;
//                    EditorGUILayout.BeginHorizontal(); EditorGUILayout.PrefixLabel("Instrument Type"); EditorGUILayout.LabelField(InstrumentFactory.InstrumentTypes[musician.Instruments[i].Type]); EditorGUILayout.EndHorizontal();
//                    EditorGUILayout.BeginHorizontal(); EditorGUILayout.PrefixLabel("Micro Generator"); EditorGUILayout.LabelField(GeneratorFactory.MicroGeneratorTypes[musician.MicroGeneratorTypes[i]]); EditorGUILayout.EndHorizontal();
//                    EditorGUI.indentLevel--;

//                    if (!active && musician.Ensemble.CurrentSection == SectionType.NONE) GUI.enabled = true;

//                    EditorGUILayout.Space();
//                }

//                EditorGUILayout.BeginHorizontal();
//                GUILayout.FlexibleSpace();
//                if (GUILayout.Button("Add New Performer") && performerWindow == null)
//                {
//                    performerWindow = (PerformerWindow)EditorWindow.GetWindow(typeof(PerformerWindow), true, "Performer");
//                    performerWindow.musician = musician;
//                    performerWindow.performerName = "Performer" + (musician.PerformerNames.Count + 1).ToString();
//                    performerWindow.instrumentMeta = ScriptableObject.CreateInstance<InstrumentMeta>();
//                }
//                EditorGUILayout.EndHorizontal();
//                EditorGUI.indentLevel--;
//            }
//            if (musician.Ensemble.CurrentSection != SectionType.NONE) GUI.enabled = true;

//            EditorGUILayout.Space();

//            // Mood selection
//            GUILayout.Box("Conductor");
//            EditorGUILayout.Space();
//            Mood mood = (Mood)EditorGUILayout.EnumPopup("Mood", musician.Mood);
//            if (mood != musician.Mood) musician.SetMood(mood);

//            if (musician.Mood == Mood.Custom)
//            {
//               EditorGUI.indentLevel++;
//                float energy = EditorGUILayout.Slider("Energy", musician.Energy, 0.0f, 1.0f);
//                if (energy != musician.Energy)
//                {
//                    musician.SetEnergy(energy);
//                }
//                float stress = EditorGUILayout.Slider("Stress", musician.Stress, 0.0f, 1.0f);
//                if (stress != musician.Stress)
//                    musician.SetStress(stress);
//                EditorGUI.indentLevel--;
//            }
//        }
//    }
//}