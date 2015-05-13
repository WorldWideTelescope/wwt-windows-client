using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace WwtDataUtils
{
    public struct PixelData
    {
        public byte blue;
        public byte green;
        public byte red;
        public byte alpha;

        public PixelData(int r, int g, int b, int a) // values between 0 and 255
        {
            blue = (byte)(b >= 255 ? 255 : (b < 0 ? 0 : b));
            red = (byte)(r >= 255 ? 255 : (r < 0 ? 0 : r));
            green = (byte)(g >= 255 ? 255 : (g < 0 ? 0 : g));
            alpha = (byte)(a >= 255 ? 255 : (a < 0 ? 0 : a));
        }
    }
    public struct PixelDataRgb
    {
        public byte blue;
        public byte green;
        public byte red;

        public PixelDataRgb(int r, int g, int b) // values between 0 and 255
        {
            blue = (byte)(b >= 255 ? 255 : (b < 0 ? 0 : b));
            red = (byte)(r >= 255 ? 255 : (r < 0 ? 0 : r));
            green = (byte)(g >= 255 ? 255 : (g < 0 ? 0 : g));
        }
    }

    public unsafe class FastBitmapEnumerator : IDisposable
    {
        int x;
        int y;
        FastBitmap fastBitmap;
        PixelData* pCurrentPixel;
        bool locked;

        public FastBitmapEnumerator(FastBitmap fastBitmap)
        {
            fastBitmap.LockBitmap();
            locked = true;
            this.fastBitmap = fastBitmap;
            x = -1;
            y = 0;
            pCurrentPixel = fastBitmap[x, y];
        }

        public void Dispose()
        {
            if (locked)
            {
                fastBitmap.UnlockBitmap();
            }
        }

        public bool MoveNext()
        {
            x++;
            pCurrentPixel++;
            if (x == fastBitmap.Size.X)
            {
                y++;
                if (y == fastBitmap.Size.Y)
                {
                    return false;
                }
                else
                {
                    x = 0;
                    pCurrentPixel = fastBitmap[0, y];
                    //Debug.WriteLine(String.Format("{0}", pCurrentPixel - fastBitmap[0, 0]));
                }
            }
            return true;
        }

        public PixelData* Current
        {
            get
            {
                return pCurrentPixel;
            }
        }


    }

    /// <summary>
    /// A bitmap class that allows fast x, y access 
    /// </summary>
    public unsafe class FastBitmap
    {
        Bitmap bitmap;

        // three elements used for MakeGreyUnsafe
        int width;
        BitmapData bitmapData = null;
        Byte* pBase = null;
        PixelData* pCurrentPixel = null;
        int xLocation;
        int yLocation;
        Point size;
        internal bool locked = false;

        /// <summary>
        /// Create an instance from an existing bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap</param>
        public FastBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;

            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF bounds = bitmap.GetBounds(ref unit);

            size = new Point((int)bounds.Width, (int)bounds.Height);
        }

        /// <summary>
        /// Save the bitmap to a file
        /// </summary>
        /// <param name="filename">Filename to save the bitmap to</param>
        public void Save(string filename)
        {
            bitmap.Save(filename, ImageFormat.Jpeg);
        }

        public void Dispose()
        {
            bitmap.Dispose();
        }

        /// <summary>
        /// Size of the bitmap in pixels
        /// </summary>
        public Point Size
        {
            get
            {
                return size;
            }
        }

        /// <summary>
        /// The Bitmap object wrapped by this instance
        /// </summary>
        public Bitmap Bitmap
        {
            get
            {
                return (bitmap);
            }
        }

        /// <summary>
        /// Start at the beginning of the bitmap
        /// </summary>
        public void InitCurrentPixel()
        {
            LockBitmap();
            pCurrentPixel = (PixelData*)pBase;
        }

        /// <summary>
        /// Return the next pixel
        /// </summary>
        /// <returns>The next pixel, or null if done</returns>
        public PixelData* GetNextPixel()
        {
            PixelData* pReturnPixel = pCurrentPixel;
            if (xLocation == size.X)
            {
                xLocation = 0;
                yLocation++;
                if (yLocation == size.Y)
                {
                    UnlockBitmap();
                    return null;
                }
                else
                {
                    pCurrentPixel = this[0, yLocation];
                }
            }
            else
            {
                xLocation++;
                pCurrentPixel++;
            }
            return pReturnPixel;
        }

        /// <summary>
        /// Get the pixel data at a specific x and y location
        /// </summary>
        public PixelData* this[int x, int y]
        {
            get
            {
                x = Math.Min(Math.Max(0, x), size.X - 1);
                y = Math.Min(Math.Max(0, y), size.Y - 1);
                return (PixelData*)(pBase + y * width + x * sizeof(PixelData));
            }
        }

        public PixelDataRgb* GetRgbPixel(int x, int y)
        {
            return (PixelDataRgb*)(pBase + y * width + x * 3);
        }

        public FastBitmapEnumerator GetEnumerator()
        {
            return new FastBitmapEnumerator(this);
        }

        /// <summary>
        /// Lock the bitmap. 
        /// </summary>
        public void LockBitmap()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            width = (int)boundsF.Width * sizeof(PixelData);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }

            bitmapData =
                bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
            locked = true;
        }

        public void LockBitmapRgb()
        {
            GraphicsUnit unit = GraphicsUnit.Pixel;
            RectangleF boundsF = bitmap.GetBounds(ref unit);
            Rectangle bounds = new Rectangle((int)boundsF.X,
                (int)boundsF.Y,
                (int)boundsF.Width,
                (int)boundsF.Height);

            // Figure out the number of bytes in a row
            // This is rounded up to be a multiple of 4
            // bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            width = (int)boundsF.Width * sizeof(PixelDataRgb);
            if (width % 4 != 0)
            {
                width = 4 * (width / 4 + 1);
            }

            bitmapData =
                bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            pBase = (Byte*)bitmapData.Scan0.ToPointer();
            locked = true;
        }
        /// <summary>
        /// Unlock the bitmap
        /// </summary>
        public void UnlockBitmap()
        {
            bitmap.UnlockBits(bitmapData);
            bitmapData = null;
            pBase = null;
            locked = false;
        }

        public PixelData GetFilteredPixel(double xd, double yd)
        {
            int x = (int)(xd);

            double xr = xd - (double)x;

            int y = (int)(yd);

            double yr = yd - (double)y;

            if (x < 0 || x > (bitmap.Width - 1) || y < 0 || y > (bitmap.Height - 1))
            {
                return new PixelData();
            }

            int stepX = 1;
            int stepY = 1;

            if (x == bitmap.Width - 1)
            {
                stepX = 0;
            }
            if (y == bitmap.Height - 1)
            {
                stepY = 0;
            }

            PixelData* tl = this[x, y];
            PixelData* tr = this[x + stepX, y];
            PixelData* bl = this[x, y + stepY];
            PixelData* br = this[x + stepX, y + stepY];

            PixelData result;

            result.alpha = (byte)((((((double)tl->alpha * (1.0 - xr)) + ((double)tr->alpha * xr))) * (1.0 - yr)) +
                             (((((double)bl->alpha * (1.0 - xr)) + ((double)br->alpha * xr))) * yr));
            result.red = (byte)((((((double)tl->red * (1.0 - xr)) + ((double)tr->red * xr))) * (1.0 - yr)) +
                             (((((double)bl->red * (1.0 - xr)) + ((double)br->red * xr))) * yr));
            result.green = (byte)((((((double)tl->green * (1.0 - xr)) + ((double)tr->green * xr))) * (1.0 - yr)) +
                             (((((double)bl->green * (1.0 - xr)) + ((double)br->green * xr))) * yr));
            result.blue = (byte)((((((double)tl->blue * (1.0 - xr)) + ((double)tr->blue * xr))) * (1.0 - yr)) +
                             (((((double)bl->blue * (1.0 - xr)) + ((double)br->blue * xr))) * yr));


            return result;
        }


    }
}
