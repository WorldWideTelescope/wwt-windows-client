using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class EulaReader : Form
    {
        public EulaReader()
        {
            InitializeComponent();
        }

        private void EulaReader_Load(object sender, EventArgs e)
        {
            try
            {
                eulaText.LoadFile("Terms_of_use_Nov_2011.rtf");
                eulaText.SelectAll();
                eulaText.SelectionColor = Color.White;
                eulaText.Select(0, 0);
            }
            catch
            {
            }
        }

        private void Accept_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void Decline_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void eulaText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            WebWindow.OpenUrl(e.LinkText, true);
        }

       
    }
}
