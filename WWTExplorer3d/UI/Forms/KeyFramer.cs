using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class KeyFramer : Form
    {
        public static void ShowTimeline()
        {
            if (Properties.Settings.Default.FloatTimeline)
            {
                if (singleton == null)
                {
                    var kf = new KeyFramer();

                    kf.Owner = Earth3d.MainWindow;
                    Earth3d.MainWindow.AddOwnedForm(kf);
                    kf.Location = new Point(Earth3d.MainWindow.Location.X, Earth3d.MainWindow.Location.Y + Earth3d.MainWindow.Height - kf.Height);
                    kf.Width = Earth3d.MainWindow.Width;
                    kf.Show();
                }
            }
            else
            {
                ContextPanel.ShowTimeline = true;
            }
        }

        public static void ShowZOrder()
        {
            if (Properties.Settings.Default.FloatTimeline)
            {
                if (singleton != null)
                {
                    singleton.Show();
                }
            }
        }


        public static void HideTimeline()
        {
            if (singleton != null)
            {
                singleton.Close();
            }

            ContextPanel.ShowTimeline = false;

            //close the key properties as well.
            KeyProperties.HideProperties();
        }

   
        static KeyFramer singleton;
        public KeyFramer()
        {
            InitializeComponent();
            singleton = this;
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            Text = Language.GetLocalizedText(1313, "Timeline Editor");
        }

        private void KeyFramer_FormClosing(object sender, FormClosingEventArgs e)
        {
            TimeLine.RemoveInstance(TimeLine);
            singleton = null;
        }

        private void KeyFramer_Load(object sender, EventArgs e)
        {

        }

        private void TimeLine_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void PushPin_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FloatTimeline = false;
            ShowTimeline();
            Close();
        }

    }

   
}
