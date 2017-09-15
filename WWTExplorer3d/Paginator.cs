using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{

  
    public partial class Paginator : UserControl
    {
        public Paginator()
        {
            InitializeComponent();
        }

        Bitmap leftArrow = global::TerraViewer.Properties.Resources.LeftArrow;
        Bitmap rightArrow = global::TerraViewer.Properties.Resources.RightArrow;
        Bitmap leftArrowDisabled = global::TerraViewer.Properties.Resources.LeftArrowDisabled;
        Bitmap rightArrowDisabled = global::TerraViewer.Properties.Resources.RightArrowDisabled;
   
        public event PageChangedEventHandler PageChanged;

        int currentPage;

        public int CurrentPage
        {
            get { return currentPage; }
            set 
            {
                if (currentPage != value)
                {
                    currentPage = value;
                    Invalidate();
                }
            }
        }
        int totalPages;

        public int TotalPages
        {
            get { return totalPages; }
            set
            {
                if (totalPages != value)
                {
                    totalPages = value;
                    Refresh();
                }
            }
        }

        private void Paginator_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            String text = String.Format(Language.GetLocalizedText(290, "{0} of {1}"), currentPage + 1, totalPages);
            RectangleF rectf = new RectangleF(0,0,Width,Height);
            if (currentPage > 0)
            {
                g.DrawImage(leftArrow,0,Height/2 - 8);
            }
            else
            {
                g.DrawImage(leftArrowDisabled,0,Height/2 - 8);
            }

            if ((currentPage+1) != totalPages)
            {
                g.DrawImage(rightArrow, Width - 14, Height / 2 - 8);
            }
            else
            {
                g.DrawImage(rightArrowDisabled, Width - 14, Height / 2 - 8);
            }

            g.DrawString(text,UiTools.StandardRegular,UiTools.StadardTextBrush, rectf,UiTools.StringFormatCenterCenter);
        }


        private void Paginator_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.X < 15)
            {
                if (PageChanged != null && currentPage > 0)
                {
                    PageChanged.Invoke(this,PageChange.Back);
                    currentPage--;
                    Refresh();
                }
            }
            if (e.X > (Width-15))
            {
                if (PageChanged != null && (currentPage+1) < totalPages)
                {
                    PageChanged.Invoke(this,PageChange.Next);
                    currentPage++;
                    Refresh();
                }
            }
        }

        private void Paginator_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X < 15)
            {
                if (PageChanged != null && currentPage > 0)
                {
                    PageChanged.Invoke(this,PageChange.First);
                    currentPage = 0;
                    Refresh();
                }
            }
            if (e.X > (Width-15))
            {
                if (PageChanged != null && (currentPage+1) < totalPages)
                {
                    PageChanged.Invoke(this,PageChange.Last);
                    currentPage = totalPages-1;
                    Refresh();
                }
            }
        }

        private void Paginator_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Enabled = false;
        }

        private void Paginator_MouseLeave(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Paginator_MouseClick(this, downArgs);
        }

        MouseEventArgs downArgs = null;


        private void Paginator_MouseDown(object sender, MouseEventArgs e)
        {
            downArgs = e;
            timer1.Interval = 250;
            timer1.Enabled = true;

        }
    }
}
