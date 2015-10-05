using System;
using System.Drawing;
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
            var dialog = new DBmp();

            dialog.pictureBox1.Image = image;
            dialog.Show();
        }

    }
}