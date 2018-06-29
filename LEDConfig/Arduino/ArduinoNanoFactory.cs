using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    public class ArduinoNanoFactory
    {
        private const string DISCOVERY_STR = "ID;";
        private const string DISCOVER_RESPONSE = "BM2018";

        public async Task<ArduinoNano> Search()
        {
            Task<ArduinoNano> task = new Task<ArduinoNano>(this.SearchTask);
            await task;
            return task.Result;
        }

        private ArduinoNano SearchTask()
        {
            foreach (string port in SerialPort.GetPortNames())
            {
                SerialPort serial = null;
                try
                {
                    serial = new SerialPort(port);
                    serial.Open();
                    serial.WriteLine(ArduinoNanoFactory.DISCOVERY_STR);
                    Thread.Sleep(100);
                    string line = serial.ReadLine();
                    if (line.Contains(ArduinoNanoFactory.DISCOVER_RESPONSE))
                        return new ArduinoNano(serial);
                }
                catch (Exception e)
                {
                }
                // close serial port if not used
                try
                {
                    if(serial?.IsOpen ?? false)
                        serial.Close();
                }
                catch (Exception e)
                {
                }
            }
            return null;
        }
    }
}
