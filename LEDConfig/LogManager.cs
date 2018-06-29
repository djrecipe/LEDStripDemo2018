using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Core;

namespace AbaciLabs.LEDConfig
{
    public abstract class LogManager
    {
        private static readonly ILog _Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static ILog Log => LogManager._Log;
    }
}
