using DSP_EventSystem;
using DSP_EventSystem.Reflection;

namespace UnknownOxygenSource
{
    [RegisterEventTrigger]
    public class UnknownOxygenSource : IEventTrigger
    {
        public UnknownOxygenSource()
        {
            Events = new EventChain();
        }

        public EventChain Events { get; }

        public bool CanTriggerMultipleTimes { get; } = false;

        public bool OnPlanetLanded(PlanetData planet)
        {
            return true;
        }

        public void OnTriggered(int stage)
        {
            
        }
    }
}
