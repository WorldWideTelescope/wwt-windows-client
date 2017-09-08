using System.Collections.Generic;
#if WINDOWS_UWP
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
    public class ThumbnailCache
    {
        static public Dictionary<string, Bitmap> ConstellationThumbnails = new Dictionary<string, Bitmap>();
    }
}
