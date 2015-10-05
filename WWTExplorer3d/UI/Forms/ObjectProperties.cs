using System;
using System.Drawing;
using System.Windows.Forms;
using AstroCalc;
using TerraViewer.Properties;


namespace TerraViewer
{
    public partial class ObjectProperties : Form
    {
        public ObjectProperties()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            label4.Text = Language.GetLocalizedText(264, "Names:");
            magnitudeLabel.Text = Language.GetLocalizedText(265, "Magnitude:");

            distanceLabel.Text = Language.GetLocalizedText(633, "Distance:");
            label7.Text = Language.GetLocalizedText(267, "Classification:");
            azLabel.Text = Language.GetLocalizedText(268, "Az :");
            altLabel.Text = Language.GetLocalizedText(269, "Alt : ");
            decLabel.Text = Language.GetLocalizedText(270, "Dec : ");
            raLabel.Text = Language.GetLocalizedText(271, "RA : ");
            nameValues.Text = Language.GetLocalizedText(264, "Names:");
            riseLabel.Text = Language.GetLocalizedText(273, "Rise:");
            setLabel.Text = Language.GetLocalizedText(274, "Set:");
            transitLabel.Text = Language.GetLocalizedText(275, "Transit:");
            imageCreditsText.Text = Language.GetLocalizedText(276, "Image Credits:");
            TileBarText.Text = Language.GetLocalizedText(277, "Finder Scope");
            ShowObject.Text = Language.GetLocalizedText(278, "Show object");
            CloseButton.Text = Language.GetLocalizedText(212, "Close");
            research.Text = Language.GetLocalizedText(279, "Research");
        }
        bool mouseDown;
        Point pntDown;

        IPlace target;

        public IPlace Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    UpdateTargetInfo();
                }
            }
        }

        public static void HideProperties()
        {
            if (props != null)
            {
                props.Close();
                props = null;
            }
        }

        public static bool Active
        {
            get
            {
                return props != null;
            }
        }
        static ObjectProperties props;

        public static ObjectProperties Props
        {
            get { return props; }
            set { props = value; }
        }
        public static void ShowAt(IPlace place, Point pnt)
        {
            HideProperties();

            props = new ObjectProperties();
            props.Target = place;

            props.Owner = Earth3d.MainWindow;
            props.Show();
            pnt.Offset(-300,-88);
            props.Location = pnt;
        }

        public static void ShowAt(Point pnt)
        {
            HideProperties();

            props = new ObjectProperties();
            props.Target = null;

            props.Owner = Earth3d.MainWindow;
            props.Show();
            pnt.Offset(-300,-88);
            props.Location = pnt;
            props.FindCurrentObject();
        }

        public static void ShowAt(IImageSet imageSet, Point pnt)
        {
            var tp = new TourPlace(imageSet.Name, 0, 0, Classification.Unidentified, "", imageSet.DataSetType, 360);
            tp.BackgroundImageSet = imageSet;

            ShowAt(tp, pnt);
        }

        public static void ShowNofinder(IImageSet imageSet, Point pnt)
        {
            var tp = new TourPlace(imageSet.Name, 0, 0, Classification.Unidentified, "", imageSet.DataSetType, 360);
            tp.StudyImageset = imageSet;

            ShowNofinder(tp, pnt);
        }
        
        bool showFinder = true;

        public static void ShowNofinder(IPlace place, Point pnt)
        {
            HideProperties();


            props = new ObjectProperties();
            props.Target = place;
            props.showFinder = false;

            props.Owner = Earth3d.MainWindow;
            props.TileBarText.Text = Language.GetLocalizedText(20, "Properties");
            props.closeBox.Left = 270;
            props.Width = 292;
            props.Height = 315;
            props.BackgroundImage = Resources.PropertiesBackgroundNoFinder;
            props.Show();
            props.Location = pnt;
            props.EnsureVisble();
            props.Focus();
        }

        private void EnsureVisble()
        {
            var rect = Screen.GetWorkingArea(this);

            if (Left + Width > rect.Width)
            {
                Left -= (Left + Width) - rect.Width;
            }

            if (Top + Height > (rect.Height-120))
            {
                Top -= (Top + Height) - (rect.Height-120);
            }
        }

        private void UpdateTargetInfo()
        {
            if (target != null)
            {
                constellationName.Text = Language.GetLocalizedText(280, "in ") + Constellations.FullName(target.Constellation);

                if (target.Magnitude != 0)
                {
                    magnitudeValue.Text = target.Magnitude.ToString();
                }
                else
                {
                    magnitudeValue.Text = Language.GetLocalizedText(281, "n/a");
                }


                thumbnail.Image = target.ThumbNail;
                classificationText.Text = FriendlyName( target.Classification.ToString());

                if (target.Classification == Classification.Unidentified && target.StudyImageset != null)
                {
                    if (target.StudyImageset.Projection == ProjectionType.Toast || target.StudyImageset.Projection == ProjectionType.Equirectangular || target.StudyImageset.Projection == ProjectionType.Mercator)
                    {
                        thumbnail.Image = UiTools.LoadThumbnailFromWeb(target.StudyImageset.ThumbnailUrl);

                        switch (target.StudyImageset.DataSetType)
                        {
                            case ImageSetType.Earth:
                                classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                constellationName.Text = Language.GetLocalizedText(283, "of Earth");
                                break;
                            case ImageSetType.Planet:
                                classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                constellationName.Text = Language.GetLocalizedText(284, "of Planet/Moon");
                                break;
                            case ImageSetType.Sky:
                                classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                constellationName.Text = Language.GetLocalizedText(285, "for Full Sky");
                                break;
                            case ImageSetType.Panorama:
                                classificationText.Text = Language.GetLocalizedText(286, "Panorama");
                                constellationName.Text = "";
                                break;
                            default:
                                break;
                        }

                        raLabel.Visible = false;
                        raText.Visible = false;
                        decText.Visible = false;
                        decLabel.Visible = false;
                        distanceLabel.Visible = false;
                        altText.Visible = false;
                        azText.Visible = false;
                        magnitudeLabel.Visible = false;
                        magnitudeValue.Visible = false;
                        distanceValue.Visible = false;
                        altLabel.Visible = false;
                        azLabel.Visible = false;
                        riseText.Visible = false;
                        riseLabel.Visible = false;
                        setLabel.Visible = false;
                        setText.Visible = false;
                        transitLabel.Visible = false;
                        transitText.Visible = false;
                        creditsText.Top = altText.Top+4;
                        creditsText.Height = 106;
                        creditsLink.Top = decText.Top+2;
                        imageCreditsText.Top = raText.Top;
                        research.Visible = false;
                        ShowObject.Visible = false;
                    }
                    else
                    {
                        classificationText.Text = Language.GetLocalizedText(287, "Study Imagery");
                    }
                }

                if (Earth3d.TouchKiosk)
                {
                    research.Visible = false;
                }

                if (target.StudyImageset != null)
                {
                    ShowImageCredits(target.StudyImageset);
                }
                else if (target.BackgroundImageSet != null)
                {
                    ShowImageCredits(target.BackgroundImageSet);
                }
                else
                {
                    ShowImageCredits(Earth3d.MainWindow.CurrentImageSet);
                }


                var first = true;
                nameValues.Text = "";
                foreach (var name in target.Names)
                {
                    if (!first)
                    {
                        nameValues.Text += "; ";
                    }
                    first = false;
                    nameValues.Text += name;
                }
                nameValues.Select();
                UpdateLiveValues();
            }
        }

        private void ShowImageCredits(IImageSet imageSet)
        {
            if (imageSet != null)
            {
                if (!string.IsNullOrEmpty(imageSet.CreditsText))
                {
                    creditsText.Text = imageSet.CreditsText;
                }
                else
                {
                    creditsText.Text = Language.GetLocalizedText(288, "No information available");
                }
                if (!string.IsNullOrEmpty(imageSet.CreditsUrl))
                {
                    creditsLink.Text = imageSet.CreditsUrl;
                }
                else
                {
                    creditsLink.Text = Language.GetLocalizedText(288, "No information available");
                }
            }
            else
            {
                creditsText.Text = Language.GetLocalizedText(288, "No information available");
                creditsLink.Text = Language.GetLocalizedText(288, "No information available");
            }
        }

        private string FriendlyName(string name)
        {
            for (var i = 1; i < name.Length; i++ )
            {
                if (char.IsUpper(name[i]))
                {
                    name = name.Substring(0, i) + " " + name.Substring(i);
                    i++;
                }
            }
            return name;
        }

        private void UpdateLiveValues()
        {
            if (target != null)
            {
                if (target.Type == ImageSetType.Planet || target.Type == ImageSetType.Earth)
                {
                    UpdateLiveValuesPlanet();
                    return;
                }

                raText.Text = Coordinates.FormatHMS(target.RA);
                decText.Text = Coordinates.FormatDMSWide(target.Dec);
                var altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(target.RA, target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);
                altText.Text = Coordinates.FormatDMSWide(altAz.Alt);
                azText.Text = Coordinates.FormatDMSWide(altAz.Az);

                RiseSetDetails details;
               // try
                {
                    if (target.Classification == Classification.SolarSystem)
                    {

                        var jNow = ((int)SpaceTimeController.JNow + .5);
                        var p1 = Planets.GetPlanetLocation(target.Name, jNow - 1);
                        var p2 = Planets.GetPlanetLocation(target.Name, jNow);
                        var p3 = Planets.GetPlanetLocation(target.Name, jNow + 1);

                        var type = 0;
                        switch (target.Name)
                        {
                            case "Sun":
                                type = 1;
                                break;
                            case "Moon":
                                type = 2;
                                break;
                            default:
                                type = 0;
                                break;
                        }
                        details = AstroCalc.AstroCalc.GetRiseTrinsitSet(jNow, SpaceTimeController.Location.Lat, -SpaceTimeController.Location.Lng, p1.RA, p1.Dec, p2.RA, p2.Dec, p3.RA, p3.Dec, type);
                    }
                    else
                    {
                        details = AstroCalc.AstroCalc.GetRiseTrinsitSet(((int)SpaceTimeController.JNow) + .5, SpaceTimeController.Location.Lat, -SpaceTimeController.Location.Lng, target.RA, Target.Dec, target.RA, Target.Dec, target.RA, Target.Dec, 0);
                    }


                    if (details.bValid)
                    {
                        riseText.Text = UiTools.FormatDecimalHours(details.Rise);
                        transitText.Text = UiTools.FormatDecimalHours(details.Transit);
                        setText.Text = UiTools.FormatDecimalHours(details.Set);
                    }
                    else
                    {
                        if (details.bNeverRises)
                        {
                            riseText.Text = transitText.Text = setText.Text = Language.GetLocalizedText(934, "Never Rises");
                        }
                        else
                        {
                            riseText.Text = transitText.Text = setText.Text = Language.GetLocalizedText(935, "Never Sets");
                        }
                    }
                    if (target.Distance != 0)
                    {
                        distanceValue.Text = UiTools.FormatDistance(target.Distance);
                    }
                    else
                    {
                        distanceValue.Text = Language.GetLocalizedText(281, "n/a");

                    }
                }
                //catch
                {
                }
            }

        }

        private void UpdateLiveValuesPlanet()
        {
            raText.Text = Coordinates.FormatDMS(target.Lng);
            decText.Text = Coordinates.FormatDMSWide(target.Lat);
            raLabel.Text = "Lng:";
            decLabel.Text = "Lat:";
            var altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(target.RA, target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);
            altText.Hide();
            azText.Hide();
            altLabel.Hide();
            azLabel.Hide();
            riseLabel.Hide();
            setLabel.Hide();
            transitLabel.Hide();
            transitText.Hide();
            magnitudeLabel.Hide();
            distanceLabel.Hide();
            magnitudeValue.Hide();
            riseText.Hide();
            setText.Hide();
            transitText.Hide();
            distanceValue.Hide();

        }

        private void ObjectProperties_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            pntDown = PointToScreen(e.Location);
        }

        bool moved;

        private void ObjectProperties_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                var loc = PointToScreen(e.Location);
                var move = new Point(loc.X - pntDown.X, loc.Y - pntDown.Y);

                Top += move.Y;
                Left += move.X;
                pntDown = loc;
                moved = true;
            }
        }

        private void ObjectProperties_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;

        }
        enum CrossHairPosition { UpperLeft, UpperRight, LowerLeft, LowerRight }

        CrossHairPosition crossharPosition = CrossHairPosition.UpperRight;

        private void ObjectProperties_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //Bitmap bmp = new Bitmap(this.BackgroundImage.Width,this.BackgroundImage.Height);
            //using (Graphics g = Graphics.FromImage(bmp))
            //{
            //    g.DrawImage(this.BackgroundImage, 0, 0);
            //}
            //bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);

            //this.BackgroundImage = bmp;

            //if (crossharPosition == CrossHairPosition.UpperLeft)
            //{
            //    this.Left -= 213;
            //    crossharPosition = CrossHairPosition.UpperRight;
            //}
            //else
            //{
            //    this.Left += 213;
            //    crossharPosition = CrossHairPosition.UpperLeft;
            //}
            Close();

        }

        const int smallJump = 10;
        const int bigJump = 50;
        double viewRA;
        double viewDec;
        double zoom;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow == null)
            {
                return;
            }

            if (showFinder)
            {
                if (Math.Abs(viewDec -  Earth3d.MainWindow.ViewLat) > .00001
                    || Math.Abs(viewRA - Earth3d.MainWindow.RA) > .00001
                    || Math.Abs(zoom - Earth3d.MainWindow.ZoomFactor) > .00001 )
                {
                    moved = true;
                    viewDec = Earth3d.MainWindow.ViewLat;
                    viewRA = Earth3d.MainWindow.RA;
                    zoom = Earth3d.MainWindow.ZoomFactor;
                }

                if (moved)
                {
                    moved = false;
                    FindCurrentObject();
                }

                if (mouseDown)
                {
                    if (Left - Earth3d.MainWindow.Left < 50)
                    {
                        Earth3d.MainWindow.MoveView(-bigJump, 0, false);
                    }
                    else if (Left - Earth3d.MainWindow.Left < 120)
                    {
                        Earth3d.MainWindow.MoveView(-smallJump, 0, false);
                    }

                    if (Earth3d.MainWindow.Right - Right < 50)
                    {
                        Earth3d.MainWindow.MoveView(bigJump, 0, false);
                    }
                    else if (Earth3d.MainWindow.Right - Right < 120)
                    {
                        Earth3d.MainWindow.MoveView(smallJump, 0, false);
                    }

                    if (Earth3d.MainWindow.Bottom - Bottom < -25)
                    {
                        Earth3d.MainWindow.MoveView(0, -bigJump, false);
                    }
                    else if (Earth3d.MainWindow.Bottom - Bottom < 45)
                    {
                        Earth3d.MainWindow.MoveView(0, -smallJump, false);
                    }

                    if (Top - Earth3d.MainWindow.Top < 50)
                    {
                        Earth3d.MainWindow.MoveView(0, bigJump, false);
                    }
                    else if (Top - Earth3d.MainWindow.Top < 120)
                    {
                        Earth3d.MainWindow.MoveView(0, smallJump, false);
                    }
                }
            }

            UpdateLiveValues();
        }
        IPlace noPlaceDefault;
        private void FindCurrentObject()
        {
            var loc = Earth3d.MainWindow.RenderWindow.PointToClient(PointToScreen(new Point(300, 88)));
            IPlace closetPlace = null;
            var result = new Coordinates(0,0);

            if (Earth3d.MainWindow.SolarSystemMode)
            {
                var pt = loc;
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                var rect = Earth3d.MainWindow.RenderWindow.ClientRectangle;

                Earth3d.MainWindow.TransformStarPickPointToWorldSpace(pt, rect.Width, rect.Height, out PickRayOrig, out PickRayDir);
                var temp = new Vector3d(PickRayOrig);
                temp.Subtract(Earth3d.MainWindow.viewCamera.ViewTarget);

                //closetPlace = Grids.FindClosestObject(temp , new Vector3d(PickRayDir));
                CallFindClosestObject(temp, new Vector3d(PickRayDir));

            }
            else
            {

                // TODO fix this for earth, plantes, panoramas
                result = Earth3d.MainWindow.GetCoordinatesForScreenPoint(loc.X, loc.Y);
                var constellation = Earth3d.MainWindow.ConstellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                //Place[] resultList = ContextSearch.FindClosestMatches(constellation, result.RA, result.Dec, ZoomFactor / 600, 5);
                closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, Earth3d.MainWindow.DegreesPerPixel * 80);

                if (closetPlace == null)
                {
                   // closetPlace = Grids.FindClosestMatch(constellation, result.RA, result.Dec, Earth3d.MainWindow.DegreesPerPixel * 80);
                    CallFindClosestMatch(constellation, result.RA, result.Dec, Earth3d.MainWindow.DegreesPerPixel * 80);
                    noPlaceDefault = new TourPlace(Language.GetLocalizedText(90, "No Object"), result.Dec, result.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
                    //Earth3d.MainWindow.SetLabelText(null, false);
                    return;
                }
                Earth3d.MainWindow.SetLabelText(closetPlace, false);
                Target = closetPlace;
            }



        }

        private void CallFindClosestMatch(string constellationID, double ra, double dec, double maxRadius)
        {
            if (invokeFindClosestMatch == null)
            {
                invokeFindClosestMatch = Grids.FindClosestMatch;
            }

            invokeFindClosestMatch.BeginInvoke(constellationID, ra, dec, maxRadius, CallBack2, null);
        }

        private FindClosestObjectDelegate invokeFindClosestObject;
        private FindClosestMatchDelegate invokeFindClosestMatch;

        // notice that the delegate is private,
        // only the command can use it.
        private delegate IPlace FindClosestObjectDelegate(Vector3d orig, Vector3d ray);
        private delegate IPlace FindClosestMatchDelegate(string constellationID, double ra, double dec, double maxRadius);

        private void CallFindClosestObject(Vector3d orig, Vector3d ray)
        {
            if (invokeFindClosestObject == null)
            {
                invokeFindClosestObject = Grids.FindClosestObject;
            }

            invokeFindClosestObject.BeginInvoke(orig, ray, CallBack, null);
        }

        private void CallBack(IAsyncResult ar)
        {
            var place = invokeFindClosestObject.EndInvoke(ar);

            if (place == null)
            {
                return;
            }

            MethodInvoker updatePlace = delegate
            {
                Target = place;
                Earth3d.MainWindow.SetLabelText(place, false);
            };

            if (InvokeRequired)
            {
                try
                {
                    Invoke(updatePlace);
                }
                catch
                {
                }
            }
            else
            {
                updatePlace();
            }
        }
        private void CallBack2(IAsyncResult ar)
        {
            var place = invokeFindClosestMatch.EndInvoke(ar);


            MethodInvoker updatePlace = delegate
            {
                if (place == null)
                {
                    Target = noPlaceDefault;
                }
                else
                {
                    Target = place;
                }        
                Earth3d.MainWindow.SetLabelText(place, false);
            };

            if (InvokeRequired)
            {
                try
                {
                    Invoke(updatePlace);
                }
                catch
                {
                }
            }
            else
            {
                updatePlace();
            }
        }

        private void closeBox_MouseEnter(object sender, EventArgs e)
        {
            closeBox.Image = Resources.CloseHover;
        }

        private void closeBox_MouseLeave(object sender, EventArgs e)
        {
            closeBox.Image = Resources.CloseRest;

        }

        private void closeBox_MouseDown(object sender, MouseEventArgs e)
        {
            closeBox.Image = Resources.ClosePush;

        }

        private void closeBox_MouseUp(object sender, MouseEventArgs e)
        {
            closeBox.Image = Resources.CloseHover;
            Close();

        }

        private void ObjectProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }

        private void thumbnail_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.GotoTarget(target, false, false, true);
            Close();
        }

        BlendState fadein;
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

        private void ObjectProperties_Load(object sender, EventArgs e)
        {
            fadein = new BlendState(false, 500);
            fadein.TargetState = true;


        }

        private void Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void research_Click(object sender, EventArgs e)
        {
            var pntShow = PointToScreen(research.Location);
            pntShow.Offset(5, research.Height);
            Earth3d.MainWindow.ShowContextMenu(target, Earth3d.MainWindow.PointToClient(pntShow), false, true);
        }


        private void ShowObject_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.GotoTarget(target, false, false, true);
            Close();
        }

        private void creditsLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebWindow.OpenUrl(creditsLink.Text, false);
        }

        private void ObjectProperties_FormClosed(object sender, FormClosedEventArgs e)
        {
            Earth3d.MainWindow.SetLabelText(null, false);
            props = null;
        }


    }
}