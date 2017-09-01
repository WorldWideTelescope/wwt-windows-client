using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Globalization;
using System.Xml.Serialization;
using System.Threading;

namespace TerraViewer
{
    public class WmsLayer : Layer, TerraViewer.ITimeSeriesDescription
    {
        public KmlGroundOverlay Overlay = new KmlGroundOverlay();
        public string filename;

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            string fName = filename;

            bool copy = !fName.Contains(ID.ToString());

            string fileName = fc.TempDirectory + string.Format("{0}\\{1}.png", fc.PackageID, this.ID.ToString());
            string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);

            if (copy)
            {
                if (File.Exists(fName) && !File.Exists(fileName))
                {
                    File.Copy(fName, fileName);
                }

            }

            if (File.Exists(fileName))
            {
                fc.AddFile(fileName);
            }
        }

        public override bool CanCopyToClipboard()
        {
            return false;
        }

        public override void CleanUp()
        {
            WmsCache.CleanUp();
            
            base.CleanUp();
        }

        public override void CopyToClipboard()
        {
            base.CopyToClipboard();
        }

        public override bool PreDraw(RenderContext11 renderContext, float opacity)
        {
            SetCurrentImage();

            Overlay.color = Color.FromArgb((int)(this.Opacity * opacity * Color.A), Color);
            RenderEngine.Engine.KmlMarkers.AddGroundOverlay(Overlay);
            return true;
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            return true;
        }

        [LayerProperty]
        public double North
        {
            get { return Overlay.north; }
            set
            {
                if (Overlay.north != value)
                {
                    version++;

                    Overlay.north = value;
                }
            }
        }

        [LayerProperty]
        public double South
        {
            get { return Overlay.south; }
            set
            {
                if (Overlay.south != value)
                {
                    version++;
                    Overlay.south = value;
                }
            }
        }

        [LayerProperty]
        public double East
        {
            get { return Overlay.east; }
            set
            {
                if (Overlay.east != value)
                {
                    version++;
                    Overlay.east = value;
                }
            }
        }

        [LayerProperty]
        public double West
        {
            get { return Overlay.west; }
            set
            {
                if (Overlay.west != value)
                {
                    version++;
                    Overlay.west = value;
                }
            }
        }
        int height = 0;
        [LayerProperty]
        public int Height
        {
            get { return height; }
            set
            {
                height = value;
            }
        }

        int width = 0;
        [LayerProperty]
        public int Width
        {
            get
            {
                return width; }
            set
            {
                width = value;
            }
        }

        [LayerProperty]
        public double Rotation
        {
            get { return Overlay.rotation; }
            set
            {
                if (Overlay.rotation != value)
                {
                    version++;
                    Overlay.rotation = value;
                }
            }
        }

        [LayerProperty]
        public double Altitude
        {
            get { return Overlay.altitude; }
            set
            {
                if (Overlay.altitude != value)
                {
                    version++;
                    Overlay.altitude = value;
                }
            }
        }
        DateTime seriesStartTime = new DateTime();

        [LayerProperty]
        public DateTime SeriesStartTime
        {
            get
            {
                return seriesStartTime;
            }
            set
            {
                seriesStartTime = value;
            }
        }


        DateTime seriesEndTime = new DateTime();

        [LayerProperty]
        public DateTime SeriesEndTime
        {
            get
            {
                return seriesEndTime;
            }
            set
            {
                seriesEndTime = value;
            }
        }

        TimeSpan timeStep = new TimeSpan();

        [LayerProperty]
        public TimeSpan TimeStep
        {
            get
            {
                return timeStep;
            }
            set
            {
                timeStep = value;
            }
        }

        string serviceUrl = "http://svs.gsfc.nasa.gov/cgi-bin/wms?";
        
        [LayerProperty]
        public string ServiceUrl
        {
            get { return serviceUrl; }
            set { serviceUrl = value; }
        }

        string layers;
        [LayerProperty]
        public string Layers
        {
            get { return layers; }
            set { layers = value; }
        }

        string styles;
        [LayerProperty]
        public string Styles
        {
            get { return styles; }
            set { styles = value; }
        }

        string wmsVersion;
        [LayerProperty]
        public string WmsVersion
        {
            get { return wmsVersion; }
            set { wmsVersion = value; }
        }

        
        public List<TimeRange> TimeRanges = new List<TimeRange>();

        public void UpdateTimeRange()
        {
            if (TimeRanges.Count == 0)
            {
                return;
            }

            SeriesStartTime = TimeRanges[0].StartTime;
            SeriesEndTime = TimeRanges[TimeRanges.Count - 1].EndTime;
            TimeStep = TimeSpan.FromSeconds(1);

        }

        string MakeWmsGetMapUrl(string layers, string styles, double west, double north, double east, double south, int width, int height, string time, string elevation)
        {
            string val = string.Format("{0}version={9}&service=WMS&request=GetMap&layers={1}&styles={2}&crs=CRS:84&srs=EPSG:4326&bbox={3},{4},{5},{6}&width={7}&height={8}&format=image/png&transparent=TRUE",
                                            ServiceUrl, layers, styles, west, south, east, north, width, height, wmsVersion);

            if (!string.IsNullOrEmpty(time))
            {
                val += "&time=" + time;
            }
            return val;
        }

        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteAttributeString("North", North.ToString());
            xmlWriter.WriteAttributeString("South", South.ToString());
            xmlWriter.WriteAttributeString("East", East.ToString());
            xmlWriter.WriteAttributeString("West", West.ToString());
            xmlWriter.WriteAttributeString("Rotation", Rotation.ToString());
            xmlWriter.WriteAttributeString("Altitude", Altitude.ToString());

            xmlWriter.WriteAttributeString("Height", Height.ToString());
            xmlWriter.WriteAttributeString("Width", Width.ToString());
            xmlWriter.WriteAttributeString("SeriesStartTime", SeriesStartTime.ToString("o"));
            xmlWriter.WriteAttributeString("SeriesEndTime", SeriesEndTime.ToString("o"));
            xmlWriter.WriteAttributeString("TimeStep", TimeStep.ToString());
            xmlWriter.WriteAttributeString("ServiceUrl", ServiceUrl);
            xmlWriter.WriteAttributeString("Layers", Layers);
            xmlWriter.WriteAttributeString("Styles", Styles);
            xmlWriter.WriteAttributeString("Version", WmsVersion);
            xmlWriter.WriteAttributeString("TimeRanges", GetDateRangeString());
        }

        private string GetDateRangeString()
        {
            StringBuilder sb = new StringBuilder();
            bool firstTime = true;
            foreach (TimeRange tr in TimeRanges)
            {
                if (!firstTime)
                {
                    sb.Append(",");
                }
                else
                {
                    firstTime = false;
                }

                sb.Append(tr.StartTimeString);
                if (tr.IsRange)
                {
                    sb.Append("/");
                    sb.Append(tr.EndTimeString);
                    sb.Append("/");
                    sb.Append(tr.TimeStepString);
                }
            }

            return sb.ToString();
        }


        private void ParseRanges(string p)
        {
            string[] parts = p.Split(new char[] { ',' });
            foreach (string part in parts)
            {
                TimeRanges.Add(new TimeRange(part));
            }
            UpdateTimeRange();
        }


        WmsImageCache WmsCache = new WmsImageCache();

        public void SetCurrentImage()
        {
            string timeString = GetTimeString();
            string url = GetCurrentPath(timeString);

            Texture11 texture = WmsCache.GetTexture(timeString,url);

            Overlay.Icon = new KmlIcon();
            Overlay.Icon.Texture = texture;

            //Earth3d.MainWindow.Text = Overlay.Icon.Href;
            
        }

        private string GetTimeString()
        {
            string timeString="";

            if (TimeRanges.Count == 0)
            {
                return timeString;
            }

            if (TimeRanges.Count == 1)
            {
                return TimeRanges[0].GetDateStampForDate(SpaceTimeController.Now);
            }

            //TimeRange now = new TimeRange(SpaceTimeController.Now);


            int location = FindDateIndex(SpaceTimeController.Now);

            if (location < 0)
            {
                location = Math.Min(-location, TimeRanges.Count - 1);
            }


            return TimeRanges[location].GetDateStampForDate(SpaceTimeController.Now);
        }

        private int FindDateIndex(DateTime dateTime)
        {
            if (dateTime <= TimeRanges[0].StartTime)
            {
                return 0;
            }

            if (dateTime >= TimeRanges[TimeRanges.Count - 1].StartTime)
            {
                return TimeRanges.Count - 1;
            }

            int indexTarget = TimeRanges.Count / 2;
            int range = indexTarget;

            while (indexTarget < TimeRanges.Count - 1)
            {
                range = (range + 1) / 2;

                if (indexTarget < 0)
                {
                    return 0;
                }

                if (dateTime >= TimeRanges[indexTarget].StartTime)
                {
                    // Going up
                    if (dateTime < TimeRanges[indexTarget + 1].StartTime)
                    {
                        return indexTarget;
                    }
                    indexTarget += range;
                    
                }
                else
                {
                    // going down;
                    indexTarget -= range;
                   
                }

            }
            return TimeRanges.Count - 1;
        }

        private string GetCurrentPath(string timeString)
        {
            

            return MakeWmsGetMapUrl(layers, styles, West, North, East, South, width, height,
                timeString, "");

        }

        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            North = Double.Parse(node.Attributes["North"].Value);

            South = Double.Parse(node.Attributes["South"].Value);

            East = double.Parse(node.Attributes["East"].Value);
            West = double.Parse(node.Attributes["West"].Value);
            Rotation = double.Parse(node.Attributes["Rotation"].Value);
            Altitude = double.Parse(node.Attributes["Altitude"].Value);

            Height = int.Parse( node.Attributes["Height"].Value);
            Width = int.Parse( node.Attributes["Width"].Value);
            SeriesStartTime = DateTime.Parse(node.Attributes["SeriesStartTime"].Value);
            SeriesEndTime = DateTime.Parse(node.Attributes["SeriesEndTime"].Value);
            TimeStep = TimeSpan.Parse(node.Attributes["TimeStep"].Value);
            ServiceUrl = node.Attributes["ServiceUrl"].Value;
            Layers = node.Attributes["Layers"].Value;
            Styles = node.Attributes["Styles"].Value;
            WmsVersion = node.Attributes["Version"].Value;
            ParseRanges(node.Attributes["TimeRanges"].Value);
        }

        

        public override void LoadData(string path)
        {
           
        }



        public bool IsTimeSeries
        {
            get
            {
                return timeStep.TotalSeconds > 0;
            }
            set
            {
                return;
            }
        }
    }

    public struct TimeRange : IComparable<TimeRange>
    {
        public TimeRange(DateTime start)
        {
            IsRange = false;
            StartTime = start;
            EndTime = start;
            TimeStep = new TimeSpan();
            StartTimeString = "";
            TimeStepString = "";
            EndTimeString = "";
        }


        public TimeRange(string dateString)
        {
            dateString = dateString.Replace("\n", "").Replace(" ", "");
            string[] dateParts = dateString.Split(new char[] { '/' });
            IsRange = false;
            StartTime = new DateTime();
            EndTime = new DateTime();
            TimeStep = new TimeSpan();
            StartTimeString = "";
            TimeStepString = "";
            EndTimeString = "";
            try
            {
                
                if (dateParts.Length > 0)
                {
                    StartTimeString = dateParts[0];
                    StartTime = DateTime.ParseExact(dateParts[0], new string[] { @"yyyy-MM-dd\THH:mm:ss.000\Z", @"yyyy-MM-dd\THH:mm:ss\Z", @"yyyy-MM-dd\THH:mm\Z", @"yyyy-MM-dd\THH\Z", @"yyyy-MM-dd", @"yyyy-MM", "o" }, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                    //layer.StartTime = DateTime.Parse(dateParts[0]);
                    if (StartTime.Kind == DateTimeKind.Local)
                    {
                        StartTime = StartTime.ToUniversalTime();
                    }
                    IsRange = false;
                }
                else
                {
                    StartTime = new DateTime();
                    StartTimeString = "";
                }

                if (dateParts.Length > 1)
                {
                    EndTimeString = dateParts[1];
                    EndTime = DateTime.ParseExact(dateParts[1], new string[] { @"yyyy-MM-dd\THH:mm:ss\Z", @"yyyy-MM-dd\THH:mm\Z", @"yyyy-MM-dd\THH\Z", @"yyyy-MM-dd", @"yyyy-MM" }, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

                    if (EndTime.Kind == DateTimeKind.Local)
                    {
                        EndTime = EndTime.ToUniversalTime();
                    }
                    IsRange = true;
                }
                else
                {
                    EndTime = StartTime;
                    EndTimeString = StartTimeString;
                }

                if (dateParts.Length > 2)
                {
                    TimeStepString = dateParts[2];
                    TimeStep = System.Xml.XmlConvert.ToTimeSpan(dateParts[2]);
                }
                else
                {
                    TimeStepString = "";
                    TimeStep = new TimeSpan();
                }
            }
            catch
            {
              
            }

       
        }

        public string GetDateStampForDate(DateTime now)
        {
            if (!IsRange)
            {
                return StartTimeString;
            }

            DateTime targetTime;
            if (SpaceTimeController.Now < StartTime)
            {
                return StartTimeString;
            }
            else if (SpaceTimeController.Now > EndTime)
            {
                return EndTimeString;
            }
            else
            {
                if (TimeStep.TotalDays == 30)
                {
                    DateTime tmp = now;

                    targetTime = new DateTime(tmp.Year, tmp.Month, 1);
                }
                else
                {
                    TimeSpan ts = now - StartTime;

                    int steps = (int)((ts.TotalSeconds / TimeStep.TotalSeconds) + .5);

                    targetTime = StartTime.Add(TimeSpan.FromSeconds(TimeStep.TotalSeconds * steps));
                }
            }

            if (TimeStep.TotalDays == 30)
            {
                DateTime tmp = targetTime;

                targetTime = new DateTime(tmp.Year, tmp.Month, 1);
            }


            string timeString = targetTime.ToString("s", System.Globalization.CultureInfo.InvariantCulture) + "Z";
            timeString = timeString.Replace("0001-01-01T00Z", "");

            if (TimeStep.TotalDays == 30)
            {
                timeString = timeString.Replace("-01T00:00:00Z", "");
            }


            if (TimeStep.TotalDays % 1 == 0)
            {
                timeString = timeString.Replace("T00:00:00Z", "");
            }

            if (TimeStep.TotalHours % 1 == 0)
            {
                timeString = timeString.Replace(":00:00Z", "");
            }

            if (TimeStep.TotalMinutes % 1 == 0)
            {
                timeString = timeString.Replace(":00Z", "");
            }

            return timeString;
        }

        public string StartTimeString;
        public string EndTimeString;
        public string TimeStepString;
        public DateTime StartTime;
        public DateTime EndTime;
        public TimeSpan TimeStep;
        public bool IsRange;

        #region IComparable<TimeRange> Members

        public int CompareTo(TimeRange other)
        {
            return StartTime.CompareTo(other.StartTime);
        }

        #endregion
    }

    class WmsCahceEntry
    {
        public WmsCahceEntry(string date, string url)
        {
            Date = date;
            URL = url;
            Texture = null;
        }
        public string Date;
        public string URL;
        public Texture11 Texture;
        public bool Requested = false;
        public bool Loaded = false;
        public int ErrorCount = 0;
    }

    class WmsImageCache : IDisposable
    {
        SortedDictionary<string, WmsCahceEntry> cache = new SortedDictionary<string, WmsCahceEntry>();

        public Texture11 GetTexture(string date, string url)
        {
            if (!cache.ContainsKey(date) )
            {
                Add(date, url);
            }

            WmsCahceEntry result = cache[date];

            if (result.Texture != null)
            {
                return result.Texture;
            }

            Texture11 tempTexture = null;

            foreach (WmsCahceEntry entry in cache.Values)
            {
                 
                if ((entry.Date.CompareTo(date) > 0 && tempTexture != null ))
                {
                    return tempTexture;
                }

                if (entry.Texture != null)
                {
                    tempTexture = entry.Texture;
                }
            }

            return tempTexture;
        }

        private void Add(string date, string url)
        {
            WmsCahceEntry entry = new WmsCahceEntry(date, url);
            cache.Add(date, entry);

            entry.Requested = true;

            // Do a background load on this
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadTexture), entry);
        }

        public static int MaxErrorCount = 3;

        public static void LoadTexture(object objEntry)
        {
            WmsCahceEntry entry = objEntry as WmsCahceEntry;
            entry.Requested = true;
            try
            {
                String dir = Properties.Settings.Default.CahceDirectory + "Data\\KmlCache\\";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                // This is a expanded timeout version of WebClient
                MyWebClient Client = new MyWebClient();


                string filename = dir + ((uint)entry.URL.GetHashCode32()).ToString() + ".png";


                Stream stream = null;

                if (Uri.IsWellFormedUriString(entry.URL, UriKind.Absolute))
                {
                    byte[] data = Client.DownloadData(entry.URL);
                    stream = new MemoryStream(data);
                }
                else
                {
                    stream = File.Open(entry.URL, FileMode.Open);
                }


                FileStream fileStream = File.OpenWrite(filename);
                byte[] buffer = new byte[32768];
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        break;
                    fileStream.Write(buffer, 0, read);
                }
                stream.Close();
                fileStream.Close();
                //todo this was loaded from the kmlicon API. Any Problems with that?
                entry.Texture = Texture11.FromFile(filename);

                entry.Loaded = true;

            }
            catch
            {

                entry.ErrorCount++;

                // retry until sucsess or MaxError Count exceeded
                if (entry.ErrorCount < MaxErrorCount)
                {
                    entry.Requested = false;
                    entry.Loaded = false;

                }
                else
                {
                    entry.Loaded = true;
                }
            }
            finally
            {
                //    mut.ReleaseMutex();
            }
        }

        public void Dispose()
        {
            CleanUp();
        }

        public void CleanUp()
        {
            foreach (WmsCahceEntry entry in cache.Values)
            {
                if (entry.Texture != null)
                {
                    entry.Texture.Dispose();
                }
            }
            cache.Clear();
        }
    }
}
