using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    [DefaultEvent("ValueChanged")]
    public partial class WwtTrackBar : UserControl
    {
        public event EventHandler ValueChanged;
        readonly Bitmap bmpTrackBarGripperNormal = Resources.trackbar_gripper_normal;
        readonly Bitmap bmpTrackBarGripperHover = Resources.trackbar_gripper_hover;
        readonly Bitmap bmpTrackBarGripperDisabled = Resources.trackbar_gripper_disabled;
        readonly Bitmap bmpTrackBarGripperPressed = Resources.trackbar_gripper_pressed;
        readonly Bitmap bmpTrackBarTrackNormal = Resources.trackbar_track_normal;
        readonly Bitmap bmpTrackBarTrackDisabled = Resources.trackbar_track_disabled;
        readonly Bitmap bmpTrackbarSelectionNormal = Resources.trackbar_selection_normal;
        public WwtTrackBar()
        {
            InitializeComponent();
        }
        int value = 50;

        public int Value
        {
            get { return value; }
            set
            {
                if (Value != value)
                {
                    this.value = value;
                    Refresh();
                }
            }
        }
        int max = 100;

        public int Max
        {
            get { return max; }
            set
            {
                if (max != value)
                {
                    max = value;
                    Refresh();
                }
            }
        }   


        private void WwtTrackBar_Paint(object sender, PaintEventArgs e)
        {
            var val = value * 60 / max + 10;

            if (Enabled)
            {
                e.Graphics.DrawImage(bmpTrackbarSelectionNormal, new Rectangle(0, 0, val, 20), new Rectangle(0, 0, val, 20), GraphicsUnit.Pixel);
                e.Graphics.DrawImage(bmpTrackBarTrackNormal, new Rectangle(val, 0, (80 - (val)), 20), new Rectangle(val, 0, (80 - (val)), 20), GraphicsUnit.Pixel);

                if (mouseDown)
                {
                    e.Graphics.DrawImage(bmpTrackBarGripperPressed, new Rectangle(val - 10, 0, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                }
                else
                {
                    if (hover)
                    {
                        e.Graphics.DrawImage(bmpTrackBarGripperHover, new Rectangle(val - 10, 0, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        e.Graphics.DrawImage(bmpTrackBarGripperNormal, new Rectangle(val - 10, 0, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);
                    }
                }
            }
            else
            {
                e.Graphics.DrawImage(bmpTrackBarTrackDisabled, new Rectangle(0, 0, 80, 20), new Rectangle(0, 0, 80, 20), GraphicsUnit.Pixel);
                e.Graphics.DrawImage(bmpTrackBarGripperDisabled, new Rectangle(val - 10, 0, 20, 20), new Rectangle(0, 0, 20, 20), GraphicsUnit.Pixel);

            }
            

        }
        bool mouseDown;
        private void WwtTrackBar_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            UpdateTrackbar(e);
        }

        private void WwtTrackBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                UpdateTrackbar(e);
                             
            }
        }

        private void UpdateTrackbar(MouseEventArgs e)
        {
            var newVal = Math.Max(0, Math.Min((e.X - 10) * max / 60, max));
            if (newVal != value)
            {
                value = newVal;
                if (ValueChanged != null)
                {
                    ValueChanged.Invoke(this, new ScrollEventArgs(ScrollEventType.ThumbTrack, value));
                }
                Refresh();
            }
        }

        private void WwtTrackBar_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            if (ValueChanged != null)
            {
                ValueChanged.Invoke(this, new ScrollEventArgs(ScrollEventType.EndScroll, value));
            }
        }
        bool hover;
        private void WwtTrackBar_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            Refresh();
        }

        private void WwtTrackBar_MouseLeave(object sender, EventArgs e)
        {
            hover = false;
            Refresh();

        }


    }
}
