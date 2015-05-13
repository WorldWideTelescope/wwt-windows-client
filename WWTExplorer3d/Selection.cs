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

namespace TerraViewer
{
    public class Selection
    {
        Texture11 SingleSelectHandles = null;
        Texture11 MultiSelectHandles = null;
        Texture11 FocusHandles = null;
        public Selection()
        {
        }
        List<Overlay> selectionSet = new List<Overlay>();

        public Overlay[] SelectionSet
        {
            get { return selectionSet.ToArray(); }
        }

        public void ClearSelection()
        {
            selectionSet.Clear();
        }

        public void AddSelection(Overlay overlay)
        {
            if (overlay != null)
            {
                if(!selectionSet.Contains(overlay))
                {
                    selectionSet.Add(overlay);
                }
               
            }
        }

        public void AddSelection(Overlay[] overlays)
        {
            selectionSet.AddRange(overlays);
        }

        public bool IsOverlaySelected(Overlay overlay)
        {
            return selectionSet.Contains(overlay);
        }
   
        public void SetSelection(Overlay overlay)
        {
            selectionSet.Clear();
            if (overlay != null)
            {
                selectionSet.Add(overlay);
            }
        }

        public bool MultiSelect
        {
            get
            {
                return selectionSet.Count > 1;
            }
        }

        public void SetSelection(Overlay[] overlays)
        {
            selectionSet.Clear();
            selectionSet.AddRange(overlays);
        }

        Overlay focus = null;

        public Overlay Focus
        {
            get { return focus; }
            set { focus = value; }
        }


        float ratio = 1.0f;
        public void Draw3D(RenderContext11 renderContext, float transparancy)
        {
            ratio = 1116f / (float)Earth3d.MainWindow.RenderWindow.ClientRectangle.Height;

            if (SingleSelectHandles == null)
            {
                SingleSelectHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, (Bitmap)global::TerraViewer.Properties.Resources.Selhand);
            }

            if (MultiSelectHandles == null)
            {
                MultiSelectHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, (Bitmap)global::TerraViewer.Properties.Resources.MultiSelhand);
            }

            if (FocusHandles == null)
            {
                FocusHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, (Bitmap)global::TerraViewer.Properties.Resources.FocusHandles);
            }

            if (selectionSet.Count > 1)
            {
                foreach (Overlay overlay in selectionSet)
                {
                    if (overlay == focus)
                    {

                        DrawSelectionHandles(renderContext, overlay, FocusHandles);
                    }
                    else
                    {

                        DrawSelectionHandles(renderContext, overlay, MultiSelectHandles);
                    }
                }
            }
            else
            {
                foreach (Overlay overlay in selectionSet)
                {
                    DrawSelectionHandles(renderContext, overlay, SingleSelectHandles);
                }
            }
        }

        private static PositionColoredTextured[] points = new PositionColoredTextured[9 * 3 * 2];
        private void DrawSelectionHandles(RenderContext11 renderContext, Overlay overlay, Texture11 handleTexture)
        {
            RectangleF[] handles = MakeHandles(overlay);
            float angle = overlay.RotationAngle;
            
            int i = 0;
            int j = 0;
            foreach (RectangleF handle in handles)
            {
                points[i + 0].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Top - centerY, angle).Vector4;
                points[i + 0].Tu = j * (1f / 9f);
                points[i + 0].Tv = 0;
                points[i + 0].Color = Color.White;


                points[i + 1].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Top - centerY, angle).Vector4;
                points[i + 1].Tu = (j + 1) * (1f / 9f);
                points[i + 1].Tv = 0;
                points[i + 1].Color = Color.White;

                points[i + 2].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Bottom - centerY, angle).Vector4;
                points[i + 2].Tu = j * (1f / 9f);
                points[i + 2].Tv = 1;
                points[i + 2].Color = Color.White;

                points[i + 3].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Top - centerY, angle).Vector4;
                points[i + 3].Tu = (j + 1) * (1f / 9f);
                points[i + 3].Tv = 0;
                points[i + 3].Color = Color.White;


                points[i + 4].Position = overlay.MakePosition(centerX, centerY, handle.Right - centerX, handle.Bottom - centerY, angle).Vector4;
                points[i + 4].Tu = (j + 1) * (1f / 9f);
                points[i + 4].Tv = 1;
                points[i + 4].Color = Color.White;


                points[i + 5].Position = overlay.MakePosition(centerX, centerY, handle.Left - centerX, handle.Bottom - centerY, angle).Vector4;
                points[i + 5].Tu = j * (1f / 9f);
                points[i + 5].Tv = 1;
                points[i + 5].Color = Color.White;

                i += 6;
                j++;
            }

            if (MultiSelect)
            {
                Sprite2d.Draw(renderContext, points, points.Length - 6, handleTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            }
            else
            {
                Sprite2d.Draw(renderContext, points, points.Length, handleTexture, SharpDX.Direct3D.PrimitiveTopology.TriangleList);
            }
        }

        public PointF PointToSelectionSpace(PointF pntIn)
        {
            System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();

            PointF[] tempPoints = new PointF[1];
            tempPoints[0] = new PointF(pntIn.X, pntIn.Y);

            mat.RotateAt(-selectionSet[0].RotationAngle, new PointF((float)(selectionSet[0].X ), (float)(selectionSet[0].Y )));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }

        public PointF PointToScreenSpace(PointF pntIn)
        {
            System.Drawing.Drawing2D.Matrix mat = new System.Drawing.Drawing2D.Matrix();

            PointF[] tempPoints = new PointF[1];
            tempPoints[0] = new PointF(pntIn.X, pntIn.Y);

            mat.RotateAt(selectionSet[0].RotationAngle, new PointF((float)(selectionSet[0].X ), (float)(selectionSet[0].Y )));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }    
        
        public SelectionAnchor HitTest(PointF position)
        {
            if (selectionSet.Count == 1)
            {
                foreach (Overlay overlay in selectionSet)
                {
                    RectangleF[] handles = MakeHandles(overlay);
                    int index = 0;

                    PointF testPoint = PointToSelectionSpace(position);
                    foreach (RectangleF rectf in handles)
                    {
                        if (rectf.Contains(testPoint))
                        {
                            return (SelectionAnchor)index;
                        }
                        index++;
                    }
                }
            }

            return SelectionAnchor.None;

        }

        float centerX = 0;
        float centerY = 0;
        public RectangleF[] MakeHandles(Overlay overlay)
        {
            float x = (float)((int)(overlay.X-(overlay.Width/2)))+.5f;
            float y = ((int)overlay.Y-(overlay.Height/2))+.5f;

            centerX = overlay.X;
            centerY = overlay.Y;

            float width = overlay.Width;
            float height = overlay.Height;
            float handleSize = 12*ratio;
            RectangleF[] handles = new RectangleF[9];

            handles[0] = new RectangleF(x - handleSize, y - handleSize, handleSize, handleSize);
            handles[1] = new RectangleF((x + (width / 2)) - (handleSize/2), y - handleSize, handleSize, handleSize);
            handles[2] = new RectangleF(x + width, y - handleSize, handleSize, handleSize);
            handles[3] = new RectangleF(x + width, (y + (height / 2)) - (handleSize / 2), handleSize, handleSize);
            handles[4] = new RectangleF(x + width, (y + height), handleSize, handleSize);
            handles[5] = new RectangleF((x + (width / 2)) - (handleSize / 2), (y + height), handleSize, handleSize);
            handles[6] = new RectangleF(x - handleSize, (y + height), handleSize, handleSize);
            handles[7] = new RectangleF(x - handleSize, (y + (height / 2)) - (handleSize / 2), handleSize, handleSize);
            handles[8] = new RectangleF((x + (width / 2)) - (handleSize / 2), y - 30f*ratio, handleSize, handleSize);
            return handles;
        }

    }
    public enum SelectionAnchor { TopLeft, Top, TopRight, Right, BottomRight, Bottom, BottomLeft, Left, Rotate, Move, Center, None };
 
}
