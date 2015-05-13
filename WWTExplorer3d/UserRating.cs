using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    [DefaultEvent("ValueChanged")]

    public partial class UserRating : UserControl
    {
        public UserRating()
        {
            InitializeComponent();
        }

        public event EventHandler ValueChanged;
        public enum Interactivity { ReadOnly, ReadWrite };

        Interactivity mode = Interactivity.ReadOnly;

        public Interactivity Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public enum StarSizes { Big, Small };
        StarSizes starSize = StarSizes.Big;

        public StarSizes StarSize
        {
            get { return starSize; }
            set
            {
                starSize = value;
                this.SetStarSize();
                this.Refresh();
            }
        }

        double stars = 2.5;

        public double Stars
        {
            get { return stars; }
            set
            {
                stars = value;
                this.Refresh();
            }
        }

        private void UserRating_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
            if (starSize == StarSizes.Big)
            {
                Rectangle rect = new Rectangle(0, 0, (int)(stars * 25.4), 24);
                e.Graphics.DrawImageUnscaled(Properties.Resources.StarRatingBackground, 0, 0);
                e.Graphics.DrawImage(Properties.Resources.StarRatingForeground, rect, rect, GraphicsUnit.Pixel);
            }
            else
            {
                Rectangle rect = new Rectangle(0, 0, (int)(stars * 14), 24);
                e.Graphics.DrawImageUnscaled(Properties.Resources.StarRatingBackgroundSmall, 0, 0);
                e.Graphics.DrawImage(Properties.Resources.StarRatingForegroundSmall, rect, rect, GraphicsUnit.Pixel);
            }
        }

        private void UserRating_Load(object sender, EventArgs e)
        {
            SetStarSize();
        }

        private void SetStarSize()
        {
            if (starSize == StarSizes.Big)
            {
                this.Height = 24;
                this.Width = 128;
            }
            else
            {
                this.Width = 72;
                this.Height = 16;
            }
            Invalidate();
        }
        bool down = false;
        private void UserRating_MouseDown(object sender, MouseEventArgs e)
        {
            down = true;
            UpsateStarCount(e.X);
        }

        private void UpsateStarCount(int xPos)
        {
            if (mode == Interactivity.ReadWrite)
            {
                double blockSize = starSize == StarSizes.Big ? 25.4 : 14;

                int count = Math.Min(5, Math.Max(0, ((int)(xPos / blockSize) + 1)));

                stars = count;
                if (ValueChanged != null)
                {
                    ValueChanged.Invoke(this, new EventArgs());
                }
                Refresh();
            }
        }

        private void UserRating_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;

        }

        private void UserRating_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                UpsateStarCount(e.X);
            }   
        }
    }
}
