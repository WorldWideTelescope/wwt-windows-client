using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS_UWP
using SysColor = TerraViewer.Color;
#else
using SysColor = System.Drawing.Color;
using System.Windows.Forms;
#endif

namespace TerraViewer
{
    class FolderPanel : RingMenuPanel
    {

        Stack<IThumbnail> breadcrumbs = new Stack<IThumbnail>();
        public FolderPanel()
        {

            SetUiStrings();

            BrowseList = new ThumbnailPanel();

            BrowseList.AddText = "Add New Item";
            BrowseList.ColCount = 4;
            BrowseList.DontStealFocus = false;
            BrowseList.EmptyAddText = "No Results";
            BrowseList.RowCount = 1;
            BrowseList.ShowAddButton = false;
            BrowseList.ThumbnailSize = TerraViewer.ThumbnailSize.Small;
            BrowseList.ItemHover += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemHover);
            BrowseList.ItemClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemClicked);
            BrowseList.ItemDoubleClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemDoubleClicked);
            BrowseList.ItemImageClicked += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemImageClicked);
            //BrowseList.ItemContextMenu += new TerraViewer.ItemClickedEventHandler(this.BrowseList_ItemContextMenu);
            //BrowseList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BrowseList_KeyDown);

        }

        public override void Navigate(int upDown, int leftRight)
        {
            BrowseList.Navigate(upDown, leftRight);
        }

        public override void Select()
        {
            this.SelectItem();
        }

        public override void MouseClick(object ringMenu, MouseEventArgs mouseEventArgs)
        {
            BrowseList.MouseClick(ringMenu, mouseEventArgs);
        }

        public override void MouseMove(object ringMenu, MouseEventArgs mouseEventArgs)
        {
            BrowseList.MouseMove(ringMenu, mouseEventArgs);
        }

        public override void Draw(UiGraphics g)
        {
            //todo draw other stuff here
            BrowseList.Draw(g);
        }
        private void SetUiStrings()
        {
            //this.exploreText.Text = Language.GetLocalizedText(219, "Select a Collection to explore...");
            //this.BrowseList.AddText = Language.GetLocalizedText(161, "Add New Item");
            //this.BrowseList.EmptyAddText = Language.GetLocalizedText(162, "No Results");
        }

        bool showMyFolders = false;

        public bool ShowMyFolders
        {
            get { return showMyFolders; }
            set { showMyFolders = value; }
        }

        public static Folder Tours = null;

        public static Dictionary<string, Tour> deduplicatedTourList = null;
        public static void AddToursToSearch()
        {
            deduplicatedTourList = new Dictionary<string, Tour>();
            AddFolderToursToSearch(Tours);
            foreach (Tour tour in deduplicatedTourList.Values)
            {
                if (!string.IsNullOrEmpty(tour.Keywords))
                {
                    string[] keywords = tour.Keywords.Split(new char[] { ';', ':' });
                    foreach (string id in keywords)
                    {
                        IPlace place = Catalogs.FindCatalogObject(id);
                        if (place != null)
                        {
                            Place tourPlace = Place.FromIPlace(place);
                            tourPlace.ThumbNail = tour.ThumbNail;
                            tourPlace.Names = new string[] { tour.Name };
                            tourPlace.Classification = Classification.Unidentified;
                            tourPlace.Tour = tour;
                            ContextSearch.AddPlaceToContextSearch(tourPlace);
                            Catalogs.AddParts(place.Name, tourPlace);
                        }
                    }
                }
            }

        }

        public void Arrived()
        {
            IThumbnail temp = breadcrumbs.Peek();
            if (temp is Folder)
            {
                Folder current = (Folder)temp;
                if (current.RefreshType == FolderRefreshType.ViewChange && string.IsNullOrEmpty(current.Url))
                {
                    current.Refresh();
                    breadcrumbs.Pop();
                    LoadFolder(current);
                }
            }

        }
        public static void AddFolderToursToSearch(Folder folder)
        {
            if (folder.Tour != null)
            {
                foreach (Tour tour in folder.Tour)
                {
                    if (!deduplicatedTourList.ContainsKey(tour.ID))
                    {
                        deduplicatedTourList.Add(tour.ID, tour);
                    }
                }
            }
            if (Tours.Folder1 != null)
            {
                foreach (Folder childFolder in folder.Folder1)
                {
                    AddFolderToursToSearch(childFolder);
                }
            }
        }

        public static ITourResult lastTourLaunched = null;

        public static Tour GetRelatedTour(string guid)
        {
            if ((lastTourLaunched as Tour) != null)
            {
                if (lastTourLaunched.Id == guid)
                {
                    return lastTourLaunched as Tour;
                }
            }

            if (Tours == null)
            {
                return null;
            }
            return FindChildTour(Tours, guid);
        }

        private static Tour FindChildTour(Folder folder, string guid)
        {
            if (folder.Tour != null)
            {
                foreach (Tour tour in folder.Tour)
                {
                    if (tour.Id == guid.ToLower().Trim())
                    {
                        return tour;
                    }
                }
            }
            if (Tours.Folder1 != null)
            {
                foreach (Folder childFolder in folder.Folder1)
                {
                    Tour tour = FindChildTour(childFolder, guid);
                    if (tour != null)
                    {
                        return tour;
                    }
                }
            }
            return null;
        }

        delegate void BackgroundTourLoad();

        BackgroundTourLoad backgroundLoad;

        public void LoadTours()
        {
            BrowseList.Clear();
            BrowseList.Invalidate();
            this.BrowseList.EmptyAddText = Language.GetLocalizedText(1278, "Loading...");
            LoadToursDeffered();
        }


        public void LoadToursDeffered()
        {
            Cursor.Current = Cursors.WaitCursor;

            string filename = Properties.Settings.Default.CahceDirectory + @"data\tours.wtml";

            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/gettours.aspx", filename, false, false);

            Tours = Folder.LoadFromFile(filename, false);
            Tours.Name = Language.GetLocalizedText(492, "Tours");
            Folder loadFolder = null;

            loadFolder = Tours;

            LoadRootFoder(loadFolder);
            this.BrowseList.EmptyAddText = Language.GetLocalizedText(162, "No Results");

            AddToursToSearch();

            Cursor.Current = Cursors.Default;
        }

        public static void LaunchTour(ITourResult result)
        {
            //string url = result.TourUrl;

            //if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "tourcache\\"))
            //{
            //    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "tourcache\\");
            //}

            //string tempFile = Properties.Settings.Default.CahceDirectory + "tourcache\\" + result.Id.ToString() + ".wtt";

            //if (FileDownload.DownloadFile(url, tempFile, false))
            //{
            //    FileInfo fi = new FileInfo(tempFile);
            //    if (fi.Length == 0)
            //    {
            //        File.Delete(tempFile);
            //        MessageBox.Show(Language.GetLocalizedText(221, "The tour file could not be downloaded and is not in cache. Check you network connection."), "WorldWide Telescope Tours");
            //        return;
            //    }
            //    Earth3d.MainWindow.LoadTourFromFile(tempFile, false, result.Id);
            //}
            //lastTourLaunched = result;
        }

        public static void LoadLayer(string url)
        {

            //if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "tourcache\\"))
            //{
            //    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "tourcache\\");
            //}

            //string tempFile = Properties.Settings.Default.CahceDirectory + "tourcache\\" + ((uint)url.GetHashCode32()).ToString() + ".wwtl";

            //if (FileDownload.DownloadFile(url, tempFile, true))
            //{
            //    FileInfo fi = new FileInfo(tempFile);
            //    if (fi.Length == 0)
            //    {
            //        File.Delete(tempFile);
            //        MessageBox.Show(Language.GetLocalizedText(221, "The layer file could not be downloaded and is not in cache. Check you network connection."), "Microsoft WorldWide Telescope");
            //        return;
            //    }
            //    LayerManager.LoadLayerFile(tempFile, "Sun", false);

            //}

        }


        //TourPopup popup = null;
        private void TourItemHover(object sender, object e)
        {
            //if (popup != null)
            //{
            //    if (e != null || !popup.Locked)
            //    {
            //        if (!popup.Bounds.Contains(Cursor.Position))
            //        {
            //            popup.Close();
            //            popup.Dispose();
            //            popup = null;
            //        }
            //    }
            //}

            ////if (e != null && e.GetType() == typeof(TourResult))
            //if (e != null && ((IThumbnail)e).IsTour)
            //{
            //    popup = new TourPopup();
            //    popup.Owner = Earth3d.MainWindow;
            //    popup.TourResult = (ITourResult)e;
            //    popup.Left = popup.TourResult.Bounds.Left;
            //    popup.Top = popup.TourResult.Bounds.Bottom - 10;
            //    popup.LaunchTour += new EventHandler(popup_LaunchTour);
            //    popup.Show();
            //}
        }

        private void TourItemDoubleClicked(object sender, Object e)
        {
            ITourResult result = (ITourResult)e;
            if (e == null)
            {
                return;
            }
            LaunchTour(result);
        }
        void popup_LaunchTour(object sender, EventArgs e)
        {
            //if (popup == null)
            //{
            //    return;
            //}
            //ITourResult result = popup.TourResult;
            //popup.Close();
            //popup.Dispose();
            //popup = null;
            //LaunchTour(result);

        }



        //private void BrowseList_ItemContextMenu(object sender, object e)
        //{
        //    IThumbnail thumb = breadcrumbs.Peek();
        //    bool readOnly = true;

        //    if (thumb is Folder)
        //    {
        //        Folder owner = (Folder)thumb;
        //        readOnly = owner.ReadOnly;
        //    }

        //    thumb = e as IThumbnail;

        //    if (thumb.IsCloudCommunityItem)
        //    {
        //        if (e is Folder)
        //        {
        //            ShowCloudCommunitiesFolderContextMenu(e as Folder);
        //        }
        //        else
        //        {
        //            ShowCloudCommunitiesItemContextMenu(e as IThumbnail);
        //        }

        //        return;
        //    }


        //    Point pntClick = Cursor.Position;
        //    IThumbnail[] ta = breadcrumbs.ToArray();
        //    if (e is IImageSet)
        //    {
        //        IImageSet imageSet = (IImageSet)e;
        //        TourPlace tp = new TourPlace(imageSet.Name, imageSet.CenterX, imageSet.CenterY, Classification.Unidentified, "", imageSet.DataSetType, 360);
        //        tp.StudyImageset = imageSet;
        //        Earth3d.MainWindow.ShowContextMenu(tp, Earth3d.MainWindow.PointToClient(Cursor.Position), true, readOnly);

        //    }
        //    else if (e is IPlace)
        //    {
        //        if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(222, "Open Images"))
        //        {
        //            readOnly = false;
        //        }
        //        //if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(222, "Open Images"))
        //        //{
        //        //    ShowOpenImageMenu((Place)e);
        //        //}
        //        //else
        //        {
        //            Earth3d.MainWindow.ShowContextMenu((IPlace)e, Earth3d.MainWindow.PointToClient(Cursor.Position), true, readOnly);
        //        }
        //    }
        //    else if (e is Folder)
        //    {
        //        if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(223, "Open Collections"))
        //        {
        //            ShowOpenFolderMenu((Folder)e);
        //        }
        //        else
        //        {
        //            ShowFolderMenu((Folder)e);
        //        }
        //    }
        //    if (e is Tour)
        //    {
        //        // TODO (Diego): fix
        //        Tour p = (Tour)e;
        //        if (p.IsTour)
        //            ShowTourMenu(p);
        //    }

        //}
        //ContextMenuStrip contextMenu = null;

        //private void ShowFolderMenu(Folder folder)
        //{


        //    if (contextMenu != null)
        //    {
        //        contextMenu.Dispose();
        //    }

        //    if (folder.ReadOnly)
        //    {
        //        ShowReadOnlyCollectionContextMenu(folder);
        //    }
        //    else
        //    {
        //        ShowMyCollectionsContextMenu(folder);
        //    }
        //}

        //private void ShowCloudCommunitiesFolderContextMenu(Folder folder)
        //{
        //    contextMenu = new ContextMenuStrip();

        //    ToolStripMenuItem showFolder = new ToolStripMenuItem(Language.GetLocalizedText(995, "Show on Community Web Site"));
        //    ToolStripMenuItem editFolder = new ToolStripMenuItem(Language.GetLocalizedText(996, "Edit on Community Web Site"));
        //    ToolStripMenuItem deleteFolder = new ToolStripMenuItem(Language.GetLocalizedText(997, "Delete from Community Web Site"));
        //    ToolStripMenuItem removeFolder = new ToolStripMenuItem(Language.GetLocalizedText(998, "Remove Subscription of this Community"));

        //    showFolder.Click += new EventHandler(showFolder_Click);
        //    editFolder.Click += new EventHandler(editFolder_Click);
        //    deleteFolder.Click += new EventHandler(deleteFolder_Click);
        //    removeFolder.Click += new EventHandler(removeFolder_Click);
        //    showFolder.Tag = folder;
        //    editFolder.Tag = folder;
        //    deleteFolder.Tag = folder;
        //    removeFolder.Tag = folder;

        //    if (folder.MSRCommunityId > 0)
        //    {
        //        contextMenu.Items.Add(showFolder);
        //    }

        //    if ((folder.Permission & 8) == 8)
        //    {
        //        contextMenu.Items.Add(editFolder);
        //        contextMenu.Items.Add(deleteFolder);
        //    }

        //    if (breadcrumbs.Count == 1 && !folder.ReadOnly)
        //    {
        //        contextMenu.Items.Add(removeFolder);
        //    }

        //    if (contextMenu.Items.Count > 0)
        //    {
        //        contextMenu.Show(Cursor.Position);
        //    }
        //}

        //void showFolder_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;

        //    if (folder.MSRCommunityId > 0)
        //    {
        //        WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/CommunityDetail/" + folder.MSRCommunityId.ToString(), true);
        //    }
        //}

        //void editFolder_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;

        //    if (folder.MSRCommunityId > 0)
        //    {
        //        WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditCommunity/" + folder.MSRCommunityId.ToString(), true);
        //    }
        //}

        //void deleteFolder_Click(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem item = (ToolStripMenuItem)sender;
        //    int id = 0;

        //    Folder folder = item.Tag as Folder;

        //    if (folder != null)
        //    {
        //        id = (int)folder.MSRCommunityId;
        //    }


        //    if (id > 0)
        //    {
        //        if (UiTools.ShowMessageBox(Language.GetLocalizedText(999, "Do you really want to permanently delete this item from your community?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        //        {
        //            EOCalls.InvokeDeleteCommunity(id);
        //            Earth3d.RefreshCommunity();
        //            return;
        //        }
        //    }
        //}

        //void removeFolder_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;
        //    if (UiTools.ShowMessageBox(Language.GetLocalizedText(1000, "Do you really want to unsubscribe this community link on this computer?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        object target = BrowseList.Selected;

        //        IThumbnail thumb = breadcrumbs.Peek();
        //        if (thumb is Folder)
        //        {
        //            Folder owner = (Folder)thumb;
        //            owner.RemoveChild(folder);
        //            BrowseList.RemoveSelected();
        //            if (File.Exists(folder.LoadedFilename))
        //            {
        //                File.Delete(folder.LoadedFilename);
        //            }
        //        }
        //        BrowseList.Refresh();
        //    }
        //}



        //private void ShowCloudCommunitiesItemContextMenu(IThumbnail item)
        //{
        //    contextMenu = new ContextMenuStrip();

        //    ToolStripMenuItem showItem = new ToolStripMenuItem(Language.GetLocalizedText(995, "Show on Community Web Site"));
        //    ToolStripMenuItem editItem = new ToolStripMenuItem(Language.GetLocalizedText(996, "Edit on Community Web Site"));
        //    ToolStripMenuItem deleteItem = new ToolStripMenuItem(Language.GetLocalizedText(997, "Delete from Community Web Site"));

        //    showItem.Click += new EventHandler(showItem_Click);
        //    editItem.Click += new EventHandler(editItem_Click);
        //    deleteItem.Click += new EventHandler(deleteItem_Click);
        //    showItem.Tag = item;
        //    editItem.Tag = item;
        //    deleteItem.Tag = item;

        //    bool edit = false;

        //    if (item is Tour)
        //    {
        //        edit = ((((Tour)item).Permission & 8) == 8);
        //    }

        //    if (item is Place)
        //    {
        //        edit = ((((Place)item).Permission & 8) == 8);
        //    }

        //    if (item is ImageSet)
        //    {
        //        edit = ((((ImageSet)item).Permission & 8) == 8);
        //    }

        //    contextMenu.Items.Add(showItem);

        //    if (edit)
        //    {
        //        contextMenu.Items.Add(editItem);
        //        contextMenu.Items.Add(deleteItem);
        //    }
        //    contextMenu.Show(Cursor.Position);
        //}


        //void showItem_Click(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem item = (ToolStripMenuItem)sender;
        //    int id = 0;

        //    Place place = item.Tag as Place;
        //    Tour tour = item.Tag as Tour;
        //    if (place != null)
        //    {
        //        id = (int)place.MSRComponentId;
        //    }

        //    if (tour != null)
        //    {
        //        id = (int)tour.MSRComponentId;
        //    }

        //    if (id > 0)
        //    {
        //        WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/ContentDetail/" + id, true);
        //    }

        //}

        //void editItem_Click(object sender, EventArgs e)
        //{

        //    ToolStripMenuItem item = (ToolStripMenuItem)sender;
        //    int id = 0;

        //    Place place = item.Tag as Place;
        //    Tour tour = item.Tag as Tour;
        //    if (place != null)
        //    {
        //        id = (int)place.MSRComponentId;
        //    }
        //    if (tour != null)
        //    {
        //        id = (int)tour.MSRComponentId;
        //    }

        //    if (id > 0)
        //    {
        //        WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditContent/" + id, true);
        //    }
        //}

        //void deleteItem_Click(object sender, EventArgs e)
        //{
        //    ToolStripMenuItem item = (ToolStripMenuItem)sender;
        //    int id = 0;

        //    Place place = item.Tag as Place;
        //    Tour tour = item.Tag as Tour;
        //    if (place != null)
        //    {
        //        id = (int)place.MSRComponentId;
        //    }

        //    if (tour != null)
        //    {
        //        id = (int)tour.MSRComponentId;
        //    }

        //    if (id > 0)
        //    {
        //        if (UiTools.ShowMessageBox(Language.GetLocalizedText(999, "Do you really want to permanently delete this item from your community?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
        //        {
        //            EOCalls.InvokeDeleteContent(id);
        //            Earth3d.RefreshCommunity();
        //            return;
        //        }
        //    }
        //}


        //private void ShowMyCollectionsContextMenu(Folder folder)
        //{
        //    if (folder == myCollections)
        //    {
        //        return;
        //    }


        //    contextMenu = new ContextMenuStrip();

        //    ToolStripMenuItem addToStack = new ToolStripMenuItem(Language.GetLocalizedText(1001, "Add Children to Image stack"));
        //    ToolStripMenuItem deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(224, Language.GetLocalizedText(224, "Delete Folder")));


        //    ToolStripMenuItem renameMenu = new ToolStripMenuItem(Language.GetLocalizedText(225, "Rename"));
        //    addToStack.Click += new EventHandler(addToStack_Click);
        //    deleteMenu.Click += new EventHandler(deleteMenu_Click);

        //    renameMenu.Click += new EventHandler(renameMenu_Click);
        //    addToStack.Tag = folder;
        //    deleteMenu.Tag = folder;
        //    renameMenu.Tag = folder;


        //    if (Earth3d.MainWindow.ImageStackVisible)
        //    {
        //        contextMenu.Items.Add(addToStack);
        //    }
        //    contextMenu.Items.Add(deleteMenu);
        //    //if (folder.MSRCommunityId > 0)
        //    //{
        //    //    contextMenu.Items.Add(editMenu);
        //    //}
        //    //else
        //    {
        //        contextMenu.Items.Add(renameMenu);
        //    }
        //    contextMenu.Show(Cursor.Position);
        //}

        //void editMenu_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;

        //    if (folder.MSRCommunityId > 0)
        //    {
        //        WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditCommunity/" + folder.MSRCommunityId, true);
        //    }

        //}

        //private void ShowReadOnlyCollectionContextMenu(Folder folder)
        //{
        //    contextMenu = new ContextMenuStrip();
        //    if (Earth3d.MainWindow.ImageStackVisible)
        //    {
        //        ToolStripMenuItem addToStack = new ToolStripMenuItem(Language.GetLocalizedText(1001, "Add Children to Image stack"));
        //        addToStack.Click += new EventHandler(addToStack_Click);
        //        addToStack.Tag = folder;
        //        contextMenu.Items.Add(addToStack);
        //    }


        //}

        private void ShowTourMenu(Tour tour)
        {

        }

        //private void removeCommunityTour_Click(object sender, EventArgs e)
        //{
        //    BrowseList.RemoveSelected();
        //    BrowseList.Refresh();
        //}

        //void addToStack_Click(object sender, EventArgs e)
        //{
        //    Earth3d.MainWindow.AddClidrenToStack((Folder)((ToolStripMenuItem)sender).Tag, false);
        //    Earth3d.MainWindow.Stack.UpdateList();
        //}


        //private void ShowOpenFolderMenu(Folder folder)
        //{

        //    if (contextMenu != null)
        //    {
        //        contextMenu.Dispose();
        //    }

        //    contextMenu = new ContextMenuStrip();

        //    ToolStripMenuItem closeMenu = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
        //    ToolStripMenuItem copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(226, "Copy To My Collections"));
        //    closeMenu.Click += new EventHandler(closeMenu_Click);
        //    copyMenu.Click += new EventHandler(copyMenu_Click);
        //    closeMenu.Tag = folder;
        //    copyMenu.Tag = folder;
        //    contextMenu.Items.Add(closeMenu);
        //    contextMenu.Items.Add(copyMenu);
        //    contextMenu.Show(Cursor.Position);
        //}
        //private void ShowOpenImageMenu(Place place)
        //{

        //    if (contextMenu != null)
        //    {
        //        contextMenu.Dispose();
        //    }

        //    contextMenu = new ContextMenuStrip();

        //    ToolStripMenuItem closeImageMenu = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
        //    ToolStripMenuItem popertiesMenu = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
        //    closeImageMenu.Click += new EventHandler(closeImageMenu_Click);
        //    popertiesMenu.Click += new EventHandler(popertiesMenu_Click);
        //    closeImageMenu.Tag = place;
        //    popertiesMenu.Tag = place;
        //    contextMenu.Items.Add(popertiesMenu);
        //    contextMenu.Items.Add(closeImageMenu);
        //    contextMenu.Show(Cursor.Position);
        //}

        //void closeImageMenu_Click(object sender, EventArgs e)
        //{
        //    Place place = (Place)((ToolStripMenuItem)sender).Tag;

        //    IThumbnail thumb = breadcrumbs.Peek();
        //    if (thumb is Folder)
        //    {
        //        Folder owner = (Folder)thumb;
        //        owner.RemoveChild(place);
        //        BrowseList.RemoveSelected();

        //        if (Earth3d.MainWindow.StudyImageset.ImageSetID == place.StudyImageset.ImageSetID)
        //        {
        //            Earth3d.MainWindow.StudyImageset = null;
        //        }
        //        if (BrowseList.Count == 0)
        //        {
        //            MoveUpOneLevel();
        //        }
        //    }
        //    BrowseList.Refresh();
        //}

        //void popertiesMenu_Click(object sender, EventArgs e)
        //{
        //    IPlace place = (IPlace)((ToolStripMenuItem)sender).Tag;

        //    ObjectProperties.ShowNofinder(place, Earth3d.MainWindow.RenderWindow.PointToScreen(Cursor.Position));

        //}
        //void copyMenu_Click(object sender, EventArgs e)
        //{
        //    IThumbnail thumb = breadcrumbs.Peek();
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;
        //    SaveFolderAs(folder);
        //}

        //bool SaveFolderAs(Folder folder)
        //{
        //    SimpleInput input = new SimpleInput(Language.GetLocalizedText(227, "Save Folder As"), Language.GetLocalizedText(228, "New Name"), folder.Name, 32);
        //    bool retry = false;
        //    do
        //    {
        //        if (input.ShowDialog() == DialogResult.OK)
        //        {
        //            if (!File.Exists(CollectionFileName(input.ResultText)))
        //            {
        //                Folder f = FindCollection(input.ResultText);
        //                if (f != null)
        //                {
        //                    MessageBox.Show(Language.GetLocalizedText(229, "Collection file name already exists, type a different name."), Language.GetLocalizedText(230, Language.GetLocalizedText(232, "Create New Collection")));
        //                    retry = true;
        //                }
        //                else
        //                {
        //                    string validfileName = @"^[A-Za-z0-9_ ]*$";
        //                    if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
        //                    {
        //                        MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
        //                        retry = true;
        //                    }
        //                    else
        //                    {
        //                        string oldName = folder.Name;
        //                        folder.Name = input.ResultText;
        //                        folder.SaveToFile(CollectionFileName(folder.Name));
        //                        myCollections.AddChildFolder(Folder.LoadFromFile(CollectionFileName(folder.Name), false));
        //                        folder.Name = oldName;
        //                        retry = false;
        //                        return true;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show(Language.GetLocalizedText(233, "Name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
        //                retry = true;
        //            }
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    } while (retry);

        //    return false;
        //}
        //void closeMenu_Click(object sender, EventArgs e)
        //{
        //    Folder parent = (Folder)breadcrumbs.Peek();
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;
        //    parent.RemoveChild(folder);
        //    MoveUpOneLevel();

        //}

        //void renameMenu_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;
        //    if (folder.ReadOnly)
        //    {
        //        return;
        //    }

        //    SimpleInput input = new SimpleInput(Language.GetLocalizedText(234, "Rename Collection"), Language.GetLocalizedText(228, "New Name"), folder.Name, 32);
        //    bool retry = false;
        //    do
        //    {
        //        if (input.ShowDialog() == DialogResult.OK)
        //        {
        //            if (input.ResultText.ToLower() == folder.Name.ToLower())
        //            {
        //                folder.Name = input.ResultText;
        //                folder.Dirty = true;
        //                return;
        //            }

        //            if (!File.Exists(CollectionFileName(input.ResultText)))
        //            {
        //                Folder f = FindCollection(input.ResultText);
        //                if (f != null)
        //                {
        //                    MessageBox.Show(Language.GetLocalizedText(229, "Collection file name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
        //                    retry = true;
        //                }
        //                else
        //                {
        //                    string validfileName = @"^[A-Za-z0-9_ ]*$";
        //                    if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
        //                    {
        //                        MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
        //                        retry = true;
        //                    }
        //                    else
        //                    {
        //                        folder.Name = input.ResultText;
        //                        if (File.Exists(folder.LoadedFilename))
        //                        {
        //                            File.Delete(folder.LoadedFilename);
        //                        }
        //                        folder.Dirty = true;
        //                        folder.LoadedFilename = CollectionFileName(input.ResultText);
        //                        retry = false;
        //                        return;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                MessageBox.Show(Language.GetLocalizedText(233, "Name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
        //                retry = true;
        //            }
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    } while (retry);
        //}

        //void deleteMenu_Click(object sender, EventArgs e)
        //{
        //    Folder folder = (Folder)((ToolStripMenuItem)sender).Tag;

        //    if (folder.MSRCommunityId > 0)
        //    {
        //        int id = (int)folder.MSRCommunityId;
        //        EOCalls.InvokeDeleteCommunity(id);
        //        Earth3d.RefreshCommunity();
        //    }
        //    else
        //    {

        //        object target = BrowseList.Selected;

        //        IThumbnail thumb = breadcrumbs.Peek();
        //        if (thumb is Folder)
        //        {
        //            Folder owner = (Folder)thumb;
        //            owner.RemoveChild(folder);
        //            BrowseList.RemoveSelected();
        //            if (File.Exists(folder.LoadedFilename))
        //            {
        //                File.Delete(folder.LoadedFilename);
        //            }
        //        }
        //        BrowseList.Refresh();
        //    }
        //}

        public void EditSelected()
        {
        }

        public void RemoveSelected()
        {
            object target = BrowseList.Selected;
            IThumbnail thumb = breadcrumbs.Peek();
            if (thumb is Folder)
            {
                Folder owner = (Folder)thumb;
                if (target is Place)
                {
                    owner.RemoveChild((Place)target);
                }
                //todo remove imageset?
                BrowseList.RemoveSelected();
            }
            BrowseList.Invalidate();
        }
        private void BrowseList_ItemClicked(object sender, object e)
        {
            ItemActivated(sender, e, false);
        }

        private void BrowseList_ItemDoubleClicked(object sender, object e)
        {
            ItemActivated(sender, e, true);
        }

        private void ItemActivated(object sender, object e, bool doubleClick)
        {


            IThumbnail item = (IThumbnail)e;
            if (item.IsFolder)
            {
                LoadFolder(item);
            }
            else
            {
                if (e is IPlace)
                {
                    IPlace p = (IPlace)e;
                    if (p.StudyImageset != null)
                    {
                        //if (p.StudyImageset.Projection != ProjectionType.SkyImage && p.StudyImageset.Projection != ProjectionType.Tangent)
                        if (p.RA == 0 && p.Dec == 0)
                        {
                            RenderEngine.Engine.GotoTarget(p, false, doubleClick, true);
                            //Earth3d.MainWindow.SetStudyImageset(imageSet, null);
                            RenderEngine.Engine.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
                            return;
                        }
                    }
                    if (!string.IsNullOrEmpty(p.Url) && !doubleClick)
                    {
                        if (p.Url.ToLower().Contains(".wwtl"))
                        {
                            LoadLayer(p.Url);
                        }
                        else
                        {
                           // WebWindow.OpenUrl(p.Url, false);
                        }
                    }
                    else
                    {
                        RenderEngine.Engine.GotoTarget(p, false, doubleClick, true);
                    }

                }
                else if (e is Tour)
                {
                    TourItemDoubleClicked(sender, e);
                }
                else if (e is IImageSet)
                {
                    IImageSet imageSet = (IImageSet)e;
                    if (imageSet.Projection == ProjectionType.SkyImage || imageSet.Projection == ProjectionType.Tangent)
                    {
                        RenderEngine.Engine.GotoTarget(new TourPlace("", imageSet.CenterY, imageSet.CenterX / 15, Classification.Unidentified, "UMA", ImageSetType.Sky, imageSet.BaseTileDegrees * 10), false, doubleClick, true);
                        RenderEngine.Engine.SetStudyImageset(imageSet, null);

                    }
                    else
                    {
                        RenderEngine.Engine.currentImageSetfield = imageSet;
                    }
                }
                else if (e is FolderUp)
                {
                    MoveUpOneLevel();
                }
            }
        }

     

        public void LoadRootFoder(IThumbnail collection)
        {
            breadcrumbs.Clear();
            LoadFolder(collection);
        }

        public bool IsCollectionLoaded(string filename, bool showCollection)
        {
            if (String.IsNullOrEmpty(filename))
            {
                return false;
            }
         
            return false;
        }

        public void LoadCollection(Folder collection)
        {
            while (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
            }
            IThumbnail top = breadcrumbs.Pop();
            breadcrumbs.Clear();
            LoadFolder(top);
            LoadFolder(collection);
        }

        public void ShowCollection(Folder collection)
        {
            while (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
            }
            IThumbnail top = breadcrumbs.Pop();
            breadcrumbs.Clear();
            LoadFolder(top);
            LoadFolder(collection);
        }
        private void LoadFolder(IThumbnail parent)
        {
            Cursor.Current = Cursors.WaitCursor;
            bool topLevel = breadcrumbs.Count == 0;

            bool communityNode = false;
            bool firstSearchable = true;

            breadcrumbs.Push(parent);
            BrowseList.Clear();

            if (topLevel)
            {
                BrowseList.ThumbnailSize = rootSize;
            }
            else
            {
                BrowseList.ThumbnailSize = ThumbnailSize.Small;
                BrowseList.Add(new FolderUp());
            }

            //Folder temp = new Folder();
            //temp.Name = "Constellations";
            object[] children = parent.Children;

            if (children != null)
            {
                foreach (object child in children)
                {
                    if (child is Folder)
                    {
                        Folder folderItem = (Folder)child;
                        if (folderItem.Browseable == FolderBrowseable.True)
                        {
                            BrowseList.Add(child);
                        }
                        if (communityNode && folderItem.Searchable == FolderSearchable.True)
                        {
                            ContextSearch.SetCommunitySearch(folderItem, firstSearchable);
                            firstSearchable = false;
                        }
                        if (!parent.ReadOnly)
                        {
                            folderItem.ReadOnly = false;
                        }
                    }
                    else
                    {
                        BrowseList.Add(child);
                        //IThumbnail c = (IThumbnail)child;

                        //if (!c.IsFolder)
                        //{
                        //    temp.AddChildPlace(Place.FromIPlace((IPlace)child));
                        //}
                    }
                }

            }

            //temp.SaveToFile("C:\\namedStars.wtml");


            StringBuilder sb = new StringBuilder();

            string text = "";




            foreach (IThumbnail item in breadcrumbs)
            {
                text = item.Name + "  > " + text;
            }

            BrowseList.ShowAddButton = (!parent.ReadOnly) && !(hideAddButtonInLowerLevels && !topLevel);

            Cursor.Current = Cursors.Default;
        }

        public void ReloadFolder()
        {
            IThumbnail current = breadcrumbs.Pop();
            LoadFolder(current);
        }

        private void BrowseList_ItemHover(object sender, object e)
        {
            TourItemHover(sender, e);

            if (e is IPlace || e is IImageSet)
            {
                IImageSet imageset = null;
                if (e is IPlace)
                {
                    IPlace p = (IPlace)e;
                    //todo UWP hover label
                    //Earth3d.MainWindow.SetLabelText(p, true);
                    if (p.BackgroundImageSet != null)
                    {
                        imageset = p.BackgroundImageSet;
                    }
                    else if (p.StudyImageset != null)
                    {
                        imageset = p.StudyImageset;
                    }
                }
                if (e is IImageSet)
                {
                    imageset = e as IImageSet;
                }

                //todo UWP set tooltip
                //toolTips.SetToolTip(BrowseList, ((IThumbnail)e).Name);

                if (imageset != null)
                {
                    RenderEngine.Engine.PreviewImageset = imageset;
                    RenderEngine.Engine.PreviewBlend.TargetState = true;
                }
                else
                {
                    RenderEngine.Engine.PreviewBlend.TargetState = false;
                }
            }
            else
            {
                //if (e != null)
                //{
                //    toolTips.SetToolTip(BrowseList, ((IThumbnail)e).Name);
                //}
                //Earth3d.MainWindow.SetLabelText(null, false);
                RenderEngine.Engine.PreviewBlend.TargetState = false;

            }
        }

        private void BrowseList_ItemImageClicked(object sender, object e)
        {
            if (e is IPlace)
            {
                IPlace p = (IPlace)e;

                RenderEngine.Engine.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
            }

        }


        public bool MoveNext()
        {
            return BrowseList.MoveNext();
        }

        public bool MovePrevious()
        {
            return BrowseList.MovePrevious();
        }

        public void SelectItem()
        {
            BrowseList.SelectItem();
        }

        public void Back()
        {
            MoveUpOneLevel();
        }

        public void SelectForeground()
        {
            object item = BrowseList.Selected;

            if (item != null)
            {
                if (item is IPlace)
                {
                    IPlace p = (IPlace)item;

                    if (p.StudyImageset != null)
                    {
                        RenderEngine.Engine.studyImageset = p.StudyImageset;
                    }
                    else if (p.BackgroundImageSet != null)
                    {
                        RenderEngine.Engine.studyImageset = p.BackgroundImageSet;
                    }

                    return;
                }
                else if (item is IImageSet)
                {
                    IImageSet imageSet = (IImageSet)item;
                    RenderEngine.Engine.studyImageset = imageSet;
                }
            }
        }

        public void SelectBackground()
        {

            object item = BrowseList.Selected;

            if (item != null)
            {
                if (item is IPlace)
                {
                    IPlace p = (IPlace)item;

                    if (p.StudyImageset != null)
                    {
                        RenderEngine.Engine.currentImageSetfield = p.StudyImageset;
                    }
                    else if (p.BackgroundImageSet != null)
                    {
                        RenderEngine.Engine.currentImageSetfield = p.BackgroundImageSet;
                    }

                    return;
                }
                else if (item is IImageSet)
                {
                    IImageSet imageSet = (IImageSet)item;
                    RenderEngine.Engine.currentImageSetfield = imageSet;
                }
            }

        }


        private void exploreText_Click(object sender, EventArgs e)
        {
            MoveUpOneLevel();
        }

        public void MoveUpOneLevel()
        {
            if (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
                IThumbnail parent = breadcrumbs.Pop();
                LoadFolder(parent);
            }
        }

        public static bool AllDirty = false;


        bool communities = false;
        ThumbnailSize rootSize = ThumbnailSize.Small;
        bool hideAddButtonInLowerLevels = false;


        internal void ShowCurrent()
        {
            BrowseList.ShowCurrent();
        }
        public ThumbnailPanel BrowseList;
       // private Paginator paginator;

    }
}