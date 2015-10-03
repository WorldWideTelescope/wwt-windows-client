using System;
using System.Windows.Forms;

namespace TerraViewer.Callibration
{
    public partial class EdgeProperties : Form
    {
        public EdgeProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Ok.Text = Language.GetLocalizedText(759, "Ok");
            this.leftLabel.Text = Language.GetLocalizedText(760, "Left Projector");
            this.label2.Text = Language.GetLocalizedText(761, "Right Projector");
            this.Text = Language.GetLocalizedText(762, "Edge Properties");
        }
        
        public Edge Edge;
       
        private void EdgeProperties_Load(object sender, EventArgs e)
        {
            foreach (var pe in Edge.Owner.Projectors)
            {
                leftProjectorCombo.Items.Add(pe);
                rightProjectorCombo.Items.Add(pe);
            }
        }

        private void leftProjectorCombo_SelectionChanged(object sender, EventArgs e)
        {
            Edge.Left = ((ProjectorEntry)leftProjectorCombo.SelectedItem).ID;
        }

        private void rightProjectorCombo_SelectionChanged(object sender, EventArgs e)
        {
            Edge.Right = ((ProjectorEntry)rightProjectorCombo.SelectedItem).ID;
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
