using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class VORegistryProperties : Form
    {
        public VORegistryProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }

        void SetUiStrings()
        {
            searchUrlLabel.Text = Language.GetLocalizedText(603, "Base URL");
        }
    }
}