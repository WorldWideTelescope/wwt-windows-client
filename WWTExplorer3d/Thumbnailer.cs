using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TerraViewer
{
    class Thumbnailer
    {
        public static Bitmap MakeThumbnail(Bitmap imgOrig, int x, int y)
        {
            try
            {
                var bmpThumb = new Bitmap(x, y);

                var g = Graphics.FromImage(bmpThumb);

                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                var clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                var cw = imgOrig.Width;
                var ch = imgOrig.Height;

                if (imageAspect < clientAspect)
                {
                    ch = (int)(cw / imageAspect);
                }
                else
                {
                    cw = (int)(ch * imageAspect);
                }

                var cx = (imgOrig.Width - cw) / 2;
                var cy = ((imgOrig.Height - ch) / 2);// - 1;
                var srcRect = new Rectangle(cx, cy, cw, ch);//+ 1);

                var destRect = new Rectangle(0, 0, x, y);
                g.DrawImage(imgOrig, destRect, srcRect, GraphicsUnit.Pixel);
                g.Dispose();
                GC.SuppressFinalize(g);
                imgOrig.Dispose();
                GC.SuppressFinalize(imgOrig);

                return bmpThumb;
            }
            catch
            {
                return null;
            }
        }
    }
}
