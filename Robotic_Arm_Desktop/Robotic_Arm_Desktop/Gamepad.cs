using System;
using System.Runtime.InteropServices;

namespace Robotic_Arm_Desktop
{
    class Gamepad
    {
        public int err_code = 0;
        private RawInputDevice[] rid;
        private IntPtr hwnd;
        public bool gamepadConnected = true;

        public Gamepad(IntPtr handler)
        {
            rid = new RawInputDevice[1];
            rid[0].UsagePage = HidUsagePage.GENERIC;
            rid[0].Usage = HidUsage.Joystick;
            rid[0].Flags = RawInputDeviceFlags.INPUTSINK;
            rid[0].Target = handler;

            if (RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])) == false)
            {
                err_code = 1;
            }

            hwnd = handler;
        }

        [DllImport("User32.dll", SetLastError = true)]
        internal static extern bool RegisterRawInputDevices(RawInputDevice[] pRawInputDevice, uint numberDevices, uint size);

        [DllImport("C:\\Users\\mt2si\\Desktop\\projekty\\S.O.C Robotic Arm\\Robotic Arm\\GamepadDLL\\x64\\Release\\GamepadDLL.dll")]
        public static extern GamepadState GamepadProcesing(IntPtr lParam);
    }



    public struct GamepadState
    {
        public int leftStickHor;
        public int leftStickVer;
        public int rightStickHor;
        public int rightStickVer;
        public int button;
        public int frontButton;
        public int mode;
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct Rawinputheader
    {
        public uint dwType;
        public uint dwSize;
        public IntPtr hDevice;
        public IntPtr wParam;

        public override string ToString()
        {
            return string.Format("RawInputHeader\n dwType : {0}\n dwSize : {1}\n hDevice : {2}\n wParam : {3}", dwType, dwSize, hDevice, wParam);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rawhid
    {
        public uint dwSizHid;
        public uint dwCount;
        public byte bRawData;

        public override string ToString()
        {
            return string.Format("Rawhib\n dwSizeHid : {0}\n dwCount : {1}\n bRawData : {2}\n", dwSizHid, dwCount, bRawData);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RawInputDevice
    {
        internal HidUsagePage UsagePage;
        internal HidUsage Usage;
        internal RawInputDeviceFlags Flags;
        internal IntPtr Target;

        public override string ToString()
        {
            return string.Format("{0}/{1}, flags: {2}, target: {3}", UsagePage, Usage, Flags, Target);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct RawData
    {
       // [FieldOffset(0)]
       // internal Rawmouse mouse;
       // [FieldOffset(0)]
       // internal Rawkeyboard keyboard;
        [FieldOffset(0)]
        internal Rawhid hid;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct InputData
    {
        public Rawinputheader header;
        public RawData data;
    }

    [Flags]
    internal enum RawInputDeviceFlags
    {
        NONE = 0,                   // No flags
        REMOVE = 0x00000001,        // Removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection. 
        EXCLUDE = 0x00000010,       // Specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with PageOnly.
        PAGEONLY = 0x00000020,      // Specifies all devices whose top level collection is from the specified UsagePage. Note that Usage must be zero. To exclude a particular top level collection, use Exclude.
        NOLEGACY = 0x00000030,      // Prevents any devices specified by UsagePage or Usage from generating legacy messages. This is only for the mouse and keyboard.
        INPUTSINK = 0x00000100,     // Enables the caller to receive the input even when the caller is not in the foreground. Note that WindowHandle must be specified.
        CAPTUREMOUSE = 0x00000200,  // Mouse button click does not activate the other window.
        NOHOTKEYS = 0x00000200,     // Application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can be specified even if NoLegacy is not specified and WindowHandle is NULL.
        APPKEYS = 0x00000400,       // Application keys are handled.  NoLegacy must be specified.  Keyboard only.

        // Enables the caller to receive input in the background only if the foreground application does not process it. 
        // In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
        EXINPUTSINK = 0x00001000,
        DEVNOTIFY = 0x00002000
    }

    public enum HidUsagePage : ushort
    {
        UNDEFINED = 0x00,   // Unknown usage page
        GENERIC = 0x01,     // Generic desktop controls
        SIMULATION = 0x02,  // Simulation controls
        VR = 0x03,          // Virtual reality controls
        SPORT = 0x04,       // Sports controls
        GAME = 0x05,        // Games controls
        KEYBOARD = 0x07,    // Keyboard controls
    }

    public enum HidUsage : ushort
    {
        Undefined = 0x00,       // Unknown usage
        Pointer = 0x01,         // Pointer
        Mouse = 0x02,           // Mouse
        Joystick = 0x04,        // Joystick
        Gamepad = 0x05,         // Game Pad
        Keyboard = 0x06,        // Keyboard
        Keypad = 0x07,          // Keypad
        SystemControl = 0x80,   // Muilt-axis Controller
        Tablet = 0x80,          // Tablet PC controls
        Consumer = 0x0C,        // Consumer
    }
}
