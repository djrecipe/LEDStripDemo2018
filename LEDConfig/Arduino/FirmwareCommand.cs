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
        private static readonly Dictionary<ColorSchemes, string> ColorSchemeStrings = new Dictionary<ColorSchemes, string>()
        {
            {ColorSchemes.PulsingRed, "pr"},
            {ColorSchemes.PulsingGreen, "pg"},
            {ColorSchemes.PulsingBlue, "pb"},
            {ColorSchemes.Rainbow, "r"},
            {ColorSchemes.Random, "x"},
            {ColorSchemes.SimpleRed, "sr"},
            {ColorSchemes.SimpleGreen, "sg"},
            {ColorSchemes.SimpleBlue, "sb"},
            {ColorSchemes.Unknown, "?"}
        };
        private static readonly Dictionary<PatternModes, string> PatternModeStrings = new Dictionary<PatternModes, string>()
        {
            {PatternModes.Unknown, "?"},
            {PatternModes.Chase, "c"},
            {PatternModes.TheaterChase, "h"},
            {PatternModes.Fill, "f"},
            {PatternModes.Rain, "r"},
            {PatternModes.Solid, "s"},
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
            // pattern mode
            PatternModes pattern_mode = PatternModeStrings.FirstOrDefault(pair => pair.Value == words[0]).Key;
            // color scheme
            ColorSchemes color_scheme = ColorSchemeStrings.FirstOrDefault(pair => pair.Value == words[1]).Key;
            // delay value
            int delay = int.Parse(words[2]);
            // rainbow increment
            int rainbow_increment = int.Parse(words[3]);
            // spacing
            int spacing = int.Parse(words[4]);
            // create firmware command
            FirmwareCommand command = new FirmwareCommand(pattern_mode, color_scheme, delay, rainbow_increment, spacing);
            return command;
        }
        #endregion
        #region Instance Members
        private readonly string commandString = null;
        #endregion
        #region Instance Properties
        public ColorSchemes ColorScheme { get; private set; }
        public int Delay {get; private set; }
        public PatternModes PatternMode { get; private set; }
        public int RainbowIncrement { get; private set; }
        public int Spacing { get; private set; }
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
        /// <param name="color_scheme">LED color scheme</param>
        /// <param name="delay">LED cycle delay value</param>
        /// <param name="rainbow_increment">LED rainbow color incrementer</param>
        /// <param name="spacing">Sequence spacing value</param>
        public FirmwareCommand(PatternModes pattern, ColorSchemes color_scheme, int delay, int rainbow_increment, int spacing)
        {
            this.ColorScheme = color_scheme;
            this.Delay = delay;
            this.PatternMode = pattern;
            this.RainbowIncrement = rainbow_increment;
            this.Spacing = spacing;

            this.commandString = this.CreateCommandString(pattern, color_scheme, delay, rainbow_increment, spacing);
            return;
        }
        private string CreateCommandString(PatternModes pattern, ColorSchemes color_scheme, int delay, int rainbow_increment, int spacing)
        {
            string command = string.Format("{0};{1};{2};{3};{4};", PatternModeStrings[pattern], ColorSchemeStrings[color_scheme], delay, rainbow_increment, spacing);
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
