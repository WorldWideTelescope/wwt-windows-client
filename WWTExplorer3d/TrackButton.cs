using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TrackButton : UserControl
    {
        public event EventHandler ValueChanged;
 
        public TrackButton()
        {
            InitializeComponent();
        }

        private int val;

        private string labelText = "";

        public string LabelText
        {
            get
            {
                if (label != null)
                {
                    labelText = label.Text;
                }
                return labelText;
            }
            set
            {
                if (labelText != value)
                {
                    labelText = value;
                    if (label != null)
                    {
                        label.Text = labelText;
                    }
                }
            }
        }


        public int Value
        {
            get
            {
                if (slider != null)
                {
                    val = slider.Value;
                }
                return val;
            }
            set
            {
                if (val != value)
                {
                    val = value;
                    if (slider != null)
                    {
                        slider.Value = val;
                    }
                }
            }
        }


        private void TrackButton_Load(object sender, EventArgs e)
        {
            slider.Max = 127;
            slider.Value = val;
            label.Text = labelText;
        }

        private void slider_ValueChanged(object sender, EventArgs e)
        {
            val = slider.Value;
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, new EventArgs());
            }
        }
    }
}
