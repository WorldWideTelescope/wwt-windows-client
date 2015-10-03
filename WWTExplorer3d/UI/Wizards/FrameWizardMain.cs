using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class FrameWizardMain : PropPage
    {
        public FrameWizardMain()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.HeadingLabel.Text = Language.GetLocalizedText(780, "Heading");
            this.PitchLabel.Text = Language.GetLocalizedText(779, "Pitch");
            this.RollLabel.Text = Language.GetLocalizedText(781, "Roll");
            this.RotationPeriodLabel.Text = Language.GetLocalizedText(792, "Rotation Period (days)");
            this.ZeroRotationLabel.Text = Language.GetLocalizedText(793, "Zero Rotation (Julian Day)");
            this.meanRadiusLabel.Text = Language.GetLocalizedText(794, "Mean Radius (meters)");
            this.OblatenessLabel.Text = Language.GetLocalizedText(795, "Oblateness (Percent)");
            this.OrbitColorLabel.Text = Language.GetLocalizedText(796, "Orbit/Point Color");
            this.ShowAsPoint.Text = Language.GetLocalizedText(797, "Show as Point at Distance");
            this.ShowOrbitPath.Text = Language.GetLocalizedText(798, "Show Orbit Path");
            this.ScaleLabel.Text = Language.GetLocalizedText(799, "Scale (Meters to Units)");
            this.StationKeeping.Text = Language.GetLocalizedText(800, "Station Keeping");
            this.label2.Text = Language.GetLocalizedText(933, "The Mean allows you to specify the primary bounds of the reference frame such as the surface height of a planet, or the bounding sphere of a 3d model of a spacecraft. Rotation period and zero rotation day allow the specification of a rotating frame of reference. Heading Pitch and roll allow for orientation relative to the parent reference frame. Station Keeping automatically orients the reference frame to track the Earth rather than tumble in orbit.");
        }
        ReferenceFrame frame;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }

        public override bool Save()
        {
            var failed = false;

            frame.MeanRadius = ParseAndValidateDouble(MeanRadius, frame.MeanRadius, ref failed);
            frame.Oblateness = ParseAndValidateDouble(Oblateness, frame.Oblateness, ref failed);
            frame.Scale = ParseAndValidateDouble(Scale, frame.Scale, ref failed);
            frame.RotationalPeriod = ParseAndValidateDouble(RotaionPeriod,frame.RotationalPeriod , ref failed);
            frame.ZeroRotationDate = ParseAndValidateDouble(ZeroRotation, frame.ZeroRotationDate, ref failed);
            frame.Heading = ParseAndValidateDouble(Heading, frame.Heading, ref failed);
            frame.Pitch = ParseAndValidateDouble(Pitch, frame.Pitch, ref failed);
            frame.Roll = ParseAndValidateDouble(Roll, frame.Roll, ref failed);
         
            frame.ShowAsPoint = ShowAsPoint.Checked;
            frame.ShowOrbitPath = ShowOrbitPath.Checked;
            return !failed;
        }

        

        private void RepresentativeColor_Click(object sender, EventArgs e)
        {
            var picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = frame.RepresentativeColor;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                frame.RepresentativeColor = picker.Color;
            }
            this.Refresh();
        }

        private void RepresentativeColor_Paint(object sender, PaintEventArgs e)
        {
            var control = (PictureBox)sender;
            e.Graphics.Clear(Color.Black);
            var pen = new Pen(frame.RepresentativeColor);
            e.Graphics.DrawLine(pen, new Point(0, control.Bounds.Height / 2),
                new Point(control.Bounds.Width, control.Bounds.Height / 2));
            pen.Dispose();
        }

        private void showAsPoint_CheckedChanged(object sender, EventArgs e)
        {
            frame.ShowAsPoint = ShowAsPoint.Checked;
        }

        private void ShowOrbitPath_CheckedChanged(object sender, EventArgs e)
        {

            frame.ShowOrbitPath = ShowOrbitPath.Checked;
        }

        private void FrameWizardMain_Load(object sender, EventArgs e)
        {
            MeanRadius.Text = frame.MeanRadius.ToString();
            Oblateness.Text = frame.Oblateness.ToString();
            Scale.Text = frame.Scale.ToString();
            RotaionPeriod.Text = frame.RotationalPeriod.ToString();
            ZeroRotation.Text = frame.ZeroRotationDate.ToString();
            Heading.Text = frame.Heading.ToString();
            Pitch.Text = frame.Pitch.ToString();
            Roll.Text = frame.Roll.ToString();

            ShowAsPoint.Checked = frame.ShowAsPoint;
            ShowOrbitPath.Checked = frame.ShowOrbitPath;
            StationKeeping.Checked = frame.StationKeeping;

        }

        private void Roll_TextChanged(object sender, EventArgs e)
        {

        }

        private void StationKeeping_CheckedChanged(object sender, EventArgs e)
        {
            frame.StationKeeping = StationKeeping.Checked;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

  


    }
}
