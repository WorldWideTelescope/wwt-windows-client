using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardFinal : PropPage
    {
        public DataWizardFinal()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.label1.Text = Language.GetLocalizedText(845, "Your data is now ready for viewing. If you need to change any options just right-click on the layer and select the Properties option on the context menu.");
        }
        TimeSeriesLayer layer = null;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            return true;
        }

        private void DataWizardFinal_Load(object sender, EventArgs e)
        {

        }
    }
}
