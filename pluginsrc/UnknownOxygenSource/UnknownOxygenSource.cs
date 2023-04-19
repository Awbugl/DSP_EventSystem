using System.Collections.Generic;
using DSP_EventSystem;
using DSP_EventSystem.Reflection;

namespace UnknownOxygenSource
{
    [RegisterEventTrigger]
    public class UnknownOxygenSource : IEventTrigger
    {
        public UnknownOxygenSource()
        {
            Events = new Queue<Event>();
            Events.Enqueue(new Event() { });
        }

        public Queue<Event> Events { get; }

        public bool CanTriggerMultipleTimes { get; } = false;

        public bool CanTrigger(PlanetData planet)
        {
            return true;
        }

        public void OnTriggered() { }
    }
}
