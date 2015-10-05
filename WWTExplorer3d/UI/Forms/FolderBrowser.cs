using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using TerraViewer.Properties;

namespace TerraViewer
{
    public partial class FolderBrowser : TabForm
    {
        readonly Stack<IThumbnail> breadcrumbs = new Stack<IThumbnail>();
        public FolderBrowser()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            exploreText.Text = Language.GetLocalizedText(219, "Select a Collection to explore...");
            BrowseList.AddText = Language.GetLocalizedText(161, "Add New Item");
            BrowseList.EmptyAddText = Language.GetLocalizedText(162, "No Results");
        }

        bool showMyFolders;

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
            foreach (var tour in deduplicatedTourList.Values)
            {
                if (!string.IsNullOrEmpty(tour.Keywords))
                {
                    var keywords = tour.Keywords.Split(new[] { ';', ':'});
                    foreach (var id in keywords)
                    {
                        var place = Search.FindCatalogObject(id);
                        if (place != null)
                        {
                            var tourPlace = Place.FromIPlace(place);
                            tourPlace.ThumbNail = tour.ThumbNail;
                            tourPlace.Names = new string[] { tour.Name };
                            tourPlace.Classification = Classification.Unidentified;
                            tourPlace.Tour = tour;
                            ContextSearch.AddPlaceToContextSearch(tourPlace);
                            Search.AddParts(place.Name, tourPlace);
                        }
                    }
                }
            }
            
        }

        public void Arrived()
        {
            var temp = breadcrumbs.Peek();
            if (temp is Folder)
            {
                var current = (Folder)temp;
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
                foreach (var tour in folder.Tour)
                {
                    if (!deduplicatedTourList.ContainsKey(tour.ID))
                    {
                        deduplicatedTourList.Add(tour.ID, tour);
                    }
                }
            }
            if (Tours.Folder1 != null)
            {
                foreach (var childFolder in folder.Folder1)
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
                foreach (var tour in folder.Tour)
                {
                    if (tour.Id == guid.ToLower().Trim())
                    {
                        return tour;
                    }
                }
            }
            if (Tours.Folder1 != null)
            {
                foreach (var childFolder in folder.Folder1)
                {
                    var tour = FindChildTour(childFolder, guid);
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
            BrowseList.Refresh();
            BrowseList.EmptyAddText = Language.GetLocalizedText(1278, "Loading...");
            backgroundLoad = LoadToursDeffered;

            backgroundLoad.BeginInvoke(null, null);

        }


        public void LoadToursDeffered()
        {
            Cursor.Current = Cursors.WaitCursor;
            
          
            try
            {
                var filename = Properties.Settings.Default.CahceDirectory + @"data\tours.wtml";

                DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/gettours.aspx", filename, false, false);

                Tours = Folder.LoadFromFile(filename, false);
                Tours.Name = Language.GetLocalizedText(492, "Tours");
                Folder loadFolder = null;


                loadFolder = Tours;

                // invoke the load

                MethodInvoker LoadIt = delegate
                {
                    LoadRootFoder(loadFolder);
                    BrowseList.EmptyAddText = Language.GetLocalizedText(162, "No Results");
                };
                try
                {
                    Invoke(LoadIt);
                }
                catch
                {
                }

                AddToursToSearch();

            }
            catch
            {
                MethodInvoker Error = delegate
                {
                    BrowseList.EmptyAddText = Language.GetLocalizedText(162, "No Results");
                    UiTools.ShowMessageBox(Language.GetLocalizedText(220, "Could not connect to the WorldWide Telescope Server to download tours, and there was no available cached tour list. Please check your network or proxy settings and make sure you have a network connection."));

                };
                try
                {
                    Invoke(Error);
                }
                catch
                {
                }
            }
            Cursor.Current = Cursors.Default;

        }

        public static void LaunchTour(ITourResult result)
        {
            var url = result.TourUrl;

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "tourcache\\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "tourcache\\");
            }

            var tempFile = Properties.Settings.Default.CahceDirectory + "tourcache\\" + result.Id + ".wtt";

            if (FileDownload.DownloadFile(url, tempFile, false))
            {
                var fi = new FileInfo(tempFile);
                if (fi.Length == 0)
                {
                    File.Delete(tempFile);
                    MessageBox.Show(Language.GetLocalizedText(221, "The tour file could not be downloaded and is not in cache. Check you network connection."), "WorldWide Telescope Tours");
                    return;
                }
                Earth3d.MainWindow.LoadTourFromFile(tempFile, false, result.Id);
            }
            lastTourLaunched = result;
        }

        public static void LoadLayer(string url)
        {
        
            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + "tourcache\\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + "tourcache\\");
            }

            var tempFile = Properties.Settings.Default.CahceDirectory + "tourcache\\" + ((uint)url.GetHashCode32()) + ".wwtl";

            if (FileDownload.DownloadFile(url, tempFile, true))
            {
                var fi = new FileInfo(tempFile);
                if (fi.Length == 0)
                {
                    File.Delete(tempFile);
                    MessageBox.Show(Language.GetLocalizedText(221, "The layer file could not be downloaded and is not in cache. Check you network connection."), "Microsoft WorldWide Telescope");
                    return;
                }
                LayerManager.LoadLayerFile(tempFile, "Sun", false);
                
            }
            
        }


        TourPopup popup;
        private void TourItemHover(object sender, object e)
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
                popup.Top = popup.TourResult.Bounds.Bottom - 10;
                popup.LaunchTour += popup_LaunchTour;
                popup.Show();
            }
        }

        private void TourItemDoubleClicked(object sender, Object e)
        {
            var result = (ITourResult)e;
            if (e == null)
            {
                return;
            }
            LaunchTour(result);
        }
        void popup_LaunchTour(object sender, EventArgs e)
        {
            if (popup == null)
            {
                return;
            }
            var result = popup.TourResult;
            popup.Close();
            popup.Dispose();
            popup = null;
            LaunchTour(result);

        }
        protected override void SetFocusedChild()
        {
            BrowseList.Focus();
        }



        private void BrowseList_ItemContextMenu(object sender, object e)
        {
            var thumb = breadcrumbs.Peek();
            var readOnly = true;

            if (thumb is Folder)
            {
                var owner = (Folder)thumb;
                readOnly = owner.ReadOnly;
            }

            thumb = e as IThumbnail;

            if (thumb.IsCloudCommunityItem)
            {
                if (e is Folder)
                {
                    ShowCloudCommunitiesFolderContextMenu(e as Folder);
                }
                else
                {
                    ShowCloudCommunitiesItemContextMenu(e as IThumbnail);
                }

                return;
            }


            var pntClick = Cursor.Position;
            var ta = breadcrumbs.ToArray();
            if (e is IImageSet)
            {
                var imageSet = (IImageSet)e;
                var tp = new TourPlace(imageSet.Name, imageSet.CenterX,imageSet.CenterY, Classification.Unidentified, "", imageSet.DataSetType, 360);
                tp.StudyImageset = imageSet;
                Earth3d.MainWindow.ShowContextMenu(tp, Earth3d.MainWindow.PointToClient(Cursor.Position), true, readOnly);

            }
            else if (e is IPlace)
            {
                if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(222, "Open Images"))
                {
                    readOnly = false;
                }
                //if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(222, "Open Images"))
                //{
                //    ShowOpenImageMenu((Place)e);
                //}
                //else
                {
                    Earth3d.MainWindow.ShowContextMenu((IPlace)e, Earth3d.MainWindow.PointToClient(Cursor.Position), true, readOnly);
                }
            }
            else if (e is Folder)
            {
                if (breadcrumbs.Count > 1 && ta[0].Name == Language.GetLocalizedText(223, "Open Collections"))
                {
                    ShowOpenFolderMenu((Folder)e);
                }
                else 
                {
                    ShowFolderMenu((Folder)e);
                }
            }
            if (e is Tour)
            {
                // TODO (Diego): fix
                var p = (Tour)e;
                if (p.IsTour)
                    ShowTourMenu(p);
            }

        }
        ContextMenuStrip contextMenu;

        private void ShowFolderMenu(Folder folder)
        {


            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }

            if (folder.ReadOnly)
            {
                ShowReadOnlyCollectionContextMenu(folder);
            }
            else
            {
                ShowMyCollectionsContextMenu(folder);
            }
        }

        private void ShowCloudCommunitiesFolderContextMenu(Folder folder)
        {
            contextMenu = new ContextMenuStrip();

            var  showFolder = new ToolStripMenuItem(Language.GetLocalizedText(995, "Show on Community Web Site"));
            var editFolder = new ToolStripMenuItem(Language.GetLocalizedText(996, "Edit on Community Web Site"));
            var deleteFolder = new ToolStripMenuItem(Language.GetLocalizedText(997, "Delete from Community Web Site"));
            var removeFolder = new ToolStripMenuItem(Language.GetLocalizedText(998, "Remove Subscription of this Community"));

            showFolder.Click += showFolder_Click;
            editFolder.Click += editFolder_Click;
            deleteFolder.Click += deleteFolder_Click;
            removeFolder.Click += removeFolder_Click;
            showFolder.Tag = folder;
            editFolder.Tag = folder;
            deleteFolder.Tag = folder;
            removeFolder.Tag = folder;

            if (folder.MSRCommunityId > 0)
            {
                contextMenu.Items.Add(showFolder);
            }

            if ((folder.Permission & 8) == 8)
            {
                contextMenu.Items.Add(editFolder);
                contextMenu.Items.Add(deleteFolder);
            }

            if (breadcrumbs.Count == 1 && !folder.ReadOnly)
            {
                contextMenu.Items.Add(removeFolder);
            }

            if (contextMenu.Items.Count > 0)
            {
                contextMenu.Show(Cursor.Position);
            }
        }

        void showFolder_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;

            if (folder.MSRCommunityId > 0)
            {
                WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/CommunityDetail/" + folder.MSRCommunityId, true);
            }
        }

        void editFolder_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;

            if (folder.MSRCommunityId > 0)
            {
                WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditCommunity/" + folder.MSRCommunityId, true);
            }
        }

        void deleteFolder_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var id = 0;

            var folder = item.Tag as Folder;
            
            if (folder != null)
            {
                id = (int)folder.MSRCommunityId;
            }


            if (id > 0)
            {
                if (UiTools.ShowMessageBox(Language.GetLocalizedText(999, "Do you really want to permanently delete this item from your community?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    EOCalls.InvokeDeleteCommunity(id);
                    Earth3d.RefreshCommunity();
                    return;
                }
            }
        }

        void removeFolder_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;
            if (UiTools.ShowMessageBox(Language.GetLocalizedText(1000, "Do you really want to unsubscribe this community link on this computer?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var target = BrowseList.Selected;

                var thumb = breadcrumbs.Peek();
                if (thumb is Folder)
                {
                    var owner = (Folder)thumb;
                    owner.RemoveChild(folder);
                    BrowseList.RemoveSelected();
                    if (File.Exists(folder.LoadedFilename))
                    {
                        File.Delete(folder.LoadedFilename);
                    }
                }
                BrowseList.Refresh();
            }
        }



        private void ShowCloudCommunitiesItemContextMenu(IThumbnail item)
        {
            contextMenu = new ContextMenuStrip();

            var showItem = new ToolStripMenuItem(Language.GetLocalizedText(995, "Show on Community Web Site"));
            var editItem = new ToolStripMenuItem(Language.GetLocalizedText(996, "Edit on Community Web Site"));
            var deleteItem = new ToolStripMenuItem(Language.GetLocalizedText(997, "Delete from Community Web Site"));
  
            showItem.Click += showItem_Click;
            editItem.Click += editItem_Click;
            deleteItem.Click += deleteItem_Click;
            showItem.Tag = item;
            editItem.Tag = item;
            deleteItem.Tag = item;

            var edit = false;

            if (item is Tour)
            {
                edit = ((((Tour)item).Permission & 8) == 8);
            }

            if (item is Place)
            {
                edit = ((((Place)item).Permission & 8) == 8);
            }

            if (item is ImageSet)
            {
                 edit = ((((ImageSet)item).Permission & 8) == 8);
            }

            contextMenu.Items.Add(showItem);
            
            if (edit)
            {
                contextMenu.Items.Add(editItem);
                contextMenu.Items.Add(deleteItem);
            }
            contextMenu.Show(Cursor.Position);
        }


        void showItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var id = 0;

            var place = item.Tag as Place;
            var tour = item.Tag as Tour;
            if (place != null)
            {
                id = (int)place.MSRComponentId;
            }

            if (tour != null)
            {
                id = (int)tour.MSRComponentId;
            }

            if (id > 0)
            {
                WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/ContentDetail/" + id, true);
            }

        }

        void editItem_Click(object sender, EventArgs e)
        {

            var item = (ToolStripMenuItem)sender;
            var id = 0;

            var place = item.Tag as Place;
            var tour = item.Tag as Tour;
            if (place != null)
            {
                id = (int)place.MSRComponentId;
            }
            if (tour != null)
            {
                id = (int)tour.MSRComponentId;
            }

            if (id > 0)
            {
                WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditContent/" + id, true);
            }
        }

        void deleteItem_Click(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            var id = 0;

            var place = item.Tag as Place;
            var tour = item.Tag as Tour;
            if (place != null)
            {
                id = (int)place.MSRComponentId;
            }

            if (tour != null)
            {
                id = (int)tour.MSRComponentId;
            }

            if (id > 0)
            {
                if (UiTools.ShowMessageBox(Language.GetLocalizedText(999, "Do you really want to permanently delete this item from your community?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    EOCalls.InvokeDeleteContent(id);
                    Earth3d.RefreshCommunity();
                    return;
                }
            }
        }


        private void ShowMyCollectionsContextMenu(Folder folder)
        {
            if (folder == myCollections)
            {
                return;
            }


            contextMenu = new ContextMenuStrip();

            var addToStack = new ToolStripMenuItem( Language.GetLocalizedText(1001, "Add Children to Image stack"));
            var deleteMenu = new ToolStripMenuItem(Language.GetLocalizedText(224, Language.GetLocalizedText(224, "Delete Folder")));


            var renameMenu = new ToolStripMenuItem(Language.GetLocalizedText(225, "Rename"));
            addToStack.Click += addToStack_Click;
            deleteMenu.Click += deleteMenu_Click;

            renameMenu.Click += renameMenu_Click;
            addToStack.Tag = folder;
            deleteMenu.Tag = folder;
            renameMenu.Tag = folder;
            
            
            if (Earth3d.MainWindow.ImageStackVisible)
            {
                contextMenu.Items.Add(addToStack);
            }
            contextMenu.Items.Add(deleteMenu);
            //if (folder.MSRCommunityId > 0)
            //{
            //    contextMenu.Items.Add(editMenu);
            //}
            //else
            {
                contextMenu.Items.Add(renameMenu);
            }
            contextMenu.Show(Cursor.Position);
        }

        void editMenu_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;

            if (folder.MSRCommunityId > 0)
            {
                WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditCommunity/" + folder.MSRCommunityId, true);
            }

        }

        private void ShowReadOnlyCollectionContextMenu(Folder folder)
        {
            contextMenu = new ContextMenuStrip();
            if (Earth3d.MainWindow.ImageStackVisible)
            {
                var addToStack = new ToolStripMenuItem(Language.GetLocalizedText(1001, "Add Children to Image stack"));
                addToStack.Click += addToStack_Click;
                addToStack.Tag = folder;
                contextMenu.Items.Add(addToStack);
            }
            

        }

        private void ShowTourMenu(Tour tour)
        {
   
        }

        private void removeCommunityTour_Click(object sender, EventArgs e)
        {
            BrowseList.RemoveSelected();
            BrowseList.Refresh();
        }

        void addToStack_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.AddClidrenToStack((Folder)((ToolStripMenuItem)sender).Tag, false);
            Earth3d.MainWindow.Stack.UpdateList();
        }

        
        private void ShowOpenFolderMenu(Folder folder)
        {

            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }

            contextMenu = new ContextMenuStrip();

            var closeMenu = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
            var copyMenu = new ToolStripMenuItem(Language.GetLocalizedText(226, "Copy To My Collections"));
            closeMenu.Click += closeMenu_Click;
            copyMenu.Click += copyMenu_Click;
            closeMenu.Tag = folder;
            copyMenu.Tag = folder;
            contextMenu.Items.Add(closeMenu);
            contextMenu.Items.Add(copyMenu);
            contextMenu.Show(Cursor.Position);
        }
        private void ShowOpenImageMenu(Place place)
        {

            if (contextMenu != null)
            {
                contextMenu.Dispose();
            }

            contextMenu = new ContextMenuStrip();

            var closeImageMenu = new ToolStripMenuItem(Language.GetLocalizedText(212, "Close"));
            var popertiesMenu = new ToolStripMenuItem(Language.GetLocalizedText(20, "Properties"));
            closeImageMenu.Click += closeImageMenu_Click;
            popertiesMenu.Click += popertiesMenu_Click;
            closeImageMenu.Tag = place;
            popertiesMenu.Tag = place;
            contextMenu.Items.Add(popertiesMenu);
            contextMenu.Items.Add(closeImageMenu);
            contextMenu.Show(Cursor.Position);
        }

        void closeImageMenu_Click(object sender, EventArgs e)
        {
            var place = (Place)((ToolStripMenuItem)sender).Tag;

            var thumb = breadcrumbs.Peek();
            if (thumb is Folder)
            {
                var owner = (Folder)thumb;
                owner.RemoveChild(place);
                BrowseList.RemoveSelected();

                if (Earth3d.MainWindow.StudyImageset.ImageSetID == place.StudyImageset.ImageSetID)
                {
                    Earth3d.MainWindow.StudyImageset = null;
                }
                if (BrowseList.Count == 0)
                {
                    MoveUpOneLevel();
                }
            }
            BrowseList.Refresh();
        } 

        void popertiesMenu_Click(object sender, EventArgs e)
        {
            var place = (IPlace)((ToolStripMenuItem)sender).Tag;

            ObjectProperties.ShowNofinder(place, Earth3d.MainWindow.RenderWindow.PointToScreen(Cursor.Position));

        }
        void copyMenu_Click(object sender, EventArgs e)
        {
            var thumb = breadcrumbs.Peek();
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;
            SaveFolderAs(folder);
        }

        bool SaveFolderAs(Folder folder)
        {
            var input = new SimpleInput(Language.GetLocalizedText(227, "Save Folder As"), Language.GetLocalizedText(228, "New Name"), folder.Name, 32);
            var retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(CollectionFileName(input.ResultText)))
                    {
                        var f = FindCollection(input.ResultText);
                        if (f != null)
                        {
                            MessageBox.Show(Language.GetLocalizedText(229, "Collection file name already exists, type a different name."), Language.GetLocalizedText(230, Language.GetLocalizedText(232, "Create New Collection")));
                            retry = true;
                        }
                        else
                        {
                            var validfileName = @"^[A-Za-z0-9_ ]*$";
                            if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
                            {
                                MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
                                retry = true;
                            }
                            else
                            {
                                var oldName = folder.Name;
                                folder.Name = input.ResultText;
                                folder.SaveToFile(CollectionFileName(folder.Name));
                                myCollections.AddChildFolder(Folder.LoadFromFile(CollectionFileName(folder.Name), false));
                                folder.Name = oldName;
                                retry = false;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Language.GetLocalizedText(233, "Name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
                        retry = true;
                    }
                }
                else
                {
                    return false;
                }
            } while (retry);

            return false;
        }
        void closeMenu_Click(object sender, EventArgs e)
        {
            var parent = (Folder)breadcrumbs.Peek();
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;
            parent.RemoveChild(folder);
            MoveUpOneLevel();
            
        }

        void renameMenu_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;
            if (folder.ReadOnly)
            {
                return;
            }

            var input = new SimpleInput(Language.GetLocalizedText(234, "Rename Collection"), Language.GetLocalizedText(228, "New Name"), folder.Name, 32);
            var retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (input.ResultText.ToLower() == folder.Name.ToLower())
                    {
                        folder.Name = input.ResultText;
                        folder.Dirty = true;
                        return;
                    }

                    if (!File.Exists(CollectionFileName(input.ResultText)))
                    {
                        var f = FindCollection(input.ResultText);
                        if (f != null)
                        {
                            MessageBox.Show(Language.GetLocalizedText(229, "Collection file name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
                            retry = true;
                        }
                        else
                        {
                            var validfileName = @"^[A-Za-z0-9_ ]*$";
                            if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
                            {
                                MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
                                retry = true;
                            }
                            else
                            {
                                folder.Name = input.ResultText;
                                if (File.Exists(folder.LoadedFilename))
                                {
                                    File.Delete(folder.LoadedFilename);
                                }
                                folder.Dirty = true;
                                folder.LoadedFilename = CollectionFileName(input.ResultText);
                                retry = false;
                                 return;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Language.GetLocalizedText(233, "Name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
                        retry = true;
                    }
                }
                else
                {
                    return;
                }
            } while (retry);
        }

        void deleteMenu_Click(object sender, EventArgs e)
        {
            var folder = (Folder)((ToolStripMenuItem)sender).Tag;

            if (folder.MSRCommunityId > 0)
            {
                var id = (int)folder.MSRCommunityId;
                EOCalls.InvokeDeleteCommunity(id);
                Earth3d.RefreshCommunity();
            }
            else
            {

                var target = BrowseList.Selected;

                var thumb = breadcrumbs.Peek();
                if (thumb is Folder)
                {
                    var owner = (Folder)thumb;
                    owner.RemoveChild(folder);
                    BrowseList.RemoveSelected();
                    if (File.Exists(folder.LoadedFilename))
                    {
                        File.Delete(folder.LoadedFilename);
                    }
                }
                BrowseList.Refresh();
            }
        }

        public void EditSelected()
        {
        }

        public void RemoveSelected()
        {
            var target = BrowseList.Selected;
            var thumb = breadcrumbs.Peek();
            if (thumb is Folder)
            {
                var owner = (Folder)thumb;
                if (target is Place)
                {
                    owner.RemoveChild((Place)target);
                }
                //todo remove imageset?
                BrowseList.RemoveSelected();
            }
            BrowseList.Refresh();
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


            var item = (IThumbnail)e;
            if (item.IsFolder)
            {
                if (communities && !Earth3d.IsLoggedIn && item.IsCloudCommunityItem)
                {
                    Earth3d.MainWindow.WindowsLiveSignIn();
                    if (!Earth3d.IsLoggedIn)
                    {
                        return;
                    }
                }
                LoadFolder(item);
            }
            else
            {
                if (e is IPlace)
                {
                    var p = (IPlace)e;
                    if (p.StudyImageset != null)
                    {
                        //if (p.StudyImageset.Projection != ProjectionType.SkyImage && p.StudyImageset.Projection != ProjectionType.Tangent)
                        if (p.RA == 0 && p.Dec == 0)
                        {
                            Earth3d.MainWindow.GotoTarget(p, false, doubleClick, true);
                            //Earth3d.MainWindow.SetStudyImageset(imageSet, null);
                            Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
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
                            WebWindow.OpenUrl(p.Url, false);
                        }
                    }
                    else
                    {
                        Earth3d.MainWindow.GotoTarget(p, false, doubleClick, true);
                    }

                }
                else if (e is Tour)
                {
                    TourItemDoubleClicked(sender, e);
                }
                else if (e is IImageSet)
                {
                    var imageSet = (IImageSet)e;
                    if (imageSet.Projection == ProjectionType.SkyImage || imageSet.Projection == ProjectionType.Tangent)
                    {
                        Earth3d.MainWindow.GotoTarget(new TourPlace("", imageSet.CenterY, imageSet.CenterX / 15, Classification.Unidentified, "UMA", ImageSetType.Sky, imageSet.BaseTileDegrees * 10), false, doubleClick, true);
                        Earth3d.MainWindow.SetStudyImageset(imageSet, null);

                    }
                    else
                    {
                        Earth3d.MainWindow.CurrentImageSet = imageSet;
                    }
                }
                else if (e is FolderUp)
                {
                    MoveUpOneLevel();
                }
                if (doubleClick)
                {
                    FlipPinupState(true);
                }
            }
        }

        Folder myCollections;
        Folder openCollections;
        Folder openImages;

        public Folder OpenImages
        {
            get
            {
                if (openImages == null)
                {
                    openImages = new Folder();
                    openImages.Name = Language.GetLocalizedText(222, "Open Images");
                    openImages.ThumbNail = Resources.Folder;
                }
                return openImages;
            }
            set { openImages = value; }
        }

        public Folder OpenCollections
        {
            get
            {
                if (openCollections == null)
                {
                    openCollections = new Folder();
                    openCollections.Name = Language.GetLocalizedText(223, "Open Collections");
                    openCollections.ThumbNail = Resources.Folder;
                }
                return openCollections;
            }
            set { openCollections = value; }
        }
        static string myCollectionsPath = "";

        public static string MyCollectionsPath
        {
            get
            {
                if (string.IsNullOrEmpty(myCollectionsPath))
                {
                    myCollectionsPath = string.Format("{0}\\WWT Collections\\", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
                }
                return myCollectionsPath; }
            set { myCollectionsPath = value; }
        }

        public Folder MyCollections
        {
            get
            {
                if (myCollections == null)
                {
                    if (Directory.Exists(string.Format("{0}\\Collections\\", Properties.Settings.Default.CahceDirectory)))
                    {
                        MoveOldCollections();
                    }
                    myCollections = new Folder();
                    myCollections.Name = Language.GetLocalizedText(236, "My Collections");
                    myCollections.ThumbNail = Resources.Folder;
                    myCollections.ReadOnly = false;
                    var collectionFolder = MyCollectionsPath;
                    if (!Directory.Exists(collectionFolder))
                    {
                        Directory.CreateDirectory(collectionFolder);
                    }
                    foreach (var file in Directory.GetFiles(collectionFolder, "*.wtml"))
                    {
                        try
                        {
                            var loadFolder = Folder.LoadFromFile(file, false);
                            loadFolder.ThumbNail = Resources.Folder;
                            loadFolder.ReadOnly = false;
                            MyCollections.AddChildFolder(loadFolder);
                        }
                        catch
                        {
                        }
                    }
                }
                return myCollections;
            }

            set { myCollections = value; }
        }

        private void MoveOldCollections()
        {
            // todo add try catch.
            var path = string.Format("{0}Collections\\", Properties.Settings.Default.CahceDirectory);

            if (!Directory.Exists(MyCollectionsPath))
            {
                Directory.CreateDirectory(myCollectionsPath);
            }

            foreach (var file in Directory.GetFiles(path,"*.wtml"))
            {
                File.Move(file, file.Replace(path, MyCollectionsPath));
            }

            Directory.Delete(path, true);

        }
        Folder myCommunities;

        public Folder MyCommunities
        {
            get
            {
                if (myCommunities == null)
                {
                    myCommunities = new Folder();
                    myCommunities.Name = Language.GetLocalizedText(237, "My Communities");
                    myCommunities.ThumbNail = Resources.Folder;
                    myCommunities.ReadOnly = false;
                    var communitiesFolder = Earth3d.CommuinitiesDirectory;
                    if (!Directory.Exists(communitiesFolder))
                    {
                        Directory.CreateDirectory(communitiesFolder);
                    }

                    // Add Perth Community entry
                    var perthCommunities = Folder.LoadFromUrl(Properties.Settings.Default.CloudCommunityUrlNew + "/Resource/Service/Payload", false);
                    if (perthCommunities != null)
                    {
                        perthCommunities.ReadOnly = true;
                        myCommunities.AddChildFolder(perthCommunities);
                    }

                    foreach (var file in Directory.GetFiles(communitiesFolder, "*.wtml"))
                    {
                        var loadFolder = Folder.LoadFromFile(file, false);
                        loadFolder.ReadOnly = false;
                        myCommunities.AddChildFolder(loadFolder);
                    }
                }
                return myCommunities;
            }
        }    

        public Folder NewCollection()
        {
            var input = new SimpleInput(Language.GetLocalizedText(232, "Create New Collection"), Language.GetLocalizedText(238, "Name"), "", 32);
            var retry = false;
            do
            {
                if (input.ShowDialog() == DialogResult.OK)
                {
                    if (!File.Exists(CollectionFileName(input.ResultText)))
                    {
                        var f = FindCollection(input.ResultText);
                        if (f != null)
                        {
                            MessageBox.Show(Language.GetLocalizedText(229, "Collection file name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
                            retry = true;
                        }
                        else
                        {
                            var validfileName = @"^[A-Za-z0-9_ ]*$";
                            if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
                            {
                                MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
                                retry = true;
                            }
                            else
                            {
                                var newCollection = new Folder();
                                newCollection.Name = input.ResultText;
                                newCollection.ThumbNail = Resources.Folder;
                                newCollection.LoadedFilename = CollectionFileName(input.ResultText);
                                newCollection.ReadOnly = false;
                                MyCollections.AddChildFolder(newCollection);
                                retry = false;
                                return newCollection;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Language.GetLocalizedText(233, "Name already exists, type a different name."), Language.GetLocalizedText(232, "Create New Collection"));
                        retry = true;
                    }
                }
                else
                {
                    return null;
                }
            } while (retry);
            return null;
        }

        public string CollectionFileName(string name)
        {
            return string.Format("{0}{1}.wtml", MyCollectionsPath, name);
        }

        public Folder FindCollection(string name)
        {
            foreach (var f in MyCollections.Folder1)
            {
                if (f.Name == name)
                {
                    return f;
                }
            }
            return null;
        }


        private void AddCollectionFolders()
        {
            if (OpenCollections.Children.GetLength(0) > 0)
            {
                BrowseList.Insert(OpenCollections);
            }

            if (OpenImages.Children.GetLength(0) > 0)
            {
                BrowseList.Insert(OpenImages);
            }     

            BrowseList.Insert(MyCollections);
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
            if (openCollections == null)
            {
                return false;
            }

            foreach (var folder in openCollections.Folder1)
            {
                if (folder.LoadedFilename.ToLower() == filename.ToLower())
                {
                    if (showCollection)
                    {
                        ShowCollection(folder);
                    }

                    return true;
                }
            }
            return false;
        }

        public void LoadCollection(Folder collection)
        {
            OpenCollections.AddChildFolder(collection);
            while (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
            }
            var top = breadcrumbs.Pop();
            breadcrumbs.Clear();
            LoadFolder(top);
            LoadFolder(OpenCollections);
            LoadFolder(collection);
        }

        public void ShowCollection(Folder collection)
        {
            while (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
            }
            var top = breadcrumbs.Pop();
            breadcrumbs.Clear();
            LoadFolder(top);
            LoadFolder(OpenCollections);
            LoadFolder(collection);
        }
        private void LoadFolder(IThumbnail parent)
        {
            Cursor.Current = Cursors.WaitCursor;
            var topLevel = breadcrumbs.Count == 0;

            var communityNode = false;
            var firstSearchable = true;
            if (breadcrumbs.Count == 1)
            {
                if (MyCommunities == breadcrumbs.Peek())
                {
                    communityNode = true;
                }
            }

            breadcrumbs.Push(parent);
            BrowseList.Clear();
            if (topLevel && showMyFolders)
            {
                AddCollectionFolders();
            }

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
            var children = parent.Children;

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child is Folder)
                    {
                        var folderItem = (Folder)child;
                        if (folderItem.Browseable == FolderBrowseable.True)
                        {
                            BrowseList.Add(child);
                        }
                        if (communityNode &&folderItem.Searchable == FolderSearchable.True)
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


            var sb = new StringBuilder();

            var text = "";

          


            foreach (var item in breadcrumbs)
            {
                text = item.Name + "  > " + text;
            }

            if (Earth3d.IsLoggedIn)
            {
                exploreText.Text ="(" + Properties.Settings.Default.LiveIdUser + ") " + text;
            }
            else
            {
                exploreText.Text = text;
            }
            BrowseList.ShowAddButton = (!parent.ReadOnly) && !(hideAddButtonInLowerLevels && !topLevel);
            Refresh();
            Cursor.Current = Cursors.Default;
        }

        public void ReloadFolder()
        {
            var current = breadcrumbs.Pop();
            LoadFolder(current);
        }

        private void BrowseList_ItemHover(object sender, object e)
        {
            TourItemHover(sender, e);
            if (Earth3d.MainWindow.IsWindowOrChildFocused())
            {
                Focus();
            }
            if (e is IPlace || e is IImageSet)
            {
                IImageSet imageset = null;
                if (e is IPlace)
                {
                    var p = (IPlace)e;
                    Earth3d.MainWindow.SetLabelText(p, true);
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

                toolTips.SetToolTip(BrowseList, ((IThumbnail)e).Name);

                if (imageset != null)
                {
                    Earth3d.MainWindow.PreviewImageset = imageset;
                    Earth3d.MainWindow.PreviewBlend.TargetState = true;
                }
                else
                {
                    Earth3d.MainWindow.PreviewBlend.TargetState = false;
                }
            }
            else
            {
                if (e != null)
                {
                    toolTips.SetToolTip(BrowseList, ((IThumbnail)e).Name);
                }
                Earth3d.MainWindow.SetLabelText(null, false);
                Earth3d.MainWindow.PreviewBlend.TargetState = false;

            }
        }

        private void BrowseList_ItemImageClicked(object sender, object e)
        {
            if (e is IPlace)
            {
                var p = (IPlace)e;

                Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);
            }
            
        }
        public override bool AdvanceSlide(bool fromStart)
        {
            return BrowseList.ShowNext(fromStart, false);
        }

        private void exploreText_MouseEnter(object sender, EventArgs e)
        {
            exploreText.ForeColor = Color.Yellow;
        }

        private void exploreText_MouseLeave(object sender, EventArgs e)
        {
            exploreText.ForeColor = Color.White;

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
            var item = BrowseList.Selected;

            if (item != null)
            {
                if (item is IPlace)
                {
                    var p = (IPlace)item;

                    if (p.StudyImageset != null)
                    {
                        Earth3d.MainWindow.StudyImageset = p.StudyImageset;
                    }
                    else if (p.BackgroundImageSet != null)
                    {
                        Earth3d.MainWindow.StudyImageset = p.BackgroundImageSet;
                    }

                    return;
                }
                if (item is IImageSet)
                {
                    var imageSet = (IImageSet)item;
                    Earth3d.MainWindow.StudyImageset = imageSet;
                }
            }
        }

        public void SelectBackground()
        {

            var item = BrowseList.Selected;

            if (item != null)
            {
                if (item is IPlace)
                {
                    var p = (IPlace)item;

                    if (p.StudyImageset != null)
                    {
                        Earth3d.MainWindow.CurrentImageSet = p.StudyImageset;
                    }
                    else if (p.BackgroundImageSet != null)
                    {
                        Earth3d.MainWindow.CurrentImageSet = p.BackgroundImageSet;
                    }

                    return;
                }
                if (item is IImageSet)
                {
                    var imageSet = (IImageSet)item;
                    Earth3d.MainWindow.CurrentImageSet = imageSet;
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
                var parent = breadcrumbs.Pop();
                LoadFolder(parent);
            }
        }

        private void BrowseList_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void FolderBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveMyCollections();
        }
        
        public static bool AllDirty = false;

        private void SaveMyCollections()
        {
            foreach (var f in MyCollections.Folder1)
            {
                if (f.Dirty || AllDirty)
                {
                    f.Save();
                }
            }
        }

        private void BrowseList_AddNewItem(object sender, object e)
        {
            var parent = breadcrumbs.Peek();
            if (parent.Name == Language.GetLocalizedText(236, "My Collections"))
            {
                var folder = NewCollection();
                if (folder != null)
                {
                    LoadFolder(folder);
                }
            }
            else
            {
                if (parent is Folder)
                {
                    var owner = (Folder)parent;
                    var input = new SimpleInput(Language.GetLocalizedText(239, "Create New Folder"), Language.GetLocalizedText(238, "Name"), "", 32);
                    var retry = false;
                    do
                    {
                        if (input.ShowDialog() == DialogResult.OK)
                        {
                            var validfileName = @"^[A-Za-z0-9_ ]*$";
                            if (input.ResultText.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !UiTools.ValidateString(input.ResultText, validfileName))
                            {
                                MessageBox.Show(Language.GetLocalizedText(231, "A name can not contain any of the following characters:") + " \\ / : * ? \" < > |", Language.GetLocalizedText(232, "Create New Collection"));
                                retry = true;
                            }
                            else
                            {
                                var newCollection = new Folder();
                                newCollection.Name = input.ResultText;
                                newCollection.ThumbNail = Resources.Folder;
                                newCollection.LoadedFilename = CollectionFileName(input.ResultText);
                                newCollection.ReadOnly = false;
                                owner.AddChildFolder(newCollection);
                                LoadFolder(newCollection);
                                retry = false;                     
                            }

                        }
                    } while (retry);
                }
            }

        }

        internal void ShowOpenImages()
        {
            while (breadcrumbs.Count > 1)
            {
                breadcrumbs.Pop();
            }
            var top = breadcrumbs.Pop();
            breadcrumbs.Clear();
            LoadFolder(top);
            LoadFolder(OpenImages);
        }

        internal void PlayNext()
        {
            BrowseList.ShowNext(false, true);
        }

        internal void LoadCommunities()
        {
            myCommunities = null;


            LoadRootFoder(MyCommunities);
        }
        bool communities;
        ThumbnailSize rootSize = ThumbnailSize.Small;
        bool hideAddButtonInLowerLevels;

        internal void SetCommunitiesMode()
        {
            rootSize = ThumbnailSize.Big;
            BrowseList.AddText = Language.GetLocalizedText(240, "Join a community");
            BrowseList.ShowAddButton = true;     
            BrowseList.AddNewItem += BrowseList_AddNewCommunity;
            hideAddButtonInLowerLevels = true;
            communities = true;
        }

        void BrowseList_AddNewCommunity(object sender, object e)
        {
            Earth3d.MainWindow.JoinCommunity();
        }

        internal void SetExploreMode()
        {
            BrowseList.AddNewItem += BrowseList_AddNewItem;
        }

        internal void AutoSave()
        {
            SaveMyCollections();
            AllDirty = false;
        }

        internal void ShowCurrent()
        {
            BrowseList.ShowCurrent();
        }
    }

    public class FolderUp : IThumbnail
    {

        #region IThumbnail Members

        public string Name
        {
            get { return Language.GetLocalizedText(946, "Up Level"); }
        }

        public Bitmap ThumbNail
        {
            get
            {
                return Resources.FolderUp;
            }
            set
            {
                return ;
            }
        }

        Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        public bool IsImage
        {
            get { return false; }
        }

        public bool IsCloudCommunityItem
        {
            get
            {
                return false;
            }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool IsFolder
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get { return true; }
        }

        public object[] Children
        {
            get { return null; }
        }

        #endregion
    }
}