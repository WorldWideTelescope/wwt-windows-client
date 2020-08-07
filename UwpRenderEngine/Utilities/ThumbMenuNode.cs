using System;
using System.Collections.Generic;
using System.Text;
#if WINDOWS_UWP
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
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
    class ThumbMenuNode : IThumbnail
    {

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        
        }


        Places places;

        public Places Places
        {
            get { return places; }
            set { places = value; }
        }

        Bitmap thumbnail;

        public bool IsCloudCommunityItem
        {
            get
            {
                return false;
            }
        }

        public Bitmap ThumbNail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                thumbnail = value;
            }
        }
        Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        public bool IsImage
        {
            get { return false; }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return true; }
        }

        bool readOnly = true;

        public bool ReadOnly
        {
            get { return readOnly; }
            set { ReadOnly = value; }
        }

        List<object> children = new List<object>();

        public void AddChild(object child)
        {
            children.Add(child);
        }

        public void AddRange(IEnumerable<object> range)
        {
            children.AddRange(range);
        }

        public object[] Children
        {
            get { return children.ToArray(); }
        }
    }
}
