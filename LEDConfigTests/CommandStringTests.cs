using System;
using System.Collections.Generic;
using AbaciLabs.LEDConfig.Arduino;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LEDConfig.Tests
{
    /// <summary>
    /// Test command strings
    /// </summary>
    [TestClass]
    public class CommandStringTests
    {
        /// <summary>
        /// Create a firmware command from explicit settings, convert to base-64 string,
        /// then parse the base-64 string back into a firmware command
        /// </summary>
        [TestMethod]
        public void CreateAndParse()
        {
            // declare expected values
            int expected_color_increment = 10;
            ColorSchemes expected_color_scheme = ColorSchemes.Rainbow;
            int expected_pattern_delay = 150;
            PatternModes expected_pattern_mode = PatternModes.Rider;
            int expected_pattern_spacing = 4;
            // create settings struct
            FirmwareSettings settings = new FirmwareSettings
            {
                ColorIncrement = expected_color_increment,
                ColorScheme = expected_color_scheme,
                PatternDelay = expected_pattern_delay,
                PatternMode = expected_pattern_mode,
                PatternSpacing = expected_pattern_spacing
            };
            // create firmware command
            FirmwareCommand command = new FirmwareCommand(settings);
            // get firmware command base-64 string
            string text = command.ToString();
            // parse command from base-64 string
            FirmwareCommand parsed_command = FirmwareCommand.Parse(text);
            // make assertions
            Assert.AreEqual(expected_color_increment, parsed_command.Settings.ColorIncrement, "Color increment mismatch");
            Assert.AreEqual(expected_color_scheme, parsed_command.Settings.ColorScheme, "Color scheme mismatch");
            Assert.AreEqual(expected_pattern_delay, parsed_command.Settings.PatternDelay, "Pattern delay mismatch");
            Assert.AreEqual(expected_pattern_mode, parsed_command.Settings.PatternMode, "Pattern mode mismatch");
            Assert.AreEqual(expected_pattern_spacing, parsed_command.Settings.PatternSpacing, "Pattern spacing mismatch");
            return;
        }
    }
}
