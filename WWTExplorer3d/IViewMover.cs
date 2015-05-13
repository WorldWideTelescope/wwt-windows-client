using System;
namespace TerraViewer
{
    public interface IViewMover
    {
        bool Complete { get; }
        CameraParameters CurrentPosition { get; }
        DateTime CurrentDateTime { get; }
        event EventHandler Midpoint;
        double MoveTime { get;}
     }
}
