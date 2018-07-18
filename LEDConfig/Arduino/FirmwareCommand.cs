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
            // color increment
            int color_increment = int.Parse(words[3]);
            // spacing
            int spacing = int.Parse(words[4]);
            // create firmware command
            FirmwareSettings settings = new FirmwareSettings()
            {
                ColorIncrement = color_increment,
                ColorScheme = color_scheme,
                Delay = delay,
                PatternMode = pattern_mode,
                PatternSpacing = spacing
            };
            FirmwareCommand command = new FirmwareCommand(settings);
            return command;
        }
        #endregion
        #region Instance Members
        private readonly string commandString = null;
        #endregion
        #region Instance Properties
        public FirmwareSettings Settings { get; private set; }
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
        /// <param name="settings">Firmware settings</param>
        public FirmwareCommand(FirmwareSettings settings)
        {
            this.Settings = settings;

            this.commandString = this.CreateCommandString(settings);
            return;
        }
        private string CreateCommandString(FirmwareSettings settings)
        {
            string command = string.Format("{0};{1};{2};{3};{4};", PatternModeStrings[settings.PatternMode],
                ColorSchemeStrings[settings.ColorScheme], settings.Delay, settings.ColorIncrement, settings.PatternSpacing);
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
