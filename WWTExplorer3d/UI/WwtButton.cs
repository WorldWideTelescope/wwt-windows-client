using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    [DefaultEvent("Click")]

    public partial class WwtButton : UserControl, IButtonControl
    {
        readonly Bitmap btnStartRest = Resources.BtnStartRest;
        readonly Bitmap btnStartHover = Resources.BtnStartHover;
        readonly Bitmap btnStartPressed = Resources.BtnStartPushed;
        readonly Bitmap btnStartDisabled = Resources.BtnStartDisabled;
        Bitmap btnStartSelected = Resources.BtnStartSelected;

        readonly Bitmap btnEndRest = Resources.BtnEndRest;
        readonly Bitmap btnEndHover = Resources.BtnEndHover;
        readonly Bitmap btnEndPressed = Resources.BtnEndPushed;
        readonly Bitmap btnEndDisabled = Resources.BtnEndDisabled;
        Bitmap btnEndSelected = Resources.BtnEndSelected;

        public WwtButton()
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
        Bitmap imageEnabled;

        public Bitmap ImageEnabled
        {
            get { return imageEnabled; }
            set 
            {
                imageEnabled = value;
                Refresh();
            }
        }
        Bitmap imageDisabled;

        public Bitmap ImageDisabled
        {
            get { return imageDisabled; }
            set
            {
                imageDisabled = value;
                Refresh();
            }
        }

        private DialogResult dialogResult = 0;
                    
        public DialogResult DialogResult
        {
            get 
            {
                return dialogResult;
            }
            set
            {
                dialogResult = value; 
            }
        }


        bool selected;

        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        bool pressed;
        bool hover;


        private void WwtButton_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            var rectf = new RectangleF(0, 0, Width, Height);
            var start = btnStartRest;
            var end = btnEndRest;
          
            if (hover)
            {
                start = btnStartHover;
                end = btnEndHover;

            }

            if (pressed)
            {
                start = btnStartPressed;
                end = btnEndPressed;

            }



            if (Enabled)
            {
                g.DrawImage(start, new Rectangle(0, 0, Width / 2, 33), new Rectangle(0, 0, Width / 2, 33), GraphicsUnit.Pixel);
                g.DrawImage(end, new Rectangle(Width / 2, 0, Width / 2, 33), new Rectangle(btnEndRest.Width - Width / 2, 0, Width / 2, 33), GraphicsUnit.Pixel);
                if (imageEnabled != null)
                {
                    g.DrawImage(imageEnabled, new Rectangle((Width / 2)-(imageEnabled.Width/2), (Height/2)-(imageEnabled.Height/2), imageEnabled.Width,imageEnabled.Height), new Rectangle(0,0,imageEnabled.Width,ImageEnabled.Height), GraphicsUnit.Pixel);
                }
                g.DrawString(Text, UiTools.StandardRegular, UiTools.StadardTextBrush, rectf,UiTools.StringFormatCenterCenter);
            }
            else
            {
                g.DrawImage(btnStartDisabled, new Rectangle(0, 0, Width/2, 33), new Rectangle(0, 0, Width/2, 33), GraphicsUnit.Pixel);
                g.DrawImage(btnEndDisabled, new Rectangle(Width / 2, 0, Width / 2, 33), new Rectangle(btnEndRest.Width - Width / 2, 0, Width / 2, 33), GraphicsUnit.Pixel);
                if (imageDisabled != null)
                {
                    g.DrawImage(imageDisabled, new Rectangle((Width / 2) - (imageDisabled.Width / 2), (Height / 2) - (imageDisabled.Height / 2), imageDisabled.Width, imageDisabled.Height), new Rectangle(0, 0, imageDisabled.Width, imageDisabled.Height), GraphicsUnit.Pixel);
                }
                else if (imageEnabled != null)
                {
                    g.DrawImage(imageEnabled, new Rectangle((Width / 2)-(imageEnabled.Width/2), (Height/2)-(imageEnabled.Height/2), imageEnabled.Width,imageEnabled.Height), new Rectangle(0,0,imageEnabled.Width,ImageEnabled.Height), GraphicsUnit.Pixel);
                }
                g.DrawString(Text, UiTools.StandardRegular, UiTools.DisabledTextBrush, rectf,UiTools.StringFormatCenterCenter);
            }


        }

        private void WwtButton_MouseDown(object sender, MouseEventArgs e)
        {
            pressed = true;
            Refresh();
        }

        private void WwtButton_MouseEnter(object sender, EventArgs e)
        {
            hover = true;
            Refresh();
        }

        private void WwtButton_MouseLeave(object sender, EventArgs e)
        {
            pressed = false;
            hover = false;
            Refresh();
        }

        private void WwtButton_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void WwtButton_MouseUp(object sender, MouseEventArgs e)
        {
            pressed = false;
            Refresh();
        }

        #region IButtonControl Members


        public void NotifyDefault(bool value)
        {
            
        }

        public void PerformClick()
        {
            OnClick(new EventArgs());
        }

        #endregion
    }
}
