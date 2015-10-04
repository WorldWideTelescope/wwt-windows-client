using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DragThumbnail : Form
    {
        public DragThumbnail()
        {
            InitializeComponent();
        }
        public Bitmap Thumbnail
        {
            get
            {
                return (Bitmap)pictureBox1.Image;
            }
            set
            {
                pictureBox1.Image = value;
            }
        }
    }
}