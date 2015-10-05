using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class PopupVolume : Form
    {
        public PopupVolume()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            ok.Text = Language.GetLocalizedText(156, "OK");
        }

        public int Volume = 100;

        private void ok_Click(object sender, EventArgs e)
        {

            Volume = volume.Value;
        }

        private void PopupVolume_Load(object sender, EventArgs e)
        {
            volume.Value = Volume;
        }
    }
}