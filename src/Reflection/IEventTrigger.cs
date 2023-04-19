using System.Collections.Generic;

namespace DSP_EventSystem.Reflection
{
    public interface IEventTrigger
    {
        EventChain Events { get; }

        bool CanTriggerMultipleTimes { get; }

        bool OnPlanetLanded(PlanetData planet);
        
        void OnTriggered(int stage);
    }
}
