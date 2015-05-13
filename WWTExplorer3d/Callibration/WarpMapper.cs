using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using WwtDataUtils;
using System.Drawing.Imaging;

namespace TerraViewer.Callibration
{
    class WarpMapper
    {
        public static Bitmap MakeWarpMap(ProjectorEntry pe, double domeSize, bool radialDistortion, List<GroundTruthPoint> gtPointList, ScreenTypes screenType)
        {



            List<Vector2d> PointTo = new List<Vector2d>();
            List<Vector2d> PointFrom = new List<Vector2d>();

            double xFactor = pe.Width / 512.0;
            double yFactor = pe.Height / 512.0;

            Bitmap bmp = new Bitmap(512, 512);
            FastBitmap fastBmp = new FastBitmap(bmp);

            SolveProjector spProjector = new SolveProjector(pe, domeSize, ProjectionType.Projector, screenType, SolveParameters.Default);
            spProjector.RadialDistorion = radialDistortion;
            
            SolveProjector spView = new SolveProjector(pe, domeSize, ProjectionType.View, ScreenTypes.Spherical, SolveParameters.Default);
            spView.RadialDistorion = false;


            foreach (GroundTruthPoint gt in gtPointList)
            {
                PointFrom.Add(new Vector2d((double)gt.X / (double)pe.Width * 512, (double)gt.Y / (double)pe.Height * 512));

                Vector2d pntOutTarget = spView.ProjectPoint(new Vector2d(gt.Az, gt.Alt));
                Vector2d AltAzMapped = spProjector.GetCoordinatesForScreenPoint(gt.X,gt.Y);
                Vector2d pntOutMapped = spView.ProjectPoint(new Vector2d(AltAzMapped.X, AltAzMapped.Y));

                pntOutTarget.X = pntOutTarget.X *(512 / 2.0) + (512 / 2.0);
                pntOutTarget.Y = pntOutTarget.Y *(-512 / 2.0) + (512 / 2.0);
                pntOutMapped.X = pntOutMapped.X *(512 / 2.0) + (512 / 2.0);
                pntOutMapped.Y =  pntOutMapped.Y *(-512 / 2.0) + (512 / 2.0);

                PointTo.Add(new Vector2d(pntOutTarget.X - pntOutMapped.X, pntOutTarget.Y - pntOutMapped.Y));

            }



            //Matrix3d projMat = spView.GetCameraMatrix();
            unsafe
            {
                fastBmp.LockBitmap();
                for (int y = 0; y < 512; y++)
                {

                    for (int x = 0; x < 512; x++)
                    {
                        Vector2d pnt = spProjector.GetCoordinatesForScreenPoint(x * xFactor, y * yFactor);

                        Vector2d pntOut = spView.ProjectPoint(pnt);
                        
                        // Map
                        pntOut.X = pntOut.X * (512 / 2.0) + (512 / 2.0);
                        pntOut.Y = pntOut.Y * (-512 / 2.0) + (512 / 2.0);

                        pntOut = MapPoint(new Vector2d(x,y), pntOut, PointTo, PointFrom);

                        pntOut.X = (pntOut.X - (512 / 2.0)) / (512 / 2.0);
                        pntOut.Y = (pntOut.Y - (512 / 2.0)) / (-512 / 2.0);
                        // End Map


                        double xo = pntOut.X * (4096 / 2.0) + (4096 / 2.0);
                        double yo = pntOut.Y * (-4096 / 2.0) + (4096 / 2.0);
                        
                        int blue = (int)xo & 255;
                        int green = (int)yo & 255;
                        int red = (((int)yo) >> 4 & 240) + (((int)xo) >> 8 & 15);
                        *fastBmp[x, y] = new PixelData(red, green, blue, 255);

                    }
                }
                fastBmp.UnlockBitmap();
                

            }
            return bmp;


        }

        private static Vector2d MapPoint(Vector2d pntScan, Vector2d pntTarget, List<Vector2d> PointTo, List<Vector2d> PointFrom)
        {
            int count = PointTo.Count;
            double totalWeight = 1.0f;
            double totalX = 0;
            double totalY = 0;
            for (int i = 0; i < count; i++)
            {
                double xDist = PointFrom[i].X - pntScan.X;
                double yDist = PointFrom[i].Y - pntScan.Y;
                double dist = (xDist * xDist + yDist * yDist);
                if (dist < 1)
                {
                    dist = 10000;
                }
                else
                {
                    dist = 1 / dist * 10000;
                }

                totalX += PointTo[i].X * dist;
                totalY += PointTo[i].Y * dist;
                totalWeight += dist;

            }

            totalX /= totalWeight;
            totalY /= totalWeight;

            return new Vector2d((pntTarget.X + totalX), (pntTarget.Y + totalY));
        }

        public static Bitmap MakeBlendMap(ProjectorEntry pe, int blur, int blurIterations, float gamma)
        {


            BlendPoint pnt = new BlendPoint();
            Bitmap bmpBlend = new Bitmap(512, 512);

            double xFactor = 512.0/pe.Width;
            double yFactor = 512.0/pe.Height;
            Graphics g = Graphics.FromImage(bmpBlend);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            g.PixelOffsetMode = PixelOffsetMode.HighQuality; 

            List<PointF> points = new List<PointF>();

            foreach (BlendPoint bp in pe.BlendPolygon)
            {
                points.Add(new PointF((float)(bp.X*xFactor), (float)(bp.Y*yFactor)));
            }

            Brush brush = new SolidBrush(Color.FromArgb(255, Gamma(pe.WhiteBalance.Red, gamma), Gamma(pe.WhiteBalance.Green, gamma), Gamma(pe.WhiteBalance.Blue, gamma)));

            if (points.Count < 3)
            {
                g.Clear(Color.White);
            }
            else
            {
                g.Clear(Color.Black);

                g.FillPolygon(brush, points.ToArray());
            }
            g.Flush();
            g.Dispose();
            g = null;
            brush.Dispose();

            for (int i = 0; i < blurIterations; i++)
            {
                BlurBitmap(bmpBlend, blur);
            }

            bmpBlend = GammaBmp(bmpBlend, gamma);

           return bmpBlend;
        }

        public static int Gamma(int val, float gamma)
        {
            return (byte)Math.Min(255, (int)((255.0 * Math.Pow(val / 255.0, 1.0 / gamma)) + 0.5));
        }

        private static Bitmap GammaBmp( Bitmap bmpBlend, float gamma)
        {
            Bitmap bmpNew = new Bitmap(bmpBlend.Width, bmpBlend.Height);
            Graphics g = Graphics.FromImage(bmpNew);

            ImageAttributes attr = new ImageAttributes();
            attr.SetGamma((float)(1.0/gamma));

            g.DrawImage(bmpBlend, new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), 0, 0, bmpBlend.Width, bmpBlend.Width, GraphicsUnit.Pixel, attr);

            g.Flush();
            g.Dispose();
            bmpBlend.Dispose();
            return bmpNew;

        }

        public static void BlurBitmap(Bitmap bmp, int blur)
        {
            unsafe
            {
                if ((blur & 1) == 0)
                {
                    blur += 1;
                }

                FastBitmap fastBmp = new FastBitmap(bmp);
                Bitmap temp = new Bitmap(bmp.Height, bmp.Height);
                FastBitmap fastTemp = new FastBitmap(temp);
                fastBmp.LockBitmap();
                fastTemp.LockBitmap();


                int height = bmp.Height;
                int width = bmp.Width;

                int offset = (blur) / 2;
                int back = blur - offset;

                {
                    // First blur on x Direction
                    for (int y = 0; y < height; y++)
                    {
                        int currentRed = 0;
                        int currentGreen = 0;
                        int currentBlue = 0;
                        // prime the running sum
                        for (int x = 0 - offset; x < offset; x++)
                        {
                            PixelData* pixelIn = fastBmp[x, y];
                            currentRed += pixelIn->red;
                            currentGreen += pixelIn->green;
                            currentBlue += pixelIn->blue;
                        }

                        for (int x = offset; x < (width + back); x++)
                        {
                            // Bring in the new data
                            PixelData* pixelIn = fastBmp[x, y];
                            currentRed += pixelIn->red;
                            currentGreen += pixelIn->green;
                            currentBlue += pixelIn->blue;


                            PixelData* pixelOut = fastTemp[x - offset, y];
                            // Move the average value to the center pixel output
                            pixelOut->alpha = 255;
                            pixelOut->red = (byte)(currentRed / blur);
                            pixelOut->green = (byte)(currentGreen / blur);
                            pixelOut->blue = (byte)(currentBlue / blur);

                            // Clear out the oldest
                            pixelIn = fastBmp[x - (offset * 2), y];
                            currentRed -= pixelIn->red;
                            currentGreen -= pixelIn->green;
                            currentBlue -= pixelIn->blue;

                        }
                    }
                }
                // Now with X & Y reveresed
                {
                    for (int y = 0; y < width; y++)
                    {
                        int currentRed = 0;
                        int currentGreen = 0;
                        int currentBlue = 0;
                        // prime the running sum
                        for (int x = 0 - offset; x < offset; x++)
                        {
                            PixelData* pixelIn = fastTemp[y, x];
                            currentRed += pixelIn->red;
                            currentGreen += pixelIn->green;
                            currentBlue += pixelIn->blue;
                        }

                        for (int x = offset; x < (height + back); x++)
                        {
                            // Bring in the new data
                            PixelData* pixelIn = fastTemp[y, x];
                            currentRed += pixelIn->red;
                            currentGreen += pixelIn->green;
                            currentBlue += pixelIn->blue;


                            PixelData* pixelOut = fastBmp[y, x - offset];
                            // Move the average value to the center pixel output
                            pixelOut->alpha = 255;
                            pixelOut->red = (byte)(currentRed / blur);
                            pixelOut->green = (byte)(currentGreen / blur);
                            pixelOut->blue = (byte)(currentBlue / blur);

                            // Clear out the oldest
                            pixelIn = fastTemp[y , x - (offset * 2)];
                            currentRed -= pixelIn->red;
                            currentGreen -= pixelIn->green;
                            currentBlue -= pixelIn->blue;

                        }
                    }
                }

                
                fastTemp.UnlockBitmap();
                fastBmp.UnlockBitmap();
            }
        }
    }
}
