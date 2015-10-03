using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TransitionPicker : UserControl
    {
        public TransitionPicker()
        {
            InitializeComponent();


        }


        public event EventHandler SelectedIndexChanged;

        private int selectedIndex = 0;

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
            global::TerraViewer.Properties.Resources.TransArrow,
            global::TerraViewer.Properties.Resources.TransCrossfade,
            global::TerraViewer.Properties.Resources.TransCut,
            global::TerraViewer.Properties.Resources.TransFadeOutIn,  
            global::TerraViewer.Properties.Resources.TransFadeIn,
            global::TerraViewer.Properties.Resources.TransFadeOut,    
        };


        private void TransitionPicker_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < images.Length; i++)
            {
                if (SelectedIndex == i)
                {
                    e.Graphics.DrawImage(global::TerraViewer.Properties.Resources.TransHighlight, i * 44+4, 4);
                }
                else
                {
                    e.Graphics.DrawImage(global::TerraViewer.Properties.Resources.TransBackground, i * 44+4, 4);
                }
                e.Graphics.DrawImage(images[i], i * 44 + 7, 6);
            }
        }

        private void TransitionPicker_MouseDown(object sender, MouseEventArgs e)
        {
            int indexHit = e.X / 44;

            indexHit = Math.Min(indexHit, 5);

            SelectedIndex = indexHit;
            
        }
    }
}
