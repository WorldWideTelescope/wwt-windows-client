using System;
using System.Globalization;
using System.Windows.Forms;

namespace TerraViewer.Callibration
{
    public partial class GroundTruthPointProperties : Form
    {
        public GroundTruthPointProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(763, "Altitude");
            this.label2.Text = Language.GetLocalizedText(764, "Constraint Type");
            this.label3.Text = Language.GetLocalizedText(765, "Azimuth");
            this.label4.Text = Language.GetLocalizedText(766, "Weight");
            this.OK.Text = Language.GetLocalizedText(759, "Ok");
            this.Text = Language.GetLocalizedText(767, "GroundTruthPointProperties");
        }

        public GroundTruthPoint Target = null;
        private void GroundTruthPointProperties_Load(object sender, EventArgs e)
        {
            this.ConstraintTypeCombo.Items.AddRange(Enum.GetNames(typeof(AxisTypes)));
            this.ConstraintTypeCombo.Items.RemoveAt(ConstraintTypeCombo.Items.Count - 1);
            this.ConstraintTypeCombo.SelectedIndex = (int)Target.AxisType;

            AzText.Text = Target.Az.ToString(CultureInfo.InvariantCulture);
            AltText.Text = Target.Alt.ToString(CultureInfo.InvariantCulture);
            WeightText.Text = Target.Weight.ToString(CultureInfo.InvariantCulture);
        }

        private void OK_Click(object sender, EventArgs e)
        {
            bool failed = false;

            Target.Az = UiTools.ParseAndValidateDouble(AzText, Target.Az, ref failed);
            Target.Alt = UiTools.ParseAndValidateDouble(AltText, Target.Alt, ref failed);
            Target.Weight = UiTools.ParseAndValidateDouble(WeightText, Target.Az, ref failed);

            if (!failed)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ConstraintTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            Target.AxisType = (AxisTypes)ConstraintTypeCombo.SelectedIndex;
        }
    }
}
