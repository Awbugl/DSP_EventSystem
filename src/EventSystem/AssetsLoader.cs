using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using DSP_EventSystem.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace DSP_EventSystem
{
    public static partial class EventSystem
    {
        internal static Dictionary<EventType, List<Event>> Events { get; }

        internal static List<IEventTrigger> EventTriggers { get; }

        private static Dictionary<PlanetEventSubType, List<Event>> PlanetEvents { get; }

        private static Dictionary<StarEventSubType, List<Event>> StarEvents { get; }

        private static T GetRandomEvent<T>(this List<T> list) where T : Event => list[Random.Next(list.Count)];
        
        private static T GetRandomEventTrigger<T>(this List<T> list) where T : IEventTrigger => list[Random.Next(list.Count)];

        static EventSystem()
        {
            AudioClip = AssetBundle
                       .LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "advisor_voice"))
                       .LoadAsset<AudioClip>("assets/voice/materialist.wav");

            var dir = Path.Combine(Paths.ConfigPath, "CustomEventSystem", "events");
            Directory.CreateDirectory(dir);

            Events = new Dictionary<EventType, List<Event>>();
            PlanetEvents = new Dictionary<PlanetEventSubType, List<Event>>();
            StarEvents = new Dictionary<StarEventSubType, List<Event>>();
            foreach (var file in Directory.GetFiles(dir, "*.json")) LoadEventJson(file);

            EventTriggers = new List<IEventTrigger>();
            foreach (var file in Directory.GetFiles(dir, "*.dll")) LoadEventDll(file);
        }

        private static void LoadEventDll(string file)
        {
            var assembly = Assembly.LoadFile(file);

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<RegisterEventTriggerAttribute>() == null ||
                    !type.IsClass ||
                    type.IsAbstract ||
                    !typeof(IEventTrigger).IsAssignableFrom(type))
                    continue;

                var instance = type.GetConstructor(Type.EmptyTypes)?.Invoke(Array.Empty<object>()) as IEventTrigger;

                if (instance == null) continue;

                EventTriggers.Add(instance);
            }
        }

        private static void LoadEventJson(string file)
        {
            var @event = JsonConvert.DeserializeObject<Event>(File.ReadAllText(file));

            switch (@event.Type)
            {
                case EventType.Planet:
                {
                    var planetEventSubType = (PlanetEventSubType)@event.SubType;

                    if (!PlanetEvents.ContainsKey(planetEventSubType)) PlanetEvents.Add(planetEventSubType, new List<Event>());

                    PlanetEvents[planetEventSubType].Add(@event);

                    goto default;
                }

                case EventType.Star:
                {
                    var starEventSubType = (StarEventSubType)@event.SubType;

                    if (!StarEvents.ContainsKey(starEventSubType)) StarEvents.Add(starEventSubType, new List<Event>());

                    StarEvents[starEventSubType].Add(@event);

                    goto default;
                }

                default:
                {
                    if (!Events.ContainsKey(@event.Type)) Events.Add(@event.Type, new List<Event>());
                    Events[@event.Type].Add(@event);

                    return;
                }
            }
        }
    }
}
