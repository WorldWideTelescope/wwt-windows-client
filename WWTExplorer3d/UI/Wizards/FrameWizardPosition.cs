using System;

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
            altDepthLabel.Text = Language.GetLocalizedText(802, "Altitude (Meters)");
            LongitudeLabel.Text = Language.GetLocalizedText(803, "Longitude (Decimal Degrees)");
            LatitudeLabel.Text = Language.GetLocalizedText(804, "Latitude (Decimal Degrees)");
            label3.Text = Language.GetLocalizedText(801, "Select the Latitude, Longitude and Altitude");
            GetFromView.Text = Language.GetLocalizedText(259, "Get From View");
        }

        ReferenceFrame frame;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }
        public override bool Save()
        {
            var failed = false;

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
            var point = Earth3d.MainWindow.GetEarthCoordinates();


            Lattitude.Text = Coordinates.FormatDMS(point.Y);
            Longitude.Text = Coordinates.FormatDMS(point.X);
            Altitude.Text = Earth3d.MainWindow.GetEarthAltitude().ToString();
        }
    }
}
