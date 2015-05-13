using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

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
            this.label3.Text = Language.GetLocalizedText(801, "Select the Latitude, Longitude and Altitude");
            this.altDepthLabel.Text = Language.GetLocalizedText(802, "Altitude (Meters)");
            this.longRALable.Text = Language.GetLocalizedText(803, "Longitude (Decimal Degrees)");
            this.latDecLabel.Text = Language.GetLocalizedText(804, "Latitude (Decimal Degrees)");

        }

        private void FrameWizardCartisian_Load(object sender, EventArgs e)
        {

        }
    }
}
