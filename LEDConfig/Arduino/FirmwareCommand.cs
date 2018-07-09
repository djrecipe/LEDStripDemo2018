using System.Collections.Generic;
using System.Linq;

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
            {PatternModes.TheaterRainbowCycle, "t"},
            {PatternModes.Unknown, "?"}
        };
        #endregion
        #region Static Properties
        /// <summary>
        /// Discovery command
        /// </summary>
        public static FirmwareCommand Discovery { get; }
        /// <summary>
        /// Export settings command
        /// </summary>
        public static FirmwareCommand ExportSettings { get; }
        #endregion
        #region Static Methods
        static FirmwareCommand()
        {
            FirmwareCommand.Discovery = new FirmwareCommand("ID;");
            FirmwareCommand.ExportSettings = new FirmwareCommand("ES;");
            return;
        }

        public static FirmwareCommand ParseCommandString(string text)
        {
            string[] words = text.Split(';');
            PatternModes pattern_mode = PatternModeStrings.FirstOrDefault(pair => pair.Value == words[0]).Key;
            int delay = int.Parse(words[1]);
            FirmwareCommand command = new FirmwareCommand(pattern_mode, delay);
            return command;
        }
        #endregion
        #region Instance Members
        private readonly string commandString = null;
        #endregion
        #region Instance Properties
        public int Delay {get; private set; }
        public PatternModes PatternMode { get; private set; }
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
            this.Delay = delay;
            this.PatternMode = pattern;

            this.commandString = this.CreateCommandString(pattern, delay);
            return;
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
