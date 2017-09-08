using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using XmlElement = Windows.Data.Xml.Dom.XmlElement;
using XmlDocument = Windows.Data.Xml.Dom.XmlDocument;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif

namespace TerraViewer
{
    /// <summary>
    /// Summary description for Dataset.
    /// </summary>
    public enum DataSetType { Place, Imageset, KML };
	public class DataSet
	{
		public DataSet()
		{
			
		}
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
		string url;
		Dictionary<string,Places> dataSets = null;
		string groupingName;

        
        private bool sky;

        public bool Sky
        {
            get { return sky; }
            set { sky = value; }
        }
        DataSetType dataSetType = DataSetType.Place;

        public DataSetType DataSetType
        {
            get { return dataSetType; }
            set { dataSetType = value; }
        }

		public DataSet(string name, string url, bool sky, DataSetType type)
        {
            this.dataSetType = type;

            this.Sky = sky;
            this.name = name;
			this.url = url;


            DataSetManager.DownloadFile(url, Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".xml", false, true);
			XmlDocument doc = new XmlDocument();
            doc.Load(Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".xml");
			
			dataSets = new Dictionary<string, Places>();


            XmlNode root = doc.GetChildByName("root");
            XmlNode dataSetsNode = root.SelectSingleNode("dataset");
            this.groupingName = dataSetsNode.Attributes["Groups"].InnerXml;
            DataSetType dst = DataSetType.Place;
            if (dataSetsNode.Attributes["type"].Value != "place")
            {
                dst = DataSetType.Imageset;
            }       
            foreach (XmlNode dataset in dataSetsNode.ChildNodes)
            {
                bool browsable = false;
                if (dataset.Attributes["Browseable"] != null)
                {
                    browsable = Convert.ToBoolean(dataset.Attributes["Browseable"].Value);
                }

                string thumbnailUrl = "";
                if (dataset.Attributes["Thumbnail"] != null)
                {
                    thumbnailUrl = dataset.Attributes["Thumbnail"].Value;
                }



                Places places = new Places(dataset.Attributes["name"].InnerXml, dataset.Attributes["url"].InnerXml, sky, thumbnailUrl, browsable, dst);
                dataSets.Add(places.Name, places);
            }
 
		}

		public string GroupingName
		{
			get
			{
				return groupingName;
			}
		}
        public Dictionary<string, Places> GetPlaces()
		{
			return dataSets;
		}

		public override string ToString()
		{
			return name;
		}
	}
}
