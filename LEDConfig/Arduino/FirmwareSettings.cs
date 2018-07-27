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
        [FieldOffset(0)] public int ColorIncrement;
        [FieldOffset(4)] public ColorSchemes ColorScheme;
        [FieldOffset(8)] public int PatternDelay;
        [FieldOffset(12)] public PatternModes PatternMode;
        [FieldOffset(16)] public int PatternSpacing;
    }
}
