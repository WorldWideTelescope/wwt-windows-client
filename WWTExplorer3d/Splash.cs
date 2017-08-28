using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace TerraViewer
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            SetUiStrings();
     
        }

        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(372, "Initializing Data...");
        }
        enum SplashState { FadingIn, Showing, FadingOut };

        SplashState splashState = SplashState.FadingIn;
        int progressCount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(splashState)
            {
                case SplashState.FadingIn:
                    this.Opacity += .05;
                    if (this.Opacity == 1.0)
                    {
                        splashState = SplashState.Showing;
                        timer1.Interval = 1000;
                    }
                    break;
                case SplashState.Showing:
                    timer1.Interval = 200;
                    progressCount++;

                    if (progressCount > 950)
                    {
                        progressCount = 100;
                    }
                    progressBar1.Value = progressCount / 10;
                    if (RenderEngine.Initialized || Earth3d.HideSplash)
                    {
                        splashState = SplashState.FadingOut;
                        timer1.Interval = 50;
                    }
                    break;
                case SplashState.FadingOut:
                    this.Opacity -= .05;
                    if (this.Opacity == 0.0)
                    {
                        this.Close();
                    }                    
                    break;
            }
        }
        static Thread splashTread;
        public static void ShowSplashScreen()
        {
            splashTread = new Thread( new ThreadStart(SplashTreadFunction));
            splashTread.Start();
        }

        private static void SplashTreadFunction()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);
            Splash splash = new Splash();
            splash.ShowDialog();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}