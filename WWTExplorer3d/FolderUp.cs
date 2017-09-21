#if !WINDOWS_UWP
using System.Drawing;
#endif

namespace TerraViewer
{
    public class FolderUp : IThumbnail
    {

        #region IThumbnail Members

        public string Name
        {
            get { return Language.GetLocalizedText(946, "Up Level"); }
        }

        public Bitmap ThumbNail
        {
            get
            {
                return Properties.Resources.FolderUp;
            }
            set
            {
                return;
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

        public bool IsCloudCommunityItem
        {
            get
            {
                return false;
            }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get { return true; }
        }

        public object[] Children
        {
            get { return null; }
        }

        #endregion
    }
}
