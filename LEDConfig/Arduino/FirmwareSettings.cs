using System.Runtime.Serialization;

namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// Firmware settings for Arduino-based LED driver
    /// </summary>
    [DataContract(Namespace = "")]
    public class FirmwareSettings
    {
        #region Instance Properties
        /// <summary>
        /// Color cycling increment value
        /// </summary>
        [DataMember]
        public int ColorIncrement { get; set; } = 1;
        /// <summary>
        /// LED color scheme
        /// </summary>
        [DataMember]
        public ColorSchemes ColorScheme { get; set; } = ColorSchemes.Unknown;
        /// <summary>
        /// Delay value for LED cycles
        /// </summary>
        [DataMember]
        public int Delay { get; set; } = 10;
        /// <summary>
        /// LED pattern mode
        /// </summary>
        [DataMember]
        public PatternModes PatternMode { get; set; } = PatternModes.Unknown;
        /// <summary>
        /// LED pattern spacing value
        /// </summary>
        [DataMember]
        public int PatternSpacing { get; set; } = 4;
        #endregion
    }
}
