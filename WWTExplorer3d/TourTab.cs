using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using TerraViewer.org.worldwidetelescope.www;

namespace TerraViewer
{
    public partial class ToursTab : TabForm
    {
        public ToursTab()
        {
            InitializeComponent();
            SetStyle(ControlStyles.ResizeRedraw, true);
        }
        private void Discover_Load(object sender, EventArgs e)
        {
            //foreach (XmlNode imageset in imageSets.ChildNodes)
            //{
            //    ImageSet newImageset =  ImageSet.FromXMLNode(imageset);
            //    newImageset.WidthFactor = 1;
            //    Place newPlace = new Place(newImageset.Name, newImageset.CenterY, newImageset.CenterX/15, Classification.BlackHole, Earth3d.MainWindow.Constellation, DataSetType.Sky, -1);
            //    newPlace.StudyImageset = newImageset;

            //    //newImageset.Levels = 1;

                
            //    string filename = newImageset.ThumbnailUrl.Substring(newImageset.ThumbnailUrl.LastIndexOf("/")+1);
            //    DataSetManager.DownloadFile(newImageset.ThumbnailUrl, Properties.Settings.Default.CahceDirectory + @"thumbnails\" + filename, false);

            //    newPlace.ThumbNail = UiTools.LoadBitmap(Properties.Settings.Default.CahceDirectory + @"thumbnails\"+ filename);
            //    resultsList.Add(newPlace);
            //    hubbleImages.Add(newPlace.StudyImageset);
            //    // Add to Constellation Objects
            //    String constellationID = Constellations.Containment.FindConstellationForPoint(newPlace.RA, newPlace.Dec);
            //    if (constellationID != "Error")
            //    {
            //        newPlace.Constellation = constellationID;
            //        ContextSearch.contellationObjects[constellationID].Add(newPlace);
            //    }


            //}

         }
        private void LoadTours()
        {
            Cursor.Current = Cursors.WaitCursor;

            resultsList.Clear();
            resultsList.Refresh();
            string filename = Properties.Settings.Default.CahceDirectory + @"data\tours.wtml";

            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/gettours.aspx", filename, false, false);

            Folder tours = Folder.LoadFromFile(filename);

            foreach (Tour result in tours.Tour)
            {
                resultsList.Add(result);
            }
            Cursor.Current = Cursors.Default;

        }
        protected override void SetFocusedChild()
        {
            resultsList.Focus();
        }

        private void LoadToursViaWebService()
        {
            Cursor.Current = Cursors.WaitCursor;

            WWTWebService webService = new WWTWebService();


            TerraViewer.org.worldwidetelescope.www.Tour[] results = webService.GetToursForDateRange("", "");


            foreach (TerraViewer.org.worldwidetelescope.www.Tour resultRow in results)
            {
                Tour result = new Tour();
                result.Id = resultRow.TourGuid.ToString();
                result.Title = resultRow.TourTitle;
                result.Description = resultRow.TourDescription;
                result.Author = resultRow.AuthorName;
                result.AuthorUrl = resultRow.AuthorURL;

                //result.AuthorImageUrl = row["Title"].ToString();
                result.OrganizationUrl = resultRow.OrganizationURL;
                result.OrgName = resultRow.OrganizationName;

                // Load tour thumbnail
                string filename = result.Id + "_tourThumb.jpg";
                //move tour url

                //todo change tours when web service moves!

                string url = String.Format("http://www.worldwidetelescope.org/wwtweb/GetTourThumbnail.aspx?GUID={0}", result.Id);

                result.ThumbNail = UiTools.LoadThumbnailFromWeb(url, filename);

                filename = result.Id + "_AuthorThumb.jpg";
                url = String.Format("http://www.worldwidetelescope.org/wwtweb/GetAuthorThumbnail.aspx?GUID={0}", result.Id);

                result.AuthorImage = UiTools.LoadThumbnailFromWeb(url, filename);


                resultsList.Add(result);

            }
            Cursor.Current = Cursors.Default;

        }

        private void Discover_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void resultsList_ItemClicked(object sender, Object e)
        {
            // Todo Popup the tour preview dialog
        }

        private void resultsList_ItemDoubleClicked(object sender, Object e)
        {
            ITourResult result = (ITourResult)e;
            if (e == null)
            {
                return;
            }
            LaunchTour(result);
        }

        private static void LaunchTour(ITourResult result)
        {
            string url = String.Format("http://www.worldwidetelescope.org/wwtweb/GetTour.aspx?GUID={0}", result.Id);

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "tourcache\\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "tourcache\\");
            }

            string tempFile = Properties.Settings.Default.CahceDirectory + "tourcache\\" + result.Id.ToString() + ".wtt";

            if (FileDownload.DownloadFile(url, tempFile, false))
            {
                FileInfo fi = new FileInfo(tempFile);
                if (fi.Length == 0)
                {
                    File.Delete(tempFile);
                    MessageBox.Show("The tour file could not be downloaded and is not in cache.Check you network connection.", "WorldWide Telescope Tours");
                    return;
                }
                Earth3d.MainWindow.LoadTourFromFile(tempFile, false, result.Id);
            }
        }

        TourPopup popup = null;
        private void resultsList_ItemHover(object sender, object e)
        {
            if (popup != null)
            {
                if (e != null || !popup.Locked)
                {
                    if (!popup.Bounds.Contains(Cursor.Position))
                    {
                        popup.Close();
                        popup.Dispose();
                        popup = null;
                    }
                }
            }

            //if (e != null && e.GetType() == typeof(TourResult))
            if (e != null && ((IThumbnail)e).IsTour)
            {
                popup = new TourPopup();
                popup.Owner = Earth3d.MainWindow;
                popup.TourResult = (ITourResult)e;
                popup.Left = popup.TourResult.Bounds.Left;
                popup.Top = popup.TourResult.Bounds.Bottom-10;
                popup.LaunchTour += new EventHandler(popup_LaunchTour);
                popup.Show();
            }
        }

        void popup_LaunchTour(object sender, EventArgs e)
        {
            ITourResult result = popup.TourResult;
            popup.Close();
            popup.Dispose();
            popup = null;       
            LaunchTour(result);

        }

        private void LoadTimer_Tick(object sender, EventArgs e)
        {
            LoadTours();
            LoadTimer.Enabled = false;
        }

        private void ToursTab_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                LoadTours();
            }
        }

        internal void PlayNext()
        {
            resultsList.ShowNext(false, true);
        }
    }
}