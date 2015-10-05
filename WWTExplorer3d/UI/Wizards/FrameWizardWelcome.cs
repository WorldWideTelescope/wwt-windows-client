using System;
using System.Drawing;

namespace TerraViewer
{
    public partial class FrameWizardWelcome : PropPage
    {
        public FrameWizardWelcome()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            label2.Text = Language.GetLocalizedText(822, "This wizard will guide you through the process of creating a new reference frame. A reference frame allows you to have local coordinates, scale and time realztive to the rest of the universe. The referernce can be based on fixed offsets, spherical coordinates, Keplarian orbits or a interpolated values from a list of date/time and position offsets.");
            OffsetTypeLabel.Text = Language.GetLocalizedText(823, "Offset Type");
            ReferenceFrameNameLabel.Text = Language.GetLocalizedText(824, "Refrerence Frame Name");

        }
        ReferenceFrame frame;
        public override void SetData(object data)
        {
            frame = data as ReferenceFrame;
        }

        public override bool Save()
        {
            if (ReferenceFrameName.Text.Length > 0)
            {
                frame.Name = ReferenceFrameName.Text.Trim();
                ReferenceFrameName.BackColor = UiTools.TextBackground;
                return true;
            }
            ReferenceFrameName.BackColor = Color.Red;

            return false;
        }
        Color backGround = Color.White;
        private void FrameWizardWelcome_Load(object sender, EventArgs e)
        {
            backGround = ReferenceFrameName.BackColor;
            ReferenceFrameName.Text = frame.Name;
            OffsetType.Items.AddRange(Enum.GetNames(typeof(ReferenceFrameTypes)));
            OffsetType.SelectedIndex = (int)frame.ReferenceFrameType;
            Binding.SendReadyStatus(this, !(string.IsNullOrEmpty(frame.Name)));

        }

        private void OffsetType_SelectionChanged(object sender, EventArgs e)
        {
            frame.ReferenceFrameType = (ReferenceFrameTypes)OffsetType.SelectedIndex;
        }

        private void ReferenceFrameName_TextChanged(object sender, EventArgs e)
        {
            var nextEnabled = false;

            nextEnabled = !(LayerManager.AllMaps.ContainsKey(ReferenceFrameName.Text.Trim()) ) && !(string.IsNullOrEmpty(ReferenceFrameName.Text.Trim()));
            ReferenceFrameName.BackColor = nextEnabled ? backGround : Color.Red;
            Binding.SendReadyStatus(this, nextEnabled );
        }
    }
}
