using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


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
            this.label4.Text = Language.GetLocalizedText(264, "Names:");
            this.magnitudeLabel.Text = Language.GetLocalizedText(265, "Magnitude:");

            this.distanceLabel.Text = Language.GetLocalizedText(633, "Distance:");
            this.label7.Text = Language.GetLocalizedText(267, "Classification:");
            this.azLabel.Text = Language.GetLocalizedText(268, "Az :");
            this.altLabel.Text = Language.GetLocalizedText(269, "Alt : ");
            this.decLabel.Text = Language.GetLocalizedText(270, "Dec : ");
            this.raLabel.Text = Language.GetLocalizedText(271, "RA : ");
            this.nameValues.Text = Language.GetLocalizedText(264, "Names:");
            this.riseLabel.Text = Language.GetLocalizedText(273, "Rise:");
            this.setLabel.Text = Language.GetLocalizedText(274, "Set:");
            this.transitLabel.Text = Language.GetLocalizedText(275, "Transit:");
            this.imageCreditsText.Text = Language.GetLocalizedText(276, "Image Credits:");
            this.TileBarText.Text = Language.GetLocalizedText(277, "Finder Scope");
            this.ShowObject.Text = Language.GetLocalizedText(278, "Show object");
            this.CloseButton.Text = Language.GetLocalizedText(212, "Close");
            this.research.Text = Language.GetLocalizedText(279, "Research");
        }
        bool mouseDown = false;
        Point pntDown;

        IPlace target = null;

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
        static ObjectProperties props = null;

        public static ObjectProperties Props
        {
            get { return ObjectProperties.props; }
            set { ObjectProperties.props = value; }
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
            TourPlace tp = new TourPlace(imageSet.Name, 0, 0, Classification.Unidentified, "", imageSet.DataSetType, 360);
            tp.BackgroundImageSet = imageSet;

            ShowAt(tp, pnt);
        }

        public static void ShowNofinder(IImageSet imageSet, Point pnt)
        {
            TourPlace tp = new TourPlace(imageSet.Name, 0, 0, Classification.Unidentified, "", imageSet.DataSetType, 360);
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
            props.BackgroundImage = Properties.Resources.PropertiesBackgroundNoFinder;
            props.Show();
            props.Location = pnt;
            props.EnsureVisble();
            props.Focus();
        }

        private void EnsureVisble()
        {
            Rectangle rect = Screen.GetWorkingArea(this);

            if (this.Left + this.Width > rect.Width)
            {
                this.Left -= (this.Left + this.Width) - rect.Width;
            }

            if (this.Top + this.Height > (rect.Height-120))
            {
                this.Top -= (this.Top + this.Height) - (rect.Height-120);
            }
        }

        private void UpdateTargetInfo()
        {
            if (target != null)
            {
                this.constellationName.Text = Language.GetLocalizedText(280, "in ") + Constellations.FullName(target.Constellation);

                if (target.Magnitude != 0)
                {
                    this.magnitudeValue.Text = target.Magnitude.ToString();
                }
                else
                {
                    this.magnitudeValue.Text = Language.GetLocalizedText(281, "n/a");
                }


                this.thumbnail.Image = target.ThumbNail;
                this.classificationText.Text = FriendlyName( target.Classification.ToString());

                if (target.Classification == Classification.Unidentified && target.StudyImageset != null)
                {
                    if (target.StudyImageset.Projection == ProjectionType.Toast || target.StudyImageset.Projection == ProjectionType.Equirectangular || target.StudyImageset.Projection == ProjectionType.Mercator)
                    {
                        this.thumbnail.Image = UiTools.LoadThumbnailFromWeb(target.StudyImageset.ThumbnailUrl);

                        switch (target.StudyImageset.DataSetType)
                        {
                            case ImageSetType.Earth:
                                this.classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                this.constellationName.Text = Language.GetLocalizedText(283, "of Earth");
                                break;
                            case ImageSetType.Planet:
                                this.classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                this.constellationName.Text = Language.GetLocalizedText(284, "of Planet/Moon");
                                break;
                            case ImageSetType.Sky:
                                this.classificationText.Text = Language.GetLocalizedText(282, "Survey Imagery");
                                this.constellationName.Text = Language.GetLocalizedText(285, "for Full Sky");
                                break;
                            case ImageSetType.Panorama:
                                this.classificationText.Text = Language.GetLocalizedText(286, "Panorama");
                                this.constellationName.Text = "";
                                break;
                            default:
                                break;
                        }

                        this.raLabel.Visible = false;
                        this.raText.Visible = false;
                        this.decText.Visible = false;
                        this.decLabel.Visible = false;
                        this.distanceLabel.Visible = false;
                        this.altText.Visible = false;
                        this.azText.Visible = false;
                        this.magnitudeLabel.Visible = false;
                        this.magnitudeValue.Visible = false;
                        this.distanceValue.Visible = false;
                        this.altLabel.Visible = false;
                        this.azLabel.Visible = false;
                        this.riseText.Visible = false;
                        this.riseLabel.Visible = false;
                        this.setLabel.Visible = false;
                        this.setText.Visible = false;
                        this.transitLabel.Visible = false;
                        this.transitText.Visible = false;
                        this.creditsText.Top = altText.Top+4;
                        this.creditsText.Height = 106;
                        this.creditsLink.Top = decText.Top+2;
                        this.imageCreditsText.Top = raText.Top;
                        this.research.Visible = false;
                        this.ShowObject.Visible = false;
                    }
                    else
                    {
                        this.classificationText.Text = Language.GetLocalizedText(287, "Study Imagery");
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


                bool first = true;
                nameValues.Text = "";
                foreach (string name in target.Names)
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
                    this.creditsText.Text = imageSet.CreditsText;
                }
                else
                {
                    this.creditsText.Text = Language.GetLocalizedText(288, "No information available");
                }
                if (!string.IsNullOrEmpty(imageSet.CreditsUrl))
                {
                    this.creditsLink.Text = imageSet.CreditsUrl;
                }
                else
                {
                    this.creditsLink.Text = Language.GetLocalizedText(288, "No information available");
                }
            }
            else
            {
                this.creditsText.Text = Language.GetLocalizedText(288, "No information available");
                this.creditsLink.Text = Language.GetLocalizedText(288, "No information available");
            }
        }

        private string FriendlyName(string name)
        {
            for (int i = 1; i < name.Length; i++ )
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
                Coordinates altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(target.RA, target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);
                altText.Text = Coordinates.FormatDMSWide(altAz.Alt);
                azText.Text = Coordinates.FormatDMSWide(altAz.Az);

                AstroCalc.RiseSetDetails details;
               // try
                {
                    if (target.Classification == Classification.SolarSystem)
                    {

                        double jNow = ((int)((int)SpaceTimeController.JNow) + .5);
                        AstroCalc.AstroRaDec p1 = Planets.GetPlanetLocation(target.Name, jNow - 1);
                        AstroCalc.AstroRaDec p2 = Planets.GetPlanetLocation(target.Name, jNow);
                        AstroCalc.AstroRaDec p3 = Planets.GetPlanetLocation(target.Name, jNow + 1);

                        int type = 0;
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


                    riseText.Text = details.bValidRise ? UiTools.FormatDecimalHours(details.Rise) : Language.GetLocalizedText(934, "Never Rises");
                    transitText.Text = details.bValidTransit ? UiTools.FormatDecimalHours(details.Transit) : Language.GetLocalizedText(934, "Never Rises");
                    setText.Text = details.bValidSet ? UiTools.FormatDecimalHours(details.Set) : Language.GetLocalizedText(935, "Never Sets");
                    
                    if (target.Distance != 0)
                    {
                        this.distanceValue.Text = UiTools.FormatDistance(target.Distance);
                    }
                    else
                    {
                        this.distanceValue.Text = Language.GetLocalizedText(281, "n/a");

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
            Coordinates altAz = Coordinates.EquitorialToHorizon(Coordinates.FromRaDec(target.RA, target.Dec), SpaceTimeController.Location, SpaceTimeController.Now);
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
            pntDown = this.PointToScreen(e.Location);
        }

        bool moved = false;

        private void ObjectProperties_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                Point loc = this.PointToScreen(e.Location);
                Point move = new Point(loc.X - pntDown.X, loc.Y - pntDown.Y);

                this.Top += move.Y;
                this.Left += move.X;
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
        double viewRA = 0;
        double viewDec = 0;
        double zoom = 0;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (Earth3d.MainWindow == null)
            {
                return;
            }

            if (showFinder)
            {
                if (Math.Abs(viewDec -  RenderEngine.Engine.ViewLat) > .00001
                    || Math.Abs(viewRA - RenderEngine.Engine.RA) > .00001
                    || Math.Abs(zoom - RenderEngine.Engine.ZoomFactor) > .00001 )
                {
                    moved = true;
                    viewDec = RenderEngine.Engine.ViewLat;
                    viewRA = RenderEngine.Engine.RA;
                    zoom = RenderEngine.Engine.ZoomFactor;
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
        IPlace noPlaceDefault = null;
        private void FindCurrentObject()
        {
            Point loc = Earth3d.MainWindow.RenderWindow.PointToClient(this.PointToScreen(new Point(300, 88)));
            IPlace closetPlace = null;
            Coordinates result = new Coordinates(0,0);

            if (Earth3d.MainWindow.SolarSystemMode)
            {
                Vector2d pt = new Vector2d(loc.X, loc.Y);
                Vector3d PickRayOrig;
                Vector3d PickRayDir;
                Rectangle rect = Earth3d.MainWindow.RenderWindow.ClientRectangle;

                RenderEngine.Engine.TransformStarPickPointToWorldSpace(pt, rect.Width, rect.Height, out PickRayOrig, out PickRayDir);
                Vector3d temp = new Vector3d(PickRayOrig);
                temp.Subtract(RenderEngine.Engine.viewCamera.ViewTarget);

                //closetPlace = Grids.FindClosestObject(temp , new Vector3d(PickRayDir));
                CallFindClosestObject(temp, new Vector3d(PickRayDir));

            }
            else
            {

                // TODO fix this for earth, plantes, panoramas
                result = Earth3d.MainWindow.GetCoordinatesForScreenPoint(loc.X, loc.Y);
                string constellation = Earth3d.MainWindow.ConstellationCheck.FindConstellationForPoint(result.RA, result.Dec);
                //Place[] resultList = ContextSearch.FindClosestMatches(constellation, result.RA, result.Dec, ZoomFactor / 600, 5);
                closetPlace = ContextSearch.FindClosestMatch(constellation, result.RA, result.Dec, RenderEngine.Engine.DegreesPerPixel * 80);

                if (closetPlace == null)
                {
                   // closetPlace = Grids.FindClosestMatch(constellation, result.RA, result.Dec, Earth3d.MainWindow.DegreesPerPixel * 80);
                    CallFindClosestMatch(constellation, result.RA, result.Dec, RenderEngine.Engine.DegreesPerPixel * 80);
                    noPlaceDefault = new TourPlace(Language.GetLocalizedText(90, "No Object"), result.Dec, result.RA, Classification.Unidentified, constellation, ImageSetType.Sky, -1);
                    //Earth3d.MainWindow.SetLabelText(null, false);
                    return;
                }
                else
                {
                    Earth3d.MainWindow.SetLabelText(closetPlace, false);
                }
                Target = closetPlace;
            }



        }

        private void CallFindClosestMatch(string constellationID, double ra, double dec, double maxRadius)
        {
            if (invokeFindClosestMatch == null)
            {
                invokeFindClosestMatch = new FindClosestMatchDelegate(Grids.FindClosestMatch);
            }

            invokeFindClosestMatch.BeginInvoke(constellationID, ra, dec, maxRadius, this.CallBack2, null);
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
                invokeFindClosestObject = new FindClosestObjectDelegate(Grids.FindClosestObject);
            }

            invokeFindClosestObject.BeginInvoke(orig, ray, this.CallBack, null);
        }

        private void CallBack(IAsyncResult ar)
        {
            IPlace place = invokeFindClosestObject.EndInvoke(ar);

            if (place == null)
            {
                return;
            }

            MethodInvoker updatePlace = delegate
            {
                Target = place;
                Earth3d.MainWindow.SetLabelText(place, false);
            };

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(updatePlace);
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
            IPlace place = invokeFindClosestMatch.EndInvoke(ar);


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

            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(updatePlace);
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
            closeBox.Image = Properties.Resources.CloseHover;
        }

        private void closeBox_MouseLeave(object sender, EventArgs e)
        {
            closeBox.Image = Properties.Resources.CloseRest;

        }

        private void closeBox_MouseDown(object sender, MouseEventArgs e)
        {
            closeBox.Image = Properties.Resources.ClosePush;

        }

        private void closeBox_MouseUp(object sender, MouseEventArgs e)
        {
            closeBox.Image = Properties.Resources.CloseHover;
            this.Close();

        }

        private void ObjectProperties_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void thumbnail_Click(object sender, EventArgs e)
        {
            RenderEngine.Engine.GotoTarget(target, false, false, true);
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
            this.Close();
        }

        private void research_Click(object sender, EventArgs e)
        {
            Point pntShow = this.PointToScreen(research.Location);
            pntShow.Offset(5, research.Height);
            Earth3d.MainWindow.ShowContextMenu(target, Earth3d.MainWindow.PointToClient(pntShow), false, true);
        }


        private void ShowObject_Click(object sender, EventArgs e)
        {
            RenderEngine.Engine.GotoTarget(target, false, false, true);
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