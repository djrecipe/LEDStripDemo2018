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
        /// Device is connected
        /// </summary>
        public bool Connected => this.serialPort?.IsOpen ?? false;
        /// <summary>
        /// Device port name
        /// </summary>
        public string Port => this.serialPort.PortName;
        /// <summary>
        /// Firmware settings
        /// </summary>
        public FirmwareSettings Settings { get; private set; } = new FirmwareSettings();
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
            string text = this.serialPort.ReadExisting().Trim();
            if(string.IsNullOrWhiteSpace(text))
                throw new IOException("Failed to retrieve settings response from Arduino device");
            //
            FirmwareCommand command = FirmwareCommand.Parse(text);
            this.Settings = command.Settings;
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
