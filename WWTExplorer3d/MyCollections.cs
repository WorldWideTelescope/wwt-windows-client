using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace TerraViewer
{
    class MyCollections : IThumbnail
    {
        string name;
        public MyCollections(string name)
        {
            this.name = name;
        }

        #region IThumbnail Members

        public string Name
        {
            get { return name; }
        }
        Bitmap thumbnail = null;
        public System.Drawing.Bitmap ThumbNail
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
        Rectangle rect;
        public System.Drawing.Rectangle Bounds
        {
            get
            {
                return rect;
            }
            set
            {
                rect = value;
            }
        }

        public bool IsCloudCommunityItem
        {
            get
            {
                return false;
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
        List<Object> children = new List<Object>();
        public object[] Children
        {
            get { return children.ToArray(); }
        }
        public bool ReadOnly
        {
            get { return true; }
        }
        #endregion
    }
}
