// Copyright Microsoft Copr 2006
// Written by Jonathan Fay

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Configuration;

namespace TerraViewer
{
	/// <summary>
	/// Summary description for Constellation.
	/// </summary>
	public class Constellations
	{
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
		string url;
		public List<Lineset> lines;
        int pointCount = 0;
        bool boundry = false;
        bool noInterpollation = false;
        public bool ReadOnly = false;
        public Constellations(string name)
        {
            instances.Add(this);
            this.name = name;
			this.url = null;
            lines = new List<Lineset>();
            foreach(string abbrv in Abbreviations.Values)
            {
                lines.Add(new Lineset(abbrv));
            }
        }

        static List<Constellations> instances = new List<Constellations>();
        public static void CleanUpAll()
        {
            foreach(Constellations item in instances)
            {
                item.CleanUp();
            }
            if (NamesBatch != null)
            {
                NamesBatch.Dispose();
                GC.SuppressFinalize(NamesBatch);
                NamesBatch = null;
            }
            
        }

        private void CleanUp()
        {
            if (constellationVertexBuffers != null)
            {
                foreach (SimpleLineList11 vb in constellationVertexBuffers.Values)
                {
                    vb.Clear();
                }
                constellationVertexBuffers.Clear();
            }
        }

        string targetPath;
        string extention;

        static public string GetFigurePath(string name)
        {
            return string.Format("{0}{1}{2}", Properties.Settings.Default.CahceDirectory + @"data\figures\", name, ".wwtfig");
        }

        public Constellations(string name, string url, bool boundry, bool noInterpollation)
        {
            instances.Add(this);

            if (boundry && !noInterpollation)
            {
                boundries = new Dictionary<string, Lineset>();
            }
            this.noInterpollation = noInterpollation;
            this.boundry = boundry;
            lines = new List<Lineset>();
			this.name = name;
			this.url = url;
            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory);
            }

            if (boundry)
            {

                targetPath = Properties.Settings.Default.CahceDirectory + @"data\";
                extention = ".wwtbnd";
            }
            else
            {
                targetPath = Properties.Settings.Default.CahceDirectory + @"data\figures\";
                extention = ".wwtfig";
            }


            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            if (!string.IsNullOrEmpty(url))
            {
                DataSetManager.DownloadFile(url, targetPath + name + extention, false, true);
            }

            Lineset lineSet = null;

            try
            {
                using (StreamReader sr = new StreamReader(targetPath + name + extention))
                {
                    string line;
                    string abrv;
                    string abrvOld = "";
                    double ra;
                    double dec;
                    double lastRa = 0;
                    PointType type = PointType.Move;
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();

                        if (line.Substring(11, 2) == "- ")
                        {
                            line = line.Substring(0, 11) + " -" + line.Substring(13, (line.Length - 13));
                        }
                        if (line.Substring(11, 2) == "+ ")
                        {
                            line = line.Substring(0, 11) + " +" + line.Substring(13, (line.Length - 13));
                        }
                        dec = Convert.ToDouble(line.Substring(11, 10));
                        if (noInterpollation)
                        {
                            ra = Convert.ToDouble(line.Substring(0, 10));
                        }
                        else
                        {
                            ra = ((Convert.ToDouble(line.Substring(0, 10)) / 24.0 * 360) - 180);
                        }

                        abrv = line.Substring(23, 4).Trim();
                        if (!boundry)
                        {
                            if (line.Substring(28, 1).Trim() != "")
                            {
                                type = (PointType)Convert.ToInt32(line.Substring(28, 1));
                            }
                        }
                        else
                        {
                            if (this.noInterpollation && line.Substring(28, 1) != "O")
                            {
                                continue;
                            }
                        }

                        if (abrv != abrvOld)
                        {
                            type = PointType.Start;
                            lineSet = new Lineset(abrv);
                            lines.Add(lineSet);
                            if (boundry && !noInterpollation)
                            {
                                boundries.Add(abrv, lineSet);
                            }
                            abrvOld = abrv;
                            lastRa = 0;
                        }


                        if (this.noInterpollation)
                        {
                            if (Math.Abs(ra - lastRa) > 12)
                            {
                                ra = ra - (24 * Math.Sign(ra - lastRa));

                            }
                            lastRa = ra;

                        }
                        string starName = null;
                        if (line.Length > 30)
                        {
                            starName = line.Substring(30).Trim();
                        }

                        if (starName == null || starName != "Empty")
                        {
                            lineSet.Add(ra, dec, type, starName);
                        }
                        pointCount++;
                        type = PointType.Line;

                    }
                    sr.Close();
                }
            }
            catch
            {

            }
		}

        public void Save(string name)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Lineset ls in lines)
            {
                if (ls.Points.Count > 0)
                {
                    foreach (Linepoint pnt in ls.Points)
                    {
                        double ra = (pnt.RA + 180) / 360 * 24;
                        string line = String.Format("{0:00.0000000} {1}{2:00.0000000} {3}  {4} {5}", ra, Math.Sign(pnt.Dec) == -1 ? "-" : "+", Math.Abs(pnt.Dec), ls.Name, (int)pnt.PointType, pnt.Name);

                        sb.AppendLine(line);
                    }
                }
                else
                {
                        sb.AppendLine(String.Format("00.0000000 +00.0000000 {0}  0 Empty",  ls.Name));

                }
            }

            if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"data\figures\"))
            {
                Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"data\figures\");
            }

            string filename = GetFigurePath(name);

            File.WriteAllText(filename, sb.ToString());

        }
        public void SaveWkt(string name)
        {

            StringBuilder sb = new StringBuilder();




            foreach (Lineset ls in lines)
            {
                bool firstItem = true;
                bool firstShape = true;
                if (ls.Points.Count > 0)
                {
                    //sb.Append("Polygon (");
                    sb.Append("Linestring (");

                    if (firstShape)
                    {
                        firstShape = false;
                    }
                    else
                    {
                        sb.Append(",");
                    }
                    foreach (Linepoint pnt in ls.Points)
                    {
                        double ra = (pnt.RA + 180);
                        double Xcoord = ra;
                        double Ycoord = pnt.Dec;
                        if (firstItem)
                        {
                            firstItem = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }

                        sb.Append(Xcoord.ToString() + " " + Ycoord.ToString());
                    }
                    sb.Append(")");

                    sb.AppendLine();
                }
            }

            File.WriteAllText(name, sb.ToString());

        }

        protected const double RC = (Math.PI / 180.0);
        protected float radius = 1.0f;
        protected Vector3d RaDecTo3d(double lat, double lng)
        {
            return new Vector3d((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));

        }


        protected SharpDX.Vector3[] points;

        public Color DrawColor = Color.CadetBlue;
        public virtual bool Draw3D(RenderContext11 renderContext, bool showOnlySelected, float opacity, string focusConsteallation, bool reverse)
        {
            constToDraw = focusConsteallation;

            Lineset lsSelected = null;
            foreach (Lineset ls in this.lines)
            {
                bool enabled = boundry ? Settings.Active.ConstellationBoundariesFilter.IsSet(ls.Name) : Settings.Active.ConstellationFiguresFilter.IsSet(ls.Name);
                if (enabled)
                {
                    if (constToDraw == ls.Name && boundry)
                    {
                        lsSelected = ls;
                    }
                    else if (!showOnlySelected || !boundry)
                    {
                        DrawSingleConstellation(renderContext, ls, opacity, reverse, true);
                    }
                }
            }

            if (lsSelected != null)
            {
                DrawSingleConstellation(renderContext, lsSelected, opacity, reverse, true);
            }

            renderContext.setRasterizerState(TriangleCullMode.Off, true);
            
            return true;
        }

        static Text3dBatch NamesBatch;
        public static void DrawConstellationNames(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (NamesBatch == null)
            {
                InitializeConstellationNames();
            }
            NamesBatch.Draw(renderContext, opacity, drawColor);
        }

        public static void InitializeConstellationNames()
        {
            NamesBatch = new Text3dBatch(80);
            foreach (IPlace centroid in ConstellationNamePositions.Values)
            {
                Vector3d center = Coordinates.RADecTo3d(centroid.RA + 12, centroid.Dec, 1);
                Vector3d up = new Vector3d(0, 1, 0);
                string name = centroid.Name;

                name = FullNames[centroid.Constellation];

                if (centroid.Name == "Triangulum Australe")
                {
                    name = name.Replace(" ", "\n   ");
                }
                NamesBatch.Add(new Text3d(center, up, name, 80, .000125));
            }
        }

        static List<IImageSet> ConstellationArt;


        public static void DrawConstellationArt(RenderContext11 renderContext, float opacity, Color drawColor)
        {
            if (ConstellationArt == null)
            {
                InitializeArt();
            }

            renderContext.BlendMode = BlendMode.Additive;
            foreach (IImageSet imageset in ConstellationArt)
            {
                BlendState bs = PictureBlendStates[imageset.Name];
                bs.TargetState = Settings.Active.ConstellationArtFilter.IsSet(imageset.Name);

                if (bs.State)
                {
                    Earth3d.MainWindow.RenderEngine.PaintLayerFullTint11(imageset, opacity * bs.Opacity * 100 * (drawColor.A / 255.0f), drawColor);
                }
            }
            renderContext.BlendMode = BlendMode.Alpha;

        }

        private static void InitializeArt()
        {
            ConstellationArt = new List<IImageSet>();

            // see if there are any files in the art directory
            if (String.IsNullOrEmpty(Properties.Settings.Default.ConstellationArtFile))
            {
                foreach( string file in Directory.GetFiles(ArtworkPath,"*.wtml"))
                {
                    try
                    {
                        Folder art = Folder.LoadFromFile(file, false);
                        if (art.Group == FolderGroup.Constellation)
                        {
                            Properties.Settings.Default.ConstellationArtFile = file;
                            break;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            if (!(String.IsNullOrEmpty(Properties.Settings.Default.ConstellationArtFile)))
            {
                LoadConstellationArt(Properties.Settings.Default.ConstellationArtFile);
            }
        }
        
        private static void LoadConstellationArt(string filename)
        {
            if (ConstellationArt == null)
            {
                ConstellationArt = new List<IImageSet>();
            }

            if (filename.ToLower() == "default.wtml" || !File.Exists(filename))
            {
                filename = ArtworkPath + "default.wtml";
            }


            ConstellationArt.Clear();
            Folder art = Folder.LoadFromFile(filename, false);
            foreach (IPlace place in art.Items)
            {
                if (place.BackgroundImageSet != null && FullNames.ContainsKey(place.BackgroundImageSet.Name))
                {
                    ConstellationArt.Add(place.BackgroundImageSet);
                }

                if (place.StudyImageset != null && FullNames.ContainsKey(place.StudyImageset.Name))
                {
                    ConstellationArt.Add(place.StudyImageset);
                }
            }


        }

      
    
        


        public static bool UseCached = true;
        Dictionary<string, SimpleLineList11> constellationVertexBuffers = new Dictionary<string, SimpleLineList11>();
 
        private void DrawSingleConstellation(RenderContext11 renderContext, Lineset ls, float opacity, bool reverse, bool drawAllSky)
        {
            if (!constellationVertexBuffers.ContainsKey(ls.Name))
            {
                int count = ls.Points.Count;

                SimpleLineList11 linelist = new SimpleLineList11();
                linelist.DepthBuffered = false;
                constellationVertexBuffers[ls.Name] = linelist;

                Vector3d currentPoint = new Vector3d();
                Vector3d temp;
 
                for (int i = 0; i < count; i++)
                {

                    if (ls.Points[i].PointType == PointType.Move || i == 0)
                    {
                        currentPoint = RaDecTo3d(ls.Points[i].Dec, ls.Points[i].RA);
                    }
                    else
                    {
                        temp = RaDecTo3d(ls.Points[i].Dec, ls.Points[i].RA);
                        linelist.AddLine(currentPoint, temp);
                        currentPoint = temp;
                    }
                }

                if (boundry)
                {
                    temp = RaDecTo3d(ls.Points[0].Dec, ls.Points[0].RA);
                    linelist.AddLine(currentPoint, temp);    
                }
             }
 
            Color col;
            if (boundry)
            {
                if (constToDraw != ls.Name)
                {
                    col = Properties.Settings.Default.ConstellationBoundryColor;
                }
                else
                {
                    col = Properties.Settings.Default.ConstellationSelectionColor;
                }
            }
            else
            {
                col = Properties.Settings.Default.ConstellationFigureColor;
            }

            constellationVertexBuffers[ls.Name].DrawLines(renderContext, opacity , col);

        }

      
        int GetTransparentColor(int color, float opacity)
        {
            Color inColor = Color.FromArgb(color);
            Color outColor = Color.FromArgb((byte)(opacity * 255f), inColor);
            return outColor.ToArgb();
        }

        public static Constellations Containment = null;
        static string constToDraw = "";

        public static Linepoint SelectedSegment = null;

        public string FindConstellationForPoint(double ra, double dec)
        {
            if (dec > 88.402)
            {
                return "UMI";
            }

            foreach (Lineset ls in this.lines)
            {
                int count = ls.Points.Count;

                int i;
                int j;
                bool inside = false;
                for ( i = 0, j= count-1; i < count; j = i++)
                {
                    
                    if ((((ls.Points[i].Dec <= dec) && (dec < ls.Points[j].Dec)) ||
                     ((ls.Points[j].Dec <= dec) && (dec < ls.Points[i].Dec))) &&
                    (ra < (ls.Points[j].RA - ls.Points[i].RA) * (dec - ls.Points[i].Dec) / (ls.Points[j].Dec - ls.Points[i].Dec) + ls.Points[i].RA))
                    {

                        inside = !inside;
                    }
                }
                if (inside)
                {
                    //constToDraw = ls.Name;
                    return ls.Name;
                }
            }
            if (ra > 0)
            {
                return FindConstellationForPoint(ra - 24, dec);
            }

            // Ursa Minor is tricky since it wraps around the poles. I can evade the point in rect test
            if (dec > 65.5)
            {
                return "UMI";
            }
            if (dec < -65.5)
            {
                return "OCT";
            }
            return "Error";
        }

        public static Dictionary<string, Lineset> boundries = null;
        public static Dictionary<String, String> FullNames;
        public static Dictionary<String, String> Abbreviations;
        
        public static Dictionary<string, BlendState> PictureBlendStates;
        public static Dictionary<string, IPlace> ConstellationCentroids;
        public static Dictionary<string, IPlace> ConstellationNamePositions;

        static Constellations()
        {
            try
            {

                if (!Directory.Exists(Properties.Settings.Default.CahceDirectory))
                {
                    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory);
                }

                if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"data"))
                {
                    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"data");
                }
                ConstellationCentroids = new Dictionary<string, IPlace>();
                DataSetManager.DownloadFile("http://www.worldwidetelescope.org/data/constellationNames_RADEC_EN.txt", Properties.Settings.Default.CahceDirectory + @"data\constellationNamesRADEC.txt", true, true);
                FullNames = new Dictionary<string, string>();
                Abbreviations = new Dictionary<string, string>();
                
                PictureBlendStates = new Dictionary<string, BlendState>();
                int id = 0;

                StreamReader sr = new StreamReader(Properties.Settings.Default.CahceDirectory + @"data\constellationNamesRADEC.txt");
                string line;
                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();
                    string[] data = line.Split(new char[] { ',' });

                    ConstellationFilter.BitIDs.Add(data[1], id++);
                    PictureBlendStates.Add(data[1], new BlendState(false, 1000, 0));

                    ConstellationCentroids.Add(data[1], new TourPlace(data[0], Convert.ToDouble(data[3]), Convert.ToDouble(data[2]), Classification.Constellation, data[1], ImageSetType.Sky, 360));
                }
                sr.Close();

                ConstellationNamePositions = new Dictionary<string, IPlace>();
                DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?q=ConstellationNamePositions_EN", Properties.Settings.Default.CahceDirectory + @"data\ConstellationNamePositions.txt", true, true);
                sr = new StreamReader(Properties.Settings.Default.CahceDirectory + @"data\ConstellationNamePositions.txt");
                while (sr.Peek() >= 0)
                {
                    line = sr.ReadLine();
                    string[] data = line.Split(new char[] { ',' });
                    ConstellationNamePositions.Add(data[1], new TourPlace(data[0], Convert.ToDouble(data[3]), Convert.ToDouble(data[2]), Classification.Constellation, data[1], ImageSetType.Sky, 360));
                }
                sr.Close();

                string path = ArtworkPath;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                DataSetManager.DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?W=hevelius", ArtworkPath + "default.wtml", false, true);
                AddAllNameMappings();
                ConstellationFilter.InitSets();
            }
            catch
            {
            }
        }

        static public void AddAllNameMappings()
        {
            AddNameMapping("AND", Language.GetLocalizedText(1179, "Andromeda"));
            AddNameMapping("ANT", Language.GetLocalizedText(1180, "Antlia"));
            AddNameMapping("APS", Language.GetLocalizedText(1181, "Apus"));
            AddNameMapping("AQR", Language.GetLocalizedText(1182, "Aquarius"));
            AddNameMapping("AQL", Language.GetLocalizedText(1183, "Aquila"));
            AddNameMapping("ARA", Language.GetLocalizedText(1184, "Ara"));
            AddNameMapping("ARI", Language.GetLocalizedText(1185, "Aries"));
            AddNameMapping("AUR", Language.GetLocalizedText(1186, "Auriga"));
            AddNameMapping("BOO", Language.GetLocalizedText(1187, "Bootes"));
            AddNameMapping("CAE", Language.GetLocalizedText(1188, "Caelum"));
            AddNameMapping("CAM", Language.GetLocalizedText(1189, "Camelopardalis"));
            AddNameMapping("CNC", Language.GetLocalizedText(1190, "Cancer"));
            AddNameMapping("CVN", Language.GetLocalizedText(1191, "Canes Venatici"));
            AddNameMapping("CMA", Language.GetLocalizedText(1192, "Canis Major"));
            AddNameMapping("CMI", Language.GetLocalizedText(1193, "Canis Minor"));
            AddNameMapping("CAP", Language.GetLocalizedText(1194, "Capricornus"));
            AddNameMapping("CAR", Language.GetLocalizedText(1195, "Carina"));
            AddNameMapping("CAS", Language.GetLocalizedText(1196, "Cassiopeia"));
            AddNameMapping("CEN", Language.GetLocalizedText(1197, "Centaurus"));
            AddNameMapping("CEP", Language.GetLocalizedText(1198, "Cepheus"));
            AddNameMapping("CET", Language.GetLocalizedText(1199, "Cetus"));
            AddNameMapping("CHA", Language.GetLocalizedText(1200, "Chamaeleon"));
            AddNameMapping("CIR", Language.GetLocalizedText(1201, "Circinus"));
            AddNameMapping("COL", Language.GetLocalizedText(1202, "Columba"));
            AddNameMapping("COM", Language.GetLocalizedText(1203, "Coma Berenices"));
            AddNameMapping("CRA", Language.GetLocalizedText(1204, "Corona Australis"));
            AddNameMapping("CRB", Language.GetLocalizedText(1205, "Corona Borealis"));
            AddNameMapping("CRV", Language.GetLocalizedText(1206, "Corvus"));
            AddNameMapping("CRT", Language.GetLocalizedText(1207, "Crater"));
            AddNameMapping("CRU", Language.GetLocalizedText(1208, "Crux"));
            AddNameMapping("CYG", Language.GetLocalizedText(1209, "Cygnus"));
            AddNameMapping("DEL", Language.GetLocalizedText(1210, "Delphinus"));
            AddNameMapping("DOR", Language.GetLocalizedText(1211, "Dorado"));
            AddNameMapping("DRA", Language.GetLocalizedText(1212, "Draco"));
            AddNameMapping("EQU", Language.GetLocalizedText(1213, "Equuleus"));
            AddNameMapping("ERI", Language.GetLocalizedText(1214, "Eridanus"));
            AddNameMapping("FOR", Language.GetLocalizedText(1215, "Fornax"));
            AddNameMapping("GEM", Language.GetLocalizedText(1216, "Gemini"));
            AddNameMapping("GRU", Language.GetLocalizedText(1217, "Grus"));
            AddNameMapping("HER", Language.GetLocalizedText(1218, "Hercules"));
            AddNameMapping("HOR", Language.GetLocalizedText(1219, "Horologium"));
            AddNameMapping("HYA", Language.GetLocalizedText(1220, "Hydra"));
            AddNameMapping("HYI", Language.GetLocalizedText(1221, "Hydrus"));
            AddNameMapping("IND", Language.GetLocalizedText(1222, "Indus"));
            AddNameMapping("LAC", Language.GetLocalizedText(1223, "Lacerta"));
            AddNameMapping("LEO", Language.GetLocalizedText(1224, "Leo"));
            AddNameMapping("LMI", Language.GetLocalizedText(1225, "Leo Minor"));
            AddNameMapping("LEP", Language.GetLocalizedText(1226, "Lepus"));
            AddNameMapping("LIB", Language.GetLocalizedText(1227, "Libra"));
            AddNameMapping("LUP", Language.GetLocalizedText(1228, "Lupus"));
            AddNameMapping("LYN", Language.GetLocalizedText(1229, "Lynx"));
            AddNameMapping("LYR", Language.GetLocalizedText(1230, "Lyra"));
            AddNameMapping("MEN", Language.GetLocalizedText(1231, "Mensa"));
            AddNameMapping("MIC", Language.GetLocalizedText(1232, "Microscopium"));
            AddNameMapping("MON", Language.GetLocalizedText(1233, "Monoceros"));
            AddNameMapping("MUS", Language.GetLocalizedText(1234, "Musca"));
            AddNameMapping("NOR", Language.GetLocalizedText(1235, "Norma"));
            AddNameMapping("OCT", Language.GetLocalizedText(1236, "Octans"));
            AddNameMapping("OPH", Language.GetLocalizedText(1237, "Ophiuchus"));
            AddNameMapping("ORI", Language.GetLocalizedText(1238, "Orion"));
            AddNameMapping("PAV", Language.GetLocalizedText(1239, "Pavo"));
            AddNameMapping("PEG", Language.GetLocalizedText(1240, "Pegasus"));
            AddNameMapping("PER", Language.GetLocalizedText(1241, "Perseus"));
            AddNameMapping("PHE", Language.GetLocalizedText(1242, "Phoenix"));
            AddNameMapping("PIC", Language.GetLocalizedText(1243, "Pictor"));
            AddNameMapping("PSC", Language.GetLocalizedText(1244, "Pisces"));
            AddNameMapping("PSA", Language.GetLocalizedText(1245, "Piscis Austrinus"));
            AddNameMapping("PUP", Language.GetLocalizedText(1246, "Puppis"));
            AddNameMapping("PYX", Language.GetLocalizedText(1247, "Pyxis"));
            AddNameMapping("RET", Language.GetLocalizedText(1248, "Reticulum"));
            AddNameMapping("SGE", Language.GetLocalizedText(1249, "Sagitta"));
            AddNameMapping("SGR", Language.GetLocalizedText(1250, "Sagittarius"));
            AddNameMapping("SCO", Language.GetLocalizedText(1251, "Scorpius"));
            AddNameMapping("SCL", Language.GetLocalizedText(1252, "Sculptor"));
            AddNameMapping("SCT", Language.GetLocalizedText(1253, "Scutum"));
            AddNameMapping("SER1", Language.GetLocalizedText(1254, "Serpens Caput"));
            AddNameMapping("SER2", Language.GetLocalizedText(1255, "Serpens Cauda"));
            AddNameMapping("SEX", Language.GetLocalizedText(1256, "Sextans"));
            AddNameMapping("TAU", Language.GetLocalizedText(1257, "Taurus"));
            AddNameMapping("TEL", Language.GetLocalizedText(1258, "Telescopium"));
            AddNameMapping("TRI", Language.GetLocalizedText(1259, "Triangulum"));
            AddNameMapping("TRA", Language.GetLocalizedText(1260, "Triangulum Australe"));
            AddNameMapping("TUC", Language.GetLocalizedText(1261, "Tucana"));
            AddNameMapping("UMA", Language.GetLocalizedText(1262, "Ursa Major"));
            AddNameMapping("UMI", Language.GetLocalizedText(1263, "Ursa Minor"));
            AddNameMapping("VEL", Language.GetLocalizedText(1264, "Vela"));
            AddNameMapping("VIR", Language.GetLocalizedText(1265, "Virgo"));
            AddNameMapping("VOL", Language.GetLocalizedText(1266, "Volans"));
            AddNameMapping("VUL", Language.GetLocalizedText(1267, "Vulpecula"));
        }

        static public void AddNameMapping(string abbrev, string name)
        {
            FullNames.Add(abbrev, name);
            Abbreviations.Add(name, abbrev);
        }

        static public string FullName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                if (FullNames.ContainsKey(name))
                {
                    return FullNames[name];
                }
                return name;
            }
            return "";
        }
        static public string Abbreviation(string name)
        {
            if (!String.IsNullOrEmpty(name) && Abbreviations.ContainsKey(name))
            {
                return Abbreviations[name];
            }

            return name;
        }

        internal void ResetConstellation(string name)
        {
            if (constellationVertexBuffers.ContainsKey(name))
            {
                constellationVertexBuffers[name].Clear();
                constellationVertexBuffers.Remove(name);
            }
        }

        public static string ArtworkPath
        {
            get
            {
                return Properties.Settings.Default.CahceDirectory + @"\data\artwork\";
            }
        }

        internal static void ImportArtFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = Language.GetLocalizedText(107, "WorldWide Telescope Collection") + "|*.wtml";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string filename = openFile.FileName;

                try
                {
                    Folder newFolder = Folder.LoadFromFile(filename, false);
                    newFolder.Type = FolderType.Sky;
                    newFolder.Group = FolderGroup.Constellation;
                    string path = ArtworkPath;
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    filename = Guid.NewGuid().ToString() + ".wtml";
                    newFolder.SaveToFile(path + filename);
                    Properties.Settings.Default.ConstellationArtFile = path + filename;
                }
                catch
                {
                    MessageBox.Show(Language.GetLocalizedText(109, "This file does not seem to be a valid collection"), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
                }
            }
        }
    }
}
