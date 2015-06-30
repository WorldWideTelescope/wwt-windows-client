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
                UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/User");
                string appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "POST";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            fileStream.Seek(0, SeekOrigin.Begin);
            byte[] buffer = new byte[fileStream.Length];
            int count = 0;
            while (count < fileStream.Length)
            {
                buffer[count++] = Convert.ToByte(fileStream.ReadByte());
            }
            fileStream.Close();
            UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/Publish/" + name);
            string appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "POST";
            request.ContentType = "text/plain";
            request.ContentLength = buffer.Length;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            Stream reqStream = request.GetRequestStream();
            reqStream.Write(buffer, 0, buffer.Length);
            reqStream.Close();
            int id = -1;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
            UriBuilder getQuestionsUri = new UriBuilder(url);
            string appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "GET";
            request.ContentLength = 0;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
                UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/" + id.ToString());
                string appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Delete";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
                UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Community/" + id.ToString());
                string appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Delete";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
                UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/Content/Rate/" + id.ToString() + "/" + rating.ToString());
                string appSecurityToken = Properties.Settings.Default.LiveIdToken;
                string body;
                WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
                request.Method = "Post";
                request.ContentLength = 0;
                request.Headers.Add(@"LiveUserToken", appSecurityToken);

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
            UriBuilder getQuestionsUri = new UriBuilder(Properties.Settings.Default.CloudCommunityUrlNew + @"/Resource/Service/User");
            string appSecurityToken = Properties.Settings.Default.LiveIdToken;
            string body;
            WebRequest request = WebRequest.Create(getQuestionsUri.Uri);
            request.Method = "Get";
            request.ContentLength = 0;
            request.Headers.Add(@"LiveUserToken", appSecurityToken);

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader responseStreamReader = new StreamReader(responseStream))
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
