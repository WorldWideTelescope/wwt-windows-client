using System;
using System.Collections;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.IO.Compression;

using System.Text;
using Microsoft.Maps.ElevationAdjustmentService.HDPhoto;
using SharpDX;
using WwtDataUtils;

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

        public virtual IndexBuffer11 GetIndexBuffer(int index, int accomidation)
        {
            return indexBuffer[index];
        }



        public virtual double GetSurfacePointAltitude(double lat, double lng, bool meters)
        {

            if (level < lastDeepestLevel)
            {
                //interate children
                foreach (long childKey in childrenId)
                {
                    Tile child = TileCache.GetCachedTile(childKey);
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
                int yOffset = 0;
                if (dataset.Mercator || dataset.BottomsUp)
                {
                    yOffset = 1;
                }
                int xOffset = 0;

                int xMax = 2;
                int childIndex = 0;
                for (int y1 = 0; y1 < 2; y1++)
                {
                    for (int x1 = 0; x1 < xMax; x1++)
                    {

                        if (level < dataset.Levels && level < (targetLevel+1))
                        {
                            Tile child = TileCache.GetTileNow(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
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
            StringBuilder sb = new StringBuilder();
			
				Tile t = this;
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
                sb.Append(dataset.Projection.ToString());
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
            int xMax = 2;
 
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
            
          

            int childIndex = 0;

            int yOffset = 0;
            if (dataset.Mercator || dataset.BottomsUp )
            {
                yOffset = 1;
            }
            int xOffset = 0;

            if (PurgeRefresh)
            {
                PurgeTextureAndDemFiles();
            }
            Matrix3d savedWorld = renderContext.World;
            Matrix3d savedView = renderContext.View;
            bool usingLocalCenter = false;
            if (localCenter != Vector3d.Empty)
            {
                usingLocalCenter = true;
                Vector3d temp = localCenter;
                renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBase * Matrix3d.Translation(-renderContext.CameraPosition);
                renderContext.View = Matrix3d.Translation(renderContext.CameraPosition) * renderContext.ViewBase;
            }

            try
            {
                bool anythingToRender = false;
                bool childRendered = false;

                for (int y1 = 0; y1 < 2; y1++)
                {
                    for (int x1 = 0; x1 < xMax; x1++)
                    {
                        //  if (level < (demEnabled ? 12 : dataset.Levels))
                        if (level < dataset.Levels)
                        {
                            Tile child = TileCache.GetTile(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
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

                if ( wireFrame)
                {
                    renderContext.MainTexture = null;
                }
                else
                {
                    renderContext.MainTexture = texture;
                }
                
                renderContext.SetVertexBuffer(vertexBuffer);

                accomidation = ComputeAccomidation();

                for (int i = 0; i < 4; i++)
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
                    if (File.Exists(this.FileName))
                    {
                        File.Delete(this.FileName);
                    }

                    if (File.Exists(this.DemFilename))
                    {
                        File.Delete(this.DemFilename);
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
            int accVal = 0;

            if (!useAccomidation)
            {
                return 0;
            }

            


            //Bottom
            Tile top = TileCache.GetCachedTile(level, x, y + 1, dataset, this);
            if (top == null || top.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 1;
            }

            //right
            Tile right = TileCache.GetCachedTile(level, x + 1, y, dataset, this);
            if (right == null || right.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 2;
            }

            //top
            Tile bottom = TileCache.GetCachedTile(level, x, y - 1, dataset, this);
            if (bottom == null || bottom.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 4;
            }
            //left
            Tile left = TileCache.GetCachedTile(level, x - 1, y, dataset, this);
            if (left == null || left.RenderedAtOrBelowGeneration < CurrentRenderGeneration - 2)
            {
                accVal += 8;
            }

            return accVal;
        }

        public virtual void RenderPart(RenderContext11 renderContext, int part, float opacity, bool combine)
        {
            int partCount = this.TriangleCount / 4;
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

            if (this.texture != null)
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:Texture Cleanup"); }
                BufferPool11.ReturnTexture(texture);
                this.texture = null;
            }
            else
            {
                if (Earth3d.Logging) { Earth3d.WriteLogMessage("Tile:Cleanup - no Texture"); }
            }

            if (this.vertexBuffer != null)
            {

                BufferPool11.ReturnPNTX2VertexBuffer(vertexBuffer);
                this.vertexBuffer = null;
            }

            if (this.indexBuffer != null)
            {
                foreach (IndexBuffer11 buffer in indexBuffer)
                {
                    if (buffer != null)
                    {
                        BufferPool11.ReturnShortIndexBuffer(buffer);
                    }
                }
                this.indexBuffer = null;
            }
        }
        public virtual void CleanUpGeometryOnly()
        {

            if (this.vertexBuffer != null)
            {
                //vertexBuffer.Created -= OnCreateVertexBuffer();
                this.vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                this.vertexBuffer = null;
            }

            if (this.indexBuffer != null)
            {
                foreach (IndexBuffer11 buffer in indexBuffer)
                {
                    if (buffer != null)
                    {
                        BufferPool11.ReturnShortIndexBuffer(buffer);
                    }
                }
                this.indexBuffer = null;
            }
        }

        public virtual void CleanUpGeometryRecursive()
        {
            foreach (long childKey in childrenId)
            {
                Tile child = TileCache.GetCachedTile(childKey);
                if (child != null)
                {
                    child.CleanUpGeometryRecursive();
                }
            }
            this.CleanUpGeometryOnly();
        }

        //todoperf this needs to be pointer free
        protected void RemoveChild(Tile child)
        {
            //We are pointer free so this is not needed

        }

        bool blendMode = false;

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
                if (this.texture == null)
                {
                    if (TextureReady)
                    {
                        iTileBuildCount++;

                        string localFilename = FileName;
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
                    this.OnCreateVertexBuffer(vertexBuffer);

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
            Vector3d[] corners = new Vector3d[4];
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
            bool useFloat = false;
        	FileInfo fi = new FileInfo(DemFilename);

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
                int yh = 0;
                for (int yl = 0; yl < 33;  yl++)
                {
                    int xh = 0;
                    for (int xl = 0; xl < 33; xl++)
                    {
                        int indexI = xl + (32-yl) * 33;
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

                Byte[] part = new Byte[4];

                for (int i = 0; i < DemData.Length; i++)
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

        
        public static SharpDX.ViewportF Viewport;
        public static SharpDX.Matrix wvp;
        public static int maxLevel = 20;
     
        
        

        virtual public bool IsTileBigEnough(RenderContext11 renderContext)
        {
            if (level > 1)
            {

                SharpDX.Vector3 topLeftScreen;
                SharpDX.Vector3 bottomRightScreen;
                SharpDX.Vector3 topRightScreen;
                SharpDX.Vector3 bottomLeftScreen;

                SharpDX.Matrix proj = renderContext.Projection.Matrix11;
                SharpDX.Matrix view = renderContext.ViewBase.Matrix11;
                SharpDX.Matrix world = renderContext.WorldBase.Matrix11;

                // Test for tile scale in view..
                topLeftScreen = TopLeft.Vector311;
                topLeftScreen = SharpDX.Vector3.Project(topLeftScreen,Viewport.X,Viewport.Y,Viewport.Width,Viewport.Height,Viewport.MinDepth,Viewport.MaxDepth, wvp);


                bottomRightScreen = BottomRight.Vector311;
                bottomRightScreen = SharpDX.Vector3.Project(bottomRightScreen, Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);

                topRightScreen = TopRight.Vector311;
                topRightScreen = SharpDX.Vector3.Project(topRightScreen, Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);
    
                bottomLeftScreen = BottomLeft.Vector311;
                bottomLeftScreen = SharpDX.Vector3.Project(bottomLeftScreen, Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);

                SharpDX.Vector3 top = topLeftScreen;
                top = SharpDX.Vector3.Subtract(top, topRightScreen);
                float topLength = top.Length();

                SharpDX.Vector3 bottom = bottomLeftScreen;
                bottom = SharpDX.Vector3.Subtract(bottom, bottomRightScreen);
                float bottomLength = bottom.Length();

                SharpDX.Vector3 left = bottomLeftScreen;
                left = SharpDX.Vector3.Subtract(left, topLeftScreen);
                float leftLength = left.Length();

                SharpDX.Vector3 right = bottomRightScreen;
                right = SharpDX.Vector3.Subtract(right, topRightScreen);
                float rightLength = right.Length();
                

                float lengthMax = Math.Max(Math.Max(rightLength, leftLength), Math.Max(bottomLength, topLength));

                float testLength = (400 - ((Earth3d.MainWindow.dumpFrameParams.Dome && SpaceTimeController.FrameDumping) ? -200 : Tile.imageQuality));

                if (Properties.Settings.Default.OverSampleTerrain)
                {
                    testLength = 150;
                }

                if (lengthMax < testLength) // was 220
                {
                    return false;

                }
                else
                {
                    deepestLevel = level > deepestLevel ? level : deepestLevel;
                }
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
            Vector3d center = sphereCenter;

            if (this.Level < 2 && (dataset.Projection == ProjectionType.Mercator || dataset.Projection == ProjectionType.Toast))
            {
                return true;
            }

            Vector4d centerV4 = new Vector4d(center.X , center.Y , center.Z , 1f);
            Vector3d length = new Vector3d(sphereRadius, 0, 0);

            double rad = length.Length();
            for (int i = 0; i < 6; i++)
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
                Vector3d retVal = new Vector3d(-(Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));
                if (useLocalCenter)
                {
                    retVal.Subtract(localCenter);
                }
                return retVal;

            }
            else
            {
                Vector3d retVal = new Vector3d((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius));
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

            double altitude = DemData[demIndex];
            Vector3d retVal = GeoTo3dWithAltitude(lat, lng, altitude, useLocalCenter);
            return retVal;
        }

        public Vector3d GeoTo3dWithAltitude(double lat, double lng, double altitude, bool useLocalCenter)
        {

            double radius = 1 + (altitude / DemScaleFactor);
            Vector3d retVal = (new Vector3d((Math.Cos(lng * RC) * Math.Cos(lat * RC) * radius), (Math.Sin(lat * RC) * radius), (Math.Sin(lng * RC) * Math.Cos(lat * RC) * radius)));
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
                    if (this.dataset.Projection == ProjectionType.SkyImage && this.dataset.Url.EndsWith("/screenshot.png"))
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
            StringBuilder sb = new StringBuilder();
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

                    if (this.dataset.Projection == ProjectionType.SkyImage && this.dataset.Url.EndsWith("/screenshot.png"))
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
            string extention = dataset.Extension.StartsWith(".") ? dataset.Extension : "." + dataset.Extension;
            StringBuilder sb = new StringBuilder();
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
                else
                {
                    return url;
                }
            }
        }
        internal static string GetWmsURL(IImageSet dataset, int level, int x, int y)
        {

            double tileDegrees = dataset.BaseTileDegrees / (Math.Pow(2, level));

            double latMin = ( (((double)y) * tileDegrees)-90 );
            double latMax = ( (((double)(y + 1)) * tileDegrees) -90 );
            double lngMin = (((double)x * tileDegrees) - 180.0);
            double lngMax = ((((double)(x + 1)) * tileDegrees) - 180.0);
            string returnUrl = dataset.Url;

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
                else
                {
                    return String.Format(dataset.Url, dataset.ImageSetID, level, x, y);
                }
            }


            string returnUrl = dataset.Url;

            returnUrl = returnUrl.Replace("{X}", x.ToString());
            returnUrl = returnUrl.Replace("{Y}", y.ToString());
            returnUrl = returnUrl.Replace("{L}", level.ToString());
            int hash = 0;
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
            else
            {
                //if (returnUrl.ToLower().Contains("virtualearth"))
                //{
                //    UiTools.ShowMessageBox("VE url that was not caught!" + returnUrl);
                //}
            }

            string id = GetTileID(dataset, level, x, y);
            string server = "";

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
            int server = (tileX & 1) + ((tileY & 1) << 1);

            return (server);
        }

        string tileId = null;
        public string GetTileID()
        {
            if (tileId == null)
            {
                tileId = GetTileID(dataset, level, x, y);
            }

            return tileId;
        }

        public static string GetTileID(IImageSet dataset, int tileLevel, int tileX, int tileY)
        {
            int netLevel = tileLevel;
            int netX = tileX;
            int netY = tileY;
            string tileId = "";
            if (dataset.Projection == ProjectionType.Equirectangular)
            {
                netLevel++;
            }

            string tileMap = dataset.QuadTreeTileMap;

            if (!string.IsNullOrEmpty(tileMap))
            {
                StringBuilder sb = new StringBuilder();

                for (int i = netLevel; i > 0; --i)
                {
                    int mask = 1 << (i - 1);
                    int val = 0;

                    if ((netX & mask) != 0)
                        val = 1;

                    if ((netY & mask) != 0)
                        val += 2;

                    sb.Append(tileMap[val]);

                }
                tileId = sb.ToString();
                return tileId;
            }
            else
            {
                tileId = "0";
                return tileId;
            }
        }

        private int vertexCount;

        protected int VertexCount
        {
            get 
            {

                return vertexCount;
            }
            set
            {
                vertexCount = value;
            }
        }
        BlendState[] renderPart = null;
        bool demEnabled = false;
        bool demInitialized = false;
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
        public Tile()
        {
            
            renderPart = new BlendState[4];
            for (int i = 0; i < 4; i++ )
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
            StringBuilder sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"dem\");
            sb.Append(Math.Abs(dataset.DemUrl.GetHashCode32()).ToString());
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
            StringBuilder sb = new StringBuilder();
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
                string baseUrl = "http://cdn.worldwidetelescope.org/wwtweb/demtile.aspx?q={0},{1},{2},M";
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

            string returnUrl = dataset.DemUrl;

            returnUrl = returnUrl.Replace("{X}", x.ToString());
            returnUrl = returnUrl.Replace("{Y}", y.ToString());
            returnUrl = returnUrl.Replace("{L}", level.ToString());
            int hash = 0;
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


            string id = GetTileID(dataset, level, x, y);
            string server = "";

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
            int looper = 0;
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
                string filename = this.FileName;
                string path = this.Directory;
                Bitmap b = null;
                PixelData pixel;
                pixel.alpha = 255;
                MAXITER = 100 + level * 38;


                double tileWidth = (4 / (Math.Pow(2, this.level)));
                double Sy = ((double)this.y * tileWidth) - 2;
                double Fy = Sy + tileWidth;
                double Sx = ((double)this.x * tileWidth) - 4;
                double Fx = Sx + tileWidth;

                b = new Bitmap(mandelWidth, mandelWidth);
                FastBitmap fb = new FastBitmap(b);
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

                bool computeAll = true;

                if (computeAll)
                {
                    for (s = 0; s < mandelWidth; s++)
                    {
                        y = ymin;
                        for (z = 0; z < mandelWidth; z++)
                        {

                            looper = MandPoint(x, y);
                            System.Drawing.Color col = (looper == MAXITER) ? System.Drawing.Color.Black : Tile.ColorTable[looper % 1011];
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
        static System.Drawing.Color[] ColorTable = {
										System.Drawing.Color.FromArgb(255,0,0),
										System.Drawing.Color.FromArgb(255,4,0),
										System.Drawing.Color.FromArgb(255,8,0),
										System.Drawing.Color.FromArgb(255,12,0),
										System.Drawing.Color.FromArgb(255,16,0),
										System.Drawing.Color.FromArgb(255,20,0),
										System.Drawing.Color.FromArgb(255,24,0),
										System.Drawing.Color.FromArgb(255,28,0),
										System.Drawing.Color.FromArgb(255,31,0),
										System.Drawing.Color.FromArgb(255,35,0),
										System.Drawing.Color.FromArgb(255,39,0),
										System.Drawing.Color.FromArgb(255,43,0),
										System.Drawing.Color.FromArgb(255,47,0),
										System.Drawing.Color.FromArgb(255,51,0),
										System.Drawing.Color.FromArgb(255,55,0),
										System.Drawing.Color.FromArgb(255,59,0),
										System.Drawing.Color.FromArgb(255,63,0),
										System.Drawing.Color.FromArgb(255,67,0),
										System.Drawing.Color.FromArgb(255,71,0),
										System.Drawing.Color.FromArgb(255,75,0),
										System.Drawing.Color.FromArgb(255,79,0),
										System.Drawing.Color.FromArgb(255,83,0),
										System.Drawing.Color.FromArgb(255,87,0),
										System.Drawing.Color.FromArgb(255,91,0),
										System.Drawing.Color.FromArgb(255,94,0),
										System.Drawing.Color.FromArgb(255,98,0),
										System.Drawing.Color.FromArgb(255,102,0),
										System.Drawing.Color.FromArgb(255,106,0),
										System.Drawing.Color.FromArgb(255,110,0),
										System.Drawing.Color.FromArgb(255,114,0),
										System.Drawing.Color.FromArgb(255,118,0),
										System.Drawing.Color.FromArgb(255,122,0),
										System.Drawing.Color.FromArgb(255,126,0),
										System.Drawing.Color.FromArgb(255,130,0),
										System.Drawing.Color.FromArgb(255,133,0),
										System.Drawing.Color.FromArgb(255,137,0),
										System.Drawing.Color.FromArgb(255,141,0),
										System.Drawing.Color.FromArgb(255,145,0),
										System.Drawing.Color.FromArgb(255,149,0),
										System.Drawing.Color.FromArgb(255,153,0),
										System.Drawing.Color.FromArgb(255,157,0),
										System.Drawing.Color.FromArgb(255,161,0),
										System.Drawing.Color.FromArgb(255,255,0),
										System.Drawing.Color.FromArgb(255,167,0),
										System.Drawing.Color.FromArgb(255,170,0),
										System.Drawing.Color.FromArgb(255,172,0),
										System.Drawing.Color.FromArgb(255,174,0),
										System.Drawing.Color.FromArgb(255,176,0),
										System.Drawing.Color.FromArgb(255,178,0),
										System.Drawing.Color.FromArgb(255,180,0),
										System.Drawing.Color.FromArgb(255,182,0),
										System.Drawing.Color.FromArgb(255,185,0),
										System.Drawing.Color.FromArgb(255,187,0),
										System.Drawing.Color.FromArgb(255,189,0),
										System.Drawing.Color.FromArgb(255,191,0),
										System.Drawing.Color.FromArgb(255,193,0),
										System.Drawing.Color.FromArgb(255,195,0),
										System.Drawing.Color.FromArgb(255,197,0),
										System.Drawing.Color.FromArgb(255,199,0),
										System.Drawing.Color.FromArgb(255,201,0),
										System.Drawing.Color.FromArgb(255,203,0),
										System.Drawing.Color.FromArgb(255,205,0),
										System.Drawing.Color.FromArgb(255,208,0),
										System.Drawing.Color.FromArgb(255,210,0),
										System.Drawing.Color.FromArgb(255,212,0),
										System.Drawing.Color.FromArgb(255,215,0),
										System.Drawing.Color.FromArgb(255,217,0),
										System.Drawing.Color.FromArgb(255,219,0),
										System.Drawing.Color.FromArgb(255,221,0),
										System.Drawing.Color.FromArgb(255,223,0),
										System.Drawing.Color.FromArgb(255,225,0),
										System.Drawing.Color.FromArgb(255,227,0),
										System.Drawing.Color.FromArgb(255,230,0),
										System.Drawing.Color.FromArgb(255,232,0),
										System.Drawing.Color.FromArgb(255,234,0),
										System.Drawing.Color.FromArgb(255,236,0),
										System.Drawing.Color.FromArgb(255,238,0),
										System.Drawing.Color.FromArgb(255,240,0),
										System.Drawing.Color.FromArgb(255,242,0),
										System.Drawing.Color.FromArgb(255,244,0),
										System.Drawing.Color.FromArgb(255,246,0),
										System.Drawing.Color.FromArgb(255,248,0),
										System.Drawing.Color.FromArgb(255,250,0),
										System.Drawing.Color.FromArgb(255,253,0),
										System.Drawing.Color.FromArgb(0,128,0),
										System.Drawing.Color.FromArgb(249,252,0),
										System.Drawing.Color.FromArgb(243,249,0),
										System.Drawing.Color.FromArgb(237,246,0),
										System.Drawing.Color.FromArgb(231,243,0),
										System.Drawing.Color.FromArgb(225,240,0),
										System.Drawing.Color.FromArgb(218,237,0),
										System.Drawing.Color.FromArgb(212,234,0),
										System.Drawing.Color.FromArgb(206,231,0),
										System.Drawing.Color.FromArgb(200,228,0),
										System.Drawing.Color.FromArgb(194,225,0),
										System.Drawing.Color.FromArgb(188,222,0),
										System.Drawing.Color.FromArgb(182,218,0),
										System.Drawing.Color.FromArgb(176,215,0),
										System.Drawing.Color.FromArgb(170,212,0),
										System.Drawing.Color.FromArgb(164,209,0),
										System.Drawing.Color.FromArgb(158,206,0),
										System.Drawing.Color.FromArgb(151,203,0),
										System.Drawing.Color.FromArgb(145,200,0),
										System.Drawing.Color.FromArgb(139,197,0),
										System.Drawing.Color.FromArgb(134,195,0),
										System.Drawing.Color.FromArgb(128,192,0),
										System.Drawing.Color.FromArgb(122,189,0),
										System.Drawing.Color.FromArgb(116,186,0),
										System.Drawing.Color.FromArgb(110,183,0),
										System.Drawing.Color.FromArgb(104,180,0),
										System.Drawing.Color.FromArgb(98,177,0),
										System.Drawing.Color.FromArgb(91,174,0),
										System.Drawing.Color.FromArgb(85,171,0),
										System.Drawing.Color.FromArgb(79,168,0),
										System.Drawing.Color.FromArgb(73,165,0),
										System.Drawing.Color.FromArgb(67,162,0),
										System.Drawing.Color.FromArgb(61,159,0),
										System.Drawing.Color.FromArgb(55,155,0),
										System.Drawing.Color.FromArgb(49,152,0),
										System.Drawing.Color.FromArgb(43,149,0),
										System.Drawing.Color.FromArgb(37,146,0),
										System.Drawing.Color.FromArgb(31,143,0),
										System.Drawing.Color.FromArgb(24,140,0),
										System.Drawing.Color.FromArgb(18,137,0),
										System.Drawing.Color.FromArgb(12,134,0),
										System.Drawing.Color.FromArgb(6,131,0),
										System.Drawing.Color.FromArgb(0,0,255),
										System.Drawing.Color.FromArgb(0,125,6),
										System.Drawing.Color.FromArgb(0,122,12),
										System.Drawing.Color.FromArgb(0,119,18),
										System.Drawing.Color.FromArgb(0,116,24),
										System.Drawing.Color.FromArgb(0,113,30),
										System.Drawing.Color.FromArgb(0,110,37),
										System.Drawing.Color.FromArgb(0,107,43),
										System.Drawing.Color.FromArgb(0,104,49),
										System.Drawing.Color.FromArgb(0,101,55),
										System.Drawing.Color.FromArgb(0,98,61),
										System.Drawing.Color.FromArgb(0,95,67),
										System.Drawing.Color.FromArgb(0,91,73),
										System.Drawing.Color.FromArgb(0,88,79),
										System.Drawing.Color.FromArgb(0,85,85),
										System.Drawing.Color.FromArgb(0,82,91),
										System.Drawing.Color.FromArgb(0,79,98),
										System.Drawing.Color.FromArgb(0,76,104),
										System.Drawing.Color.FromArgb(0,73,110),
										System.Drawing.Color.FromArgb(0,70,116),
										System.Drawing.Color.FromArgb(0,67,122),
										System.Drawing.Color.FromArgb(0,64,128),
										System.Drawing.Color.FromArgb(0,61,134),
										System.Drawing.Color.FromArgb(0,58,139),
										System.Drawing.Color.FromArgb(0,55,145),
										System.Drawing.Color.FromArgb(0,52,151),
										System.Drawing.Color.FromArgb(0,49,157),
										System.Drawing.Color.FromArgb(0,46,164),
										System.Drawing.Color.FromArgb(0,43,170),
										System.Drawing.Color.FromArgb(0,40,176),
										System.Drawing.Color.FromArgb(0,37,182),
										System.Drawing.Color.FromArgb(0,34,188),
										System.Drawing.Color.FromArgb(0,31,194),
										System.Drawing.Color.FromArgb(0,27,200),
										System.Drawing.Color.FromArgb(0,24,206),
										System.Drawing.Color.FromArgb(0,21,212),
										System.Drawing.Color.FromArgb(0,18,218),
										System.Drawing.Color.FromArgb(0,15,225),
										System.Drawing.Color.FromArgb(0,12,231),
										System.Drawing.Color.FromArgb(0,9,237),
										System.Drawing.Color.FromArgb(0,6,243),
										System.Drawing.Color.FromArgb(0,3,249),
										System.Drawing.Color.FromArgb(75,0,130),
										System.Drawing.Color.FromArgb(2,0,252),
										System.Drawing.Color.FromArgb(4,0,249),
										System.Drawing.Color.FromArgb(6,0,246),
										System.Drawing.Color.FromArgb(7,0,243),
										System.Drawing.Color.FromArgb(9,0,240),
										System.Drawing.Color.FromArgb(10,0,237),
										System.Drawing.Color.FromArgb(12,0,234),
										System.Drawing.Color.FromArgb(14,0,232),
										System.Drawing.Color.FromArgb(16,0,229),
										System.Drawing.Color.FromArgb(18,0,226),
										System.Drawing.Color.FromArgb(20,0,223),
										System.Drawing.Color.FromArgb(21,0,219),
										System.Drawing.Color.FromArgb(23,0,216),
										System.Drawing.Color.FromArgb(25,0,213),
										System.Drawing.Color.FromArgb(27,0,210),
										System.Drawing.Color.FromArgb(28,0,207),
										System.Drawing.Color.FromArgb(30,0,204),
										System.Drawing.Color.FromArgb(32,0,201),
										System.Drawing.Color.FromArgb(34,0,198),
										System.Drawing.Color.FromArgb(36,0,196),
										System.Drawing.Color.FromArgb(38,0,193),
										System.Drawing.Color.FromArgb(40,0,190),
										System.Drawing.Color.FromArgb(41,0,187),
										System.Drawing.Color.FromArgb(43,0,184),
										System.Drawing.Color.FromArgb(45,0,181),
										System.Drawing.Color.FromArgb(47,0,178),
										System.Drawing.Color.FromArgb(48,0,175),
										System.Drawing.Color.FromArgb(50,0,172),
										System.Drawing.Color.FromArgb(52,0,169),
										System.Drawing.Color.FromArgb(54,0,166),
										System.Drawing.Color.FromArgb(55,0,163),
										System.Drawing.Color.FromArgb(57,0,160),
										System.Drawing.Color.FromArgb(59,0,156),
										System.Drawing.Color.FromArgb(61,0,153),
										System.Drawing.Color.FromArgb(63,0,151),
										System.Drawing.Color.FromArgb(65,0,148),
										System.Drawing.Color.FromArgb(66,0,145),
										System.Drawing.Color.FromArgb(68,0,142),
										System.Drawing.Color.FromArgb(69,0,139),
										System.Drawing.Color.FromArgb(71,0,136),
										System.Drawing.Color.FromArgb(73,0,133),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(79,3,133),
										System.Drawing.Color.FromArgb(83,6,135),
										System.Drawing.Color.FromArgb(86,9,138),
										System.Drawing.Color.FromArgb(90,12,141),
										System.Drawing.Color.FromArgb(94,15,143),
										System.Drawing.Color.FromArgb(98,18,146),
										System.Drawing.Color.FromArgb(102,21,148),
										System.Drawing.Color.FromArgb(106,24,150),
										System.Drawing.Color.FromArgb(110,28,153),
										System.Drawing.Color.FromArgb(114,31,156),
										System.Drawing.Color.FromArgb(118,35,158),
										System.Drawing.Color.FromArgb(122,38,161),
										System.Drawing.Color.FromArgb(125,41,164),
										System.Drawing.Color.FromArgb(129,44,166),
										System.Drawing.Color.FromArgb(133,47,169),
										System.Drawing.Color.FromArgb(137,50,172),
										System.Drawing.Color.FromArgb(141,53,174),
										System.Drawing.Color.FromArgb(145,56,176),
										System.Drawing.Color.FromArgb(149,59,179),
										System.Drawing.Color.FromArgb(153,62,181),
										System.Drawing.Color.FromArgb(157,65,184),
										System.Drawing.Color.FromArgb(161,68,187),
										System.Drawing.Color.FromArgb(165,71,189),
										System.Drawing.Color.FromArgb(168,74,192),
										System.Drawing.Color.FromArgb(172,77,195),
										System.Drawing.Color.FromArgb(176,80,197),
										System.Drawing.Color.FromArgb(180,83,200),
										System.Drawing.Color.FromArgb(184,86,202),
										System.Drawing.Color.FromArgb(187,89,204),
										System.Drawing.Color.FromArgb(191,93,207),
										System.Drawing.Color.FromArgb(195,96,210),
										System.Drawing.Color.FromArgb(199,100,212),
										System.Drawing.Color.FromArgb(203,103,215),
										System.Drawing.Color.FromArgb(206,106,218),
										System.Drawing.Color.FromArgb(211,109,220),
										System.Drawing.Color.FromArgb(215,112,223),
										System.Drawing.Color.FromArgb(219,115,226),
										System.Drawing.Color.FromArgb(223,118,228),
										System.Drawing.Color.FromArgb(227,121,230),
										System.Drawing.Color.FromArgb(230,124,233),
										System.Drawing.Color.FromArgb(234,127,235),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(234,127,235),
										System.Drawing.Color.FromArgb(230,124,233),
										System.Drawing.Color.FromArgb(227,121,230),
										System.Drawing.Color.FromArgb(223,118,228),
										System.Drawing.Color.FromArgb(219,115,226),
										System.Drawing.Color.FromArgb(215,112,223),
										System.Drawing.Color.FromArgb(211,109,220),
										System.Drawing.Color.FromArgb(206,106,218),
										System.Drawing.Color.FromArgb(203,103,215),
										System.Drawing.Color.FromArgb(199,100,212),
										System.Drawing.Color.FromArgb(195,96,210),
										System.Drawing.Color.FromArgb(191,93,207),
										System.Drawing.Color.FromArgb(187,89,204),
										System.Drawing.Color.FromArgb(184,86,202),
										System.Drawing.Color.FromArgb(180,83,200),
										System.Drawing.Color.FromArgb(176,80,197),
										System.Drawing.Color.FromArgb(172,77,195),
										System.Drawing.Color.FromArgb(168,74,192),
										System.Drawing.Color.FromArgb(165,71,189),
										System.Drawing.Color.FromArgb(161,68,187),
										System.Drawing.Color.FromArgb(157,65,184),
										System.Drawing.Color.FromArgb(153,62,181),
										System.Drawing.Color.FromArgb(149,59,179),
										System.Drawing.Color.FromArgb(145,56,176),
										System.Drawing.Color.FromArgb(141,53,174),
										System.Drawing.Color.FromArgb(137,50,172),
										System.Drawing.Color.FromArgb(133,47,169),
										System.Drawing.Color.FromArgb(129,44,166),
										System.Drawing.Color.FromArgb(125,41,164),
										System.Drawing.Color.FromArgb(122,38,161),
										System.Drawing.Color.FromArgb(118,35,158),
										System.Drawing.Color.FromArgb(114,31,156),
										System.Drawing.Color.FromArgb(110,28,153),
										System.Drawing.Color.FromArgb(106,24,150),
										System.Drawing.Color.FromArgb(102,21,148),
										System.Drawing.Color.FromArgb(98,18,146),
										System.Drawing.Color.FromArgb(94,15,143),
										System.Drawing.Color.FromArgb(90,12,141),
										System.Drawing.Color.FromArgb(86,9,138),
										System.Drawing.Color.FromArgb(83,6,135),
										System.Drawing.Color.FromArgb(79,3,133),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(73,0,133),
										System.Drawing.Color.FromArgb(71,0,136),
										System.Drawing.Color.FromArgb(69,0,139),
										System.Drawing.Color.FromArgb(68,0,142),
										System.Drawing.Color.FromArgb(66,0,145),
										System.Drawing.Color.FromArgb(65,0,148),
										System.Drawing.Color.FromArgb(63,0,151),
										System.Drawing.Color.FromArgb(61,0,153),
										System.Drawing.Color.FromArgb(59,0,156),
										System.Drawing.Color.FromArgb(57,0,160),
										System.Drawing.Color.FromArgb(55,0,163),
										System.Drawing.Color.FromArgb(54,0,166),
										System.Drawing.Color.FromArgb(52,0,169),
										System.Drawing.Color.FromArgb(50,0,172),
										System.Drawing.Color.FromArgb(48,0,175),
										System.Drawing.Color.FromArgb(47,0,178),
										System.Drawing.Color.FromArgb(45,0,181),
										System.Drawing.Color.FromArgb(43,0,184),
										System.Drawing.Color.FromArgb(41,0,187),
										System.Drawing.Color.FromArgb(40,0,190),
										System.Drawing.Color.FromArgb(38,0,193),
										System.Drawing.Color.FromArgb(36,0,196),
										System.Drawing.Color.FromArgb(34,0,198),
										System.Drawing.Color.FromArgb(32,0,201),
										System.Drawing.Color.FromArgb(30,0,204),
										System.Drawing.Color.FromArgb(28,0,207),
										System.Drawing.Color.FromArgb(27,0,210),
										System.Drawing.Color.FromArgb(25,0,213),
										System.Drawing.Color.FromArgb(23,0,216),
										System.Drawing.Color.FromArgb(21,0,219),
										System.Drawing.Color.FromArgb(20,0,223),
										System.Drawing.Color.FromArgb(18,0,226),
										System.Drawing.Color.FromArgb(16,0,229),
										System.Drawing.Color.FromArgb(14,0,232),
										System.Drawing.Color.FromArgb(12,0,234),
										System.Drawing.Color.FromArgb(10,0,237),
										System.Drawing.Color.FromArgb(9,0,240),
										System.Drawing.Color.FromArgb(7,0,243),
										System.Drawing.Color.FromArgb(6,0,246),
										System.Drawing.Color.FromArgb(4,0,249),
										System.Drawing.Color.FromArgb(2,0,252),
										System.Drawing.Color.FromArgb(75,0,130),
										System.Drawing.Color.FromArgb(0,3,249),
										System.Drawing.Color.FromArgb(0,6,243),
										System.Drawing.Color.FromArgb(0,9,237),
										System.Drawing.Color.FromArgb(0,12,231),
										System.Drawing.Color.FromArgb(0,15,225),
										System.Drawing.Color.FromArgb(0,18,218),
										System.Drawing.Color.FromArgb(0,21,212),
										System.Drawing.Color.FromArgb(0,24,206),
										System.Drawing.Color.FromArgb(0,27,200),
										System.Drawing.Color.FromArgb(0,31,194),
										System.Drawing.Color.FromArgb(0,34,188),
										System.Drawing.Color.FromArgb(0,37,182),
										System.Drawing.Color.FromArgb(0,40,176),
										System.Drawing.Color.FromArgb(0,43,170),
										System.Drawing.Color.FromArgb(0,46,164),
										System.Drawing.Color.FromArgb(0,49,157),
										System.Drawing.Color.FromArgb(0,52,151),
										System.Drawing.Color.FromArgb(0,55,145),
										System.Drawing.Color.FromArgb(0,58,139),
										System.Drawing.Color.FromArgb(0,61,134),
										System.Drawing.Color.FromArgb(0,64,128),
										System.Drawing.Color.FromArgb(0,67,122),
										System.Drawing.Color.FromArgb(0,70,116),
										System.Drawing.Color.FromArgb(0,73,110),
										System.Drawing.Color.FromArgb(0,76,104),
										System.Drawing.Color.FromArgb(0,79,98),
										System.Drawing.Color.FromArgb(0,82,91),
										System.Drawing.Color.FromArgb(0,85,85),
										System.Drawing.Color.FromArgb(0,88,79),
										System.Drawing.Color.FromArgb(0,91,73),
										System.Drawing.Color.FromArgb(0,95,67),
										System.Drawing.Color.FromArgb(0,98,61),
										System.Drawing.Color.FromArgb(0,101,55),
										System.Drawing.Color.FromArgb(0,104,49),
										System.Drawing.Color.FromArgb(0,107,43),
										System.Drawing.Color.FromArgb(0,110,37),
										System.Drawing.Color.FromArgb(0,113,30),
										System.Drawing.Color.FromArgb(0,116,24),
										System.Drawing.Color.FromArgb(0,119,18),
										System.Drawing.Color.FromArgb(0,122,12),
										System.Drawing.Color.FromArgb(0,125,6),
										System.Drawing.Color.FromArgb(0,0,255),
										System.Drawing.Color.FromArgb(6,131,0),
										System.Drawing.Color.FromArgb(12,134,0),
										System.Drawing.Color.FromArgb(18,137,0),
										System.Drawing.Color.FromArgb(24,140,0),
										System.Drawing.Color.FromArgb(31,143,0),
										System.Drawing.Color.FromArgb(37,146,0),
										System.Drawing.Color.FromArgb(43,149,0),
										System.Drawing.Color.FromArgb(49,152,0),
										System.Drawing.Color.FromArgb(55,155,0),
										System.Drawing.Color.FromArgb(61,159,0),
										System.Drawing.Color.FromArgb(67,162,0),
										System.Drawing.Color.FromArgb(73,165,0),
										System.Drawing.Color.FromArgb(79,168,0),
										System.Drawing.Color.FromArgb(85,171,0),
										System.Drawing.Color.FromArgb(91,174,0),
										System.Drawing.Color.FromArgb(98,177,0),
										System.Drawing.Color.FromArgb(104,180,0),
										System.Drawing.Color.FromArgb(110,183,0),
										System.Drawing.Color.FromArgb(116,186,0),
										System.Drawing.Color.FromArgb(122,189,0),
										System.Drawing.Color.FromArgb(128,192,0),
										System.Drawing.Color.FromArgb(134,195,0),
										System.Drawing.Color.FromArgb(139,197,0),
										System.Drawing.Color.FromArgb(145,200,0),
										System.Drawing.Color.FromArgb(151,203,0),
										System.Drawing.Color.FromArgb(158,206,0),
										System.Drawing.Color.FromArgb(164,209,0),
										System.Drawing.Color.FromArgb(170,212,0),
										System.Drawing.Color.FromArgb(176,215,0),
										System.Drawing.Color.FromArgb(182,218,0),
										System.Drawing.Color.FromArgb(188,222,0),
										System.Drawing.Color.FromArgb(194,225,0),
										System.Drawing.Color.FromArgb(200,228,0),
										System.Drawing.Color.FromArgb(206,231,0),
										System.Drawing.Color.FromArgb(212,234,0),
										System.Drawing.Color.FromArgb(218,237,0),
										System.Drawing.Color.FromArgb(225,240,0),
										System.Drawing.Color.FromArgb(231,243,0),
										System.Drawing.Color.FromArgb(237,246,0),
										System.Drawing.Color.FromArgb(243,249,0),
										System.Drawing.Color.FromArgb(249,252,0),
										System.Drawing.Color.FromArgb(0,128,0),
										System.Drawing.Color.FromArgb(255,253,0),
										System.Drawing.Color.FromArgb(255,250,0),
										System.Drawing.Color.FromArgb(255,248,0),
										System.Drawing.Color.FromArgb(255,246,0),
										System.Drawing.Color.FromArgb(255,244,0),
										System.Drawing.Color.FromArgb(255,242,0),
										System.Drawing.Color.FromArgb(255,240,0),
										System.Drawing.Color.FromArgb(255,238,0),
										System.Drawing.Color.FromArgb(255,236,0),
										System.Drawing.Color.FromArgb(255,234,0),
										System.Drawing.Color.FromArgb(255,232,0),
										System.Drawing.Color.FromArgb(255,230,0),
										System.Drawing.Color.FromArgb(255,227,0),
										System.Drawing.Color.FromArgb(255,225,0),
										System.Drawing.Color.FromArgb(255,223,0),
										System.Drawing.Color.FromArgb(255,221,0),
										System.Drawing.Color.FromArgb(255,219,0),
										System.Drawing.Color.FromArgb(255,217,0),
										System.Drawing.Color.FromArgb(255,215,0),
										System.Drawing.Color.FromArgb(255,212,0),
										System.Drawing.Color.FromArgb(255,210,0),
										System.Drawing.Color.FromArgb(255,208,0),
										System.Drawing.Color.FromArgb(255,205,0),
										System.Drawing.Color.FromArgb(255,203,0),
										System.Drawing.Color.FromArgb(255,201,0),
										System.Drawing.Color.FromArgb(255,199,0),
										System.Drawing.Color.FromArgb(255,197,0),
										System.Drawing.Color.FromArgb(255,195,0),
										System.Drawing.Color.FromArgb(255,193,0),
										System.Drawing.Color.FromArgb(255,191,0),
										System.Drawing.Color.FromArgb(255,189,0),
										System.Drawing.Color.FromArgb(255,187,0),
										System.Drawing.Color.FromArgb(255,185,0),
										System.Drawing.Color.FromArgb(255,182,0),
										System.Drawing.Color.FromArgb(255,180,0),
										System.Drawing.Color.FromArgb(255,178,0),
										System.Drawing.Color.FromArgb(255,176,0),
										System.Drawing.Color.FromArgb(255,174,0),
										System.Drawing.Color.FromArgb(255,172,0),
										System.Drawing.Color.FromArgb(255,170,0),
										System.Drawing.Color.FromArgb(255,167,0),
										System.Drawing.Color.FromArgb(255,255,0),
										System.Drawing.Color.FromArgb(255,161,0),
										System.Drawing.Color.FromArgb(255,157,0),
										System.Drawing.Color.FromArgb(255,153,0),
										System.Drawing.Color.FromArgb(255,149,0),
										System.Drawing.Color.FromArgb(255,145,0),
										System.Drawing.Color.FromArgb(255,141,0),
										System.Drawing.Color.FromArgb(255,137,0),
										System.Drawing.Color.FromArgb(255,133,0),
										System.Drawing.Color.FromArgb(255,130,0),
										System.Drawing.Color.FromArgb(255,126,0),
										System.Drawing.Color.FromArgb(255,122,0),
										System.Drawing.Color.FromArgb(255,118,0),
										System.Drawing.Color.FromArgb(255,114,0),
										System.Drawing.Color.FromArgb(255,110,0),
										System.Drawing.Color.FromArgb(255,106,0),
										System.Drawing.Color.FromArgb(255,102,0),
										System.Drawing.Color.FromArgb(255,98,0),
										System.Drawing.Color.FromArgb(255,94,0),
										System.Drawing.Color.FromArgb(255,91,0),
										System.Drawing.Color.FromArgb(255,87,0),
										System.Drawing.Color.FromArgb(255,83,0),
										System.Drawing.Color.FromArgb(255,79,0),
										System.Drawing.Color.FromArgb(255,75,0),
										System.Drawing.Color.FromArgb(255,71,0),
										System.Drawing.Color.FromArgb(255,67,0),
										System.Drawing.Color.FromArgb(255,63,0),
										System.Drawing.Color.FromArgb(255,59,0),
										System.Drawing.Color.FromArgb(255,55,0),
										System.Drawing.Color.FromArgb(255,51,0),
										System.Drawing.Color.FromArgb(255,47,0),
										System.Drawing.Color.FromArgb(255,43,0),
										System.Drawing.Color.FromArgb(255,39,0),
										System.Drawing.Color.FromArgb(255,35,0),
										System.Drawing.Color.FromArgb(255,31,0),
										System.Drawing.Color.FromArgb(255,28,0),
										System.Drawing.Color.FromArgb(255,24,0),
										System.Drawing.Color.FromArgb(255,20,0),
										System.Drawing.Color.FromArgb(255,16,0),
										System.Drawing.Color.FromArgb(255,12,0),
										System.Drawing.Color.FromArgb(255,8,0),
										System.Drawing.Color.FromArgb(255,4,0),
										System.Drawing.Color.FromArgb(255,0,0),
										System.Drawing.Color.FromArgb(255,4,0),
										System.Drawing.Color.FromArgb(255,8,0),
										System.Drawing.Color.FromArgb(255,12,0),
										System.Drawing.Color.FromArgb(255,16,0),
										System.Drawing.Color.FromArgb(255,20,0),
										System.Drawing.Color.FromArgb(255,24,0),
										System.Drawing.Color.FromArgb(255,28,0),
										System.Drawing.Color.FromArgb(255,31,0),
										System.Drawing.Color.FromArgb(255,35,0),
										System.Drawing.Color.FromArgb(255,39,0),
										System.Drawing.Color.FromArgb(255,43,0),
										System.Drawing.Color.FromArgb(255,47,0),
										System.Drawing.Color.FromArgb(255,51,0),
										System.Drawing.Color.FromArgb(255,55,0),
										System.Drawing.Color.FromArgb(255,59,0),
										System.Drawing.Color.FromArgb(255,63,0),
										System.Drawing.Color.FromArgb(255,67,0),
										System.Drawing.Color.FromArgb(255,71,0),
										System.Drawing.Color.FromArgb(255,75,0),
										System.Drawing.Color.FromArgb(255,79,0),
										System.Drawing.Color.FromArgb(255,83,0),
										System.Drawing.Color.FromArgb(255,87,0),
										System.Drawing.Color.FromArgb(255,91,0),
										System.Drawing.Color.FromArgb(255,94,0),
										System.Drawing.Color.FromArgb(255,98,0),
										System.Drawing.Color.FromArgb(255,102,0),
										System.Drawing.Color.FromArgb(255,106,0),
										System.Drawing.Color.FromArgb(255,110,0),
										System.Drawing.Color.FromArgb(255,114,0),
										System.Drawing.Color.FromArgb(255,118,0),
										System.Drawing.Color.FromArgb(255,122,0),
										System.Drawing.Color.FromArgb(255,126,0),
										System.Drawing.Color.FromArgb(255,130,0),
										System.Drawing.Color.FromArgb(255,133,0),
										System.Drawing.Color.FromArgb(255,137,0),
										System.Drawing.Color.FromArgb(255,141,0),
										System.Drawing.Color.FromArgb(255,145,0),
										System.Drawing.Color.FromArgb(255,149,0),
										System.Drawing.Color.FromArgb(255,153,0),
										System.Drawing.Color.FromArgb(255,157,0),
										System.Drawing.Color.FromArgb(255,161,0),
										System.Drawing.Color.FromArgb(255,255,0),
										System.Drawing.Color.FromArgb(255,167,0),
										System.Drawing.Color.FromArgb(255,170,0),
										System.Drawing.Color.FromArgb(255,172,0),
										System.Drawing.Color.FromArgb(255,174,0),
										System.Drawing.Color.FromArgb(255,176,0),
										System.Drawing.Color.FromArgb(255,178,0),
										System.Drawing.Color.FromArgb(255,180,0),
										System.Drawing.Color.FromArgb(255,182,0),
										System.Drawing.Color.FromArgb(255,185,0),
										System.Drawing.Color.FromArgb(255,187,0),
										System.Drawing.Color.FromArgb(255,189,0),
										System.Drawing.Color.FromArgb(255,191,0),
										System.Drawing.Color.FromArgb(255,193,0),
										System.Drawing.Color.FromArgb(255,195,0),
										System.Drawing.Color.FromArgb(255,197,0),
										System.Drawing.Color.FromArgb(255,199,0),
										System.Drawing.Color.FromArgb(255,201,0),
										System.Drawing.Color.FromArgb(255,203,0),
										System.Drawing.Color.FromArgb(255,205,0),
										System.Drawing.Color.FromArgb(255,208,0),
										System.Drawing.Color.FromArgb(255,210,0),
										System.Drawing.Color.FromArgb(255,212,0),
										System.Drawing.Color.FromArgb(255,215,0),
										System.Drawing.Color.FromArgb(255,217,0),
										System.Drawing.Color.FromArgb(255,219,0),
										System.Drawing.Color.FromArgb(255,221,0),
										System.Drawing.Color.FromArgb(255,223,0),
										System.Drawing.Color.FromArgb(255,225,0),
										System.Drawing.Color.FromArgb(255,227,0),
										System.Drawing.Color.FromArgb(255,230,0),
										System.Drawing.Color.FromArgb(255,232,0),
										System.Drawing.Color.FromArgb(255,234,0),
										System.Drawing.Color.FromArgb(255,236,0),
										System.Drawing.Color.FromArgb(255,238,0),
										System.Drawing.Color.FromArgb(255,240,0),
										System.Drawing.Color.FromArgb(255,242,0),
										System.Drawing.Color.FromArgb(255,244,0),
										System.Drawing.Color.FromArgb(255,246,0),
										System.Drawing.Color.FromArgb(255,248,0),
										System.Drawing.Color.FromArgb(255,250,0),
										System.Drawing.Color.FromArgb(255,253,0),
										System.Drawing.Color.FromArgb(0,128,0),
										System.Drawing.Color.FromArgb(249,252,0),
										System.Drawing.Color.FromArgb(243,249,0),
										System.Drawing.Color.FromArgb(237,246,0),
										System.Drawing.Color.FromArgb(231,243,0),
										System.Drawing.Color.FromArgb(225,240,0),
										System.Drawing.Color.FromArgb(218,237,0),
										System.Drawing.Color.FromArgb(212,234,0),
										System.Drawing.Color.FromArgb(206,231,0),
										System.Drawing.Color.FromArgb(200,228,0),
										System.Drawing.Color.FromArgb(194,225,0),
										System.Drawing.Color.FromArgb(188,222,0),
										System.Drawing.Color.FromArgb(182,218,0),
										System.Drawing.Color.FromArgb(176,215,0),
										System.Drawing.Color.FromArgb(170,212,0),
										System.Drawing.Color.FromArgb(164,209,0),
										System.Drawing.Color.FromArgb(158,206,0),
										System.Drawing.Color.FromArgb(151,203,0),
										System.Drawing.Color.FromArgb(145,200,0),
										System.Drawing.Color.FromArgb(139,197,0),
										System.Drawing.Color.FromArgb(134,195,0),
										System.Drawing.Color.FromArgb(128,192,0),
										System.Drawing.Color.FromArgb(122,189,0),
										System.Drawing.Color.FromArgb(116,186,0),
										System.Drawing.Color.FromArgb(110,183,0),
										System.Drawing.Color.FromArgb(104,180,0),
										System.Drawing.Color.FromArgb(98,177,0),
										System.Drawing.Color.FromArgb(91,174,0),
										System.Drawing.Color.FromArgb(85,171,0),
										System.Drawing.Color.FromArgb(79,168,0),
										System.Drawing.Color.FromArgb(73,165,0),
										System.Drawing.Color.FromArgb(67,162,0),
										System.Drawing.Color.FromArgb(61,159,0),
										System.Drawing.Color.FromArgb(55,155,0),
										System.Drawing.Color.FromArgb(49,152,0),
										System.Drawing.Color.FromArgb(43,149,0),
										System.Drawing.Color.FromArgb(37,146,0),
										System.Drawing.Color.FromArgb(31,143,0),
										System.Drawing.Color.FromArgb(24,140,0),
										System.Drawing.Color.FromArgb(18,137,0),
										System.Drawing.Color.FromArgb(12,134,0),
										System.Drawing.Color.FromArgb(6,131,0),
										System.Drawing.Color.FromArgb(0,0,255),
										System.Drawing.Color.FromArgb(0,125,6),
										System.Drawing.Color.FromArgb(0,122,12),
										System.Drawing.Color.FromArgb(0,119,18),
										System.Drawing.Color.FromArgb(0,116,24),
										System.Drawing.Color.FromArgb(0,113,30),
										System.Drawing.Color.FromArgb(0,110,37),
										System.Drawing.Color.FromArgb(0,107,43),
										System.Drawing.Color.FromArgb(0,104,49),
										System.Drawing.Color.FromArgb(0,101,55),
										System.Drawing.Color.FromArgb(0,98,61),
										System.Drawing.Color.FromArgb(0,95,67),
										System.Drawing.Color.FromArgb(0,91,73),
										System.Drawing.Color.FromArgb(0,88,79),
										System.Drawing.Color.FromArgb(0,85,85),
										System.Drawing.Color.FromArgb(0,82,91),
										System.Drawing.Color.FromArgb(0,79,98),
										System.Drawing.Color.FromArgb(0,76,104),
										System.Drawing.Color.FromArgb(0,73,110),
										System.Drawing.Color.FromArgb(0,70,116),
										System.Drawing.Color.FromArgb(0,67,122),
										System.Drawing.Color.FromArgb(0,64,128),
										System.Drawing.Color.FromArgb(0,61,134),
										System.Drawing.Color.FromArgb(0,58,139),
										System.Drawing.Color.FromArgb(0,55,145),
										System.Drawing.Color.FromArgb(0,52,151),
										System.Drawing.Color.FromArgb(0,49,157),
										System.Drawing.Color.FromArgb(0,46,164),
										System.Drawing.Color.FromArgb(0,43,170),
										System.Drawing.Color.FromArgb(0,40,176),
										System.Drawing.Color.FromArgb(0,37,182),
										System.Drawing.Color.FromArgb(0,34,188),
										System.Drawing.Color.FromArgb(0,31,194),
										System.Drawing.Color.FromArgb(0,27,200),
										System.Drawing.Color.FromArgb(0,24,206),
										System.Drawing.Color.FromArgb(0,21,212),
										System.Drawing.Color.FromArgb(0,18,218),
										System.Drawing.Color.FromArgb(0,15,225),
										System.Drawing.Color.FromArgb(0,12,231),
										System.Drawing.Color.FromArgb(0,9,237),
										System.Drawing.Color.FromArgb(0,6,243),
										System.Drawing.Color.FromArgb(0,3,249),
										System.Drawing.Color.FromArgb(75,0,130),
										System.Drawing.Color.FromArgb(2,0,252),
										System.Drawing.Color.FromArgb(4,0,249),
										System.Drawing.Color.FromArgb(6,0,246),
										System.Drawing.Color.FromArgb(7,0,243),
										System.Drawing.Color.FromArgb(9,0,240),
										System.Drawing.Color.FromArgb(10,0,237),
										System.Drawing.Color.FromArgb(12,0,234),
										System.Drawing.Color.FromArgb(14,0,232),
										System.Drawing.Color.FromArgb(16,0,229),
										System.Drawing.Color.FromArgb(18,0,226),
										System.Drawing.Color.FromArgb(20,0,223),
										System.Drawing.Color.FromArgb(21,0,219),
										System.Drawing.Color.FromArgb(23,0,216),
										System.Drawing.Color.FromArgb(25,0,213),
										System.Drawing.Color.FromArgb(27,0,210),
										System.Drawing.Color.FromArgb(28,0,207),
										System.Drawing.Color.FromArgb(30,0,204),
										System.Drawing.Color.FromArgb(32,0,201),
										System.Drawing.Color.FromArgb(34,0,198),
										System.Drawing.Color.FromArgb(36,0,196),
										System.Drawing.Color.FromArgb(38,0,193),
										System.Drawing.Color.FromArgb(40,0,190),
										System.Drawing.Color.FromArgb(41,0,187),
										System.Drawing.Color.FromArgb(43,0,184),
										System.Drawing.Color.FromArgb(45,0,181),
										System.Drawing.Color.FromArgb(47,0,178),
										System.Drawing.Color.FromArgb(48,0,175),
										System.Drawing.Color.FromArgb(50,0,172),
										System.Drawing.Color.FromArgb(52,0,169),
										System.Drawing.Color.FromArgb(54,0,166),
										System.Drawing.Color.FromArgb(55,0,163),
										System.Drawing.Color.FromArgb(57,0,160),
										System.Drawing.Color.FromArgb(59,0,156),
										System.Drawing.Color.FromArgb(61,0,153),
										System.Drawing.Color.FromArgb(63,0,151),
										System.Drawing.Color.FromArgb(65,0,148),
										System.Drawing.Color.FromArgb(66,0,145),
										System.Drawing.Color.FromArgb(68,0,142),
										System.Drawing.Color.FromArgb(69,0,139),
										System.Drawing.Color.FromArgb(71,0,136),
										System.Drawing.Color.FromArgb(73,0,133),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(79,3,133),
										System.Drawing.Color.FromArgb(83,6,135),
										System.Drawing.Color.FromArgb(86,9,138),
										System.Drawing.Color.FromArgb(90,12,141),
										System.Drawing.Color.FromArgb(94,15,143),
										System.Drawing.Color.FromArgb(98,18,146),
										System.Drawing.Color.FromArgb(102,21,148),
										System.Drawing.Color.FromArgb(106,24,150),
										System.Drawing.Color.FromArgb(110,28,153),
										System.Drawing.Color.FromArgb(114,31,156),
										System.Drawing.Color.FromArgb(118,35,158),
										System.Drawing.Color.FromArgb(122,38,161),
										System.Drawing.Color.FromArgb(125,41,164),
										System.Drawing.Color.FromArgb(129,44,166),
										System.Drawing.Color.FromArgb(133,47,169),
										System.Drawing.Color.FromArgb(137,50,172),
										System.Drawing.Color.FromArgb(141,53,174),
										System.Drawing.Color.FromArgb(145,56,176),
										System.Drawing.Color.FromArgb(149,59,179),
										System.Drawing.Color.FromArgb(153,62,181),
										System.Drawing.Color.FromArgb(157,65,184),
										System.Drawing.Color.FromArgb(161,68,187),
										System.Drawing.Color.FromArgb(165,71,189),
										System.Drawing.Color.FromArgb(168,74,192),
										System.Drawing.Color.FromArgb(172,77,195),
										System.Drawing.Color.FromArgb(176,80,197),
										System.Drawing.Color.FromArgb(180,83,200),
										System.Drawing.Color.FromArgb(184,86,202),
										System.Drawing.Color.FromArgb(187,89,204),
										System.Drawing.Color.FromArgb(191,93,207),
										System.Drawing.Color.FromArgb(195,96,210),
										System.Drawing.Color.FromArgb(199,100,212),
										System.Drawing.Color.FromArgb(203,103,215),
										System.Drawing.Color.FromArgb(206,106,218),
										System.Drawing.Color.FromArgb(211,109,220),
										System.Drawing.Color.FromArgb(215,112,223),
										System.Drawing.Color.FromArgb(219,115,226),
										System.Drawing.Color.FromArgb(223,118,228),
										System.Drawing.Color.FromArgb(227,121,230),
										System.Drawing.Color.FromArgb(230,124,233),
										System.Drawing.Color.FromArgb(234,127,235),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(234,127,235),
										System.Drawing.Color.FromArgb(230,124,233),
										System.Drawing.Color.FromArgb(227,121,230),
										System.Drawing.Color.FromArgb(223,118,228),
										System.Drawing.Color.FromArgb(219,115,226),
										System.Drawing.Color.FromArgb(215,112,223),
										System.Drawing.Color.FromArgb(211,109,220),
										System.Drawing.Color.FromArgb(206,106,218),
										System.Drawing.Color.FromArgb(203,103,215),
										System.Drawing.Color.FromArgb(199,100,212),
										System.Drawing.Color.FromArgb(195,96,210),
										System.Drawing.Color.FromArgb(191,93,207),
										System.Drawing.Color.FromArgb(187,89,204),
										System.Drawing.Color.FromArgb(184,86,202),
										System.Drawing.Color.FromArgb(180,83,200),
										System.Drawing.Color.FromArgb(176,80,197),
										System.Drawing.Color.FromArgb(172,77,195),
										System.Drawing.Color.FromArgb(168,74,192),
										System.Drawing.Color.FromArgb(165,71,189),
										System.Drawing.Color.FromArgb(161,68,187),
										System.Drawing.Color.FromArgb(157,65,184),
										System.Drawing.Color.FromArgb(153,62,181),
										System.Drawing.Color.FromArgb(149,59,179),
										System.Drawing.Color.FromArgb(145,56,176),
										System.Drawing.Color.FromArgb(141,53,174),
										System.Drawing.Color.FromArgb(137,50,172),
										System.Drawing.Color.FromArgb(133,47,169),
										System.Drawing.Color.FromArgb(129,44,166),
										System.Drawing.Color.FromArgb(125,41,164),
										System.Drawing.Color.FromArgb(122,38,161),
										System.Drawing.Color.FromArgb(118,35,158),
										System.Drawing.Color.FromArgb(114,31,156),
										System.Drawing.Color.FromArgb(110,28,153),
										System.Drawing.Color.FromArgb(106,24,150),
										System.Drawing.Color.FromArgb(102,21,148),
										System.Drawing.Color.FromArgb(98,18,146),
										System.Drawing.Color.FromArgb(94,15,143),
										System.Drawing.Color.FromArgb(90,12,141),
										System.Drawing.Color.FromArgb(86,9,138),
										System.Drawing.Color.FromArgb(83,6,135),
										System.Drawing.Color.FromArgb(79,3,133),
										System.Drawing.Color.FromArgb(238,130,238),
										System.Drawing.Color.FromArgb(73,0,133),
										System.Drawing.Color.FromArgb(71,0,136),
										System.Drawing.Color.FromArgb(69,0,139),
										System.Drawing.Color.FromArgb(68,0,142),
										System.Drawing.Color.FromArgb(66,0,145),
										System.Drawing.Color.FromArgb(65,0,148),
										System.Drawing.Color.FromArgb(63,0,151),
										System.Drawing.Color.FromArgb(61,0,153),
										System.Drawing.Color.FromArgb(59,0,156),
										System.Drawing.Color.FromArgb(57,0,160),
										System.Drawing.Color.FromArgb(55,0,163),
										System.Drawing.Color.FromArgb(54,0,166),
										System.Drawing.Color.FromArgb(52,0,169),
										System.Drawing.Color.FromArgb(50,0,172),
										System.Drawing.Color.FromArgb(48,0,175),
										System.Drawing.Color.FromArgb(47,0,178),
										System.Drawing.Color.FromArgb(45,0,181),
										System.Drawing.Color.FromArgb(43,0,184),
										System.Drawing.Color.FromArgb(41,0,187),
										System.Drawing.Color.FromArgb(40,0,190),
										System.Drawing.Color.FromArgb(38,0,193),
										System.Drawing.Color.FromArgb(36,0,196),
										System.Drawing.Color.FromArgb(34,0,198),
										System.Drawing.Color.FromArgb(32,0,201),
										System.Drawing.Color.FromArgb(30,0,204),
										System.Drawing.Color.FromArgb(28,0,207),
										System.Drawing.Color.FromArgb(27,0,210),
										System.Drawing.Color.FromArgb(25,0,213),
										System.Drawing.Color.FromArgb(23,0,216),
										System.Drawing.Color.FromArgb(21,0,219),
										System.Drawing.Color.FromArgb(20,0,223),
										System.Drawing.Color.FromArgb(18,0,226),
										System.Drawing.Color.FromArgb(16,0,229),
										System.Drawing.Color.FromArgb(14,0,232),
										System.Drawing.Color.FromArgb(12,0,234),
										System.Drawing.Color.FromArgb(10,0,237),
										System.Drawing.Color.FromArgb(9,0,240),
										System.Drawing.Color.FromArgb(7,0,243),
										System.Drawing.Color.FromArgb(6,0,246),
										System.Drawing.Color.FromArgb(4,0,249),
										System.Drawing.Color.FromArgb(2,0,252),
										System.Drawing.Color.FromArgb(75,0,130),
										System.Drawing.Color.FromArgb(0,3,249),
										System.Drawing.Color.FromArgb(0,6,243),
										System.Drawing.Color.FromArgb(0,9,237),
										System.Drawing.Color.FromArgb(0,12,231),
										System.Drawing.Color.FromArgb(0,15,225),
										System.Drawing.Color.FromArgb(0,18,218),
										System.Drawing.Color.FromArgb(0,21,212),
										System.Drawing.Color.FromArgb(0,24,206),
										System.Drawing.Color.FromArgb(0,27,200),
										System.Drawing.Color.FromArgb(0,31,194),
										System.Drawing.Color.FromArgb(0,34,188),
										System.Drawing.Color.FromArgb(0,37,182),
										System.Drawing.Color.FromArgb(0,40,176),
										System.Drawing.Color.FromArgb(0,43,170),
										System.Drawing.Color.FromArgb(0,46,164),
										System.Drawing.Color.FromArgb(0,49,157),
										System.Drawing.Color.FromArgb(0,52,151),
										System.Drawing.Color.FromArgb(0,55,145),
										System.Drawing.Color.FromArgb(0,58,139),
										System.Drawing.Color.FromArgb(0,61,134),
										System.Drawing.Color.FromArgb(0,64,128),
										System.Drawing.Color.FromArgb(0,67,122),
										System.Drawing.Color.FromArgb(0,70,116),
										System.Drawing.Color.FromArgb(0,73,110),
										System.Drawing.Color.FromArgb(0,76,104),
										System.Drawing.Color.FromArgb(0,79,98),
										System.Drawing.Color.FromArgb(0,82,91),
										System.Drawing.Color.FromArgb(0,85,85),
										System.Drawing.Color.FromArgb(0,88,79),
										System.Drawing.Color.FromArgb(0,91,73),
										System.Drawing.Color.FromArgb(0,95,67),
										System.Drawing.Color.FromArgb(0,98,61),
										System.Drawing.Color.FromArgb(0,101,55),
										System.Drawing.Color.FromArgb(0,104,49),
										System.Drawing.Color.FromArgb(0,107,43),
										System.Drawing.Color.FromArgb(0,110,37),
										System.Drawing.Color.FromArgb(0,113,30),
										System.Drawing.Color.FromArgb(0,116,24),
										System.Drawing.Color.FromArgb(0,119,18),
										System.Drawing.Color.FromArgb(0,122,12),
										System.Drawing.Color.FromArgb(0,125,6),
										System.Drawing.Color.FromArgb(0,0,255),
										System.Drawing.Color.FromArgb(6,131,0),
										System.Drawing.Color.FromArgb(12,134,0),
										System.Drawing.Color.FromArgb(18,137,0),
										System.Drawing.Color.FromArgb(24,140,0),
										System.Drawing.Color.FromArgb(31,143,0),
										System.Drawing.Color.FromArgb(37,146,0),
										System.Drawing.Color.FromArgb(43,149,0),
										System.Drawing.Color.FromArgb(49,152,0),
										System.Drawing.Color.FromArgb(55,155,0),
										System.Drawing.Color.FromArgb(61,159,0),
										System.Drawing.Color.FromArgb(67,162,0),
										System.Drawing.Color.FromArgb(73,165,0),
										System.Drawing.Color.FromArgb(79,168,0),
										System.Drawing.Color.FromArgb(85,171,0),
										System.Drawing.Color.FromArgb(91,174,0),
										System.Drawing.Color.FromArgb(98,177,0),
										System.Drawing.Color.FromArgb(104,180,0),
										System.Drawing.Color.FromArgb(110,183,0),
										System.Drawing.Color.FromArgb(116,186,0),
										System.Drawing.Color.FromArgb(122,189,0),
										System.Drawing.Color.FromArgb(128,192,0),
										System.Drawing.Color.FromArgb(134,195,0),
										System.Drawing.Color.FromArgb(139,197,0),
										System.Drawing.Color.FromArgb(145,200,0),
										System.Drawing.Color.FromArgb(151,203,0),
										System.Drawing.Color.FromArgb(158,206,0),
										System.Drawing.Color.FromArgb(164,209,0),
										System.Drawing.Color.FromArgb(170,212,0),
										System.Drawing.Color.FromArgb(176,215,0),
										System.Drawing.Color.FromArgb(182,218,0),
										System.Drawing.Color.FromArgb(188,222,0),
										System.Drawing.Color.FromArgb(194,225,0),
										System.Drawing.Color.FromArgb(200,228,0),
										System.Drawing.Color.FromArgb(206,231,0),
										System.Drawing.Color.FromArgb(212,234,0),
										System.Drawing.Color.FromArgb(218,237,0),
										System.Drawing.Color.FromArgb(225,240,0),
										System.Drawing.Color.FromArgb(231,243,0),
										System.Drawing.Color.FromArgb(237,246,0),
										System.Drawing.Color.FromArgb(243,249,0),
										System.Drawing.Color.FromArgb(249,252,0),
										System.Drawing.Color.FromArgb(0,128,0),
										System.Drawing.Color.FromArgb(255,253,0),
										System.Drawing.Color.FromArgb(255,250,0),
										System.Drawing.Color.FromArgb(255,248,0),
										System.Drawing.Color.FromArgb(255,246,0),
										System.Drawing.Color.FromArgb(255,244,0),
										System.Drawing.Color.FromArgb(255,242,0),
										System.Drawing.Color.FromArgb(255,240,0),
										System.Drawing.Color.FromArgb(255,238,0),
										System.Drawing.Color.FromArgb(255,236,0),
										System.Drawing.Color.FromArgb(255,234,0),
										System.Drawing.Color.FromArgb(255,232,0),
										System.Drawing.Color.FromArgb(255,230,0),
										System.Drawing.Color.FromArgb(255,227,0),
										System.Drawing.Color.FromArgb(255,225,0),
										System.Drawing.Color.FromArgb(255,223,0),
										System.Drawing.Color.FromArgb(255,221,0),
										System.Drawing.Color.FromArgb(255,219,0),
										System.Drawing.Color.FromArgb(255,217,0),
										System.Drawing.Color.FromArgb(255,215,0),
										System.Drawing.Color.FromArgb(255,212,0),
										System.Drawing.Color.FromArgb(255,210,0),
										System.Drawing.Color.FromArgb(255,208,0),
										System.Drawing.Color.FromArgb(255,205,0),
										System.Drawing.Color.FromArgb(255,203,0),
										System.Drawing.Color.FromArgb(255,201,0),
										System.Drawing.Color.FromArgb(255,199,0),
										System.Drawing.Color.FromArgb(255,197,0),
										System.Drawing.Color.FromArgb(255,195,0),
										System.Drawing.Color.FromArgb(255,193,0),
										System.Drawing.Color.FromArgb(255,191,0),
										System.Drawing.Color.FromArgb(255,189,0),
										System.Drawing.Color.FromArgb(255,187,0),
										System.Drawing.Color.FromArgb(255,185,0),
										System.Drawing.Color.FromArgb(255,182,0),
										System.Drawing.Color.FromArgb(255,180,0),
										System.Drawing.Color.FromArgb(255,178,0),
										System.Drawing.Color.FromArgb(255,176,0),
										System.Drawing.Color.FromArgb(255,174,0),
										System.Drawing.Color.FromArgb(255,172,0),
										System.Drawing.Color.FromArgb(255,170,0),
										System.Drawing.Color.FromArgb(255,167,0),
										System.Drawing.Color.FromArgb(255,255,0),
										System.Drawing.Color.FromArgb(255,161,0),
										System.Drawing.Color.FromArgb(255,157,0),
										System.Drawing.Color.FromArgb(255,153,0),
										System.Drawing.Color.FromArgb(255,149,0),
										System.Drawing.Color.FromArgb(255,145,0),
										System.Drawing.Color.FromArgb(255,141,0),
										System.Drawing.Color.FromArgb(255,137,0),
										System.Drawing.Color.FromArgb(255,133,0),
										System.Drawing.Color.FromArgb(255,130,0),
										System.Drawing.Color.FromArgb(255,126,0),
										System.Drawing.Color.FromArgb(255,122,0),
										System.Drawing.Color.FromArgb(255,118,0),
										System.Drawing.Color.FromArgb(255,114,0),
										System.Drawing.Color.FromArgb(255,110,0),
										System.Drawing.Color.FromArgb(255,106,0),
										System.Drawing.Color.FromArgb(255,102,0),
										System.Drawing.Color.FromArgb(255,98,0),
										System.Drawing.Color.FromArgb(255,94,0),
										System.Drawing.Color.FromArgb(255,91,0),
										System.Drawing.Color.FromArgb(255,87,0),
										System.Drawing.Color.FromArgb(255,83,0),
										System.Drawing.Color.FromArgb(255,79,0),
										System.Drawing.Color.FromArgb(255,75,0),
										System.Drawing.Color.FromArgb(255,71,0),
										System.Drawing.Color.FromArgb(255,67,0),
										System.Drawing.Color.FromArgb(255,63,0),
										System.Drawing.Color.FromArgb(255,59,0),
										System.Drawing.Color.FromArgb(255,55,0),
										System.Drawing.Color.FromArgb(255,51,0),
										System.Drawing.Color.FromArgb(255,47,0),
										System.Drawing.Color.FromArgb(255,43,0),
										System.Drawing.Color.FromArgb(255,39,0),
										System.Drawing.Color.FromArgb(255,35,0),
										System.Drawing.Color.FromArgb(255,31,0),
										System.Drawing.Color.FromArgb(255,28,0),
										System.Drawing.Color.FromArgb(255,24,0),
										System.Drawing.Color.FromArgb(255,20,0),
										System.Drawing.Color.FromArgb(255,16,0),
										System.Drawing.Color.FromArgb(255,12,0),
										System.Drawing.Color.FromArgb(255,8,0),
										System.Drawing.Color.FromArgb(255,4,0),
										System.Drawing.Color.FromArgb(255,0,0),
										System.Drawing.Color.FromArgb(0,0,0),
										System.Drawing.Color.FromArgb(0,0,0),
										System.Drawing.Color.FromArgb(0,0,0)


		};
        #endregion
       
    }

  
   
}
