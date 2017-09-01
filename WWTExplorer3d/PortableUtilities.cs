using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    class PortableUtilities
    {
    }

    public class HttpWebLoader
    {
        public static bool DownloadFile(string url, string filename, bool noCheckFresh, bool versionDependent)
        {
#if !WINDOWS_UWP
           return DataSetManager.DownloadFile(url, filename, noCheckFresh, versionDependent);
        }
#else
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                Task<byte[]> task = client.GetByteArrayAsync(url);
                byte[] buffer = task.Result;

                string path = Path.GetDirectoryName(filename);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (System.IO.Stream stream = System.IO.File.Create(filename))
                {
                    stream.WriteAsync(buffer, 0, buffer.Length).Wait();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
#endif

    }
}
