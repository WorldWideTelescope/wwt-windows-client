using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class PopupColorPicker : Form
    {
        public PopupColorPicker()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(305, "Opacity");
            ok.Text = Language.GetLocalizedText(156, "OK");
        }

        public Color Color = Color.White;

        private void colors_MouseClick(object sender, MouseEventArgs e)
        {
            var bmp = (Bitmap)colors.Image;
            var temp = bmp.GetPixel(e.X, e.Y);
            Color = Color.FromArgb((int)((float)opacity.Value*255/100), temp.R, temp.G, temp.B );
            newColor.BackColor = temp;
        }

        private void PopupColorPicker_Leave(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void colors_MouseMove(object sender, MouseEventArgs e)
        {
            //Bitmap bmp = (Bitmap)colors.Image;
            //if (e.Y > -1 && e.Y < bmp.Height && e.X > -1 && e.X < bmp.Width)
            //{
            //    Color = bmp.GetPixel(e.X, e.Y);
            //    newColor.BackColor = Color;
            //}
        }

        private void PopupColorPicker_Load(object sender, EventArgs e)
        {
            oldColor.BackColor = Color;
            newColor.BackColor = Color;
            opacity.Value = Color.A * 100 / 255;
            var bmp = (Bitmap)colors.Image;
            colors.Height = bmp.Height;
            colors.Width = bmp.Width;

            var rect = Screen.GetWorkingArea(this);

            if (Left + Width > rect.Width)
            {
                Left -= (Left + Width) - rect.Width;
            }
            if (Top + Height > (rect.Height-120))
            {
                Top -= (Top + Height) - (rect.Height-120);
            }
        }

        private void colors_Click(object sender, EventArgs e)
        {


        }

        private void oldColor_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            var temp = oldColor.BackColor;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);

            Close();
        }

        private void newColor_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            var temp = newColor.BackColor;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);

            Close();
        }

        private void PopupColorPicker_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), Color.R, Color.G, Color.B);
            Close();
        }

        private void PopupColorPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                var temp = oldColor.BackColor;
                Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);
                Close();
            }
        }

        private void colors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}