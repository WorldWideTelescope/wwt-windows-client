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
            this.altDepthLabel.Text = Language.GetLocalizedText(763, "Altitude");
            this.LongitudeLabel.Text = Language.GetLocalizedText(803, "Longitude (Decimal Degrees)");
            this.LatitudeLabel.Text = Language.GetLocalizedText(804, "Latitude (Decimal Degrees)");
            this.label3.Text = Language.GetLocalizedText(801, "Select the Latitude, Longitude and Altitude");
            this.GetFromView.Text = Language.GetLocalizedText(259, "Get From View");
        }

        ReferenceFrame frame = null;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
            if (this.frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
            {
                this.AltitudeUnitsLabel.Visible = false;
                this.AltitudeUnits.Visible = false;
                this.altDepthLabel.Text = Language.GetLocalizedText(802, "Altitude (Meters)");
            }
        }

        public override bool Save()
        {
            bool failed = false;

            frame.Lat = ParseAndValidateCoordinate(Lattitude, frame.Lat, ref failed);
            frame.Lng = ParseAndValidateCoordinate(Longitude, frame.Lng, ref failed);
            frame.Altitude = ParseAndValidateDouble(Altitude, frame.Altitude, ref failed);
            try
            {
                frame.AltUnits = (AltUnits)Enum.Parse(typeof(AltUnits), AltitudeUnits.SelectedItem.ToString());
            }
            catch
            {
                failed = true;
            }
         
            return !failed;

        }

        private void FrameWizardPosition_Load(object sender, EventArgs e)
        {
            Lattitude.Text = frame.Lat.ToString();
            Longitude.Text = frame.Lng.ToString();
            Altitude.Text = frame.Altitude.ToString();

            AltitudeUnits.Items.AddRange(Enum.GetNames(typeof(AltUnits)));
            AltitudeUnits.SelectedIndex = (int)frame.AltUnits;
        }

        private void GetFromView_Click(object sender, EventArgs e)
        {
            Vector2d point = RenderEngine.Engine.GetEarthCoordinates();


            Lattitude.Text = Coordinates.FormatDMS(point.Y);
            Longitude.Text = Coordinates.FormatDMS(point.X);
            Altitude.Text = RenderEngine.Engine.GetEarthAltitude().ToString();
        }

        private void AltitudeUnits_SelectionChanged(object sender, EventArgs e)
        {
            frame.AltUnits = (AltUnits)AltitudeUnits.SelectedIndex;
        }
    }
}
