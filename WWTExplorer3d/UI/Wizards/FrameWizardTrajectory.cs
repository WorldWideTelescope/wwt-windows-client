using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class FrameWizardTrajectory : PropPage
    {
        public FrameWizardTrajectory()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(816, "Trajectory Reference Frames are based on a time series table of Julian Date/Times and heliocentric X,Y,Z coordinates in the referenced units. The reference frame will orient itself on the path described based on interpolating positions for the current time.");
            this.UnitsLabel.Text = Language.GetLocalizedText(817, "Units");
            this.Import.Text = Language.GetLocalizedText(818, "Import Path");
            this.EndDateRangeLabel.Text = Language.GetLocalizedText(819, "End Date Range");
            this.beginDateRangeLabel.Text = Language.GetLocalizedText(820, "Begin Date Range");
        }
        ReferenceFrame frame;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }
        public override bool Save()
        {
            var failed = false;

           return !failed;

        }

        private void FrameWizardTrajectory_Load(object sender, EventArgs e)
        {
            SemiMajorAxisUnits.Items.AddRange(Enum.GetNames(typeof(AltUnits)));
            SemiMajorAxisUnits.SelectedIndex = (int)frame.SemiMajorAxisUnits;


        }

        private void Import_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();

            ofd.Filter = Language.GetLocalizedText(821, "Trajectory files") +"  (*.xyz)|*.xyz";
            var tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        var fileName = ofd.FileName;

                        frame.ImportTrajectory(fileName);
                        tryAgain = false;
                    }
                    else
                    {
                        tryAgain = false;
                    }
                }
                catch
                {
                    UiTools.ShowMessageBox(Language.GetLocalizedText(947, "The File does not appear to be a Vaild Trajectory File."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        } 
    }
}
