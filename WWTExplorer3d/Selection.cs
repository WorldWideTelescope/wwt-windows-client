using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using SharpDX.Direct3D;
using TerraViewer.Properties;

namespace TerraViewer
{
    public class Selection
    {
        Texture11 SingleSelectHandles;
        Texture11 MultiSelectHandles;
        Texture11 FocusHandles;

        readonly List<Overlay> selectionSet = new List<Overlay>();

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

        Overlay focus;

        public Overlay Focus
        {
            get { return focus; }
            set { focus = value; }
        }


        float ratio = 1.0f;
        public void Draw3D(RenderContext11 renderContext, float transparancy)
        {
            ratio = 1116f / Earth3d.MainWindow.RenderWindow.ClientRectangle.Height;

            if (SingleSelectHandles == null)
            {
                SingleSelectHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, Resources.Selhand);
            }

            if (MultiSelectHandles == null)
            {
                MultiSelectHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, Resources.MultiSelhand);
            }

            if (FocusHandles == null)
            {
                FocusHandles = Texture11.FromBitmap(RenderContext11.PrepDevice, Resources.FocusHandles);
            }

            if (selectionSet.Count > 1)
            {
                foreach (var overlay in selectionSet)
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
                foreach (var overlay in selectionSet)
                {
                    DrawSelectionHandles(renderContext, overlay, SingleSelectHandles);
                }
            }
        }

        private static readonly PositionColoredTextured[] points = new PositionColoredTextured[9 * 3 * 2];
        private void DrawSelectionHandles(RenderContext11 renderContext, Overlay overlay, Texture11 handleTexture)
        {
            var handles = MakeHandles(overlay);
            var angle = overlay.RotationAngle;
            
            var i = 0;
            var j = 0;
            foreach (var handle in handles)
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
                Sprite2d.Draw(renderContext, points, points.Length - 6, handleTexture, PrimitiveTopology.TriangleList);
            }
            else
            {
                Sprite2d.Draw(renderContext, points, points.Length, handleTexture, PrimitiveTopology.TriangleList);
            }
        }

        public PointF PointToSelectionSpace(PointF pntIn)
        {
            var mat = new Matrix();

            var tempPoints = new PointF[1];
            tempPoints[0] = new PointF(pntIn.X, pntIn.Y);

            mat.RotateAt(-selectionSet[0].RotationAngle, new PointF(selectionSet[0].X, selectionSet[0].Y));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }

        public PointF PointToScreenSpace(PointF pntIn)
        {
            var mat = new Matrix();

            var tempPoints = new PointF[1];
            tempPoints[0] = new PointF(pntIn.X, pntIn.Y);

            mat.RotateAt(selectionSet[0].RotationAngle, new PointF(selectionSet[0].X, selectionSet[0].Y));
            mat.TransformPoints(tempPoints);
            return tempPoints[0];
        }    
        
        public SelectionAnchor HitTest(PointF position)
        {
            if (selectionSet.Count == 1)
            {
                foreach (var overlay in selectionSet)
                {
                    var handles = MakeHandles(overlay);
                    var index = 0;

                    var testPoint = PointToSelectionSpace(position);
                    foreach (var rectf in handles)
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

        float centerX;
        float centerY;
        public RectangleF[] MakeHandles(Overlay overlay)
        {
            var x = (int)(overlay.X-(overlay.Width/2))+.5f;
            var y = ((int)overlay.Y-(overlay.Height/2))+.5f;

            centerX = overlay.X;
            centerY = overlay.Y;

            var width = overlay.Width;
            var height = overlay.Height;
            var handleSize = 12*ratio;
            var handles = new RectangleF[9];

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
