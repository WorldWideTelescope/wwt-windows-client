using System;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DurrationEditor : Form
    {
        public DurrationEditor()
        {
            InitializeComponent();
        }
        public TourStop target = null;

        private void DurrationEditor_Load(object sender, EventArgs e)
        {
            if (target != null)
            {
                TimeText = target.Duration;
            }
            timeEdit.Font = UiTools.StandardRegular;
            Earth3d.NoStealFocus = true;
        }

        TimeSpan TimeText
        {
            get
            {
                var miniutes = Convert.ToDouble(timeEdit.Text.Substring(0, 2));
                var seconds = Convert.ToDouble(timeEdit.Text.Substring(3));
                var milliseconds = (seconds - (int)seconds) * 1000;

                return new TimeSpan(0, 0, (int)miniutes, (int)seconds, (int)milliseconds);
            }
            set
            {
                if (target != null)
                {
                    timeEdit.Text = String.Format("{0:00}:{1:00.0}", (int)value.TotalMinutes, value.Seconds + value.Milliseconds / 1000.0);
                }
            }
        }

        private void timeEdit_Validated(object sender, EventArgs e)
        {
        }

        private void timeEdit_Leave(object sender, EventArgs e)
        {
        }

        private void DurrationEditor_Leave(object sender, EventArgs e)
        {
        }

        private void timeEdit_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void timeEdit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                Close();
            }
        }

        private void DurrationEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (target.Duration != TimeText)
                {

                    if (target.KeyFramed)
                    {
                        if (TimeText.TotalSeconds < target.Duration.TotalSeconds)
                        {
                            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1361, "Do you want to trim the timeline and delete keys past the new duration (Yes) or scale keys to the new duration (No)?"), Language.GetLocalizedText(1362, "Trim or scale timeline"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1365, "Trim Timeline"), target.Owner));
                                target.ExtendTimeline(target.Duration, TimeText);
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1366, "Scale Timeline"), target.Owner));
                                target.Duration = TimeText;
                            }
                        }
                        else
                        {
                            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1364, "Do you want to extend the timeline (Yes) or scale it (No)?"), Language.GetLocalizedText(1363, "Extend or scale timeline"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1367, "Extend Timeline"), target.Owner));
                                target.ExtendTimeline(target.Duration, TimeText);
                            }
                            else
                            {
                                Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1366, "Scale Timeline"), target.Owner));
                                target.Duration = TimeText;
                            }
                        }
                    }
                    else
                    {
                        Undo.Push(new UndoTourStopChange(Language.GetLocalizedText(1368, "Edit Duration"), target.Owner));
                        target.Duration = TimeText;
                    }
                }
                Earth3d.NoStealFocus = false;
            }
            catch
            {
            }
        }

        private void DurrationEditor_Deactivate(object sender, EventArgs e)
        {
            Close();
        }

        private void arrowDown_Pushed(object sender, EventArgs e)
        {
            var ts = TimeText;
            if (ts.TotalSeconds > 1)
            {
                ts = ts - new TimeSpan(0, 0, 1);
                TimeText = ts;
            }
        }

        private void arrowUp_Pushed(object sender, EventArgs e)
        {
            var ts = TimeText;
            if (ts.TotalSeconds < 3599)
            {
                ts = ts + new TimeSpan(0, 0, 1);
                TimeText = ts;
            }

        }
    }
}