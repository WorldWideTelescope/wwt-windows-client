using System;
using System.Drawing;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public partial class ImageStack : Form
    {
        public ImageStack()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(622, "Image Stack");
        }

        internal void UpdateList()
        {
            stackList.Clear();
            if (Earth3d.MainWindow.CurrentImageSet != null)
            {
                stackList.Add(ImageSet.FromIImage(Earth3d.MainWindow.CurrentImageSet));
            }
            foreach (ImageSet set in Earth3d.MainWindow.ImageStackList)
            {
                stackList.Add(ImageSet.FromIImage(set));
            }
            if (Earth3d.MainWindow.StudyImageset != null)
            {
                stackList.Add(ImageSet.FromIImage(Earth3d.MainWindow.StudyImageset));
            }

        }

        private void closeBox_MouseEnter(object sender, EventArgs e)
        {
            closeBox.Image = Resources.CloseHover;
        }

        private void closeBox_MouseLeave(object sender, EventArgs e)
        {
            closeBox.Image = Resources.CloseRest;

        }

        private void closeBox_MouseDown(object sender, MouseEventArgs e)
        {
            closeBox.Image = Resources.ClosePush;

        }

        private void closeBox_MouseUp(object sender, MouseEventArgs e)
        {
            closeBox.Image = Resources.CloseHover;
            Close();
            Earth3d.MainWindow.ImageStackVisible = false;
            Earth3d.MainWindow.ImageStackList.Clear();
            Earth3d.MainWindow.Stack = null;
        }
        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            var rect = RectangleToScreen(ClientRectangle);
            rect = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            var inside = MenuTabs.MouseInTabs || rect.Contains(Cursor.Position) || !(TourPlayer.Playing || Earth3d.FullScreen || Properties.Settings.Default.AutoHideTabs);

            if (inside != fader.TargetState)
            {
                fader.TargetState = inside;
                fadeTimer.Enabled = true;
                fadeTimer.Interval = 10;
            }

            SetOpacity();

            if ((!fader.TargetState && fader.Opacity == 0.0) || (fader.TargetState && fader.Opacity == 1.0))
            {
                
                    if (Properties.Settings.Default.TranparentWindows)
                    {

                        Visible = true;
                    }
                    else
                    {
                        Visible = fader.TargetState;
                    }

                
                fadeTimer.Enabled = true;
                fadeTimer.Interval = 250;
            }
        }

        public void SetOpacity()
        {
            if (Properties.Settings.Default.TranparentWindows)
            {
                try
                {
                    Opacity = .0 + .8 * fader.Opacity;
                }
                catch
                {
                    Opacity = 1.0;
                }
            }
            else
            {
                Opacity = 1.0;
            }
        }

        readonly BlendState fader = new BlendState(false, 1000.0);

        private void TabForm_MouseEnter(object sender, EventArgs e)
        {
            fader.TargetState = true;
            fadeTimer.Enabled = true;
            fadeTimer.Interval = 10;
        }

        private void TabForm_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TabForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dontClose)
            {
                dontClose = false;
                e.Cancel = true;
            }
        }
        bool dontClose;
    }
}
