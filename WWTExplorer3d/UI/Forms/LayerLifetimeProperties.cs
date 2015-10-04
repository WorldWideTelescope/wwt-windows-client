using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class LayerLifetimeProperties : Form
    {
        public LayerLifetimeProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(867, "Start DateTime");
            label2.Text = Language.GetLocalizedText(868, "End DateTime");
            label3.Text = Language.GetLocalizedText(869, "Fade In/Out Time");
            label4.Text = Language.GetLocalizedText(870, "Fade Type");
            OK.Text = Language.GetLocalizedText(212, "Close");
            Text = Language.GetLocalizedText(871, "Layer Lifetime");
        }

        private void LayerLifetimeProperties_Load(object sender, EventArgs e)
        {
            startDate.DateTimeValue = target.StartTime;
            endDate.DateTimeValue = target.EndTime;
            fadeTime.Text = target.FadeSpan.TotalDays.ToString();
            fadeType.Items.AddRange(Enum.GetNames(typeof(FadeType)));
            fadeType.SelectedIndex = (int)target.FadeType;

        }

        private Layer target;

        public Layer Target
        {
            get { return target; }
            set { target = value; }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            var failed = false;
            target.StartTime = startDate.DateTimeValue;
            target.EndTime = endDate.DateTimeValue;
            var days = UiTools.ParseAndValidateDouble(fadeTime, target.FadeSpan.Days, ref failed);
            try
            {
                target.FadeSpan = TimeSpan.FromDays(Convert.ToDouble(fadeTime.Text));
            }
            catch
            {
                fadeTime.BackColor = Color.Red;
                failed = true;
            }

            target.FadeType = (FadeType)fadeType.SelectedIndex;


            if (!failed)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void startDate_SelectionChanged(object sender, EventArgs e)
        {

        }

    }
}
