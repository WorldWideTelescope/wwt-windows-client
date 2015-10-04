using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using TerraViewer.Properties;


namespace TerraViewer
{
    public partial class ContextPanel : Form
    {
        public ContextPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            InitializeComponent();
            SetUiStrings();
            FilterCombo.Tag = Classification.Unfiltered;
            
        }

        readonly Bitmap layerButton = Resources.layersButton;
        readonly Bitmap layerButtonHover = Resources.layersButtonHover;

        private void SetUiStrings()
        {
            toolTips.SetToolTip(levelLabel, Language.GetLocalizedText(171, "Filed of view height in degrees : minutes : seconds of arc"));
            toolTips.SetToolTip(ConstellationLabel, Language.GetLocalizedText(172, "Name of the constellation that the view center is in."));
            toolTips.SetToolTip(queueProgressBar, Language.GetLocalizedText(173, "Download Progress"));
            toolTips.SetToolTip(studyOpacity, Language.GetLocalizedText(174, "Crossfades background and foreground imagery"));
            toolTips.SetToolTip(viewTarget, Language.GetLocalizedText(175, "Select the type of view, Sky, Earth, etc."));
            toolTips.SetToolTip(ImageDataSetsCombo, Language.GetLocalizedText(176, "Select the imagery to display"));
            toolTips.SetToolTip(FilterCombo, Language.GetLocalizedText(177, "Filters context results"));
            toolTips.SetToolTip(contextResults, Language.GetLocalizedText(178, "Context Search - Shows interesting places in the current view."));
            toolTips.SetToolTip(overview, Language.GetLocalizedText(179, "Constellation Overview"));
            toolTips.SetToolTip(SkyBall, Language.GetLocalizedText(180, "Shows field of view relative to the celestial sphere"));
            toolTips.SetToolTip(layerToggle, Language.GetLocalizedText(953, "Show/Hide Layer Manager"));
            contextResults.AddText = Language.GetLocalizedText(161, "Add New Item");
            contextResults.EmptyAddText = Language.GetLocalizedText(162, "No Results");
            faderText.Text = Language.GetLocalizedText(181, "Image Crossfade");
            trackingText.Text = Language.GetLocalizedText(182, "Tracking");
            trackingLabel.Text = Language.GetLocalizedText(182, "Tracking");
            label1.Text = Language.GetLocalizedText(155, "Imagery");
            label2.Text = Language.GetLocalizedText(183, "Look At");
            label3.Text = Language.GetLocalizedText(184, "Context Search Filter");
            scaleText.Text = Language.GetLocalizedText(578, "Planet Size");
            bigSizeLabel.Text = Language.GetLocalizedText(579, "Large");
            actualSizeLabel.Text = Language.GetLocalizedText(580, "Actual");
        }
        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    Graphics g = e.Graphics;
        //    g.Clear(Color.Red);

        //}

        int level;

        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        double ra;

        public double RA
        {
            get { return ra; }
            set
            {
                if (ra != value)
                {
                    ra = value;
                    contextAreaChanged = true;
                }
                SkyBall.RA = ra;
            }
        }
        double dec;

        public double Dec
        {
            get { return dec; }
            set
            {
                if (dec != value)
                {
                    dec = value;
                    contextAreaChanged = true;
                }
                SkyBall.Dec = dec;
            }
        }
        string constellation= " ";

        bool contextAreaChanged;

        public string Constellation
        {
            get { return constellation; }
            set
            {
                if (constellation != value)
                {
                    constellation = value;
                    ConstellationLabel.Text = value;
                    overview.Constellation = value;
                    contextAreaChanged = true;
                }
            }
        }
        double viewLevel = 1;

        public double ViewLevel
        {
            get { return viewLevel; }
            set
            {
                if (viewLevel != value)
                {
                    viewLevel = value;
                    levelLabel.Text = Coordinates.FormatDMS(viewLevel);
                }
            }
        }

        public string ViewLabelText
        {
            get
            {
                if (levelLabel != null)
                {
                    return levelLabel.Text;
                }
                return "00:00:00";
            }
        }

        bool sandbox;

        public bool Sandbox
        {
            get { return sandbox; }
            set { sandbox = value; }
        }

        double distance = 1;
        bool lastSandbox;

        public double Distance
        {
            set
            {
                if (distance != value || lastSandbox != sandbox)
                {
                    distance = value;
                    lastSandbox = sandbox;
                    if (Sandbox)
                    {
                        levelLabel.Text = UiTools.FormatDistancePlain(distance);
                    }
                    else
                    {
                        levelLabel.Text = UiTools.FormatDistance(distance);
                    }
                }
            }
        }    
        
        Coordinates[] cornersLast;

        public void SetViewRect(Coordinates[] corners)
        {
            if (corners.Length != 4)
            {
                return;
            }
            var change = false;
            if (cornersLast != null)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (cornersLast[i] != corners[i])
                    {
                        change = true;
                        break;
                    }
                }
            }
            else
            {
                change = true;
            }

            if (!change)
            {
                return;
            }
            cornersLast = corners;
            contextAreaChanged = true;

            overview.SetViewRect(corners);
            SkyBall.SetViewRect(corners);
        }

        int queueProgress;

        public int QueueProgress
        {
            get
            {
                return queueProgress;
            }

            set
            {
                queueProgress = value;
                if (queueProgressBar.Value != value)
                {
                    queueProgressBar.Value = value;
                    queueProgressBar.Invalidate();
                }

            }
        }
        static string lastTracking = "";
        bool autoRestore;
        private void searchTimer_Tick(object sender, EventArgs e)
        {
            var space = Earth3d.MainWindow.Space;
            studyOpacity.Value = (int)Earth3d.MainWindow.StudyOpacity;
            UpdateVisibility(space);

            var rect = RectangleToScreen(ClientRectangle);
            var inside = rect.Contains(Cursor.Position) || Earth3d.TouchKiosk || !(TourPlayer.Playing || Earth3d.FullScreen || Properties.Settings.Default.AutoHideContext);

            if (inside != fader.TargetState)
            {
                fader.TargetState = inside;
                FadeTimer.Enabled = true;
                FadeTimer.Interval = 10;
            }


            if (Earth3d.MainWindow.IsMoving)
            {
                return;
            }

            if (contextAreaChanged && cornersLast != null && !String.IsNullOrEmpty(constellation) && !Earth3d.MainWindow.SolarSystemMode && !Settings.DomeView)
            {
                RunQuery();

                contextAreaChanged = false;
            }
            else if (contextAreaChanged && Earth3d.MainWindow.SolarSystemMode)
            {
                RunQuerySolarSystem();
                contextAreaChanged = false;
            }
            else if (contextAreaChanged && Earth3d.MainWindow.CurrentImageSet.DataSetType == ImageSetType.Panorama)
            {

                FillPanoramaList();
                contextAreaChanged = false;
            }
            else if ((cornersLast == null || String.IsNullOrEmpty(constellation)) && !Earth3d.MainWindow.SolarSystemMode && Earth3d.MainWindow.CurrentImageSet.DataSetType != ImageSetType.Panorama)
            {
                if (contextResults.Count > 0)
                {
                    contextResults.Clear();
                    paginator1.CurrentPage = 0;
                    paginator1.TotalPages = 1;
                }
                contextAreaChanged = false;
            }
        }

        private void FillPanoramaList()
        {
            if (contextResults.Items.Count > 0)
            {
                var im = contextResults.Items[0] as IImageSet;
                if (im != null)
                {
                    if (im.DataSetType == ImageSetType.Panorama)
                    {
                        return;
                    }
                }

            }

            contextResults.Clear();
            paginator1.CurrentPage = 1;
            paginator1.TotalPages = 1;


            foreach (var o in Earth3d.MainWindow.ExplorerRoot.Children)
            {
                if (o is Folder)
                {
                    var f = (Folder)o;
                    if (f.Name == "Panoramas")
                    {
                        AddChildern(f);
                    }
                }
            }
          

            contextResults.Invalidate();    
        }

        void AddChildern(Folder folder)
        {
            foreach (var o1 in folder.Children)
            {
                if (o1 is Folder)
                {
                    AddChildern(o1 as Folder);
                }
                else
                {
                    contextResults.Add(o1);
                }
            }
        }

        static public bool ShowTimeline = false;


        private void UpdateVisibility(bool space)
        {
            if (ShowTimeline)
            {
                Timeline.Visible = true;
                closeBox.Visible = true;
                PushPin.Visible = true;
                levelLabel.Visible = false;
                ConstellationLabel.Visible = false;
                queueProgressBar.Visible = false;
                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                studyOpacity.Visible = false;
                viewTarget.Visible = false;
                ImageDataSetsCombo.Visible = false;
                FilterCombo.Visible = false;
                contextResults.Visible = false;
                paginator1.Visible = false;
                overview.Visible = false;
                SkyBall.Visible = false;
                layerToggle.Visible = false;
                faderText.Visible = false;             
                trackingText.Visible = false;
                trackingLabel.Visible = false;
                actualSizeLabel.Visible = false;
                bigSizeLabel.Visible = false;
                scaleText.Visible = false;
                scaleLabel.Visible = false;
                scaleButton.Visible = false;
                solarSystemScaleTrackbar.Visible = false;
                trackingTarget.Visible = false;
                info.Visible = false;
            }
            else
            {
                PushPin.Visible = false;
                closeBox.Visible = false;
                levelLabel.Visible = true;
                ConstellationLabel.Visible = true;
                queueProgressBar.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                viewTarget.Visible = true;
                ImageDataSetsCombo.Visible = true;
                FilterCombo.Visible = true;
                paginator1.Visible = true;
                SkyBall.Visible = true;
                layerToggle.Visible = true;
                trackingText.Visible = true;
                trackingTarget.Visible = true;
                info.Visible = true;



                Timeline.Visible = false;



                var solarSystem = Earth3d.MainWindow.SolarSystemMode;
                studyOpacity.Visible = (Earth3d.MainWindow.StudyImageset != null);
                trackingTarget.Visible = (state != WindowsStates.StatusOnly) && solarSystem;
                contextResults.Visible = state != WindowsStates.StatusOnly;

                actualSizeLabel.Visible = solarSystem;
                bigSizeLabel.Visible = solarSystem;
                solarSystemScaleTrackbar.Visible = solarSystem;
                scaleText.Visible = solarSystem;

                if (solarSystem)
                {
                    if (solarSystemScaleTrackbar.Value != Settings.Active.SolarSystemScale)
                    {
                        solarSystemScaleTrackbar.Value = Settings.Active.SolarSystemScale;
                    }
                }

                SkyBall.Space = space;
                if (solarSystem)
                {
                    if (SkyBall.ShowSkyball)
                    {
                        SkyBall.ShowSkyball = false;
                        SkyBall.RefreshHint();
                    }
                }
                else
                {
                    SkyBall.ShowSkyball = state != WindowsStates.StatusOnly;
                    //SkyBall.ShowSkyball = space && state != WindowsStates.StatusOnly;
                }
                overview.Visible = (state != WindowsStates.StatusOnly) && !(solarSystem || !space);

                if (Earth3d.MainWindow.Tracking && (Earth3d.MainWindow.Space))
                {
                    trackingLabel.Visible = true;
                    if (lastTracking != Earth3d.MainWindow.TrackingObject.Name)
                    {
                        trackingText.Text = Earth3d.MainWindow.TrackingObject.Name.Substring(0, Math.Min(40, Earth3d.MainWindow.TrackingObject.Name.Length));
                        lastTracking = Earth3d.MainWindow.TrackingObject.Name;
                    }
                }
                else if (Earth3d.MainWindow.SolarSystemMode)
                {
                    trackingLabel.Visible = true;
                    if (Earth3d.MainWindow.TrackingObject != null && lastTracking != Earth3d.MainWindow.TrackingObject.Name)
                    {
                        trackingText.Text = Earth3d.MainWindow.TrackingObject.Name;
                        lastTracking = trackingText.Text;
                        trackingTarget.Items.Clear();
                        trackingTarget.Items.Add(Earth3d.MainWindow.TrackingObject);
                        trackingTarget.Invalidate();
                    }
                    if (contextResults.Count == 0)
                    {
                        RunQuerySolarSystem();
                        contextAreaChanged = false;
                    }
                }
                else
                {
                    trackingLabel.Visible = false;
                    trackingText.Text = "";
                }

                // COmmented to add context search to planetary mode
                //if (!(space || solarSystem) && state != WindowsStates.StatusOnly)
                //{
                //    autoRestore = true;
                //    SetWindowState(WindowsStates.StatusOnly);
                //}

                if ((space || solarSystem) && state == WindowsStates.StatusOnly && autoRestore)
                {
                    SetWindowState(WindowsStates.OneLine);
                }
            }
        }
        private void RunQueryPlanet(string planet)
        {
            if (cornersLast != null && !String.IsNullOrEmpty(planet))
            {
                contextResults.Clear();
                paginator1.CurrentPage = 1;
                paginator1.TotalPages = 1;
                var searchDistance = (float)Math.Min((0.4617486132350 * ((4.0 * (Earth3d.MainWindow.ZoomFactor / 180)) + 0.000001)), 1.4142135623730950488016887242097);
                var results = ContextSearch.FindConteallationObjectsInCone(planet, Earth3d.MainWindow.ViewLong/15, Earth3d.MainWindow.ViewLat, searchDistance, (Classification)FilterCombo.Tag);
                
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                contextResults.Invalidate();
            }
        }
        private void RunQuery()
        {
            if ( cornersLast != null && !String.IsNullOrEmpty(constellation))
            {
                contextResults.Clear();
                paginator1.CurrentPage = 1;
                paginator1.TotalPages = 1;

                //Place[] results = ContextSearch.FindConteallationObjects(Constellations.Abbreviation(constellation), cornersLast, (Classification)FilterCombo.Tag);

                var cornerUl = Coordinates.RADecTo3d(cornersLast[0].RA, cornersLast[0].Dec,1);
                var cornerLR = Coordinates.RADecTo3d(cornersLast[2].RA, cornersLast[2].Dec,1);
                var dist = Vector3d.Subtract(cornerLR, cornerUl);
                var results = ContextSearch.FindConteallationObjectsInCone("SolarSystem", Earth3d.MainWindow.RA, Earth3d.MainWindow.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                results = ContextSearch.FindConteallationObjectsInCone("Community", Earth3d.MainWindow.RA, Earth3d.MainWindow.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }

                results = ContextSearch.FindConteallationObjectsInCone(Constellations.Abbreviation(constellation), Earth3d.MainWindow.RA, Earth3d.MainWindow.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                contextResults.Invalidate();
                //paginator1.CurrentPage = 0;
                //paginator1.TotalPages = contextResults.PageCount;
            }
        }

        private void RunQuerySolarSystem()
        {
            if (contextResults.Items.Count > 0)
            {
                var pl = contextResults.Items[0] as IPlace;
                if (pl != null)
                {
                    if (pl.Classification == Classification.SolarSystem)
                    {
                        return;
                    }
                }

            }
            contextResults.Clear();
            paginator1.CurrentPage = 1;
            paginator1.TotalPages = 1;

            var results = ContextSearch.FindAllObjects("SolarSystem", Classification.SolarSystem);
            if (results != null)
            {
                contextResults.AddRange(results);
            }

            contextResults.Invalidate();           
        }

        private void ContextPanel_Load(object sender, EventArgs e)
        {

            FilterCombo.Items.AddRange(Enum.GetNames(typeof(Classification)));

            if (Earth3d.TouchKiosk)
            {
                label2.Visible = false;
                viewTarget.Visible = false;
                label3.Visible = false;
                FilterCombo.Visible = false;
                label1.Visible = false;
                ImageDataSetsCombo.Visible = false;
            }


            viewTarget.Items.AddRange(Enum.GetNames(typeof(ImageSetType)));

            var id = Properties.Settings.Default.StartUpLookAt;

            // Last is set to a Random type at FormLoad in the Main Form if the Random mode is set
            if (Properties.Settings.Default.StartUpLookAt == 5 || Properties.Settings.Default.StartUpLookAt == 6)
            {
                id = Properties.Settings.Default.LastLookAtMode;
            }

            viewTarget.SelectedIndex = id;

            LoadImageSetList();
            overview.ThumbnailAllConstellations();
            FilterCombo.Filter = (Classification)Enum.Parse(typeof(Classification), Properties.Settings.Default.ContextSearchFilter, true);
            studyOpacity.Height = 23;
            Earth3d.MainWindow.ImageSetChanged += MainWindow_ImageSetChanged;
            fader.TargetState = true;
            FadeTimer.Enabled = true;
            FadeTimer.Interval = 10;
        }

        internal void SetLookAtTarget(ImageSetType lookAt)
        {
            MethodInvoker doIt = delegate
            {
                SetLookAtTargetLocal(lookAt);
            };

            if (InvokeRequired)
            {
                try
                {
                    Invoke(doIt);
                }
                catch
                {
                }
            }
            else
            {
                doIt();
            }
        }

        private void SetLookAtTargetLocal(ImageSetType lookAt)
        {
            var index = 0;
            foreach (string name in viewTarget.Items)
            {
                if (name == lookAt.ToString())
                {
                    viewTarget.SelectedIndex = index;
                    break;
                }
                index++;
            }
            index = 0;
        }

        void MainWindow_ImageSetChanged(object sender, EventArgs e)
        {
            if (deferUpdate)
            {
                return;
            }
            deferUpdate = true;
            var index = 0;
            foreach (string name in viewTarget.Items)
            {
                if (name == Earth3d.MainWindow.CurrentImageSet.DataSetType.ToString())
                {
                    viewTarget.SelectedIndex = index;
                    break;
                }
                index++;
            }
            index = 0;
            foreach(var o in ImageDataSetsCombo.Items)
            {
                if (o is IImageSet)
                {
                    var imageset = (IImageSet)o;
                    if (imageset == Earth3d.MainWindow.CurrentImageSet)
                    {
                        ImageDataSetsCombo.SelectedIndex = index;
                        break;
                    }
                    index++;
                }
            }
            deferUpdate = false;
        }
        bool deferUpdate;
        private void LoadImageSetList()
        {
            if (viewTarget.SelectedItem == null)
            {
                return;
            }
            var target = (ImageSetType)Enum.Parse(typeof(ImageSetType),viewTarget.SelectedItem.ToString());
            ImageDataSetsCombo.Items.Clear();
            foreach (var set in Earth3d.ImageSets)
            {
                if (set.DataSetType == target)
                {
                    ImageDataSetsCombo.Items.Add(set);
                }
            }
            ImageDataSetsCombo.SelectedIndex = 0;
            if (target != ImageSetType.SolarSystem)
            {
                ImageDataSetsCombo.Items.Add(Language.GetLocalizedText(130, "Browse..."));
            }
        }


        private void contextResults_ItemClicked(object sender, Object e)
        {
            var place = e as IPlace;

            if (place != null)
            {
                var p = e as Place;
                if (p != null && p.Tour != null)
                {
                    FolderBrowser.LaunchTour(p.Tour);
                    return;
                }

                if (!string.IsNullOrEmpty(((IPlace)e).Url))
                {
                    WebWindow.OpenUrl(((IPlace)e).Url, false);
                }
                else
                {
                    Earth3d.MainWindow.GotoTarget((IPlace)e, false, false, true);
                }
            }
            else if (e is IImageSet)
            {
                Earth3d.MainWindow.CurrentImageSet= e as IImageSet;
            }
        }

        private void contextResults_ItemDoubleClicked(object sender, Object e)
        {
            if (e is IPlace)
            {
                Earth3d.MainWindow.GotoTarget((IPlace)e, false, true, true);
            }
            else if (e is IImageSet)
            {
                Earth3d.MainWindow.CurrentImageSet = e as IImageSet;
            }
        }

        private void contextResults_ItemImageClicked(object sender, object e)
        {

           
            if (e is IPlace)
            {
                var p = (IPlace)e;
                Earth3d.MainWindow.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);

            }
            else if (e is IImageSet)
            {
                Earth3d.MainWindow.CurrentImageSet = e as IImageSet;
            }
        }

        private void FilterCombo_FilterChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ContextSearchFilter = ((Classification)FilterCombo.Tag).ToString();
            RunQuery();
        }

        private void ImageDataSetsCombo_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                ObjectProperties.HideProperties();

                if (ImageDataSetsCombo.SelectedItem is IImageSet)
                {
                    if (!deferUpdate)
                    {
                        Earth3d.MainWindow.SetViewMode((IImageSet)ImageDataSetsCombo.SelectedItem);
                    }
                }
                else
                {
                    if (ImageDataSetsCombo.SelectedItem.GetType() == typeof(String) && (String)ImageDataSetsCombo.SelectedItem == Language.GetLocalizedText(130, "Browse..."))
                    {
                        var openFile = new OpenFileDialog();
                        if (openFile.ShowDialog() == DialogResult.OK)
                        {
                            try
                            {
                                //todo move to use settings for directory instead of hard code
                                File.Delete(Properties.Settings.Default.CahceDirectory + @"\imagery\999\0\0\0_0.tif");
                            }
                            catch
                            {
                            }
                            Earth3d.MainWindow.SetViewMode(new ImageSetHelper(openFile.FileName, openFile.FileName, (ImageSetType)Enum.Parse(typeof(ImageSetType), (string)viewTarget.SelectedItem), BandPass.Visible, ProjectionType.Spherical, 999, 0, 0, 256, 256, ".tif", false, "", 0, 0, 0, false, "", false, false, 1, 0, 0, "", "", "", "", 0, ""));
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void contextResults_ItemHover(object sender, Object e)
        {
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
                toolTips.SetToolTip(contextResults, ((IThumbnail)e).Name);

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
                    toolTips.SetToolTip(contextResults, ((IThumbnail)e).Name);
                }
                Earth3d.MainWindow.SetLabelText(null, false);
                Earth3d.MainWindow.PreviewBlend.TargetState = false;

            }

  
        }

        enum WindowsStates { StatusOnly, OneLine, MultiLine };

        WindowsStates state = WindowsStates.OneLine;

        private void SetWindowState(WindowsStates newState)
        {
            var diff=0;

            switch (newState)
            {
                case WindowsStates.StatusOnly:
                    pinUp.Direction = Direction.Expanding;
                    pinUp1.Visible = false;
                    diff = Height - 50;
                    break;
                case WindowsStates.OneLine:
                    pinUp.Direction = Direction.Expanding;
                    diff = Height - 120;
                    pinUp1.Visible = true;
                    autoRestore = true;
                    break;
                case WindowsStates.MultiLine:
                    pinUp.Direction = Direction.Collapsing;
                    diff = Height - 345;
                    pinUp1.Visible = true;
                    autoRestore = true;
                    break;
                default:
                    break;
            }
            Top += diff;
            Height -= diff;
            state = newState;
            SkyBall.Space = Earth3d.MainWindow.Space;
            SkyBall.ShowSkyball = SkyBall.Space && state != WindowsStates.StatusOnly;
            SkyBall.RefreshHint();
            contextResults.Invalidate();
        }


        private void pinUp_Clicked_1(object sender, EventArgs e)
        {
            if (pinUp.Direction == Direction.Expanding)
            {
                if (state == WindowsStates.StatusOnly)
                {
                    SetWindowState(WindowsStates.OneLine);
                }
                else
                {
                    SetWindowState(WindowsStates.MultiLine);
                }
            }
            else
            {
                SetWindowState(WindowsStates.OneLine);
             
            }
            contextResults.Invalidate();
            paginator1.TotalPages = contextResults.PageCount;

            Earth3d.MainWindow.SetAppMode();
        }

        private void overview_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(constellation))
            {
                var target = Constellations.ConstellationCentroids[Constellations.Abbreviation(constellation)];
                Earth3d.MainWindow.GotoTarget(target, false, false, true);
            }

        }

        private void viewTarget_SelectionChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastLookAtMode =(int)(ImageSetType)Enum.Parse(typeof(ImageSetType), viewTarget.SelectedItem.ToString());
           // Properties.Settings.Default.LastLookAtMode = viewTarget.SelectedIndex;
            LoadImageSetList();
            contextResults.Clear();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void studyOpacity_Scroll(object sender, EventArgs e)
        {
            Earth3d.MainWindow.StudyOpacity = studyOpacity.Value;

        }

        private void ContextPanel_Paint(object sender, PaintEventArgs e)
        {
            var p = new Pen(Color.FromArgb(71, 84, 108));
            //e.Graphics.Clear(this.BackColor);
          //  e.Graphics.Clear(Color.Red);
            
            e.Graphics.DrawLine(p, 0, 0, ClientSize.Width - 1, 0);
            if (!ShowTimeline)
            {
                e.Graphics.DrawLine(p, SkyBall.Left - 1, 0, SkyBall.Left - 1, ClientRectangle.Height - 1);
            }
            p.Dispose();

        }

        private void studyOpacity_Scroll(object sender, ScrollEventArgs e)
        {
        }

        private void studyOpacity_VisibleChanged(object sender, EventArgs e)
        {
            faderText.Visible = studyOpacity.Visible;
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {

            SetOpacity();


            if ((!fader.TargetState && fader.Opacity == 0.0) || (fader.TargetState && fader.Opacity == 1.0))
            {
                if (Properties.Settings.Default.TranparentWindows)
                {
                    Visible = true;
                }
                else
                {
                    Visible = fader.TargetState;
                }
                FadeTimer.Enabled = true;
                FadeTimer.Interval = 250;
            }
        }

        public void SetOpacity()
        {
            if (Properties.Settings.Default.TranparentWindows)
            { 
                try
                {
                    Opacity = .0 + .8 * fader.Opacity;
                }
                catch
                {
                    Opacity = 1.0;
                }
            }
            else
            {
                Opacity = 1.0;
            }
        }


        readonly BlendState fader = new BlendState(false, 1000.0);
        private void TourEditTab_MouseEnter(object sender, EventArgs e)
        {
            fader.TargetState = true;
            FadeTimer.Enabled = true;
            FadeTimer.Interval = 10;
        }

        private void ConstellationLabel_Click(object sender, EventArgs e)
        {

        }

        private void contextResults_ItemContextMenu(object sender, object e)
        {
            var pntClick = Cursor.Position;
            if (e is IPlace)
            {
                Earth3d.MainWindow.ShowContextMenu((IPlace)e, Earth3d.MainWindow.PointToClient(Cursor.Position), false, true);
            }
        }

        private void pinUp1_Clicked(object sender, EventArgs e)
        {
            SetWindowState(WindowsStates.StatusOnly);
            autoRestore = false;
            Earth3d.MainWindow.SetAppMode();
        }

        private void info_Click(object sender, EventArgs e)
        {
            //if (ImageDataSetsCombo.SelectedItem is IImageSet)
            if (Earth3d.MainWindow.CurrentImageSet is IImageSet)
            {
                //ObjectProperties.ShowNofinder((IImageSet)ImageDataSetsCombo.SelectedItem, Earth3d.MainWindow.PointToClient(Cursor.Position));
                ObjectProperties.ShowNofinder(Earth3d.MainWindow.CurrentImageSet, Earth3d.MainWindow.PointToClient(Cursor.Position));

            }
        }

        private void ContextPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dontClose)
            {
                dontClose = false;
                e.Cancel = true;
            }
        }

        bool dontClose;
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.F4 | Keys.Alt))
            {
                dontClose = true;
            }
            return base.ProcessDialogKey(keyData);
        }

        private void solarSystemScaleTrackbar_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.SolarSystemScale = solarSystemScaleTrackbar.Value > 0 ? solarSystemScaleTrackbar.Value : 1;
        }

        private void contextResults_Load(object sender, EventArgs e)
        {

        }

        internal void ShowNextObject()
        {
            contextResults.ShowNext(false, false);
        }

        internal void ShowPreviousObject()
        {
            contextResults.ShowPrevious();
        }

        

        private void SkyBall_Load(object sender, EventArgs e)
        {

        }
        
        private void scaleButton_Click(object sender, EventArgs e)
        {
            
            if (Earth3d.MainWindow.StudyImageset.WcsImage is FitsImage)
            {
                var pnt = scaleButton.Location;

                pnt = PointToScreen(pnt);

                pnt.Offset(10, -18);


                Histogram.ShowHistogram(Earth3d.MainWindow.StudyImageset, true, pnt);

                //hist = new Histogram();
                //hist.Owner = Earth3d.MainWindow;
                //hist.Show();
                //hist.Image = (FitsImage)Earth3d.MainWindow.StudyImageset.WcsImage;
                //hist.Tile = (SkyImageTile)TileCache.GetTile(0, 0, 0, Earth3d.MainWindow.StudyImageset, null);
            }
        }

        private void layerToggle_MouseEnter(object sender, EventArgs e)
        {
            layerToggle.Image = layerButtonHover;
        }

        private void layerToggle_MouseLeave(object sender, EventArgs e)
        {
            layerToggle.Image = layerButton;

        }

        private void layerToggle_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.ShowLayersWindows = !Earth3d.MainWindow.ShowLayersWindows;
        }

        private void closeBox_Click(object sender, EventArgs e)
        {
            ShowTimeline = false;
        }

        private void PushPin_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FloatTimeline = true;
            ShowTimeline = false;
            KeyFramer.ShowTimeline();

        }


    }
}