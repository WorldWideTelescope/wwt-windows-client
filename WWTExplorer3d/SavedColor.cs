using System;
#if WINDOWS_UWP
using Color = Windows.UI.Color;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif


namespace TerraViewer
{
    class SavedColor 
    {
        public Color Color;

        public SavedColor()
        {
            Color = SystemColors.White;
        }

        public SavedColor(int argb)
        {
            Color = UiTools.FromArgb(argb);
        }

        public enum ColorFormat
        {
            NamedColor,
            ARGBColor
        }

        public string Save()
        {
            return SavedColor.Save(this);
        }

        public void LoadInstance(string val)
        {
            this.Color = SavedColor.Load(val);
        }

        public static implicit operator Color(SavedColor saved)
        {
            return saved.Color;
        }

        public static string Save(Color color)
        {
#if !WINDOWS_UWP
            if (color.IsNamedColor)
                return string.Format("{0}:{1}",
                    ColorFormat.NamedColor, color.Name);
            else
#endif
                return string.Format("{0}:{1}:{2}:{3}:{4}",
                    ColorFormat.ARGBColor,
                    color.A, color.R, color.G, color.B);
        }

        public static Color Load(string color)
        {
            byte a, r, g, b;

            string[] pieces = color.Split(new char[] { ':' });

            ColorFormat colorType = (ColorFormat)
                Enum.Parse(typeof(ColorFormat), pieces[0], true);

            switch (colorType)
            {
                case ColorFormat.NamedColor:
#if !WINDOWS_UWP
                    return Color.FromName(pieces[1]);
#else
                    //todo uwp fix this! Really bad.
                    return SystemColors.Red;
#endif
                case ColorFormat.ARGBColor:
                    a = byte.Parse(pieces[1]);
                    r = byte.Parse(pieces[2]);
                    g = byte.Parse(pieces[3]);
                    b = byte.Parse(pieces[4]);

                    return Color.FromArgb(a, r, g, b);
            }
            return Color.FromArgb(0,0,0,0);
        }
    }
}
