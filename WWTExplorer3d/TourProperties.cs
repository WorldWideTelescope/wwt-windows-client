using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
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
                this.tourTitleTextbox.Text = editTour.Title;
                this.authorNameTextbox.Text = editTour.Author;
                this.authorEmailText.Text = editTour.AuthorEmail;
                this.tourDescriptionTextbox.Text = editTour.Description;
                this.authorImagePicturebox.Image = editTour.AuthorImage;
                this.orgUrl.Text = editTour.OrganizationUrl;
                this.OrganizationName.Text = editTour.OrgName;
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
                    this.BeginnerOption.Checked = true;
                    break;
                case UserLevel.Intermediate:
                    this.IntermediateOption.Checked = true;
                    break;
                case UserLevel.Advanced:
                case UserLevel.Professional:
                case UserLevel.Educator:
                    this.AdvancedOption.Checked = true;
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
            this.label1.Text = Language.GetLocalizedText(468, "Author Name");
            this.label2.Text = Language.GetLocalizedText(469, "Tour Description");
            this.label3.Text = Language.GetLocalizedText(470, "Import or Replace Author Image");
            this.groupBox1.Text = Language.GetLocalizedText(471, "Select the level for the tour");
            this.AdvancedOption.Text = Language.GetLocalizedText(472, "Advanced");
            this.IntermediateOption.Text = Language.GetLocalizedText(473, "Intermediate");
            this.BeginnerOption.Text = Language.GetLocalizedText(474, "Beginner");
            this.label4.Text = Language.GetLocalizedText(475, "Tour Title *");
            this.label5.Text = Language.GetLocalizedText(476, "Organization URL");
            this.ClassificationTaxonomyLabel.Text = Language.GetLocalizedText(477, "Classification Taxonomy");
            this.label6.Text = Language.GetLocalizedText(478, "* Required Field");
            this.label7.Text = Language.GetLocalizedText(479, "Author Contact Email");
            this.label8.Text = Language.GetLocalizedText(480, "Catalog Objects and Keywords (semicolon separated)");
            this.ShowTaxTree.Text = Language.GetLocalizedText(481, "Classification");
            this.OK.Text = Language.GetLocalizedText(156, "OK");
            this.Cancel.Text = Language.GetLocalizedText(157, "Cancel");
            this.ImportAuthorImage.Text = Language.GetLocalizedText(482, "Import Image");
            this.label9.Text = Language.GetLocalizedText(483, "Organization Name");
            this.Text = Language.GetLocalizedText(418, "Tour Properties");
        }

        private void OK_Click(object sender, EventArgs e)
        {
            //todo localize
            Undo.Push(new UndoTourPropertiesChange(Language.GetLocalizedText(549, "Properties Edit"), editTour));    

            editTour.Author = this.authorNameTextbox.Text;
            editTour.AuthorEmail = this.authorEmailText.Text;
            editTour.Description = this.tourDescriptionTextbox.Text;
            editTour.AuthorImage = (Bitmap)this.authorImagePicturebox.Image;
            editTour.OrganizationUrl = this.orgUrl.Text;
            editTour.OrgName = this.OrganizationName.Text;
            editTour.Title = this.tourTitleTextbox.Text;
            editTour.Taxonomy = ClassificationText.Text;
            editTour.Keywords = keywords.Text;
            editTour.Level = GetLevel();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private UserLevel GetLevel()
        {
            if (this.AdvancedOption.Checked)
            {
                return UserLevel.Advanced;
            }    
            if (this.IntermediateOption.Checked)
            {
                return UserLevel.Intermediate;
            }        

            return UserLevel.Beginner;
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
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
            TaxonomyTree treeEdit = new TaxonomyTree();
            treeEdit.Taxonomy = ClassificationText.Text;

            if (treeEdit.ShowDialog() == DialogResult.OK)
            {

                ClassificationText.Text = treeEdit.Taxonomy;
            }
        }

        private void ImportAuthorImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
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
                    this.authorImagePicturebox.Image = bmpTemp;
                    ValidateDataComplete();
                }
            }
        }

        public bool Strict = false;
        public bool highlightNeeded = false;
        public bool authorImageNeeded = false;
        private void ValidateDataComplete()
        {
            bool okEnabled = true;

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

            if (Strict && string.IsNullOrEmpty(this.authorNameTextbox.Text))
            {
                okEnabled = false;
                this.authorNameTextbox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                this.authorNameTextbox.BackColor = OrganizationName.BackColor;
            }

            if (Strict && string.IsNullOrEmpty(this.tourDescriptionTextbox.Text))
            {
                okEnabled = false;
                this.tourDescriptionTextbox.BackColor = highlightNeeded ? Color.Red : OrganizationName.BackColor;
            }
            else
            {
                this.tourDescriptionTextbox.BackColor = OrganizationName.BackColor;
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
                this.Close();
            }
        }

        public void highlightReqFields()
        {
            ValidateDataComplete();
        }
    }
}