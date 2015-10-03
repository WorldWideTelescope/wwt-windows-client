using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DatePopup : Form
    {
        
        public DatePopup()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            this.Text = Language.GetLocalizedText(187, "Date Time Selection");
            this.label8.Text = Language.GetLocalizedText(188, "Year");
            this.label9.Text = Language.GetLocalizedText(189, "Month");
            this.label10.Text = Language.GetLocalizedText(190, "Day");
            this.label1.Text = Language.GetLocalizedText(191, "Hrs");
            this.label2.Text = Language.GetLocalizedText(192, "Min");
            this.label4.Text = Language.GetLocalizedText(193, "Sec");
            this.showUtcTime.Text = Language.GetLocalizedText(194, "UTC");
            this.apply.Text = Language.GetLocalizedText(195, "Apply");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            
        }

        public event EventHandler DateChanged;

        private bool masterClock = true;

        public bool MasterClock
        {
            get { return masterClock; }
            set { masterClock = value; }
        }

        public static DatePopup Current = null;

        private void OK_Click(object sender, EventArgs e)
        {
            ApplyNewTime();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ApplyNewTime()
        {
            try
            {
                ScratchTime = new DateTime(Convert.ToInt32(year.Text), Convert.ToInt32(month.Text), Convert.ToInt32(day.Text), Convert.ToInt32(hours.Text), Convert.ToInt32(min.Text), Convert.ToInt32(sec.Text));
                if (masterClock)
                {
                    if (Properties.Settings.Default.ShowUTCTime)
                    {
                        SpaceTimeController.Now = ScratchTime;
                    }
                    else
                    {
                        SpaceTimeController.Now = ScratchTime.ToUniversalTime();
                    }
                }

            }
            catch
            {
            }

            UpdateTimeFields();

            if (DateChanged != null)
            {
                DateChanged.Invoke(this, new EventArgs());
            }
        }

        DateTime scratchTime;

        public DateTime ScratchTime
        {
            get { return scratchTime; }
            set
            {
                if (scratchTime != value)
                {
                    scratchTime = value;
                    UpdateTimeFields();
                }

            }
        }

        private void DatePopup_Load(object sender, EventArgs e)
        {
            Current = this;
            if (masterClock)
            {
                if (Properties.Settings.Default.ShowUTCTime)
                {
                    ScratchTime = SpaceTimeController.Now;
                }
                else
                {
                    ScratchTime = SpaceTimeController.Now.ToLocalTime();
                }
            }
            

            UpdateTimeFields();
            if (masterClock)
            {
                showUtcTime.Checked = Properties.Settings.Default.ShowUTCTime;
            }
            else
            {
                showUtcTime.Visible = false;
                this.PushPin.Visible = false;

            }
        }
        bool inUpdateTime = false;
        private void UpdateTimeFields()
        {
            if (ScratchTime.Year > 4000)
            {
                ScratchTime = new DateTime(4000, 12, 31, 23, 59, 59);
            }

            if (ScratchTime.Year < 1)
            {
                ScratchTime = new DateTime(0, 12, 25, 23, 59, 59);
            }

            year.Text = ScratchTime.Year.ToString();
            month.Text = ScratchTime.Month.ToString();
            day.Text = ScratchTime.Day.ToString();
            hours.Text = ScratchTime.Hour.ToString();
            min.Text = ScratchTime.Minute.ToString();
            sec.Text = ScratchTime.Second.ToString();
            if (!inUpdateTime)
            {
                inUpdateTime = true;
                ApplyNewTime();
                inUpdateTime = false;
            }
        }


        private void showUtcTime_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowUTCTime = showUtcTime.Checked;
            if (showUtcTime.Checked)
            {
                ScratchTime = ScratchTime.ToUniversalTime();
            }
            else
            {
                ScratchTime = ScratchTime.ToLocalTime();
            }

            UpdateTimeFields();
        }

        private void yearUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddYears(1);
            UpdateTimeFields();
        }

        private void yearDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddYears(-1);
            UpdateTimeFields();
        }

        private void monthUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddMonths(1);
            UpdateTimeFields();
        }

        private void monthDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddMonths(-1);
            UpdateTimeFields();
        }

        private void dayUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddDays(1);
            UpdateTimeFields();
        }

        private void dayDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddDays(-1);
            UpdateTimeFields();

        }

        private void hourUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddHours(1);
            UpdateTimeFields();

        }

        private void hourDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddHours(-1);
            UpdateTimeFields();

        }

        private void minUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddMinutes(1);
            UpdateTimeFields();
        }

        private void minDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddMinutes(-1);
            UpdateTimeFields();
        }

        private void secUp_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddSeconds(1);
            UpdateTimeFields();
        }

        private void secDown_Pushed(object sender, EventArgs e)
        {
            ScratchTime = ScratchTime.AddSeconds(-1);
            UpdateTimeFields();
        }

        private void apply_Click(object sender, EventArgs e)
        {
            ApplyNewTime();
        }

        private void PushPin_Click(object sender, EventArgs e)
        {
            if (this.FormBorderStyle == FormBorderStyle.None)
            {
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                PushPin.Hide();
                this.Top += 10;
                this.Left += 5;
            }
            else
            {
                this.FormBorderStyle = FormBorderStyle.None;
            }
        }

        private void DatePopup_FormClosed(object sender, FormClosedEventArgs e)
        {
            Current = null;
        }

        private void year_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int y = Convert.ToInt32(year.Text);
                ScratchTime = ScratchTime.AddYears(y - ScratchTime.Year);
            }
            catch
            {

            }
            UpdateTimeFields();
        }

        private void month_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int m = Convert.ToInt32(month.Text);
                ScratchTime = ScratchTime.AddMonths(m - ScratchTime.Month);
            }
            catch
            {

            }
            UpdateTimeFields();
        }


        private void day_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int d = Convert.ToInt32(day.Text);
                ScratchTime = ScratchTime.AddDays(d - ScratchTime.Day);
            }
            catch
            {

            }
            UpdateTimeFields();
        }

        private void hours_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int h = Convert.ToInt32(hours.Text);
                ScratchTime = ScratchTime.AddHours(h - ScratchTime.Hour);
            }
            catch
            {

            }
            UpdateTimeFields();
        }

        private void min_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int m = Convert.ToInt32(min.Text);
                ScratchTime = ScratchTime.AddMinutes(m - ScratchTime.Minute);
            }
            catch
            {

            }
            UpdateTimeFields();
        }

        private void sec_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                int s = Convert.ToInt32( this.sec.Text);
                ScratchTime = ScratchTime.AddSeconds(s - ScratchTime.Second);
            }
            catch
            {

            }
            UpdateTimeFields();
        }

        private void Date_TextChanged(object sender, EventArgs e)
        {
            string s = ((TextBox)sender).Text;
            string sOut = "";
            foreach (char c in s)
            {
                if (c >= '0' && c <= '9')
                {
                    sOut += c;
                }
            }
            if (s != sOut)
            {
                ((TextBox)sender).Text = sOut;
            }
        }
    }
}