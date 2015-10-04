using System;
using System.Drawing;
using System.Windows.Forms;

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
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Decline_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void eulaText_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            WebWindow.OpenUrl(e.LinkText, true);
        }

       
    }
}
