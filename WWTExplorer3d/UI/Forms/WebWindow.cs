using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace TerraViewer
{
    public partial class WebWindow : Form
    {
        public WebWindow()
        {
            InitializeComponent();
        }
        public string Url;
        public string HtmlContent;
        private DateTime startTime;
        private void WebWindow_Load(object sender, EventArgs e)
        {
            this.Owner = Earth3d.MainWindow;
            startTime = DateTime.Now;
            if (!string.IsNullOrEmpty(Url))
            {
                webBrowser.Navigate(Url);
            }
            else if (!string.IsNullOrEmpty(HtmlContent))
            {
                webBrowser.DocumentText = HtmlContent;
            }
            this.Activate();

        }

        public static void OpenUrl(string url, bool external)
        {
            if (Earth3d.TouchKiosk)
            {
                return;
            }

            if (url.ToLower().Contains("layerapi.aspx"))
            {
                var layerClient = new WebClient();
                layerClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(layerClient_DownloadStringCompleted);
                layerClient.DownloadStringAsync(new Uri(url));
                return;
            }


            if (Properties.Settings.Default.ShowLinksInFullBrowser)
            {
                external = true;
            }

            if (url.ToLower().EndsWith("&wwtfull"))
            {
                url = url.Substring(0, url.ToLower().LastIndexOf("&wwtfull"));
                external = true;
            }

            if (url.ToLower().Contains("wwtfull=true"))
            {
                external = true;
            }

            url.Replace("&wwtfull=true", "");
            url.Replace("?wwtfull=true","");
            url.Replace("&wwtfull", "");
            url.Replace("?wwtfull","");

            if (external)
            {
                try
                {
                    System.Diagnostics.Process.Start(url);
                }
                catch
                {
                }
            }
            else
            {
                var ww = new WebWindow();
               
                if (url.ToLower().Contains("worldwidetelescope.org"))
                {
                    ww.Width = 1250;
                } 
                ww.Url = url;
                ww.Show();
                ww.allowFullBrowser = true;
                
            }

            
        }

        static void layerClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // DO nothing...
        }

        bool allowFullBrowser;
        public static void OpenHtmlString(string html)
        {
            var ww = new WebWindow();
            //string style = "<style type=\"text/css\">@import url(\"http://stc.msn.com/br/hp/en-us/css/38/ushpw.css\");@import url(\"http://stc.msn.com/br/hp/en-us/css/38/ovrN.css\");</style>";
            var style = "<style> a.nav:link {color: #ffffff; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;}\n"+ 
                            "a.nav:visited {color: #FFFFFF; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none; }\n"+
                            "a.nav:active {color: #FFFFFF; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;}\n" +
                            "a.nav:hover { color: #FFFF00; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;}\n" +
                            "a.nav2:link {color: #0000FF; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;} \n" +
                            "a.nav2:visited {color: #0000FF; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none; }\n" +
                            "a.nav2:active {color: #0000FF; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;}\n" +
                            "a.nav2:hover { color: #FF0000; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;}\n" +
                            "a:link {color: #FF0000; font-family: Arial; font-size: 9pt; font-weight: bold; text-decoration : none;} \n" +
                            "a:visited {color: #FF0000;  font-weight: bold; text-decoration : none; }\n" +
                            "a:active {color: #FF0000;  font-weight: bold; text-decoration : none;}\n" +
                            "a:hover { color: #0000FF;  font-weight: bold; text-decoration : none;}\n" +
                            "body\n" +
                            "{\n" +
                            "     font-family: Arial, Helvetica, Verdana, sans-serif; font-size: 9pt;\n" +
                            "}\n" +
                            "p {\n" +
                            "\n" +
	                        "    margin-bottom: 6pt;\n" +
	                        "    font-size: 9pt;\n" +
                            "} \n" +
                            "h1 \n" +
                            "{\n" +
	                        "    color: #800000;\n" +
                            "    font-size: 12pt;\n" +
                            "}\n" +
                            "li\n" +
                            "{\n" +
	                        "    font-size: 9pt;\n" +
                            "}</style>";
            ww.HtmlContent = "<html>" + style + html + "</html>";
            //ww.HtmlContent = "<html>"  + html + "</html>";
            ww.WebAutoSize = true;
            ww.AutoClose = true;
            Earth3d.NoStealFocus = true;
            ww.Show();
            ww.Activate();
        }
        public bool WebAutoSize = true;

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if ( WebAutoSize && webBrowser.Document != null)
            {
                this.Height = webBrowser.Document.Body.ScrollRectangle.Height + (this.Height - webBrowser.Height)+15;
                this.Width = webBrowser.Document.Body.ScrollRectangle.Width + (this.Width - webBrowser.Width)+20;

                

                if (this.Height > Screen.PrimaryScreen.Bounds.Height)
                {
                    this.Height = Screen.PrimaryScreen.Bounds.Height - ((this.Height - this.ClientRectangle.Height) + 20);
                }

                if ((this.Height + this.Top) > (Screen.PrimaryScreen.Bounds.Height))
                {
                    this.Top = Screen.PrimaryScreen.Bounds.Height - this.Height;
                }
            }

            if (allowFullBrowser)
            {
                this.Text = "Web Window - Maximize window for full browser";
            }
        }
        public bool AutoClose = false;
        private void WebWindow_Deactivate(object sender, EventArgs e)
        {
            if (AutoClose && DateTime.Now.Subtract(startTime).TotalSeconds > 1)
            {
                this.Close();
            }
        }

        private void WebWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            Earth3d.NoStealFocus = false;
            Earth3d.MainWindow.Activate();
        }

        private void WebWindow_MouseClick(object sender, MouseEventArgs e)
        {

        }


        private void WebWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                this.Hide();
                try
                {
                    System.Diagnostics.Process.Start(Url);
                }
                catch
                {
                }
                this.Close();
            }

        }

        private void WebWindow_ResizeBegin(object sender, EventArgs e)
        {

        }
    }
}