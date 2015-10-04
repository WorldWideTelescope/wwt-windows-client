using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    [DefaultEvent("CheckedChanged")]

    public partial class WWTCheckbox : UserControl
    {
        readonly Bitmap uncheckedRest = Resources.checkbox_unchecked_rest;
        readonly Bitmap checkedRest = Resources.checkbox_checked_rest;
        readonly Bitmap uncheckedHover = Resources.checkbox_unchecked_hover;
        readonly Bitmap checkedHover = Resources.checkbox_checked_hover;
        readonly Bitmap uncheckedPush = Resources.checkbox_unchecked_press;
        readonly Bitmap checkedPush = Resources.checkbox_checked_press;
        readonly Bitmap uncheckedDisabled = Resources.checkbox_unchecked_disabled;
        readonly Bitmap checkedDisabled = Resources.checkbox_checked_disabled;

        public WWTCheckbox()
        {
            InitializeComponent();
        }
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

        public override string Text
        {
            get
            {
                return base.Text;

            }
            set
            {
                Invalidate();
                base.Text = value;
            }
        }

        bool pressed;
        bool hover;
        bool isChecked;
    
        public event EventHandler CheckedChanged;

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Checked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    Refresh();
                    if (CheckedChanged != null)
                    {
                        CheckedChanged.Invoke(this, new EventArgs());
                    }
                }
            }
        }

        private void WWTCheckbox_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rectf = new RectangleF(25, 0, Width-15, Height);



            if (Enabled)
            {
                if (isChecked)
                {
                    if (pressed)
                    {
                        g.DrawImage(checkedPush, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                    else if (hover)
                    {
                        g.DrawImage(checkedHover, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                    else 
                    {
                        g.DrawImage(checkedRest, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    if (pressed)
                    {
                        g.DrawImage(uncheckedPush, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                    else if (hover)
                    {
                        g.DrawImage(uncheckedHover, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                    else 
                    {
                        g.DrawImage(uncheckedRest, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                    }
                }

                g.DrawString(Text, UiTools.StandardRegular, UiTools.StadardTextBrush, rectf, UiTools.StringFormatCenterLeft);
            }
            else
            {
                if (isChecked)
                {
                    g.DrawImage(checkedDisabled, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                }
                else
                {
                    g.DrawImage(uncheckedDisabled, new Rectangle(0, 0, 25, 25), new Rectangle(0, 0, 25, 25), GraphicsUnit.Pixel);
                }
                g.DrawString(Text, UiTools.StandardRegular, UiTools.DisabledTextBrush, rectf, UiTools.StringFormatCenterLeft);
            }
        }

        private void WWTCheckbox_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            Refresh();
        }

        private void WWTCheckbox_MouseLeave(object sender, EventArgs e)
        {
            pressed = false;
            hover = false;
            Refresh();
        }

        private void WWTCheckbox_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
            Refresh();
        }

        private void WWTCheckbox_MouseUp(object sender, MouseEventArgs e)
        {
            if (hover)
            {
                Checked = !Checked;
            }

            pressed = false;
            Refresh();
        }
    }
}
