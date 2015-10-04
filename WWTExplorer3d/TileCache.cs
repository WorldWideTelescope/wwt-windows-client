using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.IO;	
using System.Threading;
using System.Text;

namespace TerraViewer
{
    public delegate Tile LoadTileFromWebDeleagte(Tile tile);

	

	/// <summary>
	/// Summary description for TileCache.
	/// </summary>
	public class TileCache
	{

        private static readonly Dictionary<long, Tile> queue = new Dictionary<long, Tile>();

        public static int QueuePercent
        {
            get
            {
                var count = queue.Count;
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

        private static readonly Dictionary<long, Tile> tiles = new Dictionary<long, Tile>();

		public static void ClearCache()
		{
            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Clear Cache"); }
            tileMutex.WaitOne();
            queueMutex.WaitOne();
            WaitingTileQueueMutex.WaitOne();
            try
            {
                WaitingTileQueue = new Queue<Tile>();
                foreach (var t in tiles.Values)
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
            var tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
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
            var tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
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
                GetTileFromWeb(retTile, false);
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
            return tiles[childKey];
        }


	    public static Tile GetCachedTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            if (level < dataset.BaseLevel)
            {
                return null;
            }

            Tile retTile = null;
            var tileKey = ImageSetHelper.GetTileKey(dataset, level, x, y);
            try
            {
                if (!tiles.ContainsKey(tileKey))
                {
                    return null;
                }
                retTile = tiles[tileKey];
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
                    queue[tile.Key].RequestHits += hitValue;
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

            var array = new object[queue.Count];
            var index = 0;
			var sb = new StringBuilder();
			foreach (var t in queue.Values)
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
        static int purgeCallCount;
        static readonly SortedList<int, Tile> notReadyCullList = new SortedList<int, Tile>();
        static readonly SortedList<int, Tile> readyCullList = new SortedList<int, Tile>();
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
                foreach (var tile in tiles.Values)
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
                    foreach (var tile in tiles.Values)
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
                    var totalToPurge = readyCullList.Count - maxReadyToRenderSize;
                    if (totalToPurge > maxTotalToPurge)
                    {
                        totalToPurge = maxTotalToPurge;
                        
                        // UiTools.Beep();
                    }
                    foreach (var tile in readyCullList.Values)
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
                    var totalToPurge = notReadyCullList.Count - maxTileCacheSize;
                    if (totalToPurge > maxTotalToPurge)
                    {
                        totalToPurge = maxTotalToPurge;
                    }           
                    foreach (var tile in notReadyCullList.Values)
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

            var count = 0;

            try
            {
                try
                {
                    foreach (var tile in tiles.Values)
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

      

        private static readonly Mutex tileMutex = new Mutex();
        private static readonly Mutex queueMutex = new Mutex();

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
            
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);
            var fileOnly = fileOnlyThreadID == Thread.CurrentThread.ManagedThreadId;
			while (running)
			{
                if (queue.Count < 1)
                {
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(1);
                }

                double minDistance = 1000000000000000000; 
                var overlayTile = false;
				long maxKey = 0;
                var level = 1000;

                queueMutex.WaitOne();
				foreach (var t in queue.Values )
				{
					 
                    if (!t.RequestPending ) // && t.InViewFrustum)
					{
                        var vectTemp = new Vector3d(t.SphereCenter);

                        vectTemp.TransformCoordinate(Earth3d.WorldMatrix);

                        if (Earth3d.MainWindow.Space)
                        {
                            vectTemp.Subtract(new Vector3d(0.0f, 0.0f, -1.0f));
                        }
                        else
                        {
                            vectTemp.Subtract(Earth3d.MainWindow.RenderContext11.CameraPosition);
                        }

                        var distTemp = Math.Max(0,vectTemp.Length()-t.SphereRadius);

                        var thisIsOverlay = (t.Dataset.Projection == ProjectionType.Tangent) || (t.Dataset.Projection == ProjectionType.SkyImage);
                        if (distTemp < minDistance && (!overlayTile || thisIsOverlay))
						{

                            var test = queue[t.Key];



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
					var workTile = queue[maxKey];
					workTile.RequestPending = true;
					RequestCount++;
                    queueMutex.ReleaseMutex();
					GetTileFromWeb(workTile, true);
                    queueMutex.WaitOne();
					RequestCount--;
                    workTile.RequestPending = false;
					queue.Remove(workTile.Key);
				}

                queueMutex.ReleaseMutex();
			}
			return;
		}

       static readonly List<long> removeList = new List<long>();

		// Age things in queue. If they are not visible they will go away in time
		public static void DecimateQueue()
		{
            queueMutex.WaitOne();
            
            removeList.Clear();
            try
            {

                foreach (var t in queue.Values)
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
                foreach (var key in removeList)
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
		static readonly Thread[] queueThreads = new Thread[THREAD_COUNT];
        static int fileOnlyThreadID;

		public static void StartQueue()
		{
			if (!running)
			{
				running = true;
				for (var i = 0; i<THREAD_COUNT; i++)
				{
					queueThreads[i] = new Thread(QueueThread);
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
			if (running)
			{
				running = false;
                WaitingTileQueueEvent.Set();
                Thread.Sleep(2000);
				for (var i = 0; i<THREAD_COUNT; i++)
				{
                    WaitingTileQueueEvent.Set();
					queueThreads[i].Abort();

				}
			}
			return;
		}

       
        public static void InitializeTile(Tile tile)
        {      
            var loaded = false;

            while (!loaded && running)
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
                var tile = retTile as SkyImageTile;
                Client = new WebClient();
                var url = tile.URL;
                tile.ImageData = Client.DownloadData(url);
                retTile.DemReady = true;
                retTile.FileExists = true;
                retTile.ReadyToRender = true;
                retTile.TextureReady = true;

                //retTile.CreateGeometry(Tile.prepDevice);
                Client.Dispose();
                return retTile;
            }

            var parent = retTile.Parent;

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


			var directory = retTile.Directory;
	
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			var filename = retTile.FileName;
			Client = new WebClient();

            var fi = new FileInfo(filename);
            var exists = fi.Exists;

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
                        var url = retTile.URL;
                        if (string.IsNullOrEmpty(url))
                        {
                            retTile.errored = true;
                        }
                        else
                        {
                            var dlFile = string.Format("{0}.tmp{1}",filename,NodeID);

                            

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
                    var tileID = retTile.GetTileID();

                    //check coverage cache before downloading
                    var gen = CoverageMap.GetCoverage(tileID);
                    if (gen > 0)
                    {
                        //try downloading mesh
                        try
                        {
                            var meshFilename = retTile.FileName + ".mesh";

                            if (!File.Exists(meshFilename))
                            {
                                Client.Headers.Add("User-Agent", "Win8Microsoft.BingMaps.3DControl/2.214.2315.0 (;;;;x64 Windows RT)");

                                var dlFile = string.Format("{0}.tmp{1}", meshFilename, NodeID);


                                Client.DownloadFile(string.Format("http://ak.t{1}.tiles.virtualearth.net/tiles/mtx{0}?g={2}", tileID, Tile.GetServerID(retTile.X, retTile.Y), gen), dlFile);

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
			catch (Exception)
			{
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile Initialize: Exception"); }
				retTile.errored = true;
			}

			return retTile;
		}
        static readonly Mutex WaitingTileQueueMutex = new Mutex();
        static Queue<Tile> WaitingTileQueue = new Queue<Tile>();
        static double CountToLoad = 10;
        static readonly EventWaitHandle WaitingTileQueueEvent = new AutoResetEvent(false);
        public static void InitNextWaitingTile()
        {
            WaitingTileQueueMutex.WaitOne();
            if (CountToLoad < Properties.Settings.Default.TileThrottling / 60.0)
            {
                CountToLoad += Properties.Settings.Default.TileThrottling / 60.0;
            }
            WaitingTileQueueMutex.ReleaseMutex();
            WaitingTileQueueEvent.Set();
        }

        public static void GetDemTileFromWeb(Tile retTile)
        {
            var directory = retTile.DemDirectory;
  
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var filename = retTile.DemFilename;
            var fi = new FileInfo(filename);
            var exists = fi.Exists;

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
                    var Client = new WebClient();
                    Client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)");        

                    var dlFile = string.Format("{0}.tmp{1}", filename, NodeID);

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
                var newUrl = url.Replace("http://www.worldwidetelescope.org/wwtweb/thumbnail.aspx?name=", BaseUrl + "/thumbnail.aspx?name=");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t1.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                var newUrl = parts[0].Replace("http://ecn.t1.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t2.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                var newUrl = parts[0].Replace("http://ecn.t2.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t3.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                var newUrl = parts[0].Replace("http://ecn.t3.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://ecn.t0.tiles.virtualearth.net/tiles/"))
            {
                parts = url.Split(new[] { '?' });
                if (parts.Length < 2)
                {
                    return url;
                }
                var newUrl = parts[0].Replace("http://ecn.t0.tiles.virtualearth.net/tiles/", BaseUrl + "/QuadCache.aspx?k=");
                newUrl += "&p=" + parts[1].Replace("&","%26;").Replace("=","%3D");
                return newUrl;
            }
            if (url.StartsWith("http://www.worldwidetelescope.org/wwtweb/tiles.aspx"))
            {
                var newUrl = url.Replace("http://www.worldwidetelescope.org/wwtweb/tiles.aspx", BaseUrl + "/tilecache.aspx");
                return newUrl;
            }
            return url;
        }
    }
}
