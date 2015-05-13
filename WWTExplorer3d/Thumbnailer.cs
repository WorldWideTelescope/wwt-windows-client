using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace TerraViewer
{
    class Thumbnailer
    {
        public static Bitmap MakeThumbnail(Bitmap imgOrig, int x, int y)
        {
            try
            {
                Bitmap bmpThumb = new Bitmap(x, y);

                Graphics g = Graphics.FromImage(bmpThumb);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                double imageAspect = ((double)imgOrig.Width) / (imgOrig.Height);

                double clientAspect = ((double)bmpThumb.Width) / bmpThumb.Height;

                int cw = imgOrig.Width;
                int ch = imgOrig.Height;

                if (imageAspect < clientAspect)
                {
                    ch = (int)((double)cw / imageAspect);
                }
                else
                {
                    cw = (int)((double)ch * imageAspect);
                }

                int cx = (imgOrig.Width - cw) / 2;
                int cy = ((imgOrig.Height - ch) / 2);// - 1;
                Rectangle srcRect = new Rectangle(cx, cy, cw, ch);//+ 1);

                Rectangle destRect = new Rectangle(0, 0, x, y);
                g.DrawImage(imgOrig, destRect, srcRect, System.Drawing.GraphicsUnit.Pixel);
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
