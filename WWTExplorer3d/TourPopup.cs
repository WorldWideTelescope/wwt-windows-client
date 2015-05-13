using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace TerraViewer
{
    public partial class TourPopup : Form
    {
        public event EventHandler LaunchTour;
        public event EventHandler ClosedTour;

        static TourPopup endTour = null;

        static public void CloseTourPopups()
        {
            if (endTour != null)
            {
                endTour.Close();
                endTour = null;
            }
        }

        static public TourPopup ShowEndTourPopup(TourDocument tour)
        {
            CloseTourPopups();
            if (endTour != null)
            {
                endTour.Close();
                endTour = null;
            }

            endTour = new TourPopup();
            endTour.PopupType = TourPopup.PopupTypes.TourOver;
            Tour tourResult = FolderBrowser.GetRelatedTour(tour.Id);
            if (tourResult == null)
            {
                tourResult = FolderBrowser.GetRelatedTour(tour.TagId);
            }

            if (tourResult == null)
            {
                tourResult = new Tour();
                tourResult.Author = tour.Author;
                tourResult.Id = tour.Id;
                tourResult.AuthorUrl = tour.AuthorUrl;
                tourResult.Description = tour.Description;
                tourResult.AuthorImage = tour.AuthorImage;
                tourResult.OrgName = tour.OrgName;
                tourResult.OrganizationUrl = tour.OrgUrl;
                tourResult.Title = tour.Title;
                tourResult.AverageUserRating = 3;
            }
            endTour.TourResult = tourResult;
            endTour.Show();
            return endTour;
        }
        static public DialogResult ShowEndTourPopupModal(TourDocument tour)
        {
            CloseTourPopups();
            if (endTour != null)
            {
                endTour.Close();
                endTour = null;
            }

            endTour = new TourPopup();
            endTour.PopupType = TourPopup.PopupTypes.TourOver;
            Tour tourResult = FolderBrowser.GetRelatedTour(tour.Id);
            if (tourResult == null)
            {
                tourResult = FolderBrowser.GetRelatedTour(tour.TagId);
            }

            if (tourResult == null)
            {
                tourResult = new Tour();
                tourResult.Author = tour.Author;
                tourResult.Id = tour.Id;
                tourResult.AuthorUrl = tour.AuthorUrl;
                tourResult.Description = tour.Description;
                tourResult.AuthorImage = tour.AuthorImage;
                tourResult.OrgName = tour.OrgName;
                tourResult.OrganizationUrl = tour.OrgUrl;
                tourResult.Title = tour.Title;
                tourResult.AverageUserRating = 3;
            }
            Cursor.Show();
            endTour.TourResult = tourResult;
            return endTour.ShowDialog();
        }
        public TourPopup()
        {
            InitializeComponent();
            SetUiStrings();
            Owner = Earth3d.MainWindow;
        }

        private void SetUiStrings()
        {
            this.ratingLabel.Text = Language.GetLocalizedText(459, "Rating:");
            this.runLengthLabel.Text = Language.GetLocalizedText(460, "Run Length:");
            this.toolTips.SetToolTip(this.averageStars, Language.GetLocalizedText(461, "Average rating from users"));
            this.toolTips.SetToolTip(this.MyRating, Language.GetLocalizedText(462, "Click to set your own rating."));
            this.yourRatingLabel.Text = Language.GetLocalizedText(463, "Your Rating:");
            this.CloseTour.Text = Language.GetLocalizedText(464, "Close Tour");
            this.label3.Text = Language.GetLocalizedText(465, "Related Tours:");
            this.WatchAgain.Text = Language.GetLocalizedText(466, "Watch Again");
            this.relatedTours.AddText = Language.GetLocalizedText(161, "Add New Item");
            this.relatedTours.EmptyAddText = Language.GetLocalizedText(162, "No Results");
        }
        ITourResult tourResult;

        public ITourResult TourResult
        {
            get { return tourResult; }
            set { tourResult = value; }
        }

        public enum PopupTypes { Popup, TourOver };

        PopupTypes popupType = PopupTypes.Popup;

        public  PopupTypes PopupType
        {
            get { return popupType; }
            set { popupType = value; }
        }


        private void TourPopup_Load(object sender, EventArgs e)
        {
            if (popupType == PopupTypes.Popup)
            {
                Height = 335;
                
                Preview.Visible = true;
                CloseTour.Visible = false;
                WatchAgain.Visible = false;
                tourWrapPanel.Visible = false;
                averageStars.Visible = true;
                ratingLabel.Visible = true;
                runLengthLabel.Visible = true;
                runLength.Visible = true;
            }
            else
            {
                Height = 405;
                TourTitle.Left -= 53;
                TourTitle.Width = 430;
                Preview.Visible = false;
                CloseTour.Visible = true;
                WatchAgain.Visible = true;
                tourWrapPanel.Visible = true;
                averageStars.Visible = false;
                ratingLabel.Visible = false;
                runLengthLabel.Visible = false;
                runLength.Visible = false;

                Left = (Earth3d.MainWindow.RenderWindow.ClientRectangle.Width - this.Width) / 2;
                Top = (Earth3d.MainWindow.RenderWindow.ClientRectangle.Height - this.Height) / 2;

                MyRating.Stars = GetMyRating(tourResult.Id);
            };

            averageStars.Stars = tourResult.AverageUserRating;


            TourTitle.Text = tourResult.Title;

            if (TourTitle.Text.Length < 23)
            {
                TourTitle.Font = UiTools.StandardGargantuan;
            }
            else if (TourTitle.Text.Length < 30)
            {
                TourTitle.Font = UiTools.StandardHuge;
            }
            else if (TourTitle.Text.Length < 40)
            {
                TourTitle.Font = UiTools.StandardLarge;
            }
            else if (TourTitle.Text.Length < 50)
            {
                TourTitle.Font = UiTools.StandardRegular;
            }
            else
            {
                TourTitle.Font = UiTools.StandardSmall;
            }

            TourDescription.Text = tourResult.Description;
            authorImage.Image = tourResult.AuthorImage;
            authorUrl.Text = tourResult.Author;
            if (tourResult.OrgName != "None")
            {
                orgUrl.Text = tourResult.OrgName;
            }
            this.ttTourPopup.SetToolTip(TourTitle, tourResult.Title);


            Rectangle rect = Screen.GetWorkingArea(this);

            if (this.Left + this.Width > rect.Width)
            {
                this.Left -= (this.Left + this.Width) - rect.Width;
            }

            TimeSpan ts = new TimeSpan(0,0,(int)tourResult.LengthInSeconds);
            this.runLength.Text = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

            fadein = new BlendState(false, 500);
            fadein.TargetState = true;
            if (!string.IsNullOrEmpty(tourResult.RelatedTours))
            {
                string[] relatedList = tourResult.RelatedTours.Split(new char[] { ';' });
                foreach (string id in relatedList)
                {
                    Tour relatedItem = FolderBrowser.GetRelatedTour(id);
                    if (relatedItem != null)
                    {
                        relatedTours.Add(relatedItem);
                    }
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            Rectangle rect = new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            Pen p = new Pen(Color.FromArgb(58, 69, 91));
            e.Graphics.DrawRectangle(p, rect);
            p.Dispose();
        }
        public bool Locked = false;

        private void TourPopup_MouseEnter(object sender, EventArgs e)
        {
            Locked = true;
        }

        private void TourPopup_MouseLeave(object sender, EventArgs e)
        {
            if (popupType == PopupTypes.Popup)
            {
                if (!Bounds.Contains(Cursor.Position))
                {
                    Close();
                }
            }
        }

        private void authorUrl_LinkClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tourResult.AuthorUrl))
            {
                return;
            }

            WebWindow.OpenUrl(tourResult.AuthorUrl, false);

            if (popupType == PopupTypes.Popup)
            {
                Close();
            }
        }

        private void orgUrl_LinkClicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(tourResult.OrganizationUrl))
            {
                return;
            }
            WebWindow.OpenUrl(tourResult.OrganizationUrl, false);
            if (popupType == PopupTypes.Popup)
            {
                Close();
            }
        }

        private void Preview_Click(object sender, EventArgs e)
        {
            if (LaunchTour != null)
            {
                LaunchTour.Invoke(this, new EventArgs());
            }
        }

        private void Preview_MouseEnter(object sender, EventArgs e)
        {
            Preview.Image = global::TerraViewer.Properties.Resources.button_play_hover;

        }

        private void Preview_MouseLeave(object sender, EventArgs e)
        {
            Preview.Image = global::TerraViewer.Properties.Resources.button_play_normal;

        }

        private void Preview_MouseDown(object sender, MouseEventArgs e)
        {
            Preview.Image = global::TerraViewer.Properties.Resources.button_play_pressed;
        }

        private void Preview_MouseUp(object sender, MouseEventArgs e)
        {
            Preview.Image = global::TerraViewer.Properties.Resources.button_play_hover;
        }
        BlendState fadein = new BlendState(false, 500);
        private void fadeInTimer_Tick(object sender, EventArgs e)
        {
            if (fadein.Opacity == 1.0)
            {
                Opacity = .8;
                fadeInTimer.Enabled = false;
            }
            else
            {
                Opacity = fadein.Opacity * .8;
            }
        }

        private void CloseTour_Click(object sender, EventArgs e)
        {
            if (ClosedTour != null)
            {
                ClosedTour.Invoke(this, new EventArgs());
            }
            this.DialogResult = DialogResult.Cancel;

            SendUpdatedRating();

            Close();
        }

        private void WatchAgain_Click(object sender, EventArgs e)
        {
            if (LaunchTour != null)
            {
                LaunchTour.Invoke(this, new EventArgs());
            }
            this.DialogResult = DialogResult.OK;
      
            SendUpdatedRating();
   
            Close();
        }
        private void SendUpdatedRating()
        {
            if (changed)
            {
                Tour tour = tourResult as Tour;
                if (tour != null)
                {
                    if (tour.MSRComponentId > 0)
                    {
                        EOCalls.InvokeRateContent((int)tour.MSRComponentId, myStars);
                        return;
                    }
                }

                string userId = Properties.Settings.Default.UserRatingGUID.ToString("D");
                SaveTourRating();
                UiTools.SendAsyncWebMessage(String.Format("http://www.worldwidetelescope.org/wwtweb/PostRatingFeedback.aspx?q={0},{1},{2}", tourResult.Id, userId, myStars.ToString()));
            }
        }

        private void SaveTourRating()
        {
            try
            {
                string directory = Properties.Settings.Default.CahceDirectory + "TourRatings\\";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(directory + tourResult.Id.ToString() + ".rating", myStars.ToString());
            }
            catch
            {
            }
        }
        private int GetMyRating(string p)
        {
            string value = "0";
            string directory = Properties.Settings.Default.CahceDirectory + "TourRatings\\";

            try
            {
                string filename = directory + tourResult.Id.ToString() + ".rating";
                if (File.Exists(filename))
                {
                    value = File.ReadAllText(filename);
                }
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }
        int myStars = -1;
        bool changed = false;
        private void MyRating_ValueChanged(object sender, EventArgs e)
        {
            myStars = (int)MyRating.Stars;
            MyRating.Stars = myStars;
            changed = true;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void relatedTours_ItemClicked(object sender, object e)
        {
            if (e is Tour)
            {
                FolderBrowser.LaunchTour((Tour)e);
                DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        private void relatedTours_ItemDoubleClicked(object sender, object e)
        {

        }

        private void relatedTours_ItemHover(object sender, object e)
        {

        }
    }
}