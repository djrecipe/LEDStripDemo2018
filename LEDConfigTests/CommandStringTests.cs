using System;
using System.Collections.Generic;
using AbaciLabs.LEDConfig.Arduino;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LEDConfig.Tests
{
    [TestClass]
    public class CommandStringTests
    {
        [TestMethod]
        public void ParseValidCommandStringResponse()
        {
            string text = "c;sr;100;1;";
            FirmwareCommand command = FirmwareCommand.Parse(text);
            Assert.AreEqual(PatternModes.Chase, command.Settings.PatternMode, "Pattern mode mismatch");
            Assert.AreEqual(100, command.Settings.Delay, "Pattern mode mismatch");
            return;
        }
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ParseInvalidPatterModeResponse()
        {
            string text = "invalid;sr;10;1;";
            FirmwareCommand command = FirmwareCommand.Parse(text);
            return;
        }
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidDelayValueResponse()
        {
            string text = "t;sr;invalid;1;";
            FirmwareCommand command = FirmwareCommand.Parse(text);
            return;
        }
    }
}
