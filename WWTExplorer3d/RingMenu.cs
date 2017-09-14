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
    public class RingMenu
    {
        List<RingMenuPanel> panels = new List<RingMenuPanel>();

        public void Initialize()
        {
#if WINDOWS_UWP
                    string path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "UwpRenderEngine\\Resources\\RingMenu\\collections.png");
#else
            string path = "hi";
#endif
            panels.Add(new Collections(path));

        }

        TriangleList triangles = new TriangleList();

        bool dirty = true;
        public void Draw(RenderContext11 renderContext, float opacity, SysColor color, TriangleList.CullMode mode = TriangleList.CullMode.None)
        {
            //if (dirty)
            //{
            //    double twoPi = Math.PI * 2;
            //    double step = twoPi / 45;
            //    double rad = .01;

            //    for (double a = 0; a < twoPi; a += step)
            //    {
            //        double b = a + Math.PI / 3;
            //        Vector3d pnt1 = new Vector3d(Math.Cos(a) * rad, Math.Sin(a) * rad, 0);
            //        Vector3d pnt2 = new Vector3d(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, 0);

            //        Vector3d pnt3 = new Vector3d(Math.Cos(a) * rad, Math.Sin(a) * rad, rad);
            //        Vector3d pnt4 = new Vector3d(Math.Cos(a + step) * rad, Math.Sin(a + step) * rad, rad);

            //        triangles.AddTriangle(pnt1, pnt2, pnt3, SysColor.FromArgb(128 + Math.Max(0, (int)(Math.Sin(b) * color.R / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.G / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.B / 2))), new Dates());
            //        triangles.AddTriangle(pnt3, pnt2, pnt4, SysColor.FromArgb(128 + Math.Max(0, (int)(Math.Sin(b) * color.R / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.G / 2)), 128 + Math.Max(0, (int)(Math.Sin(b) * color.B / 2))), new Dates());
            //    }

            //    dirty = false;
            //    triangles.ShowFarSide = true;
            //    triangles.DepthBuffered = true;
            //    triangles.TimeSeries = false;
            //    triangles.WriteZbuffer = false;
            //}

            //triangles.Draw(renderContext, opacity, TriangleList.CullMode.None);
            foreach(var p in panels)
            {
                p.Draw(renderContext);
            }
        }

        public void Clear()
        {
            triangles.Clear();
        }
    }

    public class RingMenuPanel
    {
 
        virtual public void Draw(RenderContext11 renderContext)
        {

        }

    }

    public class Collections : RingMenuPanel
    {
        Texture11 image;
        public Collections(string imageFile)
        {
            image = Texture11.FromFile(imageFile);
        }

        public override void Draw(RenderContext11 renderContext)
        {
            if (image.Texture != null)
            {
                PositionColoredTextured[] points = new PositionColoredTextured[4];

                points[0] = new PositionColoredTextured(0, 0, 0, SysColor.White, 1, 0, 1);
                points[3] = new PositionColoredTextured(.02f, .024f, 0, SysColor.White, 1, 1, 0);
                points[2] = new PositionColoredTextured(0, .024f, 0, SysColor.White, 1, 0, 0);
                points[1] = new PositionColoredTextured(.02f, 0, 0, SysColor.White, 1, 1, 1);
                Sprite2d.Draw(renderContext, points, 4, image, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);

                Text3dBatch batch = new Text3dBatch(12);
                Text3d text = new Text3d(new Vector3d(0, 0, 1), new Vector3d(.01, .011, 0), new Vector3d(0, 1, 0), RenderEngine.pointerConstellation, 12, .0001);
                //text.Rotation = 90;
                //text.Bank = 90;
                batch.Add(text);
                batch.Draw(renderContext, 1, SysColor.Red);
            }
        }
    }

   
}

