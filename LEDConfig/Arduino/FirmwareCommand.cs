using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// Firmware command parameters for operating the Arduino-controlled LED strip
    /// </summary>
    public class FirmwareCommand
    {
        #region Static Members
        private static readonly Dictionary<PatternModes, string> PatternModeStrings = new Dictionary<PatternModes, string>()
        {
            {PatternModes.Manual, "m"},
            {PatternModes.Rainbow, "b"},
            {PatternModes.RainbowCycle, "r"},
            {PatternModes.TheaterRainbowCycle, "t"}
        };
        #endregion
        #region Static Properties
        /// <summary>
        /// Discovery command
        /// </summary>
        public static FirmwareCommand Discovery { get; }
        #endregion
        #region Static Methods
        static FirmwareCommand()
        {
            FirmwareCommand.Discovery = new FirmwareCommand("ID;");
            return;
        }
        #endregion
        #region Instance Members
        private readonly string commandString = null;
        #endregion
        #region Instance Methods
        private FirmwareCommand(string command)
        {
            this.commandString = command;
            return;
        }
        /// <summary>
        /// Create a new firmware command string using the specified parameters
        /// </summary>
        /// <param name="pattern">LED pattern mode</param>
        /// <param name="delay">LED cycle delay value</param>
        public FirmwareCommand(PatternModes pattern, int delay = 10)
        {
            if(delay < 1 || delay > 1000)
                throw new ArgumentException("Invalid delay value (1 < delay < 1001)");
            this.commandString = this.CreateCommandString(pattern, delay);
        }
        private string CreateCommandString(PatternModes pattern, int delay)
        {
            string command = string.Format("{0};{1};", PatternModeStrings[pattern], delay);
            return command;
        }
        #endregion
        #region Override Methods
        /// <summary>
        /// Return a formatted firmware command string
        /// </summary>
        /// <returns>Formatted firmware command string</returns>
        public override string ToString()
        {
            return this.commandString;
        }
        #endregion
    }
}
