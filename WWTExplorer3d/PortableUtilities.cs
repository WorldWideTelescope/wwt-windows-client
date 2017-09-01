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
        public static bool DownloadFile(string url, string fileName, bool noCheckFresh, bool versionDependent)
        {
#if !WINDOWS_UWP
           return DataSetManager.DownloadFile(url, fileName, noCheckFresh, versionDependent);
        }
#else
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                Task<byte[]> task = client.GetByteArrayAsync(url);
                byte[] buffer = task.Result;

                var cfa = Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, Windows.Storage.CreationCollisionOption.ReplaceExisting);
                var cfaTask = cfa.AsTask();
                var file = cfaTask.Result;

                using (var stream = file.OpenStreamForWriteAsync().Result)
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
