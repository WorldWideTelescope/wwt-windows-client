using System.Collections.Generic;
using System.IO;
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

        static public Bitmap GetConstellationThumbnail(string name)
        {
            if (!ConstellationThumbnails.ContainsKey(name))
            {
                ConstellationThumbnails[name] = UiTools.LoadThumbnailFromWeb(@"http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=" + name.Replace(" ", ""));
            }
                return ConstellationThumbnails[name];
        }

        static public Bitmap LoadThumbnail(string name)
        {
            Stream stream = WWTThumbnails.WWTThmbnail.GetThumbnailStream(name.Replace(" ", ""));

            if (stream== null)
            {
                return null;
            }

            return new Bitmap(stream);
        }
    }
}
