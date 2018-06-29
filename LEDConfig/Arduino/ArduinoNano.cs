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
