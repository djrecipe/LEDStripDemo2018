using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    /// <summary>
    /// Discovers an Arduino device
    /// </summary>
    /// <remarks>Arduino device must return a discovery response of "BM2018"</remarks>
    public class ArduinoNanoFactory
    {
        #region Static Members
        private const string DISCOVER_RESPONSE = "BM2018";
        #endregion
        #region Instance Methods
        /// <summary>
        /// Search for an Arduino device asynchronously
        /// </summary>
        /// <returns>A task containing an Arduino device, if found</returns>
        public async Task<ArduinoNano> Search()
        {
            Task<ArduinoNano> task = new Task<ArduinoNano>(this.SearchTask);
            task.Start();
            await task;
            return task.Result;
        }

        private ArduinoNano SearchTask()
        {
            foreach (string port in SerialPort.GetPortNames())
            {
                LogManager.Log.Info(string.Format("Checking COM port '{0}'", port));
                SerialPort serial = null;
                try
                {
                    serial = new SerialPort(port);
                    serial.BaudRate = 9600;
                    serial.NewLine = "\n";
                    serial.Open();
                    serial.WriteLine(FirmwareCommand.Discovery.ToString());
                    Thread.Sleep(500);
                    string line = serial.ReadExisting();
                    if (line.Contains(ArduinoNanoFactory.DISCOVER_RESPONSE))
                    {
                        LogManager.Log.Info(string.Format("Discovered Arduino device on COM port '{0}'", port));
                        return new ArduinoNano(serial);
                    }
                }
                catch (Exception e)
                {
                    LogManager.Log.Warn(string.Format("Error while checking COM port '{0}'", port), e);
                }
                // close serial port if not used
                try
                {
                    if(serial?.IsOpen ?? false)
                        serial.Close();
                }
                catch (Exception e)
                {
                    LogManager.Log.Warn(string.Format("Faield to close connection on COM port '{0}'", port), e);
                }
            }
            return null;
        }
        #endregion
    }
}
