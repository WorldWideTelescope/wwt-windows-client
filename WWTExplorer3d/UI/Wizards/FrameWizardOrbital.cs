using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class FrameWizardOrbital : PropPage
    {
        public FrameWizardOrbital()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            SemiMajorAxisLable.Text = Language.GetLocalizedText(805, "Semimajor Axis (a)");
            EccintricityLabel.Text = Language.GetLocalizedText(806, "Eccentricity (e)");
            InclinationLabel.Text = Language.GetLocalizedText(807, "Inclination");
            ArgumentOfPeriapsisLabel.Text = Language.GetLocalizedText(808, "Argument of periapsis");
            LongitudeOfAscendingNodeLabel.Text = Language.GetLocalizedText(809, "Longitude of the Ascending Node");
            MeanAnomolyAtEpochLabel.Text = Language.GetLocalizedText(810, "Mean anomaly at epoch");
            EpocLabel.Text = Language.GetLocalizedText(811, "Epoch (Julian Date)");
            MeanDailyMotionLabel.Text = Language.GetLocalizedText(812, "Mean Daily Motion");
            label2.Text = Language.GetLocalizedText(813, "A Keplarian orbit is an elliptical orbit defined by at least 6 parameters.  There are some variations in how those parameters are described, but most orbits can be translated into the terms below.");
            SemimajorAxisUnitsLabel.Text = Language.GetLocalizedText(814, "Semimajor Axis Units");
            PasteFromTle.Text = Language.GetLocalizedText(815, "Paste TLE");
          }
        ReferenceFrame frame;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }

        public override bool Save()
        {
            var failed = false;

            frame.SemiMajorAxis = ParseAndValidateDouble(SemimajorAxis, frame.SemiMajorAxis, ref failed);
            frame.Eccentricity = ParseAndValidateDouble(Eccintricity, frame.Eccentricity, ref failed);
            frame.Inclination = ParseAndValidateDouble(Inclination, frame.Inclination, ref failed);
            frame.ArgumentOfPeriapsis = ParseAndValidateDouble(ArgumentOfPeriapsis, frame.ArgumentOfPeriapsis, ref failed);
            frame.LongitudeOfAscendingNode = ParseAndValidateDouble(LongitudeOfAscendingNode, frame.LongitudeOfAscendingNode, ref failed);
            frame.MeanAnomolyAtEpoch = ParseAndValidateDouble(MeanAnomolyAtEpoch, frame.MeanAnomolyAtEpoch, ref failed);
            frame.Epoch = ParseAndValidateDouble(Epoch, frame.Epoch, ref failed);
            frame.MeanDailyMotion = ParseAndValidateDouble(MeanDailyMotion, frame.MeanDailyMotion, ref failed);

            return !failed;
        }

        private void FrameWizardOrbital_Load(object sender, EventArgs e)
        {
            SyncToFrame();


        }

        private void SyncToFrame()
        {
            SemiMajorAxisUnits.Items.AddRange(Enum.GetNames(typeof(AltUnits)));
            SemiMajorAxisUnits.SelectedIndex = (int)frame.SemiMajorAxisUnits;

            SemimajorAxis.Text = frame.SemiMajorAxis.ToString();
            Eccintricity.Text = frame.Eccentricity.ToString();
            Inclination.Text = frame.Inclination.ToString();
            ArgumentOfPeriapsis.Text = frame.ArgumentOfPeriapsis.ToString();
            LongitudeOfAscendingNode.Text = frame.LongitudeOfAscendingNode.ToString();
            MeanAnomolyAtEpoch.Text = frame.MeanAnomolyAtEpoch.ToString();
            Epoch.Text = frame.Epoch.ToString();
            MeanDailyMotion.Text = frame.MeanDailyMotion.ToString();
        }

        private void SemiMajorAxisUnits_SelectionChanged(object sender, EventArgs e)
        {
            frame.SemiMajorAxisUnits = (AltUnits)SemiMajorAxisUnits.SelectedIndex;
        }

        private void PasteFromTle_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText() && Clipboard.GetText().Length > 0)
            {
                var data = Clipboard.GetText(TextDataFormat.UnicodeText);
                var lines = data.Split(new[] {'\n','\r'});
                var line1 = "";
                var line2 = "";
                for (var i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim();
                    if (lines[i].Length == 69 && ReferenceFrame.IsTLECheckSumGood(lines[i]))
                    {
                        if (line1.Length == 0 && lines[i].Substring(0, 1) == "1" )
                        {
                            line1 = lines[i];
                        }
                        if (line2.Length == 0 && lines[i].Substring(0, 1) == "2")
                        {
                            line2 = lines[i];
                        }                 
                    }
                }

                if (line1.Length == 69 && line2.Length == 69)
                {
                        frame.FromTLE(line1, line2, 398600441800000);
                        SyncToFrame();
                        return;        
                }

            }
            
            UiTools.ShowMessageBox(Language.GetLocalizedText(756, "The clipbboard does not appear to contain a valid TLE set. Copy the contents of a Two Line Elements (TLE) set from your source to the clipboard in plain text and click paste again"), Language.GetLocalizedText(755, "Paste Orbital Elements from TLE"));
            
        }
    }

}
