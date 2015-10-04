using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TourProperties : Form
    {
        TourDocument editTour;

        internal TourDocument EditTour
        {
            get { return editTour; }
            set
            {
                editTour = value;
                tourTitleTextbox.Text = editTour.Title;
                authorNameTextbox.Text = editTour.Author;
                authorEmailText.Text = editTour.AuthorEmail;
                tourDescriptionTextbox.Text = editTour.Description;
                authorImagePicturebox.Image = editTour.AuthorImage;
                orgUrl.Text = editTour.OrganizationUrl;
                OrganizationName.Text = editTour.OrgName;
                keywords.Text = editTour.Keywords;
                ClassificationText.Text = editTour.Taxonomy;
                SetLevel(editTour.Level);

            }
        }

        private void SetLevel(UserLevel userLevel)
        {
            switch (userLevel)
            {
                case UserLevel.Beginner:
                    BeginnerOption.Checked = true;
                    break;
                case UserLevel.Intermediate:
                    IntermediateOption.Checked = true;
                    break;
                case UserLevel.Advanced:
                case UserLevel.Professional:
                case UserLevel.Educator:
                    AdvancedOption.Checked = true;
                    break;
            }
        }

        public TourProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            label1.Text = Language.GetLocalizedText(468, "Author Name");
            label2.Text = Language.GetLocalizedText(469, "Tour Description");
            label3.Text = Language.GetLocalizedText(470, "Import or Replace Author Image");
            groupBox1.Text = Language.GetLocalizedText(471, "Select the level for the tour");
            AdvancedOption.Text = Language.GetLocalizedText(472, "Advanced");
            IntermediateOption.Text = Language.GetLocalizedText(473, "Intermediate");
            BeginnerOption.Text = Language.GetLocalizedText(474, "Beginner");
            label4.Text = Language.GetLocalizedText(475, "Tour Title *");
            label5.Text = Language.GetLocalizedText(476, "Organization URL");
            ClassificationTaxonomyLabel.Text = Language.GetLocalizedText(477, "Classification Taxonomy");
            label6.Text = Language.GetLocalizedText(478, "* Required Field");
            label7.Text = Language.GetLocalizedText(479, "Author Contact Email");
            label8.Text = Language.GetLocalizedText(480, "Catalog Objects and Keywords (semicolon separated)");
            ShowTaxTree.Text = Language.GetLocalizedText(481, "Classification");
            OK.Text = Language.GetLocalizedText(156, "OK");
            Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            ImportAuthorImage.Text = Language.GetLocalizedText(482, "Import Image");
            label9.Text = Language.GetLocalizedText(483, "Organization Name");
            Text = Language.GetLocalizedText(418, "Tour Properties");
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourPropertiesChange(Language.GetLocalizedText(549, "Properties Edit"), editTour));    

            editTour.Author = authorNameTextbox.Text;
            editTour.AuthorEmail = authorEmailText.Text;
            editTour.Description = tourDescriptionTextbox.Text;
            editTour.AuthorImage = (Bitmap)authorImagePicturebox.Image;
            editTour.OrganizationUrl = orgUrl.Text;
            editTour.OrgName = OrganizationName.Text;
            editTour.Title = tourTitleTextbox.Text;
            editTour.Taxonomy = ClassificationText.Text;
            editTour.Keywords = keywords.Text;
            editTour.Level = GetLevel();
            DialogResult = DialogResult.OK;
            Close();
        }

        private UserLevel GetLevel()
        {
            if (AdvancedOption.Checked)
            {
                return UserLevel.Advanced;
            }    
            if (IntermediateOption.Checked)
            {
                return UserLevel.Intermediate;
            }        

            return UserLevel.Beginner;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void TourProperties_Load(object sender, EventArgs e)
        {
            wwtCombo1.Tag = Classification.Galaxy;
            if (tourTitleTextbox.Text == "")
            {
                tourTitleTextbox.Text = Language.GetLocalizedText(485, "Please enter a title for the tour...");
            }
        }

        private void Classification_Click(object sender, EventArgs e)
        {
            var treeEdit = new TaxonomyTree();
            treeEdit.Taxonomy = ClassificationText.Text;

            if (treeEdit.ShowDialog() == DialogResult.OK)
            {

                ClassificationText.Text = treeEdit.Taxonomy;
            }
        }

        private void ImportAuthorImage_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(486, "Image Files")+"(*.BMP;*.JPG;*.PNG;*.TIF)|*.BMP;*.JPG;*.PNG;*.TIF;*.TIFF|All files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                Bitmap bmpTemp = null;
                
                try
                {
                    bmpTemp = Thumbnailer.MakeThumbnail(UiTools.LoadBitmap(openFile.FileName), 72, 96);
                }
                catch
                {
                    bmpTemp = null;
                }

                if (bmpTemp != null)
                {
                    authorImagePicturebox.Image = bmpTemp;
                    ValidateDataComplete();
                }
            }
        }

        public bool Strict = false;
        public bool highlightNeeded = false;
        public bool authorImageNeeded = false;
        private void ValidateDataComplete()
        {
            var okEnabled = true;

            if (String.IsNullOrEmpty(tourTitleTextbox.Text) || tourTitleTextbox.Text == Language.GetLocalizedText(485, "Please enter a title for the tour..."))
            {
                okEnabled = false;
                tourTitleTextbox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                tourTitleTextbox.BackColor = OrganizationName.BackColor;
            }

           // if (string.IsNullOrEmpty(authorEmailText.Text) || UiTools.IsEmail(authorEmailText.Text))
            if (Strict && !UiTools.IsEmail(authorEmailText.Text))
            {
                okEnabled = false;
                authorEmailText.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                authorEmailText.BackColor = OrganizationName.BackColor;
            }

            //todo validate URLS's

            if (Strict && string.IsNullOrEmpty(authorNameTextbox.Text))
            {
                okEnabled = false;
                authorNameTextbox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                authorNameTextbox.BackColor = OrganizationName.BackColor;
            }

            if (Strict && string.IsNullOrEmpty(tourDescriptionTextbox.Text))
            {
                okEnabled = false;
                tourDescriptionTextbox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                tourDescriptionTextbox.BackColor = OrganizationName.BackColor;
            }

            if (authorImageNeeded && authorImagePicturebox.Image == null)
            {
                okEnabled = false;
                authorImagePicturebox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                authorImagePicturebox.BackColor = OrganizationName.BackColor;
            }

            OK.Enabled = okEnabled;
        }

        private void tourTitleTextbox_TextChanged(object sender, EventArgs e)
        {
            ValidateDataComplete();
        }

        private void tourTitleTextbox_Enter(object sender, EventArgs e)
        {
            if (tourTitleTextbox.Text == Language.GetLocalizedText(485, "Please enter a title for the tour..."))
            {
                tourTitleTextbox.SelectAll();
            }
        }

        private void tourTitleTextbox_MouseClick(object sender, MouseEventArgs e)
        {
            if (tourTitleTextbox.Text == Language.GetLocalizedText(485, "Please enter a title for the tour..."))
            {
                tourTitleTextbox.SelectAll();
            }
        }


        private void tourDescriptionTextbox_TextChanged(object sender, EventArgs e)
        {
            ValidateDataComplete();
        }

        private void authorNameTextbox_TextChanged(object sender, EventArgs e)
        {
            ValidateDataComplete();
        }

        private void authorEmailText_TextChanged(object sender, EventArgs e)
        {
            ValidateDataComplete();
        }

        private void TourProperties_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        public void highlightReqFields()
        {
            ValidateDataComplete();
        }
    }
}