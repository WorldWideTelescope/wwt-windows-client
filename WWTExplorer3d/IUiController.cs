using System;
#if WINDOWS_UWP
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Windows.Forms;
using System.Drawing;
#endif
namespace TerraViewer
{
    public interface IUiController
    {
        void Render(RenderEngine renderEngine);
        void PreRender(RenderEngine renderEngine);
        bool MouseDown(object sender, MouseEventArgs e);
        bool MouseUp(object sender, MouseEventArgs e);
        bool MouseMove(object sender, MouseEventArgs e);
        bool MouseClick(object sender, MouseEventArgs e);
        bool Click(object sender, EventArgs e);
        bool MouseDoubleClick(object sender,MouseEventArgs e);
        bool KeyDown(object sender, KeyEventArgs e);
        bool KeyUp(object sender, KeyEventArgs e);
        bool Hover(Point pnt);
    }
}
