using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace TerraViewer
{
	/// <summary>
	/// Summary description for DataSetManager.
	/// </summary>
	public class DataSetManager
	{
        static Dictionary<string,DataSet> dataSets;
		public DataSetManager()
		{
            var trys = 3;
            var citiesLoaded = false;
            XmlDocument doc;
            while (trys-- > 0 && !citiesLoaded)
            {
                try
                {
                    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"data\places\");
                    DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=citiesdata", Properties.Settings.Default.CahceDirectory + @"data\places\datasets.xml", false, true);
                    doc = new XmlDocument();
                    doc.Load(Properties.Settings.Default.CahceDirectory + @"data\places\datasets.xml");

                    dataSets = new Dictionary<string, DataSet>();

                    XmlNode root = doc["root"];
                    var datasets = root.FirstChild;
                    foreach (XmlNode dataset in datasets.ChildNodes)
                    {
                        var ds = new DataSet(dataset.Attributes["name"].InnerXml, dataset.Attributes["url"].InnerXml, false, DataSetType.Place);
                        dataSets.Add(ds.Name,ds);
                    }
                    citiesLoaded = true;
                }
                catch
                {
                    File.Delete(Properties.Settings.Default.CahceDirectory + @"data\places\datasets.xml");
                }
            }

            if (!citiesLoaded)
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(185, "The Cities Catalog data file could not be downloaded or has been corrupted. Restart the application with a network connection to download a new file."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
            }

            var datasetsLoaded = false;
            trys = 3;
            while (trys-- > 0 && !datasetsLoaded)
            {
                // SPACE	
                try
                {
                    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"data\");
                    DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=spacecatalogs", Properties.Settings.Default.CahceDirectory + @"data\spaceCatalogs.xml", false, true);
                    doc = new XmlDocument();
                    doc.Load(Properties.Settings.Default.CahceDirectory + @"data\spaceCatalogs.xml");


                    XmlNode root = doc["root"];
                    var datasets = root.FirstChild;
                    foreach (XmlNode dataset in datasets.ChildNodes)
                    {


                        var ds = new DataSet(dataset.Attributes["name"].InnerXml, dataset.Attributes["url"].InnerXml, true, DataSetType.Place);
                        dataSets.Add(ds.Name, ds);
                    }
                    datasetsLoaded = true;
                }
                catch
                {
                    File.Delete(Properties.Settings.Default.CahceDirectory + @"data\spaceCatalogs.xml");
                }
            }
            if (!datasetsLoaded)
            {
                UiTools.ShowMessageBox(Language.GetLocalizedText(186, "The Catalog data file could not be downloaded or has been corrupted. Restart the application with a network connection to download a new file."), Language.GetLocalizedText(3, "Microsoft WorldWide Telescope"));
            }
		}

        public static bool DataFresh = false;


        public static bool DownloadFile(string url, string fileName, bool noCheckFresh, bool versionDependent)
        {

            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
 
            var didDownload = false;

            WebResponse response = null;


            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Timeout = 30000;
                request.Headers.Add("LiveUserToken", Properties.Settings.Default.LiveIdToken);
                string etag;

                if (File.Exists(fileName) && noCheckFresh)
                {
                    return false;
                }

                if (File.Exists(fileName))
                {
                    if ((DataFresh && versionDependent) || noCheckFresh)
                    {
                        return false;
                    }  
                    if (File.Exists(fileName + ".etag"))
                    {
                        etag = File.ReadAllText(fileName + ".etag");

                        if (!string.IsNullOrEmpty(etag))
                        {
                            request.Headers.Add("If-None-Match", etag);
                        }
                    }
                }

                response = request.GetResponse();



                etag = response.Headers["Etag"];
                if (!string.IsNullOrEmpty(etag))
                {
                    File.WriteAllText(fileName + ".etag", etag);
                }

                Stream s = null;
                FileStream fs = null;
                try
                {
                    s = response.GetResponseStream();
                    fs = new FileStream(fileName, FileMode.Create);

                    var buffer = new byte[4096];

                    while (true)
                    {
                        var count = s.Read(buffer, 0, 4096);
                        fs.Write(buffer, 0, count);
                        if (count == 0)
                        {
                            break;
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    if (s != null)
                    {
                        s.Close();
                        s.Dispose();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
                didDownload = true;
            }
            catch (WebException e)
            {
                if (e.ToString().IndexOf("304", StringComparison.Ordinal) == -1)
                {
                    return false;
                }

            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }
            return didDownload;
        }

        public static Dictionary<string, DataSet> GetDataSets()
		{
            return dataSets;
		}
        public static IThumbnail GetDataSetsAsFolder()
        {
            var parent = new ThumbMenuNode {Name = Language.GetLocalizedText(646, "Collections")};

            var ds = GetDataSets();
            foreach (var d in ds.Values)
            {
                // Todo Change this to exploere earth, moon etc.
                if (d.Sky == true)
                {
                        var placesList = d.GetPlaces();
                        foreach (var places in placesList.Values)
                        {
                            if (places != null && places.Browseable)
                            {
                                parent.AddChild(places);
                            }
                        }
                }
            }
            return parent; 
        }
	}
}
