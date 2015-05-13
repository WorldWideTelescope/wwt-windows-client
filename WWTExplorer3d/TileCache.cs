using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;	
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace TerraViewer
{
    public delegate Tile LoadTileFromWebDeleagte(Tile tile);

	

	/// <summary>
	/// Summary description for TileCache.
	/// </summary>
	public class TileCache
	{

        private static Dictionary<long, Tile> queue = new Dictionary<long, Tile>();

        public static int QueuePercent
        {
            get
            {
                int count = queue.Count;
                try
                {
                    return Math.Max(0, Math.Min(100, (int)(100.0 - (Math.Log(count + 1, 1.05) - 14))));
                }
                catch
                {
                    return 0;
                }

            }
        }

        private static Dictionary<long, Tile> tiles = new Dictionary<long, Tile>();

		public static void ClearCache()
		{
            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Clear Cache"); }
            tileMutex.WaitOne();
            queueMutex.WaitOne();
            WaitingTileQueueMutex.WaitOne();
            try
            {
                WaitingTileQueue = new Queue<Tile>();
                foreach (Tile t in tiles.Values)
                {
                    try
                    {
                        t.CleanUp(true);
                    }
                    catch
                    {
                       
                    }
                }
                tiles.Clear();
            }
            finally
            {
                tileMutex.ReleaseMutex();
                queueMutex.ReleaseMutex();
                WaitingTileQueueMutex.ReleaseMutex();
            }
            return;
		}

		public static int Count
		{
			get
			{
				return tiles.Count;
			}
		}

		public static void PurgeQueue()
		{
            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Purging Queue"); }
            queueMutex.WaitOne();
            queue.Clear();
			RequestCount = 0;
            queueMutex.ReleaseMutex();
		}

		public static int AccessID = 0;

        public static Tile GetTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            if (level < dataset.BaseLevel)
            {
                return null;
            }

            Tile retTile = null;
            long tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
            try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    retTile = ImageSetHelper.GetNewTile(dataset, level, x, y, parent);
                    tileMutex.WaitOne();
                    tiles.Add(tileKey, retTile);
                    tileMutex.ReleaseMutex();
                }
                else
                {
                    retTile = tiles[tileKey];
                }
            }
            catch
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("GetTile: Exception"); }
            }
            finally
            {
                //todoperf get rid of this write for GC
                retTile.AccessCount = AccessID++;
            }


            return retTile;

        }

        public static Tile GetTileNow(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            if (level < dataset.BaseLevel)
            {
                return null;
            }

            Tile retTile = null;
            long tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
            try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    retTile = ImageSetHelper.GetNewTile(dataset, level, x, y, parent);
                    tileMutex.WaitOne();
                    tiles.Add(tileKey, retTile);
                    tileMutex.ReleaseMutex();
                }
                else
                {
                    retTile = tiles[tileKey];
                }
            }
            catch
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("GetTile: Exception"); }
            }
            finally
            {
                //todoperf get rid of this write for GC
                retTile.AccessCount = AccessID++;
            }

            // Create if not ready to render

            if (!retTile.ReadyToRender)
            {
                TileCache.GetTileFromWeb(retTile, false);
                retTile.CreateGeometry(Earth3d.MainWindow.RenderContext11, false);
            }


            return retTile;

        }

        public static void RemoveTile(Tile tile)
        {
            tile.CleanUp(true);
            tileMutex.WaitOne();
            tiles.Remove(tile.Key);
            tileMutex.ReleaseMutex();
        }

        internal static Tile GetCachedTile(long childKey)
        {
            if (!tiles.ContainsKey(childKey))
            {
                return null;
            }
            else
            {
                return tiles[childKey];
            }
        }


        public static Tile GetCachedTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            if (level < dataset.BaseLevel)
            {
                return null;
            }

            Tile retTile = null;
            long tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
            try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    return null;
                }
                else
                {
                    retTile = tiles[tileKey];
                }
            }
            catch
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile Initialize: Exception"); }
            }
            


            return retTile;

        }


		public static bool AddTileToQueue(Tile tile)
		{
            queueMutex.WaitOne();

			int hitValue;

            hitValue = 256;


            if (!tile.TextureReady)
            {

                if (queue.ContainsKey(tile.Key))
                {
                    ((Tile)queue[tile.Key]).RequestHits += hitValue;
                }
                else
                {
                    tile.RequestHits = hitValue;
                    queue[tile.Key] = tile;
                }
            }

            queueMutex.ReleaseMutex();
			return true;
		}

		public static object[] GetQueueList()
		{
            queueMutex.WaitOne();

            object[] array = new object[queue.Count];
            int index = 0;
			StringBuilder sb = new StringBuilder();
			foreach (Tile t in queue.Values)
			{
                array[index++] = t;
			}
            queueMutex.ReleaseMutex();
			return array;
		}
        public static int maxTileCacheSize = 1200;
        public static int maxReadyToRenderSize = 600;


        static int maxTotalToPurge = 100;

        static int purgeFrequesncy = 30;
        static int purgeCallCount = 0;
        static SortedList<int, Tile> notReadyCullList = new SortedList<int, Tile>();
        static SortedList<int, Tile> readyCullList = new SortedList<int, Tile>();
        public static void PurgeLRU()
        {

            purgeCallCount++;
            if (purgeCallCount < purgeFrequesncy)
            {
                return;
            }
            purgeCallCount = 0;

            tileMutex.WaitOne();
            try
            {
                foreach (Tile tile in tiles.Values)
                {
                    if (tile.RenderedGeneration < (Tile.CurrentRenderGeneration - 10))
                    {
                        if (tile.ReadyToRender)
                        {
                            if (tile.Dataset.Projection == ProjectionType.Spherical)
                            {
                                tile.CleanUp(false);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            finally
            {
                tileMutex.ReleaseMutex();
            }

            if (tiles.Count < maxReadyToRenderSize)
            {
                return;
            }
            
            tileMutex.WaitOne();
            try
            {
                try
                {
                    foreach (Tile tile in tiles.Values)
                    {
                        if (tile.RenderedGeneration < (Tile.CurrentRenderGeneration - 10))
                        {
                            if (tile.ReadyToRender)
                            {
                                readyCullList.Add(tile.AccessCount, tile);
                                if (tile.Dataset.Projection == ProjectionType.Spherical)
                                {
                                    tile.CleanUp(false);
                                }
                            }
                            else
                            {
                                notReadyCullList.Add(tile.AccessCount, tile);
                            }
                        }
                    }
                }
                catch
                {
                   
                }

                if (readyCullList.Count > maxReadyToRenderSize)
                {
                    int totalToPurge = readyCullList.Count - maxReadyToRenderSize;
                    if (totalToPurge > maxTotalToPurge)
                    {
                        totalToPurge = maxTotalToPurge;
                        
                        // UiTools.Beep();
                    }
                    foreach (Tile tile in readyCullList.Values)
                    {
                        if (totalToPurge < 1)
                        {
                            break;
                        }
                        tile.CleanUp(false);
                        //
                        totalToPurge--;
                    }
                }

                if (tiles.Count < maxTileCacheSize)
                {
                    return;
                }

                if (notReadyCullList.Count > maxTileCacheSize)
                {
                    int totalToPurge = notReadyCullList.Count - maxTileCacheSize;
                    if (totalToPurge > maxTotalToPurge)
                    {
                        totalToPurge = maxTotalToPurge;
                    }           
                    foreach (Tile tile in notReadyCullList.Values)
                    {
                        if (totalToPurge < 1)
                        {
                            break;
                        }
                        tile.CleanUp(true);
                        tiles.Remove(tile.Key);

                        totalToPurge--;
                    }
                }
            }
            catch
            {
            }
            finally
            {
                tileMutex.ReleaseMutex();
                notReadyCullList.Clear();
                readyCullList.Clear();
            }

            return;
        }
        public static int GetTotalCount()
        {
            return tiles.Count;
        }

        public static int GetReadyCount()
        {
            // return;
            tileMutex.WaitOne();

            int count = 0;

            try
            {
                try
                {
                    foreach (Tile tile in tiles.Values)
                    {

                        if (tile.ReadyToRender)
                        {
                            count++;
                        }
                    }
                }
                catch
                {

                }
            }
            catch
            {
            }
            finally
            {
                tileMutex.ReleaseMutex();
            }

            return count;
        }

      

        private static Mutex tileMutex = new Mutex();
        private static Mutex queueMutex = new Mutex();

		public static bool running = false;
		public static int RequestCount = 0;
		public static int currentLevel = 0;
		public static int CurrentLevel
		{
			set
			{
				currentLevel = value;

			}
			get
			{
				return currentLevel;
			}
		}
        static DateTime lastLRUPurge = DateTime.Now;
		public static void QueueThread()
		{
            
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);
            bool fileOnly = fileOnlyThreadID == Thread.CurrentThread.ManagedThreadId;
			while (running)
			{
                if (queue.Count < 1)
                {
                    System.Threading.Thread.Sleep(50);
                }
                else
                {
                    System.Threading.Thread.Sleep(1);
                }

                double minDistance = 1000000000000000000; 
                bool overlayTile = false;
				long maxKey = 0;
                int level = 1000;

                queueMutex.WaitOne();
				foreach (Tile t in queue.Values )
				{
					 
                    if (!t.RequestPending ) // && t.InViewFrustum)
					{
                        Vector3d vectTemp = new Vector3d(t.SphereCenter);

                        vectTemp.TransformCoordinate(Earth3d.WorldMatrix);

                        if (Earth3d.MainWindow.Space)
                        {
                            vectTemp.Subtract(new Vector3d(0.0f, 0.0f, -1.0f));
                        }
                        else
                        {
                            vectTemp.Subtract(Earth3d.MainWindow.RenderContext11.CameraPosition);
                        }

                        double distTemp = Math.Max(0,vectTemp.Length()-t.SphereRadius);

                        bool thisIsOverlay = (t.Dataset.Projection == ProjectionType.Tangent) || (t.Dataset.Projection == ProjectionType.SkyImage);
                        if (distTemp < minDistance && (!overlayTile || thisIsOverlay))
						{

                            Tile test = (Tile)queue[t.Key];



                            if (!test.FileChecked)
                            {


                                test.FileExists = File.Exists(test.FileName);
                                test.FileChecked = true;
                                if (test.Volitile)
                                {
                                    test.FileExists = false;
                                }
                            }

                            if (test.FileExists || (!test.FileExists && !fileOnly))
                            {
                                minDistance = distTemp;
                                maxKey = t.Key;
                                level = t.Level;
                                overlayTile = thisIsOverlay;
                            }
    					}
					}
		
				}
				if (maxKey != 0)
				{
					Tile workTile = (Tile)queue[maxKey];
					workTile.RequestPending = true;
					TileCache.RequestCount++;
                    queueMutex.ReleaseMutex();
					TileCache.GetTileFromWeb(workTile, true);
                    queueMutex.WaitOne();
					TileCache.RequestCount--;
                    workTile.RequestPending = false;
					queue.Remove(workTile.Key);
				}

                queueMutex.ReleaseMutex();
			}
			return;
		}

       static List<long> removeList = new List<long>();

		// Age things in queue. If they are not visible they will go away in time
		public static void DecimateQueue()
		{
            queueMutex.WaitOne();
            
            removeList.Clear();
            try
            {

                foreach (Tile t in queue.Values)
                {
                    if (!t.RequestPending)
                    {
                        t.RequestHits = t.RequestHits / 2;
                        try
                        {
                            if (t.RequestHits < 2)// && !t.DirectLoad)
                            {
                                removeList.Add(t.Key);
                            }
                            else if (!t.InViewFrustum)
                            {
                                removeList.Add(t.Key);
                            }
                        }
                        catch
                        {
                        }
                    }

                }
                foreach (long key in removeList)
                {
                    queue.Remove(key);
                }
            }
            catch
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("DecimateQueue: Exception"); }
            }
            finally
            {
                
                queueMutex.ReleaseMutex();
            }
				

		}


		public static int THREAD_COUNT = 5;
		static Thread[] queueThreads = new Thread[THREAD_COUNT];
        static int fileOnlyThreadID = 0;

		public static void StartQueue()
		{
			if (!TileCache.running)
			{
				TileCache.running = true;
				for (int i = 0; i<THREAD_COUNT; i++)
				{
					queueThreads[i] = new Thread(new ThreadStart(TileCache.QueueThread));
                    queueThreads[i].Priority = ThreadPriority.BelowNormal;
					// Start the thread.
					queueThreads[i].Start();
                    if (i == 0)
                    {
                        fileOnlyThreadID = queueThreads[i].ManagedThreadId;
                    }
				}
			}
		}


		public static void ShutdownQueue()
		{
			if (TileCache.running)
			{
				TileCache.running = false;
                WaitingTileQueueEvent.Set();
                Thread.Sleep(2000);
				for (int i = 0; i<THREAD_COUNT; i++)
				{
                    WaitingTileQueueEvent.Set();
					queueThreads[i].Abort();

				}
			}
			return;
		}

       
        public static void InitializeTile(Tile tile)
        {      
            bool loaded = false;

            while (!loaded && TileCache.running)
            {
                WaitingTileQueueMutex.WaitOne();

                if (CountToLoad > 0)
                {
                    CountToLoad--;
                    CountToLoad =  Math.Max(0, CountToLoad);
                    // CountToLoad = 0;
                    loaded = true;
                }

                WaitingTileQueueMutex.ReleaseMutex();

                if (loaded)
                {
                    if (tile != null)
                    {
                        tile.CreateGeometry(Earth3d.MainWindow.RenderContext11, false);
                        tilesLoadedThisFrame++;
                    }
                }
                else
                {
                    WaitingTileQueueEvent.WaitOne();
                }

            }
        }


        static public int tilesLoadedThisFrame = 0;
        static public int NodeID = 0;

        public static Tile GetTileFromWeb(Tile retTile, bool Initialize)
		{
            WebClient Client = null;
            if (retTile.Dataset.Projection == ProjectionType.SkyImage && retTile.Dataset.Url.EndsWith("/screenshot.png"))
            {
                SkyImageTile tile = retTile as SkyImageTile;
                Client = new WebClient();
                string url = tile.URL;
                tile.ImageData = Client.DownloadData(url);
                retTile.DemReady = true;
                retTile.FileExists = true;
                retTile.ReadyToRender = true;
                retTile.TextureReady = true;

                //retTile.CreateGeometry(Tile.prepDevice);
                Client.Dispose();
                return retTile;
            }

            Tile parent = retTile.Parent;

            if (retTile.DemEnabled && (parent == null || (parent != null && parent.DemGeneration == 0)))
            {
                GetDemTileFromWeb(retTile);
            }
            else
            {
                retTile.DemReady = true;
            }

            if (retTile.Dataset.WcsImage != null && retTile.Dataset.WcsImage is FitsImage)
            {
                retTile.TextureReady = true;
                InitializeTile(retTile);
                return retTile;
            }


			string directory = retTile.Directory;
	
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			string filename = retTile.FileName;
			Client = new WebClient();

            FileInfo fi = new FileInfo(filename);
            bool exists = fi.Exists;

            if (exists)
			{
                if (fi.Length != 8 && fi.Length < 100 || retTile.Volitile)
				{
					try
					{
						File.Delete(filename);
					}
					catch
					{
                      
					}
                    exists = false;
				}
			}


            if (!exists)
			{
                try
                {
                    if (retTile.Dataset.IsMandelbrot)
                    {
                        retTile.ComputeMandel();
                        fi = new FileInfo(filename);
                        exists = fi.Exists;
                    }
                    else
                    {
                        Client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");
                        string url = retTile.URL;
                        if (string.IsNullOrEmpty(url))
                        {
                            retTile.errored = true;
                        }
                        else
                        {
                            string dlFile = string.Format("{0}.tmp{1}",filename,NodeID);

                            

                            Client.DownloadFile(CacheProxy.GetCacheUrl(url), dlFile);
                            try
                            {
                                if (File.Exists(dlFile))
                                {
                                    if (File.Exists(filename))
                                    {
                                        File.Delete(filename);
                                    }
                                    File.Move(dlFile, filename);
                                }
                            }
                            catch
                            {
                             //   UiTools.ShowMessageBox("File Download collision catch");
                            }
                            fi = new FileInfo(filename);
                            exists = fi.Exists;
                        }
                        // Code for drawing tile it onto tile for debuggin
                        //if (retTile.Dataset.Projection == ProjectionType.Toast)
                        //{
                        //    //Bitmap bmpText = new Bitmap(filename);
                        //    Bitmap bmpText = UiTools.LoadBitmap(filename);
                        //    Graphics g = Graphics.FromImage(bmpText);
                        //    g.DrawString(retTile.Key, UiTools.StandardRegular, UiTools.StadardTextBrush, new RectangleF(0, 0, 255, 255), UiTools.StringFormatCenterCenter);
                        //    g.Flush();
                        //    g.Dispose();
                        //    bmpText.Save(filename);

                        //    bmpText.Dispose();
                        //}
                    }
                }
                catch 
                {
                    //todo retry login on non - HTTP failuers.
                    if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile Download: Exception" ); }
                    retTile.errored = true;
                }
			}
			try 
			{
				if (exists)
				{
					if (fi.Length < 100 || (fi.Length == 1033))
					{
						retTile.errored = true;
						return retTile;
					}			
				}

                // todo 3d Cities remove support for 3d Cities for now
                if (retTile.Dataset.Projection == ProjectionType.Mercator && Properties.Settings.Default.Show3dCities)
                {
                    string tileID = retTile.GetTileID();

                    //check coverage cache before downloading
                    int gen = CoverageMap.GetCoverage(tileID);
                    if (gen > 0)
                    {
                        //try downloading mesh
                        try
                        {
                            string meshFilename = retTile.FileName + ".mesh";

                            if (!File.Exists(meshFilename))
                            {
                                Client.Headers.Add("User-Agent", "Win8Microsoft.BingMaps.3DControl/2.214.2315.0 (;;;;x64 Windows RT)");

                                string dlFile = string.Format("{0}.tmp{1}", meshFilename, NodeID);


                                Client.DownloadFile(string.Format("http://ak.t{1}.tiles.virtualearth.net/tiles/mtx{0}?g={2}", tileID, Tile.GetServerID(retTile.X, retTile.Y), gen.ToString()), dlFile);

                                try
                                {
                                    if (File.Exists(dlFile))
                                    {
                                        if (File.Exists(meshFilename))
                                        {
                                            File.Delete(meshFilename);
                                        }
                                        File.Move(dlFile, meshFilename);
                                    }
                                }
                                catch
                                {
                                  //  UiTools.ShowMessageBox("File Download collision catch");
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                }

                retTile.FileExists = true;
                
                retTile.TextureReady = true;


                if (Initialize)
                {
                    InitializeTile(retTile);
                }
			}
			catch (System.Exception)
			{
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile Initialize: Exception"); }
				retTile.errored = true;
			}

			return retTile;
		}
        static Mutex WaitingTileQueueMutex = new Mutex();
        static Queue<Tile> WaitingTileQueue = new Queue<Tile>();
        static double CountToLoad = 10;
        static EventWaitHandle WaitingTileQueueEvent = new AutoResetEvent(false);
        public static void InitNextWaitingTile()
        {
            WaitingTileQueueMutex.WaitOne();
            if (CountToLoad < (double)Properties.Settings.Default.TileThrottling / 60.0)
            {
                CountToLoad += (double)Properties.Settings.Default.TileThrottling / 60.0;
            }
            WaitingTileQueueMutex.ReleaseMutex();
            WaitingTileQueueEvent.Set();
        }

        public static void GetDemTileFromWeb(Tile retTile)
        {
            string directory = retTile.DemDirectory;
  
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = retTile.DemFilename;
            FileInfo fi = new FileInfo(filename);
            bool exists = fi.Exists;

            if (exists)
            {
                if (fi.Length != 2178 && fi.Length != 1026 && fi.Length != 2052 && (retTile.Dataset.Projection != ProjectionType.Mercator))
                {
                    try
                    {
                        File.Delete(filename);
                    }
                    catch
                    {
                    }
                    exists = false;
                }

            }


            if (!exists)
            {
                try
                {
                    WebClient Client = new WebClient();
                    Client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");        

                    string dlFile = string.Format("{0}.tmp{1}", filename, NodeID);

                    Client.DownloadFile(CacheProxy.GetCacheUrl(retTile.DemURL), dlFile);
                    try
                    {
                        if (File.Exists(dlFile))
                        {
                            if (File.Exists(filename))
                            {
                                File.Delete(filename);
                            }
                            File.Move(dlFile, filename);
                        }
                    }
                    catch
                    {
                       
                    }
                    fi = new FileInfo(filename);
                    exists = fi.Exists;
                }
                catch
                {
                    if (Earth3d.Logging) { Earth3d.WriteLogMessage("Dem Download: Exception"); }
                    exists = false;

                }
            }

            retTile.DemReady = true;


            retTile.demFileExists = exists;

            return;
        }

        
    }
    public class FakeMutex
    {
        public FakeMutex()
        {
        }
        public void WaitOne()
        {
        }
        public void ReleaseMutex()
        {
        }
    }

    public class CacheProxy
    {
        static public string BaseUrl;

        static public string GetCacheUrl(string url)
        {
            if (string.IsNullOrEmpty(BaseUrl))
            {
                return url;
            }
            // map QuadCache.aspx

            //map thumbnails
            string[] parts;

            if (url.StartsWith("http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name="))
            {
                string newUrl = url.Replace("http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=", BaseUrl + "/thumbnail.aspx?name=");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t1.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new char[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                string newUrl = parts[0].Replace("http://ecn.t1.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t2.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new char[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                string newUrl = parts[0].Replace("http://ecn.t2.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t3.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new char[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                string newUrl = parts[0].Replace("http://ecn.t3.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t0.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new char[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                string newUrl = parts[0].Replace("http://ecn.t0.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://www.worldwidetelescope.org/wwtweb/tiles.aspx"))
            {
                string newUrl = url.Replace("http://www.worldwidetelescope.org/wwtweb/tiles.aspx", BaseUrl + "/tilecache.aspx");
                return newUrl;
            }
            return url;
        }
    }
}
