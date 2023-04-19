using System.Collections.Generic;

namespace DSP_EventSystem.Reflection
{
    public interface IEventTrigger
    {
        Queue<Event> Events { get; }

        bool CanTriggerMultipleTimes { get; }

        bool CanTrigger(PlanetData planet);

        void OnTriggered();
    }
}
