using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class FrameWizardPosition : PropPage
    {
        public FrameWizardPosition()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.altDepthLabel.Text = Language.GetLocalizedText(802, "Altitude (Meters)");
            this.LongitudeLabel.Text = Language.GetLocalizedText(803, "Longitude (Decimal Degrees)");
            this.LatitudeLabel.Text = Language.GetLocalizedText(804, "Latitude (Decimal Degrees)");
            this.label3.Text = Language.GetLocalizedText(801, "Select the Latitude, Longitude and Altitude");
            this.GetFromView.Text = Language.GetLocalizedText(259, "Get From View");
        }

        ReferenceFrame frame = null;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }
        public override bool Save()
        {
            bool failed = false;

            frame.Lat = ParseAndValidateCoordinate(Lattitude, frame.Lat, ref failed);
            frame.Lng = ParseAndValidateCoordinate(Longitude, frame.Lng, ref failed);
            frame.Altitude = ParseAndValidateDouble(Altitude, frame.Altitude, ref failed);
         
            return !failed;

        }

        private void FrameWizardPosition_Load(object sender, EventArgs e)
        {
            Lattitude.Text = frame.Lat.ToString();
            Longitude.Text = frame.Lng.ToString();
            Altitude.Text = frame.Altitude.ToString();


        }

        private void GetFromView_Click(object sender, EventArgs e)
        {
            Vector2d point = Earth3d.MainWindow.GetEarthCoordinates();


            Lattitude.Text = Coordinates.FormatDMS(point.Y);
            Longitude.Text = Coordinates.FormatDMS(point.X);
            Altitude.Text = Earth3d.MainWindow.GetEarthAltitude().ToString();
        }
    }
}
