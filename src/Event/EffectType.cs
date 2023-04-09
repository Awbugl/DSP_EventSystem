using System;

namespace DSP_EventSystem
{
    [Flags]
    public enum EffectType
    {
        None = 0,
        AddItem = 1,
        AddVein = 2,
        AddTechHash = 4,
    }
}
