using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    class Utils
    {
        static bool logging = false;

        public static bool Logging
        {
            get { return logging; }
            set
            {
                if (logging != value)
                {
                    logging = value;

                    if (logFilestream != null)
                    {
                        logFilestream.Close();
                        logFilestream = null;
                    }

                    if (logging)
                    {
                        FrameNumber = masterSyncFrameNumber;
                        logFilestream = new StreamWriter("C:\\wwtconfig\\wwtrenderlog.txt");
                        logFilestream.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}", "Frame Number", "Master Frame", "Render Time", "Tiles Loaded", "Textures", "Garbage Collections", "Memory", "Status Report");
                    }
                }
            }
        }

        private static System.Threading.Mutex logMutex = new System.Threading.Mutex();
        public static void WriteLogMessage(string message)
        {
            if (logging)
            {
                long ticks = HiResTimer.TickCount - lastRender;

                int ms = (int)((ticks * 1000) / HiResTimer.Frequency);

                logMutex.WaitOne();
                logFilestream.WriteLine("{0}\t{1}\t{2}\t{3}", frameNumber, masterSyncFrameNumber, ms, message);
                logMutex.ReleaseMutex();
            }
        }

        public static StreamWriter logFilestream = null;

        public static int frameNumber = 0;

        public static int FrameNumber
        {
            get { return frameNumber; }
            set { frameNumber = value; }
        }

        public static int masterSyncFrameNumber = 0;
        public static long lastRender = HiResTimer.TickCount;
    }
}
