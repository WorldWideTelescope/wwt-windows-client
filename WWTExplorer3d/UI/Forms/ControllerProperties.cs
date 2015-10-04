using System;
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
            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            OK.Text = Language.GetLocalizedText(156, "OK");
            label4.Text = Language.GetLocalizedText(1150, "Image URL");
            DeviceName.Text = Language.GetLocalizedText(1151, "Device Name Here");
            DeviceTypeLabel.Text = Language.GetLocalizedText(1152, "Device Type");
            DeviceType.Text = Language.GetLocalizedText(1153, "MIDI Controller");
            DeviceStatusLabel.Text = Language.GetLocalizedText(1154, "Status");
            DeviceStatus.Text = Language.GetLocalizedText(1155, "Connected");
            SentStatus.Text = Language.GetLocalizedText(1156, "Allows Status Update");
            Text = Language.GetLocalizedText(1157, "Controller Properties");


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
            DialogResult = DialogResult.OK;
            DeviceMap.DeviceImageUrl = DeviceImageUrl.Text;
            DeviceMap.Dirty = true;
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DeviceStatus.Text = DeviceMap.Connected ? Language.GetLocalizedText(1155, "Connected") : Language.GetLocalizedText(1158, "Not Found");
        }
    }
}
