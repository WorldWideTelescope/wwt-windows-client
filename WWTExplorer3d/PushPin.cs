using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using TerraViewer.Properties;

namespace TerraViewer
{
    class PushPin
    {
        static readonly Dictionary<int,Texture11> pinTextureCache = new Dictionary<int,Texture11>();
        static readonly Bitmap Pins = Resources.pins;
        public static Texture11 GetPushPinTexture(int pinId)
        {
            if (pinTextureCache.ContainsKey(pinId))
            {
                return pinTextureCache[pinId];
            }

            var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

            var gOut = Graphics.FromImage(bmp);

            var row = pinId / 16;
            var col = pinId % 16;
            gOut.DrawImage(Pins, new Rectangle(0, 0, 32, 32), (col * 32), (row * 32), 32, 32, GraphicsUnit.Pixel);

            gOut.Flush();
            gOut.Dispose();
            var tex = Texture11.FromBitmap( bmp, 0xFF000000);
            bmp.Dispose();
            pinTextureCache.Add(pinId, tex);
            return tex;
        }

        static readonly Dictionary<int, Bitmap> pinBitmapCache = new Dictionary<int, Bitmap>();

        public static Bitmap GetPushPinBitmap(int pinId)
        {
            if (pinBitmapCache.ContainsKey(pinId))
            {
                return pinBitmapCache[pinId];
            }

            var bmp = new Bitmap(32, 32, PixelFormat.Format32bppArgb);

            var gOut = Graphics.FromImage(bmp);

            var row = pinId / 16;
            var col = pinId % 16;
            gOut.DrawImage(Pins, new Rectangle(0, 0, 32, 32), (col * 32), (row * 32), 32, 32, GraphicsUnit.Pixel);

            gOut.Flush();
            gOut.Dispose();
            pinBitmapCache.Add(pinId, bmp);
            return bmp;
        }

        public static void DrawAt(Graphics g, int pinId, int x, int y)
        {
            var row = pinId / 16;
            var col = pinId % 16;
            g.DrawImage(Pins, new Rectangle(x, y, 32, 32), (col * 32), (row * 32), 32, 32, GraphicsUnit.Pixel);

        }

        public static int PinCount
        {
            get
            {
                return 348;
            }
        }
    }
}
