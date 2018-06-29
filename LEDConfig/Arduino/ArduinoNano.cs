using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
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
        private string _PortName = "COM1";
        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName
        {
            get { return this._PortName; }
            set
            {
                if(string.IsNullOrWhiteSpace(value) || value.StartsWith(@"\"))
                    throw new ArgumentException("Invalid port name");
                this._PortName = value;
                this.UpdateSerialPort();
                return;
            }
        }
        #endregion
        #region Instance Methods
        /// <summary>
        /// Connect to the serial port
        /// </summary>
        public void Connect()
        {
            if(!this.serialPort.IsOpen)
                this.serialPort.Open();
            return;
        }
        /// <summary>
        /// Disconnect from the serial port
        /// </summary>
        public void Disconnect()
        {
            if(this.serialPort.IsOpen)
                this.serialPort.Close();
            return;
        }
        /// <summary>
        /// Clean-up the object and prepare for deletion
        /// </summary>
        public void Dispose()
        {
            this.Disconnect();
            this.serialPort?.Dispose();
            return;
        }
        private void UpdateSerialPort()
        {
            this.Disconnect();
            this.serialPort.PortName = this.PortName;
            return;
        }
        #endregion

    }
}
