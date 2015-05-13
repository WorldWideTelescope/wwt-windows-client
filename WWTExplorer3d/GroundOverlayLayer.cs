using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace TerraViewer
{
    public class GroundOverlayLayer : Layer
    {
        public KmlGroundOverlay Overlay = new KmlGroundOverlay();
        public string filename;

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            string fName = filename;

            bool copy = !fName.Contains(ID.ToString());

            string fileName = fc.TempDirectory + string.Format("{0}\\{1}.png", fc.PackageID, this.ID.ToString());
            string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);

            if (copy)
            {
                if (File.Exists(fName) && !File.Exists(fileName))
                {
                    File.Copy(fName, fileName);
                }

            }

            if (File.Exists(fileName))
            {
                fc.AddFile(fileName);
            }
        }

        public override bool CanCopyToClipboard()
        {
            return false;
        }

        public override void CleanUp()
        {
            base.CleanUp();
        }

        public override void CopyToClipboard()
        {
            base.CopyToClipboard();
        }

        public override bool PreDraw(RenderContext11 renderContext, float opacity)
        {
            Overlay.color = Color.FromArgb((int)(this.Opacity * opacity * Color.A), Color);
            Earth3d.MainWindow.KmlMarkers.AddGroundOverlay(Overlay);
            
            return true;
        }

        public LineList UiLines = null;
        public bool ShowEditUi = false;

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (ShowEditUi && UiLines != null)
            {
                UiLines.DrawLines(renderContext, 1);
            }
            else
            {
                UiLines = null;
            }
            ShowEditUi = false;
            return true;
        }

        [LayerProperty]
        public double North
        {
            get { return Overlay.north; }
            set
            {
                if (Overlay.north != value)
                {
                    version++;

                    Overlay.north = value;
                }
            }
        }

        [LayerProperty]
        public double South
        {
            get { return Overlay.south; }
            set
            {
                if (Overlay.south != value)
                {
                    version++;
                    Overlay.south = value;
                }
            }
        }

        [LayerProperty]
        public double East
        {
            get { return Overlay.east; }
            set
            {
                if (Overlay.east != value)
                {
                    version++;
                    Overlay.east = value;
                }
            }
        }

        [LayerProperty]
        public double West
        {
            get { return Overlay.west; }
            set
            {
                if (Overlay.west != value)
                {
                    version++;
                    Overlay.west = value;
                }
            }
        }

        [LayerProperty]
        public double Rotation
        {
            get { return Overlay.rotation; }
            set
            {
                if (Overlay.rotation != value)
                {
                    version++;
                    Overlay.rotation = value;
                }
            }
        }

        [LayerProperty]
        public double Altitude
        {
            get { return Overlay.altitude; }
            set
            {
                if (Overlay.altitude != value)
                {
                    version++;
                    Overlay.altitude = value;
                }
            }
        }

        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("North", North.ToString());
            xmlWriter.WriteAttributeString("South", South.ToString());
            xmlWriter.WriteAttributeString("East", East.ToString());
            xmlWriter.WriteAttributeString("West", West.ToString());
            xmlWriter.WriteAttributeString("Rotation", Rotation.ToString());
            xmlWriter.WriteAttributeString("Altitude", Altitude.ToString());

        }



        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            North = Double.Parse(node.Attributes["North"].Value);

            South = Double.Parse(node.Attributes["South"].Value);

            East = double.Parse(node.Attributes["East"].Value);
            West = double.Parse(node.Attributes["West"].Value);
            Rotation = double.Parse(node.Attributes["Rotation"].Value);
            Altitude = double.Parse(node.Attributes["Altitude"].Value);
        }
        public override void LoadData(string path)
        {
            filename = path.Replace(".txt", ".png");
            CreateFromFile(filename);
        }

        internal void CreateFromFile(string path)
        {


            Overlay.Icon = new KmlIcon();

            Overlay.Icon.Href = path;
            filename = path;
        }
    }
}
