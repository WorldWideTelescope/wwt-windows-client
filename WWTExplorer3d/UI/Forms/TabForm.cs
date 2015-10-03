using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TabForm : Form
    {
        public TabForm()
        {
            InitializeComponent();
        }
        static public TabForm CurrentForm = null;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            var g = e.Graphics;
            Brush b = new LinearGradientBrush(new Point(0, 0), new Point(0, Height), Color.FromArgb(20, 30, 39), Color.FromArgb(41, 49, 73));
            var p = new Pen(Color.FromArgb(71,84,108));           
            g.FillRectangle(b, this.ClientRectangle);
            g.DrawRectangle(p, new Rectangle(0,ClientSize.Height-1,ClientSize.Width-1,ClientSize.Height-1));
            p.Dispose();
            GC.SuppressFinalize(p);
            b.Dispose();
            GC.SuppressFinalize(b);
        }

        private void TabForm_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void TabForm_ResizeBegin(object sender, EventArgs e)
        {

        }
        int normalHeight = 110;

        public int NormalHeight
        {
            get { return normalHeight; }
            set { normalHeight = value; }
        }
        int minimizedHeight = 33;

        public int MinimizedHeight
        {
            get { return minimizedHeight; }
            set { minimizedHeight = value; }
        }
        int maximizedHeight = 485;

        public int MaximizedHeight
        {
            get { return maximizedHeight; }
            set { maximizedHeight = value; }
        }

        private void pinUp_Clicked(object sender, EventArgs e)
        {
            FlipPinupState(false);
        }

        public void FlipPinupState(bool forceClosed)
        {
            if (pinUp.Direction == Direction.Expanding && !forceClosed)
            {
                pinUp.Direction = Direction.Collapsing;
                var diff = maximizedHeight - Height;
                Height += diff;
            }
            else if (pinUp.Direction == Direction.Collapsing) 
            {
                pinUp.Direction = Direction.Expanding;
                var diff = maximizedHeight - normalHeight;
                Height -= diff;
            }
            pinUp.Left = (Width / 2) - pinUp.Width / 2;
            pinUp.Top = Height - pinUp.Height;
        }

        private void pinUp_Resize(object sender, EventArgs e)
        {
            pinUp.Left = ( Width / 2) - pinUp.Width / 2;
            pinUp.Top = Height - pinUp.Height;
        }

        protected virtual void SetFocusedChild()
        {
        }

        private void TabForm_MouseHover(object sender, EventArgs e)
        {
            if (!this.ContainsFocus)
            {
                if (UiTools.IsAppFocused())
                {
                    Focus();
                }
            }
        }

        public virtual bool AdvanceSlide(bool fromStart)
        {

            return false;
        }
        public static bool InsideTabRect = false;
        private void fadeTimer_Tick(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            var rect = this.RectangleToScreen(this.ClientRectangle);
            rect = new Rectangle(rect.X, rect.Y , rect.Width, rect.Height);

            InsideTabRect = rect.Contains(Cursor.Position);

            var inside = MenuTabs.MouseInTabs || LayerManager.InsideLayerManagerRect || Earth3d.TouchKiosk || rect.Contains(Cursor.Position) || !((TourPlayer.Playing && !Settings.DomeView) || Earth3d.FullScreen || Properties.Settings.Default.AutoHideTabs);

            if (inside != fader.TargetState)
            {
                fader.TargetState = inside;
                fadeTimer.Enabled = true;
                fadeTimer.Interval = 10;
            }

            SetOpacity();

            if ((!fader.TargetState && fader.Opacity == 0.0) || (fader.TargetState && fader.Opacity == 1.0))
            {
                if (CurrentForm == this)
                {
                    if (Properties.Settings.Default.TranparentWindows)
                    {

                        this.Visible = true;
                    }
                    else
                    {
                        this.Visible = fader.TargetState;
                    }
                    if (Earth3d.FullScreen)
                    {
                        Earth3d.MainWindow.menuTabs.IsVisible = fader.TargetState && !Earth3d.TouchKiosk;
                    }
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
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.F4 | Keys.Alt))
            {
                dontClose = true;
            }
            if (keyData == (Keys.F1))
            {
                if (this is TourEditTab)
                {
                    WebWindow.OpenUrl("http://www.worldwidetelescope.org/Learn/?Authoring#slidebasedtours", true);
                }
                else
                {
                    Earth3d.LaunchHelp();
                }
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}