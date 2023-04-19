using DSP_EventSystem;
using DSP_EventSystem.Reflection;

namespace UnknownOxygenSource
{
    [RegisterEventTrigger]
    public class UnknownOxygenSource : Event, IEventTrigger 
    {
        public bool CanTriggerMultipleTimes { get; }
        
        public bool EventChain { get; }

        public bool CanTrigger(PlanetData planet)
        {
            throw new System.NotImplementedException();
        }

        public void OnTriggered()
        {
            throw new System.NotImplementedException();
        }
    }
}
