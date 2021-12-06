
using System;
using System.Collections.Generic;
using System.Text;
using ShapefileTools;
using System.Drawing;
using System.Data;
using System.IO;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.SqlServer.Types;
using System.Data.SqlTypes;
using System.Xml;
using System.Net;


using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    public class SpreadSheetLayer : TimeSeriesLayer
    {
        public SpreadSheetLayer()
        {
        }

        SpreadSheetLayerUI primaryUI = null;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new SpreadSheetLayerUI(this);
            }

            return primaryUI;
        }

        public override string[] Header
        {
            get { return table.Header; }
        }

        public SpreadSheetLayer(string filename)
        {
            string data = File.ReadAllText(filename);
            LoadFromString(data, false, false, false, true);
            ComputeDateDomainRange(-1, -1);
        }

        public override bool CanCopyToClipboard()
        {

            return true;
        }

        public override void CopyToClipboard()
        {
            //todo copy binary format

            
            Clipboard.SetText(table.ToString());

        }

        


        bool dataDirty = false;
        public SpreadSheetLayer(string data, bool something)
        {
            LoadFromString(data, false, false, false, true);
            ComputeDateDomainRange(-1, -1);
        }

        public override bool DynamicUpdate()
        {
            string data = GetDatafromFeed(DataSourceUrl);
            if (data != null)
            {
                UpadteData(data, false, true, true);
                GuessHeaderAssignments();
                return true;
            }
            return false;
        }

        private static string GetDatafromFeed(string url)
        {
            string xml = ExecuteQuery(url);

            if (xml == null)
            {
                return null;
            }

            try
            {

                XmlDocument xmlDoc = new XmlDocument();
                XmlNamespaceManager xmlNsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
                xmlNsMgr.AddNamespace("atom", "http://www.w3.org/2005/Atom");
                xmlNsMgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                xmlNsMgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");

                xmlDoc.LoadXml(xml);
                XmlNodeList elements = xmlDoc.DocumentElement.SelectNodes("./atom:entry", xmlNsMgr);
                StringBuilder sb = new StringBuilder();

                if (elements != null && elements.Count > 0)
                {
                    // Add ODATA properties as first row
                    XmlNodeList properties = elements[0].SelectSingleNode("./atom:content/m:properties", xmlNsMgr).ChildNodes;
                    int columnCount = 1;
                    foreach (XmlNode property in properties)
                    {
                        if (columnCount != 1)
                        {
                            sb.Append("\t");
                        }

                        sb.Append(property.Name.Substring(property.Name.IndexOf(":") + 1, property.Name.Length - property.Name.IndexOf(":") - 1));
                        columnCount++;
                    }

                    sb.AppendLine(string.Empty);

                    // Add ODATA property values from second row onwards
                    foreach (XmlNode element in elements)
                    {
                        XmlNodeList propertyValues = element.SelectSingleNode("./atom:content/m:properties", xmlNsMgr).ChildNodes;
                        // Reset Column Count
                        columnCount = 1;
                        foreach (XmlNode propertyValue in propertyValues)
                        {
                            if (columnCount != 1)
                            {
                                sb.Append("\t");
                            }

                            sb.Append(propertyValue.InnerText);
                            columnCount++;
                        }

                        sb.AppendLine(string.Empty);
                    }
                }

                return sb.ToString();
            }
            catch
            {
                return xml;
            }
        }

        private static string ExecuteQuery(string url)
        {
            try
            {

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Method = "GET";
                request.Accept = "application/atom+xml, text/plain";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader readStream = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                    {
                        return readStream.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public override bool UpadteData(object data, bool purgeOld, bool purgeAll, bool hasHeader)
        {

            LoadFromString(data as string, true, purgeOld, purgeAll, hasHeader);
            ComputeDateDomainRange(-1, -1);
            dataDirty = true;
            return true;
        }

        public override void LoadData(string path)
        {
            table = Table.Load(path, '\t');
            ComputeDateDomainRange(-1, -1);

            if (DynamicData && AutoUpdate)
            {
                DynamicUpdate();
            }
        }
 

        string fileName;
        public override void AddFilesToCabinet(FileCabinet fc)
        {
            //if (string.IsNullOrEmpty(fileName))
            {
                fileName = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID,this.ID.ToString());
            }
            string dir = fileName.Substring(0, fileName.LastIndexOf("\\"));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(fileName) || dataDirty)
            {
                table.Save(fileName);
            }

            fc.AddFile(fileName);

            base.AddFilesToCabinet(fc);
        }

        public void GuessHeaderAssignments()
        {
            int index = 0;
            foreach (string headerName in table.Header)
            {
                string name = headerName.ToLower();

                if (name.Contains("lat") && latColumn == -1)
                {
                    latColumn = index;
                }

                if ((name.Contains("lon") || name.Contains("lng")) && lngColumn == -1)
                {
                    lngColumn = index;
                }

                if (name.Contains("dec") && latColumn == -1)
                {
                    latColumn = index;
                    astronomical = true;
                }

                if ((name.Contains("ra") || name.Contains("ascen")) && lngColumn == -1)
                {
                    lngColumn = index;
                    astronomical = true;
                    pointScaleType = PointScaleTypes.StellarMagnitude;
                }

                if ((name.Contains("mag") || name.Contains("size")) && sizeColumn == -1)
                {
                    sizeColumn = index;
                }

                if ((name.Contains("date") || name.Contains("time") || name.Contains("dt") || name.Contains("tm")))
                {
                    if (name.Contains("end") && endDateColumn == -1)
                    {
                        endDateColumn = index;
                    }
                    else if (startDateColumn == -1)
                    {
                        startDateColumn = index;
                    }
                }
            

                if ((name.Contains("altitude") || name.Contains("alt")) && altColumn == -1)
                {
                    altColumn = index;
                    AltType = AltTypes.Altitude;
                    AltUnit = AltUnits.Meters;
                }

                if (name.Contains("depth") && altColumn == -1)
                {
                    altColumn = index;
                    AltType = AltTypes.Depth;
                    AltUnit = AltUnits.Kilometers;
                }

                if (name.StartsWith("x") && XAxisColumn == -1)
                {
                    XAxisColumn = index;
                }

                if (name.StartsWith("y") && YAxisColumn == -1)
                {
                    YAxisColumn = index;
                }

                if (name.StartsWith("z") && ZAxisColumn == -1)
                {
                    ZAxisColumn = index;
                }

                if (name.Contains("color") && ColorMapColumn == -1)
                {
                    ColorMapColumn = index;
                }

                if ((name.Contains("geometry") || name.Contains("geography")) && geometryColumn == -1)
                {
                    geometryColumn = index;
                } 
                index++;
            }

            if (table.Header.Length > 0)
            {
                nameColumn = 0;
            }
        }
        

        public override void ComputeDateDomainRange(int columnStart, int columnEnd)
        {
            if (columnStart == -1)
            {
                columnStart = startDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = endDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = columnStart;
            }

            BeginRange = DateTime.MaxValue;
            EndRange = DateTime.MinValue;

            foreach (string[] row in table.Rows)
            {
                try
                {
                    if (columnStart > -1)
                    {
                        bool sucsess = false;
                        DateTime dateTimeStart = DateTime.MinValue;
                        sucsess = TryParseDate(row[columnStart], out dateTimeStart);

                        DateTime dateTimeEnd = DateTime.MinValue;


                        if (sucsess && dateTimeStart < BeginRange)
                        {
                            BeginRange = dateTimeStart;
                        }
                        if (columnEnd > -1)
                        {
                            sucsess = TryParseDate(row[columnEnd], out dateTimeEnd);
                            if (sucsess && dateTimeEnd > EndRange)
                            {
                                EndRange = dateTimeEnd;
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void ComputeDateRange(int columnStart, int columnEnd, out DateTime beginRange, out DateTime endRange)
        {
            if (columnStart == -1)
            {
                columnStart = startDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = endDateColumn;
            }

            if (columnEnd == -1)
            {
                columnEnd = columnStart;
            }

            beginRange = DateTime.MaxValue;
            endRange = DateTime.MinValue;

            int count = 0;

            foreach (string[] row in table.Rows)
            {
                bool selected = Filters.Count == 0;
                bool firstFilter = true;
                foreach (FilterGraphTool fgt in Filters)
                {
                    ColumnStats filter = fgt.Stats;
                    if (filter.Computed && (selected || firstFilter))
                    {
                        if (filter.IsSelected(row))
                        {
                            selected = true;
                        }
                        else
                        {
                            selected = false;
                        }
                        firstFilter = false;

                    }
                }
                if (selected)
                {

                    try
                    {
                        if (columnStart > -1)
                        {
                            bool sucsess = false;
                            DateTime dateTimeStart = DateTime.MinValue;
                            sucsess = TryParseDate(row[columnStart], out dateTimeStart);

                            DateTime dateTimeEnd = DateTime.MinValue;


                            if (sucsess && dateTimeStart < beginRange)
                            {
                                beginRange = dateTimeStart;
                                count++;
                            }
                            if (columnEnd > -1)
                            {
                                sucsess = TryParseDate(row[columnEnd], out dateTimeEnd);
                                if (sucsess && dateTimeEnd > endRange)
                                {
                                    endRange = dateTimeEnd;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }


        public ColumnStats GetSingleColumnHistogram(int column)
        {
         
            

            ColumnStats stats = new ColumnStats();
            stats.Computed = true;
            stats.Buckets = 20;
            stats.TargetColumn = column;
            stats.DomainColumn = -1;
            List<double> sortList = new List<double>();
            table.Lock();
            stats.Max = double.MinValue;
            stats.Min = double.MaxValue;
            if (column > -1)
            {
                // First Pass - Basic statistics..
                foreach (string[] row in table.Rows)
                {
                    bool selected = Filters.Count == 0;
                    bool firstFilter = true;
                    foreach (FilterGraphTool fgt in Filters)
                    {
                        ColumnStats filter = fgt.Stats;
                        if (filter.Computed && (selected || firstFilter))
                        {
                            if (filter.IsSelected(row))
                            {
                                selected = true;
                            }
                            else
                            {
                                selected = false;
                            }
                            firstFilter = false;

                        }
                    }
                    if (selected)
                    {
                        try
                        {

                            bool sucsess = false;
                            double val = 0;
                            sucsess = double.TryParse(row[column], out val);
                            if (sucsess)
                            {
                                if (val > stats.Max)
                                {
                                    stats.Max = val;
                                }

                                if (val < stats.Min)
                                {
                                    stats.Min = val;
                                }
                                stats.Count++;
                                stats.Sum += val;
                                sortList.Add(val);
                            }
                        }

                        catch
                        {
                        }
                    }
                }
            }
            table.Unlock();

            //2nd pass does not use the table anymore. working from sortList
            if (stats.Count > 0)
            {
                stats.Average = stats.Sum / stats.Count;
                sortList.Sort();
                int midPoint = stats.Count / 2;
                if (stats.Count % 2 == 0)
                {
                    stats.Median = (sortList[midPoint] + sortList[midPoint + 1]) / 2;
                }
                else
                {
                    stats.Median = (sortList[midPoint]);
                }

                stats.BucketWidth = (stats.Max - stats.Min) / stats.Buckets;

                stats.Histogram = new double[stats.Buckets];

                foreach (double v in sortList)
                {
                    int bucket = (int)((v - stats.Min) / stats.BucketWidth);
                    stats.Histogram[Math.Min(stats.Buckets - 1, bucket)]++;
                }

                stats.Selected = new bool[stats.Buckets];
                for (int i = 0; i < stats.Buckets; i++)
                {
                    stats.Selected[i] = false;
                }

                foreach (int max in stats.Histogram)
                {
                    if (stats.HistogramMax < max)
                    {
                        stats.HistogramMax = max;
                    }
                }
            }

            return stats;
        }

        

        public ColumnStats GetDateHistogram(int column, DateFilter type)
        {

            ColumnStats stats = new ColumnStats();
            stats.Computed = true;
            stats.Buckets = 20;
            stats.TargetColumn = column;
            stats.DomainColumn = -1;
            table.Lock();
            stats.Max = double.MinValue;
            stats.Min = double.MaxValue;

            switch (type)
            {
                case DateFilter.Year:
                    ComputeDateRange(column, -1, out stats.BeginDate, out stats.EndDate);
                    stats.BeginDate = stats.BeginDate.Date;
                    stats.EndDate = stats.EndDate.Date;
                    TimeSpan ts = stats.EndDate - stats.BeginDate;

                    stats.Buckets = (int)(ts.TotalDays + 1);
                    stats.DomainValues = new string[stats.Buckets];
                    DateTime now = stats.BeginDate;
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i]=now.ToShortDateString();
                        now = now.AddDays(1);
                    }

                    break;
                case DateFilter.Month:
                    stats.Buckets = 12;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = UiTools.GetMonthName(i, false);
                    }
                    break;
                case DateFilter.Day:
                    stats.Buckets = 31;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = (i+1).ToString();
                    }
                    break;
                case DateFilter.DayOfWeek:
                    stats.Buckets = 7;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = UiTools.GetDayName(i, false);
                    }
                    break;
                case DateFilter.DayOfYear:
                    stats.Buckets = 366;
                    stats.DomainValues = new string[stats.Buckets];
                    for (int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = (i + 1).ToString();
                    }
                    break;
                case DateFilter.Hour:
                    stats.Buckets = 24;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = UiTools.GetHourName(i);
                    }
                    break;
                case DateFilter.Minute:
                    stats.Buckets = 60;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = i.ToString();
                    }
                    break;
                case DateFilter.Second:
                    stats.Buckets = 60;
                    stats.DomainValues = new string[stats.Buckets];
                    
                    for(int i = 0; i < stats.Buckets; i++)
                    {
                        stats.DomainValues[i] = i.ToString();
                    }
                    break;
                default:
                    break;
            }       
            
            stats.Histogram = new double[stats.Buckets];
            stats.BucketWidth = 1;
            stats.DateFilter = type;
            // First Pass - Basic statistics..
            foreach (string[] row in table.Rows)
            {
                bool selected = Filters.Count == 0;
                bool firstFilter = true;
                foreach (FilterGraphTool fgt in Filters)
                {
                    ColumnStats filter = fgt.Stats;
                    if (filter.Computed && (selected || firstFilter))
                    {
                        if (filter.IsSelected(row))
                        {
                            selected = true;
                        }
                        else
                        {
                            selected = false;
                        }
                        firstFilter = false;

                    }
                }
                if (selected)
                {
                    try
                    {
                        if (column > -1)
                        {

                            DateTime date = ParseDate(row[column]);
                            int bucket = 0;
                            switch (type)
                            {
                                case DateFilter.Year:
                                    bucket = (date.Date - stats.BeginDate).Days;
                                    break;
                                case DateFilter.Month:
                                    bucket = date.Month - 1;
                                    break;
                                case DateFilter.Day:
                                    bucket = date.Day - 1;
                                    break;
                                case DateFilter.DayOfWeek:
                                    bucket = (int)date.DayOfWeek;
                                    break;
                                case DateFilter.DayOfYear:
                                    bucket = date.DayOfYear - 1;
                                    break;
                                case DateFilter.Hour:
                                    bucket = date.Hour;
                                    break;
                                case DateFilter.Minute:
                                    bucket = date.Minute;
                                    break;
                                case DateFilter.Second:
                                    bucket = date.Second;
                                    break;
                                default:
                                    break;
                            }

                            stats.Count++;
                            stats.Sum += 1;
                            stats.Histogram[bucket]++;
                        }


                    }
                    catch
                    {
                    }
                }
            }

            stats.Selected = new bool[stats.Buckets];
            for (int i = 0; i < stats.Buckets; i++)
            {
                stats.Selected[i] = false;
            }

            foreach (int max in stats.Histogram)
            {
                if (stats.HistogramMax < max)
                {
                    stats.HistogramMax = max;
                }
            }

            table.Unlock();




            return stats;
        }

        public void CheckState()
        {
          
        }
        public ColumnStats GetDomainValueBarChart(int domainColumn, int dataColumn, int denominatorColumn, StatTypes statType)
        {
            CheckState();
            ColumnStats stats = new ColumnStats();
            stats.Computed = true;
            Dictionary<string, int> domainIdList = new Dictionary<string, int>();
            stats.DomainValues = GetDomainValues(domainColumn);
            stats.Buckets = stats.DomainValues.Length;
            stats.Histogram = new double[stats.Buckets];
            stats.TargetColumn = dataColumn;
            stats.DomainColumn = domainColumn;
            stats.DomainStatType = statType;
            stats.DemoninatorColumn = denominatorColumn;

            List<double>[] sortLists = new List<double>[stats.Buckets];
            double[] min = new double[stats.Buckets];
            double[] max = new double[stats.Buckets];
            double[] sum = new double[stats.Buckets];
            double[] denominatorSum = new double[stats.Buckets];
            stats.Selected = new bool[stats.Buckets];

            for (int i = 0; i < stats.Buckets; i++)
            {
                domainIdList.Add(stats.DomainValues[i], i);
                max[i] = double.MinValue;
                min[i] = double.MaxValue;
                sortLists[i] = new List<double>();
                stats.Selected[i] = false;
            }

            table.Lock();


            // First Pass - Basic statistics..
            foreach (string[] row in table.Rows)
            {
                bool selected = Filters.Count == 0;
                bool firstFilter = true;
                foreach (FilterGraphTool fgt in Filters)
                {
                    ColumnStats filter = fgt.Stats;
                    if (filter.Computed && (selected || firstFilter))
                    {
                        if (filter.IsSelected(row))
                        {
                            selected = true;
                        }
                        else
                        {
                            selected = false;
                        }
                        firstFilter = false;

                    }
                }
                if (selected)
                {
                    try
                    {
                        if (dataColumn > -1)
                        {
                            int domainID = domainIdList[row[domainColumn]];

                            bool sucsess = false;
                            double val = 0;
                            sucsess = double.TryParse(row[dataColumn], out val);
                            if (sucsess)
                            {
                                if (val > max[domainID])
                                {
                                    max[domainID] = val;
                                }

                                if (val < min[domainID])
                                {
                                    min[domainID] = val;
                                }
                                stats.Count++;
                                stats.Sum += val;
                                sum[domainID] += val;
                                sortLists[domainID].Add(val);
                            }
                            else if (statType == StatTypes.Count) // && domainColumn == dataColumn)
                            {
                                sortLists[domainID].Add(0);
                            }

                            if (statType == StatTypes.Ratio && denominatorColumn != -1)
                            {
                                sucsess = double.TryParse(row[denominatorColumn], out val);
                                if (sucsess)
                                {
                                    denominatorSum[domainID] += val;
                                }
                            }




                        }
                    }
                    catch
                    {
                    }
                }
            }
            table.Unlock();

            //2nd pass does not use the table anymore. working from sortList

            for (int i = 0; i < stats.Buckets; i++)
            {
                if (sortLists[i].Count > 0)
                {
                    double average = sum[i] / sortLists[i].Count;
                    double median = 0;
                    sortLists[i].Sort();
                    int midPoint = Math.Max(0, (sortLists[i].Count / 2) - 1);

                    try
                    {
                        if (sortLists[i].Count % 2 == 0)
                        {
                            median = (sortLists[i][midPoint] + sortLists[i][midPoint + 1]) / 2;
                        }
                        else
                        {
                            median = (sortLists[i][midPoint]);
                        }
                    }
                    catch
                    {
                    }
                    switch (stats.DomainStatType)
                    {
                        case StatTypes.Count:
                            stats.Histogram[i] = sortLists[i].Count;
                            break;
                        case StatTypes.Average:
                            stats.Histogram[i] = average;
                            break;
                        case StatTypes.Median:
                            stats.Histogram[i] = median;
                            break;
                        case StatTypes.Sum:
                            stats.Histogram[i] = sum[i];
                            break;
                        case StatTypes.Min:
                            stats.Histogram[i] = min[i];
                            break;
                        case StatTypes.Max:
                            stats.Histogram[i] = max[i];
                            break;
                        case StatTypes.Ratio:
                            stats.Histogram[i] = sum[i] / denominatorSum[i];
                            break;
                        default:
                            break;
                    }
                }
            }

            foreach (double j in stats.Histogram)
            {
                if (stats.HistogramMax < j)
                {
                    stats.HistogramMax = j;
                }
            }

            return stats;
        }

      



        public double GetMaxValue(int column)
        {

            double max = 0;
            table.Lock();
            foreach (string[] row in table.Rows)
            {
                try
                {
                    if (column > -1)
                    {
                        bool sucsess = false;
                        double val = 0;
                        sucsess = double.TryParse(row[column], out val);

                        if (sucsess && val > max)
                        {
                            max = val;
                        }
                    }
                }
                catch
                {
                }
            }
            table.Unlock();
            return max;
        }

        public override string[] GetDomainValues(int column)
        {
            List<string> domainValues = new List<string>();
            table.Lock();
            foreach (string[] row in table.Rows)
            {
                bool selected = Filters.Count == 0;
                bool firstFilter = true;
                foreach (FilterGraphTool fgt in Filters)
                {
                    ColumnStats filter = fgt.Stats;
                    if (filter.Computed && (selected || firstFilter))
                    {
                        if (filter.IsSelected(row))
                        {
                            selected = true;
                        }
                        else
                        {
                            selected = false;
                        }
                        firstFilter = false;

                    }
                }
                if (selected)
                {
                    try
                    {
                        if (column > -1)
                        {
                            if (!domainValues.Contains(row[column]))
                            {
                                domainValues.Add(row[column]);
                            }
                        }
                    }

                    catch
                    {
                    }
                }
            }
            domainValues.Sort();
            table.Unlock();
            return domainValues.ToArray();
        }


        int barChartBitmask = 0;

        public int BarChartBitmask
        {
            get { return barChartBitmask; }
            set { barChartBitmask = value; }
        }
        double barScaleFactor = 20;

        protected void MakeBarChart(string[] row, double lat, double lng, double scale, double factor, Color color, bool selected, Dates date)
        {
            if (barChartBitmask == 0)
            {
                return;
            }

            Vector3d center = Coordinates.GeoTo3dDouble(lat, lng, 1);
            Vector3d up = Coordinates.GeoTo3dDouble(lat + 90, lng, 1);
            Vector3d right = Vector3d.Cross(center, up);
            Vector3d upleft = Vector3d.Subtract(up, right);
            upleft.Normalize();
            upleft.Multiply(.0005);

            Vector3d upright = Vector3d.Add(up, right);
            upright.Normalize();
            upright.Multiply(.0005);

            Vector3d base1 = Vector3d.Add(center, upright);
            Vector3d base2 = Vector3d.Add(center, upleft);
            Vector3d base3 = Vector3d.Subtract(center, upleft);
            Vector3d base4 = Vector3d.Subtract(center, upright);


            double currentBase = 1;

            int colorIndex = 0;

            TriangleList targetList = triangleList2d;

            for (int col = 0; col < Header.Length; col++)
            {
                if (((int)Math.Pow(2, col) & barChartBitmask) > 0)
                {

                    double alt = 0;
                    if (!double.TryParse(row[col], out alt))
                    {
                        alt = 0;
                    }
                    alt = factor * alt;

                    double alt2 = 1 + (factor * (alt + currentBase) / meanRadius);



                    Vector3d top1 = base1;
                    Vector3d top2 = base2;
                    Vector3d top3 = base3;
                    Vector3d top4 = base4;
                    top1.Normalize();
                    top2.Normalize();
                    top3.Normalize();
                    top4.Normalize();

                    top1.Multiply(alt2);
                    top2.Multiply(alt2);
                    top3.Multiply(alt2);
                    top4.Multiply(alt2);

                    Color currentColor = (colorIndex == 0) ? Color.FromArgb(192, color) : Color.FromArgb(128, Color.Yellow);
                    targetList.AddQuad(top1, top2, top3, top4, currentColor, date);
                    // this.triangleList.AddQuad(top

                    targetList.AddQuad(top2, top1, base2, base1, currentColor, date);
                    targetList.AddQuad(top4, top2, base4, base2, currentColor, date);
                    targetList.AddQuad(top1, top3, base1, base3, currentColor, date);
                    targetList.AddQuad(top3, top4, base3, base4, currentColor, date);


                    base1 = top1;
                    base2 = top2;
                    base3 = top3;
                    base4 = top4;
                    colorIndex++;
                    currentBase += alt;
                }
            }
        }




        private List<FilterGraphTool> filters = new List<FilterGraphTool>();

        public List<FilterGraphTool> Filters
        {
            get { return filters; }
            set { filters = value; }
        }

        public void AddFilter(FilterGraphTool fgt )
        {     
            fgt.ComputeChart();
            Filters.Add(fgt);
            primaryUI.UpdateNodes();
        }

        private double meanRadius = 6371000;

        double ecliptic = 23.5;
        
        protected override bool PrepVertexBuffer(float opacity)
        {
            table.Lock();

            CleanAndReady = true;

            if (lineList != null)
            {
                lineList.Clear();
            }

            if (lineList2d != null)
            {
                lineList2d.Clear();
            }        
            if (triangleList != null)
            {
                triangleList.Clear();
            }

            if (triangleList2d != null)
            {
                triangleList2d.Clear();
            }

            if (textBatch != null)
            {
                textBatch.CleanUp();
            }

            if (lineList == null)
            {
                lineList = new LineList();
            }

            lineList.TimeSeries = this.timeSeries;

            if (lineList2d == null)
            {
                lineList2d = new LineList();
                lineList2d.DepthBuffered = false;

            }

            lineList.TimeSeries = this.timeSeries;



            if (triangleList == null)
            {
                triangleList = new TriangleList();
            }

            if (triangleList2d == null)
            {
                triangleList2d = new TriangleList();
                triangleList2d.DepthBuffered = false;
            }

            List<TimeSeriesPointVertex> vertList = new List<TimeSeriesPointVertex>();
            List<UInt32> indexList = new List<UInt32>();
            TimeSeriesPointVertex lastItem = new TimeSeriesPointVertex();
            positions.Clear();
            UInt32 currentIndex = 0;

            Color color = Color.FromArgb((int)((float)Color.A ), Color); 
      
            // for space 3d
            ecliptic = Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI;

            DateTime baseDate = new DateTime(2010,1,1,12,00,00);


            foreach (FilterGraphTool fgt in Filters)
            {
                ColumnStats filter = fgt.Stats;
                filter.SelectDomain.Clear();
                if (filter.Computed)
                {
                    if (filter.DomainColumn > -1)
                    {
                        int i = 0;
                        foreach (string domainValue in filter.DomainValues)
                        {
                            if (filter.Selected[i])
                            {
                                filter.SelectDomain.Add(domainValue, filter.Selected[i]);
                            }
                            i++;
                        }
                    }
                }
            }

            double mr = LayerManager.AllMaps[ReferenceFrame].Frame.MeanRadius;
            if (mr != 0)
            {
                meanRadius = mr;
            }

            
            foreach (string[] row in table.Rows)
            {
                try
                {
                    bool selected = false;
                    bool firstFilter = true;
                    foreach (FilterGraphTool fgt in Filters)
                    {
                        ColumnStats filter = fgt.Stats;
                        if (filter.Computed && (selected || firstFilter))
                        {
                            if (filter.IsSelected(row))
                            {
                                selected = true;
                            }
                            else
                            {
                                selected = false;
                            }
                            firstFilter = false;

                        }
                    }

                    if (geometryColumn > -1 || (this.CoordinatesType == CoordinatesTypes.Spherical && (lngColumn > -1 && latColumn > -1)) || ((this.CoordinatesType == CoordinatesTypes.Rectangular) && (XAxisColumn > -1 && YAxisColumn > -1)))
                    {
                        double Xcoord = 0;
                        double Ycoord = 0;
                        double Zcoord = 0;

                        double alt = 1;
                        double altitude = 0;
                        double distParces = 0;
                        double factor = UiTools.GetScaleFactor(AltUnit, 1);
                        if (altColumn == -1 || AltType == AltTypes.SeaLevel || bufferIsFlat)
                        {
                            alt = 1;
                            if (astronomical & !bufferIsFlat)
                            {
                                alt = UiTools.AuPerLightYear * 100;
                            }
                        }
                        else
                        {
                            if (AltType == AltTypes.Depth)
                            {
                                factor = -factor;
                            }

                            if (!double.TryParse(row[altColumn], out alt))
                            {
                                alt = 0;
                            }

                            if (astronomical)
                            {
                                factor = factor / (1000 * UiTools.KilometersPerAu);
                                distParces = (alt * factor) / UiTools.AuPerParsec;

                                altitude = (factor * alt);
                                alt = (factor * alt);
                            }
                            else if (AltType == AltTypes.Distance)
                            {
                                altitude = (factor * alt);
                                alt = (factor * alt / meanRadius);
                            }
                            else
                            {
                                altitude = (factor * alt);
                                alt = 1 + (factor * alt / meanRadius);
                            }
                        }



                        if (CoordinatesType == CoordinatesTypes.Spherical && lngColumn > -1 && latColumn > -1)
                        {
                            Xcoord = Coordinates.Parse(row[lngColumn]);
                            Ycoord = Coordinates.Parse(row[latColumn]);

                            if (astronomical)
                            {
                                if (RaUnits == RAUnits.Hours)
                                {
                                    Xcoord *= 015;
                                }
                                if (bufferIsFlat)
                                {
                                    Xcoord += 180;
                                }
                            }
                            if (!astronomical)
                            {
                                double offset = EGM96Geoid.Height(Ycoord, Xcoord);

                                altitude += offset;
                                alt += offset / meanRadius;
                            }
                            Vector3d pos = Coordinates.GeoTo3dDouble(Ycoord, Xcoord, alt);

                            lastItem.Position = pos.Vector311;

                            positions.Add(lastItem.Position);

                        }
                        else if (this.CoordinatesType == CoordinatesTypes.Rectangular)
                        {
                            double xyzScale = UiTools.GetScaleFactor(CartesianScale, CartesianCustomScale) / meanRadius;

                            if (ZAxisColumn > -1)
                            {
                                Zcoord = Convert.ToDouble(row[ZAxisColumn]);
                            }

                            Xcoord = Convert.ToDouble(row[XAxisColumn]);
                            Ycoord = Convert.ToDouble(row[YAxisColumn]);

                            if (XAxisReverse)
                            {
                                Xcoord = -Xcoord;
                            }
                            if (YAxisReverse)
                            {
                                Ycoord = -Ycoord;
                            }
                            if (ZAxisReverse)
                            {
                                Zcoord = -Zcoord;
                            }


                            lastItem.Position = new Vector3((float)(Xcoord * xyzScale), (float)(Zcoord * xyzScale), (float)(Ycoord * xyzScale));
                            positions.Add(lastItem.Position);
                        }


                        switch (ColorMap)
                        {
                            case ColorMaps.Same_For_All:
                                lastItem.Color = color;
                                break;
                            case ColorMaps.Per_Column_Literal:
                                if (ColorMapColumn > -1)
                                {
                                    lastItem.Color = ParseColor(row[ColorMapColumn], color);
                                }
                                else
                                {
                                    lastItem.Color = color;
                                }
                                break;
                            //case ColorMaps.Group_by_Range:
                            //    break;
                            //case ColorMaps.Gradients_by_Range:
                            //    break;       
                            case ColorMaps.Group_by_Values:
                                if (!ColorDomainValues.ContainsKey(row[ColorMapColumn]))
                                {
                                    MakeColorDomainValues();
                                }
                                lastItem.Color = Color.FromArgb(ColorDomainValues[row[ColorMapColumn]].MarkerIndex);
                                break;

                            default:
                                break;
                        }

                        if (selected)
                        {
                            lastItem.Color = Color.Yellow;
                        }

                        if (sizeColumn > -1)
                        {
                            switch (pointScaleType)
                            {
                                case PointScaleTypes.Linear:
                                    lastItem.PointSize = Convert.ToSingle(row[sizeColumn]);
                                    break;
                                case PointScaleTypes.Log:
                                    lastItem.PointSize = (float)Math.Log(Convert.ToSingle(row[sizeColumn]));
                                    break;
                                case PointScaleTypes.Power:
                                    {
                                        double size = 0;
                                        if (double.TryParse(row[sizeColumn], out size))
                                        {
                                            lastItem.PointSize = (float)Math.Pow(2, size);
                                        }
                                        else
                                        {
                                            lastItem.PointSize = 0;
                                        }
                                    }
                                    break;
                                case PointScaleTypes.StellarMagnitude:
                                    {
                                        double size = 0;
                                        if (double.TryParse(row[sizeColumn], out size))
                                        {

                                            if (!bufferIsFlat)
                                            {
                                                size = size - 5 * ((Math.Log10(distParces) - 1));
                                                lastItem.PointSize = (float)(120000000 / Math.Pow(1.6, size));
                                            }
                                            else
                                            {
                                                lastItem.PointSize = (float)(40 / Math.Pow(1.6, size));
                                            }

                                        }
                                        else
                                        {
                                            lastItem.PointSize = 0;
                                        }
                                        
                                    }
                                    break;

                                case PointScaleTypes.Constant:
                                    lastItem.PointSize = 1;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            lastItem.PointSize = (float)1;
                        }
                        if (PlotType == PlotTypes.Point)
                        {
                            lastItem.PointSize = (float)1;
                        }


                        if (startDateColumn > -1)
                        {
                            DateTime dateTime = ParseDate(row[startDateColumn]);
                            lastItem.Tu = (float)(SpaceTimeController.UtcToJulian(dateTime) - SpaceTimeController.UtcToJulian(baseDate));

                            if (endDateColumn > -1)
                            {
                                dateTime = ParseDate(row[endDateColumn]);

                                lastItem.Tv = (float)(SpaceTimeController.UtcToJulian(dateTime) - SpaceTimeController.UtcToJulian(baseDate));
                            }
                            else
                            {
                                lastItem.Tv = lastItem.Tu;
                            }
                        }

                        if ((this.CoordinatesType == CoordinatesTypes.Spherical && (lngColumn > -1 && latColumn > -1)) || ((this.CoordinatesType == CoordinatesTypes.Rectangular) && (XAxisColumn > -1 && YAxisColumn > -1)))
                        {
                            vertList.Add(lastItem);
                            if (barChartBitmask != 0)
                            {
                                MakeBarChart(row, Ycoord, Xcoord, lastItem.PointSize, factor, lastItem.Color, selected, new Dates(lastItem.Tu, lastItem.Tv));
                            }
                        }

                        if (geometryColumn > -1)
                        {

                            ParseGeometry(row[geometryColumn], lastItem.Color, lastItem.Color, altitude, new Dates(lastItem.Tu, lastItem.Tv));

                        }

                        currentIndex++;
                    }
                }
                catch
                {
                }
                lines = false;
            }

            shapeVertexCount = vertList.Count;
            if (vertList.Count != 0)
            {
                shapeFileVertex = new TimeSeriesPointSpriteSet(RenderContext11.PrepDevice, vertList.ToArray());
                table.Unlock();
                return true;
            }
            
            table.Unlock();
            return false;
        }

        private void ParseGeometry(string gs, Color lineColor, Color polyColor, double alt, Dates date)
        {


            gs = gs.Trim();

            int index = gs.IndexOf('(');

            if (index < 0)
            {
                return;
            }

            if (!gs.EndsWith(")"))
            {
                return;
            }
            string commandPart = gs.Substring(0, index ).Trim();

            string parens = gs.Substring(index);

            string[] parts = commandPart.Split(new char[] { ' ' });

            string command = null;
            string mods = null;
            if (parts.Length > 0)
            {
                foreach (string item in parts)
                {
                    if (string.IsNullOrEmpty(command))
                    {
                        command = item;
                    }
                    else if (string.IsNullOrEmpty(mods))
                    {
                        mods = item;
                    }
                }
            }

            switch (command.ToLower())
            {
                case "multipolygon":
                case "polygon":
                    {
                        ParsePolygon(parens, mods, lineColor, polyColor, alt, date);
 
                    }
                    break;
                case "multilinestring":
                    {
                        ParseLineString(parens, mods, lineColor, alt, false, date);
                    }
                    break;         
                case "linestring":
                    {
                        ParseLineString(parens, mods, lineColor, alt, true, date);
                    }
                    break;
                case "geometrycollection":
                    {
                         parens = parens.Substring(1, parens.Length - 2);
                         string[] shapes = UiTools.SplitString(parens, ',');
                         foreach (string shape in shapes)
                         {
                             ParseGeometry(shape, lineColor, polyColor, alt, date);
                         }
                    }
                    break;
                case "text":
                    {
                        ParseTextString(parens, mods, lineColor, polyColor, alt, date);
                    }
                    break;
                default:
                    break;
            }

        }

        // TEXT WKT like syntax
        //
        // Text("String to Display", sizeInDegrees, lat lng alt, rotation tilt bank)

        private void ParseTextString(string parens, string mods, Color lineColor, Color polyColor, double alt, Dates date)
        {
            if (!parens.StartsWith("(") && parens.EndsWith(")"))
            {
                return;
            }
            try
            {
                parens = parens.Substring(1, parens.Length - 2);

                string[] parts = UiTools.SplitString(parens, ',');

                if (textBatch == null)
                {
                    textBatch = new Text3dBatch();
                }

                string text = parts[0];
                double rawSize = float.Parse(parts[1].Trim());
                float textSize = (float)(.00012f * rawSize);

                // Test to compare angle vs est angle.
                //double textAngle = 2 * Math.Tan(((rawSize/180)*Math.PI) /2);


                string[] lla = parts[2].Trim().Split(new char[] { ' ' });

                double lat = double.Parse(lla[1]);
                double lng = double.Parse(lla[0]);
                if (astronomical && bufferIsFlat)
                {
                    lng -= 180;
                }
                double altitude = alt == 0 ? 1 : alt;


                if (lla.Length > 2)
                {
                    altitude = 1 + double.Parse(lla[2]) / (astronomical ? 1 : meanRadius);
                }

                if (alt == 0 && astronomical && !bufferIsFlat)
                {
                    altitude = 1000 * UiTools.AuPerLightYear;

                    textSize = (float)(textSize * altitude);

                }

                if (alt == 0 && astronomical && !bufferIsFlat)
                {
                    altitude = 1000 * UiTools.AuPerLightYear;

                    textSize = (float)(textSize * altitude);

                }
                Vector3d location = Coordinates.GeoTo3dDouble(lat, lng, altitude);
                Vector3d up = Coordinates.GeoTo3dDouble(lat + 90, lng, 1);

                double rotation = 0;
                double tilt = 0;
                double bank = 0;

                if (parts.Length > 3)
                {
                    string[] rtb = parts[3].Trim().Split(new char[] { ' ' });
                    rotation = double.Parse(rtb[0]);
                    if (rtb.Length > 1)
                    {
                        tilt = double.Parse(rtb[1]);
                    }

                    if (rtb.Length > 2)
                    {
                        bank = double.Parse(rtb[2]);
                    }
                }


                Text3d text3d = new Text3d(location, up, text, astronomical ? 1 : -1, textSize);
                text3d.Color = polyColor;
                text3d.Rotation = rotation;
                text3d.Tilt = tilt;
                text3d.Bank = bank;
                textBatch.Add(text3d);
            }
            catch
            {
            }
            
        }



        private void ParsePolygon(string parens, string mods, Color lineColor, Color polyColor, double alt, Dates date)
        {

            if (!parens.StartsWith("(") && parens.EndsWith(")"))
            {
                return;
            }
            // string the top level of parens
            parens = parens.Substring(1, parens.Length - 2);

            string[] shapes = UiTools.SplitString(parens, ',');
            foreach (string shape in shapes)
            {
                KmlLineList lineList = new KmlLineList();
                lineList.Astronomical = astronomical;
                lineList.MeanRadius = meanRadius;
                lineList.ParseWkt(shape, mods, alt, date);
                if (alt == 0)
                {
                    AddPolygonFlat(false, lineList, 1, polyColor, lineColor, true, true, date);
                }
                else
                {
                    AddPolygon(false, lineList, 1, polyColor, lineColor, true, true, date);
                }
            }
        }

        private void ParseLineString(string parens, string mods, Color lineColor, double alt, bool single, Dates date)
        {

            if (!parens.StartsWith("(") && parens.EndsWith(")"))
            {
                return;
            }
            if (!single)
            {
                // string the top level of parens
                parens = parens.Substring(1, parens.Length - 2);
            }
            string[] shapes = UiTools.SplitString(parens, ',');
            foreach (string shape in shapes)
            {
                KmlLineList lineList = new KmlLineList();
                lineList.Astronomical = astronomical;
                lineList.MeanRadius = meanRadius;

                lineList.ParseWkt(shape, mods, alt, date);


                AddPolygon(!bufferIsFlat && astronomical, lineList, 1, Color.White, lineColor, false, false, date);
            }


        }

        private string[] SplitShapes(string shapes)
        {
            List<string> shapeList = new List<string>();

            int nesting = 0;

            int current = 0;
            while (current < shapes.Length)
            {
                if (shapes[current] == '(')
                {
                    nesting++;
                }
            }

            return shapeList.ToArray();
        }

        private void AddPolygon(bool sky, KmlLineList geo, float lineWidth, Color polyColor, Color lineColor, bool extrude, bool fill, Dates date)
        {

            //todo can we save this work for later?
            List<Vector3d> vertexList = new List<Vector3d>();
            List<Vector3d> vertexListGround = new List<Vector3d>();

            //todo list 
            // We need to Wrap Around for complete polygone
            // we aldo need to do intereor
            //todo space? using RA/DEC
            for (int i = 0; i < (geo.PointList.Count); i++)
            {
                vertexList.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / meanRadius)));
                vertexListGround.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1));
            }


            for (int i = 0; i < (geo.PointList.Count - 1); i++)
            {
                if (sky)
                {
                    this.lineList2d.AddLine
                        (Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15 , geo.PointList[i].Lat,1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15 , geo.PointList[i + 1].Lat,1), lineColor, date);
                }
                else
                {
                    if (extrude)
                    {

                        this.triangleList.AddQuad(vertexList[i], vertexList[i + 1], vertexListGround[i], vertexListGround[i + 1], polyColor, date);

                    }
                    if (lineWidth > 0)
                    {
                        if (extrude)
                        {
                            this.lineList.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                        }
                        else
                        {
                            this.lineList2d.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                        }
                        if (extrude)
                        {
                            this.lineList.AddLine(vertexListGround[i], vertexListGround[i + 1], lineColor, date);
                            this.lineList.AddLine(vertexList[i], vertexListGround[i], lineColor, date);
                            this.lineList.AddLine(vertexList[i + 1], vertexListGround[i + 1], lineColor, date);
                        }
                    }
                }
            }
            if (fill)
            {
                List<int> indexes = Glu.TesselateSimplePolyB(vertexList);

                for (int i = 0; i < indexes.Count; i += 3)
                {
                    this.triangleList.AddTriangle(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], polyColor, date);
                }
            }
        }

        private void AddPolygonFlat(bool sky, KmlLineList geo, float lineWidth, Color polyColor, Color lineColor, bool extrude, bool fill, Dates date)
        {

            //todo can we save this work for later?
            List<Vector3d> vertexList = new List<Vector3d>();

            for (int i = 0; i < (geo.PointList.Count); i++)
            {
                vertexList.Add(Coordinates.GeoTo3dDouble(geo.PointList[i].Lat, geo.PointList[i].Lng, 1 + (geo.PointList[i].Alt / meanRadius)));
            }


            for (int i = 0; i < (geo.PointList.Count - 1); i++)
            {
                if (sky)
                {
                    this.lineList2d.AddLine
                        (Coordinates.RADecTo3d(-(180.0 - geo.PointList[i].Lng) / 15, geo.PointList[i].Lat, 1), Coordinates.RADecTo3d(-(180.0 - geo.PointList[i + 1].Lng) / 15, geo.PointList[i + 1].Lat, 1), lineColor, date);
                }
                else
                {
                    if (lineWidth > 0)
                    {
                        this.lineList2d.AddLine(vertexList[i], vertexList[i + 1], lineColor, date);
                    }
                }
            }
            if (fill)
            {
                List<int> indexes = Glu.TesselateSimplePoly(vertexList);

                for (int i = 0; i < indexes.Count; i += 3)
                {
                    this.triangleList2d.AddTriangle(vertexList[indexes[i]], vertexList[indexes[i + 1]], vertexList[indexes[i + 2]], polyColor, date, 2);
                }
            }
        }
      

        private Color ParseColor(string colorText, Color defaultColor)
        {
            try
            {
                int val = 0;

                bool match = int.TryParse(colorText, System.Globalization.NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out val);


                if (match)
                {
                    return Color.FromArgb(val);
                }
            }
            catch
            {
            }
            try
            {
                float opacity = 1.0f;
                int pos = colorText.IndexOf("%");
                if (pos > -1)
                {
                    float opa = 0;
                    if (float.TryParse(colorText.Substring(0, pos), out opa))
                    {
                        opacity = opa/100f;
                    }

                    colorText = colorText.Substring(pos+1);
                }

                Color foundColor = Color.FromName(colorText.Replace(" ",""));

                foundColor = Color.FromArgb((int)Math.Min(255, Math.Max(0, opacity * 255)), foundColor);

                return foundColor;
            }
            catch
            {

            }



            return defaultColor;
        }

        public static DateTime ParseDate(string date)
        {
            DateTime dt = DateTime.Now;
            if (!DateTime.TryParse(date, out dt))
            {
                double excelDate = 0;
                if (double.TryParse(date, out excelDate))
                {
                    return ExeclToDateTime(excelDate);
                }
            }

            if (dt.Kind == DateTimeKind.Local )
            {
                dt = dt.ToUniversalTime();
            }


            return dt;
        }

        private static bool TryParseDate(string date, out DateTime outDate)
        {
            DateTime dt = DateTime.MinValue;

            if (date.Contains(",") && date.Contains("UTC"))
            {
                date = date.Substring(date.IndexOf(",")).Replace(" UTC", "");
            }

            if (!DateTime.TryParse(date, out dt))
            {
                double excelDate = 0;
                if (double.TryParse(date, out excelDate))
                {
                    outDate = ExeclToDateTime(excelDate);
                    return true;
                }
                outDate = DateTime.MinValue;
                return false;
            }


            outDate = dt;
            return true;
        }

        public static DateTime ExeclToDateTime(double excelDate)
        {
            if (excelDate > 59)
            {
                excelDate -= 1;
            }
            if (excelDate > 730000)
            {
                excelDate = 730000;
            }
            return new DateTime(1899, 12, 31).AddDays(excelDate);
        }

        public override IPlace FindClosest(Coordinates target, float distance, IPlace defaultPlace, bool astronomical)
        {
            Vector3d searchPoint = Coordinates.GeoTo3dDouble(target.Lat,target.Lng);
           
            //searchPoint = -searchPoint;
            Vector3d dist;
            if (defaultPlace != null)
            {
                Vector3d testPoint = Coordinates.RADecTo3d(defaultPlace.RA, -defaultPlace.Dec, -1.0);
                dist = searchPoint - testPoint;
                distance = (float)dist.Length();
            }

            int closestItem = -1;
            int index = 0;
            foreach (Vector3 point in positions)
            {
                dist = searchPoint - new Vector3d(point);
                if (dist.Length() < distance)
                {
                    distance = (float)dist.Length();
                    closestItem = index;
                }
                index++;
            }


            if (closestItem == -1)
            {
                return defaultPlace;
            }

            Coordinates pnt = Coordinates.CartesianToSpherical2(positions[closestItem]);

            string name = "";
            try
            {
                if (this.nameColumn > -1)
                {
                    name = table.Rows[closestItem][this.nameColumn];
                    if (nameColumn == startDateColumn || nameColumn == endDateColumn)
                    {
                        name = ParseDate(name).ToString("u");
                    }
                }
            }
            catch
            {
            }

            if (String.IsNullOrEmpty(name))
            {
                if (astronomical)
                {
                    name = string.Format("RA={0}, Dec={1}", Coordinates.FormatHMS(pnt.RA), Coordinates.FormatDMS(pnt.Dec));
                }
                else
                {
                    name = string.Format("Lng={0}, Lat={1}", Coordinates.FormatDMS(pnt.Lng), Coordinates.FormatDMS(pnt.Lat)); ;
                }

            }
            TourPlace place = new TourPlace(name, pnt.Lat, pnt.Lng, Classification.Unidentified, "", ImageSetType.Earth, -1);

            Dictionary<String, String> rowData = new Dictionary<string, string>();
            for (int i = 0; i < table.Header.GetLength(0); i++)
            {
                string colValue = table.Rows[closestItem][i];
                if (i == startDateColumn || i == endDateColumn)
                {
                    colValue = ParseDate(colValue).ToString("u");
                }

                if (!rowData.ContainsKey(table.Header[i]) && !string.IsNullOrEmpty(table.Header[i]))
                {
                    rowData.Add(table.Header[i], colValue);
                }
                else
                {
                    rowData.Add("Column" + i.ToString(), colValue);
                }
            }
            place.Tag = rowData;

            return place;
        }




        Table table = new Table();

        internal Table Table
        {
            get { return table; }
            set { table = value; }
        }

        public void LoadFromString(string data, bool isUpdate, bool purgeOld, bool purgeAll, bool hasHeader)
        {

            if (!isUpdate )
            {
                table = new Table();
            }
            table.Lock();
            table.LoadFromString(data, isUpdate, purgeAll, hasHeader);
            if (!isUpdate)
            {
                GuessHeaderAssignments();
            }

            if (astronomical && lngColumn > -1)
            {
                double max = GetMaxValue(lngColumn);
                if (max > 24)
                {
                    RaUnits = RAUnits.Degrees;
                }
            }


            if (purgeOld)
            {
                PurgeByTime();
            }
            table.Unlock();
        }

        public void PurgeByTime()
        {
            if (startDateColumn < 0)
            {
                return;
            }
            int columnToUse = startDateColumn;
            if (endDateColumn > -1)
            {
                columnToUse = endDateColumn;
            }

            DateTime threasholdTime = SpaceTimeController.Now;
            TimeSpan ts = TimeSpan.FromDays(decay);
            threasholdTime -= ts;

            int count = table.Rows.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    string[] row = table.Rows[i];
                    DateTime colDate = Convert.ToDateTime(row[columnToUse]);
                    if (colDate < threasholdTime)
                    {
                        table.Rows.RemoveAt(i);
                        count--;
                        i--;
                    }

                }
                catch
                {
                }
            }
        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {

            if (table.Locked)
            {
                return false;
            }
            table.Lock();
            bool bVal = base.Draw(renderContext, opacity, flat);
            table.Unlock();
            return bVal;

        }

        public override void CleanUp()
        {
            table.Lock();
            base.CleanUp();
            table.Unlock();
        }

    }

    public class BarValue
    {
        public double Value = 0;
        public string Name = "";
        public bool Selected = false;

        public BarValue()
        {
        }
        public BarValue(string name, double value, bool selected )
        {
            Name = name;
            Value = value;
            Selected = selected;
        }

    }
    public enum DateFilter { None, Year, Month, Day, DayOfWeek, DayOfYear, Hour, Minute, Second };

    public class ColumnStats
    {
        public int TargetColumn;
        public int DemoninatorColumn;
        public int DomainColumn;
        public string FilterValue;
        public double Min;
        public double Max;
        public double Average;
        public double Median;
        public double Sum;
        public int Count;
        public double[] Histogram;
        public string[] DomainValues;
        public bool[] Selected;
        public StatTypes DomainStatType;
        public int Buckets;
        public double BucketWidth; 
        public bool Computed;
        public double HistogramMax;
        public DateFilter DateFilter;
        public DateTime BeginDate;
        public DateTime EndDate;

        public Dictionary<string, bool> SelectDomain = new Dictionary<string, bool>(); 


        public bool IsSelected(double value)
        {
            if (Selected == null)
            {
                return true;
            }

            int bucket = Math.Min(this.Buckets - 1,(int)((value - this.Min) / this.BucketWidth));

            return Selected[bucket];
 
        }

        public bool IsSelected(string[] row)
        {
            if (!Computed)
            {
                return true;
            }

            try
            {
                if (DomainColumn > -1)
                {
                    if (SelectDomain.ContainsKey(row[DomainColumn]))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (DateFilter != DateFilter.None)
                {
                    DateTime date = SpreadSheetLayer.ParseDate(row[TargetColumn]);
                    int bucket = 0;
                    switch (DateFilter)
                    {
                        case DateFilter.Year:
                            bucket = (date.Date - BeginDate).Days;
                            break;
                        case DateFilter.Month:
                            bucket = date.Month - 1;
                            break;
                        case DateFilter.Day:
                            bucket = date.Day - 1;
                            break;
                        case DateFilter.DayOfWeek:
                            bucket = (int)date.DayOfWeek;
                            break;
                        case DateFilter.DayOfYear:
                            bucket = date.DayOfYear - 1;
                            break;
                        case DateFilter.Hour:
                            bucket = date.Hour;
                            break;
                        case DateFilter.Minute:
                            bucket = date.Minute;
                            break;
                        case DateFilter.Second:
                            bucket = date.Second;
                            break;
                        default:
                            break;
                    }

                    return Selected[bucket];

                }
                else
                {
                    bool sucsess = false;
                    double val = 0;
                    double denominator = 0;

                    sucsess = double.TryParse(row[TargetColumn], out val);
                    if (!sucsess)
                    {
                        return false;
                    }

                    if (this.DomainStatType == StatTypes.Ratio)
                    {
                        sucsess = double.TryParse(row[this.DemoninatorColumn], out denominator);
                        if (!sucsess)
                        {
                            return false;
                        }
                        val = val / denominator;
                    }

                    int bucket = Math.Max(0, Math.Min(this.Buckets - 1, (int)((val - this.Min) / this.BucketWidth)));

                    return Selected[bucket];
                }
                
            }
            catch
            {
                return false;
            }
        }

        public void Sort(int sortType)
        {
            List<BarValue> sortList = new List<BarValue>();

            for (int i = 0; i < Buckets; i++)
            {
                sortList.Add(new BarValue(DomainValues[i], Histogram[i], Selected[i]));
            }

            switch (sortType)
            {
                case 0:
                    sortList.Sort(delegate(BarValue p1, BarValue p2) { return p1.Name.CompareTo(p2.Name); });
                    break;
                case 1:
                    sortList.Sort(delegate(BarValue p1, BarValue p2) { return -p1.Name.CompareTo(p2.Name); });
                    break;
                case 2: 
                    sortList.Sort(delegate(BarValue p1, BarValue p2) { return p1.Value.CompareTo(p2.Value); });
                    break;
                case 3:
                    sortList.Sort(delegate(BarValue p1, BarValue p2) { return -p1.Value.CompareTo(p2.Value); });
                    break;
            }

            int index = 0;
            foreach (BarValue bar in sortList)
            {
                DomainValues[index] = bar.Name;
                Histogram[index] = bar.Value;
                Selected[index] = bar.Selected;
                index++;
            }
        }

        //public ColumnStats()
        //{
        //    TargetColumn = -1;
        //    FilterColumn = -1;
        //    FilterValue = null;
        //    Min = 0;
        //    Max = 0;
        //    Average = 0;
        //    Median = 0;
        //    Sum = 0;
        //    Count = 0;
        //    BucketWidth = 0;
        //    Histogram = null;
        //    Buckets = 256;
        //    Computed = false;
        //}

    }

    
   

    public class SpreadSheetLayerUI : LayerUI
    {
        public SpreadSheetLayer Layer;
        public SpreadSheetLayerUI(SpreadSheetLayer layer)
        {
            Layer = layer;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            List<LayerUITreeNode> nodes = new List<LayerUITreeNode>();
           

                if (Layer.Filters.Count > 0)
                {
                    foreach (FilterGraphTool fgt in Layer.Filters)
                    {
                        LayerUITreeNode node = new LayerUITreeNode();
                        node.Name = fgt.Title;
                        node.Tag = fgt;
                        node.Checked = fgt == Earth3d.MainWindow.UiController;
                        node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                        node.NodeChecked += new LayerUITreeNodeCheckedDelegate(node_NodeChecked);
                        node.NodeActivated += new LayerUITreeNodeActivatedDelegate(node_NodeActivated);
                        nodes.Add(node);
                    }
                }


            
            return nodes;
        }

        void node_NodeActivated(LayerUITreeNode node)
        {
            Earth3d.MainWindow.UiController = node.Tag as FilterGraphTool;
        }

       
        void node_NodeChecked(LayerUITreeNode node, bool newState)
        {
           

        }

        void node_NodeSelected(LayerUITreeNode node)
        {

        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            List<LayerUIMenuItem> items = new List<LayerUIMenuItem>();

            //LayerUIMenuItem ColorMenu = new LayerUIMenuItem();
            //ColorMenu.Name = "Color";
            //ColorMenu.MenuItemSelected += new MenuItemSelectedDelegate(ColorMenu_MenuItemSelected);
            //ColorMenu.Tag = node.Tag;
            //items.Add(ColorMenu);

            return items;
        }

        void ColorMenu_MenuItemSelected(LayerUIMenuItem item)
        {

        }

        internal void UpdateNodes()
        {
            if (callbacks != null)
            {
                callbacks.UpdateNode(Layer, null);
            }
        }

        IUIServicesCallbacks callbacks;
        public override void SetUICallbacks(IUIServicesCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }
    }
}
