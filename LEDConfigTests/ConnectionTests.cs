using System;
using System.Threading.Tasks;
using AbaciLabs.LEDConfig.Arduino;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbaciLabs.LEDConfig.Tests
{
    /// <summary>
    /// Hardware connection tests
    /// </summary>
    [TestClass]
    public class ConnectionTests
    {
        /// <summary>
        /// Test connection to a live Arduino device
        /// </summary>
        [TestMethod, TestCategory("Hardware")]
        public void ConnectArduino()
        {
        }
        
        /// <summary>
        /// Test discovery of a live Arduino device
        /// </summary>
        [TestMethod, TestCategory("Hardware")]
        public void DiscoverArduino()
        {
            ArduinoNanoFactory factory = new ArduinoNanoFactory();
            Task<ArduinoNano> arduino = factory.Search();
            Assert.IsNotNull(arduino?.Result, "Failed to discovery Arduino device");
            return;
        }
    }
}
