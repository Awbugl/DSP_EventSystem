using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using Newtonsoft.Json;
using UnityEngine;

namespace DSP_EventSystem
{
    public static partial class EventSystem
    {
        internal static Dictionary<EventType, List<Event>> Events { get; }

        private static Dictionary<PlanetEventSubType, List<Event>> PlanetEvents { get; }

        private static Dictionary<StarEventSubType, List<Event>> StarEvents { get; }

        private static T GetRandomEvent<T>(this List<T> list) where T : Event => list[Random.Next(list.Count)];

        static EventSystem()
        {
            AudioClip = AssetBundle
                       .LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "advisor_voice"))
                       .LoadAsset<AudioClip>("assets/voice/materialist.wav");

            var jsonDir = Path.Combine(Paths.ConfigPath, "CustomEventSystem", "events");
            Directory.CreateDirectory(jsonDir);

            Events = new Dictionary<EventType, List<Event>>();
            PlanetEvents = new Dictionary<PlanetEventSubType, List<Event>>();
            StarEvents = new Dictionary<StarEventSubType, List<Event>>();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var file in Directory.GetFiles(jsonDir, "*.json"))
            {
                var jevent = JsonConvert.DeserializeObject<Event>(File.ReadAllText(file));

                switch (jevent.Type)
                {
                    case EventType.Planet:
                    {
                        var planetEventSubType = (PlanetEventSubType)jevent.SubType;

                        if (!PlanetEvents.ContainsKey(planetEventSubType))
                            PlanetEvents.Add(planetEventSubType, new List<Event>());

                        PlanetEvents[planetEventSubType].Add(jevent);

                        goto default;
                    }

                    case EventType.Star:
                    {
                        var starEventSubType = (StarEventSubType)jevent.SubType;

                        if (!StarEvents.ContainsKey(starEventSubType))
                            StarEvents.Add(starEventSubType, new List<Event>());

                        StarEvents[starEventSubType].Add(jevent);

                        goto default;
                    }

                    default:
                    {
                        if (!Events.ContainsKey(jevent.Type)) Events.Add(jevent.Type, new List<Event>());
                        Events[jevent.Type].Add(jevent);

                        continue;
                    }
                }
            }
        }
    }
}
