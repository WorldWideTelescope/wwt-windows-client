using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerraViewer.Callibration
{
    public partial class GridCreator : Form
    {
        public GridCreatorParams Paramaters = new GridCreatorParams();

        public GridCreator()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            try
            {
                Paramaters.AltMin = float.Parse(AltMin.Text);
                Paramaters.AltMax = float.Parse(AltMax.Text);
                Paramaters.AltStep = float.Parse(AltStep.Text);
                Paramaters.AzMin = float.Parse(AzMin.Text);
                Paramaters.AzMax = float.Parse(AzMax.Text);
                Paramaters.AzStep = float.Parse(AzStep.Text);
                DialogResult = DialogResult.OK;
            }
            catch
            {
             
            }
        }

        private void GridCreator_Load(object sender, EventArgs e)
        {
            AltMin.Text = Paramaters.AltMin.ToString();
            AltMax.Text = Paramaters.AltMax.ToString();
            AltStep.Text = Paramaters.AltStep.ToString();
            AzMin.Text = Paramaters.AzMin.ToString();
            AzMax.Text = Paramaters.AzMax.ToString();
            AzStep.Text =  Paramaters.AzStep.ToString();
        }
    }

    public class GridCreatorParams
    {
        public float AltMin = 0;
        public float AltMax = 80;
        public float AltStep = 10;
        public float AzMin = 0;
        public float AzMax = 360;
        public float AzStep = 10;
    }
}
