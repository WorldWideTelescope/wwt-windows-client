// Copyright Microsoft Copr 2006
// Written by Jonathan Fay

using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using Color = Windows.UI.Color;
using PointF = Windows.Foundation.Point;
using SizeF = Windows.Foundation.Size;
using RectangleF = Windows.Foundation.Rect;
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif

using System.IO;
namespace TerraViewer
{
    public class FieldOfView
    {
        public static List<Telescope> Telescopes;
        public static List<Camera> Cameras;
 
        static FieldOfView()
        {
            Telescopes = new List<Telescope>();
            Cameras = new List<Camera>();

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory);
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory+@"data"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory+@"data");
            }

            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=instruments", Properties.Settings.Default.CahceDirectory + @"data\instruments.xml", false, true);
            XmlDocument doc = new XmlDocument();
            doc.Load(Properties.Settings.Default.CahceDirectory + @"data\instruments.xml");

            
            XmlNode root = doc.GetChildByName("root");
            XmlNode scopes = root.SelectSingleNode("Telescopes");
            foreach (XmlNode child in scopes.ChildNodes)
            {
                Telescope scope = new Telescope(child.Attributes["Manufacturer"].Value, child.InnerText,
                    Convert.ToDouble(child.Attributes["FocalLength"].Value.ToString()),
                    Convert.ToDouble(child.Attributes["Aperture"].Value.ToString()),
                    child.Attributes["ManufacturerUrl"].Value.ToString(),
                    child.Attributes["MountType"].Value.ToString(),
                    child.Attributes["OpticalDesign"].Value.ToString());
                Telescopes.Add(scope);
            }

            XmlNode cams = root.SelectSingleNode("Cameras");
            foreach (XmlNode child in cams.ChildNodes)
            {
                Camera camera = new Camera
                    (
                    child.Attributes["Manufacturer"].Value,
                    child.InnerText.Trim(),
                    child.Attributes["ManufacturersURL"].Value.ToString()
                    );
                foreach (XmlNode grandChild in child.ChildNodes)
                {
                    if (grandChild.Name != "#text")
                    {
                        Imager imager = new Imager
                            (
                                Convert.ToInt32(grandChild.Attributes["ID"].Value.ToString()),
                                grandChild.Attributes["Type"].Value.ToString(),
                                Convert.ToDouble(grandChild.Attributes["Width"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["Height"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["HorizontalPixels"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["VerticalPixels"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["CenterX"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["CenterY"].Value.ToString()),
                                Convert.ToDouble(grandChild.Attributes["Rotation"].Value.ToString()),
                                grandChild.Attributes["Filter"].Value.ToString());
                        camera.Chips.Add(imager);
                    }
                }
                Cameras.Add(camera);
            }
        }

        public FieldOfView(int telescope, int camera, int eyepiece)
        {
            this.Telescope = GetTelescope(telescope);
            this.Camera = GetCamera(camera);
        }
        public FieldOfView()
        {
            this.Telescope = null;
            this.Camera = null;
        }
        public static Camera GetCamera(int hash)
        {
            foreach (Camera cam in Cameras)
            {
                if (hash == ((string)(cam.Manufacturer + cam.Name)).GetHashCode32())
                {
                    return cam;
                }
            }
            if (Cameras.Count > 0)
            {
                return Cameras[0];
            }
            return null;
        }

        public static Telescope GetTelescope(int hash)
        {
            foreach (Telescope scope in Telescopes)
            {
                if (hash == ((string)(scope.Manufacturer + scope.Name)).GetHashCode32())
                {
                    return scope;
                }
            }
            if (Telescopes.Count > 0)
            {
                return Telescopes[0];
            }

            return null;
        }

        protected const double RC = (Math.PI / 180.0);
        protected float radius = 1.0f;
        protected Vector3d RaDecTo3d(double dec, double ra)
        {
            return new Vector3d((Math.Cos(dec * RC) * Math.Cos(ra * RC) * radius), (Math.Sin(ra * RC) * radius), (Math.Sin(dec * RC) * Math.Cos(ra * RC) * radius));

        }


        protected PositionColoredTextured[] points;

        public Color DrawColor = SystemColors.Gray;
    
        public Telescope Telescope= null;
        public Camera Camera = null;

        public double angle = 0;

        public virtual bool Draw3D(RenderContext11 renderContext, float opacity, double ra, double dec)
        {
            if (this.Camera == null || Telescope == null)
            {
                return false;
            }
          
            DrawFOV(renderContext, opacity, ra, dec);
 
            return true;
        }

        private void DrawFOV(RenderContext11 renderContext, float opacity, double ra, double dec)
        {
            Color color = UiTools.FromArgb((int)(opacity * 255f), Properties.Settings.Default.FovColor);

            foreach (Imager chip in Camera.Chips)
            {


                double halfWidth = (Math.Atan(chip.Width / (2 * Telescope.FocalLength))) / RC;
                double halfHeight = (Math.Atan(chip.Height / (2 * Telescope.FocalLength))) / RC;
              
                double centerOffsetY = 2 * (Math.Atan(chip.CenterY / (2 * Telescope.FocalLength))) / RC;
                double centerOffsetX = 2 * (Math.Atan(chip.CenterX / (2 * Telescope.FocalLength))) / RC;

                SharpDX.Matrix mat = SharpDX.Matrix.RotationX((float)(((chip.Rotation + angle)) / 180f * Math.PI));
                mat = SharpDX.Matrix.Multiply( mat, SharpDX.Matrix.RotationZ((float)((dec) / 180f * Math.PI)));
                mat = SharpDX.Matrix.Multiply( mat, SharpDX.Matrix.RotationY((float)(((24 - (ra + 12))) / 12f * Math.PI)));

                int count = 4;

                points = new PositionColoredTextured[count + 1];
                int index = 0;

                points[index].Position = RaDecTo3d(-halfWidth + centerOffsetX, -halfHeight + centerOffsetY).Vector4;
                points[index].Color = color;

                index++;
                points[index].Position = RaDecTo3d(halfWidth + centerOffsetX, -halfHeight + centerOffsetY).Vector4;
                points[index].Color = color;

                index++;
                points[index].Position = RaDecTo3d(halfWidth + centerOffsetX, halfHeight + centerOffsetY).Vector4;
                points[index].Color = color;

                index++;
                points[index].Position = RaDecTo3d(-halfWidth + centerOffsetX, halfHeight + centerOffsetY).Vector4;
                points[index].Color = color;

                index++;
                points[index].Position = RaDecTo3d(-halfWidth + centerOffsetX, -halfHeight + centerOffsetY).Vector4;
                points[index].Color = color;


                for (int i = 0; i < points.Length; i++)
                {
                    points[i].Pos3= SharpDX.Vector3.TransformCoordinate(points[i].Pos3, mat);
                }

                SharpDX.Matrix matV =   (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
        
                Sprite2d.DrawLines(renderContext, points, 5, matV, true);
            }
        }


        void ComputePrimeFocusFov(Telescope scope, Imager chip)
        {
            double width = (2 * Math.Atan(chip.Width / (2 * scope.FocalLength))) / RC;
            double height = (2 * Math.Atan(chip.Height / (2 * scope.FocalLength))) / RC;

        }

        int GetTransparentColor(int color, float opacity)
        {
            Color inColor = UiTools.FromArgb(color);
            Color outColor = UiTools.FromArgb((byte)(opacity * 255f), inColor);
            return outColor.ToArgb();
        }
    }
}
