using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class ControllerProperties : Form
    {
        public MidiMap DeviceMap = null;

        public ControllerProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.label4.Text = Language.GetLocalizedText(1150, "Image URL");
            this.DeviceName.Text = Language.GetLocalizedText(1151, "Device Name Here");
            this.DeviceTypeLabel.Text = Language.GetLocalizedText(1152, "Device Type");
            this.DeviceType.Text = Language.GetLocalizedText(1153, "MIDI Controller");
            this.DeviceStatusLabel.Text = Language.GetLocalizedText(1154, "Status");
            this.DeviceStatus.Text = Language.GetLocalizedText(1155, "Connected");
            this.SentStatus.Text = Language.GetLocalizedText(1156, "Allows Status Update");
            this.Text = Language.GetLocalizedText(1157, "Controller Properties");


        }

        private void DeviceImageUrl_TextChanged(object sender, EventArgs e)
        {
            DeviceImage.ImageLocation = DeviceImageUrl.Text;
            //DeviceImage.Load();
        }

        private void ControllerProperties_Load(object sender, EventArgs e)
        {
            if (DeviceMap != null)
            {
                timer1.Enabled = true;
                DeviceImageUrl.Text = DeviceMap.DeviceImageUrl;
                DeviceName.Text = DeviceMap.Name;
                DeviceType.Text = Language.GetLocalizedText(1153, "MIDI Controller");
                DeviceStatus.Text = DeviceMap.Connected ? Language.GetLocalizedText(1155, "Connected") : Language.GetLocalizedText(1158, "Not Found");
            }
        }

        private void OK_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            DeviceMap.DeviceImageUrl = DeviceImageUrl.Text;
            DeviceMap.Dirty = true;
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DeviceStatus.Text = DeviceMap.Connected ? Language.GetLocalizedText(1155, "Connected") : Language.GetLocalizedText(1158, "Not Found");
        }
    }
}
