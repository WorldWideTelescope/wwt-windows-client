using System;
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
