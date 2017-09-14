using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace TerraViewer
{
    class PointerRay
    {
        TriangleList triangles = new TriangleList();

        bool dirty = true;
        public void Draw (RenderContext11 renderContext, float opacity, SysColor color, TriangleList.CullMode mode = TriangleList.CullMode.None)
        {
            if (dirty)
            {
                double twoPi = Math.PI * 2;
                double step = twoPi / 45;
                double rad = .01;

                for (double a = 0; a < twoPi; a += step)
                {
                    double b = a + Math.PI / 3;
                    Vector3d pnt1 = new Vector3d(Math.Cos(a) * rad, Math.Sin(a) * rad, 0);
                    Vector3d pnt2 = new Vector3d(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, 0);

                    Vector3d pnt3 = new Vector3d(Math.Cos(a) * rad, Math.Sin(a) * rad, 10);
                    Vector3d pnt4 = new Vector3d(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, 10);

                    triangles.AddTriangle(pnt1, pnt2, pnt3, SysColor.FromArgb(128 + Math.Max(0, (int)(Math.Sin(b) * color.R / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.G / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.B / 2))), new Dates());
                    triangles.AddTriangle(pnt3, pnt2, pnt4, SysColor.FromArgb(128 + Math.Max(0, (int)(Math.Sin(b) * color.R / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.G / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.B / 2))), new Dates());
                }

                dirty = false;
                triangles.ShowFarSide = true;
                triangles.DepthBuffered = false;
                triangles.TimeSeries = false;
                triangles.WriteZbuffer = false;
            }

            triangles.Draw(renderContext, opacity, TriangleList.CullMode.CounterClockwise);
        }

        public void Clear()
        {
            triangles.Clear();
        }
    }
}
