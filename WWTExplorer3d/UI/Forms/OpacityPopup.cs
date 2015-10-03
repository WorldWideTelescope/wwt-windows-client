using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class OpacityPopup : Form
    {
        public OpacityPopup()
        {
            InitializeComponent();
            valueSlider.MouseCaptureChanged += new EventHandler(OpacityPopup_MouseCaptureChanged);
        }

        void OpacityPopup_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (!Capture)
            {
                Close();
            }
        }


        Layer target;

        public Layer Target
        {
            get { return target; }
            set
            {
                target = value;
                CurrentOpacity = target.Opacity;
            }
        }
        float opacity = 1;

        public float CurrentOpacity
        {
            get { return opacity ; }
            set
            {
                opacity = value;
                valueSlider.Value = (int)(opacity * 100);
            }
        }


        private void valueSlider_ValueChanged(object sender, EventArgs e)
        {
            opacity = (float)valueSlider.Value / 100.0f;

            target.Opacity = opacity;

        }

        private void OpacityPopup_Load(object sender, EventArgs e)
        {
            valueSlider.Value = (int)(opacity * 100);
            valueSlider.Capture = true;
            
        }

        private void closeBox_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OpacityPopup_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OpacityPopup_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.X < 0 || e.X > valueSlider.Width || e.Y < 0 || e.Y > valueSlider.Height)
            {
                Close();
            }
        }

        private void valueSlider_Leave(object sender, EventArgs e)
        {
            
        }
    }
}
