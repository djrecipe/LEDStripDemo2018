namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// LED pattern modes
    /// </summary>
    public enum PatternModes : int
    {
        Unknown = 0,
        Chase = 1,
        TheaterChase = 2,
        Fill = 3,
        Rain = 4,
        Solid = 5
    }
    /// <summary>
    /// LED color schemes
    /// </summary>
    public enum ColorSchemes : int
    {
        Unknown = 0,
        SimpleGreen = 1,
        SimpleRed = 2,
        SimpleBlue = 3,
        PulsingGreen = 4,
        PulsingRed = 5,
        PulsingBlue = 6,
        Rainbow = 7,
        Random = 8
    }
}
