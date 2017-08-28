using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;	
using System.Threading;
using System.Text;
using System.Xml;
namespace TerraViewer
{
	/// <summary>
	/// Summary description for Places.
	/// </summary>
	public class Places : IThumbnail
	{
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
		string url;
		System.Collections.ArrayList dataList;
        private bool sky;

        public bool Sky
        {
            get { return sky; }
            set { sky = value; }
        }
        private string thumbnailUrl;
        public string ThumbnailUrl
        {
            get { return thumbnailUrl; }
            set { thumbnailUrl = value; }
        }
        private bool browseable;

        public bool Browseable
        {
            get { return browseable; }
            set { browseable = value; }
        }
        DataSetType dataSetType = DataSetType.Place;

        public DataSetType DataSetType
        {
            get { return dataSetType; }
            set { dataSetType = value; }
        }

		public Places(string name, string url, bool sky, string thumbUrl, bool isBrowseable, DataSetType type)
        {

            browseable = isBrowseable;
            thumbnailUrl = thumbUrl;
            this.sky = sky;
            this.name = name;
			this.url = url;
            this.dataSetType = type;
		}

		public ArrayList GetPlaceList()
		{
			if (dataList == null || CheckExpiration())
			{
                dataList = new System.Collections.ArrayList();
                if (dataSetType == DataSetType.Place)
                {
                    DataSetManager.DownloadFile(url, Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".txt", false, true);


                    TourPlace place;
                    StreamReader sr = new StreamReader(Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".txt");
                    string line;
                    while (sr.Peek() >= 0)
                    {
                        line = sr.ReadLine();

                        place = new TourPlace(line, sky);
                        dataList.Add(place);
                    }
                    sr.Close();
                }
                else if (dataSetType == DataSetType.Imageset)
                {

                    string filename = Properties.Settings.Default.CahceDirectory + @"data\places\" + name + ".xml";

                    DataSetManager.DownloadFile(url, filename, false, true);


                    
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filename);

                    if (!Directory.Exists(Properties.Settings.Default.CahceDirectory + @"thumbnails\"))
                    {
                        Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"thumbnails\");
                    }

                    XmlNode imageSets = doc["ImageSets"];
                    if (imageSets == null)
                    {
                        imageSets = doc["Folder"];
                    }

                    foreach (XmlNode imageset in imageSets.ChildNodes)
                    {
                        ImageSetHelper newImageset = ImageSetHelper.FromXMLNode(imageset);
                        if (newImageset != null)
                        {
                            TourPlace newPlace = new TourPlace(newImageset.Name, (newImageset.CenterY), (newImageset.CenterX) / 15, Classification.Unidentified, "Err", ImageSetType.Sky, newImageset.BaseTileDegrees*10);
                            newPlace.StudyImageset = newImageset;

                            newPlace.ThumbNail = UiTools.LoadThumbnailFromWeb(newImageset.ThumbnailUrl);
                            dataList.Add(newPlace);
                            if (!String.IsNullOrEmpty(newImageset.AltUrl) && !RenderEngine.ReplacementImageSets.ContainsKey(newImageset.AltUrl))
                            {
                                RenderEngine.ReplacementImageSets.Add(newImageset.AltUrl, newImageset);
                            }
                        }

                    }
                }


			}
			return dataList;
		}

        private bool CheckExpiration()
        {
            return false;

        }
		public override string ToString()
		{
			return this.name;
		}

        #region IThumbnail Members

        Bitmap thumbnail = null;

        public Bitmap ThumbNail
        {
            get
            {
                if (thumbnail == null)
                {
                    thumbnail = UiTools.LoadThumbnailFromWeb(ThumbnailUrl);
                }
                return thumbnail;
            }
            set
            {
                if (thumbnail != null)
                {
                    thumbnail.Dispose();
                    GC.SuppressFinalize(thumbnail);
                }
                thumbnail = value;
            }
        }

        public bool IsCloudCommunityItem
        {
            get
            {
                return false;
            }

        }

        Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
            }
        }

        public bool IsImage
        {
            get { return false; }
        }

        public bool IsTour
        {
            get { return false; }
        }

        public bool ReadOnly
        {
            get { return true; }
        }

        public bool IsFolder
        {
            get { return true; }
        }

        public object[] Children
        {
            get
            {
                ArrayList list = GetPlaceList();

                object[] array = new object[list.Count];
                for(int i = 0 ; i< list.Count;i++)
                {
                    array[i] = list[i];
                }
                return array;
            }
        }

        #endregion
    }
}
