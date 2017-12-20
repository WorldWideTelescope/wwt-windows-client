using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using MicrosoftInternal.AdvancedCollections;
using TerraViewer.org.worldwidetelescope.www;
using System.Threading;
namespace TerraViewer
{
    public partial class Search : TabForm
    {

        public Search()
        {
            InitializeComponent();
            SetUiStrings();

        }

        private void SetUiStrings()
        {
            this.searchText.Text = Language.GetLocalizedText(308, "Type your search here");
            this.toolTips.SetToolTip(this.searchText, Language.GetLocalizedText(309, "Enter your search terms here."));
            this.raLabel.Text = Language.GetLocalizedText(310, "RA");
            this.decLabel.Text = Language.GetLocalizedText(311, "Dec");
            this.searchResults.AddText = Language.GetLocalizedText(161, "Add New Item");
            this.searchResults.EmptyAddText = Language.GetLocalizedText(162, "No Results");
            this.SearchView.Text = Language.GetLocalizedText(312, "Search View");
            this.wwtButton1.Text = Language.GetLocalizedText(313, "Server");
            this.plotResults.Text = Language.GetLocalizedText(314, "Plot Results");
            this.toolTips.SetToolTip(this.plotResults, Language.GetLocalizedText(316, "Show the location of each result on the sky map."));
            this.GoToRADec.Text = Language.GetLocalizedText(315, "Go");
            this.measureTool.Text = Language.GetLocalizedText(931, "Distance");
        }
        protected override void SetFocusedChild()
        {
            searchResults.Focus();
        }

       


        bool textChanged = false;

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            textChanged = true;
            searchTimer.Enabled = true;
          
        }

        private void searchText_KeyUp(object sender, KeyEventArgs e)
        {
   
        }

        private void SearchView_Click(object sender, EventArgs e)
        {
            Coordinates[] corners = RenderEngine.Engine.CurrentViewCorners;

            if (corners != null && !String.IsNullOrEmpty(Earth3d.MainWindow.Constellation))
            {

                IPlace[] results = ContextSearch.FindConteallationObjects(Earth3d.MainWindow.Constellation, corners, Classification.Unfiltered);
                searchResults.Clear();
                if (results != null)
                {
                    searchResults.AddRange(results);
                    searchResults.Refresh();    
                }


            }
            UpdateMarkers();

        }

       

        private void searchTimer_Tick(object sender, EventArgs e)
        {
            Catalogs.InitSearchTable();

            if (RenderEngine.Engine.Space != plotResults.Visible)
            {
                plotResults.Visible = RenderEngine.Engine.Space;
            }



            if ( textChanged )
            {
                if (string.IsNullOrEmpty(searchText.Text))
                {
                    searchResults.Clear();
                    textChanged = false;
                    return;
                }

                string searchString = Catalogs.CleanSearchString(searchText.Text);

                
                SearchCriteria sc = new SearchCriteria(searchString);

                int length = sc.Target.Length;

                List<Object> tempList = new List<Object>();
 
                foreach (KeyValuePair<string, IPlace> kv in Catalogs.AutoCompleteList.StartFromKey(sc.Target, TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
                {
                    if (tempList.Count > 100)
                    {
                        break;
                    }

                    if (kv.Value.Classification != Classification.Unidentified && (sc.Classification & kv.Value.Classification) == 0)
                    {
                        continue;
                    }

                    if (sc.Constellation != null && sc.Constellation != kv.Value.Constellation)
                    {
                        continue;
                    }

                    if (kv.Value.Magnitude > sc.MagnitudeMax)
                    {
                        continue;
                    }

                    if (kv.Value.Magnitude < sc.MagnitudeMin)
                    {
                        continue;
                    }

                    if (kv.Key == sc.Target || (kv.Key.Length > length && kv.Key.Substring(0, length) == sc.Target))
                    {
                        if (!tempList.Contains(kv.Value))
                        {
                            Place place = kv.Value as Place;
                            if (place != null && place.Name.ToLower() == searchString)
                            {
                                tempList.Insert(0, kv.Value);
                            }
                            else
                            {

                                tempList.Add(kv.Value);
                            }
                        }
                    }
                    else
                    {
                        int len = Math.Min(kv.Key.Length, sc.Target.Length);
                        if (string.Compare(kv.Key.Substring(0, len), sc.Target.Substring(0, len)) > 0)
                        {
                            break;
                        }
                    }
                }

                if (sc.Keywords != null && sc.Keywords.Count > 0)
                {
                    foreach (string keyword in sc.Keywords)
                    {
                        foreach (KeyValuePair<string, IPlace> kv in Catalogs.AutoCompleteList.StartFromKey(keyword, TraversalStartingPoint.EqualOrMore, TraversalDirection.LowToHigh))
                        {
                            length = keyword.Length;

                            if (tempList.Count > 100)
                            {
                                break;
                            }

                            if (kv.Value.Classification != Classification.Unidentified && (sc.Classification & kv.Value.Classification) == 0)
                            {
                                continue;
                            }

                            if (kv.Value.Magnitude > sc.MagnitudeMax)
                            {
                                continue;
                            }

                            if (kv.Value.Magnitude < sc.MagnitudeMin)
                            {
                                continue;
                            }

                            if (sc.Constellation != null && sc.Constellation != kv.Value.Constellation)
                            {
                                continue;
                            }


                            if (kv.Key == keyword || (kv.Key.Length > length && kv.Key.Substring(0, length) == keyword))
                            {
                                if (!tempList.Contains(kv.Value))
                                {
                                    Place place = kv.Value as Place;
                                    //System.Diagnostics.Debug.WriteLine(place.Name);
                                    if (place != null && place.Name == searchString)
                                    {
                                        tempList.Insert(0, kv.Value);
                                    }
                                    else
                                    {

                                        tempList.Add(kv.Value);
                                    }
                                }
                            }
                            else
                            {
                                int len = Math.Min(kv.Key.Length, sc.Target.Length);
                                if (string.Compare(kv.Key.Substring(0, len), sc.Target.Substring(0, len)) > 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }


                searchResults.Clear();
                searchResults.AddRange( tempList);
                UpdateMarkers();
            }
            //searchTimer.Enabled = false;
            textChanged = false;

        }

       

        public override bool AdvanceSlide(bool fromStart)
        {
            return searchResults.ShowNext(fromStart, false);
        }

        private void searchResults_ItemClicked(object sender, Object e)
        {
            if (e is Place)
            {
                Place p = (Place)e;
                if (p.Tour != null)
                {
                    FolderBrowser.LaunchTour(p.Tour);
                    return;
                }
            }
            
            RenderEngine.Engine.GotoTarget((IPlace)e, false, false, true);

        }

        private void searchResults_ItemDoubleClicked(object sender, Object e)
        {
            RenderEngine.Engine.GotoTarget((IPlace)e, false, true, true);

        }

        private void searchResults_ItemHover(object sender, Object e)
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

                toolTips.SetToolTip(searchResults, ((IThumbnail)e).Name);

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
                    toolTips.SetToolTip(searchResults, ((IThumbnail)e).Name);
                }
                Earth3d.MainWindow.SetLabelText(null, false);
                RenderEngine.Engine.PreviewBlend.TargetState = false;

            }


            //if (e != null)
            //{
            //    IPlace p = (IPlace)e;
            //    Earth3d.MainWindow.SetLabelText(p, true);
            //    toolTips.SetToolTip(searchResults, p.Name);
            //}
            //else
            //{
            //    if (e != null)
            //    {
            //        toolTips.SetToolTip(searchResults, ((IThumbnail)e).Name);
            //    }       
            //    Earth3d.MainWindow.SetLabelText(null, false);

            //}
        }

        //private void wwtButton1_Click(object sender, EventArgs e)
        //{
        //    WWTWebService wwtWebService = new WWTWebService();
        //    AstroObjectsDataset dataset = wwtWebService.GetAstroObjectsByName(searchText.Text);
        //    string a = dataset.ToString();
        //    searchResults.Clear();

        //    foreach (AstroObjectsDataset.spGetAstroObjectsRow row in dataset.spGetAstroObjects.Rows)
        //    {
        //        IPlace place = Place.FromAstroObjectsRow(row);
        //        searchResults.Add(place);
        //    }
        //}


        private void searchText_Enter(object sender, EventArgs e)
        {
            if (searchText.Text == Language.GetLocalizedText(308, "Type your search here"))
            {
                searchText.SelectAll();
                //searchText.Text = "";
            }
            Earth3d.NoStealFocus = true;
            searchResults.DontStealFocus = true;

        }

        public string SearchStringText
        {
            get { return searchText.Text; }
        }

        private void searchText_MouseClick(object sender, MouseEventArgs e)
        {
            if (searchText.Text == Language.GetLocalizedText(308, "Type your search here"))
            {
                //searchText.SelectAll();
                searchText.Text = "";
            }
        }

        private void searchText_Leave(object sender, EventArgs e)
        {
            Earth3d.NoStealFocus = false;
        }

        private void Search_Load(object sender, EventArgs e)
        {
            searchText.Text = Language.GetLocalizedText(308, "Type your search here");
            coordinateType.Items.Add(Language.GetLocalizedText(555, "J2000"));
            coordinateType.Items.Add(Language.GetLocalizedText(556, "Alt/Az"));
            coordinateType.Items.Add(Language.GetLocalizedText(557, "Galactic"));
            coordinateType.Items.Add(Language.GetLocalizedText(558, "Ecliptic"));
            coordinateType.Items.Add(Language.GetLocalizedText(932, "Lat/Lng"));
            coordinateType.SelectedIndex = 0;
        }

  

        public void DisplaySearchResults(VoTable table)
        {
            searchResults.Clear();
            int count = 0;
            foreach(VoRow row in table.Rows)
            {
                double ra = Convert.ToDouble(row["RA"])/15;
                double dec = Convert.ToDouble(row["DEC"]);

                TourPlace pl = new TourPlace(row["id"].ToString(), dec, ra, Classification.Star, Constellations.Containment.FindConstellationForPoint(ra, dec), ImageSetType.Sky, -1);
                searchResults.Add( pl);
                if (count++ > 200)
                {
                    break;
                }
            }
            UpdateMarkers();
        }

        private void plotResults_CheckedChanged(object sender, EventArgs e)
        {
            RenderEngine.Engine.ShowKmlMarkers = plotResults.Checked;

            UpdateMarkers();
            
        }

        private void UpdateMarkers()
        {
            if (RenderEngine.Engine.KmlMarkers != null)
            {
                RenderEngine.Engine.KmlMarkers.ClearPoints();
                if (plotResults.Checked)
                {
                    foreach (object o in searchResults.Items)
                    {
                        IPlace p = (IPlace)o;
                        RenderEngine.Engine.KmlMarkers.AddPoint(p.Name, p.RA, p.Dec);
                    }
                }
            }
        }

        private void searchResults_ItemContextMenu(object sender, object e)
        {
            Point pntClick = Cursor.Position;

            if (e is IPlace)
            {
                Earth3d.MainWindow.ShowContextMenu((IPlace)e, Earth3d.MainWindow.PointToClient(Cursor.Position), false, true);
            }
        }

        private void GoToRADec_Click(object sender, EventArgs e)
        {
            int index = coordinateType.SelectedIndex;


            double ra = 0;
            double dec = 0;
            bool raValid = false;
            bool decValid = false;
            switch (index)
            {
                case 0: // Equitorial
                    {
                        ra = Coordinates.ParseRA(raText.Text, false);
                        dec = Coordinates.ParseDec(decText.Text);
                        raValid = Coordinates.ValidateRA(raText.Text);
                        decValid = Coordinates.ValidateDec(decText.Text);
                    }
                    break;
                case 2: // Galactic
                    {
                        double l = Coordinates.Parse(raText.Text);
                        double b = Coordinates.ParseDec(decText.Text);
                        raValid = Coordinates.Validate(raText.Text);
                        decValid = Coordinates.ValidateDec(decText.Text);
                        if (raValid && decValid)
                        {

                            double[] result = Earth3d.GalactictoJ2000(l, b);
                            ra = result[0] / 15;
                            dec = result[1];
                        }
                    }
                    break;
                case 3: // Ecliptic
                    {
                        double l = Coordinates.Parse(raText.Text);
                        double b = Coordinates.ParseDec(decText.Text);
                        raValid = Coordinates.Validate(raText.Text);
                        decValid = Coordinates.ValidateDec(decText.Text);
                        if (raValid && decValid)
                        {

                            AstroCalc.AstroRaDec radec = AstroCalc.AstroCalc.EclipticToJ2000(l, b, SpaceTimeController.JNow);
                            ra = radec.RA;
                            dec = radec.Dec;
                        }
                    }
                    break;
                case 4: // Geo
                    {
                        ra = -Coordinates.Parse(raText.Text)/15;
                        dec = Coordinates.ParseDec(decText.Text);
                        raValid = Coordinates.Validate(raText.Text);
                        decValid = Coordinates.ValidateDec(decText.Text);
                     
                    }
                    break;     
                case 1: // alt/az
                    {
                        double az = Coordinates.Parse(raText.Text);
                        double alt = Coordinates.ParseDec(decText.Text);
                        raValid = Coordinates.Validate(raText.Text);
                        decValid = Coordinates.ValidateDec(decText.Text);
                        Coordinates radec= Coordinates.HorizonToEquitorial(Coordinates.FromLatLng(alt, az), SpaceTimeController.Location, SpaceTimeController.Now);
                        ra = radec.RA;
                        dec = radec.Dec;
                    }
                    break;
            }
            if (raValid && decValid)
            {
                if (Earth3d.MainWindow.SolarSystemMode)
                {
                    Vector3d pnt = Coordinates.GeoTo3dDouble(dec, Coordinates.Parse(raText.Text));


                    pnt = Vector3d.TransformCoordinate(pnt, Planets.EarthMatrix);
                    pnt.Normalize();
                    Vector2d radec = Coordinates.CartesianToLatLng(pnt);

                    RenderEngine.Engine.TargetLat = radec.Y;
                    RenderEngine.Engine.TargetLong = radec.X - 90;

                }
                else
                {

                    RenderEngine.Engine.GotoTargetRADec(ra, dec, true, false);
                }
            }
        }

        private void raText_Enter(object sender, EventArgs e)
        {
            searchResults.DontStealFocus = true;
            Earth3d.NoStealFocus = true;

        }

        private void decText_Enter(object sender, EventArgs e)
        {
            searchResults.DontStealFocus = true;
            Earth3d.NoStealFocus = true;

        }

        private void searchResults_Enter(object sender, EventArgs e)
        {
            searchResults.DontStealFocus = false;
            Earth3d.NoStealFocus = false;
        }

        private void raText_Validating(object sender, CancelEventArgs e)
        {
            bool valid = false;

            switch (coordinateType.SelectedIndex)
            {
                case 0:
                    valid = Coordinates.ValidateRA(raText.Text);
                    break;
                case 1:
                    valid = Coordinates.Validate(raText.Text);
                    break;
                case 4:
                case 3:
                case 2:
                    valid = Coordinates.Validate(raText.Text);
                    break;
            }

            if (valid)
            {
                raText.BackColor = searchText.BackColor;
            }
            else
            {
                raText.BackColor = Color.Red;
            }
        }

        private void decText_Validating(object sender, CancelEventArgs e)
        {
            bool valid = Coordinates.ValidateDec(decText.Text);

            if (valid)
            {
                decText.BackColor = searchText.BackColor;
            }
            else
            {
                decText.BackColor = Color.Red;
            }
        }

        private void Search_Shown(object sender, EventArgs e)
        {
            searchText.Focus();
        }

        private void coordinateType_SelectionChanged(object sender, EventArgs e)
        {
            int index = coordinateType.SelectedIndex;
            switch (index)
            {
                case 0: // RA-DEC
                    this.raLabel.Text = Language.GetLocalizedText(310, "RA");
                    this.decLabel.Text = Language.GetLocalizedText(311, "Dec");
                    break;
                case 1: // alt-az
                    this.raLabel.Text = Language.GetLocalizedText(268, "Az :");
                    this.decLabel.Text = Language.GetLocalizedText(269, "Alt : ");
                    break;
                case 2: // Galactic
                    this.raLabel.Text = "\u03BB";
                    this.decLabel.Text = "\u03B2";
                    break;
                case 3: // Ecliptic
                    this.raLabel.Text = "\u03BB";
                    this.decLabel.Text = "\u03B2";
                    break;
                case 4: // Ecliptic
                    this.raLabel.Text = Language.GetLocalizedText(950, "Lng");
                    this.decLabel.Text = Language.GetLocalizedText(951, "Lat");
                    break;
            }
        }

        private void measureTool_Click(object sender, EventArgs e)
        {
            Earth3d.MainWindow.Measuring = !Earth3d.MainWindow.Measuring;
        }
    }

 
}