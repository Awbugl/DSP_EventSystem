using System;

namespace DSP_EventSystem
{
    [Flags]
    public enum EffectType
    {
        None = 0,
        AddItem = 1,
        AddVein = 2,
        AddEntity = 4,
        AddTechHash = 8,
        TriggerItemEvent = 16,
        TriggerVeinEvent = 32,
        TriggerEntityEvent = 64,
        TriggerPlanetEvent = 128,
        TriggerStarEvent = 256,
        PlanetEffect = 512,
        StarEffect = 1024,
    }
}
