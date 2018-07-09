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
        /// Delay value for LED cycles
        /// </summary>
        [DataMember]
        public int Delay { get; set; } = 10;
        /// <summary>
        /// LED pattern mode
        /// </summary>
        [DataMember]
        public PatternModes PatternMode { get; set; } = PatternModes.Unknown;
        #endregion
    }
}
