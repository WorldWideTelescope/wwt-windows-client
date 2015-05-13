using System;
using System.Drawing;
namespace TerraViewer
{
    public interface IThumbnail
    {
        string Name { get; }
        System.Drawing.Bitmap ThumbNail { get; set; }
        Rectangle Bounds { get; set;}
        bool IsImage { get;}
        bool IsTour { get;}
        bool IsFolder { get;}
        bool IsCloudCommunityItem { get; }
        bool ReadOnly { get;}
        object[] Children { get;}
    }
}
