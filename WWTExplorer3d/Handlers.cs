/*  
===============================================================================
 2007-2008 Copyright © Microsoft Corporation.  All rights reserved.
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
using System.Collections;
using System.Threading ;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TerraViewer
{
	public class QueryString
	{
	    readonly Hashtable _htRequest;
		public QueryString(string request)
		{
			_htRequest = new Hashtable();

            if (request.IndexOf("?") == -1)
            {
                return;
            }

			var remaining = request.Substring(request.IndexOf("?")+1);
			var parameters = remaining.Split('&');
			
            if (parameters.GetLength(0) == 1 & parameters[0] == "")
            {
                return;
            }

			foreach(var keyval in parameters)
			{   
                var key = "";
                var val = "";
                if (keyval.IndexOf("=") > -1)
                {
                    key = keyval.Substring(0, keyval.IndexOf("="));
                }
                if (keyval.IndexOf("=") > -1)
                {
                    val = keyval.Substring(keyval.IndexOf("=") + 1);
                    val = HttpUtility.UrlDecode(val);
                }

                if (!_htRequest.ContainsKey(key))
                {
                    _htRequest[key] = new ArrayList();
                }
                
				var arValues = (ArrayList)_htRequest[key];
			    arValues.Add(val);
			}
		}
		public string[] GetValues(string key)
		{
			var arValues = (ArrayList)_htRequest[key];
			var vals = new string[arValues.Count];
			arValues.CopyTo(vals, 0);
			return vals;
		}
		public string this[string key]
		{
			get 
			{
				var arValues = (ArrayList)_htRequest[key];
				if (arValues == null)
					return null;
				return (string)arValues[0];
			}
		}	
	}

	public abstract class RequestHandler
	{
		private static readonly ArrayList _Handlers = new ArrayList();
		public static string HttpVersion="";
        static string _RootWebfiles = "TerraViewer.Cosmos."; // "IMDFrame.WebFiles." 
        static readonly Dictionary<string, string> _WebTextFileCache = new Dictionary<string, string>();

		public static void RegisterHandler(RequestHandler rh)
		{
			_Handlers.Add(rh);
		}
        public static RequestHandler GetHandler(string request)
		{
			foreach (RequestHandler rh in _Handlers)
			{
				if (rh.Handles(request))
					return rh;
			}
			return null;
		}
		public abstract bool Handles(string request);
        public static string Checked(bool isChecked)
        {
            if (isChecked)
            {
                return "true";
            }
            else
            {
                return "false";
            }
        }	
        public abstract void ProcessRequest(string request, ref Socket socket, bool authenticated, string body);
        public static void SendContinue(ref Socket mySocket)
        {
            var sBuffer = new StringBuilder("", 1024);

            sBuffer.Append(HttpVersion + " 100 Continue\r\n\r\n");
            var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString());
            SendToBrowser(bSendData, bSendData.Length, ref mySocket);
        }

        public static string GetBody(int size, ref Socket mySocket)
        {
            if (size > 0)
            {
                var bReceive = new Byte[size];
                mySocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 40000);
                var i = mySocket.Receive(bReceive, bReceive.Length, 0);

                //Convert Byte to String
                return Encoding.UTF8.GetString(bReceive, 0, i);
            }
            return "";
        }

		/// <summary>
		/// This function send the Header Information to the client (Browser)
		/// </summary>
		/// <param _Name="HttpVersion">HTTP Version</param>
		/// <param _Name="sMIMEHeader">Mime Type</param>
		/// <param _Name="iTotBytes">Total Bytes to be sent in the body</param>
		/// <param _Name="mySocket">Socket reference</param>
		/// <returns></returns>
		public static void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
		{
			var sBuffer = new StringBuilder("", 1024);
			// if Mime _CacheType is not provided set default to _Text/html
			if (sMIMEHeader.Length == 0 )
			{
				sMIMEHeader = "_Text/html";  // Default Mime Type is _Text/html
			}
			sBuffer.Append(sHttpVersion + sStatusCode + "\r\n");
			sBuffer.Append("Server: cx1193719-b\r\n");
            sBuffer.Append("Cache-Control: no-cache\r\n");
			sBuffer.Append("Content-Type: " + sMIMEHeader + "\r\n");
			sBuffer.Append("Accept-Ranges: bytes\r\n");
			sBuffer.Append("Content-Length: " + iTotBytes + "\r\n\r\n");
			var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString()); 
			SendToBrowser( bSendData, bSendData.Length, ref mySocket);
		}



		public static void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket, bool cache)
		{
			var sBuffer = new StringBuilder("", 1024);
			// if Mime _CacheType is not provided set default to _Text/html
			if (sMIMEHeader.Length == 0 )
			{
				sMIMEHeader = "_Text/html";  // Default Mime Type is _Text/html
			}
			sBuffer.Append(sHttpVersion + sStatusCode + "\r\n");
			sBuffer.Append("Server: cx1193719-b\r\n");
            if (cache)
            {
                sBuffer.Append("Cache-Control: max-age=604800\r\n");
            }
            else
            {
                sBuffer.Append("Cache-Control: no-cache\r\n");
            }
			sBuffer.Append("Content-Type: " + sMIMEHeader + "\r\n");
			sBuffer.Append("Accept-Ranges: bytes\r\n");
			sBuffer.Append("Content-Length: " + iTotBytes + "\r\n\r\n");
			var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString()); 
			SendToBrowser( bSendData, bSendData.Length, ref mySocket);
		}
        public static void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, string location, ref Socket mySocket)
        {
            var sBuffer = new StringBuilder("", 1024);
            // if Mime _CacheType is not provided set default to _Text/html
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "_Text/html";  // Default Mime Type is _Text/html
            }
            sBuffer.Append(sHttpVersion + sStatusCode + "\r\n");
            sBuffer.Append("Server: cx1193719-b\r\n");
            sBuffer.Append("Content-Type: " + sMIMEHeader + "\r\n");
            sBuffer.Append("Accept-Ranges: bytes\r\n");
            sBuffer.Append("Location: "+location+"\r\n");

            sBuffer.Append("Content-Length: " + iTotBytes + "\r\n\r\n");
            var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString());
            SendToBrowser(bSendData, bSendData.Length, ref mySocket);
        }

        public static void SendCookieHeaderRedirect(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, string location, ref Socket mySocket, string cookie)
        {
            var sBuffer = new StringBuilder("", 1024);
            // if Mime _CacheType is not provided set default to _Text/html
            if (sMIMEHeader.Length == 0)
            {
                sMIMEHeader = "_Text/html";  // Default Mime Type is _Text/html
            }
            sBuffer.Append(sHttpVersion + sStatusCode + "\r\n");
            sBuffer.Append("Server: cx1193719-b\r\n");
            sBuffer.Append("Content-Type: " + sMIMEHeader + "\r\n");
            sBuffer.Append("Accept-Ranges: bytes\r\n");
            sBuffer.Append("Location: "+location+"\r\n");
            sBuffer.Append("Set-Cookie: " + cookie + "\r\n");

            sBuffer.Append("Content-Length: " + iTotBytes + "\r\n\r\n");
            var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString());
            SendToBrowser(bSendData, bSendData.Length, ref mySocket);
        }

		public static void SendHeader(string sHttpVersion, string sMIMEHeader, string filename, bool renderInBrowser, int iTotBytes, string sStatusCode, ref Socket mySocket)
		{
			var sBuffer = new StringBuilder("", 1024);
			// if Mime _CacheType is not provided set default to _Text/html
			if (sMIMEHeader.Length == 0 )
			{
				sMIMEHeader = "_Text/html";  // Default Mime Type is _Text/html
			}
			sBuffer.Append(sHttpVersion + sStatusCode + "\r\n");
			sBuffer.Append("Server: cx1193719-b\r\n");
			sBuffer.Append("Content-Type: " + sMIMEHeader + "\r\n");
			if (renderInBrowser)
			{
				sBuffer.Append("Content-Disposition: " + "inline; filename=" + filename + "\r\n");
			}
			else
			{
				sBuffer.Append("Content-Disposition: " + "attachment; filename=" + filename + "\r\n");
			}
			sBuffer.Append("Accept-Ranges: bytes\r\n");
			sBuffer.Append("Content-Length: " + iTotBytes + "\r\n\r\n");
			var bSendData = Encoding.UTF8.GetBytes(sBuffer.ToString()); 
			SendToBrowser( bSendData, bSendData.Length, ref mySocket);
		}

        public string ReadWebFile(string fileName)
        {
            string contents = null;
            try
            {
                fileName = fileName.ToLower();
       
                var fileNameKey = fileName;
                
                if (_WebTextFileCache.ContainsKey(fileNameKey))
                {
                    return _WebTextFileCache[fileNameKey];
                }

                var file = (Stream)Assembly.GetExecutingAssembly().GetManifestResourceStream(_RootWebfiles + fileName);

                var reader = new StreamReader(file);
                contents = reader.ReadToEnd();

                contents = TranslateContents(contents);

                reader.Close();

                _WebTextFileCache.Add(fileNameKey, contents);

            }
            catch
            {
            }
            return contents;
        }
        public string TranslateContents(string contents)
        {
            MatchCollection mc = null;

            var matchPatern = @"<!--\((?<1>\d*)\)-->[^<]*<!--\(.\)-->";
            var documentTitle = new Regex(matchPatern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.Singleline);
            mc = documentTitle.Matches(contents);

            if (mc.Count > 0)
            {
                var keywords = new List<string>();
                foreach (Match m in mc)
                {
                    var id = Convert.ToInt32(m.Result("$1"));
                   // contents = contents.Replace(m.Value,Language.GetLocalizedText(id));
                }
            }
            return contents;
        }
        public byte[] ReadBinaryWebFile(string fileName)
        {
            byte[] data = null;
            try
            {
                fileName = fileName.ToLower();
                var file = (Stream)Assembly.GetExecutingAssembly().GetManifestResourceStream(_RootWebfiles+ fileName);
                data = new byte[file.Length];

                file.Read(data, 0, (int)file.Length);

                file.Close();

            }
            catch
            {
            }
            return data;

        }
        public byte[] ReadBinaryWebFileFromDisk(string fileName)
        {
            byte[] data = null;
            try
            {
                fileName = fileName.ToLower();
                Stream file = File.Open(fileName, FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
                data = new byte[file.Length];

                file.Read(data, 0, (int)file.Length);

                file.Close();

            }
            catch
            {
            }
            return data;

        }


        public static void SendBinaryFileFromDisk(string filename, ref Socket socket, String sMimeType)
        {
            var fi = new FileInfo(filename);

            var iTotBytes = (int)fi.Length;
            SendHeader(HttpVersion, sMimeType, iTotBytes, " 200 OK", ref socket);


            Stream file = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var data = new byte[4096];
            while (iTotBytes > 0)
            {
                var sizeToRead = Math.Min(iTotBytes, 4096);

                sizeToRead = file.Read(data, 0, sizeToRead);

                iTotBytes -= sizeToRead;

                SendToBrowser(data, sizeToRead, ref socket);
            }
            file.Close();
        }



        public static void SendHeaderAndData(string sData, ref Socket socket, String sMimeType)
        {
            var buf = Encoding.UTF8.GetBytes(sData);
            var iTotBytes = buf.Length;
            SendHeader(HttpVersion, sMimeType, iTotBytes, " 200 OK", ref socket);
            SendToBrowser(buf, iTotBytes, ref socket);
        }

		/// <summary>
		/// Overloaded Function, takes string, convert to bytes and calls 
		/// overloaded sendToBrowserFunction.
		/// </summary>
		/// <param _Name="sData">The data to be sent to the browser(client)</param>
		/// <param _Name="mySocket">Socket reference</param>
        public static void SendToBrowser(String sData, ref Socket mySocket)
        {
            var buf = Encoding.ASCII.GetBytes(sData);
            SendToBrowser(buf, buf.Length, ref mySocket);
        }



		/// <summary>
		/// Sends data to the browser (client)
		/// </summary>
		/// <param _Name="bSendData">Byte Array</param>
		/// <param _Name="mySocket">Socket reference</param>
		public static void SendToBrowser(Byte[] bSendData, int len, ref Socket mySocket)
		{
			var numBytes = 0;
			
			try
			{
                if (mySocket.Connected)
                {
                    numBytes = mySocket.Send(bSendData, len, 0);
                }
			}
			catch
			{
				
							
			}
		}

        public void RedirectToAuthPage(ref Socket socket)
        {
            var data = "<HTML>Redirected</HTML";

            var iTotBytes = data.Length;
            SendHeader(HttpVersion, "", iTotBytes, " 302 Moved", "/Configuration/sign_in.html", ref socket);
            SendToBrowser(data, ref socket);
        }
	
	}
}
