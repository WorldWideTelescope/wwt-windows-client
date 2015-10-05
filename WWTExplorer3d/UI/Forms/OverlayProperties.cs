using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class OverlayProperties : Form
    {
        public OverlayProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            OK.Text = Language.GetLocalizedText(156, "OK");
            label2.Text = Language.GetLocalizedText(209, "Width");
            label3.Text = "Height";
            Text = "Overlay Properties";
            posXLabel.Text = "X";
            label1.Text = "Y";
            NameLabel.Text = Language.GetLocalizedText(238, "Name");
        }

        public Overlay Overlay = null;


        private void OverlayProperties_Load(object sender, EventArgs e)
        {
            if (Overlay != null)
            {
                positionX.Text = Overlay.Position.X.ToString();
                positionY.Text = Overlay.Position.Y.ToString();
                sizeX.Text = Overlay.Width.ToString();
                sizeY.Text = Overlay.Height.ToString();
                Rotation.Text = Overlay.RotationAngle.ToString();
                OverlayName.Text = Overlay.Name;
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void positionX_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            var val  = UiTools.ParseAndValidateDouble(sender as TextBox, Overlay.Position.X, ref failed);

            if (!failed)
            {
                Overlay.Position = new PointF((float)val, Overlay.Position.Y);
            }

        }

        private void positionY_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            var val = UiTools.ParseAndValidateDouble(sender as TextBox, Overlay.Position.X, ref failed);

            if (!failed)
            {
                Overlay.Position = new PointF(Overlay.Position.X, (float)val);
            }

        }

        private void sizeX_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            var val = UiTools.ParseAndValidateDouble(sender as TextBox, Overlay.Width, ref failed);

            if (!failed)
            {
                Overlay.Width = (float)val;
            }
        }

        private void sizeY_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            var val = UiTools.ParseAndValidateDouble(sender as TextBox, Overlay.Height, ref failed);

            if (!failed)
            {
                Overlay.Height = (float)val;
            }
        }

        private void Name_TextChanged(object sender, EventArgs e)
        {
            if (Overlay != null)
            {
                Overlay.Name = OverlayName.Text;
            }
        }

        private void Rotation_TextChanged(object sender, EventArgs e)
        {
            var failed = false;

            var val = UiTools.ParseAndValidateDouble(sender as TextBox, Overlay.RotationAngle, ref failed);

            if (!failed)
            {
                Overlay.RotationAngle = (float)val;
            }
        }
    }
}
