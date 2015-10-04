using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Maps.ElevationAdjustmentService.HDPhoto;
using SharpDX;
using SharpDX.Direct3D11;
using WwtDataUtils;
using Color = System.Drawing.Color;

namespace TerraViewer
{

    public abstract class Tile
    {
        public const bool UseDefault = false;
        //public int tileSize = 256;
        //public static bool ShowGrid = false;
        protected int level;
        public int AccessCount = TileCache.AccessID++;
        public double[] DemData = null;
        public bool DemReady = false;
        protected int demIndex = 0;
        public bool TextureReady = false;
        public static int CurrentRenderGeneration = 0;
        public int Level
        {
            get
            {
                return level;
            }
            set
            {
                level = value;
            }
        }
        protected int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        public int y;
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        public bool FileChecked = false;
        public bool FileExists = false;

        protected VertexBuffer11 vertexBuffer = null;
        protected IndexBuffer11[] indexBuffer = null;
        public Texture11 texture = null;

        public static int iTileBuildCount = 0;
        public bool ReadyToRender = false;
        public bool InViewFrustum = true;
        public static int TilesInView = 0;
        public static int TrianglesRendered = 0;
        public static int TilesTouched = 0;
        //public static List<Tile> frustumList = null;

        protected Vector3d TopLeft;
        protected Vector3d BottomRight;
        protected Vector3d TopRight;
        protected Vector3d BottomLeft;

        public virtual IndexBuffer11 GetIndexBuffer(int index, int accom)
        {
            return indexBuffer[index];
        }



        public virtual double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (level < lastDeepestLevel)
            {
                //interate children
                foreach (var childKey in childrenId)
                {
                    var child = TileCache.GetCachedTile(childKey);
                    if (child != null)
                    {
                        if (child.IsPointInTile(lat, lng))
                        {
                            return child.GetSurfacePointAltitude(lat, lng, meters);
                        }
                    }
                }
            }

            return demAverage / DemScaleFactor;
        }


        public virtual double GetSurfacePointAltitudeNow(double lat, double lng, bool meters, int targetLevel)
        {

            if (level < targetLevel)
            {
                var yOffset = 0;
                if (dataset.Mercator || dataset.BottomsUp)
                {
                    yOffset = 1;
                }
                const int xOffset = 0;

                const int xMax = 2;
                var childIndex = 0;
                for (var y1 = 0; y1 < 2; y1++)
                {
                    for (var x1 = 0; x1 < xMax; x1++)
                    {

                        if (level < dataset.Levels && level < (targetLevel+1))
                        {
                            var child = TileCache.GetTileNow(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
                            childrenId[childIndex++] = child.Key;
                            if (child != null)
                            {
                                if (child.IsPointInTile(lat, lng))
                                {
                                    return child.GetSurfacePointAltitudeNow(lat, lng, meters, targetLevel);
                                }
                            }
                        }
                    }
                }
            }

            return demAverage / DemScaleFactor;
        }

        public virtual bool IsPointInTile(double lat, double lng)
        {
            return false;
        }

        internal double demAverage = 0;

        public override string ToString()
        {
            var sb = new StringBuilder();
			
				var t = this;
				sb.Append(t.Level);
				sb.Append(" - ");
				sb.Append(t.X);
				sb.Append("_");
				sb.Append(t.Y);
				sb.Append("\t: ");
				sb.Append(t.RequestHits);
				if (t.RequestPending)
				{
					sb.Append("*");
				}
               
                sb.Append(", ");
                sb.Append(dataset.Projection);
				sb.Append("\r\n");
                return sb.ToString();
        }

        // todoperf get rid of pointers here and look them up directly
        //protected Tile[] children = new Tile[4] { null, null, null, null };
        //public Tile Parent = null;

        public Tile Parent
        {
            get
            {
                return TileCache.GetCachedTile(level - 1, x / 2, y / 2, dataset, null);
            }
        }

        public long[] childrenId = new long[4];


        public static bool PurgeRefresh = false;
        internal static bool wireFrame = false;
        public Vector3d localCenter = new Vector3d(0, 0, 0);
        public int RenderedAtOrBelowGeneration = 0;
        public static bool fastLoad = false;
        public static bool fastLoadAutoReset = false;
        public virtual bool Draw3D(RenderContext11 renderContext, float opacity, Tile parent)
        {
            RenderedGeneration = CurrentRenderGeneration;
            TilesTouched++;
            AccessCount = TileCache.AccessID++;

            if (errored)
            {
                return false;
            }
            const int xMax = 2;
 
            InViewFrustum = true;


            if (!ReadyToRender)
            {
                if (fastLoad)
                {
                    TextureReady = true;
                    DemReady = true;
                    if (!CreateGeometry(renderContext, false))
                    {
                        TextureReady = false;
                        DemReady = false;
                        TileCache.AddTileToQueue(this);
                        return false;
                    }
                }
                else
                {
                    TileCache.AddTileToQueue(this);
                    return false;
                }
            }
            
          

            var childIndex = 0;

            var yOffset = 0;
            if (dataset.Mercator || dataset.BottomsUp )
            {
                yOffset = 1;
            }
            const int xOffset = 0;

            if (PurgeRefresh)
            {
                PurgeTextureAndDemFiles();
            }
            var savedWorld = renderContext.World;
            var savedView = renderContext.View;
            var usingLocalCenter = false;
            if (localCenter != Vector3d.Empty)
            {
                usingLocalCenter = true;
                var temp = localCenter;
                renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBase * Matrix3d.Translation(-renderContext.CameraPosition);
                renderContext.View = Matrix3d.Translation(renderContext.CameraPosition) * renderContext.ViewBase;
            }

            try
            {
                var anythingToRender = false;
                var childRendered = false;

                for (var y1 = 0; y1 < 2; y1++)
                {
                    for (var x1 = 0; x1 < xMax; x1++)
                    {
                        //  if (level < (demEnabled ? 12 : dataset.Levels))
                        if (level < dataset.Levels)
                        {
                            var child = TileCache.GetTile(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
                            childrenId[childIndex] = child.Key;

                            if (child.IsTileInFrustum(renderContext.Frustum))
                            {
                                InViewFrustum = true;
                                if (child.IsTileBigEnough(renderContext))
                                {
                                    renderPart[childIndex].TargetState = !child.Draw3D(renderContext, opacity, this);
                                    if (renderPart[childIndex].TargetState)
                                    {
                                        childRendered = true;
                                    }
                                }
                                else
                                {
                                    renderPart[childIndex].TargetState = true;
                                }
                            }
                            else
                            {
                                renderPart[childIndex].TargetState = renderPart[childIndex].State = false;
                            }

                            if (renderPart[childIndex].TargetState == true || !blendMode)
                            {
                                renderPart[childIndex].State = renderPart[childIndex].TargetState;
                            }
                        }
                        else
                        {
                            renderPart[childIndex].State = true;
                        }
                        if (renderPart[childIndex].State == true)
                        {
                            anythingToRender = true;
                        }
                        childIndex++;
                    }
                }

                if (childRendered || anythingToRender)
                {
                    RenderedAtOrBelowGeneration = CurrentRenderGeneration;
                    if (parent != null)
                    {
                        parent.RenderedAtOrBelowGeneration = RenderedAtOrBelowGeneration;
                    }
                }

                if (!anythingToRender)
                {
                    return true;
                }


                if (!CreateGeometry(renderContext, true))
                {
                    return false;
                }

                TilesInView++;

                renderContext.MainTexture = wireFrame ? null : texture;
                
                renderContext.SetVertexBuffer(vertexBuffer);

                accomidation = ComputeAccomidation();

                for (var i = 0; i < 4; i++)
                {
                    if (blendMode) //|| ShowElevation == false)
                    {
                        if ((renderPart[i].State && opacity == 1.0) || renderPart[i].TargetState)
                        {
                            renderContext.LocalCenter = localCenter;
                            renderContext.PreDraw();      
                            if (dataset.DataSetType == ImageSetType.Sky)
                            {
                                HDRPixelShader.constants.opacity = renderPart[i].Opacity * opacity;
                                HDRPixelShader.Use(renderContext.devContext);
                            }
                            
                            RenderPart(renderContext, i, renderPart[i].Opacity * opacity, false);
                        }

                    }
                    else
                    {
                        if (renderPart[i].TargetState)
                        {
                            renderContext.LocalCenter = localCenter;
                            renderContext.PreDraw();
                            if (dataset.DataSetType == ImageSetType.Sky)
                            {
                                HDRPixelShader.constants.opacity = opacity;
                                HDRPixelShader.Use(renderContext.devContext);
                            }
                            RenderPart(renderContext, i, opacity, false);
                        }
                    }
                }
            }
            finally
            {
                
                if (usingLocalCenter)
                {
                    renderContext.World = savedWorld;
                    renderContext.View = savedView;
                }
            }
            return true;
        }
        public int RenderedGeneration = 0;
        private void PurgeTextureAndDemFiles()
        {
            if (dataset.Projection != ProjectionType.SkyImage)
            {
                try
                {
                    if (File.Exists(FileName))
                    {
                        File.Delete(FileName);
                    }

                    if (File.Exists(DemFilename))
                    {
                        File.Delete(DemFilename);
                    }
                }
                catch
                {
                }
            }
        }

        public static bool GrayscaleStyle = true;

        public int accomidation = 0;
        public static bool useAccomidation = true;
        private int ComputeAccomidation()
        {
            var accVal = 0;

            if (!useAccomidation)
            {
                return 0;
            }

            


            //Bottom
            var top = TileCache.GetCachedTile(level, x, y + 1, dataset, this);
            if (top == null || top.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 1;
            }

            //right
            var right = TileCache.GetCachedTile(level, x + 1, y, dataset, this);
            if (right == null || right.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 2;
            }

            //top
            var bottom = TileCache.GetCachedTile(level, x, y - 1, dataset, this);
            if (bottom == null || bottom.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 4;
            }
            //left
            var left = TileCache.GetCachedTile(level, x - 1, y, dataset, this);
            if (left == null || left.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 8;
            }

            return accVal;
        }

        public virtual void RenderPart(RenderContext11 renderContext, int part, float opacity, bool combine)
        {
            var partCount = TriangleCount / 4;
            TrianglesRendered += partCount;

            renderContext.SetIndexBuffer(GetIndexBuffer(part, accomidation));
            renderContext.devContext.DrawIndexed(partCount*3, 0, 0);
        }
 
        public int DemGeneration = 0;
        public virtual void CleanUp(bool removeFromParent)
        {

            ReadyToRender = false;
            TextureReady = false;

            DemData = null;
            DemReady = false;
            hdTile = null;

            if (texture != null)
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:Texture Cleanup"); }
                BufferPool11.ReturnTexture(texture);
                texture = null;
            }
            else
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:Cleanup - no Texture"); }
            }

            if (vertexBuffer != null)
            {

                BufferPool11.ReturnPNTX2VertexBuffer(vertexBuffer);
                vertexBuffer = null;
            }

            if (indexBuffer != null)
            {
                foreach (var buffer in indexBuffer)
                {
                    if (buffer != null)
                    {
                        BufferPool11.ReturnShortIndexBuffer(buffer);
                    }
                }
                indexBuffer = null;
            }
        }
        public virtual void CleanUpGeometryOnly()
        {

            if (vertexBuffer != null)
            {
                //vertexBuffer.Created -= OnCreateVertexBuffer();
                vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                vertexBuffer = null;
            }

            if (indexBuffer != null)
            {
                foreach (var buffer in indexBuffer)
                {
                    if (buffer != null)
                    {
                        BufferPool11.ReturnShortIndexBuffer(buffer);
                    }
                }
                indexBuffer = null;
            }
        }

        public virtual void CleanUpGeometryRecursive()
        {
            foreach (var childKey in childrenId)
            {
                var child = TileCache.GetCachedTile(childKey);
                if (child != null)
                {
                    child.CleanUpGeometryRecursive();
                }
            }
            CleanUpGeometryOnly();
        }

        //todoperf this needs to be pointer free
        protected void RemoveChild(Tile child)
        {
            //We are pointer free so this is not needed

        }

        bool blendMode;

        public static int TexturesLoaded = 0;

        public virtual bool PreCreateGeometry(RenderContext11 renderContext)
        {
            return false;
        }

        public virtual bool CreateGeometry(RenderContext11 renderContext, bool uiThread)
        {

            if (uiThread && !ReadyToRender)
            {
                return false;
            }

            

            if (texture == null)
            {
                if (PreCreateGeometry(renderContext))
                {
                    ReadyToRender = true;
                    TextureReady = true;
                    blendMode = false;
                    if (DemEnabled && DemReady && DemData == null)
                    {
                        if (!LoadDemData())
                        {
                            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:CreateGeometry:Loading Dem Failed"); }
                            return false;
                        }
                    }
                    return true;
                }

                blendMode = (dataset.DataSetType == ImageSetType.Sky || dataset.DataSetType == ImageSetType.Panorama) && !Settings.DomeView;
                //blendMode = false;
                if (texture == null)
                {
                    if (TextureReady)
                    {
                        iTileBuildCount++;

                        var localFilename = FileName;
                        if (GrayscaleStyle)
                        {
                            localFilename = UiTools.MakeGrayScaleImage(localFilename);
                        }

                        if (FileExists)
                        {
                            if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:CreateGeometry:Loading Texture"); }
                            texture = BufferPool11.GetTexture(localFilename);
                            if (texture == null)
                            {
                                try
                                {
                                    // bad texture
                                    TextureReady = false;
                                    File.Delete(localFilename);
                                }
                                catch
                                {
                                    if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:CreateGeometry:Loading Texture: Exception"); }
                                    errored = true;
                                }
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }

                        TexturesLoaded++;

                    }
                    else
                    {
                        return false;
                    }
                }


                if (DemEnabled  && DemReady && DemData == null)
                {
                    if (!LoadDemData())
                    {
                        if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:CreateGeometry:Loading Dem Failed"); }
                        return false;
                    }
                }

                if (DemEnabled && DemData == null)
                {
                    return false;
                }             
            }


            if (vertexBuffer == null)
            {
                vertexBuffer = BufferPool11.GetPNTX2VertexBuffer(VertexCount);
                vertexBuffer.ComputeSphereOnUnlock = true;
                indexBuffer = new IndexBuffer11[4];


                if (vertexBuffer != null)
                {
                    OnCreateVertexBuffer(vertexBuffer);

                    sphereRadius = vertexBuffer.SphereRadius;
                    sphereCenter = vertexBuffer.SphereCenter;
                    sphereCenter.Add(localCenter);
                }

            }



            ReadyToRender = true;
            return true;
        }
        protected void CalcSphere()
        {
            var corners = new Vector3d[4];
            corners[0] = TopLeft;
            corners[1] = BottomRight;
            corners[2] = TopRight;
            corners[3] = BottomLeft;

            ConvexHull.FindEnclosingSphere(corners, out sphereCenter, out sphereRadius);
        }

        internal bool isHdTile = false;
        protected int demSize = 33 * 33;
        internal bool demFileExists = false;
        internal DemTile hdTile;
        private bool LoadDemData()
        {
            if (!demFileExists)
            {

                return CreateDemFromParent();                
            }
            var useFloat = false;
        	var fi = new FileInfo(DemFilename);

            if (dataset.Projection == ProjectionType.Mercator)
            {
                using (Stream demStream = File.Open(DemFilename, FileMode.Open))
                {
                    hdTile = DemCodec.Decompress(demStream);
                    demStream.Close();
                }
                   
            }


            if (hdTile != null)
            {
                isHdTile = true;
                DemData = new double[demSize];
                var yh = 0;
                for (var yl = 0; yl < 33;  yl++)
                {
                    var xh = 0;
                    for (var xl = 0; xl < 33; xl++)
                    {
                        var indexI = xl + (32-yl) * 33;
                        DemData[indexI] = hdTile.AltitudeInMeters(yh, xh);
                        demAverage += DemData[indexI];
                       
                        
                        xh += 8;
                    }
                    yh += 8;
                   
                }
                demAverage /= DemData.Length;
               
                return true;
            }

            if (fi.Length != 2178 && fi.Length != 1026 && fi.Length != 4356 && fi.Length != 2052)
            {
                return CreateDemFromParent(); 
            }

            if (fi.Length == 4356 || fi.Length == 2052)
            {
                useFloat = true;
            }

            FileStream fs = null;
             BinaryReader br = null;
            try
            {
                fs = new FileStream(DemFilename, FileMode.Open);
                br = new BinaryReader(fs);


                demAverage = 0;
                DemData = new double[demSize];

                for (var i = 0; i < DemData.Length; i++)
                {
                    if (useFloat)
                    {
                        DemData[i] = br.ReadSingle();
                        if (Double.IsNaN(DemData[i]))
                        {
                            DemData[i] = demAverage;
                        }
                    }
                    else
                    {
                        DemData[i] = br.ReadInt16();
                    }

                    demAverage += DemData[i];
                }

                demAverage /= DemData.Length;

                return true;
            }
            catch
            {
                if (File.Exists(DemFilename))
                {
                    File.Delete(DemFilename);
                }
                DemReady = false;
                return false;
            }
            finally
            {
                if (br != null)
                {
                    br.Close();
                }
                else if (fs != null)
                {
                    fs.Close();
                }
            }

        }

        public virtual bool CreateDemFromParent()
        {
            return false;
        }

        
        public static Viewport Viewport;
        public static Matrix wvp;
        public static int maxLevel = 20;
     
        
        

        virtual public bool IsTileBigEnough(RenderContext11 renderContext)
        {
            if (level > 1)
            {
                // Test for tile scale in view..
                Vector3 topLeftScreen = TopLeft.Vector311;
                topLeftScreen = Vector3.Project(topLeftScreen,Viewport.TopLeftX,Viewport.TopLeftY,Viewport.Width,Viewport.Height,Viewport.MinDepth,Viewport.MaxDepth, wvp);


                Vector3 bottomRightScreen = BottomRight.Vector311;
                bottomRightScreen = Vector3.Project(bottomRightScreen, Viewport.TopLeftX, Viewport.TopLeftY, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);

                Vector3 topRightScreen = TopRight.Vector311;
                topRightScreen = Vector3.Project(topRightScreen, Viewport.TopLeftX, Viewport.TopLeftY, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);
    
                Vector3 bottomLeftScreen = BottomLeft.Vector311;
                bottomLeftScreen = Vector3.Project(bottomLeftScreen, Viewport.TopLeftX, Viewport.TopLeftY, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);

                var top = topLeftScreen;
                top = Vector3.Subtract(top, topRightScreen);
                var topLength = top.Length();

                var bottom = bottomLeftScreen;
                bottom = Vector3.Subtract(bottom, bottomRightScreen);
                var bottomLength = bottom.Length();

                var left = bottomLeftScreen;
                left = Vector3.Subtract(left, topLeftScreen);
                var leftLength = left.Length();

                var right = bottomRightScreen;
                right = Vector3.Subtract(right, topRightScreen);
                var rightLength = right.Length();
                

                var lengthMax = Math.Max(Math.Max(rightLength, leftLength), Math.Max(bottomLength, topLength));
                if (lengthMax < (400 - ((Earth3d.MainWindow.dumpFrameParams.Dome && SpaceTimeController.FrameDumping) ? -200 : imageQuality))) // was 220
                {
                    return false;

                }
                deepestLevel = level > deepestLevel ? level : deepestLevel;
            }
            return true;
        }
        
        public static int meshComplexity = 50;
        public static int imageQuality = 50;
        public static int lastDeepestLevel = 0;
        public static int deepestLevel = 0;
        virtual public bool IsTileInFrustum(PlaneD[]frustum)
        {
            InViewFrustum = false;
            var center = sphereCenter;

            if (Level < 2 && (dataset.Projection == ProjectionType.Mercator || dataset.Projection == ProjectionType.Toast))
            {
                return true;
            }

            var centerV4 = new Vector4d(center.X , center.Y , center.Z , 1f);
            var length = new Vector3d(sphereRadius, 0, 0);

            var rad = length.Length();
            for (var i = 0; i < 6; i++)
            {
                if (frustum[i].Dot(centerV4) + rad < 0)
                {
                    return false;
                }
            }
            InViewFrustum = true;

            return true;  
        }

     
        protected double sphereRadius;

        public double SphereRadius
        {
            get { return sphereRadius; }
        }

        protected Vector3d localOrigin;

        protected Vector3d sphereCenter;

        public Vector3d SphereCenter
        {
            get { return sphereCenter; }
        }

        private double demScaleFactor = 6371000;

        internal double DemScaleFactor
        {
            get { return demScaleFactor; }
            set 
            {
                demScaleFactor = value / Properties.Settings.Default.TerrainScaling;
            }
        }
                                                    

        protected const double RC = (Math.PI / 180.0);
        protected float radius = 1;
        protected Vector3d GeoTo3d(double lat, double lng, bool useLocalCenter)
        {

            if (dataset.DataSetType == ImageSetType.Panorama)
            {
                var retVal = new Vector3d(-(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));
                if (useLocalCenter)
                {
                    retVal.Subtract(localCenter);
                }
                return retVal;

            }
            else
            {
                var retVal = new Vector3d((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));
                if (useLocalCenter)
                {
                    retVal.Subtract(localCenter);
                }
                return retVal;
            }

        }
        protected Vector3d GeoTo3dWithAltitude(double lat, double lng, bool useLocalCenter)
        {
            lat = Math.Max(Math.Min(90, lat), -90);
            lng = Math.Max(Math.Min(180, lng), -180);
            if (!DemEnabled || DemData == null)
            {
                return GeoTo3d(lat, lng, useLocalCenter);
            }

            var altitude = DemData[demIndex];
            var retVal = GeoTo3dWithAltitude(lat, lng, altitude, useLocalCenter);
            return retVal;
        }

        public Vector3d GeoTo3dWithAltitude(double lat, double lng, double altitude, bool useLocalCenter)
        {

            var radius = 1 + (altitude / DemScaleFactor);
            var retVal = (new Vector3d((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius)));
            if (useLocalCenter)
            {
                retVal.Subtract(localCenter);
            }
            return retVal;
        }

        static protected int SubDivisions
        {
            get { return 32; /*DemEnabled ? 32 : 16; */ }
        }
  
        protected int TriangleCount = 0;

        public virtual void OnCreateVertexBuffer(VertexBuffer11 vb)
        {
 
        }

        public int RequestHits;
        public bool RequestPending = false;
        public bool errored = false;
        protected IImageSet dataset;
        public IImageSet Dataset
        {
            get
            {
                return dataset;
            }
            set
            {
                dataset = value;
            }
        }


        long key;
        public long Key
        {
            get
            {
                if (key == 0)
                {
                    key = ImageSetHelper.GetTileKey(dataset, level, x, y);
                }

                return key;
            }

        }
        
        string directory;
        
        public String Directory
        {
            get
            {
                if (directory == null)
                {
                    if (dataset.Projection == ProjectionType.SkyImage && dataset.Url.EndsWith("/screenshot.png"))
                    {
                        Volitile = true;
                        directory = Properties.Settings.Default.CahceDirectory + @"Imagery\";
                    }
                    else
                    {
                        directory = GetDirectory(dataset, level, x, y);
                    }
                }
                return directory;
            }

        }

        
        internal static string GetDirectory(IImageSet dataset, int level, int x, int y)
        {
            var sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"Imagery\");
            sb.Append(dataset.ImageSetID.ToString());
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());

            return sb.ToString();
        }

        public bool Volitile = false;
        public String filename;
        public String FileName
        {
            get
            {
                if (filename == null)
                {

                    if (dataset.Projection == ProjectionType.SkyImage && dataset.Url.EndsWith("/screenshot.png"))
                    {
                        Volitile = true;
                        filename = Properties.Settings.Default.CahceDirectory + @"Imagery\screenshop.png";
                    }
                    else
                    {
                        filename = GetFilename(dataset, level, x, y);
                    }
                    return filename;
                }
                return filename;

            }

        }

        internal static string GetFilename(IImageSet dataset, int level, int x, int y)
        {
            var extention = dataset.Extension.StartsWith(".") ? dataset.Extension : "." + dataset.Extension;
            var sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"Imagery\");
            sb.Append(dataset.ImageSetID.ToString());
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append("_");
            sb.Append(x.ToString());
            sb.Append(extention);
            return sb.ToString();
        }

        // URL parameters
        //{0} ImageSetID
        //{1} level
        //{2} x tile id
        //{3} y tile id
        //{4} quadtree address (VE style)
        //{5} quadtree address (Google maps style)
        //{6} top left corner RA
        //{7} top left corner Dec
        //{8} bottom right corner RA
        //{9} bottom right corner dec
        //{10} bottom left corner RA
        //{11} bottom left corner dec
        //{12} top right corner RA
        //{13} top right corner dec

        //{X} - Tile X value
        //{Y} - Tile Y value
        //{L} - Tile Level
        //{Q} - Quad Key ID
        //{S} - Last Digit of Quadkey
        //
        string url;
        public String URL
        {
            get
            {
                if (url == null)
                {
                    url = GetUrl(dataset, level, x, y);
                    return url;
                }
                return url;
            }
        }
        internal static string GetWmsURL(IImageSet dataset, int level, int x, int y)
        {

            var tileDegrees = dataset.BaseTileDegrees / (Math.Pow(2, level));

            var latMin = ( (y * tileDegrees)-90 );
            var latMax = ( ((y + 1) * tileDegrees) -90 );
            var lngMin = ((x * tileDegrees) - 180.0);
            var lngMax = (((x + 1) * tileDegrees) - 180.0);
            var returnUrl = dataset.Url;

            returnUrl = returnUrl.Replace("{latMin}", latMin.ToString());
            returnUrl = returnUrl.Replace("{latMax}", latMax.ToString());
            returnUrl = returnUrl.Replace("{lngMin}", lngMin.ToString());
            returnUrl = returnUrl.Replace("{lngMax}", lngMax.ToString());

            return returnUrl;
        }

        internal static string GetUrl(IImageSet dataset, int level, int x, int y)
        {
            if (dataset.Url.Contains("{latMin}"))
            {
                return GetWmsURL(dataset, level, x, y);
            }

            if (dataset.Url.Contains("{1}"))
            {
                // Old style URL
                if (dataset.Projection == ProjectionType.Mercator && !string.IsNullOrEmpty(dataset.QuadTreeTileMap))
                {
                    return String.Format(dataset.Url, GetServerID(x, y), GetTileID(dataset, level, x, y));
                }
                return String.Format(dataset.Url, dataset.ImageSetID, level, x, y);
            }


            var returnUrl = dataset.Url;

            returnUrl = returnUrl.Replace("{X}", x.ToString());
            returnUrl = returnUrl.Replace("{Y}", y.ToString());
            returnUrl = returnUrl.Replace("{L}", level.ToString());
            var hash = 0;
            if (returnUrl.Contains("{S:0}"))
            {
                hash = 0;
                returnUrl = returnUrl.Replace("{S:0}", "{S}");
            }
            if (returnUrl.Contains("{S:1}"))
            {
                hash = 1;
                returnUrl = returnUrl.Replace("{S:1}", "{S}");
            }
            if (returnUrl.Contains("{S:2}"))
            {
                hash = 2;
                returnUrl = returnUrl.Replace("{S:2}", "{S}");
            }
            if (returnUrl.Contains("{S:3}"))
            {
                hash = 3;
                returnUrl = returnUrl.Replace("{S:3}", "{S}");
            }

            if (returnUrl.Contains("a{S}"))
            {
                returnUrl = returnUrl.Replace("a{S}", "r{S}");
            }

            if (returnUrl.Contains("h{S}"))
            {
                returnUrl = returnUrl.Replace("h{S}", "r{S}");
            }


            if (returnUrl.Contains("http://r{S}.ortho.tiles.virtualearth.net"))
            {
                returnUrl = returnUrl.Replace("http://r{S}.ortho.tiles.virtualearth.net", "http://ecn.t{S}.tiles.virtualearth.net");
            }

            var id = GetTileID(dataset, level, x, y);
            var server = "";

            if (id.Length == 0)
            {
                server = hash.ToString();
            }
            else
            {
                server = id[id.Length - 1].ToString();
            }

            //if (returnUrl == "http://r{S}.ortho.tiles.virtualearth.net/tiles/wtl00{Q}?g=121&band=wwt_rgb" && id.Length > 7 && (id.StartsWith("2") || id.StartsWith("3")))
            //{
            //    returnUrl = "http://r{S}.ortho.tiles.virtualearth.net/tiles/wtl00{Q}?g=120&band=wwt_jpg";
            //}


            returnUrl = returnUrl.Replace("{Q}", id);

            returnUrl = returnUrl.Replace("{S}", server);

            return returnUrl;
        }

        public static int GetServerID(int tileX, int tileY)
        {
            var server = (tileX & 1) + ((tileY & 1) << 1);

            return (server);
        }

        string tileId;
        public string GetTileID()
        {
            return tileId ?? (tileId = GetTileID(dataset, level, x, y));
        }

        public static string GetTileID(IImageSet dataset, int tileLevel, int tileX, int tileY)
        {
            var netLevel = tileLevel;
            var netX = tileX;
            var netY = tileY;
            var tileId = "";
            if (dataset.Projection == ProjectionType.Equirectangular)
            {
                netLevel++;
            }

            var tileMap = dataset.QuadTreeTileMap;

            if (!string.IsNullOrEmpty(tileMap))
            {
                var sb = new StringBuilder();

                for (var i = netLevel; i > 0; --i)
                {
                    var mask = 1 << (i - 1);
                    var val = 0;

                    if ((netX & mask) != 0)
                        val = 1;

                    if ((netY & mask) != 0)
                        val += 2;

                    sb.Append(tileMap[val]);

                }
                tileId = sb.ToString();
                return tileId;
            }
            tileId = "0";
            return tileId;
        }

        protected int VertexCount { get; set; }

        readonly BlendState[] renderPart;
        bool demEnabled;
        private const bool demInitialized = false;

        public bool DemEnabled
        {
            get 
            {
                if (!demInitialized)
                {
                    demEnabled = dataset.ElevationModel && Settings.Active.ShowElevationModel;
                }

                return demEnabled; 
            }
            set { demEnabled = value; }
        }
        protected bool insideOut = false;

        protected Tile()
        {
            
            renderPart = new BlendState[4];
            for (var i = 0; i < 4; i++ )
            {
                renderPart[i] = new BlendState(false, 500);
            }

        }

        public virtual string DemDirectory
        {
            get
            {
                if (dataset.DemUrl == null)
                {
                    dataset.DemUrl = "";
                }
                return GetDemDirectory(dataset, level, x, y);
            }
        }

        internal static string GetDemDirectory(IImageSet dataset, int level, int x, int y)
        {
            var sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"dem\");
            sb.Append(Math.Abs(dataset.DemUrl.GetHashCode32()).ToString(CultureInfo.InvariantCulture));
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());       
            return sb.ToString();
        }

        public virtual string DemFilename
        {
            get
            {
                if (string.IsNullOrEmpty(dataset.DemUrl))
                {
                    return "";
                }
                return GetDemFilename(dataset, level, x, y);
            }
        }

        internal static string GetDemFilename(IImageSet dataset, int level, int x, int y)
        {
            var sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"dem\");
            sb.Append(Math.Abs(dataset.DemUrl.GetHashCode32()).ToString());
            sb.Append(@"\");
            sb.Append(level.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append(@"\");
            sb.Append(y.ToString());
            sb.Append("_");
            sb.Append(x.ToString());
            sb.Append(".dem");
            return sb.ToString();
        }

        public virtual string DemURL
        {
            get
            {
                return GetDemUrl(dataset, level, x, y);
            }
        }

        internal static string GetDemUrl(IImageSet dataset, int level, int x, int y)
        {
            if (dataset.Projection == ProjectionType.Mercator)
            {
                var baseUrl = "http://cdn.worldwidetelescope.org/wwtweb/demtile.aspx?q={0},{1},{2},M";
                if (!String.IsNullOrEmpty(dataset.DemUrl))
                {
                    baseUrl = dataset.DemUrl;
                }

                //return string.Format(baseUrl, level.ToString(), x.ToString(), y.ToString());
            }


            if (dataset.DemUrl.Contains("{1}"))
            {
                return String.Format(dataset.DemUrl + "&new", level, x, y);
            }

            var returnUrl = dataset.DemUrl;

            returnUrl = returnUrl.Replace("{X}", x.ToString());
            returnUrl = returnUrl.Replace("{Y}", y.ToString());
            returnUrl = returnUrl.Replace("{L}", level.ToString());
            var hash = 0;
            if (returnUrl.Contains("{S:0}"))
            {
                hash = 0;
                returnUrl = returnUrl.Replace("{S:0}", "{S}");
            }
            if (returnUrl.Contains("{S:1}"))
            {
                hash = 1;
                returnUrl = returnUrl.Replace("{S:1}", "{S}");
            }
            if (returnUrl.Contains("{S:2}"))
            {
                hash = 2;
                returnUrl = returnUrl.Replace("{S:2}", "{S}");
            }
            if (returnUrl.Contains("{S:3}"))
            {
                hash = 3;
                returnUrl = returnUrl.Replace("{S:3}", "{S}");
            }


            var id = GetTileID(dataset, level, x, y);
            var server = "";

            if (id.Length == 0)
            {
                server = hash.ToString();
            }
            else
            {
                server = id[id.Length - 1].ToString();
            }


            returnUrl = returnUrl.Replace("{Q}", id);

            returnUrl = returnUrl.Replace("{S}", server);

            return returnUrl;
        }

        int MAXITER = 300;
        private int MandPoint(double x, double y)
        {
            var looper = 0;
            double x1 = 0;
            double y1 = 0;
            double xx;
            while (looper < MAXITER && ((x1 * x1) + (y1 * y1)) < 4)
            {
                looper++;
                xx = (x1 * x1) - (y1 * y1) + x;
                y1 = 2 * x1 * y1 + y;
                x1 = xx;
            }
            return looper;
        }

        const int mandelWidth = 256;
        public void ComputeMandel()
        {
            unsafe
            {
                var filename = FileName;
                var path = Directory;
                Bitmap b = null;
                PixelData pixel;
                pixel.alpha = 255;
                MAXITER = 100 + level * 38;


                var tileWidth = (4 / (Math.Pow(2, level)));
                var Sy = (this.y * tileWidth) - 2;
                var Fy = Sy + tileWidth;
                var Sx = (this.x * tileWidth) - 4;
                var Fx = Sx + tileWidth;

                b = new Bitmap(mandelWidth, mandelWidth);
                var fb = new FastBitmap(b);
                fb.LockBitmap();
                double x, y, xmin, xmax, ymin, ymax = 0.0;
                int looper, s, z = 0;
                double intigralX, intigralY = 0.0;
                xmin = Sx;
                ymin = Sy;
                xmax = Fx;
                ymax = Fy;
                intigralX = (xmax - xmin) / mandelWidth;
                intigralY = (ymax - ymin) / mandelWidth;
                x = xmin;

                var computeAll = true;

                if (computeAll)
                {
                    for (s = 0; s < mandelWidth; s++)
                    {
                        y = ymin;
                        for (z = 0; z < mandelWidth; z++)
                        {

                            looper = MandPoint(x, y);
                            var col = (looper == MAXITER) ? Color.Black : ColorTable[looper % 1011];
                            pixel.red = col.R;
                            pixel.green = col.G;
                            pixel.blue = col.B;
                            //b.SetPixel(s, z, col);
                            *fb[s, z] = pixel;
                            y += intigralY;
                        }
                        x += intigralX;
                    }
                }

                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                fb.UnlockBitmap();
                b.Save(filename);
                b.Dispose();
                GC.SuppressFinalize(b);
            }
        }



        #region
        static readonly Color[] ColorTable = {
										Color.FromArgb(255,0,0),
										Color.FromArgb(255,4,0),
										Color.FromArgb(255,8,0),
										Color.FromArgb(255,12,0),
										Color.FromArgb(255,16,0),
										Color.FromArgb(255,20,0),
										Color.FromArgb(255,24,0),
										Color.FromArgb(255,28,0),
										Color.FromArgb(255,31,0),
										Color.FromArgb(255,35,0),
										Color.FromArgb(255,39,0),
										Color.FromArgb(255,43,0),
										Color.FromArgb(255,47,0),
										Color.FromArgb(255,51,0),
										Color.FromArgb(255,55,0),
										Color.FromArgb(255,59,0),
										Color.FromArgb(255,63,0),
										Color.FromArgb(255,67,0),
										Color.FromArgb(255,71,0),
										Color.FromArgb(255,75,0),
										Color.FromArgb(255,79,0),
										Color.FromArgb(255,83,0),
										Color.FromArgb(255,87,0),
										Color.FromArgb(255,91,0),
										Color.FromArgb(255,94,0),
										Color.FromArgb(255,98,0),
										Color.FromArgb(255,102,0),
										Color.FromArgb(255,106,0),
										Color.FromArgb(255,110,0),
										Color.FromArgb(255,114,0),
										Color.FromArgb(255,118,0),
										Color.FromArgb(255,122,0),
										Color.FromArgb(255,126,0),
										Color.FromArgb(255,130,0),
										Color.FromArgb(255,133,0),
										Color.FromArgb(255,137,0),
										Color.FromArgb(255,141,0),
										Color.FromArgb(255,145,0),
										Color.FromArgb(255,149,0),
										Color.FromArgb(255,153,0),
										Color.FromArgb(255,157,0),
										Color.FromArgb(255,161,0),
										Color.FromArgb(255,255,0),
										Color.FromArgb(255,167,0),
										Color.FromArgb(255,170,0),
										Color.FromArgb(255,172,0),
										Color.FromArgb(255,174,0),
										Color.FromArgb(255,176,0),
										Color.FromArgb(255,178,0),
										Color.FromArgb(255,180,0),
										Color.FromArgb(255,182,0),
										Color.FromArgb(255,185,0),
										Color.FromArgb(255,187,0),
										Color.FromArgb(255,189,0),
										Color.FromArgb(255,191,0),
										Color.FromArgb(255,193,0),
										Color.FromArgb(255,195,0),
										Color.FromArgb(255,197,0),
										Color.FromArgb(255,199,0),
										Color.FromArgb(255,201,0),
										Color.FromArgb(255,203,0),
										Color.FromArgb(255,205,0),
										Color.FromArgb(255,208,0),
										Color.FromArgb(255,210,0),
										Color.FromArgb(255,212,0),
										Color.FromArgb(255,215,0),
										Color.FromArgb(255,217,0),
										Color.FromArgb(255,219,0),
										Color.FromArgb(255,221,0),
										Color.FromArgb(255,223,0),
										Color.FromArgb(255,225,0),
										Color.FromArgb(255,227,0),
										Color.FromArgb(255,230,0),
										Color.FromArgb(255,232,0),
										Color.FromArgb(255,234,0),
										Color.FromArgb(255,236,0),
										Color.FromArgb(255,238,0),
										Color.FromArgb(255,240,0),
										Color.FromArgb(255,242,0),
										Color.FromArgb(255,244,0),
										Color.FromArgb(255,246,0),
										Color.FromArgb(255,248,0),
										Color.FromArgb(255,250,0),
										Color.FromArgb(255,253,0),
										Color.FromArgb(0,128,0),
										Color.FromArgb(249,252,0),
										Color.FromArgb(243,249,0),
										Color.FromArgb(237,246,0),
										Color.FromArgb(231,243,0),
										Color.FromArgb(225,240,0),
										Color.FromArgb(218,237,0),
										Color.FromArgb(212,234,0),
										Color.FromArgb(206,231,0),
										Color.FromArgb(200,228,0),
										Color.FromArgb(194,225,0),
										Color.FromArgb(188,222,0),
										Color.FromArgb(182,218,0),
										Color.FromArgb(176,215,0),
										Color.FromArgb(170,212,0),
										Color.FromArgb(164,209,0),
										Color.FromArgb(158,206,0),
										Color.FromArgb(151,203,0),
										Color.FromArgb(145,200,0),
										Color.FromArgb(139,197,0),
										Color.FromArgb(134,195,0),
										Color.FromArgb(128,192,0),
										Color.FromArgb(122,189,0),
										Color.FromArgb(116,186,0),
										Color.FromArgb(110,183,0),
										Color.FromArgb(104,180,0),
										Color.FromArgb(98,177,0),
										Color.FromArgb(91,174,0),
										Color.FromArgb(85,171,0),
										Color.FromArgb(79,168,0),
										Color.FromArgb(73,165,0),
										Color.FromArgb(67,162,0),
										Color.FromArgb(61,159,0),
										Color.FromArgb(55,155,0),
										Color.FromArgb(49,152,0),
										Color.FromArgb(43,149,0),
										Color.FromArgb(37,146,0),
										Color.FromArgb(31,143,0),
										Color.FromArgb(24,140,0),
										Color.FromArgb(18,137,0),
										Color.FromArgb(12,134,0),
										Color.FromArgb(6,131,0),
										Color.FromArgb(0,0,255),
										Color.FromArgb(0,125,6),
										Color.FromArgb(0,122,12),
										Color.FromArgb(0,119,18),
										Color.FromArgb(0,116,24),
										Color.FromArgb(0,113,30),
										Color.FromArgb(0,110,37),
										Color.FromArgb(0,107,43),
										Color.FromArgb(0,104,49),
										Color.FromArgb(0,101,55),
										Color.FromArgb(0,98,61),
										Color.FromArgb(0,95,67),
										Color.FromArgb(0,91,73),
										Color.FromArgb(0,88,79),
										Color.FromArgb(0,85,85),
										Color.FromArgb(0,82,91),
										Color.FromArgb(0,79,98),
										Color.FromArgb(0,76,104),
										Color.FromArgb(0,73,110),
										Color.FromArgb(0,70,116),
										Color.FromArgb(0,67,122),
										Color.FromArgb(0,64,128),
										Color.FromArgb(0,61,134),
										Color.FromArgb(0,58,139),
										Color.FromArgb(0,55,145),
										Color.FromArgb(0,52,151),
										Color.FromArgb(0,49,157),
										Color.FromArgb(0,46,164),
										Color.FromArgb(0,43,170),
										Color.FromArgb(0,40,176),
										Color.FromArgb(0,37,182),
										Color.FromArgb(0,34,188),
										Color.FromArgb(0,31,194),
										Color.FromArgb(0,27,200),
										Color.FromArgb(0,24,206),
										Color.FromArgb(0,21,212),
										Color.FromArgb(0,18,218),
										Color.FromArgb(0,15,225),
										Color.FromArgb(0,12,231),
										Color.FromArgb(0,9,237),
										Color.FromArgb(0,6,243),
										Color.FromArgb(0,3,249),
										Color.FromArgb(75,0,130),
										Color.FromArgb(2,0,252),
										Color.FromArgb(4,0,249),
										Color.FromArgb(6,0,246),
										Color.FromArgb(7,0,243),
										Color.FromArgb(9,0,240),
										Color.FromArgb(10,0,237),
										Color.FromArgb(12,0,234),
										Color.FromArgb(14,0,232),
										Color.FromArgb(16,0,229),
										Color.FromArgb(18,0,226),
										Color.FromArgb(20,0,223),
										Color.FromArgb(21,0,219),
										Color.FromArgb(23,0,216),
										Color.FromArgb(25,0,213),
										Color.FromArgb(27,0,210),
										Color.FromArgb(28,0,207),
										Color.FromArgb(30,0,204),
										Color.FromArgb(32,0,201),
										Color.FromArgb(34,0,198),
										Color.FromArgb(36,0,196),
										Color.FromArgb(38,0,193),
										Color.FromArgb(40,0,190),
										Color.FromArgb(41,0,187),
										Color.FromArgb(43,0,184),
										Color.FromArgb(45,0,181),
										Color.FromArgb(47,0,178),
										Color.FromArgb(48,0,175),
										Color.FromArgb(50,0,172),
										Color.FromArgb(52,0,169),
										Color.FromArgb(54,0,166),
										Color.FromArgb(55,0,163),
										Color.FromArgb(57,0,160),
										Color.FromArgb(59,0,156),
										Color.FromArgb(61,0,153),
										Color.FromArgb(63,0,151),
										Color.FromArgb(65,0,148),
										Color.FromArgb(66,0,145),
										Color.FromArgb(68,0,142),
										Color.FromArgb(69,0,139),
										Color.FromArgb(71,0,136),
										Color.FromArgb(73,0,133),
										Color.FromArgb(238,130,238),
										Color.FromArgb(79,3,133),
										Color.FromArgb(83,6,135),
										Color.FromArgb(86,9,138),
										Color.FromArgb(90,12,141),
										Color.FromArgb(94,15,143),
										Color.FromArgb(98,18,146),
										Color.FromArgb(102,21,148),
										Color.FromArgb(106,24,150),
										Color.FromArgb(110,28,153),
										Color.FromArgb(114,31,156),
										Color.FromArgb(118,35,158),
										Color.FromArgb(122,38,161),
										Color.FromArgb(125,41,164),
										Color.FromArgb(129,44,166),
										Color.FromArgb(133,47,169),
										Color.FromArgb(137,50,172),
										Color.FromArgb(141,53,174),
										Color.FromArgb(145,56,176),
										Color.FromArgb(149,59,179),
										Color.FromArgb(153,62,181),
										Color.FromArgb(157,65,184),
										Color.FromArgb(161,68,187),
										Color.FromArgb(165,71,189),
										Color.FromArgb(168,74,192),
										Color.FromArgb(172,77,195),
										Color.FromArgb(176,80,197),
										Color.FromArgb(180,83,200),
										Color.FromArgb(184,86,202),
										Color.FromArgb(187,89,204),
										Color.FromArgb(191,93,207),
										Color.FromArgb(195,96,210),
										Color.FromArgb(199,100,212),
										Color.FromArgb(203,103,215),
										Color.FromArgb(206,106,218),
										Color.FromArgb(211,109,220),
										Color.FromArgb(215,112,223),
										Color.FromArgb(219,115,226),
										Color.FromArgb(223,118,228),
										Color.FromArgb(227,121,230),
										Color.FromArgb(230,124,233),
										Color.FromArgb(234,127,235),
										Color.FromArgb(238,130,238),
										Color.FromArgb(234,127,235),
										Color.FromArgb(230,124,233),
										Color.FromArgb(227,121,230),
										Color.FromArgb(223,118,228),
										Color.FromArgb(219,115,226),
										Color.FromArgb(215,112,223),
										Color.FromArgb(211,109,220),
										Color.FromArgb(206,106,218),
										Color.FromArgb(203,103,215),
										Color.FromArgb(199,100,212),
										Color.FromArgb(195,96,210),
										Color.FromArgb(191,93,207),
										Color.FromArgb(187,89,204),
										Color.FromArgb(184,86,202),
										Color.FromArgb(180,83,200),
										Color.FromArgb(176,80,197),
										Color.FromArgb(172,77,195),
										Color.FromArgb(168,74,192),
										Color.FromArgb(165,71,189),
										Color.FromArgb(161,68,187),
										Color.FromArgb(157,65,184),
										Color.FromArgb(153,62,181),
										Color.FromArgb(149,59,179),
										Color.FromArgb(145,56,176),
										Color.FromArgb(141,53,174),
										Color.FromArgb(137,50,172),
										Color.FromArgb(133,47,169),
										Color.FromArgb(129,44,166),
										Color.FromArgb(125,41,164),
										Color.FromArgb(122,38,161),
										Color.FromArgb(118,35,158),
										Color.FromArgb(114,31,156),
										Color.FromArgb(110,28,153),
										Color.FromArgb(106,24,150),
										Color.FromArgb(102,21,148),
										Color.FromArgb(98,18,146),
										Color.FromArgb(94,15,143),
										Color.FromArgb(90,12,141),
										Color.FromArgb(86,9,138),
										Color.FromArgb(83,6,135),
										Color.FromArgb(79,3,133),
										Color.FromArgb(238,130,238),
										Color.FromArgb(73,0,133),
										Color.FromArgb(71,0,136),
										Color.FromArgb(69,0,139),
										Color.FromArgb(68,0,142),
										Color.FromArgb(66,0,145),
										Color.FromArgb(65,0,148),
										Color.FromArgb(63,0,151),
										Color.FromArgb(61,0,153),
										Color.FromArgb(59,0,156),
										Color.FromArgb(57,0,160),
										Color.FromArgb(55,0,163),
										Color.FromArgb(54,0,166),
										Color.FromArgb(52,0,169),
										Color.FromArgb(50,0,172),
										Color.FromArgb(48,0,175),
										Color.FromArgb(47,0,178),
										Color.FromArgb(45,0,181),
										Color.FromArgb(43,0,184),
										Color.FromArgb(41,0,187),
										Color.FromArgb(40,0,190),
										Color.FromArgb(38,0,193),
										Color.FromArgb(36,0,196),
										Color.FromArgb(34,0,198),
										Color.FromArgb(32,0,201),
										Color.FromArgb(30,0,204),
										Color.FromArgb(28,0,207),
										Color.FromArgb(27,0,210),
										Color.FromArgb(25,0,213),
										Color.FromArgb(23,0,216),
										Color.FromArgb(21,0,219),
										Color.FromArgb(20,0,223),
										Color.FromArgb(18,0,226),
										Color.FromArgb(16,0,229),
										Color.FromArgb(14,0,232),
										Color.FromArgb(12,0,234),
										Color.FromArgb(10,0,237),
										Color.FromArgb(9,0,240),
										Color.FromArgb(7,0,243),
										Color.FromArgb(6,0,246),
										Color.FromArgb(4,0,249),
										Color.FromArgb(2,0,252),
										Color.FromArgb(75,0,130),
										Color.FromArgb(0,3,249),
										Color.FromArgb(0,6,243),
										Color.FromArgb(0,9,237),
										Color.FromArgb(0,12,231),
										Color.FromArgb(0,15,225),
										Color.FromArgb(0,18,218),
										Color.FromArgb(0,21,212),
										Color.FromArgb(0,24,206),
										Color.FromArgb(0,27,200),
										Color.FromArgb(0,31,194),
										Color.FromArgb(0,34,188),
										Color.FromArgb(0,37,182),
										Color.FromArgb(0,40,176),
										Color.FromArgb(0,43,170),
										Color.FromArgb(0,46,164),
										Color.FromArgb(0,49,157),
										Color.FromArgb(0,52,151),
										Color.FromArgb(0,55,145),
										Color.FromArgb(0,58,139),
										Color.FromArgb(0,61,134),
										Color.FromArgb(0,64,128),
										Color.FromArgb(0,67,122),
										Color.FromArgb(0,70,116),
										Color.FromArgb(0,73,110),
										Color.FromArgb(0,76,104),
										Color.FromArgb(0,79,98),
										Color.FromArgb(0,82,91),
										Color.FromArgb(0,85,85),
										Color.FromArgb(0,88,79),
										Color.FromArgb(0,91,73),
										Color.FromArgb(0,95,67),
										Color.FromArgb(0,98,61),
										Color.FromArgb(0,101,55),
										Color.FromArgb(0,104,49),
										Color.FromArgb(0,107,43),
										Color.FromArgb(0,110,37),
										Color.FromArgb(0,113,30),
										Color.FromArgb(0,116,24),
										Color.FromArgb(0,119,18),
										Color.FromArgb(0,122,12),
										Color.FromArgb(0,125,6),
										Color.FromArgb(0,0,255),
										Color.FromArgb(6,131,0),
										Color.FromArgb(12,134,0),
										Color.FromArgb(18,137,0),
										Color.FromArgb(24,140,0),
										Color.FromArgb(31,143,0),
										Color.FromArgb(37,146,0),
										Color.FromArgb(43,149,0),
										Color.FromArgb(49,152,0),
										Color.FromArgb(55,155,0),
										Color.FromArgb(61,159,0),
										Color.FromArgb(67,162,0),
										Color.FromArgb(73,165,0),
										Color.FromArgb(79,168,0),
										Color.FromArgb(85,171,0),
										Color.FromArgb(91,174,0),
										Color.FromArgb(98,177,0),
										Color.FromArgb(104,180,0),
										Color.FromArgb(110,183,0),
										Color.FromArgb(116,186,0),
										Color.FromArgb(122,189,0),
										Color.FromArgb(128,192,0),
										Color.FromArgb(134,195,0),
										Color.FromArgb(139,197,0),
										Color.FromArgb(145,200,0),
										Color.FromArgb(151,203,0),
										Color.FromArgb(158,206,0),
										Color.FromArgb(164,209,0),
										Color.FromArgb(170,212,0),
										Color.FromArgb(176,215,0),
										Color.FromArgb(182,218,0),
										Color.FromArgb(188,222,0),
										Color.FromArgb(194,225,0),
										Color.FromArgb(200,228,0),
										Color.FromArgb(206,231,0),
										Color.FromArgb(212,234,0),
										Color.FromArgb(218,237,0),
										Color.FromArgb(225,240,0),
										Color.FromArgb(231,243,0),
										Color.FromArgb(237,246,0),
										Color.FromArgb(243,249,0),
										Color.FromArgb(249,252,0),
										Color.FromArgb(0,128,0),
										Color.FromArgb(255,253,0),
										Color.FromArgb(255,250,0),
										Color.FromArgb(255,248,0),
										Color.FromArgb(255,246,0),
										Color.FromArgb(255,244,0),
										Color.FromArgb(255,242,0),
										Color.FromArgb(255,240,0),
										Color.FromArgb(255,238,0),
										Color.FromArgb(255,236,0),
										Color.FromArgb(255,234,0),
										Color.FromArgb(255,232,0),
										Color.FromArgb(255,230,0),
										Color.FromArgb(255,227,0),
										Color.FromArgb(255,225,0),
										Color.FromArgb(255,223,0),
										Color.FromArgb(255,221,0),
										Color.FromArgb(255,219,0),
										Color.FromArgb(255,217,0),
										Color.FromArgb(255,215,0),
										Color.FromArgb(255,212,0),
										Color.FromArgb(255,210,0),
										Color.FromArgb(255,208,0),
										Color.FromArgb(255,205,0),
										Color.FromArgb(255,203,0),
										Color.FromArgb(255,201,0),
										Color.FromArgb(255,199,0),
										Color.FromArgb(255,197,0),
										Color.FromArgb(255,195,0),
										Color.FromArgb(255,193,0),
										Color.FromArgb(255,191,0),
										Color.FromArgb(255,189,0),
										Color.FromArgb(255,187,0),
										Color.FromArgb(255,185,0),
										Color.FromArgb(255,182,0),
										Color.FromArgb(255,180,0),
										Color.FromArgb(255,178,0),
										Color.FromArgb(255,176,0),
										Color.FromArgb(255,174,0),
										Color.FromArgb(255,172,0),
										Color.FromArgb(255,170,0),
										Color.FromArgb(255,167,0),
										Color.FromArgb(255,255,0),
										Color.FromArgb(255,161,0),
										Color.FromArgb(255,157,0),
										Color.FromArgb(255,153,0),
										Color.FromArgb(255,149,0),
										Color.FromArgb(255,145,0),
										Color.FromArgb(255,141,0),
										Color.FromArgb(255,137,0),
										Color.FromArgb(255,133,0),
										Color.FromArgb(255,130,0),
										Color.FromArgb(255,126,0),
										Color.FromArgb(255,122,0),
										Color.FromArgb(255,118,0),
										Color.FromArgb(255,114,0),
										Color.FromArgb(255,110,0),
										Color.FromArgb(255,106,0),
										Color.FromArgb(255,102,0),
										Color.FromArgb(255,98,0),
										Color.FromArgb(255,94,0),
										Color.FromArgb(255,91,0),
										Color.FromArgb(255,87,0),
										Color.FromArgb(255,83,0),
										Color.FromArgb(255,79,0),
										Color.FromArgb(255,75,0),
										Color.FromArgb(255,71,0),
										Color.FromArgb(255,67,0),
										Color.FromArgb(255,63,0),
										Color.FromArgb(255,59,0),
										Color.FromArgb(255,55,0),
										Color.FromArgb(255,51,0),
										Color.FromArgb(255,47,0),
										Color.FromArgb(255,43,0),
										Color.FromArgb(255,39,0),
										Color.FromArgb(255,35,0),
										Color.FromArgb(255,31,0),
										Color.FromArgb(255,28,0),
										Color.FromArgb(255,24,0),
										Color.FromArgb(255,20,0),
										Color.FromArgb(255,16,0),
										Color.FromArgb(255,12,0),
										Color.FromArgb(255,8,0),
										Color.FromArgb(255,4,0),
										Color.FromArgb(255,0,0),
										Color.FromArgb(255,4,0),
										Color.FromArgb(255,8,0),
										Color.FromArgb(255,12,0),
										Color.FromArgb(255,16,0),
										Color.FromArgb(255,20,0),
										Color.FromArgb(255,24,0),
										Color.FromArgb(255,28,0),
										Color.FromArgb(255,31,0),
										Color.FromArgb(255,35,0),
										Color.FromArgb(255,39,0),
										Color.FromArgb(255,43,0),
										Color.FromArgb(255,47,0),
										Color.FromArgb(255,51,0),
										Color.FromArgb(255,55,0),
										Color.FromArgb(255,59,0),
										Color.FromArgb(255,63,0),
										Color.FromArgb(255,67,0),
										Color.FromArgb(255,71,0),
										Color.FromArgb(255,75,0),
										Color.FromArgb(255,79,0),
										Color.FromArgb(255,83,0),
										Color.FromArgb(255,87,0),
										Color.FromArgb(255,91,0),
										Color.FromArgb(255,94,0),
										Color.FromArgb(255,98,0),
										Color.FromArgb(255,102,0),
										Color.FromArgb(255,106,0),
										Color.FromArgb(255,110,0),
										Color.FromArgb(255,114,0),
										Color.FromArgb(255,118,0),
										Color.FromArgb(255,122,0),
										Color.FromArgb(255,126,0),
										Color.FromArgb(255,130,0),
										Color.FromArgb(255,133,0),
										Color.FromArgb(255,137,0),
										Color.FromArgb(255,141,0),
										Color.FromArgb(255,145,0),
										Color.FromArgb(255,149,0),
										Color.FromArgb(255,153,0),
										Color.FromArgb(255,157,0),
										Color.FromArgb(255,161,0),
										Color.FromArgb(255,255,0),
										Color.FromArgb(255,167,0),
										Color.FromArgb(255,170,0),
										Color.FromArgb(255,172,0),
										Color.FromArgb(255,174,0),
										Color.FromArgb(255,176,0),
										Color.FromArgb(255,178,0),
										Color.FromArgb(255,180,0),
										Color.FromArgb(255,182,0),
										Color.FromArgb(255,185,0),
										Color.FromArgb(255,187,0),
										Color.FromArgb(255,189,0),
										Color.FromArgb(255,191,0),
										Color.FromArgb(255,193,0),
										Color.FromArgb(255,195,0),
										Color.FromArgb(255,197,0),
										Color.FromArgb(255,199,0),
										Color.FromArgb(255,201,0),
										Color.FromArgb(255,203,0),
										Color.FromArgb(255,205,0),
										Color.FromArgb(255,208,0),
										Color.FromArgb(255,210,0),
										Color.FromArgb(255,212,0),
										Color.FromArgb(255,215,0),
										Color.FromArgb(255,217,0),
										Color.FromArgb(255,219,0),
										Color.FromArgb(255,221,0),
										Color.FromArgb(255,223,0),
										Color.FromArgb(255,225,0),
										Color.FromArgb(255,227,0),
										Color.FromArgb(255,230,0),
										Color.FromArgb(255,232,0),
										Color.FromArgb(255,234,0),
										Color.FromArgb(255,236,0),
										Color.FromArgb(255,238,0),
										Color.FromArgb(255,240,0),
										Color.FromArgb(255,242,0),
										Color.FromArgb(255,244,0),
										Color.FromArgb(255,246,0),
										Color.FromArgb(255,248,0),
										Color.FromArgb(255,250,0),
										Color.FromArgb(255,253,0),
										Color.FromArgb(0,128,0),
										Color.FromArgb(249,252,0),
										Color.FromArgb(243,249,0),
										Color.FromArgb(237,246,0),
										Color.FromArgb(231,243,0),
										Color.FromArgb(225,240,0),
										Color.FromArgb(218,237,0),
										Color.FromArgb(212,234,0),
										Color.FromArgb(206,231,0),
										Color.FromArgb(200,228,0),
										Color.FromArgb(194,225,0),
										Color.FromArgb(188,222,0),
										Color.FromArgb(182,218,0),
										Color.FromArgb(176,215,0),
										Color.FromArgb(170,212,0),
										Color.FromArgb(164,209,0),
										Color.FromArgb(158,206,0),
										Color.FromArgb(151,203,0),
										Color.FromArgb(145,200,0),
										Color.FromArgb(139,197,0),
										Color.FromArgb(134,195,0),
										Color.FromArgb(128,192,0),
										Color.FromArgb(122,189,0),
										Color.FromArgb(116,186,0),
										Color.FromArgb(110,183,0),
										Color.FromArgb(104,180,0),
										Color.FromArgb(98,177,0),
										Color.FromArgb(91,174,0),
										Color.FromArgb(85,171,0),
										Color.FromArgb(79,168,0),
										Color.FromArgb(73,165,0),
										Color.FromArgb(67,162,0),
										Color.FromArgb(61,159,0),
										Color.FromArgb(55,155,0),
										Color.FromArgb(49,152,0),
										Color.FromArgb(43,149,0),
										Color.FromArgb(37,146,0),
										Color.FromArgb(31,143,0),
										Color.FromArgb(24,140,0),
										Color.FromArgb(18,137,0),
										Color.FromArgb(12,134,0),
										Color.FromArgb(6,131,0),
										Color.FromArgb(0,0,255),
										Color.FromArgb(0,125,6),
										Color.FromArgb(0,122,12),
										Color.FromArgb(0,119,18),
										Color.FromArgb(0,116,24),
										Color.FromArgb(0,113,30),
										Color.FromArgb(0,110,37),
										Color.FromArgb(0,107,43),
										Color.FromArgb(0,104,49),
										Color.FromArgb(0,101,55),
										Color.FromArgb(0,98,61),
										Color.FromArgb(0,95,67),
										Color.FromArgb(0,91,73),
										Color.FromArgb(0,88,79),
										Color.FromArgb(0,85,85),
										Color.FromArgb(0,82,91),
										Color.FromArgb(0,79,98),
										Color.FromArgb(0,76,104),
										Color.FromArgb(0,73,110),
										Color.FromArgb(0,70,116),
										Color.FromArgb(0,67,122),
										Color.FromArgb(0,64,128),
										Color.FromArgb(0,61,134),
										Color.FromArgb(0,58,139),
										Color.FromArgb(0,55,145),
										Color.FromArgb(0,52,151),
										Color.FromArgb(0,49,157),
										Color.FromArgb(0,46,164),
										Color.FromArgb(0,43,170),
										Color.FromArgb(0,40,176),
										Color.FromArgb(0,37,182),
										Color.FromArgb(0,34,188),
										Color.FromArgb(0,31,194),
										Color.FromArgb(0,27,200),
										Color.FromArgb(0,24,206),
										Color.FromArgb(0,21,212),
										Color.FromArgb(0,18,218),
										Color.FromArgb(0,15,225),
										Color.FromArgb(0,12,231),
										Color.FromArgb(0,9,237),
										Color.FromArgb(0,6,243),
										Color.FromArgb(0,3,249),
										Color.FromArgb(75,0,130),
										Color.FromArgb(2,0,252),
										Color.FromArgb(4,0,249),
										Color.FromArgb(6,0,246),
										Color.FromArgb(7,0,243),
										Color.FromArgb(9,0,240),
										Color.FromArgb(10,0,237),
										Color.FromArgb(12,0,234),
										Color.FromArgb(14,0,232),
										Color.FromArgb(16,0,229),
										Color.FromArgb(18,0,226),
										Color.FromArgb(20,0,223),
										Color.FromArgb(21,0,219),
										Color.FromArgb(23,0,216),
										Color.FromArgb(25,0,213),
										Color.FromArgb(27,0,210),
										Color.FromArgb(28,0,207),
										Color.FromArgb(30,0,204),
										Color.FromArgb(32,0,201),
										Color.FromArgb(34,0,198),
										Color.FromArgb(36,0,196),
										Color.FromArgb(38,0,193),
										Color.FromArgb(40,0,190),
										Color.FromArgb(41,0,187),
										Color.FromArgb(43,0,184),
										Color.FromArgb(45,0,181),
										Color.FromArgb(47,0,178),
										Color.FromArgb(48,0,175),
										Color.FromArgb(50,0,172),
										Color.FromArgb(52,0,169),
										Color.FromArgb(54,0,166),
										Color.FromArgb(55,0,163),
										Color.FromArgb(57,0,160),
										Color.FromArgb(59,0,156),
										Color.FromArgb(61,0,153),
										Color.FromArgb(63,0,151),
										Color.FromArgb(65,0,148),
										Color.FromArgb(66,0,145),
										Color.FromArgb(68,0,142),
										Color.FromArgb(69,0,139),
										Color.FromArgb(71,0,136),
										Color.FromArgb(73,0,133),
										Color.FromArgb(238,130,238),
										Color.FromArgb(79,3,133),
										Color.FromArgb(83,6,135),
										Color.FromArgb(86,9,138),
										Color.FromArgb(90,12,141),
										Color.FromArgb(94,15,143),
										Color.FromArgb(98,18,146),
										Color.FromArgb(102,21,148),
										Color.FromArgb(106,24,150),
										Color.FromArgb(110,28,153),
										Color.FromArgb(114,31,156),
										Color.FromArgb(118,35,158),
										Color.FromArgb(122,38,161),
										Color.FromArgb(125,41,164),
										Color.FromArgb(129,44,166),
										Color.FromArgb(133,47,169),
										Color.FromArgb(137,50,172),
										Color.FromArgb(141,53,174),
										Color.FromArgb(145,56,176),
										Color.FromArgb(149,59,179),
										Color.FromArgb(153,62,181),
										Color.FromArgb(157,65,184),
										Color.FromArgb(161,68,187),
										Color.FromArgb(165,71,189),
										Color.FromArgb(168,74,192),
										Color.FromArgb(172,77,195),
										Color.FromArgb(176,80,197),
										Color.FromArgb(180,83,200),
										Color.FromArgb(184,86,202),
										Color.FromArgb(187,89,204),
										Color.FromArgb(191,93,207),
										Color.FromArgb(195,96,210),
										Color.FromArgb(199,100,212),
										Color.FromArgb(203,103,215),
										Color.FromArgb(206,106,218),
										Color.FromArgb(211,109,220),
										Color.FromArgb(215,112,223),
										Color.FromArgb(219,115,226),
										Color.FromArgb(223,118,228),
										Color.FromArgb(227,121,230),
										Color.FromArgb(230,124,233),
										Color.FromArgb(234,127,235),
										Color.FromArgb(238,130,238),
										Color.FromArgb(234,127,235),
										Color.FromArgb(230,124,233),
										Color.FromArgb(227,121,230),
										Color.FromArgb(223,118,228),
										Color.FromArgb(219,115,226),
										Color.FromArgb(215,112,223),
										Color.FromArgb(211,109,220),
										Color.FromArgb(206,106,218),
										Color.FromArgb(203,103,215),
										Color.FromArgb(199,100,212),
										Color.FromArgb(195,96,210),
										Color.FromArgb(191,93,207),
										Color.FromArgb(187,89,204),
										Color.FromArgb(184,86,202),
										Color.FromArgb(180,83,200),
										Color.FromArgb(176,80,197),
										Color.FromArgb(172,77,195),
										Color.FromArgb(168,74,192),
										Color.FromArgb(165,71,189),
										Color.FromArgb(161,68,187),
										Color.FromArgb(157,65,184),
										Color.FromArgb(153,62,181),
										Color.FromArgb(149,59,179),
										Color.FromArgb(145,56,176),
										Color.FromArgb(141,53,174),
										Color.FromArgb(137,50,172),
										Color.FromArgb(133,47,169),
										Color.FromArgb(129,44,166),
										Color.FromArgb(125,41,164),
										Color.FromArgb(122,38,161),
										Color.FromArgb(118,35,158),
										Color.FromArgb(114,31,156),
										Color.FromArgb(110,28,153),
										Color.FromArgb(106,24,150),
										Color.FromArgb(102,21,148),
										Color.FromArgb(98,18,146),
										Color.FromArgb(94,15,143),
										Color.FromArgb(90,12,141),
										Color.FromArgb(86,9,138),
										Color.FromArgb(83,6,135),
										Color.FromArgb(79,3,133),
										Color.FromArgb(238,130,238),
										Color.FromArgb(73,0,133),
										Color.FromArgb(71,0,136),
										Color.FromArgb(69,0,139),
										Color.FromArgb(68,0,142),
										Color.FromArgb(66,0,145),
										Color.FromArgb(65,0,148),
										Color.FromArgb(63,0,151),
										Color.FromArgb(61,0,153),
										Color.FromArgb(59,0,156),
										Color.FromArgb(57,0,160),
										Color.FromArgb(55,0,163),
										Color.FromArgb(54,0,166),
										Color.FromArgb(52,0,169),
										Color.FromArgb(50,0,172),
										Color.FromArgb(48,0,175),
										Color.FromArgb(47,0,178),
										Color.FromArgb(45,0,181),
										Color.FromArgb(43,0,184),
										Color.FromArgb(41,0,187),
										Color.FromArgb(40,0,190),
										Color.FromArgb(38,0,193),
										Color.FromArgb(36,0,196),
										Color.FromArgb(34,0,198),
										Color.FromArgb(32,0,201),
										Color.FromArgb(30,0,204),
										Color.FromArgb(28,0,207),
										Color.FromArgb(27,0,210),
										Color.FromArgb(25,0,213),
										Color.FromArgb(23,0,216),
										Color.FromArgb(21,0,219),
										Color.FromArgb(20,0,223),
										Color.FromArgb(18,0,226),
										Color.FromArgb(16,0,229),
										Color.FromArgb(14,0,232),
										Color.FromArgb(12,0,234),
										Color.FromArgb(10,0,237),
										Color.FromArgb(9,0,240),
										Color.FromArgb(7,0,243),
										Color.FromArgb(6,0,246),
										Color.FromArgb(4,0,249),
										Color.FromArgb(2,0,252),
										Color.FromArgb(75,0,130),
										Color.FromArgb(0,3,249),
										Color.FromArgb(0,6,243),
										Color.FromArgb(0,9,237),
										Color.FromArgb(0,12,231),
										Color.FromArgb(0,15,225),
										Color.FromArgb(0,18,218),
										Color.FromArgb(0,21,212),
										Color.FromArgb(0,24,206),
										Color.FromArgb(0,27,200),
										Color.FromArgb(0,31,194),
										Color.FromArgb(0,34,188),
										Color.FromArgb(0,37,182),
										Color.FromArgb(0,40,176),
										Color.FromArgb(0,43,170),
										Color.FromArgb(0,46,164),
										Color.FromArgb(0,49,157),
										Color.FromArgb(0,52,151),
										Color.FromArgb(0,55,145),
										Color.FromArgb(0,58,139),
										Color.FromArgb(0,61,134),
										Color.FromArgb(0,64,128),
										Color.FromArgb(0,67,122),
										Color.FromArgb(0,70,116),
										Color.FromArgb(0,73,110),
										Color.FromArgb(0,76,104),
										Color.FromArgb(0,79,98),
										Color.FromArgb(0,82,91),
										Color.FromArgb(0,85,85),
										Color.FromArgb(0,88,79),
										Color.FromArgb(0,91,73),
										Color.FromArgb(0,95,67),
										Color.FromArgb(0,98,61),
										Color.FromArgb(0,101,55),
										Color.FromArgb(0,104,49),
										Color.FromArgb(0,107,43),
										Color.FromArgb(0,110,37),
										Color.FromArgb(0,113,30),
										Color.FromArgb(0,116,24),
										Color.FromArgb(0,119,18),
										Color.FromArgb(0,122,12),
										Color.FromArgb(0,125,6),
										Color.FromArgb(0,0,255),
										Color.FromArgb(6,131,0),
										Color.FromArgb(12,134,0),
										Color.FromArgb(18,137,0),
										Color.FromArgb(24,140,0),
										Color.FromArgb(31,143,0),
										Color.FromArgb(37,146,0),
										Color.FromArgb(43,149,0),
										Color.FromArgb(49,152,0),
										Color.FromArgb(55,155,0),
										Color.FromArgb(61,159,0),
										Color.FromArgb(67,162,0),
										Color.FromArgb(73,165,0),
										Color.FromArgb(79,168,0),
										Color.FromArgb(85,171,0),
										Color.FromArgb(91,174,0),
										Color.FromArgb(98,177,0),
										Color.FromArgb(104,180,0),
										Color.FromArgb(110,183,0),
										Color.FromArgb(116,186,0),
										Color.FromArgb(122,189,0),
										Color.FromArgb(128,192,0),
										Color.FromArgb(134,195,0),
										Color.FromArgb(139,197,0),
										Color.FromArgb(145,200,0),
										Color.FromArgb(151,203,0),
										Color.FromArgb(158,206,0),
										Color.FromArgb(164,209,0),
										Color.FromArgb(170,212,0),
										Color.FromArgb(176,215,0),
										Color.FromArgb(182,218,0),
										Color.FromArgb(188,222,0),
										Color.FromArgb(194,225,0),
										Color.FromArgb(200,228,0),
										Color.FromArgb(206,231,0),
										Color.FromArgb(212,234,0),
										Color.FromArgb(218,237,0),
										Color.FromArgb(225,240,0),
										Color.FromArgb(231,243,0),
										Color.FromArgb(237,246,0),
										Color.FromArgb(243,249,0),
										Color.FromArgb(249,252,0),
										Color.FromArgb(0,128,0),
										Color.FromArgb(255,253,0),
										Color.FromArgb(255,250,0),
										Color.FromArgb(255,248,0),
										Color.FromArgb(255,246,0),
										Color.FromArgb(255,244,0),
										Color.FromArgb(255,242,0),
										Color.FromArgb(255,240,0),
										Color.FromArgb(255,238,0),
										Color.FromArgb(255,236,0),
										Color.FromArgb(255,234,0),
										Color.FromArgb(255,232,0),
										Color.FromArgb(255,230,0),
										Color.FromArgb(255,227,0),
										Color.FromArgb(255,225,0),
										Color.FromArgb(255,223,0),
										Color.FromArgb(255,221,0),
										Color.FromArgb(255,219,0),
										Color.FromArgb(255,217,0),
										Color.FromArgb(255,215,0),
										Color.FromArgb(255,212,0),
										Color.FromArgb(255,210,0),
										Color.FromArgb(255,208,0),
										Color.FromArgb(255,205,0),
										Color.FromArgb(255,203,0),
										Color.FromArgb(255,201,0),
										Color.FromArgb(255,199,0),
										Color.FromArgb(255,197,0),
										Color.FromArgb(255,195,0),
										Color.FromArgb(255,193,0),
										Color.FromArgb(255,191,0),
										Color.FromArgb(255,189,0),
										Color.FromArgb(255,187,0),
										Color.FromArgb(255,185,0),
										Color.FromArgb(255,182,0),
										Color.FromArgb(255,180,0),
										Color.FromArgb(255,178,0),
										Color.FromArgb(255,176,0),
										Color.FromArgb(255,174,0),
										Color.FromArgb(255,172,0),
										Color.FromArgb(255,170,0),
										Color.FromArgb(255,167,0),
										Color.FromArgb(255,255,0),
										Color.FromArgb(255,161,0),
										Color.FromArgb(255,157,0),
										Color.FromArgb(255,153,0),
										Color.FromArgb(255,149,0),
										Color.FromArgb(255,145,0),
										Color.FromArgb(255,141,0),
										Color.FromArgb(255,137,0),
										Color.FromArgb(255,133,0),
										Color.FromArgb(255,130,0),
										Color.FromArgb(255,126,0),
										Color.FromArgb(255,122,0),
										Color.FromArgb(255,118,0),
										Color.FromArgb(255,114,0),
										Color.FromArgb(255,110,0),
										Color.FromArgb(255,106,0),
										Color.FromArgb(255,102,0),
										Color.FromArgb(255,98,0),
										Color.FromArgb(255,94,0),
										Color.FromArgb(255,91,0),
										Color.FromArgb(255,87,0),
										Color.FromArgb(255,83,0),
										Color.FromArgb(255,79,0),
										Color.FromArgb(255,75,0),
										Color.FromArgb(255,71,0),
										Color.FromArgb(255,67,0),
										Color.FromArgb(255,63,0),
										Color.FromArgb(255,59,0),
										Color.FromArgb(255,55,0),
										Color.FromArgb(255,51,0),
										Color.FromArgb(255,47,0),
										Color.FromArgb(255,43,0),
										Color.FromArgb(255,39,0),
										Color.FromArgb(255,35,0),
										Color.FromArgb(255,31,0),
										Color.FromArgb(255,28,0),
										Color.FromArgb(255,24,0),
										Color.FromArgb(255,20,0),
										Color.FromArgb(255,16,0),
										Color.FromArgb(255,12,0),
										Color.FromArgb(255,8,0),
										Color.FromArgb(255,4,0),
										Color.FromArgb(255,0,0),
										Color.FromArgb(0,0,0),
										Color.FromArgb(0,0,0),
										Color.FromArgb(0,0,0)


		};
        #endregion
       
    }

  
   
}
