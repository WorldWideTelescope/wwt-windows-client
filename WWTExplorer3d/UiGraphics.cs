using System;
using System.Collections.Generic;

#if WINDOWS_UWP
using SysColor = TerraViewer.Color;

#else
using SysColor = System.Drawing.Color;
using System.Drawing;

#endif
namespace TerraViewer
{
    public class UiGraphics
    {
        RenderContext11 renderContext;
        Text3dBatch batch = new Text3dBatch(12);
        public UiGraphics(RenderContext11 renderContext)
        {
            this.renderContext = renderContext;
        }

        public enum TextAlignment {  Left, Center, Right};

        //internal void DrawImage(Texture11 texture11, int x, int y)
        //{
        //    if (texture11.Texture != null)
        //    {
        //        PositionColoredTextured[] points = new PositionColoredTextured[4];
        //        int width = texture11.Width;
        //        int height = texture11.Height;
        //        points[0] = new PositionColoredTextured(x, y, 0, SysColor.White, 1, 0, 0);
        //        points[3] = new PositionColoredTextured(x + width, y + height, 0, SysColor.White, 1, 1, 1);
        //        points[2] = new PositionColoredTextured(x, y + height, 0, SysColor.White, 1, 0, 1);
        //        points[1] = new PositionColoredTextured(x + width, y, 0, SysColor.White, 1, 1, 0);
        //        Sprite2d.Draw(renderContext, points, 4, texture11, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
        //    }
        //}
        internal void DrawImage(Bitmap bitmap, int x, int y)
        {
            var texture11 = UiGraphics.GetTextureForThumbnail(bitmap);
            if (texture11.Texture != null)
            {
                PositionColoredTextured[] points = new PositionColoredTextured[4];
                int width = texture11.Width;
                int height = texture11.Height;
                points[0] = new PositionColoredTextured(x, y, 0, SysColor.White, 1, 0, 0);
                points[3] = new PositionColoredTextured(x + width, y + height, 0, SysColor.White, 1, 1, 1);
                points[2] = new PositionColoredTextured(x, y + height, 0, SysColor.White, 1, 0, 1);
                points[1] = new PositionColoredTextured(x + width, y, 0, SysColor.White, 1, 1, 0);
                Sprite2d.Draw(renderContext, points, 4, texture11, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
            }
        }
        internal void DrawString(string text, int fontSize, SysColor color, RectangleF rectf, TextAlignment alignment)
        {
            Text3d text3d = new Text3d(
                new Vector3d(0, 0, -1),
                alignment == TextAlignment.Left
                          ? new Vector3d(rectf.X, rectf.Y + rectf.Height / 2, 0)
                          : new Vector3d(rectf.X + rectf.Width / 2, rectf.Y + rectf.Height / 2, 0),
                new Vector3d(0, -1, 0), text, fontSize, 1.5);
            text3d.alignment = alignment == TextAlignment.Left ? Text3d.Alignment.Left : Text3d.Alignment.Center;
            if (alignment == TextAlignment.Left)
            {
                text3d.TextLength = rectf.Width;
            }
            batch.Add(text3d);
        }

        internal void DrawImage(Texture11 bmpThumb, Rectangle dst, Rectangle src, GraphicsUnit pixel)
        {
            PositionColoredTextured[] points = new PositionColoredTextured[4];
            int x = dst.X;
            int y = dst.Y;
            int width = bmpThumb.Width;
            int height = bmpThumb.Height;

            float srcX = (float)src.X / (float)width;
            float srcY = (float)src.Y / (float)height;

            float srcX1 = srcX + ((float)src.Width / (float)width);
            float srcY1 = srcY + ((float)src.Height / (float)height);

            points[0] = new PositionColoredTextured(x, y, 0, SysColor.White, 1, srcX, 1-srcY1);
            points[3] = new PositionColoredTextured(x + dst.Width, y + dst.Height, 0, SysColor.White, 1, srcX1, 1-srcY);
            points[2] = new PositionColoredTextured(x, y + dst.Height, 0, SysColor.White, 1, srcX, 1-srcY);
            points[1] = new PositionColoredTextured(x + dst.Width, y, 0, SysColor.White, 1, srcX1, 1-srcY1);
            Sprite2d.Draw(renderContext, points, 4, bmpThumb, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
        }

        internal void DrawImage(Bitmap bmp, Rectangle dst, Rectangle src, GraphicsUnit pixel, SysColor color)
        {

            var texture11 = UiGraphics.GetTextureForThumbnail(bmp);
            if (texture11 != null)
            {
                PositionColoredTextured[] points = new PositionColoredTextured[4];
                int x = dst.X;
                int y = dst.Y;
                int width = src.Width;
                int height = src.Height;

                float srcX = (float)src.X / (float)width;
                float srcY = (float)src.Y / (float)height;

                float srcX1 = srcX + ((float)src.Width / (float)width);
                float srcY1 = srcY + ((float)src.Height / (float)height);

                points[0] = new PositionColoredTextured(x, y, 0, color, 1, srcX, 1 - srcY1);
                points[3] = new PositionColoredTextured(x + dst.Width, y + dst.Height, 0, color, 1, srcX1, 1 - srcY);
                points[2] = new PositionColoredTextured(x, y + dst.Height, 0, color, 1, srcX, 1 - srcY);
                points[1] = new PositionColoredTextured(x + dst.Width, y, 0, color, 1, srcX1, 1 - srcY1);
                Sprite2d.Draw(renderContext, points, 4, texture11, SharpDX.Direct3D.PrimitiveTopology.TriangleStrip);
            }
        }

        public void Prep()
        {
            batch.CleanUp();
        }

        public void Flush()
        {
            batch.Draw(renderContext, 1, SysColor.White);
        }


        static Dictionary<Bitmap, Texture11> map = new Dictionary<Bitmap, Texture11>();

        static private Texture11 GetTextureForThumbnail(Bitmap bmp)
        {
            if (bmp == null)
            {
                return null;
            }

            if (!map.ContainsKey(bmp))
            {
                map[bmp] = UiTools.TextureFromBitmap(bmp);
            }
            var texture =  map[bmp];
#if WINDOWS_UWP
            bmp.Width = texture.Width;
            bmp.Height = texture.Height;
#endif
            return texture;
        }
    }
}
