using System;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using TerraViewer.Healpix;

namespace TerraViewer
{

    /**
     * Added by Yunfei Xu on 2016/09/10
     */
    public class HealpixTile : Tile
    {
        bool topDown = false;

        protected PositionTexture[,] bounds;
        protected bool backslash = false;
        List<PositionTexture> vertexList = null;
        //List<Triangle>[] childTriangleList = new List<Triangle>[4];

        int nside = 2;
        int npface; //nside * nside;
        protected double[] demArray;
        public int tileIndex = -1;
        short[] indexArray;
        int step;
        int face;
        public int Face
        {
            get
            {
                return face;
            }
        }
        int quadIndexStart;
        int quadIndexEnd;

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

        public HealpixTile(int level, int x, int y, IImageSet dataset, Tile parent)
        {
            //if (x != 0 && y != 0)
            //    return;
            this.level = level;
            this.x = x;
            this.y = y;
            this.dataset = dataset;
            this.topDown = !dataset.BottomsUp;
            if(level ==0)
            {
                this.nside = 4;
            }
            else { 
            this.nside = (int)Math.Pow(2, level + 1);

            //if current order is 0, eg. there is no parent for the current tile, the face index is 4x+y

            }


            if (parent == null)
            {
                this.face = x * 4 + y;
                quadIndexStart = 0;
                quadIndexEnd = 15;
                //this.tileIndex = this.face;
            }
            else
            {
                // if not, current tile's face index is its parent's face index
                HealpixTile parentTile = (HealpixTile)parent;
                this.face = parentTile.face;
                // if no parent, the current tile's index is 2y+x
                if (parentTile.tileIndex == -1)
                {
                    this.tileIndex = y * 2 + x;
                }
                else
                {
                    //if has parent, the index is 4*parenttileindex+2y+x
                    this.tileIndex = parentTile.tileIndex * 4 + y * 2 + x;
                }
                quadIndexStart = this.tileIndex * 4;
                quadIndexEnd = quadIndexStart + 3;
                this.Parent = parent;
            }

            demSize = 513;
            if (dataset.MeanRadius != 0)
            {
                this.DemScaleFactor = dataset.MeanRadius;
            }
            else
            {
                if (dataset.DataSetType == ImageSetType.Earth)
                {
                    DemScaleFactor = 6371000;
                }
                else
                {
                    DemScaleFactor = 3396010;
                }
            }

            ComputeQuadrant();

            // insideOut = this.Dataset.DataSetType == ImageSetType.Sky || this.Dataset.DataSetType == ImageSetType.Panorama;
            
            // All healpix is inside out
            insideOut = true;
            ComputeBoundingSphere();
        }

        protected void ComputeBoundingSphere()
        {
            setStep();
            if (vertexList == null)
            {
                createGeometry();
            }

            Vector3d[] pointList = BufferPool11.GetVector3dBuffer(vertexList.Count);
            for (int i = 0; i < vertexList.Count; i++)
            {
                pointList[i] = vertexList[i].Position;
            }

            CalcSphere(pointList);
            //localCenter = sphereCenter;
            VertexCount = vertexList.Count;
        }

        private void createGeometry()
        {
            vertexList = BufferPool11.GetPositionTextureList();
            //int face = x * 4 + y;

            int nQuads = (int)Math.Pow(nside, 2);// quads of one face in a specific order 


            //int ppq = 4;// points per quad
            //int nPoints = nQuads * ppq;
            int faceoff = nQuads * face;
            Vector3d[] points;

            try
            {
                int index = 0;
                Vector3d tmp;
                int quadIndex = 0;

                if (level == 0)

                {
                    vertexListOfLevel0(vertexList, quadIndexStart, quadIndexEnd, faceoff);
                }
                else
                {
                    for (int q = quadIndexStart; q <= quadIndexEnd; q++)
                    {
                        points = this.boundaries(faceoff + q);
                        double u = 0, v = 0;
                        if (quadIndex == 0)
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                int tx = i / step;
                                int ty = i % step;

                                if (tx == 0)
                                {
                                    u = 0.5;
                                    v = 0.5 - 0.5 / step * ty;
                                }
                                else if (tx == 1)
                                {
                                    u = 0.5 - 0.5 / step * ty;
                                    v = 0;
                                }
                                else if (tx == 2)
                                {
                                    u = 0;
                                    v = 0.5 / step * ty;
                                }
                                else if (tx == 3)
                                {
                                    u = 0.5 / step * ty;
                                    v = 0.5;
                                }
                                vertexList.Add(new PositionTexture(points[i], u, v));
                            }
                        }
                        else if (quadIndex == 1)
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                int tx = i / step;
                                int ty = i % step;

                                if (tx == 0)
                                {
                                    u = 0.5;
                                    v = 1 - 0.5 / step * ty;
                                }
                                else if (tx == 1)
                                {
                                    u = 0.5 - 0.5 / step * ty;
                                    v = 0.5;
                                }
                                else if (tx == 2)
                                {
                                    u = 0;
                                    v = 0.5 + 0.5 / step * ty;
                                }
                                else if (tx == 3)
                                {
                                    u = 0.5 / step * ty;
                                    v = 1;
                                }
                                vertexList.Add(new PositionTexture(points[i], u, v));
                            }
                        }
                        else if (quadIndex == 2)
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                int tx = i / step;
                                int ty = i % step;

                                if (tx == 0)
                                {
                                    u = 1;
                                    v = 0.5 - 0.5 / step * ty;
                                }
                                else if (tx == 1)
                                {
                                    u = 1 - 0.5 / step * ty;
                                    v = 0;
                                }
                                else if (tx == 2)
                                {
                                    u = 0.5;
                                    v = 0.5 / step * ty;
                                }
                                else if (tx == 3)
                                {
                                    u = 0.5 + 0.5 / step * ty;
                                    v = 0.5;
                                }
                                vertexList.Add(new PositionTexture(points[i], u, v));
                            }
                        }
                        else if (quadIndex == 3)
                        {
                            for (int i = 0; i < points.Length; i++)
                            {
                                int tx = i / step;
                                int ty = i % step;

                                if (tx == 0)
                                {
                                    u = 1;
                                    v = 1 - 0.5 / step * ty;
                                }
                                else if (tx == 1)
                                {
                                    u = 1 - 0.5 / step * ty;
                                    v = 0.5;
                                }
                                else if (tx == 2)
                                {
                                    u = 0.5;
                                    v = 0.5 + 0.5 / step * ty;
                                }
                                else if (tx == 3)
                                {
                                    u = 0.5 + 0.5 / step * ty;
                                    v = 1;
                                }
                                vertexList.Add(new PositionTexture(points[i], u, v));
                            }
                        }
                        quadIndex++;
                    }
                }
            }
            catch (Exception e)
            {
            }
        }


        public string GetDirectory(IImageSet dataset, int level, int x, int y)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"Imagery\HiPS\");
            sb.Append(dataset.Name.Replace(" ", "_"));
            sb.Append(@"\Norder" + (level));
            sb.Append(@"\Dir");
            int tileTextureIndex = this.face * nside * nside / 4 + this.tileIndex;
            int subDirIndex = tileTextureIndex / 10000;
            if (subDirIndex != 0)
            {
                sb.Append(subDirIndex);
                sb.Append(@"0000\");
            }
            else
            {
                sb.Append(@"0\");
            }
            return sb.ToString();
        }

        public string GetFilename()
        {
            string extention = dataset.Extension.StartsWith(".") ? dataset.Extension : "." + dataset.Extension;

            StringBuilder sb = new StringBuilder();
            sb.Append(Properties.Settings.Default.CahceDirectory);
            sb.Append(@"Imagery\HiPS\");
            sb.Append(dataset.Name.Replace(" ", "_"));
            sb.Append("\\");
            if (level < 0)
            {
                if (!System.IO.Directory.Exists(sb.ToString()))
                {
                    System.IO.Directory.CreateDirectory(sb.ToString());
                }
                sb.Append(@"fake.png");

                if (!File.Exists(sb.ToString()))
                {
                    CreateFakePNG(sb.ToString());
                }
            }

            else
            {
                sb.Append(@"Norder" + (level));
                sb.Append(@"\Dir");
                int tileTextureIndex = 0;
                if (level == 0)
                {
                    tileTextureIndex = this.face;
                }
                else
                {
                    tileTextureIndex = this.face * nside * nside / 4 + this.tileIndex;
                }
                if (tileTextureIndex == -1)
                {
                    tileTextureIndex = 0;
                }
                int subDirIndex = tileTextureIndex / 10000;
                if (subDirIndex != 0)
                {
                    sb.Append(subDirIndex);
                    sb.Append(@"0000\");
                }
                else
                {
                    sb.Append(@"0\");
                }
                sb.Append(@"Npix");
                sb.Append(tileTextureIndex);
                sb.Append(extention);
            }
            return sb.ToString();
        }

        private void CreateFakePNG(string path)
        {


            Bitmap bmp = new Bitmap(512, 512);
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            g.FillRectangle(Brushes.Red, 100, 100, 100, 100);

            g.Flush();
            bmp.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }

        public static void GenerateLevel2(string filename)
        {
            //if(Directory.Exists())

            string extention = Path.GetExtension(filename);
            string path = filename.Replace("Allsky" + extention, "Dir0");
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            var imgarray = new Image[27 * 29];
            var img = Image.FromFile(filename);
            for (int i = 0; i < 29; i++)
            {
                for (int j = 0; j < 27; j++)
                {
                    var index = i * 27 + j;
                    imgarray[index] = new Bitmap(64, 64);
                    var graphics = Graphics.FromImage(imgarray[index]);
                    graphics.DrawImage(img, new Rectangle(0, 0, 64, 64), new Rectangle(j * 64, i * 64, 64, 64), GraphicsUnit.Pixel);
                    graphics.Dispose();
                    imgarray[index].Save(path + "\\Npix" + index + extention);
                }
            }

        }

        public string GetUrl(IImageSet dataset, int level, int x, int y)
        {

            string returnUrl = "";
            string extention = dataset.Extension.StartsWith(".") ? dataset.Extension : "." + dataset.Extension;

            int tileTextureIndex = -1;
            if (level == 0)
            {
                tileTextureIndex = this.face;
            }
            else
            {
                tileTextureIndex = this.face * nside * nside / 4 + this.tileIndex;
            }
            StringBuilder sb = new StringBuilder();

            int subDirIndex = tileTextureIndex / 10000;

            if (subDirIndex != 0)
            {
                sb.Append(subDirIndex);
                sb.Append("0000");
            }
            else
            {
                sb.Append("0");
            }

            returnUrl = string.Format(dataset.Url, level.ToString(), sb.ToString(), tileTextureIndex.ToString() + extention);
            //}


            return returnUrl;
        }

        private void computeUV(int pi, int count)
        {
            int l = count / 4;//points per edge;

        }


        public Vector3d[] boundaries(long pix)
        {
            Vector3d[] points = new Vector3d[4 * step];
            Xyf xyf = pix2xyf(pix);
            double dc = 0.5 / nside;
            double xc = (xyf.ix + 0.5) / nside, yc = (xyf.iy + 0.5) / nside;

            double d = 1d / (step * nside);
            for (int i = 0; i < step; ++i)
            {
                if (insideOut)
                {
                    points[i] = new Fxyf(xc + dc - i * d, yc + dc, xyf.face).toVec3();
                    points[i + step] = new Fxyf(xc - dc, yc + dc - i * d, xyf.face).toVec3();
                    points[i + 2 * step] = new Fxyf(xc - dc + i * d, yc - dc, xyf.face).toVec3();
                    points[i + 3 * step] = new Fxyf(xc + dc, yc - dc + i * d, xyf.face).toVec3();
                }

                else
                {
                    Vector3d tmp = new Fxyf(xc + dc - i * d, yc + dc, xyf.face).toVec3();
                    points[i] = new Vector3d(-tmp.X, tmp.Y, -tmp.Z);
                    tmp = new Fxyf(xc - dc, yc + dc - i * d, xyf.face).toVec3();
                    points[i + step] = new Vector3d(-tmp.X, tmp.Y, -tmp.Z);
                    tmp = new Fxyf(xc - dc + i * d, yc - dc, xyf.face).toVec3();
                    points[i + 2 * step] = new Vector3d(-tmp.X, tmp.Y, -tmp.Z);
                    tmp = new Fxyf(xc + dc, yc - dc + i * d, xyf.face).toVec3();
                    points[i + 3 * step] = new Vector3d(-tmp.X, tmp.Y, -tmp.Z);
                }
                if (i == 0)
                {
                    TopLeft = points[i];
                    BottomLeft = points[i + step];
                    BottomRight = points[i + 2 * step];
                    TopRight = points[i + 3 * step];
                }
            }


            return points;
        }

        public override bool Draw3D(RenderContext11 renderContext, float transparancy, Tile parent)
        {
            //if (this.level > 0)
            //    return false;

            int tileTextureIndex =-1;
            if(level == 0)
            {
                tileTextureIndex = this.face;
            }
            else
            {
                tileTextureIndex = this.face * nside * nside / 4 + this.tileIndex;
            }

            RenderedGeneration = CurrentRenderGeneration;
            TilesTouched++;

            InViewFrustum = true;

            if (!ReadyToRender)
            {
                TileCache.AddTileToQueue(this);

                return false;
            }



            TilesInView++;

            if (!CreateGeometry(renderContext, true))
            {
                if (level > 3)
                {
                    return false;
                }
            }

            int partCount = this.TriangleCount;
            TrianglesRendered += partCount;

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
                int childIndex = 0;


                for (int y1 = 0; y1 < 2; y1++)
                {
                    for (int x1 = 0; x1 < 2; x1++)
                    {


                        if (level < dataset.Levels)
                        {

                            HealpixTile child;
                            child = (HealpixTile)TileCache.GetTile(level + 1, x1, y1, dataset, this);
                            childrenId[childIndex] = child.Key;
                            if (child.IsTileInFrustum(renderContext.Frustum))
                            {

                                InViewFrustum = true;
                                if (child.IsTileBigEnough(renderContext))
                                {
                                    renderPart[childIndex].TargetState = !child.Draw3D(renderContext, transparancy, this);

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

                            if (renderPart[childIndex].TargetState == true)
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

                if (wireFrame)
                {
                    renderContext.MainTexture = null;
                }
                else
                {

                    renderContext.MainTexture = texture;

                }

                if (dataset.DataSetType == ImageSetType.Sky )
                {
                    HDRPixelShader.constants.opacity = transparancy;
                    HDRPixelShader.Use(renderContext.devContext);
                }

                renderContext.SetVertexBuffer(vertexBuffer);

                renderContext.SetIndexBuffer(indexBuffer[0]);

                renderContext.devContext.DrawIndexed(indexBuffer[0].Count, 0, 0);


            }
            catch (Exception e)
            {

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

        //public override bool IsTileInFrustum(PlaneD[] frustum)
        //{


        //    if (level > dataset.Levels)
        //    {
        //        return false;
        //    }
        //    InViewFrustum = false;
        //    Vector3d center = sphereCenter;

        //    if (this.Level < 2 )
        //    {
        //        return true;
        //    }

        //    Vector4d centerV4 = new Vector4d(center.X, center.Y, center.Z, 1f);
        //    Vector3d length = new Vector3d(sphereRadius, 0, 0);

        //    double rad = length.Length();
        //    double tmp;
        //    for (int i = 0; i < 6; i++)
        //    {
        //        tmp = frustum[i].Dot(centerV4) + rad;
        //        if (insideOut)
        //        {
        //            //The method used in toast tiles doesn't work here, so several threshold values were tested based on different levels' view.
        //            if (level <= 4)
        //            {

        //                if (i == 4)
        //                {
        //                    if (tmp < -0.19)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.16)
        //                        return false;

        //                }
        //            }
        //            else if (level <= 6)
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -0.19)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.02)
        //                        return false;

        //                }
        //            }
        //            else if (level <= 8)
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -0.19)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.02)
        //                        return false;

        //                }
        //            }
        //            else
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -0.19)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.02)
        //                        return false;

        //                }
        //            }
        //        }
        //        else
        //        {

        //            if (level <= 4)
        //            {

        //                if (i == 0)
        //                {
        //                    if (tmp < -0.8)
        //                        return false;
        //                }
        //                else if (i == 1)
        //                {
        //                    if (tmp < -0.8)
        //                        return false;
        //                }
        //                else if (i == 2)
        //                {
        //                    if (tmp < -0.8)
        //                        return false;
        //                }
        //                else if (i == 3)
        //                {
        //                    if (tmp < -0.8)
        //                        return false;
        //                }
        //                else if (i == 4)
        //                {
        //                    if (tmp < -1)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //            }
        //            else if (level <= 6)
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -1)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.64)
        //                        return false;

        //                }
        //            }
        //            else if (level <= 8)
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -1)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.61)
        //                        return false;

        //                }
        //            }
        //            else
        //            {
        //                if (i == 4)
        //                {
        //                    if (tmp < -1)
        //                        return false;
        //                }
        //                else if (i == 5)
        //                {
        //                    if (tmp < 0)
        //                        return false;
        //                }
        //                else
        //                {
        //                    if (tmp < -0.6)
        //                        return false;

        //                }
        //            }
        //        }
        //    }
        //    InViewFrustum = true;

        //    return true;
        //}


        public int GetTileTextureIndex()
        {
            int tileTextureIndex = this.face * nside * nside / 4 + this.tileIndex;
            return tileTextureIndex;
        }

        public override bool IsTileBigEnough(RenderContext11 renderContext)
        {

            if (level > 2)
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
                topLeftScreen = SharpDX.Vector3.Project(topLeftScreen, Viewport.X, Viewport.Y, Viewport.Width, Viewport.Height, Viewport.MinDepth, Viewport.MaxDepth, wvp);


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

        protected Xyf pix2xyf(long ipix)
        {
            npface = nside * nside;
            long pix = ipix & (npface - 1);//在具体某个面中的quadindex
            return new Xyf(compress_bits(pix), compress_bits(unsignRM(pix, 1)),
                            (int)(unsignRM(ipix, (2 * nside2order(nside)))));
        }

        protected void setStep()
        {
            step = 4;
            if (nside >= 8)
                step = 4;
            if (nside >= 16)
                step = 2;
            if (nside >= 32)
                step = 1;
            //if (nside >= 64)
            //    step = 1;
        }


        public static int nside2order(long nside)
        {
            HealpixUtils.check(nside > 0, "nside must be positive");
            return ((nside & (nside - 1)) != 0) ? -1 : HealpixUtils.ilog2(nside);
        }

        private static int compress_bits(long v)
        {
            long raw = v & 0x5555555555555555L;
            raw |= unsignRM(raw, 15);
            int raw1 = (int)(raw & 0xffffL), raw2 = (int)((unsignRM(raw, 32)) & 0xffffL);
            int result = 0;

            short a = HealpixTables.ctab[raw1 & 0xff];
            short b = (short)(HealpixTables.ctab[unsignRM(raw1, 8)] << 4);
            short c = (short)(HealpixTables.ctab[raw2 & 0xff] << 16);
            short d = (short)(HealpixTables.ctab[unsignRM(raw2, 8)] << 20);
            result = a | b | c | d;

            return result;
        }

        public static long unsignRM(long x, int y)
        {
            int mask = 0x7fffffff; //Integer.MAX_VALUE
            for (int i = 0; i < y; i++)
            {
                x >>= 1;
                x &= mask;
            }
            return x;
        }

        static protected IndexBuffer11[,] slashIndexBuffer = new IndexBuffer11[4, 16];
        static protected IndexBuffer11[,] backSlashIndexBuffer = new IndexBuffer11[4, 16];
        static protected IndexBuffer11[] rootIndexBuffer = new IndexBuffer11[4];

        public override IndexBuffer11 GetIndexBuffer(int index, int accomidation)
        {
            if (level == 0)
            {
                return rootIndexBuffer[index];
            }

            if (backslash)
            {
                return backSlashIndexBuffer[index, accomidation];
            }
            else
            {
                return slashIndexBuffer[index, accomidation];
            }
        }

        protected void CalcSphere(Vector3d[] list)
        {
            ConvexHull.FindEnclosingSphere(list, out sphereCenter, out sphereRadius);
        }

        public override bool IsPointInTile(double lat, double lng)
        {
            if (level == 0)
            {
                return true;
            }

            if (level == 1)
            {
                if ((lng >= 0 && lng <= 90) && (X == 0 && Y == 1))
                {
                    return true;
                }
                if ((lng > 90 && lng <= 180) && (X == 1 && Y == 1))
                {
                    return true;
                }
                if ((lng < 0 && lng >= -90) && (X == 0 && Y == 0))
                {
                    return true;
                }
                if ((lng < -90 && lng >= -180) && (X == 1 && Y == 0))
                {
                    return true;
                }
            }


            Vector3d testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            bool top = IsLeftOfHalfSpace(TopLeft, TopRight, testPoint);
            bool right = IsLeftOfHalfSpace(TopRight, BottomRight, testPoint);
            bool bottom = IsLeftOfHalfSpace(BottomRight, BottomLeft, testPoint);
            bool left = IsLeftOfHalfSpace(BottomLeft, TopLeft, testPoint);

            if (top && right && bottom && left)
            {
                return true;
            }
            return false; ;

        }

        private bool IsLeftOfHalfSpace(Vector3d pntA, Vector3d pntB, Vector3d pntTest)
        {
            pntA.Normalize();
            pntB.Normalize();
            Vector3d cross = Vector3d.Cross(pntA, pntB);

            double dot = Vector3d.Dot(cross, pntTest);

            return dot > 0;
        }

        public override double GetSurfacePointAltitude(double lat, double lng, bool meters)
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
                            double retVal = child.GetSurfacePointAltitude(lat, lng, meters);
                            if (retVal != 0)
                            {
                                return retVal;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return GetAltitudeFromLatLng(lat, lng, meters);
        }

        private double GetAltitudeFromLatLng(double lat, double lng, bool meters)
        {
            Vector3d testPoint = Coordinates.GeoTo3dDouble(lat, lng);
            Vector2d uv = DistanceCalc.GetUVFromInnerPoint(TopLeft, TopRight, BottomLeft, BottomRight, testPoint);

            // Get 4 samples and interpolate
            double uud = Math.Max(0, Math.Min(16, (uv.X * 16)));
            double vvd = Math.Max(0, Math.Min(16, (uv.Y * 16)));



            int uu = Math.Max(0, Math.Min(15, (int)(uv.X * 16)));
            int vv = Math.Max(0, Math.Min(15, (int)(uv.Y * 16)));

            double ha = uud - uu;
            double va = vvd - vv;

            if (demArray != null)
            {
                // 4 nearest neighbors
                double ul = demArray[uu + 17 * vv];
                double ur = demArray[(uu + 1) + 17 * vv];
                double ll = demArray[uu + 17 * (vv + 1)];
                double lr = demArray[(uu + 1) + 17 * (vv + 1)];

                double top = ul * (1 - ha) + ha * ur;
                double bottom = ll * (1 - ha) + ha * lr;
                double val = top * (1 - va) + va * bottom;

                return val / (meters ? 1 : DemScaleFactor);
            }

            return demAverage / (meters ? 1 : DemScaleFactor);
        }

        static int countCreatedForNow = 0;

        public override double GetSurfacePointAltitudeNow(double lat, double lng, bool meters, int targetLevel)
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
                        if (level < dataset.Levels && level < (targetLevel + 1))
                        {
                            Tile child = TileCache.GetCachedTile(childrenId[childIndex]);
                            if (child == null || !child.ReadyToRender)
                            {
                                countCreatedForNow++;
                                child = TileCache.GetTileNow(level + 1, x * 2 + ((x1 + xOffset) % 2), y * 2 + ((y1 + yOffset) % 2), dataset, this);
                                childrenId[childIndex] = child.Key;
                            }
                            childIndex++;
                            if (child != null)
                            {
                                if (child.IsPointInTile(lat, lng))
                                {
                                    double retVal = child.GetSurfacePointAltitudeNow(lat, lng, meters, targetLevel);
                                    if (retVal != 0)
                                    {
                                        return retVal;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return GetAltitudeFromLatLng(lat, lng, meters);
        }

        private PositionTexture Midpoint(PositionTexture positionNormalTextured, PositionTexture positionNormalTextured_2)
        {
            Vector3d a1 = Vector3d.Lerp(positionNormalTextured.Position, positionNormalTextured_2.Position, .5f);
            Vector2d a1uv = Vector2d.Lerp(new Vector2d(positionNormalTextured.Tu, positionNormalTextured.Tv), new Vector2d(positionNormalTextured_2.Tu, positionNormalTextured_2.Tv), .5f);

            a1.Normalize();
            return new PositionTexture(a1, a1uv.X, a1uv.Y);
        }

        private Vector3d MidPoint3d(Vector3d vector1, Vector3d vector2)
        {
            Vector3d a1 = Vector3d.Lerp(vector1, vector2, .5f);
            return a1;
        }

        //int subDivisionLevel = 4;

        bool subDivided = false;

        public static Mutex dumpMutex = new Mutex();

        public override void OnCreateVertexBuffer(VertexBuffer11 vb)
        {

            if (!subDivided)
            {

                if (vertexList == null)
                {
                    createGeometry();
                }

                try
                {

                    // Create a vertex buffer 
                    PositionNormalTexturedX2[] verts = (PositionNormalTexturedX2[])vb.Lock(0, 0); // Lock the buffer (which will return our structs)
                    int index = 0;
                    foreach (PositionTexture vert in vertexList)
                    {
                        verts[index++] = vert.PositionNormalTextured(new Vector3d(0, 0, 0), false);

                    }

                    vb.Unlock();


                    if (level == 0)
                    {
                        this.indexBuffer[0] = new IndexBuffer11(typeof(short), 6 * 16, RenderContext11.PrepDevice);
                        index = 0;
                        indexArray = (short[])this.indexBuffer[0].Lock();
                        int offset = verts.Length / 16;
                        for (int i = 0; i < 16; i++)
                        {
                            indexArray[i * 6 + 0] = (short)(2 * step + offset * i);
                            indexArray[i * 6 + 1] = (short)(3 * step + offset * i);
                            indexArray[i * 6 + 2] = (short)(1 * step + offset * i);
                            indexArray[i * 6 + 3] = (short)(3 * step + offset * i);
                            indexArray[i * 6 + 4] = (short)(0 * step + offset * i);
                            indexArray[i * 6 + 5] = (short)(1 * step + offset * i);

                            //indexArray[i * 6 + 1] = (short)(2 * step + offset * i);
                            //indexArray[i * 6 + 3] = (short)(3 * step + offset * i);
                            //indexArray[i * 6 + 2] = (short)(1 * step + offset * i);
                            //indexArray[i * 6 + 4] = (short)(3 * step + offset * i);
                            //indexArray[i * 6 + 5] = (short)(0 * step + offset * i);
                            //indexArray[i * 6 + 0] = (short)(1 * step + offset * i);
                        }

                    }
                    else
                    {
                        this.indexBuffer[0] = new IndexBuffer11(typeof(short), 6 * 4, RenderContext11.PrepDevice);
                        index = 0;
                        indexArray = (short[])this.indexBuffer[0].Lock();
                        int offset = verts.Length / 4;
                        for (int i = 0; i < 4; i++)
                        {  
                            indexArray[i * 6 + 0] = (short)(2 * step + offset * i);
                            indexArray[i * 6 + 1] = (short)(3 * step + offset * i);
                            indexArray[i * 6 + 2] = (short)(1 * step + offset * i);
                            indexArray[i * 6 + 3] = (short)(3 * step + offset * i);
                            indexArray[i * 6 + 4] = (short)(0 * step + offset * i);
                            indexArray[i * 6 + 5] = (short)(1 * step + offset * i);
                        }
                }


                    //indexArray[index] = (short)(0);
                    //indexArray[index + 1] = (short)(1);
                    //indexArray[index + 2] = (short)(2);
                    //indexArray[index + 3] = (short)(1);
                    //indexArray[index + 4] = (short)(3);
                    //indexArray[index + 5] = (short)(2);
                this.indexBuffer[0].Unlock();


                }
                catch (Exception e)
                {

                }

                ReturnBuffers();
            }
        }

        private void ProcessIndexBuffer(short[] indexArray, int part)
        {

            if (level == 0)
            {
                rootIndexBuffer[part] = new IndexBuffer11(RenderContext11.PrepDevice, indexArray);
                return;
            }

            for (int a = 0; a < 16; a++)
            {
                short[] partArray = indexArray.Clone() as short[];
                if (backslash)
                {
                    backSlashIndexBuffer[part, a] = new IndexBuffer11(RenderContext11.PrepDevice, partArray);
                }
                else
                {
                    slashIndexBuffer[part, a] = new IndexBuffer11(RenderContext11.PrepDevice, partArray);
                }
            }
        }

        private static byte[] slashXIndex;
        private static byte[] slashYIndex;
        private static byte[] backslashXIndex;
        private static byte[] backslashYIndex;

        private byte[] tempSlashXIndex;
        private byte[] tempSlashYIndex;
        private byte[] tempBackslashXIndex;
        private byte[] tempBackslashYIndex;


        public override bool CreateDemFromParent()
        {


            HealpixTile parent = Parent as HealpixTile;
            if (parent == null)
            {
                return false;
            }

            int offsetX = ((X % 2) == 1 ? 8 : 0);
            int offsetY = ((Y % 2) == 0 ? 8 : 0);


            demArray = new double[17 * 17];
            // Interpolate accross 
            for (int y = 0; y < 17; y += 2)
            {
                bool copy = true;
                for (int x = 0; x < 17; x++)
                {
                    if (copy)
                    {
                        demArray[(16 - y) * 17 + x] = parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY);
                    }
                    else
                    {
                        demArray[(16 - y) * 17 + x] =
                            (
                            (
                                parent.GetDemSample((x / 2) + offsetX, (y / 2) + offsetY) +
                                parent.GetDemSample(((x / 2) + offsetX) + 1, (y / 2) + offsetY)
                            ) / 2);
                    }
                    copy = !copy;

                }
            }
            // Interpolate down
            for (int y = 1; y < 17; y += 2)
            {
                for (int x = 0; x < 17; x++)
                {

                    demArray[(16 - y) * 17 + x] =
                        (
                        (
                            GetDemSample(x, y - 1) +
                            GetDemSample(x, y + 1)
                        ) / 2);

                }
            }

            // Convert the dem array back to the arranged DEM list thu slash/backslash mapping tables


            DemData = new double[demSize];
            for (int i = 0; i < demSize; i++)
            {
                if (backslash)
                {
                    DemData[i] = demArray[backslashXIndex[i] + backslashYIndex[i] * 17];
                }
                else
                {
                    DemData[i] = demArray[slashXIndex[i] + slashYIndex[i] * 17];
                }
                demAverage += DemData[i];

            }

            // WriteDemIndexArrays();

            // Get Average value for new DemData table

            demAverage /= DemData.Length;

            DemReady = true;
            return true;
        }

        private static void WriteDemIndexArrays()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(" byte[] backslashXIndex = new byte[] {");
            foreach (byte b in backslashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");
            sb.AppendLine("");

            sb.Append(" byte[] backslashYIndex = new byte[] {");
            foreach (byte b in backslashYIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");
            sb.AppendLine("");

            sb.Append(" byte[] slashXIndex = new byte[] {");
            foreach (byte b in slashXIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");

            sb.AppendLine("");

            sb.Append(" byte[] slashYIndex = new byte[] {");
            foreach (byte b in slashYIndex)
            {
                sb.Append(b.ToString());
                sb.Append(", ");
            }

            sb.Append("};");

            File.WriteAllText("c:\\tmp\\demIndex.cs", sb.ToString());

        }

        private static void WriteArray(byte[] data)
        {

        }

        private double GetDemSample(int x, int y)
        {
            return demArray[(16 - y) * 17 + x];
        }

        int quadrant = 0;

        private void ComputeQuadrant()
        {
            int xQuad = 0;
            int yQuad = 0;
            int tiles = (int)Math.Pow(2, this.level);

            if (x > (tiles / 2) - 1)
            {
                xQuad = 1;
            }

            if (y > (tiles / 2) - 1)
            {
                yQuad = 1;
            }
            quadrant = yQuad * 2 + xQuad;
        }



        public override void CleanUp(bool removeFromParent)
        {
            base.CleanUp(removeFromParent);
            ReturnBuffers();
            subDivided = false;
        }

        private void ReturnBuffers()
        {


            if (vertexList != null)
            {
                BufferPool11.ReturnPositionTextureList(vertexList);
                vertexList = null;
            }

        }

        private void vertexListOfLevel0(List<PositionTexture> vertexList, int quadIndexStart, int quadIndexEnd, int faceoff)
        {
            Vector3d[] points;
            int quadIndex = 0;
            for (int q = quadIndexStart; q <= quadIndexEnd; q++)
            {
                points = this.boundaries(faceoff + q);
                
                //测试新算法
                double u = 0, v = 0;
                if (quadIndex == 0)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25;
                            v = 0.25 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty;
                            v = 0;
                        }
                        else if (tx == 2)
                        {
                            u = 0;
                            v = 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty;
                            v = 0.25;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }


                }
                else if (quadIndex == 1)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25;
                            v = 0.5 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty;
                            v = 0.25;
                        }
                        else if (tx == 2)
                        {
                            u = 0;
                            v = 0.25 + 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty;
                            v = 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 2)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5;
                            v = 0.25 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty;
                            v = 0;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25;
                            v = 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty;
                            v = 0.25;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 3)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5;
                            v = 0.5 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty;
                            v = 0.25;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25;
                            v = 0.25 + 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty;
                            v = 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 4)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25;
                            v = 0.25 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty;
                            v = 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0;
                            v = 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty;
                            v = 0.25 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }


                }
                else if (quadIndex == 5)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25;
                            v = 0.5 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty;
                            v = 0.25 +0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0;
                            v = 0.25 + 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty;
                            v = 0.5 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 6)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5;
                            v = 0.25 - 0.25 / step * ty+ 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty;
                            v = 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25;
                            v = 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty;
                            v = 0.25 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 7)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5;
                            v = 0.5 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty;
                            v = 0.25 + 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25;
                            v = 0.25 + 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty;
                            v = 0.5+ 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 8)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25 + 0.5;
                            v = 0.25 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty + 0.5;
                            v = 0;
                        }
                        else if (tx == 2)
                        {
                            u = 0 + 0.5;
                            v = 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty +0.5;
                            v = 0.25;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }


                }
                else if (quadIndex == 9)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25 + 0.5;
                            v = 0.5 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty + 0.5;
                            v = 0.25;
                        }
                        else if (tx == 2)
                        {
                            u = 0 + 0.5;
                            v = 0.25 + 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty + 0.5;
                            v = 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 10)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5+0.5;
                            v = 0.25 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty + 0.5;
                            v = 0;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25+0.5;
                            v = 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty + 0.5;
                            v = 0.25;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 11)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5 + 0.5;
                            v = 0.5 - 0.25 / step * ty;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty + 0.5;
                            v = 0.25;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25 + 0.5;
                            v = 0.25 + 0.25 / step * ty;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty + 0.5;
                            v = 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                if (quadIndex == 12)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25 +0.5;
                            v = 0.25 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty + 0.5;
                            v = 0 +0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0 + 0.5;
                            v = 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty + 0.5;
                            v = 0.25 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 13)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.25 + 0.5;
                            v = 0.5 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.25 - 0.25 / step * ty + 0.5;
                            v = 0.25 + 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0 + 0.5;
                            v = 0.25 + 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 / step * ty + 0.5;
                            v = 0.5 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 14)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5 + 0.5;
                            v = 0.25 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty + 0.5;
                            v = 0 + 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25 + 0.5;
                            v = 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty + 0.5;
                            v = 0.25 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                }
                else if (quadIndex == 15)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        int tx = i / step;
                        int ty = i % step;

                        if (tx == 0)
                        {
                            u = 0.5 + 0.5;
                            v = 0.5 - 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 1)
                        {
                            u = 0.5 - 0.25 / step * ty + 0.5;
                            v = 0.25 + 0.5;
                        }
                        else if (tx == 2)
                        {
                            u = 0.25 + 0.5;
                            v = 0.25 + 0.25 / step * ty + 0.5;
                        }
                        else if (tx == 3)
                        {
                            u = 0.25 + 0.25 / step * ty + 0.5;
                            v = 0.5 + 0.5;
                        }
                        vertexList.Add(new PositionTexture(points[i], u, v));
                    }
                   
                    
                }

                quadIndex++;
            }
        }
    }
    /*
     * 在当前order的第几个face（f）的行列号（x,y）
     */
    public class Xyf
    {
        public int ix, iy, face;
        public Xyf() { }
        public Xyf(int x, int y, int f)
        { ix = x; iy = y; face = f; }
    }

}
