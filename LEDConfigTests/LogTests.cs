using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbaciLabs.LEDConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AbaciLabs.LEDConfig.Tests
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void TestLogInfo()
        {
            LogManager.Log.Info("TestLogInfo()");
            return;
        }
    }
}
