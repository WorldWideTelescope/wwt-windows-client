using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace TerraViewer
{
    public static class EOCalls
    {

        public static void InvokeRegisterUser()
        {
            try
            {
                var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/User");
                var appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                var request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            body = responseStreamReader.ReadToEnd();
                            body = body.Substring(body.IndexOf('>') + 1);
                            body = body.Substring(0, body.IndexOf('<'));
                        }
                    }
                }
            }
            catch
            {

            }
        }

        public static void InvokePublishFile(string filePath, string name)
        {
            var fileStream = new FileStream(filePath, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);
            var buffer = new byte[fileStream.Length];
            var count = 0;
            while (count < fileStream.Length)
            {
                buffer[count++] = Convert.ToByte(fileStream.ReadByte());
            }
            fileStream.Close();
            var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/Publish/" + name);
            var appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            var request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "POST";
            request.ContentType = "text/plain";
            request.ContentLength = buffer.Length;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            var reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
            var id = -1;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        body = responseStreamReader.ReadToEnd();
                        body = body.Substring(body.IndexOf('>') + 1);
                        body = body.Substring(0, body.IndexOf('<'));
                        id = int.Parse(body);

                        if (id > 0)
                        {
                            WebWindow.OpenUrl(Properties.Settings.Default.CloudCommunityUrlNew + @"/Community#/EditContent/" + id.ToString(), true);
                        }
                        // MessageBox.Show(body);
                    }
                }
            }
        }

        public static void InvokeGetCalls(string url)
        {
            var getQuestionsUri = new UriBuilder(url);
            var appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            var request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        body = responseStreamReader.ReadToEnd();
                        MessageBox.Show(body);
                    }
                }
            }
        }

        public static bool InvokeDeleteContent(int id)
        {
            try
            {
                var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/" + id.ToString());
                var appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                var request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Delete";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            body = responseStreamReader.ReadToEnd();
                            body = body.Substring(body.IndexOf('>') + 1);
                            body = body.Substring(0, body.IndexOf('<'));
                            return Boolean.Parse(body);
                        }
                    }
                }
            }
            catch
            {

            }
            return false;



        }

        public static bool InvokeDeleteCommunity(int id)
        {
            try
            {
                var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Community/" + id.ToString());
                var appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                var request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Delete";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            body = responseStreamReader.ReadToEnd();
                            body = body.Substring(body.IndexOf('>') + 1);
                            body = body.Substring(0, body.IndexOf('<'));
                            return Boolean.Parse(body);
                        }
                    }
                }
            }
            catch
            {

            }
            return false;



        }

        public static bool InvokeRateContent(int id, int rating)
        {
            try
            {
                var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/Rate/" + id.ToString() + "/" + rating.ToString());
                var appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                var request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Post";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var responseStreamReader = new StreamReader(responseStream))
                        {
                            body = responseStreamReader.ReadToEnd();
                            body = body.Substring(body.IndexOf('>') + 1);
                            body = body.Substring(0, body.IndexOf('<'));
                            return Boolean.Parse(body);
                        }
                    }
                }
            }
            catch
            {

            }
            return false;
        }

        public static bool IsUserRegistered()
        {
            var getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/User");
            var appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            var request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "Get";
            request.ContentLength = 0;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    using (var responseStreamReader = new StreamReader(responseStream))
                    {
                        body = responseStreamReader.ReadToEnd();
                        body = body.Substring(body.IndexOf('>') + 1);
                        body = body.Substring(0, body.IndexOf('<'));
                        return Boolean.Parse(body);
                    }
                }
            }
        }
    }
}
