using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
#else
using SysColor = System.Drawing.Color;
using System.Windows.Forms;
using System.Drawing;
#endif

namespace TerraViewer
{
    public class RingMenu
    {
        List<RingMenuPanel> panels = new List<RingMenuPanel>();

        RingMenuPanel activePanel = null;


        public void AddPanel( RingMenuPanel panel)
        {
            panels.Add(panel);
            activePanel = panel;
        }

        public void SetActivePanel(int index)
        {
            activePanel = panels[index];
        }

        public void Initialize()
        {
#if WINDOWS_UWP
                    string path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "UwpRenderEngine\\Resources\\RingMenu\\collections.png");
#else
            string path = "hi";
#endif
           // panels.Add(new Collections(path));

        }

        bool draggging = false;

        Point cursorLocation = new Point(0, 0);
        Vector2d pointTouched = new Vector2d();

        public void HandleControlerInput(HandController controller)
        {
            if (controller.Events.HasFlag(HandControllerStatus.TouchDown))
            {
 //               Point pnt = new Point((int)((controller.TouchX + 1) * 200), (int)((-controller.TouchY + 1) * 250));
                //activePanel.MouseClick(this, new MouseEventArgs(MouseButtons.Left, 0, cursorLocation.X,cursorLocation.Y, 0));
                activePanel.Select();
            }

            if (controller.Events.HasFlag(HandControllerStatus.Touched))
            {
                pointTouched = new Vector2d(controller.TouchX, controller.TouchY);
                draggging = true;
            }

            if (controller.Events.HasFlag(HandControllerStatus.UnTouch))
            {
                draggging = false;
            }

            if (controller.Touched)
            {
                double deltaX = controller.TouchX - pointTouched.X;
                double deltaY = controller.TouchY - pointTouched.Y;

                int x = cursorLocation.X + (int)(deltaX * touchFactor*5);
                int y = cursorLocation.Y - (int)(deltaY * touchFactor*5);

                x = Math.Min(x, 400);
                y = Math.Min(y, 500);

                x = Math.Max(0, x);
                y = Math.Max(0, y);

                cursorLocation = new Point(x,y);

                //activePanel.MouseMove(this, new MouseEventArgs(MouseButtons.Left, 0, cursorLocation.X, cursorLocation.Y, 0));
                double minStep = .3;
                if (Math.Abs(deltaX) > minStep || Math.Abs(deltaY) > minStep)
                {
                    int leftRight = (int)(deltaX * 1.25 / minStep);
                    int upDown = -(int)(deltaY * 1.25 / minStep);

                    activePanel.Navigate(upDown, leftRight);
                    pointTouched = new Vector2d(controller.TouchX, controller.TouchY);
                }
   
            }
     
        }

        const double touchFactor = 25;

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
            //foreach(var p in panels)
            {
                UiGraphics g = new UiGraphics(renderContext);
                g.Prep();
                activePanel.Draw(g);
                g.Flush();
            }
        }

        public void Clear()
        {
            triangles.Clear();
        }
    }

    public class RingMenuPanel
    {
 
        virtual public void Draw(UiGraphics g)
        {

        }

        virtual public void MouseClick(object ringMenu, MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        virtual public void MouseMove(object ringMenu, MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        virtual public void Select()
        {

        }

        virtual public void Navigate(int upDown, int leftRight)
        {

        }
    }

    //public class Collections : RingMenuPanel
    //{
    //    Texture11 image;
    //    public Collections(string imageFile)
    //    {
    //        image = Texture11.FromFile(imageFile);
    //    }

    //    public override void Draw(RenderContext11 renderContext)
    //    {
    //        if (image.Texture != null)
    //        {
    //            PositionColoredTextured[] points = new PositionColoredTextured[4];

    //            points[0] = new PositionColoredTextured(0, 0, 0, SysColor.White, 1, 0, 1);
    //            points[3] = new PositionColoredTextured(.2f, .24f, 0, SysColor.White, 1, 1, 0);
    //            points[2] = new PositionColoredTextured(0, .24f, 0, SysColor.White, 1, 0, 0);
    //            points[1] = new PositionColoredTextured(.2f, 0, 0, SysColor.White, 1, 1, 1);
    //            Sprite2d.Draw(renderContext, points, 4, image, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);

    //            Text3dBatch batch = new Text3dBatch(12);
    //            Text3d text = new Text3d(new Vector3d(0, 0, 1), new Vector3d(.1, .11, 0), new Vector3d(0, 1, 0), RenderEngine.pointerConstellation, 12, .001);
    //            //text.Rotation = 90;
    //            //text.Bank = 90;
    //            batch.Add(text);
    //            batch.Draw(renderContext, 1, SysColor.Red);
    //        }
    //    }
    //}

    //public class FolderViewer : RingMenuPanel
    //{
    //    List<IThumbnail> items = new List<IThumbnail>();
    //    public FolderViewer (IThumbnail folder)
    //    {

    //    }
    //    public override void Draw(RenderContext11 renderContext)
    //    {
            
    //    }
    //}

   
}

