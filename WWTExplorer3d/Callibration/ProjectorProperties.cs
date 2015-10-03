using System;
using System.Globalization;
using System.Windows.Forms;

namespace TerraViewer.Callibration
{
    public partial class ProjectorProperties : Form
    {
        public ProjectorProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(782, "ID");
            this.label2.Text = Language.GetLocalizedText(238, "Name");
            this.projectorListLabel.Text = Language.GetLocalizedText(783, "Projector List");
            this.ProjectorTab.Title = Language.GetLocalizedText(784, "Projector");
            this.ViewTab.Title = Language.GetLocalizedText(140, "View");
            this.OK.Text = Language.GetLocalizedText(759, "Ok");
            this.Solved.Title = Language.GetLocalizedText(785, "Solved");
            this.CopySolved.Text = Language.GetLocalizedText(786, "Copy Solved");
            this.CopyView.Text = Language.GetLocalizedText(787, "Copy View");
            this.updateClient.Text = Language.GetLocalizedText(788, "Send Update");
            this.Text = Language.GetLocalizedText(789, "Projector Properties");
        }

        public ProjectorEntry Projector;
        public CalibrationInfo CalibrationInfo;
        public bool AddMode = false;

        private void ProjectorProperties_Load(object sender, EventArgs e)
        {
            var selected = 0;
            var index = 0;
            if (!AddMode)
            {
                foreach (var pe in CalibrationInfo.Projectors)
                {
                    if (pe == Projector)
                    {
                        selected = index;
                    }
                    projectorList.Items.Add(pe);
                    index++;
                }
                projectorList.SelectedIndex = selected;
            }
            else
            {
                projectorList.Visible = false;
                projectorListLabel.Visible = false;
            }
            SyncProjectorEntry();
        }

        private void SyncProjectorEntry()
        {
            projectorID.Text = Projector.ID.ToString(CultureInfo.InvariantCulture);
            projectorName.Text = Projector.Name;
            if (ViewTab.Selected)
            {
                projectionTransformEditor.ProjTarget = Projector.ViewProjection;
                projectionTransformEditor.TransTarget = Projector.ViewTransform;
            }
            else if (ProjectorTab.Selected)
            {
                projectionTransformEditor.ProjTarget = Projector.ProjectorProjection;
                projectionTransformEditor.TransTarget = Projector.ProjectorTransform;
            }
            else
            {
                projectionTransformEditor.ProjTarget = Projector.SolvedProjection;
                projectionTransformEditor.TransTarget = Projector.SolvedTransform;
            }
        }

        private void ok_Click(object sender, EventArgs e)
        {
            UpdateProjectorEntry();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void UpdateProjectorEntry()
        {
            if (!string.IsNullOrEmpty(projectorName.Text))
            {
                Projector.Name = projectorName.Text;

            }
            if (!string.IsNullOrEmpty(projectorID.Text))
            {
                Projector.ID = int.Parse(projectorID.Text);

            }
        }

        private void ViewTab_Click(object sender, EventArgs e)
        {
            ViewTab.Selected = true;
            ProjectorTab.Selected = false;
            Solved.Selected = false;
            projectionTransformEditor.ProjTarget = Projector.ViewProjection;
            projectionTransformEditor.TransTarget = Projector.ViewTransform;

        }



        private void ProjectorTab_Click(object sender, EventArgs e)
        {
            ViewTab.Selected = false;
            ProjectorTab.Selected = true;
            Solved.Selected = false;
            projectionTransformEditor.ProjTarget = Projector.ProjectorProjection;
            projectionTransformEditor.TransTarget = Projector.ProjectorTransform;

        }

        private void Solved_Click(object sender, EventArgs e)
        {
            Solved.Selected = true;
            ViewTab.Selected = false;
            ProjectorTab.Selected = false;

            projectionTransformEditor.ProjTarget = Projector.SolvedProjection;
            projectionTransformEditor.TransTarget = Projector.SolvedTransform;

        }

        private void projectorList_SelectionChanged(object sender, EventArgs e)
        {
            UpdateProjectorEntry();
            Projector = (ProjectorEntry)projectorList.SelectedItem;
            SyncProjectorEntry();
        }

        private void CopySolved_Click(object sender, EventArgs e)
        {
            foreach (var pe in CalibrationInfo.Projectors)
            {
                pe.ProjectorProjection = pe.SolvedProjection.Copy();
                pe.ProjectorTransform = pe.SolvedTransform.Copy();
              
            }
            SyncProjectorEntry();
        }

        private void CopyView_Click(object sender, EventArgs e)
        {
            foreach (var pe in CalibrationInfo.Projectors)
            {
                pe.ProjectorProjection = pe.ViewProjection.Copy();
                pe.ProjectorTransform = pe.ViewTransform.Copy();

            }
            SyncProjectorEntry();
        }
        
        private void updateClient_Click(object sender, EventArgs e)
        {
            Calibration.SendViewConfig(Projector.ID, Projector, CalibrationInfo.DomeTilt);
        }

        private void projectorID_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButtonStatus();
        }

        private void projectorName_TextChanged(object sender, EventArgs e)
        {
            UpdateOkButtonStatus();
        }

        void UpdateOkButtonStatus()
        {
            if (projectorID.Text.Length > 0 && projectorName.Text.Length > 0)
            {
                OK.Enabled = true;
            }
            else
            {
                OK.Enabled = false;
            }
        }
    

    }
}
