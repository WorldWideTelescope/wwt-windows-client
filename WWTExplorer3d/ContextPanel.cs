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
    public partial class ContextPanel : Form
    {
        public ContextPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            InitializeComponent();
            SetUiStrings();
            FilterCombo.Tag = Classification.Unfiltered;
            
        }
        Bitmap layerButton = global::TerraViewer.Properties.Resources.layersButton;
        Bitmap layerButtonHover = global::TerraViewer.Properties.Resources.layersButtonHover;

        private void SetUiStrings()
        {
            this.toolTips.SetToolTip(this.levelLabel, Language.GetLocalizedText(171, "Filed of view height in degrees : minutes : seconds of arc"));
            this.toolTips.SetToolTip(this.ConstellationLabel, Language.GetLocalizedText(172, "Name of the constellation that the view center is in."));
            this.toolTips.SetToolTip(this.queueProgressBar, Language.GetLocalizedText(173, "Download Progress"));
            this.toolTips.SetToolTip(this.studyOpacity, Language.GetLocalizedText(174, "Crossfades background and foreground imagery"));
            this.toolTips.SetToolTip(this.viewTarget, Language.GetLocalizedText(175, "Select the type of view, Sky, Earth, etc."));
            this.toolTips.SetToolTip(this.ImageDataSetsCombo, Language.GetLocalizedText(176, "Select the imagery to display"));
            this.toolTips.SetToolTip(this.FilterCombo, Language.GetLocalizedText(177, "Filters context results"));
            this.toolTips.SetToolTip(this.contextResults, Language.GetLocalizedText(178, "Context Search - Shows interesting places in the current view."));
            this.toolTips.SetToolTip(this.overview, Language.GetLocalizedText(179, "Constellation Overview"));
            this.toolTips.SetToolTip(this.SkyBall, Language.GetLocalizedText(180, "Shows field of view relative to the celestial sphere"));
            this.toolTips.SetToolTip(this.layerToggle, Language.GetLocalizedText(953, "Show/Hide Layer Manager"));
            this.contextResults.AddText = Language.GetLocalizedText(161, "Add New Item");
            this.contextResults.EmptyAddText = Language.GetLocalizedText(162, "No Results");
            this.faderText.Text = Language.GetLocalizedText(181, "Image Crossfade");
            this.trackingText.Text = Language.GetLocalizedText(182, "Tracking");
            this.trackingLabel.Text = Language.GetLocalizedText(182, "Tracking");
            this.label1.Text = Language.GetLocalizedText(155, "Imagery");
            this.label2.Text = Language.GetLocalizedText(183, "Look At");
            this.label3.Text = Language.GetLocalizedText(184, "Context Search Filter");
            this.scaleText.Text = Language.GetLocalizedText(578, "Planet Size");
            this.bigSizeLabel.Text = Language.GetLocalizedText(579, "Large");
            this.actualSizeLabel.Text = Language.GetLocalizedText(580, "Actual");
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

        static bool contextAreaChanged = false;

        static public bool ContextAreaChanged { get => contextAreaChanged; set => contextAreaChanged = value; }

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
                else
                {
                    return "00:00:00";
                }
            }
        }

        bool sandbox = false;

        public bool Sandbox
        {
            get { return sandbox; }
            set { sandbox = value; }
        }

        double distance = 1;
        bool lastSandbox = false;

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
        
        Coordinates[] cornersLast = null;

        public void SetViewRect(Coordinates[] corners)
        {
            if (corners.Length != 4)
            {
                return;
            }
            bool change = false;
            if (this.cornersLast != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (this.cornersLast[i] != corners[i])
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
            this.cornersLast = corners;
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

        static public int SearchGeneration = 0;

        static string lastTracking = "";
        bool autoRestore = false;
        private void searchTimer_Tick(object sender, EventArgs e)
        {
            bool space = RenderEngine.Engine.Space;
            studyOpacity.Value = (int)RenderEngine.Engine.StudyOpacity;
            UpdateVisibility(space);

            Rectangle rect = this.RectangleToScreen(this.ClientRectangle);
            bool inside = rect.Contains(Cursor.Position) || Earth3d.TouchKiosk || !(TourPlayer.Playing || Earth3d.FullScreen || Properties.Settings.Default.AutoHideContext);

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
                IImageSet im = contextResults.Items[0] as IImageSet;
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


            foreach (object o in Earth3d.MainWindow.ExplorerRoot.Children)
            {
                if (o is Folder)
                {
                    Folder f = (Folder)o;
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
            foreach (object o1 in folder.Children)
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
                this.levelLabel.Visible = false;
                this.ConstellationLabel.Visible = false;
                this.queueProgressBar.Visible = false;
                this.label1.Visible = false;
                this.label2.Visible = false;
                this.label3.Visible = false;
                this.studyOpacity.Visible = false;
                this.viewTarget.Visible = false;
                this.ImageDataSetsCombo.Visible = false;
                this.FilterCombo.Visible = false;
                this.contextResults.Visible = false;
                this.paginator1.Visible = false;
                this.overview.Visible = false;
                this.SkyBall.Visible = false;
                this.layerToggle.Visible = false;
                this.faderText.Visible = false;             
                this.trackingText.Visible = false;
                this.trackingLabel.Visible = false;
                this.actualSizeLabel.Visible = false;
                this.bigSizeLabel.Visible = false;
                this.scaleText.Visible = false;
                this.scaleLabel.Visible = false;
                this.scaleButton.Visible = false;
                this.solarSystemScaleTrackbar.Visible = false;
                this.trackingTarget.Visible = false;
                this.info.Visible = false;
            }
            else
            {
                this.PushPin.Visible = false;
                this.closeBox.Visible = false;
                this.levelLabel.Visible = true;
                this.ConstellationLabel.Visible = true;
                this.queueProgressBar.Visible = true;
                this.label1.Visible = true;
                this.label2.Visible = true;
                this.label3.Visible = true;
                this.viewTarget.Visible = true;
                this.ImageDataSetsCombo.Visible = true;
                this.FilterCombo.Visible = true;
                this.paginator1.Visible = true;
                this.SkyBall.Visible = true;
                this.layerToggle.Visible = true;
                this.trackingText.Visible = true;
                this.trackingTarget.Visible = true;
                this.info.Visible = true;



                Timeline.Visible = false;



                bool solarSystem = Earth3d.MainWindow.SolarSystemMode;
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

                if (RenderEngine.Engine.Tracking && (RenderEngine.Engine.Space))
                {
                    trackingLabel.Visible = true;
                    if (lastTracking != RenderEngine.Engine.TrackingObject.Name)
                    {
                        trackingText.Text = RenderEngine.Engine.TrackingObject.Name.Substring(0, Math.Min(40, RenderEngine.Engine.TrackingObject.Name.Length));
                        lastTracking = RenderEngine.Engine.TrackingObject.Name;
                    }
                }
                else if (Earth3d.MainWindow.SolarSystemMode)
                {
                    trackingLabel.Visible = true;
                    if (RenderEngine.Engine.TrackingObject != null && lastTracking != RenderEngine.Engine.TrackingObject.Name)
                    {
                        trackingText.Text = RenderEngine.Engine.TrackingObject.Name;
                        lastTracking = trackingText.Text;
                        trackingTarget.Items.Clear();
                        trackingTarget.Items.Add(RenderEngine.Engine.TrackingObject);
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
                float searchDistance = (float)Math.Min((0.4617486132350 * ((4.0 * (RenderEngine.Engine.ZoomFactor / 180)) + 0.000001)), 1.4142135623730950488016887242097);
                IPlace[] results = ContextSearch.FindConteallationObjectsInCone(planet, RenderEngine.Engine.ViewLong/15, RenderEngine.Engine.ViewLat, searchDistance, (Classification)FilterCombo.Tag);
                
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

                Vector3d cornerUl = Coordinates.RADecTo3d(cornersLast[0].RA, cornersLast[0].Dec,1);
                Vector3d cornerLR = Coordinates.RADecTo3d(cornersLast[2].RA, cornersLast[2].Dec,1);
                Vector3d dist = Vector3d.Subtract(cornerLR, cornerUl);
                IPlace[] results = ContextSearch.FindConteallationObjectsInCone("SolarSystem", RenderEngine.Engine.RA, RenderEngine.Engine.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                results = ContextSearch.FindConteallationObjectsInCone("Community", RenderEngine.Engine.RA, RenderEngine.Engine.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }

                results = ContextSearch.FindConteallationObjectsInCone(Constellations.Abbreviation(constellation), RenderEngine.Engine.RA, RenderEngine.Engine.Dec, (float)dist.Length() / 2.0f, (Classification)FilterCombo.Tag);
                if (results != null)
                {
                    contextResults.AddRange(results);
                }
                SearchGeneration++;
                contextResults.Invalidate();
            }
        }

        private void RunQuerySolarSystem()
        {
            if (contextResults.Items.Count > 0)
            {
                IPlace pl = contextResults.Items[0] as IPlace;
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

            IPlace[] results = ContextSearch.FindAllObjects("SolarSystem", Classification.SolarSystem);
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

            int id = Properties.Settings.Default.StartUpLookAt;

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
            Earth3d.MainWindow.ImageSetChanged += new EventHandler(MainWindow_ImageSetChanged);
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

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(doIt);
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
            int index = 0;
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
            int index = 0;
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
            foreach(object o in ImageDataSetsCombo.Items)
            {
                if (o is IImageSet)
                {
                    IImageSet imageset = (IImageSet)o;
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
        bool deferUpdate = false;
        private void LoadImageSetList()
        {
            if (viewTarget.SelectedItem == null)
            {
                return;
            }
            ImageSetType target = (ImageSetType)Enum.Parse(typeof(ImageSetType),viewTarget.SelectedItem.ToString());
            ImageDataSetsCombo.Items.Clear();
            foreach (IImageSet set in RenderEngine.ImageSets)
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
            IPlace place = e as IPlace;

            if (place != null)
            {
                Place p = e as Place;
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
                    RenderEngine.Engine.GotoTarget((IPlace)e, false, false, true);
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
                RenderEngine.Engine.GotoTarget((IPlace)e, false, true, true);
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
                IPlace p = (IPlace)e;
                RenderEngine.Engine.SetStudyImageset(p.StudyImageset, p.BackgroundImageSet);

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
                        RenderEngine.Engine.SetViewMode((IImageSet)ImageDataSetsCombo.SelectedItem);
                    }
                }
                else
                {
                    if (ImageDataSetsCombo.SelectedItem.GetType() == typeof(String) && (String)ImageDataSetsCombo.SelectedItem == Language.GetLocalizedText(130, "Browse..."))
                    {
                        OpenFileDialog openFile = new OpenFileDialog();
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
                            RenderEngine.Engine.SetViewMode(new ImageSetHelper(openFile.FileName, openFile.FileName, (ImageSetType)Enum.Parse(typeof(ImageSetType), (string)viewTarget.SelectedItem), BandPass.Visible, ProjectionType.Spherical, 999, 0, 0, 256, 256, ".tif", false, "", 0, 0, 0, false, "", false, false, 1, 0, 0, "", "", "", "", 0, ""));
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
                this.Focus();
            }
            if (e is IPlace || e is IImageSet)
            {
                IImageSet imageset = null;
                if (e is IPlace)
                {
                    IPlace p = (IPlace)e;
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
                if (e != null)
                {
                    toolTips.SetToolTip(contextResults, ((IThumbnail)e).Name);
                }
                Earth3d.MainWindow.SetLabelText(null, false);
                RenderEngine.Engine.PreviewBlend.TargetState = false;

            }

  
        }

        enum WindowsStates { StatusOnly, OneLine, MultiLine };

        WindowsStates state = WindowsStates.OneLine;

        private void SetWindowState(WindowsStates newState)
        {
            int diff=0;

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
            SkyBall.Space = RenderEngine.Engine.Space;
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
                IPlace target = Constellations.ConstellationCentroids[Constellations.Abbreviation(constellation)];
                RenderEngine.Engine.GotoTarget(target, false, false, true);
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
            RenderEngine.Engine.StudyOpacity = studyOpacity.Value;

        }

        private void ContextPanel_Paint(object sender, PaintEventArgs e)
        {
            Pen p = new Pen(Color.FromArgb(71, 84, 108));
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
                    this.Visible = true;
                }
                else
                {
                    this.Visible = fader.TargetState;
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


        BlendState fader = new BlendState(false, 1000.0);
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
            Point pntClick = Cursor.Position;
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
                ObjectProperties.ShowNofinder((IImageSet)Earth3d.MainWindow.CurrentImageSet, Earth3d.MainWindow.PointToClient(Cursor.Position));

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

        bool dontClose = false;
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
                Point pnt = scaleButton.Location;

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