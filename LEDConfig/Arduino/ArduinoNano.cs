using System;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// Arduino device
    /// </summary>
    public class ArduinoNano : IDisposable
    {
        #region Instance Members
        private readonly SerialPort serialPort = new SerialPort();
        #endregion
        #region Instance Properties
        /// <summary>
        /// LED color scheme
        /// </summary>
        public ColorSchemes ColorScheme { get; private set; } = ColorSchemes.Unknown;
        /// <summary>
        /// Device is connected and serial port is open
        /// </summary>
        public bool Connected
        {
            get { return this.serialPort.IsOpen; }
        }
        public int Delay { get; private set; }
        /// <summary>
        /// LED pattern mode
        /// </summary>
        public PatternModes PatternMode { get; private set; } = PatternModes.Unknown;
        /// <summary>
        /// Device port name
        /// </summary>
        public string Port => this.serialPort.PortName;
        /// <summary>
        /// Rainbow color incrementer
        /// </summary>
        public int RainbowIncrement { get; private set; }
        public int Spacing { get; private set; }
        #endregion
        #region Instance Methods
        /// <summary>
        /// Instantiate a new Arduino device using the specified serial port
        /// </summary>
        /// <param name="port">Device serial port</param>
        public ArduinoNano(SerialPort port)
        {
            if(port == null)
                throw new ArgumentNullException("NULL serial port");
            if(!port.IsOpen)
                throw new ArgumentException("Port is not open");
            this.serialPort = port;
            return;
        }
        /// <summary>
        /// Retrieve settings from Arduino device asynchronously
        /// </summary>
        public void RetrieveSettings()
        {
            //
            this.SendCommand(FirmwareCommand.ExportSettings);
            //
            string text = this.serialPort.ReadExisting();
            if(string.IsNullOrWhiteSpace(text))
                throw new IOException("Failed to retrieve settings response from Arduino device");
            //
            int start_index = text.IndexOf(":ES:");
            if(start_index < 0)
                throw new IOException(string.Format("Failed to locate settings response prefix ('{0}')", text));
            start_index += 4;
            int end_index = text.IndexOf(":ES:", start_index);
            if(end_index < 0)
                throw new IOException(string.Format("Failed to locate settings response suffix ('{0}')", text));
            text = text.Substring(start_index, end_index - start_index);
            //
            FirmwareCommand command = FirmwareCommand.ParseCommandString(text);
            //
            this.ColorScheme = command.ColorScheme;
            this.PatternMode = command.PatternMode;
            this.Delay = command.Delay;
            this.RainbowIncrement = command.RainbowIncrement;
            this.Spacing = command.Spacing;
            return;
        }
        /// <summary>
        /// Send a fimrware command to the Arduino device
        /// </summary>
        /// <param name="command">Firmware command to send</param>
        public void SendCommand(FirmwareCommand command)
        {
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();
            this.serialPort.WriteLine(command.ToString());
            Thread.Sleep(2000);
        }
        /// <summary>
        /// Clean-up the object and prepare for deletion
        /// </summary>
        public void Dispose()
        {
            if(this.serialPort.IsOpen)
                this.serialPort.Close();
            this.serialPort.Dispose();
            return;
        }
        #endregion

    }
}
