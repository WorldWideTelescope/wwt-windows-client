using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ContextPanelOld : Form
    {
        public ContextPanelOld()
        {
            SetStyle(ControlStyles.UserPaint |  ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();
            paginator1.PageChanged += this.contextResults.PageChanged;
        }
        int level;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        double ra;

        public double RA
        {
            get { return ra; }
            set
            {
                ra = value;
                SkyBall.RA = ra;
            }
        }
        double dec;

        public double Dec
        {
            get { return dec; }
            set 
            {
                dec = value;
                SkyBall.Dec = dec;
            }
        }
        string constellation;

        bool contextAreaChanged = false;

        public string Constellation
        {
            get { return constellation; }
            set
            {
                if (constellation != value)
                {
                    constellation = value;
                    ContellationLabel.Text = value;
                    overview.Constellation = value;
                    contextAreaChanged = true;
                }
            }
        }
        int viewLevel = 1;

        public int ViewLevel
        {
            get { return viewLevel; }
            set 
            {
                if (viewLevel != value)
                {
                    viewLevel = value;
                    levelLabel.Text = string.Format("L{0}", viewLevel);
                }
            }
        }
        Coordinates[] cornersLast = null;

        public void SetViewRect(Coordinates[] corners)
        {
            if (corners.Length != 4)
            {
                return;
            }
            bool change = false;
            if (this.cornersLast != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (this.cornersLast[i] != corners[i])
                    {
                        change = true;
                        break;
                    }
                }
            }
            else
            {
                change = true;
            }

            if (!change)
            {
                return;
            }
            this.cornersLast = corners;
            contextAreaChanged = true;

            overview.SetViewRect(corners);
            SkyBall.SetViewRect(corners);
        }

        int queueProgress;

        public int QueueProgress
        {
            get
            {
                return queueProgress;
            }

            set
            {
                queueProgress = value;
                queueProgressBar.Value = value;
                queueProgressBar.Refresh();

            }
        }

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            if (contextAreaChanged && cornersLast != null && !String.IsNullOrEmpty(constellation))
            {
                contextResults.Clear();
               
                Place[] results = ContextSearch.FindConteallationObjects(Constellations.Abbreviation(constellation), cornersLast, Classification.Unfiltered);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                contextResults.Refresh();
                paginator1.CurrentPage = 0;
                paginator1.TotalPages = contextResults.PageCount;
            }

            contextAreaChanged = false;
        }

        private void ContextPanel_Load(object sender, EventArgs e)
        {
            FilterCombo.Items.AddRange(Enum.GetNames(typeof(Classification)));
        }

        private void pinUp_Clicked(object sender, EventArgs e)
        {
            int c = 1;
        }

        private void contextResults_ItemClicked(object sender, Place e)
        {
            Earth3d.MainWindow.GotoTarget(e, false, false);
        }

        private void contextResults_ItemDoubleClicked(object sender, Place e)
        {
            Earth3d.MainWindow.GotoTarget(e, false, false);
        }
    }
}