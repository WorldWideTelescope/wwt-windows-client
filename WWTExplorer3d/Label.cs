using System;
using System.Collections.Generic;

#if WINDOWS_UWP
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
#endif

using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    public enum LabelSytle {Arrow, Telrad};
    public class SkyLabel : IDisposable
    {
        static Texture11 texture;
        public double RA;
        public double Dec;
        public double Distance;
        public string Text;
        public LabelSytle Style;
        Vector3d pos;
        Vector3 center = new Vector3(0, 0, 0);
        //Rectangle rect = new Rectangle(0, 0, 20, 20);

        Text3dBatch textBatch = null;
        public SkyLabel(RenderContext11 renderContext, double ra, double dec, string text, LabelSytle style, double distance)
        {
            RA = ra;
            Dec = dec;
            Text = text;
            Style = style;

            Distance = distance;

            if (texture == null)
            {
                texture = Texture11.FromBitmap(Properties.Resources.circle, 0);
            }

            Vector3d up = new Vector3d();
            Vector3d textPos = new Vector3d();
            if (Earth3d.MainWindow.SolarSystemMode)
            {
                pos = Coordinates.RADecTo3d(ra, -dec, distance);
                up = Coordinates.RADecTo3d(ra, -dec + 90, distance);

                pos.RotateX(Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI);
                pos.Add(Planets.GetPlanet3dLocation(SolarSystemObjects.Earth));
                
                up.RotateX(Coordinates.MeanObliquityOfEcliptic(SpaceTimeController.JNow) / 180.0 * Math.PI);
                up.Add(Planets.GetPlanet3dLocation(SolarSystemObjects.Earth));
           }
            else
            {
                pos = Coordinates.RADecTo3d(ra+12, dec, distance);
                textPos = Coordinates.RADecTo3d(ra + 12, dec + 2, distance);
                up = Coordinates.RADecTo3d(ra+12, dec + 92, distance);

            }
            center = new Vector3(9, 9, 0);

            textBatch = new Text3dBatch(24);
            if (style == LabelSytle.Telrad)
            {
                // Telrad-style labels are always screen-aligned
                Text3d t3 = new Text3d(new Vector3d(0, 0, 0.1), new Vector3d(0, 1, 0), text, 20, .01);
                t3.alignment = Text3d.Alignment.Left;
                textBatch.Add(t3);
            }
            else
            {
                up.Normalize();
                textPos.Normalize();
                textBatch.Add(new Text3d(textPos, up, text, 20, .0005));
            }
        }

        public SkyLabel(RenderContext11 renderContext, Vector3d point, string text, LabelSytle style)
        {

            Text = text;
            Style = style;

            if (texture == null)
            {
                texture = Texture11.FromBitmap(Properties.Resources.circle, 0);
            }


            pos = point;

            center = new Vector3(9, 9, 0);

            textBatch = new Text3dBatch(80);

            if (style == LabelSytle.Telrad)
            {
                // Telrad-style labels are always screen-aligned
                Text3d t3 = new Text3d(new Vector3d(0, 0, 0.1), new Vector3d(0, 1, 0), text, 20, .01);
                t3.alignment = Text3d.Alignment.Left;
                textBatch.Add(t3);
            }
            else
            {
                // This will produce sky or orbit aligned text
                textBatch.Add(new Text3d(pos, new Vector3d(0,1,0), text, 20, .01));
            }
        }

        public void Draw(RenderContext11 renderContext, bool space3d)
        {
            Vector3d cam = Vector3d.TransformCoordinate(Earth3d.MainWindow.RenderContext11.CameraPosition, Matrix3d.Invert(RenderEngine.WorldMatrix));

            if (!space3d)
            {
                if (Vector3d.Dot(cam, pos) < 0)
                {
                    return;
                }
            }
            Vector3d temp = pos;

            if (Earth3d.MainWindow.SolarSystemMode)
            {
                temp.Add( RenderEngine.Engine.viewCamera.ViewTarget);
            }

            Matrix3d wvp = renderContext.World * renderContext.View * renderContext.Projection;

            Vector3 screenPos = Vector3.Project(temp.Vector311, renderContext.ViewPort.X, renderContext.ViewPort.Y, renderContext.ViewPort.Width, renderContext.ViewPort.Height, 0, 1, wvp.Matrix11);
            
            // Get the w component of the transformed object position; if it's negative the
            // object is behind the viewer.
            double w = wvp.M14 * temp.X + wvp.M24 * temp.Y + wvp.M34 * temp.Z + wvp.M44;
            if (w < 0.0 && Earth3d.MainWindow.SolarSystemMode)
            {
                // Don't show labels that are behind the viewer
                return;
            }


            screenPos = new Vector3((float)(int)screenPos.X, (float)(int)screenPos.Y, 1);

            Sprite2d.Draw2D(renderContext, texture, new SizeF(20, 20), new PointF(0, 0), 0, new PointF(screenPos.X, screenPos.Y), Color.White);

            if (Earth3d.MainWindow.SolarSystemMode || Style == LabelSytle.Telrad)
            {
                Matrix3d worldMatrix = renderContext.World;
                Matrix3d viewMatrix = renderContext.View;
                Matrix3d projectionMatrix = renderContext.Projection;

                double labelScale = Earth3d.MainWindow.SolarSystemMode ? 8.0 : 30.0;
                renderContext.World =
                    Matrix3d.Scaling(labelScale, labelScale, 1.0) *
                    Matrix3d.Translation(screenPos.X + 10.0, -screenPos.Y, 0.0) *
                    Matrix3d.Translation(-renderContext.ViewPort.Width / 2, renderContext.ViewPort.Height / 2, 0);
                renderContext.View = Matrix3d.Identity;
                renderContext.Projection = Matrix3d.OrthoLH(renderContext.ViewPort.Width, renderContext.ViewPort.Height, 1, -1);

                renderContext.BlendMode = BlendMode.PremultipliedAlpha;
                textBatch.Draw(renderContext, 1, Color.White);

                renderContext.World = worldMatrix;
                renderContext.View = viewMatrix;
                renderContext.Projection = projectionMatrix;
            }
            else
            {
                renderContext.BlendMode = BlendMode.PremultipliedAlpha;
                textBatch.Draw(renderContext, 1, Color.White);
            }

            //todo11 Implement this 
            //sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortTexture);

            //sprite.Draw(texture, rect, center, screenPos, Color.White);
            //Rectangle recttext = new Rectangle((int)(screenPos.X + 15), (int)(screenPos.Y - 8), 0, 0);
            //Earth3d.MainWindow.labelFont.DrawText(sprite, Text, recttext,
            //DrawTextFormat.NoClip, System.Drawing.Color.White);


            //sprite.End();

        }


        #region IDisposable Members

        public void Dispose()
        {
            if (textBatch != null)
            {
                textBatch.Dispose();
            }

        }

        #endregion
    }


  
}
