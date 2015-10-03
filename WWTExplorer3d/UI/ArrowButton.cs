using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    [DefaultEvent("Pushed")]
    public partial class ArrowButton : UserControl
    {
        public ArrowButton()
        {
            InitializeComponent();
        }

        public event EventHandler Pushed;

        private Bitmap bmpNormal;
        private Bitmap bmpHover;
        private Bitmap bmpPressed;
        private Bitmap bmpDisabled;

        private void ArrowButton_Load(object sender, EventArgs e)
        {
            LoadBitmaps();
        }

        private void LoadBitmaps()
        {
            if (buttonSize == ButtonSizes.Big)
            {
                if (buttonType == ButtonTypes.Up)
                {
                    bmpNormal = Properties.Resources.button_arrow_up_normal;
                    bmpHover = Properties.Resources.button_arrow_up_hover;
                    bmpPressed = Properties.Resources.button_arrow_up_pressed;
                    bmpDisabled = Properties.Resources.button_arrow_up_disabled;
                }
                else
                {
                    bmpNormal = Properties.Resources.button_arrow_down_normal;
                    bmpHover = Properties.Resources.button_arrow_down_hover;
                    bmpPressed = Properties.Resources.button_arrow_down_pressed;
                    bmpDisabled = Properties.Resources.button_arrow_down_disabled;
                }
            }
            else
            {
                this.Width = 33;
                this.Height = 15;
                if (buttonType == ButtonTypes.Up)
                {
                    bmpNormal = Properties.Resources.PinT_C_R;
                    bmpHover = Properties.Resources.PinT_C_H;
                    bmpPressed = Properties.Resources.PinT_C_P;
                    bmpDisabled = Properties.Resources.PinT_C_D;
                }
                else
                {
                    bmpNormal = Properties.Resources.PinB_C_R;
                    bmpHover = Properties.Resources.PinB_C_H;
                    bmpPressed = Properties.Resources.PinB_C_P;
                    bmpDisabled = Properties.Resources.PinB_C_D;
                }
            }
        }

        public enum ButtonTypes { Up, Down };

        ButtonTypes buttonType = ButtonTypes.Up;

        public ButtonTypes ButtonType
        {
            get { return buttonType; }
            set
            {
                if (buttonType != value)
                {
                    buttonType = value;
                    LoadBitmaps();
                }
            }
        }
        public enum ButtonSizes { Big, Small };

        ButtonSizes buttonSize = ButtonSizes.Big;

        public ButtonSizes ButtonSize
        {
            get { return buttonSize; }
            set { buttonSize = value; }
        }

        bool hover = false;
        bool pressed = false;

        private void ArrowButton_Paint(object sender, PaintEventArgs e)
        {
            if (Enabled)
            {
                if (pressed)
                {
                    e.Graphics.DrawImageUnscaled(bmpPressed, 0, 0);
                }
                else if (hover)
                {
                    e.Graphics.DrawImageUnscaled(bmpHover, 0, 0);
                }
                else
                {
                    e.Graphics.DrawImageUnscaled(bmpNormal, 0, 0);
                }
            }
            else
            {
                e.Graphics.DrawImageUnscaled(bmpDisabled, 0, 0);
            }
        }

        private void ArrowButton_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
            if (Pushed != null)
            {
                Pushed.Invoke(this, new EventArgs());
            }
            repeatTimer.Interval = 400;
            repeatTimer.Enabled = true;
            Refresh();
        }

        private void ArrowButton_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            Refresh();
        }

        private void ArrowButton_MouseLeave(object sender, EventArgs e)
        {
            hover = false;
            Refresh();
        }

        private void ArrowButton_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
            Refresh();
        }

        private void repeatTimer_Tick(object sender, EventArgs e)
        {
            if (Pushed != null && pressed)
            {
                Pushed.Invoke(this, new EventArgs());
            }
            repeatTimer.Interval = 150;
            repeatTimer.Enabled = true;
        }
    }
}
