using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class RenderTarget : UserControl
    {
        public RenderTarget()
        {
            InitializeComponent();
        }
        protected override void OnResize(EventArgs e)
        {
            try
            {
                base.OnResize(e);
            }
            catch
            {
            }
        }
    }
}
