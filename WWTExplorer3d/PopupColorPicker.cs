using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
            this.label1.Text = Language.GetLocalizedText(305, "Opacity");
            this.ok.Text = Language.GetLocalizedText(156, "OK");
        }

        public Color Color = Color.White;

        private void colors_MouseClick(object sender, MouseEventArgs e)
        {
            Bitmap bmp = (Bitmap)colors.Image;
            Color temp = bmp.GetPixel(e.X, e.Y);
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
            Bitmap bmp = (Bitmap)colors.Image;
            colors.Height = bmp.Height;
            colors.Width = bmp.Width;

            Rectangle rect = Screen.GetWorkingArea(this);

            if (this.Left + this.Width > rect.Width)
            {
                this.Left -= (this.Left + this.Width) - rect.Width;
            }
            if (this.Top + this.Height > (rect.Height-120))
            {
                this.Top -= (this.Top + this.Height) - (rect.Height-120);
            }
        }

        private void colors_Click(object sender, EventArgs e)
        {


        }

        private void oldColor_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Color temp = oldColor.BackColor;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);

            Close();
        }

        private void newColor_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Color temp = newColor.BackColor;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);

            Close();
        }

        private void PopupColorPicker_Deactivate(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), Color.R, Color.G, Color.B);
            this.Close();
        }

        private void PopupColorPicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Color temp = oldColor.BackColor;
                Color = Color.FromArgb((int)((float)opacity.Value * 255 / 100), temp.R, temp.G, temp.B);
                this.Close();
            }
        }

        private void colors_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}