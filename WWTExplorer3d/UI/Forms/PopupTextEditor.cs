using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class PopupTextEditor : Form
    {
        public PopupTextEditor()
        {
            InitializeComponent();
        }
        public TourStop target = null;

        private void PopupTextEditor_Load(object sender, EventArgs e)
        {
            textEdit.Font = UiTools.StandardRegular;
            if (target != null)
            {
                textEdit.Text = target.Description;
            }
            Earth3d.NoStealFocus = true;
        }

        private void PopupTextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                this.Close();
            }
        }

        private void PopupTextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void PopupTextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                target.Description = textEdit.Text;
                Earth3d.NoStealFocus = false;
            }
            catch
            {
            }
        }

        private void PopupTextEditor_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void textEdit_TextChanged(object sender, EventArgs e)
        {
            target.Description = textEdit.Text;
        }
    }
}