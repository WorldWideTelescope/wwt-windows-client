using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class SimbadSearch : Form
    {
        public SimbadSearch()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.txtName.Text = Language.GetLocalizedText(255, "<Type name here>");
            this.label1.Text = Language.GetLocalizedText(362, "Object Name");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Text = Language.GetLocalizedText(363, "SIMBAD Search");
        }
        string objectName = "";

        public string ObejctName
        {
            get { return objectName; }
            set { objectName = value; }
        }


        private void OK_Click(object sender, EventArgs e)
        {
            objectName = txtName.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void SimbadSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, e);
            }
        }

        private void SimbadSearch_Load(object sender, EventArgs e)
        {
            txtName.Text = objectName;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OK_Click(this, e);
            }
        }


    }
}