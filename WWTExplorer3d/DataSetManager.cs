using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
#if WINDOWS_UWP
using Color = Windows.UI.Color;
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
    /// Summary description for DataSetManager.
    /// </summary>
    public class DataSetManager
	{
        static Dictionary<string,DataSet> dataSets = null;
		public DataSetManager()
		{
            int trys = 3;
            bool citiesLoaded = false;
            XmlDocument doc = null;
            while (trys-- > 0 && !citiesLoaded)
            {
                try
                {
                    Directory.CreateDirectory(Properties.Settings.Default.CahceDirectory + @"data\places\");
                    DownloadFile("http://www.worldwidetelescope.org/wwtweb/catalog.aspx?X=citiesdata", Properties.Settings.Default.CahceDirectory + @"data\places\datasets.xml", false, true);
                    doc = new XmlDocument();
                    doc.Load(Properties.Settings.Default.CahceDirectory + @"data\places\datasets.xml");

                    dataSets = new Dictionary<string, DataSet>();

                    XmlNode root = doc.GetChildByName("root");
                    XmlNode datasets = root.FirstChild;
                    foreach (XmlNode dataset in datasets.ChildNodes)
                    {
                        DataSet ds = new DataSet(dataset.Attributes["name"].InnerText, dataset.Attributes["url"].InnerText, false, DataSetType.Place);
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

            bool datasetsLoaded = false;
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


                    XmlNode root = doc.GetChildByName("root");
                    XmlNode datasets = root.FirstChild;
                    foreach (XmlNode dataset in datasets.ChildNodes)
                    {


                        DataSet ds = new DataSet(dataset.Attributes["name"].InnerText, dataset.Attributes["url"].InnerText, true, DataSetType.Place);
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

        public static string[] ReadAllFileLines(string filename)
        {
#if !WINDOWS_UWP
            return System.IO.File.ReadAllLines(filename);
#else
            //Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            //var asfi = storageFolder.GetFileAsync(filename);

            //var aa = asfi.AsTask();
            //var file = aa.Result;

            var asfi = Windows.Storage.StorageFile.GetFileFromPathAsync(filename);

            var aa = asfi.AsTask();
            var file = aa.Result;
            
            return Windows.Storage.FileIO.ReadLinesAsync(file).AsTask().Result.ToArray();
#endif
        }


        public static bool DownloadFile(string url, string fileName, bool noCheckFresh, bool versionDependent)
        {
#if !WINDOWS_UWP

            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            bool didDownload = false;

            System.Net.WebRequest request = null;
            System.Net.WebResponse response = null;


            try
            {
                request = System.Net.WebRequest.Create(url);
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

                    byte[] buffer = new byte[4096];

                    while (true)
                    {
                        int count = s.Read(buffer, 0, 4096);
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
            catch (System.Net.WebException e)
            {
                if (e.ToString().IndexOf("304") == -1)
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
#else
          //  try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                Task<byte[]> task = client.GetByteArrayAsync(url);
                byte[] buffer = task.Result;


                //var cfa = Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                //var cfaTask = cfa.AsTask();
                //var file = cfaTask.Result;

                //using (var stream = file.OpenStreamForWriteAsync().Result)

                string path = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (System.IO.Stream stream = System.IO.File.Create(fileName))
                {
                    stream.WriteAsync(buffer, 0, buffer.Length).Wait();
                }

                return true;
            }
       //     catch
            {
                return false;
            }
        }
#endif


        public static Dictionary<string, DataSet> GetDataSets()
		{
            return dataSets;
		}
        public static IThumbnail GetDataSetsAsFolder()
        {
            ThumbMenuNode parent = new ThumbMenuNode();

            parent.Name = Language.GetLocalizedText(646, "Collections");

            Dictionary<string, DataSet> dataSets = DataSetManager.GetDataSets();
            foreach (DataSet d in dataSets.Values)
            {
                // Todo Change this to exploere earth, moon etc.
                if (d.Sky == true)
                {
                    if (d != null)
                    {
                        Dictionary<string, Places> placesList = d.GetPlaces();
                        foreach (Places places in placesList.Values)
                        {
                            if (places != null && places.Browseable)
                            {
                                parent.AddChild(places);
                            }
                        }
                    }
                }
            }
            return parent; 
        }
	}
}
