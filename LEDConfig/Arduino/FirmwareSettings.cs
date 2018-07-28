using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FirmwareSettings
    {
        [FieldOffset(0)] public short ColorIncrement;
        [FieldOffset(2)] public ColorSchemes ColorScheme;
        [FieldOffset(4)] public short PatternDelay;
        [FieldOffset(6)] public PatternModes PatternMode;
        [FieldOffset(8)] public short PatternSpacing;
    }
}


//TODO : retrieve FirmwareSettingsClass back from GitHub and use it, but convert to/from this struct