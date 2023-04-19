namespace DSP_EventSystem.Reflection
{
    public interface IEventTrigger
    {
        EventType Type { get; }

        int SubType { get; }

        bool CanTriggerMultipleTimes { get; }
        
        bool EventChain { get; }

        bool CanTrigger(PlanetData planet);

        void OnTriggered();
    }
}
