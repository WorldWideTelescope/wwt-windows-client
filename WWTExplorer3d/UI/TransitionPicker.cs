using System;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public partial class TransitionPicker : UserControl
    {
        public TransitionPicker()
        {
            InitializeComponent();


        }


        public event EventHandler SelectedIndexChanged;

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    if (SelectedIndexChanged != null)
                    {
                        SelectedIndexChanged.Invoke(this, new EventArgs());
                    }
                    Refresh();
                }
            }
        }



        public Bitmap[] images = new Bitmap[] { 
            Resources.TransArrow,
            Resources.TransCrossfade,
            Resources.TransCut,
            Resources.TransFadeOutIn,  
            Resources.TransFadeIn,
            Resources.TransFadeOut,    
        };


        private void TransitionPicker_Paint(object sender, PaintEventArgs e)
        {
            for (var i = 0; i < images.Length; i++)
            {
                if (SelectedIndex == i)
                {
                    e.Graphics.DrawImage(Resources.TransHighlight, i * 44+4, 4);
                }
                else
                {
                    e.Graphics.DrawImage(Resources.TransBackground, i * 44+4, 4);
                }
                e.Graphics.DrawImage(images[i], i * 44 + 7, 6);
            }
        }

        private void TransitionPicker_MouseDown(object sender, MouseEventArgs e)
        {
            var indexHit = e.X / 44;

            indexHit = Math.Min(indexHit, 5);

            SelectedIndex = indexHit;
            
        }
    }
}
