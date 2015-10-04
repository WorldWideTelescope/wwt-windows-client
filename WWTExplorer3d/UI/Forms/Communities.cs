using System;

namespace TerraViewer
{
    public partial class Communities : TabForm
    {
        public Communities()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            resultsList.AddText = Language.GetLocalizedText(161, "Add New Item");
            resultsList.EmptyAddText = Language.GetLocalizedText(162, "No Results");
            selectText.Text = Language.GetLocalizedText(163, "Select a Community...");
            Text = Language.GetLocalizedText(138, "Community");
        }

        private void Communities_Load(object sender, EventArgs e)
        {

            resultsList.ThumbnailSize = ThumbnailSize.Big;
            resultsList.AddText = Language.GetLocalizedText(161, "Add New Item");
            resultsList.ShowAddButton = true;
        }

        private void resultsList_Load(object sender, EventArgs e)
        {

        }

        private void resultsList_ItemDoubleClicked(object sender, Object e)
        {

        }
        protected override void SetFocusedChild()
        {
            resultsList.Focus();
        }

        private void resultsList_AddNewItem(object sender, object e)
        {
            WebWindow.OpenUrl("http://www.worldwidetelescope.org/Community/Profile", true);
        }
    }
}