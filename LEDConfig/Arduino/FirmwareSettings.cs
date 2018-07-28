using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbaciLabs.LEDConfig.Arduino
{
    [DataContract]
    public class FirmwareSettings
    {
        #region Types
        [StructLayout(LayoutKind.Explicit)]
        private struct FirmwareSettingsStruct
        {
            [FieldOffset(0)] public short ColorIncrement;
            [FieldOffset(2)] public ColorSchemes ColorScheme;
            [FieldOffset(4)] public short PatternDelay;
            [FieldOffset(6)] public PatternModes PatternMode;
            [FieldOffset(8)] public short PatternSpacing;
        }
        #endregion
        #region Static Methods
        // User-defined conversion from Digit to double
        public static implicit operator byte[](FirmwareSettings settings)
        {
            // get structure pointer
            int size = Marshal.SizeOf(settings.values);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(settings.values, ptr, true);
            // get raw bytes of structure
            byte[] bytes = new byte[size];
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }
        //  User-defined conversion from double to Digit
        public static implicit operator FirmwareSettings(byte[] bytes)
        {
            FirmwareSettings settings = new FirmwareSettings();
            int size = Marshal.SizeOf(settings.values);
            IntPtr ptr = Marshal.AllocHGlobal(size);
            // copy bytes to pointer
            Marshal.Copy(bytes, 0, ptr, size);
            // convert pointer to struct
            settings.values = (FirmwareSettingsStruct)Marshal.PtrToStructure(ptr, settings.values.GetType());
            Marshal.FreeHGlobal(ptr);
            return settings;
        }
        #endregion
        #region Instance Members
        private FirmwareSettingsStruct values;
        #endregion
        #region Instance Properties
        /// <summary>
        /// Color cycling increment value
        /// </summary>
        [DataMember]
        public short ColorIncrement
        {
            get
            {
                return this.values.ColorIncrement;
            }
            set
            {
                this.values.ColorIncrement = value;
            }
        }
        /// <summary>
        /// LED color scheme
        /// </summary>
        [DataMember]
        public ColorSchemes ColorScheme
        {
            get
            {
                return this.values.ColorScheme;
            }
            set
            {
                this.values.ColorScheme = value;
            }
        }
        /// <summary>
        /// Delay value for LED cycles
        /// </summary>
        [DataMember]
        public short PatternDelay
        {
            get
            {
                return this.values.PatternDelay;
            }
            set
            {
                this.values.PatternDelay = value;
            }
        }
        /// <summary>
        /// LED pattern mode
        /// </summary>
        [DataMember]
        public PatternModes PatternMode
        {
            get
            {
                return this.values.PatternMode;
            }
            set
            {
                this.values.PatternMode = value;
            }
        }
        /// <summary>
        /// LED pattern spacing value
        /// </summary>
        [DataMember]
        public short PatternSpacing
        {
            get
            {
                return this.values.PatternSpacing;
            }
            set
            {
                this.values.PatternSpacing = value;
            }
        }
        #endregion
    }
}
