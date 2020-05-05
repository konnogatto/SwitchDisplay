using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
namespace SwitchDisplay
{
    public class DisplayHandler
    {
        public List<DeviceInfo> Enumerate()
        {
            var displays = new List<DeviceInfo>();
            DISPLAY_DEVICE display = new DISPLAY_DEVICE(DisplayDeviceStateFlags.AttachedToDesktop);
            uint displayIndex = 0;
            while (EnumDisplayDevices(null, displayIndex, ref display, 0))
            {
                displayIndex++;
                display.cb = Marshal.SizeOf(display);
                uint monitorIndex = 0;
                DISPLAY_DEVICE monitor = new DISPLAY_DEVICE(DisplayDeviceStateFlags.AttachedToDesktop);

                var devMode = new DEVMODE();
                EnumDisplaySettings(display.DeviceName, -1, ref devMode);


                while (EnumDisplayDevices(display.DeviceName, monitorIndex, ref monitor, 0))
                {
                    monitorIndex++;
                    displays.Add(new DeviceInfo {
                        DeviceIndex = displayIndex,
                        DeviceName = display.DeviceName,
                        DeviceString = display.DeviceString,
                        MonitorIndex = monitorIndex,
                        MonitorName = monitor.DeviceName,
                        MonitorString = String.Format("{0} {1}x{2}", monitor.DeviceString, devMode.dmPelsWidth, devMode.dmPelsHeight)
                    });
                    display.cb = Marshal.SizeOf(display);
                }
                
            }
            return displays;
        }

        public bool SwitchPrimaryDisplay(string deviceName)
        {
            var displays = Enumerate();
            var device = displays.Find(d => d.DeviceName.Equals(deviceName));

            if(!device.DeviceName.Equals(deviceName))
            {
                return false;
            }

            var deviceMode = new DEVMODE();
     
            if (!EnumDisplaySettings(device.DeviceName, -1, ref deviceMode)){
                return false;
            }
            
            var offsetx = deviceMode.dmPosition.x;
            var offsety = deviceMode.dmPosition.y;
            if(offsetx == 0 && offsety == 0)
            {
                return true;
            }
            deviceMode.dmPosition.x = 0;
            deviceMode.dmPosition.y = 0;

            if(ChangeDisplaySettingsEx(
                device.DeviceName,
                ref deviceMode,
                (IntPtr)null,
                (ChangeDisplaySettingsFlags.CDS_SET_PRIMARY | ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET),
                IntPtr.Zero) != DISP_CHANGE.Successful)
            {
                return false;
            }

            var otherDisplays = displays.FindAll(d => !d.DeviceName.Equals(deviceName));
            foreach (var otherDisplay in otherDisplays)
            {
                var otherMode = new DEVMODE();
                if(!EnumDisplaySettings(otherDisplay.DeviceName, -1, ref otherMode))
                {
                    return false;
                }

                otherMode.dmPosition.x -= offsetx;
                otherMode.dmPosition.y -= offsety;
                if(ChangeDisplaySettingsEx(
                    otherDisplay.DeviceName,
                    ref otherMode,
                    (IntPtr)null,
                    (ChangeDisplaySettingsFlags.CDS_UPDATEREGISTRY | ChangeDisplaySettingsFlags.CDS_NORESET),
                    IntPtr.Zero) != DISP_CHANGE.Successful)
                {
                    return false;
                }
            }
            return ChangeDisplaySettingsEx(null, IntPtr.Zero, (IntPtr)null, ChangeDisplaySettingsFlags.CDS_NONE, (IntPtr)null) == DISP_CHANGE.Successful;
        }

        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern DISP_CHANGE ChangeDisplaySettingsEx(string lpszDeviceName, IntPtr lpDevMode, IntPtr hwnd, ChangeDisplaySettingsFlags dwflags, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);
    }

    public struct DeviceInfo
    {
        public string DeviceName;
        public string DeviceString;
        public uint DeviceIndex;
        public string MonitorName;
        public uint MonitorIndex;
        public string MonitorString;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        public const int CCHDEVICENAME = 32;
        public const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        [System.Runtime.InteropServices.FieldOffset(0)]
        public string dmDeviceName;
        [System.Runtime.InteropServices.FieldOffset(32)]
        public Int16 dmSpecVersion;
        [System.Runtime.InteropServices.FieldOffset(34)]
        public Int16 dmDriverVersion;
        [System.Runtime.InteropServices.FieldOffset(36)]
        public Int16 dmSize;
        [System.Runtime.InteropServices.FieldOffset(38)]
        public Int16 dmDriverExtra;
        [System.Runtime.InteropServices.FieldOffset(40)]
        public UInt32 dmFields;

        [System.Runtime.InteropServices.FieldOffset(44)]
        Int16 dmOrientation;
        [System.Runtime.InteropServices.FieldOffset(46)]
        Int16 dmPaperSize;
        [System.Runtime.InteropServices.FieldOffset(48)]
        Int16 dmPaperLength;
        [System.Runtime.InteropServices.FieldOffset(50)]
        Int16 dmPaperWidth;
        [System.Runtime.InteropServices.FieldOffset(52)]
        Int16 dmScale;
        [System.Runtime.InteropServices.FieldOffset(54)]
        Int16 dmCopies;
        [System.Runtime.InteropServices.FieldOffset(56)]
        Int16 dmDefaultSource;
        [System.Runtime.InteropServices.FieldOffset(58)]
        Int16 dmPrintQuality;

        [System.Runtime.InteropServices.FieldOffset(44)]
        public POINTL dmPosition;
        [System.Runtime.InteropServices.FieldOffset(52)]
        public Int32 dmDisplayOrientation;
        [System.Runtime.InteropServices.FieldOffset(56)]
        public Int32 dmDisplayFixedOutput;

        [System.Runtime.InteropServices.FieldOffset(60)]
        public short dmColor; // See note below!
        [System.Runtime.InteropServices.FieldOffset(62)]
        public short dmDuplex; // See note below!
        [System.Runtime.InteropServices.FieldOffset(64)]
        public short dmYResolution;
        [System.Runtime.InteropServices.FieldOffset(66)]
        public short dmTTOption;
        [System.Runtime.InteropServices.FieldOffset(68)]
        public short dmCollate; // See note below!
        [System.Runtime.InteropServices.FieldOffset(72)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [System.Runtime.InteropServices.FieldOffset(102)]
        public Int16 dmLogPixels;
        [System.Runtime.InteropServices.FieldOffset(104)]
        public Int32 dmBitsPerPel;
        [System.Runtime.InteropServices.FieldOffset(108)]
        public Int32 dmPelsWidth;
        [System.Runtime.InteropServices.FieldOffset(112)]
        public Int32 dmPelsHeight;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmDisplayFlags;
        [System.Runtime.InteropServices.FieldOffset(116)]
        public Int32 dmNup;
        [System.Runtime.InteropServices.FieldOffset(120)]
        public Int32 dmDisplayFrequency;
    }

    public enum DISP_CHANGE : int
    {
        Successful = 0,
        Restart = 1,
        Failed = -1,
        BadMode = -2,
        NotUpdated = -3,
        BadFlags = -4,
        BadParam = -5,
        BadDualView = -6
    }



    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;

        public DISPLAY_DEVICE(DisplayDeviceStateFlags flags)
        {
            cb = 0;
            StateFlags = flags;
            DeviceName = new string((char)32, 32);
            DeviceString = new string((char)32, 128);
            DeviceID = new string((char)32, 128);
            DeviceKey = new string((char)32, 128);
            cb = Marshal.SizeOf(this);
        }
    }

    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        PrimaryDevice = 0x4,
        MirroringDriver = 0x8,
        VGACompatible = 0x10,
        Removable = 0x20,
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000,
    }

    [Flags()]
    public enum ChangeDisplaySettingsFlags : uint
    {
        CDS_NONE = 0,
        CDS_UPDATEREGISTRY = 0x00000001,
        CDS_TEST = 0x00000002,
        CDS_FULLSCREEN = 0x00000004,
        CDS_GLOBAL = 0x00000008,
        CDS_SET_PRIMARY = 0x00000010,
        CDS_VIDEOPARAMETERS = 0x00000020,
        CDS_ENABLE_UNSAFE_MODES = 0x00000100,
        CDS_DISABLE_UNSAFE_MODES = 0x00000200,
        CDS_RESET = 0x40000000,
        CDS_RESET_EX = 0x20000000,
        CDS_NORESET = 0x10000000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        public int x;
        public int y;
    }

}
