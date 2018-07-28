using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// Firmware command parameters for operating the Arduino-controlled LED strip
    /// </summary>
    public class FirmwareCommand
    {
        #region Static Members
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
        /// <summary>
        /// Parse a base-64 string into a firmware command object
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Firmware command</returns>
        public static FirmwareCommand Parse(string text)
        {
            // get bytes from base-64 string
            byte[] bytes = Convert.FromBase64String(text);
            // create struct pointer
            FirmwareSettings settings = bytes;
            // create firmware command base on settings struct
            FirmwareCommand command = new FirmwareCommand(settings);
            return command;
        }
        #endregion
        #region Instance Members
        private readonly string commandString = null;
        #endregion
        #region Instance Properties
        /// <summary>
        /// Settings to be received/transmitted by the firmware command
        /// </summary>
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
            // return base-64 string
            return Convert.ToBase64String(settings);
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
