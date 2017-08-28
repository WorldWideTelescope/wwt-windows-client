using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    class ImageAlignmentUI : IUiController
    {
        #region IUiController Members


        public void PreRender(RenderEngine renderEngine)
        {
            
        }
        public void Render(RenderEngine renderEngine)
        {
            if (popup == null)
            {
                popup = new ImageAlignPopup();
                popup.Owner = Earth3d.MainWindow;
                popup.Show();
                
                
            }
            // todo Draw Achor and tanget points..
            return;
        }

        public void Close()
        {
            if (popup != null)
            {
                popup.Close();
            }
        }

        ImageAlignPopup popup = null;

        bool dragging = false;
        Point pntDown;
        bool anchored = false;
        bool mouseDown = false;
        Coordinates anchoredPoint1;
        Vector2d anchorPoint1;
        Coordinates anchoredPoint2;
        Vector2d anchorPoint2;
        double startRotation = 0;
        double startCenterY = 0;
        double startCenterX = 0;
        double startScale = 0;
        double startLength = 0;
        double startAngle = 0;
        const double RC = (double)(3.1415927 / 180);

        public bool MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Earth3d.MainWindow.StudyImageset == null)
            {
                mouseDown = false;
                return false;
            }
            Tile root = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.StudyImageset, null);
            if (root == null)
            {
                mouseDown = false;
                return false;
            }
            if (e.Button == MouseButtons.Right && (root is SkyImageTile))
            {
                anchored = !anchored;
                popup.SetPivotMode(anchored);
                if (anchored)
                {
                    anchoredPoint1 = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                    TourPlace place = new TourPlace("", anchoredPoint1.Dec, anchoredPoint1.RA, Classification.Unidentified, "UMA", ImageSetType.Sky, -1);
                    Earth3d.MainWindow.SetLabelText(place, false);
                    if (root is TangentTile)
                    {
                        TangentTile tile = (TangentTile)root;
                        Vector3d vector = tile.TransformPoint(12, 12);
                        vector = Coordinates.GeoTo3dDouble(anchoredPoint1.Lat, anchoredPoint1.Lng);
                        double x;
                        double y;
                        tile.UnTransformPoint(vector, out x, out y);
                    }
                    else if (root is SkyImageTile)
                    {
                        SkyImageTile tile = (SkyImageTile)root;
                        anchorPoint1 = tile.GetImagePixel(anchoredPoint1);
                    }
                }
                mouseDown = true;
                return true;
            }
            else if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                pntDown = e.Location;
                if (anchored)
                {
                    if (root is TangentTile)
                    {
                        startRotation = Earth3d.MainWindow.StudyImageset.Rotation;
                        startCenterX = Earth3d.MainWindow.StudyImageset.OffsetX;
                        startCenterY = Earth3d.MainWindow.StudyImageset.OffsetY;
                        startScale = Earth3d.MainWindow.StudyImageset.BaseTileDegrees;
                        Coordinates downPoint = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                        startLength = anchoredPoint1.Distance(downPoint);
                        startAngle = anchoredPoint1.Angle(downPoint) / RC;
                    }
                    else if (root is SkyImageTile)
                    {
                        SkyImageTile tile = (SkyImageTile)root;
                        anchoredPoint2 = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                        anchorPoint2 = tile.GetImagePixel(anchoredPoint2);

                    }
                }
                mouseDown = true;
                return true;
            }
            else
            {
                mouseDown = false;
                return false;
            }
        }

        public bool MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                dragging = false;
                return true;
            }
            return false;
        }

        public bool MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Earth3d.MainWindow.StudyImageset == null)
            {
                return false;
            }
            if (dragging)
            {
                Tile root = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.StudyImageset, null);
                root.CleanUpGeometryRecursive();

                bool twoRoots = false;

                if (Earth3d.MainWindow.StudyImageset.Projection == ProjectionType.Tangent && Earth3d.MainWindow.StudyImageset.WidthFactor == 1)
                {
                    twoRoots = true;
                }

                if (anchored)
                {
                    if (root is SkyImageTile)
                    {
                        SkyImageTile tile = (SkyImageTile)root;
                        Coordinates point2 = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                        

                        WcsFitter fitter = new WcsFitter(tile.Width, tile.Height);
                        fitter.AddPoint(anchoredPoint1, anchorPoint1);
                        fitter.AddPoint(point2, anchorPoint2);
                        fitter.Solve();
                        Earth3d.MainWindow.StudyImageset.BaseTileDegrees = fitter.Solution.Scale;
                        Earth3d.MainWindow.StudyImageset.Rotation = (fitter.Solution.Rotation);
                        Earth3d.MainWindow.StudyImageset.CenterX = (fitter.Solution.CenterX*15);
                        Earth3d.MainWindow.StudyImageset.CenterY = fitter.Solution.CenterY;
                        Earth3d.MainWindow.StudyImageset.OffsetX = fitter.Solution.OffsetX;
                        Earth3d.MainWindow.StudyImageset.OffsetY = fitter.Solution.OfsetY;


                    }
                    else
                    {
                        Coordinates downPoint = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                        double len = anchoredPoint1.Distance(downPoint);
                        double angle = anchoredPoint1.Angle(downPoint) / RC;
                        Earth3d.MainWindow.Text = String.Format("Angle = {0}", angle);
                        Earth3d.MainWindow.StudyImageset.BaseTileDegrees = startScale * (len / startLength);
                        Earth3d.MainWindow.StudyImageset.Rotation = startRotation - (angle - startAngle);
                       
                    }
                }
                else
                {
                    double factor = 1.0;
                    if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                    {
                        factor = .01;
                    }

                    if ((Control.ModifierKeys & Keys.Control )== Keys.Control)
                    {
                        double rotation = (pntDown.X - e.Location.X) / 50.0;
                        rotation *= factor;
                        Earth3d.MainWindow.StudyImageset.Rotation += rotation;
                        Earth3d.MainWindow.StudyImageset.Rotation = Earth3d.MainWindow.StudyImageset.Rotation % 360;
                    }
                    else if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                    {
                        double scale = 1.0 + (((pntDown.X - e.Location.X) / 500.0)*factor);
                        Earth3d.MainWindow.StudyImageset.BaseTileDegrees *= scale;
                        if (Earth3d.MainWindow.StudyImageset.BaseTileDegrees > 180)
                        {
                            Earth3d.MainWindow.StudyImageset.BaseTileDegrees = 180;
                        }
                    }
                    else
                    {
                        double moveX = (pntDown.X - e.Location.X) * Earth3d.MainWindow.GetPixelScaleX(true);
                        double moveY = (pntDown.Y - e.Location.Y) * Earth3d.MainWindow.GetPixelScaleY();
                        Earth3d.MainWindow.StudyImageset.CenterX += moveX;
                        Earth3d.MainWindow.StudyImageset.CenterY += moveY;

                    }
                    pntDown = e.Location;
                }

                if (twoRoots)
                {
                    Tile root2 = TileCache.GetTile(0, 1, 0, Earth3d.MainWindow.StudyImageset, null);
                    root2.CleanUpGeometryRecursive();
                }

            }
            return false;
        }

        public bool MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public bool Click(object sender, EventArgs e)
        {
            return false;
        }

        public bool MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            return false;
        }

        public bool KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.C:
                    //center
                    if (Earth3d.MainWindow.StudyImageset == null)
                    {
                        return false;
                    }
                    Tile root = TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.StudyImageset, null);
                    if (root != null)
                    {
                        root.CleanUpGeometryRecursive();
                    }
                    Earth3d.MainWindow.StudyImageset.CenterX = Earth3d.MainWindow.RenderEngine.RA *15;
                    Earth3d.MainWindow.StudyImageset.CenterY = Earth3d.MainWindow.RenderEngine.Dec;
                    return true;
                case Keys.S:
                    //scale to view
                    return true;
            }
            
            return false;
        }

        public bool KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            return false;
        }

        public bool Hover(System.Drawing.Point pnt)
        {
            return false;
        }

        #endregion


    }
}
