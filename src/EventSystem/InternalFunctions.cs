using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

namespace DSP_EventSystem
{
    public static partial class EventSystem
    {
        private static HashSet<int> _landedPlanets = new HashSet<int>();
        private static int _birthPlanetId = -1; 
        
        private static readonly AudioClip AudioClip;
        private static UIEventWindow _uiEventWindow;
        private static readonly DotNet35Random Random = new DotNet35Random();

        internal static ManualLogSource Logger;

        private static readonly bool Devmode = true;
        
        internal static void Export(BinaryWriter w)
        {
            w.Write(_landedPlanets.Count);

            foreach (var planetId in _landedPlanets) w.Write(planetId);
        }

        internal static void Import(BinaryReader r)
        {
            ReInitAll();

            var landedPlanetscount = r.ReadInt32();

            for (var j = 0; j < landedPlanetscount; j++) _landedPlanets.Add(r.ReadInt32());
        }

        internal static void IntoOtherSave() => ReInitAll();

        private static void ReInitAll()
        {
            _landedPlanets = new HashSet<int>();
            _birthPlanetId = -1;
        }
    }
}
