using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class VideoOutputType
    {
        public string Name;
        public double Fps;
        public int Width;
        public int Height;
        public bool Dome;
        public int TotalFrames = 0;
        public int StartFrame = 0;
        public bool WaitDownload = false;
        public VideoOutputType()
        {

        }

        public VideoOutputType(string name, int width, int height, double fps, bool dome)
        {
            Name = name;
            Width = width;
            Height = height;
            Fps = fps;
            Dome = dome;
        }
        public override string ToString()
        {
            return string.Format("{0} ({1} x {2} : {3}{4})", Name, Width, Height, Fps, Language.GetLocalizedText(886, "FPS"));
        }
    }
}
