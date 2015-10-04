using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ProgressPopup : Form
    {
        static ProgressPopup master;
        public static void Show(Form owner, string titleText, string ProgressText)
        {
            canceled = false;
            if (master == null)
            {
                master = new ProgressPopup();
            }
            master.Owner = owner;
            master.Title = titleText;
            SetProgress(0, ProgressText);
            if (!master.Visible)
            {
                master.Show(owner);
            }
            Application.DoEvents();
        }
        public static void Done()
        {
            if (master != null)
            {
                if (master.Visible)
                {
                    master.Close();
                }
                master = null;
            }

        }
        
        public ProgressPopup()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }

        static public bool SetProgress(int percentComplete, string progressText)
        {
            try
            {
                if (master != null)
                {
                    master.ProgressText.Text = progressText;
                    master.progressBar.Value = percentComplete;
                }
            }
            catch
            {
            }

            Application.DoEvents();
            return !canceled;
        }

        static bool canceled;

        public bool Canceled
        {
            get { return canceled; }
            set { canceled = value; }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            canceled = true;
        }

        private void ProgressPopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            master = null;
        }
    }
}
