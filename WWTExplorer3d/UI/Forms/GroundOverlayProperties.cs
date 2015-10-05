using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class GroundOverlayProperties : Form, IUiController
    {
        public GroundOverlayProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(241, "North");
            label2.Text = Language.GetLocalizedText(246, "South");
            label3.Text = Language.GetLocalizedText(248, "West");
            label4.Text = Language.GetLocalizedText(243, "East");
            label5.Text = Language.GetLocalizedText(861, "Rotation");
            ok.Text = Language.GetLocalizedText(759, "Ok");
            Text = Language.GetLocalizedText(862, "Ground Overlay Properties");
        }

        public KmlGroundOverlay Overlay;
        public GroundOverlayLayer OverlayLayer;

         private void ok_Click(object sender, EventArgs e)
        {
            double.TryParse(North.Text, out Overlay.north);
            double.TryParse(South.Text, out Overlay.south);
            double.TryParse(West.Text, out Overlay.west);
            double.TryParse(East.Text, out Overlay.east);
            double.TryParse(Rotation.Text, out Overlay.rotation);

            DialogResult = DialogResult.OK;
            Close();
        }
        bool initialized;
        private void GroundOverlayProperties_Load(object sender, EventArgs e)
        {
            Earth3d.MainWindow.UiController = this;
            lines = new LineList();
            lines.DepthBuffered = false;
            UpdateTextBoxes();
            initialized = true;
            UpdateLines();
        }

        private void UpdateTextBoxes()
        {
            North.Text = Overlay.north.ToString();
            South.Text = Overlay.south.ToString();
            West.Text = Overlay.west.ToString();
            East.Text = Overlay.east.ToString();
            Rotation.Text = Overlay.rotation.ToString();
        }

        private void UpdateLines()
        {
            if (!initialized)
            {
                return;
            }
            lines.Clear();

            var width = Overlay.east - Overlay.west;
            var height = Overlay.north - Overlay.south;

            var altitude = 1+Earth3d.MainWindow.GetScaledAltitudeForLatLong((Overlay.north + Overlay.south) / 2, (Overlay.east + Overlay.west) / 2);

            var topLeftA = Coordinates.GeoTo3dDouble(Overlay.north - height / 20, Overlay.west, altitude);
            var topLeftB = Coordinates.GeoTo3dDouble(Overlay.north, Overlay.west, altitude);
            var topLeftC = Coordinates.GeoTo3dDouble(Overlay.north, Overlay.west + width / 20, altitude);
            var topMiddleA = Coordinates.GeoTo3dDouble(Overlay.north, ((Overlay.east + Overlay.west) / 2) - width / 20, altitude);
            var topMiddleB = Coordinates.GeoTo3dDouble(Overlay.north, ((Overlay.east + Overlay.west) / 2) + width / 20, altitude);
            var topRightA = Coordinates.GeoTo3dDouble(Overlay.north - height / 20, Overlay.east, altitude);
            var topRightB = Coordinates.GeoTo3dDouble(Overlay.north, Overlay.east, altitude);
            var topRightC = Coordinates.GeoTo3dDouble(Overlay.north, Overlay.east - width / 20, altitude);

            var middleRightA = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 - height / 20, Overlay.west, altitude);
            var middleRightB = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 + height / 20, Overlay.west, altitude);
            var centerA = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2, ((Overlay.east + Overlay.west) / 2) - width / 20, altitude);
            var centerB = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2, ((Overlay.east + Overlay.west) / 2) + width / 20, altitude);
            var centerC = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 - height / 20, ((Overlay.east + Overlay.west) / 2), altitude);
            var centerD = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 + height / 20, ((Overlay.east + Overlay.west) / 2), altitude);
            var middleLeftA = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 - height / 20, Overlay.east, altitude);
            var middleLeftB = Coordinates.GeoTo3dDouble((Overlay.north + Overlay.south) / 2 + height / 20, Overlay.east, altitude);



            var bottomLeftA = Coordinates.GeoTo3dDouble(Overlay.south + height / 20, Overlay.west, altitude);
            var bottomLeftB = Coordinates.GeoTo3dDouble(Overlay.south, Overlay.west, altitude);
            var bottomLeftC = Coordinates.GeoTo3dDouble(Overlay.south, Overlay.west + width / 20, altitude);
            var bottomMiddleA = Coordinates.GeoTo3dDouble(Overlay.south, ((Overlay.east + Overlay.west) / 2) - width / 20, altitude);
            var bottomMiddleB = Coordinates.GeoTo3dDouble(Overlay.south, ((Overlay.east + Overlay.west) / 2) + width / 20, altitude);
            var bottomRightA = Coordinates.GeoTo3dDouble(Overlay.south + height / 20, Overlay.east, altitude);
            var bottomRightB = Coordinates.GeoTo3dDouble(Overlay.south, Overlay.east, altitude);
            var bottomRightC = Coordinates.GeoTo3dDouble(Overlay.south, Overlay.east - width / 20, altitude);
            var lineColor = Color.Yellow;

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
            double.TryParse(North.Text, out Overlay.north);
            UpdateLines();
        }

        private void South_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(South.Text, out Overlay.south);
            UpdateLines();

        }

        private void West_TextChanged(object sender, EventArgs e)
        {

            double.TryParse(West.Text, out Overlay.west);
            UpdateLines();
        }

        private void East_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(East.Text, out Overlay.east);
            UpdateLines();

        }

        private void Rotation_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(Rotation.Text, out Overlay.rotation);
            UpdateLines();
        }

        private void GroundOverlayProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            lines.Clear();
            if (Earth3d.MainWindow.UiController == this)
            {
                Earth3d.MainWindow.UiController = null;
            }

        }


        #region IUiController Members

        LineList lines;

        public void PreRender(Earth3d window)
        {
            if (OverlayLayer != null)
            {
                OverlayLayer.ShowEditUi = true;
                OverlayLayer.UiLines = lines;
            }
        }

        void IUiController.Render(Earth3d window)
        {
            return;
        }
        enum DragCorners { None, NW, N, NE, W, C, E, SW, S, SE };

        bool drag;

        DragCorners dragCorner = DragCorners.None;

        Coordinates mouseDown;
        bool IUiController.MouseDown(object sender, MouseEventArgs e)
        {

            var width = Overlay.east - Overlay.west;
            var height = Overlay.north - Overlay.south;

            var range = Math.Max(width / 40, height / 40);
            var cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);

            if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, Overlay.west)) < range)
            {
                dragCorner = DragCorners.NW;
                drag = true;
            }
            if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, (Overlay.west+Overlay.east)/2)) < range)
            {
                dragCorner = DragCorners.N;
                drag = true;

            }
            if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, Overlay.east)) < range)
            {
                dragCorner = DragCorners.NE;
                drag = true;

            }
            
            if (cursor.Distance(Coordinates.FromLatLng((Overlay.north+Overlay.south)/2, Overlay.west)) < range)
            {
                dragCorner = DragCorners.W;
                drag = true;


            }

            if (cursor.Distance(Coordinates.FromLatLng((Overlay.north + Overlay.south) / 2, (Overlay.west + Overlay.east) / 2)) < range)
            {
                dragCorner = DragCorners.C;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng((Overlay.north + Overlay.south) / 2,  Overlay.east)) < range)
            {
                dragCorner = DragCorners.E;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, Overlay.west)) < range)
            {
                dragCorner = DragCorners.SW;
                drag = true;
            }

            if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, (Overlay.west + Overlay.east) / 2)) < range)
            {
                dragCorner = DragCorners.S;
                drag = true;

            }

            if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, Overlay.east)) < range)
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
              
                var cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);

                switch (dragCorner)
                {
                    case DragCorners.NW:
                        Overlay.north = cursor.Lat;
                        Overlay.west = cursor.Lng;
                        break;
                    case DragCorners.N:
                        Overlay.north = cursor.Lat;
                        break;
                    case DragCorners.NE:
                        Overlay.north = cursor.Lat;
                        Overlay.east = cursor.Lng;
                        break;
                    case DragCorners.W:
                        Overlay.west = cursor.Lng;
                        break;
                    case DragCorners.C:
                        Overlay.north -= (mouseDown.Lat - cursor.Lat);
                        Overlay.west -= (mouseDown.Lng - cursor.Lng);
                        Overlay.south -= (mouseDown.Lat - cursor.Lat);
                        Overlay.east -= (mouseDown.Lng - cursor.Lng);

                        break;
                    case DragCorners.E:
                        Overlay.east = cursor.Lng;
                        break;
                    case DragCorners.SW:
                        Overlay.south = cursor.Lat;
                        Overlay.west = cursor.Lng;
                        break;
                    case DragCorners.S:
                        Overlay.south = cursor.Lat;
                        break;
                    case DragCorners.SE:
                        Overlay.south = cursor.Lat;
                        Overlay.east = cursor.Lng;
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
                var wnd = (Control)sender;
                var width = Overlay.east - Overlay.west;
                var height = Overlay.north - Overlay.south;

                var range = Math.Max(width / 40, height / 40);
                var cursor = Earth3d.MainWindow.GetCoordinatesForScreenPoint(e.X, e.Y);
                var dragCorner = DragCorners.None;
                if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, Overlay.west)) < range)
                {
                    dragCorner = DragCorners.NW;
                }
                if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, (Overlay.west + Overlay.east) / 2)) < range)
                {
                    dragCorner = DragCorners.N;

                }
                if (cursor.Distance(Coordinates.FromLatLng(Overlay.north, Overlay.east)) < range)
                {
                    dragCorner = DragCorners.NE;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Overlay.north + Overlay.south) / 2, Overlay.west)) < range)
                {
                    dragCorner = DragCorners.W;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Overlay.north + Overlay.south) / 2, (Overlay.west + Overlay.east) / 2)) < range)
                {
                    dragCorner = DragCorners.C;

                }

                if (cursor.Distance(Coordinates.FromLatLng((Overlay.north + Overlay.south) / 2, Overlay.east)) < range)
                {
                    dragCorner = DragCorners.E;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, Overlay.west)) < range)
                {
                    dragCorner = DragCorners.SW;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, (Overlay.west + Overlay.east) / 2)) < range)
                {
                    dragCorner = DragCorners.S;
                }

                if (cursor.Distance(Coordinates.FromLatLng(Overlay.south, Overlay.east)) < range)
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

        #endregion

 
      
    }
}
