using System;
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
            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            Text = Language.GetLocalizedText(901, "RenderProgress");
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
