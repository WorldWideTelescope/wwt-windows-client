using System;
using System.Globalization;
using System.Windows.Forms;

namespace TerraViewer.Callibration
{
    public partial class ProjectionTransformEditor : UserControl
    {
        public ProjectionTransformEditor()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(768, "Off-Axis X");
            label2.Text = Language.GetLocalizedText(769, "Off-Axis Y");
            label3.Text = Language.GetLocalizedText(770, "FOV");
            label4.Text = Language.GetLocalizedText(771, "Aspect");
            label5.Text = Language.GetLocalizedText(772, "Projection");
            label6.Text = Language.GetLocalizedText(773, "X");
            label7.Text = Language.GetLocalizedText(774, "Y");
            label8.Text = Language.GetLocalizedText(775, "Z");
            label9.Text = Language.GetLocalizedText(776, "Pos");
            label10.Text = Language.GetLocalizedText(777, "Rot");
            label11.Text = Language.GetLocalizedText(778, "Transform");
            label12.Text = Language.GetLocalizedText(779, "Pitch");
            label13.Text = Language.GetLocalizedText(780, "Heading");
            label14.Text = Language.GetLocalizedText(781, "Roll");
        }
        private Projection projTarget = new Projection();

        public Projection ProjTarget
        {
            get { return projTarget; }
            set
            {
                projTarget = value;
                SyncTargets();
            }
        }
        private Transform transTarget = new Transform();

        public Transform TransTarget
        {
            get { return transTarget; }
            set
            {
                transTarget = value;
                SyncTargets();
            }
        }

        private void ProjectionTransformEditor_Load(object sender, EventArgs e)
        {
            SyncTargets();
        }

        private void SyncTargets()
        {
            if (projTarget != null)
            {
                FovText.Text = ProjTarget.FOV.ToString(CultureInfo.InvariantCulture);
                AspectText.Text = ProjTarget.Aspect.ToString(CultureInfo.InvariantCulture);
                OffAxisXText.Text = ProjTarget.XOffset.ToString(CultureInfo.InvariantCulture);
                OffAxisYText.Text = ProjTarget.YOffset.ToString(CultureInfo.InvariantCulture);
            }

            if (transTarget != null)
            {
                PosXText.Text = TransTarget.X.ToString(CultureInfo.InvariantCulture);
                PosYText.Text = TransTarget.Y.ToString(CultureInfo.InvariantCulture);
                PosZText.Text = TransTarget.Z.ToString(CultureInfo.InvariantCulture);

                PitchText.Text = TransTarget.Pitch.ToString(CultureInfo.InvariantCulture);
                HeadingText.Text = TransTarget.Heading.ToString(CultureInfo.InvariantCulture);
                RollText.Text = TransTarget.Roll.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void FovText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            ProjTarget.FOV = UiTools.ParseAndValidateDouble(FovText, ProjTarget.FOV, ref failed);
        }

        private void AspectText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            ProjTarget.Aspect = UiTools.ParseAndValidateDouble(AspectText, ProjTarget.Aspect, ref failed);

        }

        private void OffAxisXText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            ProjTarget.XOffset = UiTools.ParseAndValidateDouble(OffAxisXText, ProjTarget.XOffset, ref failed);

        }

        private void OffAxisYText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            ProjTarget.YOffset = UiTools.ParseAndValidateDouble(OffAxisYText, ProjTarget.YOffset, ref failed);

        }

        private void PosXText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.X = UiTools.ParseAndValidateDouble(PosXText, TransTarget.X, ref failed);

        }

        private void PosYText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.Y = UiTools.ParseAndValidateDouble(PosYText, TransTarget.Y, ref failed);

        }

        private void PosZText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.Z = UiTools.ParseAndValidateDouble(PosZText, TransTarget.Z, ref failed);

        }

        private void RotXText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.Pitch = UiTools.ParseAndValidateDouble(PitchText, TransTarget.Pitch, ref failed);

        }

        private void RotYText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.Heading = UiTools.ParseAndValidateDouble(HeadingText, TransTarget.Heading, ref failed);

        }

        private void RotZText_TextChanged(object sender, EventArgs e)
        {
            var failed = false;
            TransTarget.Roll = UiTools.ParseAndValidateDouble(RollText, TransTarget.Roll, ref failed);
        }
    }
}
