using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DBmp : Form
    {
        public DBmp()
        {
            InitializeComponent();
        }

        private void DebugBitmap_Load(object sender, EventArgs e)
        {

        }
        public static void ShowBitmap(Bitmap image)
        {
            DBmp dialog = new DBmp();

            dialog.pictureBox1.Image = image;
            dialog.Show();
        }

    }
}