using System;

namespace TerraViewer
{
    public partial class FrameWizardCartisian : PropPage
    {
        public FrameWizardCartisian()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        { 
            label3.Text = Language.GetLocalizedText(801, "Select the Latitude, Longitude and Altitude");
            altDepthLabel.Text = Language.GetLocalizedText(802, "Altitude (Meters)");
            longRALable.Text = Language.GetLocalizedText(803, "Longitude (Decimal Degrees)");
            latDecLabel.Text = Language.GetLocalizedText(804, "Latitude (Decimal Degrees)");

        }

        private void FrameWizardCartisian_Load(object sender, EventArgs e)
        {

        }
    }
}
