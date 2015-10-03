using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class RenderProgress : Form
    {
        public RenderProgress()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.Text = Language.GetLocalizedText(901, "RenderProgress");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!SpaceTimeController.FrameDumping)
            {
                Close();
            }
            else
            {
                FrameCountText.Text = string.Format(Language.GetLocalizedText(900, "Rendering frame {0} of {1}."), SpaceTimeController.CurrentFrameNumber, SpaceTimeController.TotalFrames);
                progressBar.Value = (100*SpaceTimeController.CurrentFrameNumber)/( SpaceTimeController.TotalFrames);
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            SpaceTimeController.CancelFrameDump = true;
        }
    }
}
