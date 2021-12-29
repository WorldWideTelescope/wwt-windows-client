/*  
===============================================================================
 2007-2015 Copyright © Microsoft Corporation.  All rights reserved.
 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
 OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
 LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 FITNESS FOR A PARTICULAR PURPOSE.
===============================================================================
*/
using System;	
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading ;
using System.Drawing;
using Microsoft.Win32;
using System.Collections.Generic;


namespace TerraViewer
{
    public class WebListener
    {
        public WebListener(ParameterizedThreadStart target, IPAddress ipAddress)
        {
            IpAddress = ipAddress;
            //start listing on the given _Port
            Listener = new TcpListener(ipAddress, Port);        
            ListenThread = new Thread(target);
            
        }

        public void Start()
        {
            Listener.Start();
            ListenThread.Start( this );
        }

        public void Stop()
        {
            try
            {
                 Listener.Stop();
            }
            catch
            {
            }
            try
            {
                ListenThread.Join();
            }
            catch
            {
            }
        }

        int port = 5050;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public Thread ListenThread;
        public TcpListener Listener;
        public IPAddress IpAddress;
    }


	public class MyWebServer 
	{

        List<WebListener> listeners = new List<WebListener>();
        public static string IpAddress;

		private bool _bQuit = false;

        private static int _RunCount = 0;

        public static int RunCount
        {
            get { return MyWebServer._RunCount; }
            set { MyWebServer._RunCount = value; }
        }
		public MyWebServer()
		{
		}
        static public Dictionary<string,string> WhiteList = new Dictionary<string,string>();

        static bool initializedOnce = false;

		public bool Startup()
		{
            if (Utils.Logging) { Utils.WriteLogMessage("Starting Web Server"); }
            SetAccessLists();

            if (!initializedOnce)
            {
                RequestHandler.RegisterHandler(new HTTPImagesetWtml());
                RequestHandler.RegisterHandler(new HTTPLayerApi());
                RequestHandler.RegisterHandler(new HttpXmlRpc());
                RequestHandler.RegisterHandler(new HttpImageFiles());
                RequestHandler.RegisterHandler(new HttpCrossDomain());
                RequestHandler.RegisterHandler(new HttpClientStatus());
                XmlRpcMethod.RegisterDispatchMap(new SampClientReceiveNotification());
                initializedOnce = true;
            }


			try
			{	
                 // Find IPV4 Address
                _bQuit = false;
                IpAddress = "";
                listeners.Clear();
                WebListener listener = null;
                foreach (IPAddress ipAdd in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ipAdd.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (string.IsNullOrEmpty(IpAddress) )
                        {
                            IpAddress = ipAdd.ToString();
                        }
                        if (ipAdd.ToString() != "127.0.0.1")
                        {
                            if (Utils.Logging)
                            {
                                Utils.WriteLogMessage(" Web Server - Adding:" + ipAdd.ToString());
                            }
                            listener = new WebListener(new ParameterizedThreadStart(ListenerThreadFunc), ipAdd);
                            listeners.Add(listener);
                            listener.Start();
                        }
                    }
                }
                if (Utils.Logging)
                {
                    Utils.WriteLogMessage(" Web Server - Adding Loopback");
                }
                // Add Loopback localhost
                listener = new WebListener(new ParameterizedThreadStart(ListenerThreadFunc), IPAddress.Loopback);
                listeners.Add(listener);
                listener.Start();


        	}
			catch (Exception e)
			{
                if (Utils.Logging)
                {
                    Utils.WriteLogMessage("Failed Starting Web Server");
                    Utils.WriteLogMessage(e.Message);
                    Utils.WriteLogMessage(e.Source);
                    Utils.WriteLogMessage(e.StackTrace);

                }
                _bQuit = true;
                return false;
			}
            return true;
		}

        public static void SetAccessLists()
        {
            Dictionary<string,string> whiteList = new Dictionary<string, string>();

            string[] ipList = Properties.Settings.Default.HTTPWhiteList.Split(new char[] { ';' });

            foreach (string ip in ipList)
            {
                whiteList.Add(ip, ip);
            }


            WhiteList = whiteList;
        }
		private void Log(string msg)
		{
			
		}

        public static bool IsAuthorized(IPAddress ip)
        {
            string address = ip.ToString();
            string[] parts = address.Split(new char[] { '.' });

            string wild1 = string.Format("{0}.{1}.{2}.*", parts[0], parts[1], parts[2]);
            string wild2 = string.Format("{0}.{1}.*.*", parts[0], parts[1]);
            string wild3 = string.Format("{0}.*.*.*", parts[0]);

            if (Properties.Settings.Default.AllowLocalHTTP)
            {
                if (ip.ToString() == "127.0.0.1")
                {
                    return true;
                }

                foreach (IPAddress ipAdd in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (ipAdd.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ip.ToString() == ipAdd.ToString())
                        {
                            return true;
                        }
                        break;
                    }
                }
            }

            if (Properties.Settings.Default.AllowAllRemoteHTTP)
            {
                return true;
            }
            else
            {
                if (WhiteList.ContainsKey("*.*.*.*"))
                {
                    return true;
                }

                if (WhiteList.ContainsKey(address) || WhiteList.ContainsKey(wild1) || WhiteList.ContainsKey(wild2) || WhiteList.ContainsKey(wild3))
                {
                    return true;
                }
            }

            RemoteAccessControl.ipTarget = address;

            return false;
        }


		public void Shutdown()
		{
            if (Utils.Logging) { Utils.WriteLogMessage("Shutting Down Web Server"); }
            _bQuit = true;
            foreach (WebListener wl in listeners)
            {
                wl.Stop();
            }
            listeners.Clear();
		}


		/// <summary>
		/// Returns The Default File Name
		/// Input : WebServerRoot Folder
		/// Output: Default File Name
		/// </summary>
		/// <param _Name="sMyWebServerRoot"></param>
		/// <returns></returns>
		public string GetTheDefaultFileName(string sLocalDirectory)
		{
			return "";
		}

        public bool AsyncRequests = true;



		/// <summary>re
		/// Returns the Physical Path
		/// </summary>
		/// <param _Name="sMyWebServerRoot">Web Server Root Directory</param>
		/// <param _Name="sDirName">Virtual Directory </param>
		/// <returns>Physical local Path</returns>
		public string GetLocalPath(string sMyWebServerRoot, string sDirName)
		{

			return "\\";
		}

		public void ListenerThreadFunc(object data)
		{
            WebListener webListener = data as WebListener;
            if (webListener == null)
            {
                return;
            }

            _RunCount++;
			while(!_bQuit)
			{
				try
				{
					//Accept a new connection
                    Socket mySocket = webListener.Listener.AcceptSocket();
                    if (mySocket.Connected)
                    {
                        if (AsyncRequests)
                        {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(HandleRequest), mySocket);

                        }
                        else
                        {
                            HandleRequest(mySocket);
                        }
                    }
				}
				catch 
				{
                    if (Utils.Logging) { Utils.WriteLogMessage("Web Server Listener Exception"); }
				}
			}
            _RunCount--;
		}

        

        public static void HandleRequest(object param)
        {
            Socket mySocket = (Socket) param;
            int iStartPos = 0;
            String sRequest;
            String sErrorMessage;
            try
            {


                //Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\nLocal IP {1}", 		mySocket.RemoteEndPoint, mySocket.LocalEndPoint) ;

                //make a byte array and receive data from the client 
                Byte[] bReceive = new Byte[4096];
                int i = mySocket.Receive(bReceive, bReceive.Length, 0);

                //Convert Byte to String
                string sBuffer = Encoding.UTF8.GetString(bReceive, 0, i);
                string body = "";
                int bodySize = 0;

                
                if (string.IsNullOrEmpty(sBuffer) || (sBuffer.Substring(0, 3) != "GET" && sBuffer.Substring(0, 4) != "POST"))
                {
                    mySocket.Close();
                    return;
                }

                if (sBuffer.Substring(0, 4) == "POST")
                {
                    string contentLength = "Content-Length:";
                    int start = sBuffer.IndexOf(contentLength);
                    int bodyLeft = bodySize;
                    if (start > -1)
                    {
                        start += contentLength.Length;
                        string length = sBuffer.Substring(start);
                        length = length.Substring(0, length.IndexOf("\r"));
                        bodySize = Convert.ToInt32(length);
                        bodyLeft = bodySize;
                        int bodyStart = sBuffer.IndexOf("\r\n\r\n")+4;
                        if (sBuffer.Length > bodyStart)
                        {
                            body = sBuffer.Substring(bodyStart);
                            bodyLeft = bodySize - body.Length;
                        }

                    }
                    bool sentContinue = false;



                    int count = 0;
                    bool firstTime = true;
                   
                    if (bodyLeft > 0)
                    { 
                        StringBuilder sb = new StringBuilder(body, bodySize);
                        while (bodyLeft > 0)
                        {
                            if (!firstTime && (bodyLeft == bodySize) && !sentContinue && (sBuffer.Contains("Expect: 100-continue")))
                            {
                                RequestHandler.SendContinue(ref mySocket);
                                sentContinue = true;
                            }
                            firstTime = false;
                            string moreData = RequestHandler.GetBody(bodyLeft, ref mySocket);
                            bodyLeft -= moreData.Length;
                            sb.Append(moreData);
                            count++;
                        }
                        body = sb.ToString();
                    }
                }



                // Look for HTTP request
                iStartPos = sBuffer.IndexOf("HTTP", 1);
                // Get the HTTP _Text and version e.g. it will return "HTTP/1.1"
                RequestHandler.HttpVersion = sBuffer.Substring(iStartPos, 8);
                // Extract the Requested Type and Requested file/directory
                sRequest = sBuffer.Substring(0, iStartPos - 1);



                //Replace backslash with Forward Slash, if Any
                sRequest.Replace("\\", "/");
                //Console.WriteLine("Request: " + sRequest);

                // Check authentication from whitelist

                IPEndPoint ipep = mySocket.RemoteEndPoint as IPEndPoint;


                bool authenticated = true;

                if (ipep != null)
                {
                    authenticated = IsAuthorized(ipep.Address);
                }


                try
                {
                    if (Utils.Logging) { Utils.WriteLogMessage("Web Request:" + sRequest); }
                    RequestHandler rh = RequestHandler.GetHandler(sRequest);
                    if (rh != null)
                    {
                        rh.ProcessRequest(sRequest, ref mySocket, authenticated, body);
                        mySocket.Close();
                        return;
                    }

                    //Console.WriteLine("Sending 404 error");
                    sErrorMessage = "<H2>Error!! No Default File Name Specified</H2>";
                    RequestHandler.SendHeader(RequestHandler.HttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                    RequestHandler.SendToBrowser(sErrorMessage, ref mySocket);
                    mySocket.Close();
                }
                catch (Exception ex)
                {
                    if (Utils.Logging) { Utils.WriteLogMessage("Web Server Request Exception" + ex.Message); }
                    sErrorMessage = "<H2>Error: " + ex.ToString() + "</H2>";
                    RequestHandler.SendHeader(RequestHandler.HttpVersion, "", sErrorMessage.Length, " 200 OK", ref mySocket);
                    RequestHandler.SendHeader(RequestHandler.HttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                    RequestHandler.SendToBrowser(sErrorMessage, ref mySocket);
                    mySocket.Close();
                    return;
                }

            }
            catch
            {

            }
            
        }
	}
}
