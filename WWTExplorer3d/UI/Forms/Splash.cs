using System;
using System.Globalization;
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
            label1.Text = Language.GetLocalizedText(372, "Initializing Data...");
        }
        enum SplashState { FadingIn, Showing, FadingOut };

        SplashState splashState = SplashState.FadingIn;
        int progressCount;
        private void timer1_Tick(object sender, EventArgs e)
        {
            switch(splashState)
            {
                case SplashState.FadingIn:
                    Opacity += .05;
                    if (Opacity == 1.0)
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
                    if (Earth3d.Initialized || Earth3d.HideSplash)
                    {
                        splashState = SplashState.FadingOut;
                        timer1.Interval = 50;
                    }
                    break;
                case SplashState.FadingOut:
                    Opacity -= .05;
                    if (Opacity == 0.0)
                    {
                        Close();
                    }                    
                    break;
            }
        }
        static Thread splashTread;
        public static void ShowSplashScreen()
        {
            splashTread = new Thread( SplashTreadFunction);
            splashTread.Start();
        }

        private static void SplashTreadFunction()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            var splash = new Splash();
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