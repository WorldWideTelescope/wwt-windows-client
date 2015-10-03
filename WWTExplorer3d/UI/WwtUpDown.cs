using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    [DefaultEvent("Up")]
    public partial class WwtUpDown : UserControl
    {
        public event EventHandler Up;
        public event EventHandler Down;

        
        public WwtUpDown()
        {
            InitializeComponent();
        }

        private void down_Pushed(object sender, EventArgs e)
        {
            if (Down != null)
            {
                Down.Invoke(this, e);
            }
        }

        private void up_Pushed(object sender, EventArgs e)
        {
            if (Up != null)
            {
                Up.Invoke(this, e);
            }
        }
    }
}
