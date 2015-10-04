using System;using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MIDI
{

    public enum MidiMessage { None, NoteOn, NoteOff, AfterTouch, PitchWheel, Control };
    public delegate void MidiMessageReceived(object sender, MidiMessage message, int channel, int key, int value);

    public class MidiInput
    {
        private readonly NativeMethods.MidiInProc midiInProc;
        private IntPtr handle;
        
        public MidiInput()
        {
            midiInProc = MidiProc;
            handle = IntPtr.Zero;
        }

        public static int Count
        {
            get { return NativeMethods.midiInGetNumDevs(); }
        }

        public static int GetDeviceIdByName(string name)
        {
           
            var count = Count;

            for (var i = 0; i < count; i++)
            {
                if (GetDeviceName(i) == name)
                {
                    return i;
                }
            }

            return -1;
        }

        public static string[] GetDeviceList()
        {
            var list = new List<string>();

            var count = Count;

            for (var i = 0; i < count; i++)
            {
                list.Add(GetDeviceName(i));
            }

            return list.ToArray();
        }

        public static string GetDeviceName(int id)
        {
            var caps = new MidiInCaps();
            caps.Name = new char[32];
            var result = NativeMethods.midiInGetDevCaps(id, ref caps,
            (uint)Marshal.SizeOf(caps));
            var len = 0;
            for (var i = 0; i < 32; i++)
            {
                if (caps.Name[i] == 0)
                {
                    len = i;
                    break;
                }
            }


            return new string(caps.Name, 0, len);
        }
        public bool Close()
        {
            var result = NativeMethods.midiInClose(handle)
                == NativeMethods.MMSYSERR_NOERROR;
            handle = IntPtr.Zero;
            return result;
        }

        public bool Open(int id)
        {
            return NativeMethods.midiInOpen(
                out handle,
                id,
                midiInProc,
                IntPtr.Zero,
                NativeMethods.CALLBACK_FUNCTION)
                    == NativeMethods.MMSYSERR_NOERROR;
        }

        public bool Start()
        {
            return NativeMethods.midiInStart(handle) == NativeMethods.MMSYSERR_NOERROR;
        }

        public bool Stop()
        {
            return NativeMethods.midiInStop(handle) == NativeMethods.MMSYSERR_NOERROR;
        }

        public event MidiMessageReceived MessageReceived;

        private void MidiProc(IntPtr hMidiIn, int wMsg, IntPtr dwInstance, int dwParam1, int dwParam2)
        {
            if (wMsg == 0x03c3)
            {
                // Data Message
                var status = (byte)(dwParam1 & 255);
                var key = (byte)((dwParam1 >> 8) & 255);
                var data = (byte)((dwParam1 >> 16) & 255);


                Debug.WriteLine(string.Format("Status:{0}, Key:{1}, Value:{2}", status, key, data));
                var channel = status & 15;
                var message = MidiMessage.None;
                switch (status >> 4)
                {
                    case 8: // Note off
                        message = MidiMessage.NoteOff;
                        break;
                    case 9: // Note on
                        if (data == 0)
                        {
                            message = MidiMessage.NoteOff;
                        }
                        else
                        {
                            message = MidiMessage.NoteOn;
                        }
                        break;
                    case 10: // Aftertouch
                        message = MidiMessage.AfterTouch;
                        break;
                    case 11: // Coontrol
                        message = MidiMessage.Control;
                        break;
                    default:
                        return;
                }

                if (MessageReceived != null)
                {
                    MessageReceived.Invoke(this, message, channel, key, data);
                }
            }
            else
            {
                var x = wMsg;
            }
        }
    }

    public class MidiOutput
    {
        public static string GetDeviceName(int id)
        {
            var caps = new MidiOutCaps();
            caps.Name = new char[32];
            var result = NativeMethods.midiOutGetDevCaps(id, ref caps,
            (uint)Marshal.SizeOf(caps));
            var len = 0;
            for (var i = 0; i < 32; i++)
            {
                if (caps.Name[i] == 0)
                {
                    len = i;
                    break;
                }
            }
            return new string(caps.Name, 0, len);
        }

        public static int GetDeviceIdByName(string name)
        {

            var count = Count;

            for (var i = 0; i < count; i++)
            {
                if (GetDeviceName(i) == name)
                {
                    return i;
                }
            }

            return -1;
        }

        private IntPtr handle;

        public MidiOutput()
        {
       
            handle = IntPtr.Zero;
        }

        public static int Count
        {
            get { return NativeMethods.midiOutGetNumDevs(); }
        }

        public bool Close()
        {
            var result = NativeMethods.midiOutClose(handle)
                == NativeMethods.MMSYSERR_NOERROR;
            handle = IntPtr.Zero;
            return result;
        }

        public bool Open(int id)
        {
            return NativeMethods.midiOutOpen(
                out handle,
                id,
                null,
                IntPtr.Zero,
                0)
                    == NativeMethods.MMSYSERR_NOERROR;
        }

        public void SendMessageShort(byte cmd, byte key, byte val)
        {
            NativeMethods.midiOutShortMsg(handle, cmd | (key << 8) | (val << 16));
        }

      
    }


    internal struct MidiOutCaps
    {
        ushort Mid;
        ushort Pid;
        uint driverVersion;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Name;
        public int Formats;
        public short Channels;
        public short Reserved1;
    }

    internal struct MidiInCaps
    {
        ushort Mid;
        ushort Pid;
        uint driverVersion;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] Name;
        public int Support;
    }


    internal static class NativeMethods
    {
        internal const int MMSYSERR_NOERROR = 0;
        internal const int CALLBACK_FUNCTION = 0x00030000;

        internal delegate void MidiInProc(
            IntPtr hMidiIn,
            int wMsg,
            IntPtr dwInstance,
            int dwParam1,
            int dwParam2);

        [DllImport("winmm.dll")]
        internal static extern int midiInGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern int midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        internal static extern int midiInClose(
            IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        internal static extern int midiOutClose(
            IntPtr hMidiOut);

        [DllImport("winmm.dll")]
        internal static extern int midiInOpen(
            out IntPtr lphMidiIn,
            int uDeviceID,
            MidiInProc dwCallback,
            IntPtr dwCallbackInstance,
            int dwFlags);

        [DllImport("winmm.dll")]
        internal static extern int midiInStart(
            IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        internal static extern int midiInStop(
            IntPtr hMidiIn);

        [DllImport("winmm.dll")]
        internal static extern int midiOutShortMsg(
            IntPtr hMidiIn, int dwMsg);

        [DllImport("winmm.dll")]
        internal static extern int midiOutOpen(
            out IntPtr lphMidiOut,
            int uDeviceID,
            MidiInProc dwCallback,
            IntPtr dwCallbackInstance,
            int dwFlags); 
        
        
        [DllImport("winmm.dll")]
        internal static extern int midiOutGetDevCaps(
            int uDeviceID,
            ref MidiOutCaps caps,
            uint cbMidiOutCaps);

        [DllImport("winmm.dll")]
        internal static extern int midiInGetDevCaps(
            int uDeviceID,
            ref MidiInCaps caps,
            uint cbMidiInCaps);
    }
}
