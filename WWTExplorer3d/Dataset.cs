using System;
using System.Collections.Generic;
using System.Xml;

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

	    readonly Dictionary<string,Places> dataSets;
	    readonly string groupingName;


	    public bool Sky { get; set; }

	    DataSetType dataSetType = DataSetType.Place;

        public DataSetType DataSetType
        {
            get { return dataSetType; }
            set { dataSetType = value; }
        }

		public DataSet(string name, string url, bool sky, DataSetType type)
        {
            dataSetType = type;

            Sky = sky;
            this.name = name;


		    DataSetManager.DownloadFile(url, Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".xml", false, true);
			var doc = new XmlDocument();
            doc.Load(Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".xml");
			
			dataSets = new Dictionary<string, Places>();


            XmlNode root = doc["root"];
            var dataSetsNode = root.SelectSingleNode("dataset");
            groupingName = dataSetsNode.Attributes["Groups"].InnerXml;
            var dst = DataSetType.Place;
            if (dataSetsNode.Attributes["type"].Value != "place")
            {
                dst = DataSetType.Imageset;
            }       
            foreach (XmlNode dataset in dataSetsNode.ChildNodes)
            {
                var browsable = false;
                if (dataset.Attributes["Browseable"] != null)
                {
                    browsable = Convert.ToBoolean(dataset.Attributes["Browseable"].Value);
                }

                var thumbnailUrl = "";
                if (dataset.Attributes["Thumbnail"] != null)
                {
                    thumbnailUrl = dataset.Attributes["Thumbnail"].Value;
                }



                var places = new Places(dataset.Attributes["name"].InnerXml, dataset.Attributes["url"].InnerXml, sky, thumbnailUrl, browsable, dst);
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
