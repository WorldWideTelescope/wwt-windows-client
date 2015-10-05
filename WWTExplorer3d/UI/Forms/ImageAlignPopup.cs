using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ImageAlignPopup : Form
    {
        public ImageAlignPopup()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(864, "Ctrl+E               Toggle Edit Mode\nMouse Drag     Move Image\nShift+Drag        Scale Image\nCtrl+Drag          Rotate Image\nAlt+                   Fine Adjustment Scale/Rotate\nRight Click        Pick Pivot + Toggle Pivot Mode\nC                       Centers Image to View\n\nPivot Mode Instructions:\n* Align a star in both image and background.\n* Right Click on centroid of aligned star to select pivot.\n* Left Drag on a second star and align it to background.\n* Right Click to exit pivot mode.");
            Text = Language.GetLocalizedText(865, "Image Alignment");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ImageAlignUI_Load(object sender, EventArgs e)
        {

        }

        internal void SetPivotMode(bool anchored)
        {
            Text = "Image Alignment" + (anchored ? " - Pivot Mode" : "");
        }

        private void ImageAlignPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Earth3d.MainWindow.UiController = null;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Earth3d.MainWindow.Focus();
        }
        
    }
}