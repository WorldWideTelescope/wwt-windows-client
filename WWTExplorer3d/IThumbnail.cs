using System;

namespace TerraViewer
{
    public interface IThumbnail
    {
        string Name { get; }
#if WINDOWS_UWP
        Bitmap ThumbNail { get; set; }
        Rectangle Bounds { get; set; }
#else
        System.Drawing.Bitmap ThumbNail { get; set; }
        System.Drawing.Rectangle Bounds { get; set;}
#endif

        bool IsImage { get;}
        bool IsTour { get;}
        bool IsFolder { get;}
        bool IsCloudCommunityItem { get; }
        bool ReadOnly { get;}
        object[] Children { get;}
    }
}
