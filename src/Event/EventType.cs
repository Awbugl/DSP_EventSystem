namespace DSP_EventSystem
{
    public enum EventType
    {
        None, // Should not be used
        Item,
        Vein,
        Entity,
        Planet,
        Star,
        Galaxy,
        Player
    }
    
    public enum PlanetEventSubType
    {
        None, // Should not be used
        Terrestrial,
        GasGiant
    }
    
    public enum StarEventSubType
    {
        None, // Should not be used
        NormalStar,
        BlackHole,
        NeutronStar,
    }
}
