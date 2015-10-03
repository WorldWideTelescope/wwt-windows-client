using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

// Drawn heavily from Emma Burrows codeproject sample
// which closely follows the C++ MS WM_INPUT sample.

namespace SpaceNavigator
{
    public sealed class _3DxMouse
    {
        #region const definitions

        private const int RIDEV_INPUTSINK = 0x00000100;
        private const int RID_HEADER = 0x10000005;
        private const int RID_INPUT = 0x10000003;

        private const int HID_USAGE_GENERIC_MULTIAXIS_CONTROLLER = 0x08;
        private const int HID_USAGE_PAGE_GENERIC = 0x01;
        private const int LOGITECH_3DX_VID = 0x046d;
        private const int SIZEOF_STANDARD_REPORT = 15;

        private const int FAPPCOMMAND_MASK = 0xF000;
        private const int FAPPCOMMAND_MOUSE = 0x8000;
        private const int FAPPCOMMAND_OEM = 0x1000;

        private const int RIM_TYPEHID = 2;

        private const int RIDI_DEVICENAME = 0x20000007;  // return length is the character length not the byte size
        private const int RIDI_DEVICEINFO = 0x2000000b;

        private const int WM_INPUT = 0x00FF;


        #endregion const definitions

        #region structs & enums

        public enum RAW3DxMouseEventType
        {
            TranslationVector = 1,
            RotationVector = 2,
            ButtonReport = 3
        }

        #region Windows.h structure declarations
        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTHEADER
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwSize;
            public IntPtr hDevice;
            public int wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWHID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizHid;
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RID_DEVICE_INFO_HID
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int dwType;
            [MarshalAs(UnmanagedType.U4)]
            public int dwVendorID;
            [MarshalAs(UnmanagedType.U4)]
            public int dwProductID;
            [MarshalAs(UnmanagedType.U4)]
            public int dwVersionNumber;
            [MarshalAs(UnmanagedType.U2)]
            public short usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public short usUsage;
            [MarshalAs(UnmanagedType.U4)]  // padding to get the size right
            public int unused1;
            [MarshalAs(UnmanagedType.U4)]
            public int unused2;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct BUTTONSSTR
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonFlags;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usButtonData;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAW3DMOUSE_EVENTTYPE
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid; // byte size of each report
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount; // number of input packed
            [MarshalAs(UnmanagedType.U1)]
            public byte eventType;  // 1 for translation vector, 2 for rot vector, 3 for buttons
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAW3DMOUSEMOTION_T
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid; // byte size of each report
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount; // number of input packed
            [MarshalAs(UnmanagedType.U1)]
            public byte eventType;  // 1 for translation vector
            [MarshalAs(UnmanagedType.U1)]
            public byte X_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte X_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_hb;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAW3DMOUSEMOTION_TR_COMBINED
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid; // byte size of each report
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount; // number of input packed
            [MarshalAs(UnmanagedType.U1)]
            public byte eventType;  // 1 for translation vector
            [MarshalAs(UnmanagedType.U1)]
            public byte X_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte X_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RX_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RX_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RY_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RY_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RZ_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte RZ_hb;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAW3DMOUSEMOTION_R
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid; // byte size of each report
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount; // number of input packed
            [MarshalAs(UnmanagedType.U1)]
            public byte eventType;  // 2 for rotation vector
            public byte X_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte X_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Y_hb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_lb;
            [MarshalAs(UnmanagedType.U1)]
            public byte Z_hb;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAW3DMOUSEBUTTONS
        {
            [MarshalAs(UnmanagedType.U4)]
            public int dwSizeHid; // byte size of each report
            [MarshalAs(UnmanagedType.U4)]
            public int dwCount; // number of input packed
            [MarshalAs(UnmanagedType.U1)]
            public byte eventType; // 3 for buttons report
            [MarshalAs(UnmanagedType.U1)]
            public byte b1;
            [MarshalAs(UnmanagedType.U1)]
            public byte b2;
            [MarshalAs(UnmanagedType.U1)]
            public byte b3;
            [MarshalAs(UnmanagedType.U1)]
            public byte b4;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RAWINPUTDEVICE
        {
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsagePage;
            [MarshalAs(UnmanagedType.U2)]
            public ushort usUsage;
            [MarshalAs(UnmanagedType.U4)]
            public int dwFlags;
            public IntPtr hwndTarget;
        }
        #endregion Windows.h structure declarations

        /// <summary>
        /// Class encapsulating the information about a
        /// keyboard event, including the device it
        /// originated with and what key was pressed
        /// </summary>
        public class DeviceInfo
        {
            public string deviceName;
            public IntPtr deviceHandle;
        }

        /// <summary>
        /// ButtonMask from device
        /// </summary>
        public class ButtonMask
        {
            public ButtonMask(byte b1, byte b2, byte b3, byte b4)
            {
                this.mask = (System.UInt32)(b1 + (b2 << 8) + (b3 << 16) + (b4 << 24));
            }

            public UInt32 Mask
            {
                get { return mask; }
                set { mask = value; }
            }

            public UInt32 Pressed
            {
                get { return mask; }
            }

            private System.UInt32 mask;
        }

        /// <summary>
        /// Translation Vector from device
        /// </summary>
        public class TranslationVector
        {
            public TranslationVector(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public TranslationVector(byte xl, byte xh, byte yl, byte yh, byte zl, byte zh)
            {
                this.x = (int)(xl + (System.Int16)((System.Int16)xh << 8));
                this.y = (int)(yl + (System.Int16)((System.Int16)yh << 8));
                this.z = (int)(zl + (System.Int16)((System.Int16)zh << 8));
            }

            public int X
            {
                get { return x; }
                set { x = value; }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }
            public int Z
            {
                get { return z; }
                set { z = value; }
            }

            private int x;
            private int y;
            private int z;
        }

        /// <summary>
        /// Rotation Vector from device
        /// </summary>
        public class RotationVector
        {
            public RotationVector(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public RotationVector(byte xl, byte xh, byte yl, byte yh, byte zl, byte zh)
            {
                this.x = (int)(xl + (System.Int16)((System.Int16)xh << 8));
                this.y = (int)(yl + (System.Int16)((System.Int16)yh << 8));
                this.z = (int)(zl + (System.Int16)((System.Int16)zh << 8));
            }

            public int X
            {
                get { return x; }
                set { x = value; }
            }
            public int Y
            {
                get { return y; }
                set { y = value; }
            }
            public int Z
            {
                get { return z; }
                set { z = value; }
            }

            private int x;
            private int y;
            private int z;

        }

        #endregion structs & enums

        #region DllImports

        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("User32.dll")]
        extern static bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevice, uint uiNumDevices, uint cbSize);

        [DllImport("User32.dll")]
        extern static uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        #endregion DllImports

        #region Variables and event handling

        /// <summary>
        /// List of 3Dx devices
        /// Key: the device handle
        /// Value: the device info class
        /// </summary>
        public Hashtable deviceList = new Hashtable();

        //Event and delegate
        public delegate void MotionEventHandler(object sender, MotionEventArgs e);
        public event MotionEventHandler MotionEvent;
        public delegate void ButtonEventHandler(object sender, ButtonEventArgs e);
        public event ButtonEventHandler ButtonEvent;

        /// <summary>
        /// Arguments provided by the handler for a motion event
        /// </summary>
        public class MotionEventArgs : EventArgs
        {
            private DeviceInfo m_deviceInfo;
            private TranslationVector m_translationVector;
            private RotationVector m_rotationVector;

            public MotionEventArgs(DeviceInfo dInfo, TranslationVector translationVector, RotationVector rotationVector)
            {
                m_deviceInfo = dInfo;
                m_translationVector = translationVector;
                m_rotationVector = rotationVector;
            }

            public MotionEventArgs(DeviceInfo dInfo, TranslationVector translationVector)
            {
                m_deviceInfo = dInfo;
                m_translationVector = translationVector;
            }

            public MotionEventArgs(DeviceInfo dInfo, RotationVector rotationVector)
            {
                m_deviceInfo = dInfo;
                m_rotationVector = rotationVector;
            }

            public MotionEventArgs()
            {
            }

            public TranslationVector TranslationVector
            {
                get { return m_translationVector; }
                set { m_translationVector = value; }
            }

            public RotationVector RotationVector
            {
                get { return m_rotationVector; }
                set { m_rotationVector = value; }
            }

            public DeviceInfo DeviceInfo
            {
                get { return m_deviceInfo; }
                set { m_deviceInfo = value; }
            }
        }

        /// <summary>
        /// Arguments provided by the handler for a button event
        /// </summary>
        public class ButtonEventArgs : EventArgs
        {
            private DeviceInfo m_deviceInfo;
            private ButtonMask m_buttonMask;

            public ButtonEventArgs(DeviceInfo dInfo, ButtonMask buttonMask)
            {
                m_deviceInfo = dInfo;
                m_buttonMask = buttonMask;
            }

            public ButtonEventArgs()
            {
            }

            public ButtonMask ButtonMask
            {
                get { return m_buttonMask; }
                set { m_buttonMask = value; }
            }

            public DeviceInfo DeviceInfo
            {
                get { return m_deviceInfo; }
                set { m_deviceInfo = value; }
            }
        }

        #endregion Variables and event handling

        #region _3DxMouse( IntPtr hwnd )

        /// <summary>
        /// InputDevice constructor; registers the raw input devices
        /// for the calling window.
        /// </summary>
        /// <param name="hwnd">Handle of the window listening for key presses</param>
        public _3DxMouse(IntPtr hwnd)
        {
            var rid = new RAWINPUTDEVICE[1];

            rid[0].usUsagePage = HID_USAGE_PAGE_GENERIC;
            rid[0].usUsage = HID_USAGE_GENERIC_MULTIAXIS_CONTROLLER;
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = hwnd;

            // Don't register each physical device, just the type (MultiAxis Controller)
            if (!RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(rid[0])))
            {
                throw new ApplicationException("Failed to register raw input device(s).");
            }
        }

        #endregion _3DxMouse( IntPtr hwnd )

        #region int EnumerateDevices()

        /// <summary>
        /// Iterates through the list provided by GetRawInputDeviceList,
        /// counting 3Dx devices and adding them to deviceList.
        /// </summary>
        /// <returns>The number of 3Dx devices found.</returns>
        public int EnumerateDevices()
        {

            var NumberOfDevices = 0;
            uint deviceCount = 0;
            var dwSize = (Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

            if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                var pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                for (var i = 0; i < deviceCount; i++)
                {
                    uint pcbSize = 0;

                    var rid = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(
                                               new IntPtr((pRawInputDeviceList.ToInt32() + (dwSize * i))),
                                               typeof(RAWINPUTDEVICELIST));

                    GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize > 0)
                    {
                        var pData = Marshal.AllocHGlobal((int)pcbSize);
                        GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICENAME, pData, ref pcbSize);
                        var deviceName = Marshal.PtrToStringAnsi(pData);

                        //If the device is identified as a HID device with usagePage 1, usage 8,
                        //create a DeviceInfo object to store information about it
                        if (rid.dwType == RIM_TYPEHID)
                        {
                            var dInfo = new DeviceInfo();

                            // Create an object to set the size in the struct.  Then marshall it to a ptr to pass to RI.
                            var hidInfo = new RID_DEVICE_INFO_HID();
                            var dwHidInfoSize = Marshal.SizeOf(hidInfo);
                            hidInfo.cbSize = dwHidInfoSize;
                            var pHIDData = Marshal.AllocHGlobal(dwHidInfoSize);
                            Marshal.StructureToPtr(hidInfo, pHIDData, true);

                            var uHidInfoSize = (uint)dwHidInfoSize;

                            GetRawInputDeviceInfo(rid.hDevice, RIDI_DEVICEINFO, pHIDData, ref uHidInfoSize);

                            // Marshal back to an object to retrieve info
                            hidInfo = (RID_DEVICE_INFO_HID)Marshal.PtrToStructure(pHIDData, typeof(RID_DEVICE_INFO_HID));

                            dInfo.deviceName = Marshal.PtrToStringAnsi(pData);
                            dInfo.deviceHandle = rid.hDevice;


                            //If it is a 3Dx device and it isn't already in the list,
                            //add it to the deviceList hashtable and increase the
                            //NumberOfDevices count
                            if (Is3DxDevice(hidInfo) && !deviceList.Contains(rid.hDevice))
                            {
                                Console.WriteLine("Using 3Dx device: " + deviceName);
                                NumberOfDevices++;
                                deviceList.Add(rid.hDevice, dInfo);
                            }
                            Marshal.FreeHGlobal(pHIDData);
                        }
                        Marshal.FreeHGlobal(pData);
                    }
                }

                Marshal.FreeHGlobal(pRawInputDeviceList);
                return NumberOfDevices;
            }
            else
            {
                throw new ApplicationException("Error!");
            }
        }

        private bool Is3DxDevice(RID_DEVICE_INFO_HID hidInfo)
        {
            return hidInfo.dwVendorID == LOGITECH_3DX_VID &&
                hidInfo.usUsagePage == HID_USAGE_PAGE_GENERIC &&
                hidInfo.usUsage == HID_USAGE_GENERIC_MULTIAXIS_CONTROLLER;
        }

        #endregion EnumerateDevices()

        #region ProcessInputCommand( Message message )

        /// <summary>
        /// Processes WM_INPUT messages to retrieve information about any
        /// keyboard events that occur.
        /// </summary>
        /// <param name="message">The WM_INPUT message to process.</param>
        public void ProcessInputCommand(Message message)
        {
            uint dwSize = 0;

            // First call to GetRawInputData sets the value of dwSize
            // dwSize can then be used to allocate the appropriate amount of memory,
            // storing the pointer in "buffer".
            GetRawInputData(message.LParam,
                             RID_HEADER, IntPtr.Zero,
                             ref dwSize,
                             (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER)));

            var headerBuffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {
                // Check that buffer points to something, and if so,
                // call GetRawInputData again to fill the allocated memory
                // with information about the input
                if (headerBuffer != IntPtr.Zero &&
                    GetRawInputData(message.LParam,
                                     RID_HEADER,
                                     headerBuffer,
                                     ref dwSize,
                                     (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == dwSize)
                {

                    var header = (RAWINPUTHEADER)Marshal.PtrToStructure(headerBuffer, typeof(RAWINPUTHEADER));
                    if (header.dwType == RIM_TYPEHID)
                    {
                        DeviceInfo dInfo = null;

                        if (deviceList.Contains(header.hDevice))
                        {
                            dInfo = (DeviceInfo)deviceList[header.hDevice];
                        }
                        else
                        {
                            // Device not in list.  Reenumerate all of them again.  Could warn the code with some sort of Connect/Disconnect event.
                            EnumerateDevices();
                            dInfo = (DeviceInfo)deviceList[header.hDevice];
                        }

                        // The header tells us the size of the actual event
                        var eventBuffer = Marshal.AllocHGlobal(header.dwSize);

                        var eventSize = (uint)header.dwSize;
                        if (eventBuffer != IntPtr.Zero &&
                            GetRawInputData(message.LParam,
                                            RID_INPUT,
                                            eventBuffer,
                                            ref eventSize,
                                            (uint)Marshal.SizeOf(typeof(RAWINPUTHEADER))) == header.dwSize)
                        {
                            var eventType = (RAW3DMOUSE_EVENTTYPE)Marshal.PtrToStructure(new IntPtr(eventBuffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER))), typeof(RAW3DMOUSE_EVENTTYPE));
                            switch (eventType.eventType)
                            {
                                case (byte)RAW3DxMouseEventType.TranslationVector:
                                    if (header.dwSize == Marshal.SizeOf(typeof(RAWINPUTHEADER)) + SIZEOF_STANDARD_REPORT)  // standard length T report
                                    {
                                        var t = (RAW3DMOUSEMOTION_T)Marshal.PtrToStructure(new IntPtr(eventBuffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER))), typeof(RAW3DMOUSEMOTION_T));
                                        var tv = new TranslationVector(t.X_lb, t.X_hb, t.Y_lb, t.Y_hb, t.Z_lb, t.Z_hb);
                                        // Console.Write("Motion Event = {0} {1} {2}", tv.X, tv.Y, tv.Z);
                                        MotionEvent(this, new MotionEventArgs(dInfo, tv));
                                    }
                                    else // "High Speed" firmware version includes both T and R vector in the same report
                                    {
                                        var tr = (RAW3DMOUSEMOTION_TR_COMBINED)Marshal.PtrToStructure(new IntPtr(eventBuffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER))), typeof(RAW3DMOUSEMOTION_TR_COMBINED));
                                        var tv = new TranslationVector(tr.X_lb, tr.X_hb, tr.Y_lb, tr.Y_hb, tr.Z_lb, tr.Z_hb);
                                        var rv = new RotationVector(tr.RX_lb, tr.RX_hb, tr.RY_lb, tr.RY_hb, tr.RZ_lb, tr.RZ_hb);
                                        // Console.WriteLine("6DOF Motion Event = {0} {1} {2} {3} {4} {5}", tv.X, tv.Y, tv.Z, rv.X, rv.Y, rv.Z);
                                        MotionEvent(this, new MotionEventArgs(dInfo, tv, rv));
                                    }
                                    break;

                                case (byte)RAW3DxMouseEventType.RotationVector:
                                    {
                                        var r = (RAW3DMOUSEMOTION_R)Marshal.PtrToStructure(new IntPtr(eventBuffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER))), typeof(RAW3DMOUSEMOTION_R));
                                        var rv = new RotationVector(r.X_lb, r.X_hb, r.Y_lb, r.Y_hb, r.Z_lb, r.Z_hb);
                                        // Console.WriteLine(" {0} {1} {2}", rv.X, rv.Y, rv.Z);
                                        MotionEvent(this, new MotionEventArgs(dInfo, rv));
                                    }
                                    break;

                                case (byte)RAW3DxMouseEventType.ButtonReport:
                                    var b = (RAW3DMOUSEBUTTONS)Marshal.PtrToStructure(new IntPtr(eventBuffer.ToInt32() + Marshal.SizeOf(typeof(RAWINPUTHEADER))), typeof(RAW3DMOUSEBUTTONS));
                                    var bm = new ButtonMask(b.b1, b.b2, b.b3, b.b4);
                                    Console.WriteLine("raw.buttons = {0:X}", bm.Pressed);
                                    ButtonEvent(this, new ButtonEventArgs(dInfo, bm));
                                    break;
                            }
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(headerBuffer);
            }
        }

        #endregion ProcessInputCommand( Message message )

        #region ProcessMessage(Message message)

        /// <summary>
        /// Filters Windows messages for WM_INPUT messages and calls
        /// ProcessInputCommand if necessary.
        /// </summary>
        /// <param name="message">The Windows message.</param>
        public void ProcessMessage(Message message)
        {
            switch (message.Msg)
            {
                case WM_INPUT:
                    {
                        ProcessInputCommand(message);
                    }
                    break;
            }
        }

        #endregion ProcessMessage( Message message )
    }
}
