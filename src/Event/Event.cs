using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace DSP_EventSystem
{
    public class Event
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
        public int SubType { get; set; }
        public Effect[] Effects { get; set; }
        public StringProto[] Translations { get; set; }
    }

    public class Effect
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public EffectType Type { get; set; }
        public Dictionary<EffectType, int[]> Value { get; set; }
        [CanBeNull] public Action OnEffect { get; set; } = null;
    }
}
