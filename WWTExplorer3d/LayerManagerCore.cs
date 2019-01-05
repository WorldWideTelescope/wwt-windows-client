using ShapefileTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
#if WINDOWS_UWP
using Color= TerraViewer.Color;
#else
using System.Drawing;
using System.Xml;
#endif

namespace TerraViewer
{
    public partial class LayerManager
    {
        static LayerManager master = null;
        static int version = 0;

        public static int Version
        {
            get { return LayerManager.version; }
            set { LayerManager.version = value; }
        }

        static bool tourLayers = false;

        public static bool TourLayers
        {
            get { return LayerManager.tourLayers; }
            set
            {
                if (LayerManager.tourLayers != value && value == false)
                {
                    ClearLayers();
                    LayerManager.tourLayers = value;
                    LoadTree();
                }
                else if (LayerManager.tourLayers != value && value == true)
                {
                    LayerManager.tourLayers = value;
                    InitLayers();
                }

            }
        }

        static Dictionary<string, LayerMap> layerMaps = new Dictionary<string, LayerMap>();
        static Dictionary<string, LayerMap> layerMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> LayerMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerMapsTours;
                }
                else
                {
                    return LayerManager.layerMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerMapsTours = value;
                }
                else
                {
                    LayerManager.layerMaps = value;
                }
            }
        }

        private static Dictionary<string, LayerMap> allMaps = new Dictionary<string, LayerMap>();
        private static Dictionary<string, LayerMap> allMapsTours = new Dictionary<string, LayerMap>();

        public static Dictionary<string, LayerMap> AllMaps
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.allMapsTours;
                }
                else
                {
                    return LayerManager.allMaps;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.allMapsTours = value;
                }
                else
                {
                    LayerManager.allMaps = value;
                }
            }
        }

        static string currentMap = "Earth";

        public static string CurrentMap
        {
            get { return LayerManager.currentMap; }
            set { LayerManager.currentMap = value; }
        }

        private static Dictionary<Guid, Layer> layerList = new Dictionary<Guid, Layer>();
        static Dictionary<Guid, Layer> layerListTours = new Dictionary<Guid, Layer>();

        public static Dictionary<Guid, Layer> LayerList
        {
            get
            {
                if (TourLayers)
                {
                    return LayerManager.layerListTours;
                }
                else
                {
                    return LayerManager.layerList;
                }
            }
            set
            {
                if (TourLayers)
                {
                    LayerManager.layerListTours = value;
                }
                else
                {
                    LayerManager.layerList = value;
                }
            }
        }

        static object currentSelection = null;

        public static object CurrentSelection
        {
            get { return LayerManager.currentSelection; }
            set { LayerManager.currentSelection = value; }
        }

        //static List<Layer> layers = new List<Layer>();
        static LayerManager()
        {
            InitLayers();
        }

        public static IScriptable ScriptInterface
        {
            get
            {
                return new LayerScripting();
            }
        }
        public static Layer LoadShapeFile(string path, string currentMap)
        {


            ShapeFile shapefile = new ShapeFile(path);
            shapefile.Read();

            ShapeFileRenderer layer = new ShapeFileRenderer(shapefile);
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;
        }


        internal static Layer Load3dModelFile(string path, string currentMap)
        {


            Object3dLayer layer = new Object3dLayer();
            layer.LoadData(path);

            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;

        }

        internal static Layer LoadDataTable(string path, string currentMap, bool noInteraction)
        {

#if !BASICWWT
            string data = File.ReadAllText(path);
            string layerName = path.Substring(path.LastIndexOf('\\') + 1);


            SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
            layer.Enabled = true;
            layer.Name = layerName;

#if !WINDOWS_UWP
            if (noInteraction || DataWizard.ShowWizard(layer) ==  System.Windows.Forms.DialogResult.OK)
            {
                LayerList.Add(layer.ID, layer);
                layer.ReferenceFrame = currentMap;
                AllMaps[currentMap].Layers.Add(layer);
                AllMaps[currentMap].Open = true;
                LoadTree();

            }
#endif
            version++;
            return layer;
#else
            return null;
#endif

        }


        internal static Layer AddImagesetLayer(IImageSet imageSet)
        {
            if (!string.IsNullOrEmpty(CurrentMap))
            {
                ImageSetLayer layer = new ImageSetLayer(imageSet);
                layer.Name = imageSet.Name;
                LayerList.Add(layer.ID, layer);
                layer.ReferenceFrame = CurrentMap;
                AllMaps[CurrentMap].Layers.Add(layer);
                AllMaps[CurrentMap].Open = true;
                LoadTree();
                version++;
                return layer;
            }
            return null;
        }


        public static void Add(Layer layer, bool updateTree)
        {
            if (!LayerList.ContainsKey(layer.ID))
            {
                if (AllMaps.ContainsKey(layer.ReferenceFrame))
                {
                    LayerList.Add(layer.ID, layer);

                    AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    version++;
                    if (updateTree)
                    {
                        LoadTree();
                    }
                }
            }
        }


        static void AddIss()
        {
            ISSLayer layer = new ISSLayer();

            layer.Name = Language.GetLocalizedText(1314, "ISS Model  (Toshiyuki Takahei)");
            layer.Enabled = Properties.Settings.Default.ShowISSModel;
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = "ISS";
            AllMaps["ISS"].Layers.Add(layer);
            AllMaps["ISS"].Open = true;
        }

        static void AddIssToSandbox()
        {
            ISSLayer layer = new ISSLayer();

            layer.Name = Language.GetLocalizedText(1314, "ISS Model");
            layer.Enabled = true;
            layer.ID = new Guid();
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = "Sandbox";
            AllMaps["Sandbox"].Layers.Add(layer);
            layer.Scale = new Vector3d(50, 50, 50);
            
        }

        static public Vector3d GetPrimarySandboxLight()
        {
            LayerMap sandbox = AllMaps["Sandbox"];
            if (sandbox != null)
            {
                foreach (Layer layer in sandbox.Layers)
                {
                    Object3dLayer light = layer as Object3dLayer;
                    if (light != null && light.LightID == 1)
                    {
                        return light.Translate;
                    }
                }
            }

            return new Vector3d(10, 10, 10);
        }
        static public Color GetPrimarySandboxLightColor()
        {
            LayerMap sandbox = AllMaps["Sandbox"];
            if (sandbox != null)
            {
                foreach (Layer layer in sandbox.Layers)
                {
                    Object3dLayer light = layer as Object3dLayer;
                    if (light != null && light.LightID == 1)
                    {
                        return light.Color;
                    }
                }
            }

            return Color.White;
        }

        static public void InitLayers()
        {
            ClearLayers();
            //StockSkyOverlay.StockOverlays.Clear();

            LayerMap iss = null;
            LayerMap ol = null;
            LayerMap l1 = null;
            LayerMap l2 = null;
            if (!TourLayers)
            {
                string[] isstle = new string[0];
                try
                {
                    //This is downloaded now on startup
                    string url = "http://www.worldwidetelescope.org/wwtweb/isstle.aspx";
                    string filename = string.Format(@"{0}data\isstle.txt", Properties.Settings.Default.CahceDirectory);
                    DataSetManager.DownloadFile(url, filename, false, false);

                    isstle = File.ReadAllLines(filename);
                }
                catch
                {
                }

                iss = new LayerMap("ISS", ReferenceFrames.Custom);
                iss.Frame.Epoch = SpaceTimeController.TwoLineDateToJulian("10184.51609218");
                iss.Frame.SemiMajorAxis = 6728829.41;
                iss.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                iss.Frame.Inclination = 51.6442;
                iss.Frame.LongitudeOfAscendingNode = 147.0262;
                iss.Frame.Eccentricity = .0009909;
                iss.Frame.MeanAnomolyAtEpoch = 325.5563;
                iss.Frame.MeanDailyMotion = 360 * 15.72172655;
                iss.Frame.ArgumentOfPeriapsis = 286.4623;
                iss.Frame.Scale = 1;
                iss.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                iss.Frame.MeanRadius = 130;
                iss.Frame.Oblateness = 0;
                iss.Frame.ShowOrbitPath = false;
                if (isstle.Length > 1)
                {
                    try
                    {
                        iss.Frame.FromTLE(isstle[0], isstle[1], 398600441800000);
                    }
                    catch
                    {
                    }
                }
                iss.Enabled = true;
            }
            ol = new LayerMap("Observing Location", ReferenceFrames.Custom);
            ol.Frame.ReferenceFrameType = ReferenceFrameTypes.FixedSherical;
            ol.Frame.ObservingLocation = true;
            ol.Frame.Lat = SpaceTimeController.Location.Lat;
            ol.Frame.Lng = SpaceTimeController.Location.Lng;
            ol.Frame.Altitude = SpaceTimeController.Altitude;
            ol.Frame.SystemGenerated = true;

            l1 = new LayerMap("Sun-Earth L1 Point", ReferenceFrames.Custom);
            l1.Frame.ReferenceFrameType = ReferenceFrameTypes.Synodic;
            l1.Frame.Translation = new Vector3d(-1.5e6, 0.0, 0.0);
            l1.Enabled = false;
            l1.Frame.SystemGenerated = true;

            l2 = new LayerMap("Sun-Earth L2 Point", ReferenceFrames.Custom);
            l2.Frame.ReferenceFrameType = ReferenceFrameTypes.Synodic;
            l2.Frame.Translation = new Vector3d(1.5e6, 0.0, 0.0);
            l2.Enabled = false;
            l2.Frame.SystemGenerated = true;

            LayerMaps.Add("Sun", new LayerMap("Sun", ReferenceFrames.Sun));
            LayerMaps["Sun"].AddChild(new LayerMap("Mercury", ReferenceFrames.Mercury));
            LayerMaps["Sun"].AddChild(new LayerMap("Venus", ReferenceFrames.Venus));
            LayerMaps["Sun"].AddChild(new LayerMap("Earth", ReferenceFrames.Earth));
            LayerMaps["Sun"].ChildMaps["Earth"].Layers.Add(new SkyOverlays(SkyOverlaysType.Earth));
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(new LayerMap("Moon", ReferenceFrames.Moon));

            if (!TourLayers)
            {
                LayerMaps["Sun"].ChildMaps["Earth"].AddChild(iss);
            }
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(ol);
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l1);
            LayerMaps["Sun"].ChildMaps["Earth"].AddChild(l2);

            LayerMaps["Sun"].AddChild(new LayerMap("Mars", ReferenceFrames.Mars));
            LayerMaps["Sun"].AddChild(new LayerMap("Jupiter", ReferenceFrames.Jupiter));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Io", ReferenceFrames.Io));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Europa", ReferenceFrames.Europa));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Ganymede", ReferenceFrames.Ganymede));
            LayerMaps["Sun"].ChildMaps["Jupiter"].AddChild(new LayerMap("Callisto", ReferenceFrames.Callisto));
            LayerMaps["Sun"].AddChild(new LayerMap("Saturn", ReferenceFrames.Saturn));
            LayerMaps["Sun"].AddChild(new LayerMap("Uranus", ReferenceFrames.Uranus));
            LayerMaps["Sun"].AddChild(new LayerMap("Neptune", ReferenceFrames.Neptune));
            LayerMaps["Sun"].AddChild(new LayerMap("Pluto", ReferenceFrames.Pluto));

            AddMoons();

            LayerMaps.Add("Sky", new LayerMap("Sky", ReferenceFrames.Sky));

            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.Sky));
            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.Sky2d));
            LayerMaps["Sky"].Layers.Add(new SkyOverlays(SkyOverlaysType.SolarSystem));
            LayerMaps["Sky"].Open = true;
            LayerMaps["Sun"].Open = true;

            LayerMaps.Add("Sandbox", new LayerMap("Sandbox", ReferenceFrames.Sandbox));

            LayerMaps.Add("Dome", new LayerMap("Dome", ReferenceFrames.Identity));
            LayerMaps["Dome"].Layers.Add(new SkyOverlays(SkyOverlaysType.Dome));
            LayerMaps["Dome"].Open = true;




            AllMaps.Clear();

            AddAllMaps(LayerMaps, null);
            if (!TourLayers)
            {
                AddIss();
            }

#if WINDOWS_UWP
            AddIssToSandbox();
#endif
            version++;
            LoadTree();


        }

        private static void ClearLayers()
        {
            foreach (Layer layer in LayerList.Values)
            {
                layer.CleanUp();
            }

            LayerList.Clear();
            LayerMaps.Clear();
        }

        private static void AddMoons()
        {
            string filename = Properties.Settings.Default.CahceDirectory + "\\data\\moons.txt";
            DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?Q=moons", filename, false, true);

            string[] data = File.ReadAllLines(filename);

            bool first = true;
            foreach (string line in data)
            {
                if (first)
                {
                    first = false;
                    continue;
                }
                string[] parts = line.Split(new char[] { '\t' });
                string planet = parts[0];
                LayerMap frame = new LayerMap(parts[2], ReferenceFrames.Custom);
                frame.Frame.SystemGenerated = true;
                frame.Frame.Epoch = double.Parse(parts[1]);
                frame.Frame.SemiMajorAxis = double.Parse(parts[3]) * 1000;
                frame.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                frame.Frame.Inclination = double.Parse(parts[7]);
                frame.Frame.LongitudeOfAscendingNode = double.Parse(parts[8]);
                frame.Frame.Eccentricity = double.Parse(parts[4]);
                frame.Frame.MeanAnomolyAtEpoch = double.Parse(parts[6]);
                frame.Frame.MeanDailyMotion = double.Parse(parts[9]);
                frame.Frame.ArgumentOfPeriapsis = double.Parse(parts[5]);
                frame.Frame.Scale = 1;
                frame.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                frame.Frame.MeanRadius = double.Parse(parts[16]) * 1000;
                frame.Frame.RotationalPeriod = double.Parse(parts[17]);
                frame.Frame.ShowAsPoint = false;
                frame.Frame.ShowOrbitPath = true;
                frame.Frame.RepresentativeColor = Color.LightBlue;
                frame.Frame.Oblateness = 0;

                LayerMaps["Sun"].ChildMaps[planet].AddChild(frame);
            }
        }

        private static void AddAllMaps(Dictionary<string, LayerMap> maps, String parent)
        {
            foreach (LayerMap map in maps.Values)
            {
                map.Frame.Parent = parent;
                AllMaps.Add(map.Name, map);
                AddAllMaps(map.ChildMaps, map.Name);
            }
        }

        // This is only for Edit Mode Tours... Not for Tours Layers
        public static bool CheckForTourLoadedLayers()
        {
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    return true;
                }
            }
            return false;
        }

        internal static void CloseAllTourLoadedLayers()
        {
            List<Guid> purgeTargets = new List<Guid>();
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    purgeTargets.Add(layer.ID);
                }
            }

            foreach (Guid guid in purgeTargets)
            {
                DeleteLayerByID(guid, true, false);
            }

            List<string> purgeMapsNames = new List<string>();

            foreach (LayerMap map in AllMaps.Values)
            {
                if (map.LoadedFromTour && map.Layers.Count == 0)
                {
                    purgeMapsNames.Add(map.Name);
                }
            }

            foreach (string name in purgeMapsNames)
            {
                PurgeLayerMapDeep(AllMaps[name], true);
            }



            Version++;
            LoadTree();

        }

        public static bool DeleteLayerByID(Guid ID, bool removeFromParent, bool updateTree)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                layer.CleanUp();
                if (removeFromParent)
                {
                    AllMaps[layer.ReferenceFrame].Layers.Remove(layer);
                }
                LayerList.Remove(ID);
                version++;
                if (updateTree)
                {
                    LoadTree();
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        internal static void CleanAllTourLoadedLayers()
        {
            foreach (Layer layer in layerList.Values)
            {
                if (layer.LoadedFromTour)
                {
                    //todo We may want to copy layers into a temp directory later, for now we are just leaving the layer data files in the temp tour directory. 
                    layer.LoadedFromTour = false;
                }
            }
        }

        // Merged layers from Tour Player Alternate universe into the real layer manager layers list
        public static void MergeToursLayers()
        {

            tourLayers = false;
            bool OverWrite = false;
            bool CollisionChecked = false;

            foreach (LayerMap map in allMapsTours.Values)
            {
                if (!allMaps.ContainsKey(map.Name))
                {
                    LayerMap newMap = new LayerMap(map.Name, ReferenceFrames.Custom);
                    newMap.Frame = map.Frame;
                    newMap.LoadedFromTour = true;
                    LayerManager.AllMaps.Add(newMap.Name, newMap);
                }
            }
            ConnectAllChildren();


            foreach (Layer layer in layerListTours.Values)
            {
                if (LayerList.ContainsKey(layer.ID))
                {
                    if (!CollisionChecked)
                    {
#if WINDOWS_UWP
                        OverWrite = true;
#else
                        if (UiTools.ShowMessageBox(Language.GetLocalizedText(958, "There are layers with the same name. Overwrite existing layers?"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"), System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            OverWrite = true;
                        }
                        else
                        {
                            OverWrite = false;
                        }
#endif
                        CollisionChecked = true;
                    }

                    if (OverWrite)
                    {
                        LayerManager.DeleteLayerByID(layer.ID, true, false);
                    }
                }

                if (!LayerList.ContainsKey(layer.ID))
                {
                    if (AllMaps.ContainsKey(layer.ReferenceFrame))
                    {
                        LayerList.Add(layer.ID, layer);

                        AllMaps[layer.ReferenceFrame].Layers.Add(layer);
                    }
                }
                else
                {
                    layer.CleanUp();
                }
            }

            layerListTours.Clear();
            allMapsTours.Clear();
            layerMapsTours.Clear();
            LoadTree();
        }
        static string GetMpcAsTLE(string id, LayerMap target)
        {
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent", "WWT");
            client.Headers.Add("Content-Type", "text/xml");
            string data = client.DownloadString("http://www.minorplanetcenter.net/db_search/show_object?object_id=" + id);


            int startform = data.IndexOf("show-orbit-button");

            int lastForm = data.IndexOf("/form", startform);

            string formpart = data.Substring(startform, lastForm - startform);

            string name = id;

            ReferenceFrame frame = new ReferenceFrame();

            frame.Oblateness = 0;
            frame.ShowOrbitPath = true;
            frame.ShowAsPoint = true;

            frame.Epoch = SpaceTimeController.UtcToJulian(DateTime.Parse(GetValueByID(formpart, "epoch").Substring(0, 10)));
            frame.SemiMajorAxis = double.Parse(GetValueByID(formpart, "a")) * UiTools.KilometersPerAu * 1000;
            frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
            frame.Inclination = double.Parse(GetValueByID(formpart, "incl"));
            frame.LongitudeOfAscendingNode = double.Parse(GetValueByID(formpart, "node"));
            frame.Eccentricity = double.Parse(GetValueByID(formpart, "e"));
            frame.MeanAnomolyAtEpoch = double.Parse(GetValueByID(formpart, "m"));
            frame.MeanDailyMotion = CAAElliptical.MeanMotionFromSemiMajorAxis(double.Parse(GetValueByID(formpart, "a")));
            frame.ArgumentOfPeriapsis = double.Parse(GetValueByID(formpart, "peri"));
            frame.Scale = 1;
            frame.SemiMajorAxisUnits = AltUnits.Meters;
            frame.MeanRadius = 10;
            frame.Oblateness = 0;

            String TLE = name + "\n" + frame.ToTLE();

            String filename = Path.GetTempPath() + "\\" + name;

            File.WriteAllText(filename, TLE);

            LoadOrbitsFile(filename, target.Name);

            LoadTree();

            return null;
        }

        string ConvertToTLE(LayerMap map)
        {

            LayerMap target = map.Parent;

            ReferenceFrame frame = map.Frame;
            string name = frame.Name;

            String TLE = name + "\n" + frame.ToTLE();

            String filename = Path.GetTempPath() + "\\" + name;

            File.WriteAllText(filename, TLE);

            LoadOrbitsFile(filename, target.Name);

            LoadTree();

            return null;
        }

        static string GetMpc(string id, LayerMap target)
        {

            WebClient client = new WebClient();

            string data = client.DownloadString("http://www.minorplanetcenter.net/db_search/show_object?object_id=" + id);


            int startform = data.IndexOf("show-orbit-button");

            int lastForm = data.IndexOf("/form", startform);

            string formpart = data.Substring(startform, lastForm - startform);

            string name = id;

            LayerMap orbit = new LayerMap(name.Trim(), ReferenceFrames.Custom);
            orbit.Frame.Oblateness = 0;
            orbit.Frame.ShowOrbitPath = true;
            orbit.Frame.ShowAsPoint = true;

            orbit.Frame.Epoch = SpaceTimeController.UtcToJulian(DateTime.Parse(GetValueByID(formpart, "epoch").Substring(0, 10)));
            orbit.Frame.SemiMajorAxis = double.Parse(GetValueByID(formpart, "a")) * UiTools.KilometersPerAu * 1000;
            orbit.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
            orbit.Frame.Inclination = double.Parse(GetValueByID(formpart, "incl"));
            orbit.Frame.LongitudeOfAscendingNode = double.Parse(GetValueByID(formpart, "node"));
            orbit.Frame.Eccentricity = double.Parse(GetValueByID(formpart, "e"));
            orbit.Frame.MeanAnomolyAtEpoch = double.Parse(GetValueByID(formpart, "m"));
            orbit.Frame.MeanDailyMotion = CAAElliptical.MeanMotionFromSemiMajorAxis(double.Parse(GetValueByID(formpart, "a")));
            orbit.Frame.ArgumentOfPeriapsis = double.Parse(GetValueByID(formpart, "peri"));
            orbit.Frame.Scale = 1;
            orbit.Frame.SemiMajorAxisUnits = AltUnits.Meters;
            orbit.Frame.MeanRadius = 10;
            orbit.Frame.Oblateness = 0;

            if (!AllMaps[target.Name].ChildMaps.ContainsKey(name.Trim()))
            {
                AllMaps[target.Name].AddChild(orbit);
            }

            AllMaps.Add(orbit.Name, orbit);

            orbit.Frame.Parent = target.Name;

            MakeLayerGroup("Minor Planet", orbit);

            LoadTree();

            return null;
        }

        static string GetValueByID(string data, string id)
        {

            int valStart = data.IndexOf("id=\"" + id + "\"");
            valStart = data.IndexOf("value=", valStart) + 7;
            int valEnd = data.IndexOf("\"", valStart);
            return data.Substring(valStart, valEnd - valStart);
        }

        private static void ImportTLEFile(string filename, LayerMap target)
        {
            string name = filename.Substring(filename.LastIndexOf("\\") + 1);
            name = name.Substring(0, name.LastIndexOf("."));

            MakeLayerGroup(name, target);

            string[] data = File.ReadAllLines(filename);

            for (int i = 0; i < data.Length; i += 3)
            {
                LayerMap orbit = new LayerMap(data[i].Trim(), ReferenceFrames.Custom);
                orbit.Frame.Oblateness = 0;
                orbit.Frame.ShowOrbitPath = true;
                orbit.Frame.ShowAsPoint = true;
                orbit.Frame.Epoch = SpaceTimeController.TwoLineDateToJulian("10184.51609218");
                orbit.Frame.SemiMajorAxis = 6728829.41;
                orbit.Frame.ReferenceFrameType = ReferenceFrameTypes.Orbital;
                orbit.Frame.Inclination = 51.6442;
                orbit.Frame.LongitudeOfAscendingNode = 147.0262;
                orbit.Frame.Eccentricity = .0009909;
                orbit.Frame.MeanAnomolyAtEpoch = 325.5563;
                orbit.Frame.MeanDailyMotion = 360 * 15.72172655;
                orbit.Frame.ArgumentOfPeriapsis = 286.4623;
                orbit.Frame.Scale = 1;
                orbit.Frame.SemiMajorAxisUnits = AltUnits.Meters;
                orbit.Frame.MeanRadius = 10;
                orbit.Frame.Oblateness = 0;
                orbit.Frame.FromTLE(data[i + 1], data[i + 2], 398600441800000);
                if (!AllMaps[name].ChildMaps.ContainsKey(data[i].Trim()))
                {
                    AllMaps[name].AddChild(orbit);
                }
            }
            version++;
        }

        private static void MakeLayerGroup(string name, LayerMap target)
        {
            ReferenceFrame frame = new ReferenceFrame();
            frame.Name = name;
            frame.Reference = ReferenceFrames.Identity;
            LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Identity);
            newMap.Frame = frame;
            newMap.Frame.SystemGenerated = false;
            target.AddChild(newMap);

            newMap.Frame.Parent = target.Name;
            AllMaps.Add(frame.Name, newMap);
            version++;
        }

        public static void PurgeLayerMapDeep(LayerMap target, bool topLevel)
        {

            foreach (Layer layer in target.Layers)
            {
                LayerManager.DeleteLayerByID(layer.ID, false, false);
            }

            target.Layers.Clear();

            foreach (LayerMap map in target.ChildMaps.Values)
            {
                PurgeLayerMapDeep(map, false);
            }

            target.ChildMaps.Clear();
            if (topLevel)
            {
                if (!String.IsNullOrEmpty(target.Frame.Parent))
                {
                    if (AllMaps.ContainsKey(target.Frame.Parent))
                    {
                        AllMaps[target.Frame.Parent].ChildMaps.Remove(target.Name);
                    }
                }
                else
                {
                    if (LayerMaps.ContainsKey(target.Name))
                    {
                        LayerMaps.Remove(target.Name);
                    }
                }
            }
            AllMaps.Remove(target.Name);
            version++;
        }

        public static bool CreateReferenceFrame(string name, string parent, string xml)
        {
            if (!AllMaps.ContainsKey(name) && AllMaps.ContainsKey(parent))
            {
                LayerMap target = AllMaps[parent];
                ReferenceFrame frame = new ReferenceFrame();
                frame.Name = name;

                if (!string.IsNullOrEmpty(xml))
                {
                    if (!frame.SetProps(xml))
                    {
                        return false;
                    }
                }

                LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Custom);
                newMap.Frame = frame;
                target.AddChild(newMap);
                newMap.Frame.Parent = target.Name;
                AllMaps.Add(frame.Name, newMap);

                version++;
                LoadTree();
                return true;
            }
            return false;
        }
        public static string GetFramePropsByName(string name)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;

                return frame.GetProps();
            }
            else
            {
                return "";
            }
        }

        public static string GetFramePropByName(string name, string propName, out ReferenceFrame frame)
        {
            if (AllMaps.ContainsKey(name))
            {
                frame = AllMaps[name].Frame;

                return frame.GetProp(propName);
            }
            else
            {
                frame = null;
                return "";
            }

        }

        public static bool SetFramePropsByName(string name, string xml)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;

                bool retVal = frame.SetProps(xml);
                //LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }
        }

        public static bool SetFramePropByName(string name, string propName, string propValue)
        {
            if (AllMaps.ContainsKey(name))
            {
                ReferenceFrame frame = AllMaps[name].Frame;
                return frame.SetProp(propName, propValue);
            }
            else
            {
                return false;
            }
        }
        internal static bool UpdateLayer(Guid layerID, object data, bool show, string name, bool noPurge, bool purgeAll, bool hasHeader)
        {

            if (LayerList.ContainsKey(layerID))
            {
                Layer layer = LayerList[layerID];
                layer.UpadteData(data, !noPurge, purgeAll, hasHeader);
                layer.CleanUp();
                layer.Enabled = show;
                if (!string.IsNullOrEmpty(name))
                {
                    layer.Name = name;
                }
                LoadTree();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static IPlace FindClosest(Coordinates target, float distance, bool astronomical, string referenceLayer)
        {
            IPlace closestPlace = null;
            if (referenceLayer == null)
            {
                return closestPlace;
            }
            if (AllMaps.ContainsKey(referenceLayer))
            {
                float currentDistance = distance;

                foreach (Layer layer in AllMaps[referenceLayer].Layers)
                {
                    if (layer.Enabled && astronomical == layer.Astronomical)
                    {
                        closestPlace = layer.FindClosest(target, distance, closestPlace, astronomical);
                    }
                }
                if (closestPlace != null && master != null)
                {
#if !WINDOWS_UWP
                    master.ShowPlace(closestPlace);
#endif
                }

            }
            return closestPlace;

        }
        internal static Vector3d GetFrameTarget(RenderContext11 renderContext, string TrackingFrame)
        {
            Vector3d targetPoint = Vector3d.Empty;

            if (!AllMaps.ContainsKey(TrackingFrame))
            {
                return targetPoint;
            }

            List<LayerMap> mapList = new List<LayerMap>();

            LayerMap current = AllMaps[TrackingFrame];

            mapList.Add(current);

            while (current.Frame.Reference == ReferenceFrames.Custom)
            {
                current = current.Parent;
                mapList.Insert(0, current);
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            Matrix3d matOldBase = renderContext.WorldBase;
            double oldNominalRadius = renderContext.NominalRadius;

            foreach (LayerMap map in mapList)
            {
                if (map.Frame.Reference != ReferenceFrames.Custom)
                {
                    Planets.SetupPlanetMatrix(renderContext, (int)Enum.Parse(typeof(SolarSystemObjects), map.Frame.Name), Vector3d.Empty, false);
                }
                else
                {
                    map.ComputeFrame(renderContext);
                    if (map.Frame.useRotatingParentFrame())
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.World;
                    }
                    else
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                    }
                    if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                    {
                        renderContext.WorldBaseNonRotating = renderContext.World;
                    }
                    renderContext.NominalRadius = map.Frame.MeanRadius;
                }
            }

            targetPoint = renderContext.World.Transform(targetPoint);


            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
            renderContext.WorldBase = matOldBase;

            return targetPoint;
        }

        internal static Vector3d GetFrameTarget(RenderContext11 renderContext, string TrackingFrame, out Matrix3d matOut)
        {
            Vector3d targetPoint = Vector3d.Empty;
            matOut = Matrix3d.Identity;

            if (!AllMaps.ContainsKey(TrackingFrame))
            {
                return targetPoint;
            }

            List<LayerMap> mapList = new List<LayerMap>();

            LayerMap current = AllMaps[TrackingFrame];

            mapList.Add(current);

            while (current.Frame.Reference == ReferenceFrames.Custom)
            {
                current = current.Parent;
                mapList.Insert(0, current);
            }

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            Matrix3d matOldBase = renderContext.WorldBase;
            double oldNominalRadius = renderContext.NominalRadius;

            foreach (LayerMap map in mapList)
            {
                if (map.Frame.Reference != ReferenceFrames.Custom && map.Frame.Reference != ReferenceFrames.Sandbox)
                {
                    Planets.SetupPlanetMatrix(renderContext, (int)Enum.Parse(typeof(SolarSystemObjects), map.Frame.Name), Vector3d.Empty, false);
                }
                else
                {
                    map.ComputeFrame(renderContext);
                    if (map.Frame.useRotatingParentFrame())
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.World;
                    }
                    else
                    {
                        renderContext.World = map.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                    }
                    if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                    {
                        renderContext.WorldBaseNonRotating = renderContext.World;
                    }

                    renderContext.NominalRadius = map.Frame.MeanRadius;
                }
            }

            targetPoint = renderContext.World.Transform(targetPoint);

            Vector3d lookAt = renderContext.World.Transform(new Vector3d(0, 0, 1));

            Vector3d lookUp = renderContext.World.Transform(new Vector3d(0, 1, 0)) - targetPoint;


            lookUp.Normalize();


            matOut = Matrix3d.LookAtLH(new Vector3d(0, 0, 0), lookAt - targetPoint, lookUp);


            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
            renderContext.WorldBase = matOldBase;

            return targetPoint;
        }

        private static int currentSlideID = -1;

        internal static int CurrentSlideID
        {
            get { return LayerManager.currentSlideID; }
            set
            {
                if (currentSlideID != value)
                {
                    SlideChanged = true;
                }
                else
                {
                    SlideChanged = false;
                }
                LayerManager.currentSlideID = value;
            }
        }

        internal static float SlideTweenPosition = 0;

        internal static bool SlideChanged = false;

        internal static void PrepTourLayers()
        {
#if !BASICWWT
            if (TourPlayer.Playing)
            {
                TourPlayer player = RenderEngine.Engine.uiController as TourPlayer;
                if (player != null)
                {
                    TourDocument tour = player.Tour;

                    if (tour.ProjectorServer)
                    {
                        tour.CurrentTourstopIndex = CurrentSlideID;
                    }

                    if (tour.CurrentTourStop != null)
                    {
                        SlideTweenPosition = player.UpdateTweenPosition(RenderEngine.ProjectorServer ? SlideTweenPosition : -1);
                        CurrentSlideID = tour.CurrentTourstopIndex;

                        if (!tour.CurrentTourStop.KeyFramed)
                        {
                            tour.CurrentTourStop.UpdateLayerOpacity();

                            foreach (LayerInfo info in tour.CurrentTourStop.Layers.Values)
                            {
                                if (LayerList.ContainsKey(info.ID))
                                {
                                    LayerList[info.ID].Opacity = info.FrameOpacity;
                                    LayerList[info.ID].SetParams(info.FrameParams);
                                }
                            }
                        }
                    }
                }
            }
            else
#endif
            {
                CurrentSlideID = -1;
                SlideTweenPosition = 0;
            }


        }

        private static bool IsSphereInFrustum(Vector3d sphereCenter, double sphereRadius, PlaneD[] frustum)
        {
            Vector4d center4 = new Vector4d(sphereCenter.X, sphereCenter.Y, sphereCenter.Z, 1.0);

            for (int i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(center4) < -sphereRadius)
                {
                    return false;
                }
            }

            return true;
        }

        // Get the radius in pixels of a sphere with the specified center and radius
        // The sphere center should be a point in camera space
        private static double ProjectedSizeInPixels(RenderContext11 renderContext, Vector3d center, double radius)
        {
            Matrix3d projection = renderContext.Projection;
            SharpDX.ViewportF viewport = renderContext.ViewPort;

            double distance = center.Length();

            // Calculate pixelsPerUnit which is the number of pixels covered
            // by an object 1 AU at the distance of the planet center from
            // the camera. This calculation works regardless of the projection
            // type.
            double viewportHeight = viewport.Height;
            double p11 = projection.M11;
            double p34 = projection.M34;
            double p44 = projection.M44;
            double w = Math.Abs(p34) * distance + p44;
            double pixelsPerUnit = (p11 / w) * viewportHeight;

            return radius * pixelsPerUnit;
        }
        internal static void Draw(RenderContext11 renderContext, float opacity, bool astronomical, string referenceFrame, bool nested, bool flatSky)
        {

            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }

            LayerMap thisMap = AllMaps[referenceFrame];

            if (!thisMap.Enabled || (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0 && !(thisMap.Frame.ShowAsPoint || thisMap.Frame.ShowOrbitPath)))
            {
                return;
            }

            //PrepTourLayers();

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom | thisMap.Frame.Reference == ReferenceFrames.Sandbox)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.useRotatingParentFrame())
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.World;
                }
                else
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                }
                if (thisMap.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                {
                    renderContext.WorldBaseNonRotating = renderContext.World;
                }

                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }


            if (thisMap.Frame.ShowAsPoint)
            {
                Planets.DrawPointPlanet(renderContext, new Vector3d(0, 0, 0), (float).2, thisMap.Frame.RepresentativeColor, true, opacity);
            }


            PlaneD[] viewFrustum = new PlaneD[6];
            //todo UWP fix this to use frustum from HMD view
            RenderContext11.ComputeFrustum(renderContext.Projection, viewFrustum);

            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        bool skipLayer = false;
                        if (pass == 0)
                        {
                            // Skip default image set layer so that it's not drawn twice
                            skipLayer = !astronomical && ((ImageSetLayer)layer).OverrideDefaultLayer && AllMaps[referenceFrame].Frame.Reference != ReferenceFrames.Custom;
                        }

                        if (layer.Enabled && !skipLayer) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.In || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.Out || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / layer.FadeSpan.TotalDays);
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / layer.FadeSpan.TotalDays);
                                }
                                layer.Astronomical = astronomical;

                                layer.Draw(renderContext, opacity * fadeOpacity, flatSky);
                            }
                        }
                    }
                }
            }
            if (nested)
            {
                foreach (LayerMap map in AllMaps[referenceFrame].ChildMaps.Values)
                {
                    if (map.Enabled && map.Frame.ShowOrbitPath && Properties.Settings.Default.SolarSystemMinorOrbits.State)
                    {
                        if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Orbital)
                        {
                            if (map.Frame.Orbit == null)
                            {
                                map.Frame.Orbit = new Orbit(map.Frame.Elements, 360, map.Frame.RepresentativeColor, 1,/* referenceFrame == "Sun" ? (float)(UiTools.KilometersPerAu*1000.0):*/ (float)renderContext.NominalRadius);
                            }

                            double dd = renderContext.NominalRadius;

                            double distss = UiTools.SolarSystemToMeters(RenderEngine.Engine.SolarSystemCameraDistance);



                            Matrix3d matSaved = renderContext.World;
                            renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;

                            // orbitCenter is a position in camera space
                            Vector3d orbitCenter = Vector3d.TransformCoordinate(new Vector3d(0, 0, 0), Matrix3d.Multiply(renderContext.World, renderContext.View));
                            double worldScale = Math.Sqrt(renderContext.World.M11 * renderContext.World.M11 + renderContext.World.M12 * renderContext.World.M12 + renderContext.World.M13 * renderContext.World.M13) * UiTools.KilometersPerAu;



                            double orbitRadius = map.Frame.Orbit.BoundingRadius / UiTools.KilometersPerAu * worldScale;
                            bool cull = !IsSphereInFrustum(orbitCenter, orbitRadius, viewFrustum);



                            float fade = (float)Math.Min(1, Math.Max(Math.Log(UiTools.SolarSystemToMeters(RenderEngine.Engine.SolarSystemCameraDistance), 10) - 7.3, 0));
                            if (RenderEngine.Engine.TrackingFrame == map.Frame.Name)
                            {
                                double ratio = map.Frame.MeanRadius / distss;

                                double val = Math.Log(ratio, 10) + 2.7;
                                fade = (float)Math.Min(1, Math.Max(-val, 0));

                            }


                            fade *= Properties.Settings.Default.SolarSystemMinorOrbits.Opacity;
                            if (fade > 0)
                            {


                                map.Frame.Orbit.Draw3D(renderContext, fade, new Vector3d(0, 0, 0));
                                renderContext.World = matSaved;
                            }
                        }
                        else if (map.Frame.ReferenceFrameType == ReferenceFrameTypes.Trajectory)
                        {
                            if (map.Frame.trajectoryLines == null)
                            {
                                map.Frame.trajectoryLines = new LineList();
                                map.Frame.trajectoryLines.ShowFarSide = true;
                                map.Frame.trajectoryLines.UseNonRotatingFrame = true;

                                int count = map.Frame.Trajectory.Count - 1;
                                for (int i = 0; i < count; i++)
                                {
                                    Vector3d pos1 = map.Frame.Trajectory[i].Position;
                                    Vector3d pos2 = map.Frame.Trajectory[i + 1].Position;
                                    pos1.Multiply(1 / renderContext.NominalRadius);
                                    pos2.Multiply(1 / renderContext.NominalRadius);
                                    map.Frame.trajectoryLines.AddLine(pos1, pos2, map.Frame.RepresentativeColor, new Dates());
                                }
                            }
                            Matrix3d matSaved = renderContext.World;
                            renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;
                            double distss = UiTools.SolarSystemToMeters(RenderEngine.Engine.SolarSystemCameraDistance);

                            float fade = (float)Math.Min(1, Math.Max(Math.Log(distss, 10) - 7.3, 0));
                            if (RenderEngine.Engine.TrackingFrame == map.Frame.Name)
                            {
                                double ratio = map.Frame.MeanRadius / distss;

                                double val = Math.Log(ratio, 10) + 2.7;
                                fade = (float)Math.Min(1, Math.Max(-val, 0));
                            }

                            map.Frame.trajectoryLines.DrawLines(renderContext, Properties.Settings.Default.SolarSystemMinorOrbits.Opacity * fade);
                            renderContext.World = matSaved;
                        }
                    }

                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        Draw(renderContext, opacity, astronomical, map.Name, nested, flatSky);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }

        //todo remove the stuff from draw that is redundant once predraw has run
        internal static void PreDraw(RenderContext11 renderContext, float opacity, bool astronomical, string referenceFrame, bool nested)
        {


            if (!AllMaps.ContainsKey(referenceFrame))
            {
                return;
            }



            LayerMap thisMap = AllMaps[referenceFrame];

            if (thisMap.ChildMaps.Count == 0 && thisMap.Layers.Count == 0)
            {
                return;
            }

            //PrepTourLayers();

            Matrix3d matOld = renderContext.World;
            Matrix3d matOldNonRotating = renderContext.WorldBaseNonRotating;
            double oldNominalRadius = renderContext.NominalRadius;
            if (thisMap.Frame.Reference == ReferenceFrames.Custom)
            {
                thisMap.ComputeFrame(renderContext);
                if (thisMap.Frame.useRotatingParentFrame())
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.World;
                }
                else
                {
                    renderContext.World = thisMap.Frame.WorldMatrix * renderContext.WorldBaseNonRotating;
                }
                if (thisMap.Frame.ReferenceFrameType == ReferenceFrameTypes.Synodic)
                {
                    renderContext.WorldBaseNonRotating = renderContext.World;
                }

                renderContext.NominalRadius = thisMap.Frame.MeanRadius;
            }



            for (int pass = 0; pass < 2; pass++)
            {
                foreach (Layer layer in AllMaps[referenceFrame].Layers)
                {
                    if ((pass == 0 && layer is ImageSetLayer) || (pass == 1 && !(layer is ImageSetLayer)))
                    {
                        if (layer.Enabled) // && astronomical == layer.Astronomical)
                        {
                            double layerStart = SpaceTimeController.UtcToJulian(layer.StartTime);
                            double layerEnd = SpaceTimeController.UtcToJulian(layer.EndTime);
                            double fadeIn = SpaceTimeController.UtcToJulian(layer.StartTime) - ((layer.FadeType == FadeType.In || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);
                            double fadeOut = SpaceTimeController.UtcToJulian(layer.EndTime) + ((layer.FadeType == FadeType.Out || layer.FadeType == FadeType.Both) ? layer.FadeSpan.TotalDays : 0);

                            if (SpaceTimeController.JNow > fadeIn && SpaceTimeController.JNow < fadeOut)
                            {
                                float fadeOpacity = 1;
                                if (SpaceTimeController.JNow < layerStart)
                                {
                                    fadeOpacity = (float)((SpaceTimeController.JNow - fadeIn) / layer.FadeSpan.TotalDays);
                                }

                                if (SpaceTimeController.JNow > layerEnd)
                                {
                                    fadeOpacity = (float)((fadeOut - SpaceTimeController.JNow) / layer.FadeSpan.TotalDays);
                                }
                                if (thisMap.Frame.Reference == ReferenceFrames.Sky)
                                {
                                    layer.Astronomical = true;
                                }
                                layer.PreDraw(renderContext, opacity * fadeOpacity);
                            }
                        }
                    }

                }
            }
            if (nested)
            {
                foreach (LayerMap map in AllMaps[referenceFrame].ChildMaps.Values)
                {
                    if ((map.Frame.Reference == ReferenceFrames.Custom || map.Frame.Reference == ReferenceFrames.Identity))
                    {
                        PreDraw(renderContext, opacity, astronomical, map.Name, nested);
                    }
                }
            }
            renderContext.NominalRadius = oldNominalRadius;
            renderContext.World = matOld;
            renderContext.WorldBaseNonRotating = matOldNonRotating;
        }
#if !BASICWWT
        public static Guid CreateLayerFromString(string data, string name, string referenceFrame, bool showUI, int color, DateTime beginDate, DateTime endDate, FadeType fadeType, double fadeRange)
        {
            SpreadSheetLayer layer = new SpreadSheetLayer((string)data, true);
            layer.Enabled = true;
            layer.Name = name;
            layer.TimeSeries = true;
            layer.ReferenceFrame = referenceFrame;
            layer.Color = Color.FromArgb(color);
            layer.BeginRange = beginDate;
            layer.EndRange = endDate;
#if !WINDOWS_UWP
            if (showUI)
            {
                if (DataWizard.ShowWizard(layer) != System.Windows.Forms.DialogResult.OK)
                {
                    return Guid.Empty;
                }
            }
#endif
            LayerList.Add(layer.ID, layer);
            AllMaps[referenceFrame].Layers.Add(layer);
            AllMaps[referenceFrame].Open = true;
            version++;
            LoadTree();


            return layer.ID;
        }
#endif
        public static void UpdateLayerTime()
        {
#if !WINDOWS_UWP
            if (LayerManager.master != null && master.autoLoop)
            {
                master.UpdateLayerTimeLocal();
            }
#endif
        }

        internal static Dictionary<Guid, LayerInfo> GetVisibleLayerList(Dictionary<Guid, LayerInfo> previous)
        {
            Dictionary<Guid, LayerInfo> list = new Dictionary<Guid, LayerInfo>();

            foreach (Layer layer in LayerList.Values)
            {
                if (layer.Enabled)
                {
                    LayerInfo info = new LayerInfo();
                    info.StartOpacity = info.EndOpacity = layer.Opacity;
                    info.ID = layer.ID;
                    info.StartParams = layer.GetParams();


                    if (previous.ContainsKey(info.ID))
                    {
                        info.EndOpacity = previous[info.ID].EndOpacity;
                        info.EndParams = previous[info.ID].EndParams;
                    }
                    else
                    {
                        info.EndParams = layer.GetParams();
                    }
                    list.Add(layer.ID, info);
                }
            }
            return list;
        }

        internal static void SetVisibleLayerList(Dictionary<Guid, LayerInfo> list)
        {
            foreach (Layer layer in LayerList.Values)
            {
                layer.Enabled = list.ContainsKey(layer.ID);
                try
                {
                    if (layer.Enabled)
                    {
                        layer.Opacity = list[layer.ID].FrameOpacity;
                        layer.SetParams(list[layer.ID].FrameParams);
                    }
                }
                catch
                {
                }
            }
            SyncLayerState();
        }

        public static void SyncLayerState()
        {
            if (master != null)
            {
#if !WINDOWS_UWP
                if (master.InvokeRequired)
                {
                    System.Windows.Forms.MethodInvoker CallSync = delegate
                    {
                        master.SyncEnabledStatus();
                    };
                    try
                    {
                        master.Invoke(CallSync);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.SyncEnabledStatus();
                }
#endif
            }
        }

        public static bool InsideLayerManagerRect = false;

        public static void ConnectAllChildren()
        {
            foreach (LayerMap map in AllMaps.Values)
            {
                if (String.IsNullOrEmpty(map.Frame.Parent) && !LayerMaps.ContainsKey(map.Frame.Name))
                {
                    LayerMaps.Add(map.Name, map);
                }
                else if (!String.IsNullOrEmpty(map.Frame.Parent) && AllMaps.ContainsKey(map.Frame.Parent))
                {
                    if (!AllMaps[map.Frame.Parent].ChildMaps.ContainsKey(map.Frame.Name))
                    {
                        AllMaps[map.Frame.Parent].AddChild(map);
                    }
                }
            }
        }
        internal static bool CreateLayerGroup(string name, string referenceFrame)
        {
            LayerMap parent = AllMaps[referenceFrame];
            if (parent == null)
            {
                return false;
            }
            try
            {
                ReferenceFrame frame = new ReferenceFrame();
                frame.Name = name;
                frame.Reference = ReferenceFrames.Identity;
                LayerMap newMap = new LayerMap(frame.Name, ReferenceFrames.Identity);
                newMap.Frame = frame;
                newMap.Frame.SystemGenerated = false;
                parent.AddChild(newMap);
                newMap.Frame.Parent = parent.Name;
                AllMaps.Add(frame.Name, newMap);
                LoadTree();
                version++;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static Layer LoadLayer(string filename, string parentFrame, bool interactive, bool referenceFrameRightClick)
        {
            version++;
            if (filename.ToLower().EndsWith(".wwtl"))
            {
                return LoadLayerFile(filename, parentFrame, referenceFrameRightClick);
            }
            else if (filename.ToLower().EndsWith(".png") || filename.ToLower().EndsWith(".jpg"))
            {
                return LoadGroundOverlayFile(filename, parentFrame, interactive);
            }
            else if (filename.ToLower().EndsWith(".shp"))
            {
                return LoadShapeFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".3ds"))
            {
                return Load3dModelFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".obj"))
            {
                return Load3dModelFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".glb"))
            {
                return Load3dModelFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".wtml"))
            {
                return LoadWtmlFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".kml") || filename.ToLower().EndsWith(".kmz"))
            {
                return LoadKmlFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".tle"))
            {
                return LoadOrbitsFile(filename, parentFrame);
            }
            else if (filename.ToLower().EndsWith(".layers"))
            {
                LayerContainer layers = LayerContainer.FromFile(filename, false, parentFrame, referenceFrameRightClick);
                layers.Dispose();
                GC.SuppressFinalize(layers);
                LoadTree();
                return null;
            }
            else
            {
                return LoadDataTable(filename, parentFrame, interactive);
            }
        }

        public static Layer LoadLayerFile(string filename, string parentFrame, bool referenceFrameRightClick)
        {
            LayerContainer layers = LayerContainer.FromFile(filename, false, parentFrame, referenceFrameRightClick);
            //   layers.ClearTempFiles();
            ShowLayers(true);

            return layers.LastLoadedLayer;
        }

        private static void ShowLayers(bool show)
        {
#if WINDOWS_UWP

#else
            Earth3d.MainWindow.ShowLayersWindows = show;
#endif
        }


        internal static Layer LoadOrbitsFile(string path, string currentMap)
        {
            OrbitLayer layer = new OrbitLayer();
            layer.LoadData(path);
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            return layer;
        }

        private static Layer LoadGroundOverlayFile(string path, string parentFrame, bool interactive)
        {
            GroundOverlayLayer layer = new GroundOverlayLayer();

            layer.CreateFromFile(path);
            layer.Overlay.north = RenderEngine.Engine.viewCamera.Lat + 5;
            layer.Overlay.south = RenderEngine.Engine.viewCamera.Lat - 5;
            layer.Overlay.west = RenderEngine.Engine.viewCamera.Lng - 5;
            layer.Overlay.east = RenderEngine.Engine.viewCamera.Lng + 5;

            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
#if !WINDOWS_UWP
            if (interactive)
            {
                GroundOverlayProperties props = new GroundOverlayProperties();
                props.Overlay = layer.Overlay;
                props.OverlayLayer = layer;
                props.Owner = Earth3d.MainWindow;
                props.Show();
            }
#endif
            return layer;
        }

        private static void AddGreatCircleLayer()
        {
            GreatCirlceRouteLayer layer = new GreatCirlceRouteLayer();
            layer.LatStart = RenderEngine.Engine.viewCamera.Lat;
            layer.LatEnd = RenderEngine.Engine.viewCamera.Lat - 5;
            layer.LngStart = RenderEngine.Engine.viewCamera.Lng;
            layer.LngEnd = RenderEngine.Engine.viewCamera.Lng + 5;
            layer.Width = 4;
            layer.Enabled = true;
            layer.Name = Language.GetLocalizedText(1144, "Great Circle Route");
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
#if !WINDOWS_UWP
            GreatCircleProperties props = new GreatCircleProperties();
            props.Layer = layer;
            props.Owner = Earth3d.MainWindow;
            props.Show();
#endif
        }

        private static Layer LoadKmlFile(string path, string parentFrame)
        {
            KmlRoot newRoot = new KmlRoot(path, (KmlRoot)null);
            KmlLayer layer = new KmlLayer();
            layer.root = newRoot;
            KmlCollection.UpdateRootLinks(layer.root, RenderEngine.Engine.KmlViewInfo);
            layer.UpdateRetainedVisuals();
            layer.Enabled = true;
            layer.Name = path.Substring(path.LastIndexOf('\\') + 1);
            LayerList.Add(layer.ID, layer);
            layer.ReferenceFrame = currentMap;
            AllMaps[currentMap].Layers.Add(layer);
            AllMaps[currentMap].Open = true;
            version++;
            LoadTree();
            if (newRoot.children != null && newRoot.children.Count > 0)
            {
                if (newRoot.children[0].LookAt != null)
                {
                    KmlLayerUI.GotoLookAt(newRoot.children[0]);
                }
            }
            return layer;
        }

        private static Layer LoadWtmlFile(string filename, string parentFrame)
        {
            IImageSet imageset = null;
            Folder newFolder = Folder.LoadFromFile(filename, false);

            if (newFolder.Children.Length > 0)
            {

                if (newFolder.Children[0] is Place)
                {
                    Place place = (Place)newFolder.Children[0];
                    if (place.BackgroundImageSet != null && place.BackgroundImageSet.ImageSet != null)
                    {
                        imageset = place.BackgroundImageSet.ImageSet;
                    }
                    else if (place.StudyImageset != null && place.StudyImageset != null)
                    {
                        imageset = place.StudyImageset;
                    }

                }
                else if (newFolder.Children[0] is IImageSet)
                {
                    imageset = (IImageSet)newFolder.Children[0];
                }
            }
            else
            {
                return null;
            }

            Layer layer = AddImagesetLayer(imageset);
            version++;
            return layer;
        }

        public static Guid LoadLayer(string name, string referenceFrame, string filename, int color, DateTime beginDate, DateTime endDate, FadeType fadeType, double fadeRange)
        {
            try
            {
                Layer layer = LoadLayer(filename, referenceFrame, false, false);
                layer.StartTime = beginDate;
                layer.EndTime = endDate;
                layer.Color = Color.FromArgb(color);
                if (!string.IsNullOrEmpty(name))
                {
                    layer.Name = name;
                    version++;
                    LoadTree();
                }
                return layer.ID;
            }
            catch
            {
                return Guid.Empty;
            }
        }



        public static string GetLayerPropByID(Guid ID, string propName, out Layer layer)
        {
            if (LayerList.ContainsKey(ID))
            {
                layer = LayerList[ID];
                return layer.GetProp(propName);
            }
            else
            {
                layer = null;
                return "";
            }

        }

        public static string GetLayerPropsByID(Guid ID)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                return layer.GetProps();
            }
            else
            {
                return "";
            }
        }


        public static bool SetLayerPropByID(Guid ID, string propName, string propValue)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];
                bool retVal = layer.SetProp(propName, propValue);
                layer.CleanUp();
                LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }

        }

        public static void LoadTree()
        {
            if (master != null)
            {
#if !WINDOWS_UWP
                if (master.InvokeRequired)
                {
                    System.Windows.Forms.MethodInvoker updatePlace = delegate
                    {
                        master.LoadTreeLocal();
                    };
                    try
                    {
                        master.Invoke(updatePlace);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    master.LoadTreeLocal();
                }
#endif
            }
        }

        internal static string GetLayerList(bool layersOnly)
        {
            MemoryStream ms = new MemoryStream();
            using (XmlTextWriter xmlWriter = new XmlTextWriter(ms, System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("LayerApi");
                xmlWriter.WriteElementString("Status", "Success");
                xmlWriter.WriteStartElement("LayerList");
                xmlWriter.WriteAttributeString("Version", LayerManager.Version.ToString());

                PrintLayers(xmlWriter, layersOnly, LayerMaps);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteFullEndElement();
                xmlWriter.Close();

            }
            byte[] data = ms.GetBuffer();
            return System.Text.Encoding.UTF8.GetString(data);
        }

        internal static bool SetLayerPropsByID(Guid ID, string xml)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];

                bool retVal = layer.SetProps(xml);
                layer.CleanUp();
                LoadTree();
                return retVal;
            }
            else
            {
                return false;
            }
        }


        private static void PrintLayers(XmlTextWriter xmlWriter, bool layersOnly, Dictionary<string, LayerMap> LayerMaps)
        {
            foreach (LayerMap map in LayerMaps.Values)
            {
                List<Layer> layers = map.Layers;
                if (!layersOnly)
                {
                    if (map.Frame.Reference == ReferenceFrames.Identity)
                    {
                        xmlWriter.WriteStartElement("LayerGroup");
                    }
                    else
                    {
                        xmlWriter.WriteStartElement("ReferenceFrame");
                    }
                    xmlWriter.WriteAttributeString("Name", map.Name);
                    xmlWriter.WriteAttributeString("Enabled", map.Enabled.ToString());
                }
                foreach (Layer layer in layers)
                {
                    xmlWriter.WriteStartElement("Layer");
                    xmlWriter.WriteAttributeString("Name", layer.Name);
                    xmlWriter.WriteAttributeString("ID", layer.ID.ToString());
                    xmlWriter.WriteAttributeString("Type", layer.GetType().ToString().Replace("TerraViewer.", ""));
                    xmlWriter.WriteAttributeString("Enabled", layer.Enabled.ToString());
                    xmlWriter.WriteAttributeString("Version", layer.Version.ToString());
                    xmlWriter.WriteEndElement();
                }

                PrintLayers(xmlWriter, layersOnly, map.ChildMaps);

                if (!layersOnly)
                {
                    xmlWriter.WriteEndElement();
                }
            }
        }

#if !WINDOWS_UWP
        internal static string GetLayerDataID(Guid ID)
        {
            if (LayerList.ContainsKey(ID))
            {
                Layer layer = LayerList[ID];

                SpreadSheetLayer sheet = layer as SpreadSheetLayer;
                if (sheet != null)
                {
                    return sheet.Table.ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "";
            }
        }
#endif
        internal static bool DeleteFrameByName(string referenceFrame)
        {
            if (AllMaps.ContainsKey(referenceFrame))
            {
                LayerMap target = AllMaps[referenceFrame];
                PurgeLayerMapDeep(target, true);
                version++;
                LoadTree();
                return true;
            }
            return false;
        }


    }
}
