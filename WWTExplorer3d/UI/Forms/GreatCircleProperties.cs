using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class GreatCircleProperties : Form
    {
        public GreatCircleProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label5.Text = Language.GetLocalizedText(1005, "Percentage");
            label4.Text = Language.GetLocalizedText(950, "Lng");
            label3.Text = Language.GetLocalizedText(951, "Lat");
            label1.Text = Language.GetLocalizedText(951, "Lat");
            ok.Text = Language.GetLocalizedText(759, "Ok");
            label6.Text = Language.GetLocalizedText(1006, "Line Width");
            StartFromView.Text = Language.GetLocalizedText(1007, "<< Get From View");
            EndFromView.Text = Language.GetLocalizedText(1007, "<< Get From View");
            Text = Language.GetLocalizedText(1008, "GreatCircleProperties");

        }
        public GreatCirlceRouteLayer Layer = null;

        private void StartFromView_Click(object sender, EventArgs e)
        {

            var pnt = Earth3d.MainWindow.GetEarthCoordinates();
            StartLat.Text = Coordinates.FormatDMS(pnt.Y);
            StartLng.Text = Coordinates.FormatDMS(pnt.X);
        }

        private void EndFromView_Click(object sender, EventArgs e)
        {
            var pnt = Earth3d.MainWindow.GetEarthCoordinates();
            EndLat.Text = Coordinates.FormatDMS(pnt.Y);
            EndLng.Text = Coordinates.FormatDMS(pnt.X);
        }

        private void StartLat_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.LatStart = UiTools.ParseAndValidateCoordinate(StartLat, Layer.LatStart, ref failed);
            Layer.CleanUp();
        }

        private void StartLng_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.LngStart = UiTools.ParseAndValidateCoordinate(StartLng, Layer.LngStart, ref failed);
            Layer.CleanUp();
        }

        private void EndLat_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.LatEnd = UiTools.ParseAndValidateCoordinate(EndLat, Layer.LatEnd, ref failed);
            Layer.CleanUp();
        }

        private void EndLng_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.LngEnd = UiTools.ParseAndValidateCoordinate(EndLng, Layer.LngEnd, ref failed);
            Layer.CleanUp();
        }

        private void Percentage_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.PercentComplete = UiTools.ParseAndValidateCoordinate(Percentage, Layer.PercentComplete, ref failed);
            Layer.CleanUp();
        }

        private void LineWidth_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            Layer.Width = UiTools.ParseAndValidateCoordinate(LineWidth, Layer.Width, ref failed);
            Layer.CleanUp();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            Close();

        }

        private void GreatCircleProperties_Load(object sender, EventArgs e)
        {
            LineWidth.Text = Layer.Width.ToString();
            StartLat.Text = Layer.LatStart.ToString();
            StartLng.Text = Layer.LngStart.ToString();
            EndLat.Text = Layer.LatEnd.ToString();
            EndLng.Text = Layer.LngEnd.ToString();
            Percentage.Text = Layer.PercentComplete.ToString();
        }


    }
}
