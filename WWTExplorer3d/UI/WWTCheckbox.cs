using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    [DefaultEvent("CheckedChanged")]

    public partial class WWTCheckbox : UserControl
    {
        Bitmap uncheckedRest = global::TerraViewer.Properties.Resources.checkbox_unchecked_rest;
        Bitmap checkedRest = global::TerraViewer.Properties.Resources.checkbox_checked_rest;
        Bitmap uncheckedHover = global::TerraViewer.Properties.Resources.checkbox_unchecked_hover;
        Bitmap checkedHover = global::TerraViewer.Properties.Resources.checkbox_checked_hover;
        Bitmap uncheckedPush = global::TerraViewer.Properties.Resources.checkbox_unchecked_press;
        Bitmap checkedPush = global::TerraViewer.Properties.Resources.checkbox_checked_press;
        Bitmap uncheckedDisabled = global::TerraViewer.Properties.Resources.checkbox_unchecked_disabled;
        Bitmap checkedDisabled = global::TerraViewer.Properties.Resources.checkbox_checked_disabled;

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
                this.Invalidate();
                base.Text = value;
            }
        }

        bool pressed = false;
        bool hover = false;
        bool isChecked = false;
    
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
            Graphics g = e.Graphics;
            RectangleF rectf = new RectangleF(25, 0, Width-15, Height);



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

                g.DrawString(this.Text, UiTools.StandardRegular, UiTools.StadardTextBrush, rectf, UiTools.StringFormatCenterLeft);
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
                g.DrawString(this.Text, UiTools.StandardRegular, UiTools.DisabledTextBrush, rectf, UiTools.StringFormatCenterLeft);
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
