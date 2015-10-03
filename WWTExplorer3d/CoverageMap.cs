using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Threading;

namespace TerraViewer
{
    class CoverageMap
    {
        static readonly List<CoverageItem> map = new List<CoverageItem>();
        static readonly Mutex storeMutex = new Mutex();

        public static int GetCoverage(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return 0;
            }

            var level = id.Length;

            if (level < 10)
            {
                return 0;
            }

            try
            {
                storeMutex.WaitOne();

                foreach (var item in map)
                {
                    if (id.StartsWith(item.Prefix))
                    {
                        if (level > item.MaxZoom || level < item.MinZoom)
                        {
                            continue;
                        }

                        return item.Generation;
                    }
                }

                return DownloadCoverage(id);
            }
            finally
            {
                storeMutex.ReleaseMutex();
            }
        }

        static int DownloadCoverage(string id)
        {
            var Client = new WebClient();
            Client.Headers.Add("User-Agent", "Win8Microsoft.BingMaps.3DControl/2.214.2315.0 (;;;;x64 Windows RT)");
            try
            {
                var xml = Client.DownloadString(string.Format("http://ak.t{0}.tiles.virtualearth.net/tiles/coverage?g=2536&imagetype=mtx&quadkey={1}", id.Substring(id.Length - 1, 1), id));

                var doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode root = doc["CoverageInfo"];


                var prefix = root.Attributes["Prefix"].Value;
                var max = int.Parse(root.Attributes["MaxZoom"].Value);
                var min = int.Parse(root.Attributes["MinZoom"].Value);
                var gen = int.Parse(root.Attributes["Generation"].Value);

                foreach (var item in map)
                {
                    if (item.MaxZoom == max && item.MinZoom == min && item.Generation == gen && item.Prefix == prefix)
                    {
                        // Empty?
                    }
                }


                map.Add(new CoverageItem(gen,min,max,prefix));

                return gen;
            }
            catch
            {
                
            }
            
            return 0;
        }
    }

    class CoverageItem
    {
        public int MinZoom = 0;
        public int MaxZoom = 0;
        public int Generation = 0;
        public string Prefix = "";

        public CoverageItem(int gen, int min, int max, string prefix)
        {
            MinZoom = min;
            MaxZoom = max;
            Generation = gen;
            Prefix = prefix;

        }
    }
}
