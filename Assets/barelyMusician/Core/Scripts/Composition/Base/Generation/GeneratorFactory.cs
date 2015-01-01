// ----------------------------------------------------------------------
//   Adaptive music composition engine implementation for interactive systems.
//
//     Copyright 2014 Alper Gungormusler. All rights reserved.
//
// ------------------------------------------------------------------------

using UnityEngine;
using System;
using System.Collections;

namespace BarelyAPI
{
    public class GeneratorFactory
    {
        private string[] macroGeneratorTypes;
        public static string[] MacroGeneratorTypes
        {
            get { return instance.macroGeneratorTypes; }
        }

        private string[] mesoGeneratorTypes;
        public static string[] MesoGeneratorTypes
        {
            get { return instance.mesoGeneratorTypes; }
        }

        private static GeneratorFactory _instance;
        private static GeneratorFactory instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GeneratorFactory();

                return _instance;
            }
        }

        GeneratorFactory()
        {
            setGeneratorTypes("MacroGenerators");
            setGeneratorTypes("MesoGenerators");

            Resources.UnloadUnusedAssets();
        }

        void setGeneratorTypes(string type)
        {
            UnityEngine.Object[] assets = Resources.LoadAll("Presets/Generators/" + type);

            string[] types = new string[assets.Length];
            for (int i = 0; i < types.Length; ++i)
            {
                types[i] = assets[i].name;
            }

            switch (type)
            {
                case "MacroGenerators":
                    macroGeneratorTypes = types;
                    break;
                case "MesoGenerators":
                    mesoGeneratorTypes = types;
                    break;
            }
        }

        public static MacroGenerator CreateMacroGenerator(int typeIndex, int sequenceLength, bool loop = true)
        {
            return createMacroGenerator(MacroGeneratorTypes[typeIndex], sequenceLength, loop);
        }

        public static MesoGenerator CreateMesoGenerator(int typeIndex, Sequencer sequencer)
        {
            return createMesoGenerator(MesoGeneratorTypes[typeIndex], sequencer);
        }

        static MacroGenerator createMacroGenerator(string type, int sequenceLength, bool loop)
        {
            Type macroType = Type.GetType("BarelyAPI." + type);
            if (macroType == null) macroType = Type.GetType("BarelyAPI.DefaultMacroGenerator");

            return (MacroGenerator)Activator.CreateInstance(macroType, sequenceLength, loop);
        }

        static MesoGenerator createMesoGenerator(string type, Sequencer sequencer)
        {
            Type mesoType = Type.GetType("BarelyAPI." + type);
            if (mesoType == null) mesoType = Type.GetType("BarelyAPI.DefaultMesoGenerator");

            return (MesoGenerator)Activator.CreateInstance(mesoType, sequencer);
        }
    }
}