using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TerraViewer;

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
            this.label1.Text = Language.GetLocalizedText(768, "Off-Axis X");
            this.label2.Text = Language.GetLocalizedText(769, "Off-Axis Y");
            this.label3.Text = Language.GetLocalizedText(770, "FOV");
            this.label4.Text = Language.GetLocalizedText(771, "Aspect");
            this.label5.Text = Language.GetLocalizedText(772, "Projection");
            this.label6.Text = Language.GetLocalizedText(773, "X");
            this.label7.Text = Language.GetLocalizedText(774, "Y");
            this.label8.Text = Language.GetLocalizedText(775, "Z");
            this.label9.Text = Language.GetLocalizedText(776, "Pos");
            this.label10.Text = Language.GetLocalizedText(777, "Rot");
            this.label11.Text = Language.GetLocalizedText(778, "Transform");
            this.label12.Text = Language.GetLocalizedText(779, "Pitch");
            this.label13.Text = Language.GetLocalizedText(780, "Heading");
            this.label14.Text = Language.GetLocalizedText(781, "Roll");
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
                FovText.Text = ProjTarget.FOV.ToString();
                AspectText.Text = ProjTarget.Aspect.ToString();
                OffAxisXText.Text = ProjTarget.XOffset.ToString();
                OffAxisYText.Text = ProjTarget.YOffset.ToString();
            }

            if (transTarget != null)
            {
                PosXText.Text = TransTarget.X.ToString();
                PosYText.Text = TransTarget.Y.ToString();
                PosZText.Text = TransTarget.Z.ToString();

                PitchText.Text = TransTarget.Pitch.ToString();
                HeadingText.Text = TransTarget.Heading.ToString();
                RollText.Text = TransTarget.Roll.ToString();
            }
        }

        private void FovText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            ProjTarget.FOV = UiTools.ParseAndValidateDouble(FovText, ProjTarget.FOV, ref failed);
        }

        private void AspectText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            ProjTarget.Aspect = UiTools.ParseAndValidateDouble(AspectText, ProjTarget.Aspect, ref failed);

        }

        private void OffAxisXText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            ProjTarget.XOffset = UiTools.ParseAndValidateDouble(OffAxisXText, ProjTarget.XOffset, ref failed);

        }

        private void OffAxisYText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            ProjTarget.YOffset = UiTools.ParseAndValidateDouble(OffAxisYText, ProjTarget.YOffset, ref failed);

        }

        private void PosXText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.X = UiTools.ParseAndValidateDouble(PosXText, TransTarget.X, ref failed);

        }

        private void PosYText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.Y = UiTools.ParseAndValidateDouble(PosYText, TransTarget.Y, ref failed);

        }

        private void PosZText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.Z = UiTools.ParseAndValidateDouble(PosZText, TransTarget.Z, ref failed);

        }

        private void RotXText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.Pitch = UiTools.ParseAndValidateDouble(PitchText, TransTarget.Pitch, ref failed);

        }

        private void RotYText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.Heading = UiTools.ParseAndValidateDouble(HeadingText, TransTarget.Heading, ref failed);

        }

        private void RotZText_TextChanged(object sender, EventArgs e)
        {
            bool failed = false;
            TransTarget.Roll = UiTools.ParseAndValidateDouble(RollText, TransTarget.Roll, ref failed);
        }
    }
}
