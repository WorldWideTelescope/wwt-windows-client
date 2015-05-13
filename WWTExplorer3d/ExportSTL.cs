using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class ExportSTL : Form, IUiController
    {
        public ExportSTL()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(241, "North");
            this.label2.Text = Language.GetLocalizedText(246, "South");
            this.label3.Text = Language.GetLocalizedText(248, "West");
            this.label4.Text = Language.GetLocalizedText(243, "East");
            this.label5.Text = "Density";
            this.Text = "Export STL File for 3D Printing";
        }

       
        bool initialized = false;

        private void ExportSTL_Load(object sender, EventArgs e)
        {
            Earth3d.MainWindow.UiController = this;
            lines = new LineList();
            lines.DepthBuffered = false;
            UpdateTextBoxes();
            initialized = true;
            UpdateLines();
        }

        double top;
        double left;
        double right;
        double bottom;
        int density = 100;

        private void UpdateTextBoxes()
        {
            North.Text = Rect.North.ToString();
            South.Text = Rect.South.ToString();
            West.Text = Rect.West.ToString();
            East.Text = Rect.East.ToString();
            Density.Text = density.ToString();
        }

        private void UpdateLines()
        {
            if (!initialized)
            {
                return;
            }
            lines.Clear();

            double width = Rect.East - Rect.West;
            double height = Rect.North - Rect.South;

            double altitude = 1 + Earth3d.MainWindow.GetScaledAltitudeForLatLong((Rect.North + Rect.South) / 2, (Rect.East + Rect.West) / 2);

            Vector3d topLeftA = Coordinates.GeoTo3dDouble(Rect.North - height / 20, Rect.West, altitude);
            Vector3d topLeftB = Coordinates.GeoTo3dDouble(Rect.North, Rect.West, altitude);
            Vector3d topLeftC = Coordinates.GeoTo3dDouble(Rect.North, Rect.West + width / 20, altitude);
            Vector3d topMiddleA = Coordinates.GeoTo3dDouble(Rect.North, ((Rect.East + Rect.West) / 2) - width / 20, altitude);
            Vector3d topMiddleB = Coordinates.GeoTo3dDouble(Rect.North, ((Rect.East + Rect.West) / 2) + width / 20, altitude);
            Vector3d topRightA = Coordinates.GeoTo3dDouble(Rect.North - height / 20, Rect.East, altitude);
            Vector3d topRightB = Coordinates.GeoTo3dDouble(Rect.North, Rect.East, altitude);
            Vector3d topRightC = Coordinates.GeoTo3dDouble(Rect.North, Rect.East - width / 20, altitude);

            Vector3d middleRightA = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 - height / 20, Rect.West, altitude);
            Vector3d middleRightB = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 + height / 20, Rect.West, altitude);
            Vector3d centerA = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2, ((Rect.East + Rect.West) / 2) - width / 20, altitude);
            Vector3d centerB = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2, ((Rect.East + Rect.West) / 2) + width / 20, altitude);
            Vector3d centerC = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 - height / 20, ((Rect.East + Rect.West) / 2), altitude);
            Vector3d centerD = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 + height / 20, ((Rect.East + Rect.West) / 2), altitude);
            Vector3d middleLeftA = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 - height / 20, Rect.East, altitude);
            Vector3d middleLeftB = Coordinates.GeoTo3dDouble((Rect.North + Rect.South) / 2 + height / 20, Rect.East, altitude);



            Vector3d bottomLeftA = Coordinates.GeoTo3dDouble(Rect.South + height / 20, Rect.West, altitude);
            Vector3d bottomLeftB = Coordinates.GeoTo3dDouble(Rect.South, Rect.West, altitude);
            Vector3d bottomLeftC = Coordinates.GeoTo3dDouble(Rect.South, Rect.West + width / 20, altitude);
            Vector3d bottomMiddleA = Coordinates.GeoTo3dDouble(Rect.South, ((Rect.East + Rect.West) / 2) - width / 20, altitude);
            Vector3d bottomMiddleB = Coordinates.GeoTo3dDouble(Rect.South, ((Rect.East + Rect.West) / 2) + width / 20, altitude);
            Vector3d bottomRightA = Coordinates.GeoTo3dDouble(Rect.South + height / 20, Rect.East, altitude);
            Vector3d bottomRightB = Coordinates.GeoTo3dDouble(Rect.South, Rect.East, altitude);
            Vector3d bottomRightC = Coordinates.GeoTo3dDouble(Rect.South, Rect.East - width / 20, altitude);
            Color lineColor = Color.Yellow;

            lines.AddLine(topLeftA, topLeftB, lineColor, new Dates());
            lines.AddLine(topLeftB, topLeftC, lineColor, new Dates());
            lines.AddLine(topMiddleA, topMiddleB, lineColor, new Dates());
            lines.AddLine(topRightA, topRightB, lineColor, new Dates());
            lines.AddLine(topRightB, topRightC, lineColor, new Dates());

            lines.AddLine(middleRightA, middleRightB, lineColor, new Dates());
            lines.AddLine(centerA, centerB, lineColor, new Dates());
            lines.AddLine(centerC, centerD, lineColor, new Dates());
            lines.AddLine(middleLeftA, middleLeftB, lineColor, new Dates());


            lines.AddLine(bottomLeftA, bottomLeftB, lineColor, new Dates());
            lines.AddLine(bottomLeftB, bottomLeftC, lineColor, new Dates());
            lines.AddLine(bottomMiddleA, bottomMiddleB, lineColor, new Dates());
            lines.AddLine(bottomRightA, bottomRightB, lineColor, new Dates());
            lines.AddLine(bottomRightB, bottomRightC, lineColor, new Dates());


        }

        private void North_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(North.Text, out top);
            UpdateLines();
        }

        private void South_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(South.Text, out bottom);
            UpdateLines();

        }

        private void West_TextChanged(object sender, EventArgs e)
        {

            double.TryParse(West.Text, out left);
            UpdateLines();
        }

        private void East_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(East.Text, out right);
            UpdateLines();

        }

        LineList lines = null;

 
        void IUiController.Render(Earth3d window)
        {
            lines.DrawLines(window.RenderContext11, 1);

            return;
        }
        enum DragCorners { None, NW, N, NE, W, C, E, SW, S, SE };

        bool drag = false;

        DragCorners dragCorner = DragCorners.None;

        Coordinates mouseDown;
        bool IUiController.MouseDown(object sender, MouseEventArgs e)
        {

            double width = Rect.East - Rect.West;
            double height = Rect.North - Rect.South;

            double range = Math.Max(width / 40, height / 40);
            Coordinates cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);

            if (cursor.Distance(Coordinates.FromLatLng(Rect.North, Rect.West)) < range)
            {
                dragCorner = DragCorners.NW;
                drag = true;
            }
            if (cursor.Distance(Coordinates.FromLatLng(Rect.North, (Rect.West + Rect.East) / 2)) < range)
            {
                dragCorner = DragCorners.N;
                drag = true;

            }
            if (cursor.Distance(Coordinates.FromLatLng(Rect.North, Rect.East)) < range)
            {
                dragCorner = DragCorners.NE;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, Rect.West)) < range)
            {
                dragCorner = DragCorners.W;
                drag = true;


            }

            if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, (Rect.West + Rect.East) / 2)) < range)
            {
                dragCorner = DragCorners.C;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, Rect.East)) < range)
            {
                dragCorner = DragCorners.E;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng(Rect.South, Rect.West)) < range)
            {
                dragCorner = DragCorners.SW;
                drag = true;
            }

            if (cursor.Distance(Coordinates.FromLatLng(Rect.South, (Rect.West + Rect.East) / 2)) < range)
            {
                dragCorner = DragCorners.S;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng(Rect.South, Rect.East)) < range)
            {
                dragCorner = DragCorners.SE;
                drag = true;

            }

            if (drag)
            {
                mouseDown = cursor;
            }
            return drag;
        }

        bool IUiController.MouseUp(object sender, MouseEventArgs e)
        {
            if (!drag)
            {
                return false;
            }
            drag = false;
            return true;
        }

        bool IUiController.MouseMove(object sender, MouseEventArgs e)
        {
            if (drag)
            {

                Coordinates cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);

                switch (dragCorner)
                {
                    case DragCorners.NW:
                        Rect.North = cursor.Lat;
                        Rect.West = cursor.Lng;
                        break;
                    case DragCorners.N:
                        Rect.North = cursor.Lat;
                        break;
                    case DragCorners.NE:
                        Rect.North = cursor.Lat;
                        Rect.East = cursor.Lng;
                        break;
                    case DragCorners.W:
                        Rect.West = cursor.Lng;
                        break;
                    case DragCorners.C:
                        Rect.North -= (mouseDown.Lat - cursor.Lat);
                        Rect.West -= (mouseDown.Lng - cursor.Lng);
                        Rect.South -= (mouseDown.Lat - cursor.Lat);
                        Rect.East -= (mouseDown.Lng - cursor.Lng);

                        break;
                    case DragCorners.E:
                        Rect.East = cursor.Lng;
                        break;
                    case DragCorners.SW:
                        Rect.South = cursor.Lat;
                        Rect.West = cursor.Lng;
                        break;
                    case DragCorners.S:
                        Rect.South = cursor.Lat;
                        break;
                    case DragCorners.SE:
                        Rect.South = cursor.Lat;
                        Rect.East = cursor.Lng;
                        break;
                    default:
                        break;
                }
                mouseDown = cursor;
                UpdateTextBoxes();
                UpdateLines();
                return true;
            }
            else
            {
                Control wnd = (Control)sender;
                double width = Rect.East - Rect.West;
                double height = Rect.North - Rect.South;

                double range = Math.Max(width / 40, height / 40);
                Coordinates cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                DragCorners dragCorner = DragCorners.None;
                if (cursor.Distance(Coordinates.FromLatLng(Rect.North, Rect.West)) < range)
                {
                    dragCorner = DragCorners.NW;
                }
                if (cursor.Distance(Coordinates.FromLatLng(Rect.North, (Rect.West + Rect.East) / 2)) < range)
                {
                    dragCorner = DragCorners.N;

                }
                if (cursor.Distance(Coordinates.FromLatLng(Rect.North, Rect.East)) < range)
                {
                    dragCorner = DragCorners.NE;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, Rect.West)) < range)
                {
                    dragCorner = DragCorners.W;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, (Rect.West + Rect.East) / 2)) < range)
                {
                    dragCorner = DragCorners.C;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Rect.North + Rect.South) / 2, Rect.East)) < range)
                {
                    dragCorner = DragCorners.E;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Rect.South, Rect.West)) < range)
                {
                    dragCorner = DragCorners.SW;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Rect.South, (Rect.West + Rect.East) / 2)) < range)
                {
                    dragCorner = DragCorners.S;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Rect.South, Rect.East)) < range)
                {
                    dragCorner = DragCorners.SE;
                }

                switch (dragCorner)
                {
                    case DragCorners.SE:
                    case DragCorners.NW:
                        wnd.Cursor = Cursors.SizeNWSE;
                        break;
                    case DragCorners.N:
                    case DragCorners.S:
                        wnd.Cursor = Cursors.SizeNS;
                        break;
                    case DragCorners.W:
                    case DragCorners.E:
                        wnd.Cursor = Cursors.SizeWE;
                        break;
                    case DragCorners.C:
                        wnd.Cursor = Cursors.Hand;
                        break;
                    case DragCorners.SW:
                    case DragCorners.NE:
                        wnd.Cursor = Cursors.SizeNESW;
                        break;

                    default:
                        wnd.Cursor = Cursors.Default;
                        break;
                }
            }
            return false;
        }

        bool IUiController.MouseClick(object sender, MouseEventArgs e)
        {
            return false;
        }

        bool IUiController.Click(object sender, EventArgs e)
        {
            return false;
        }

        bool IUiController.MouseDoubleClick(object sender, MouseEventArgs e)
        {
            return false;
        }

        bool IUiController.KeyDown(object sender, KeyEventArgs e)
        {
            return false;
        }

        bool IUiController.KeyUp(object sender, KeyEventArgs e)
        {
            return false;
        }

        bool IUiController.Hover(Point pnt)
        {
            return false;
        }
        public GeoRect Rect;


        public void PreRender(Earth3d window)
        {
            
        }

        private void Export_Click(object sender, EventArgs e)
        {
            string filename = "";

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter =  "Standard Tessellation Language" + "|*.stl";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".stl";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                filename = saveDialog.FileName;

                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
            }
            else
            {
                return;
            }

            ProgressPopup.Show(this, "Export STL File", "Scanning Elevation Map");

            double baseOffset = double.Parse(baseHeight.Text);

            density = int.Parse(Density.Text);

            double xRate = Rect.West - Rect.East;
            double yRate = Rect.North - Rect.South;

            double latCenter = (Rect.North + Rect.South) / 2;

            double ratio = Math.Cos(latCenter / 180 * Math.PI);

            double sizeY = 100;
            double sizeX = Math.Abs(((xRate * ratio) / yRate) * sizeY);

            int stepsX = (int)(sizeX * density / 10);
            int stepsY = (int)(sizeY * density / 10);
     
            
            //Computer relative altitude to latitude scaling for this planet.
            double radius = Earth3d.MainWindow.RenderContext11.NominalRadius;
            double altScaleFactor = ((radius * Math.PI * 2) / 360) * (yRate/sizeY);

            altScaleFactor = 1 / altScaleFactor;

            

            xRate /= stepsX;
            yRate /= stepsY;


            Vector3d[,] points = new Vector3d[stepsX, stepsY];
            double[,] altitude = new double[stepsX, stepsY];
            double maxAltitude = -10000000;
            double minAltitude = 100000000;
            double altScale = double.Parse(AltitudeScale.Text) / 100;


            int estimatedTotal = stepsX * stepsY;
            int actualTotal = 0;


            for (int y = 0; y < stepsY; y++)
            {
                for (int x = 0; x < stepsX; x++)
                {
                    double lat = Rect.North - (yRate * y);
                    double lng = Rect.East + (xRate * x);

                    double alt = Earth3d.MainWindow.GetAltitudeForLatLongNow(lat, lng);
                    altitude[x, y] = alt;
                    maxAltitude = Math.Max(alt, maxAltitude);
                    minAltitude = Math.Min(minAltitude, alt);
                    actualTotal++;
                }

                if (!ProgressPopup.SetProgress(((actualTotal * 100) / estimatedTotal), "Scanning Elevation Map"))
                {
                    ProgressPopup.Done();
                    return;
                }

                
            }

            double altRange = maxAltitude - minAltitude;

            // old altScaleFactor = (10 / altRange) * altScale;
            altScaleFactor *= altScale;

            double stepScaleX = sizeX / stepsX;
            double stepScaleY = sizeY / stepsY;

            // make the verticies
            for (int y = 0; y < stepsY; y++)
            {
                for (int x = 0; x < stepsX; x++)
                {
                    altitude[x, y] = ((altitude[x, y] - minAltitude) * altScaleFactor) + baseOffset;

                    points[x, y] = new Vector3d(x * stepScaleX, y * stepScaleY, altitude[x, y]);


                }
            }

           


            ProgressPopup.SetProgress(0, "Writing File");

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            FileStream fs = File.OpenWrite(filename);
            BinaryWriter bw = new BinaryWriter(fs);

            // Write File Header
            bw.Write(new byte[80]);

            // x-1*y-1*2
            int count = ((stepsX - 1) * (stepsY - 1) + (stepsY - 1) + (stepsY - 1) + (stepsX - 1) + (stepsX - 1) + (stepsX - 1) * (stepsY - 1)) * 2;

 
            // Write Triangle Count
            bw.Write(count);


            // Loop thru and create triangles for all quads..

            int writeCount = 0;

            for (int y = 0; y < stepsY - 1; y++)
            {
                for (int x = 0; x < stepsX - 1; x++)
                {
                    // Write dummy Normal
                    bw.Write(0f);
                    bw.Write(0f);
                    bw.Write(0f);


                    // Vertexes - triangle 1
                    WriteVertex(bw, points[x, y]);
                    WriteVertex(bw, points[x + 1, y]);
                    WriteVertex(bw, points[x + 1, y + 1]);
                    bw.Write((UInt16)(0));
                    writeCount++;


                    // Write dummy Normal
                    bw.Write(0f);
                    bw.Write(0f);
                    bw.Write(0f);
                    // Vertexes - triangle 2
                    WriteVertex(bw, points[x, y]);
                    WriteVertex(bw, points[x + 1, y + 1]);
                    WriteVertex(bw, points[x, y + 1]);
                    bw.Write((UInt16)(0));
                    writeCount++;
                }
            }
            ProgressPopup.SetProgress(35, "Writing File");

            Vector3d pnt = new Vector3d();

            // Make side Skirts
            for (int y = 0; y < stepsY - 1; y++)
            {
                int x = 0;
                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);


                // Vertexes - triangle 1
                WriteVertex(bw, points[x, y]);
                WriteVertex(bw, points[x, y + 1]);
                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;


                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);
                // Vertexes - triangle 2
                WriteVertex(bw, points[x, y + 1]);

                pnt = points[x, y + 1];
                pnt.Z = 0;
                WriteVertex(bw, pnt);

                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;

            }

            ProgressPopup.SetProgress(45, "Writing File");

            for (int y = 0; y < stepsY - 1; y++)
            {
                int x = stepsX - 1;
                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);


                // Vertexes - triangle 1
                WriteVertex(bw, points[x, y + 1]);
                WriteVertex(bw, points[x, y]);
                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;


                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);
                // Vertexes - triangle 2

                pnt = points[x, y + 1];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                WriteVertex(bw, points[x, y + 1]);

                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;

            }

            ProgressPopup.SetProgress(50, "Writing File");

            for (int x = 0; x < stepsX - 1; x++)
            {
                int y = 0;
                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);


                // Vertexes - triangle 1
                WriteVertex(bw, points[x+ 1, y ]);
                WriteVertex(bw, points[x, y]);
                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;


                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);
                // Vertexes - triangle 2

                pnt = points[x+ 1, y ];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                WriteVertex(bw, points[x + 1, y]);

                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;

            }

            ProgressPopup.SetProgress(55, "Writing File");

            for (int x = 0; x < stepsX - 1; x++)
            {
                int y = stepsY - 1;
                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);


                // Vertexes - triangle 1
                WriteVertex(bw, points[x, y]);
                WriteVertex(bw, points[x + 1, y]);
                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;


                // Write dummy Normal
                bw.Write(0f);
                bw.Write(0f);
                bw.Write(0f);
                // Vertexes - triangle 2
                WriteVertex(bw, points[x + 1, y]);

                pnt = points[x + 1, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);

                pnt = points[x, y];
                pnt.Z = 0;
                WriteVertex(bw, pnt);
                bw.Write((UInt16)(0));
                writeCount++;

            }

            ProgressPopup.SetProgress(65, "Writing File");


            ProgressPopup.SetProgress(75, "Writing File");

            for (int y = 0; y < stepsY - 1; y++)
            {
                for (int x = 0; x < stepsX - 1; x++)
                {
                    // Write dummy Normal
                    bw.Write(0f);
                    bw.Write(0f);
                    bw.Write(0f);


                    // Vertexes - triangle 1
                    pnt = points[x, y];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);

                    pnt = points[x + 1, y + 1];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);

                    pnt = points[x + 1, y];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);

                  

                    bw.Write((UInt16)(0));
                    writeCount++;


                    // Write dummy Normal
                    bw.Write(0f);
                    bw.Write(0f);
                    bw.Write(0f);

                    // Vertexes - triangle 2
                    pnt = points[x, y];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);

                    pnt = points[x, y + 1];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);  
                    
                    pnt = points[x + 1, y + 1];
                    pnt.Z = 0;
                    WriteVertex(bw, pnt);

                    bw.Write((UInt16)(0));
                    writeCount++;
                }
            }



            // Make Bottom

            bw.Close();

            ProgressPopup.Done();

        }

        static void WriteVertex(BinaryWriter bw, Vector3d point)
        {
            bw.Write((float)point.X);
            bw.Write((float)point.Y);
            bw.Write((float)point.Z);
        }

        private void ExportSTL_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Earth3d.MainWindow.UiController == this)
            {
                Earth3d.MainWindow.UiController = null;
            }
        }
    }

    public struct GeoRect
    {
        public double North;
        public double South;
        public double East;
        public double West;
    }
}
