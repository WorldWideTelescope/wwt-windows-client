// Copyright Microsoft Copr 2006
// Written by Jonathan Fay

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SharpDX;
using Color = System.Drawing.Color;

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
            var doc = new XmlDocument();
            doc.Load(Properties.Settings.Default.CahceDirectory + @"data\instruments.xml");

            
            XmlNode root = doc["root"];
            var scopes = root.SelectSingleNode("Telescopes");
            foreach (XmlNode child in scopes.ChildNodes)
            {
                var scope = new Telescope(child.Attributes["Manufacturer"].Value, child.InnerText,
                    Convert.ToDouble(child.Attributes["FocalLength"].Value),
                    Convert.ToDouble(child.Attributes["Aperture"].Value),
                    child.Attributes["ManufacturerUrl"].Value,
                    child.Attributes["MountType"].Value,
                    child.Attributes["OpticalDesign"].Value);
                Telescopes.Add(scope);
            }

            var cams = root.SelectSingleNode("Cameras");
            foreach (XmlNode child in cams.ChildNodes)
            {
                var camera = new Camera
                    (
                    child.Attributes["Manufacturer"].Value,
                    child.InnerText.Trim(),
                    child.Attributes["ManufacturersURL"].Value
                    );
                foreach (XmlNode grandChild in child)
                {
                    if (grandChild.Name != "#text")
                    {
                        var imager = new Imager
                            (
                                Convert.ToInt32(grandChild.Attributes["ID"].Value),
                                grandChild.Attributes["Type"].Value,
                                Convert.ToDouble(grandChild.Attributes["Width"].Value),
                                Convert.ToDouble(grandChild.Attributes["Height"].Value),
                                Convert.ToDouble(grandChild.Attributes["HorizontalPixels"].Value),
                                Convert.ToDouble(grandChild.Attributes["VerticalPixels"].Value),
                                Convert.ToDouble(grandChild.Attributes["CenterX"].Value),
                                Convert.ToDouble(grandChild.Attributes["CenterY"].Value),
                                Convert.ToDouble(grandChild.Attributes["Rotation"].Value),
                                grandChild.Attributes["Filter"].Value);
                        camera.Chips.Add(imager);
                    }
                }
                Cameras.Add(camera);
            }
        }

        public FieldOfView(int telescope, int camera, int eyepiece)
        {
            Telescope = GetTelescope(telescope);
            Camera = GetCamera(camera);
        }
        public FieldOfView()
        {
            Telescope = null;
            Camera = null;
        }
        public static Camera GetCamera(int hash)
        {
            foreach (var cam in Cameras)
            {
                if (hash == (cam.Manufacturer + cam.Name).GetHashCode32())
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
            foreach (var scope in Telescopes)
            {
                if (hash == (scope.Manufacturer + scope.Name).GetHashCode32())
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

        public Color DrawColor = Color.Gray;
    
        public Telescope Telescope= null;
        public Camera Camera = null;

        public double angle = 0;

        public virtual bool Draw3D(RenderContext11 renderContext, float opacity, double ra, double dec)
        {
            if (Camera == null || Telescope == null)
            {
                return false;
            }
          
            DrawFOV(renderContext, opacity, ra, dec);
 
            return true;
        }

        private void DrawFOV(RenderContext11 renderContext, float opacity, double ra, double dec)
        {
            var color = Color.FromArgb((int)(opacity*255f),Properties.Settings.Default.FovColor );

            foreach (var chip in Camera.Chips)
            {


                var halfWidth = (Math.Atan(chip.Width / (2 * Telescope.FocalLength))) / RC;
                var halfHeight = (Math.Atan(chip.Height / (2 * Telescope.FocalLength))) / RC;
              
                var centerOffsetY = 2 * (Math.Atan(chip.CenterY / (2 * Telescope.FocalLength))) / RC;
                var centerOffsetX = 2 * (Math.Atan(chip.CenterX / (2 * Telescope.FocalLength))) / RC;

                var mat = Matrix.RotationX((float)(((chip.Rotation + angle)) / 180f * Math.PI));
                mat = Matrix.Multiply( mat, Matrix.RotationZ((float)((dec) / 180f * Math.PI)));
                mat = Matrix.Multiply( mat, Matrix.RotationY((float)(((24 - (ra + 12))) / 12f * Math.PI)));

                var count = 4;

                points = new PositionColoredTextured[count + 1];
                var index = 0;

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


                for (var i = 0; i < points.Length; i++)
                {
                    points[i].Pos3= Vector3.TransformCoordinate(points[i].Pos3, mat);
                }

                var matV =   (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
        
                Sprite2d.DrawLines(renderContext, points, 5, matV, true);
            }
        }


        void ComputePrimeFocusFov(Telescope scope, Imager chip)
        {
            var width = (2 * Math.Atan(chip.Width / (2 * scope.FocalLength))) / RC;
            var height = (2 * Math.Atan(chip.Height / (2 * scope.FocalLength))) / RC;

        }

        int GetTransparentColor(int color, float opacity)
        {
            var inColor = Color.FromArgb(color);
            var outColor = Color.FromArgb((byte)(opacity * 255f), inColor);
            return outColor.ToArgb();
        }
    }
}
