using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class KioskTitleBar : UserControl
    {
        public KioskTitleBar()
        {
            InitializeComponent();

            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.titleBar.Text = Language.GetLocalizedText(866, "Microsoft WorldWide Telescope - Take it home with you at http://www.worldwidetelescope.org");
        }
    }
}
