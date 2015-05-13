using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Xml;

namespace TerraViewer
{
    public partial class Explore : TabForm
    {
        //public enum BrowseType { Constellation, Hubble, Messier, 
        public Explore()
        {
            InitializeComponent();
            SetUiStrings();
            SetStyle(ControlStyles.ResizeRedraw, true);
            BrowseList.Paginator = paginator;
        }
        bool topLevel = true;
        private void Explore_Load(object sender, EventArgs e)
        {
            //foreach(KeyValuePair<string,Place> kv in Constellations.ConstellationCentroids)
            //{
            //    BrowseList.Add(kv.Value);
            //}
            LoadTopLevel();
        }
        protected override void SetFocusedChild()
        {
            BrowseList.Focus();
        }
        private void LoadTopLevel()
        {
            BrowseList.Clear();
            exploreText.Text = "Collections";
            topLevel = true;
            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"thumbnails\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"thumbnails\");
            }

            AddCollectionFolders();

            ArrayList dataSets = DataSetManager.GetDataSets();
            foreach (DataSet d in dataSets)
            {
                // Todo Change this to exploere earth, moon etc.
                if (d.Sky == true)
                {
                    if (d != null)
                    {
                        ArrayList placesList = d.GetPlaces();
                        foreach (Places places in placesList)
                        {
                            if (places != null && places.Browseable)
                            {
                                ThumbMenuNode node = new ThumbMenuNode();
                                node.Name = places.Name;
                                node.Places = places;

                                //string filename = places.ThumbnailUrl.Substring(places.ThumbnailUrl.LastIndexOf("/") + 1);
                                //DataSetManager.DownloadFile(places.ThumbnailUrl, Properties.Settings.Default.CahceDirectory + @"thumbnails\" + filename, true);

                                //node.ThumbNail = UiTools.LoadBitmap(Properties.Settings.Default.CahceDirectory + @"thumbnails\" + filename);
                                node.ThumbNail = UiTools.LoadThumbnailFromWeb(places.ThumbnailUrl);
                                BrowseList.Add(node);
                            }
                        }
                    }
                }
            }
            Refresh();
        }

        Folder myCollections = null;

        public Folder MyCollections
        {
            get
            {
                if (myCollections == null)
                {
                    myCollections = new Folder();
                    myCollections.Name = "My Collections";
                    myCollections.ThumbNail = Properties.Resources.Folder;
                }
                return myCollections;
            }
        
            set { myCollections = value; }
        }

        private void AddCollectionFolders()
        {
            BrowseList.Add(MyCollections);
        }
 

        private void ConstellationList_ItemClicked(object sender, Object e)
        {
            if (topLevel)
            {
                //Todo drill into next level

   //             KMLTools.UnzipKMZFile(@"C:\WorldWideTelescope\Data\KML Samples\best_hubble_n.zip", @"C:\temp");

                LoadPlaces(((ThumbMenuNode)e).Places);
                topLevel = false;
            }
            else
            {
                IPlace p = (IPlace)e;
                if (p.StudyImageset != null)
                {
                    if (p.StudyImageset.Projection != ProjectionType.Tangent)
                    {
                        Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
                        return;
                    }
                }
                Earth3d.MainWindow.GotoTarget((IPlace)e, false, false, true);
            }

        }

        private void ConstellationList_ItemDoubleClicked(object sender, Object e)
        {
            if (topLevel)
            {
                LoadPlaces(((ThumbMenuNode)e).Places);
                topLevel = false;
            }
            else
            {
                if (e is IPlace)
                {
                    IPlace p = (IPlace)e;
                    if (p.StudyImageset != null)
                    {
                        if (p.StudyImageset.Projection != ProjectionType.Tangent)
                        {
                            Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
                            return;
                        }
                    }
                    Earth3d.MainWindow.GotoTarget((IPlace)e, false, true, true);
                    FlipPinupState(true);

                }
            }
        }

        private void BrowseList_ItemImageClicked(object sender, object e)
        {
            if (e is IPlace)
            {
                IPlace p = (IPlace)e;

                Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
            }
        }

        private void ConstellationList_ItemHover(object sender, Object e)
        {
            if (Earth3d.MainWindow.IsWindowOrChildFocused())
            {
                this.Focus();
            }
            if (e != null && !topLevel)
            {
                IPlace p = (IPlace)e;
                Earth3d.MainWindow.SetLabelText(p.Name, p.RA, p.Dec);
                toolTips.SetToolTip(BrowseList, p.Name);

            }
            else
            {
                if (e != null)
                {
                    toolTips.SetToolTip(BrowseList, ((IThumbnail)e).Name);
                }
                Earth3d.MainWindow.SetLabelText("", 0, 0);

            }
        }

        private void LoadPlaces(Places places)
        {
            BrowseList.Clear();

            exploreText.Text = "Collections > " + places.Name;
            List<object> items = new List<object>();

            foreach (IPlace place in places.GetPlaceList())
	        {
                items.Add(place);
            }
            BrowseList.AddRange(items);
            this.Refresh();
        }

        private void LoadPlaces(IPlace[] places)
        {
            BrowseList.Clear();

            exploreText.Text = "Collections > " + "KML Collection";
            List<object> items = new List<object>();

            foreach (IPlace place in places)
            {
                items.Add(place);
            }
            BrowseList.AddRange(items);
            this.Refresh();
        }

        public void LoadKml(string filename)
        {
            topLevel = false;
            LoadPlaces(KMLTools.GetPlacesFromKML(filename, true));

        }

        private void exploreText_Click(object sender, EventArgs e)
        {
            if (!topLevel)
            {
                LoadTopLevel();
            }
        }

        private void exploreText_MouseEnter(object sender, EventArgs e)
        {
            exploreText.ForeColor = Color.Yellow;
        }

        private void exploreText_MouseLeave(object sender, EventArgs e)
        {
            exploreText.ForeColor = Color.White;

        }

        private void BrowseList_ItemContextMenu(object sender, object e)
        {
            Point pntClick = Cursor.Position;
            if (e is IPlace)
            {
                Earth3d.MainWindow.ShowContextMenu((IPlace)e, Earth3d.MainWindow.PointToClient(Cursor.Position), false, true);
            }
        }


    }
}