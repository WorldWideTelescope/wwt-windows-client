using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using SysColor = Windows.UI.Color;
#else
using SysColor = System.Drawing.Color;
#endif

using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    class Primitives3d
    {
    }

    public struct Dates
    {

        public Dates(double start, double end)
        {
            StartDate = start;
            EndDate = end;
        }
        public double StartDate;
        public double EndDate;
    }

    public class LineList
    {
        public LineList()
        {
        }
        bool zBuffer = true;

        public bool DepthBuffered
        {
            get { return zBuffer; }
            set { zBuffer = value; }
        }
        public bool TimeSeries = false;
        public bool ShowFarSide = true;
        public bool Sky = false;
        public double Decay = 0;
        public bool UseNonRotatingFrame = false;
   
        List<Vector3d> linePoints = new List<Vector3d>();
        List<SysColor> lineColors = new List<SysColor>();
        List<Dates> lineDates = new List<Dates>();
        public void AddLine(Vector3d v1, Vector3d v2, SysColor color, Dates date)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            lineColors.Add(color);
            lineDates.Add(date);
            EmptyLineBuffer();

        }

        public void AddLine(Vector3d v1, Vector3d v2, SysColor color)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            lineColors.Add(color);
            lineDates.Add(new Dates());
            EmptyLineBuffer();

        }
        
        public void Clear()
        {        
            linePoints.Clear();
            lineColors.Clear();
            lineDates.Clear();
        }
        bool usingLocalCenter = true;
        Vector3d localCenter;
        public void DrawLines(RenderContext11 renderContext, float opacity)
        {
            if (linePoints.Count < 2 || opacity <= 0)
            {
                return;
            }
            InitLineBuffer();
            Matrix3d savedWorld = renderContext.World;
            Matrix3d savedView = renderContext.View;
            if (localCenter != Vector3d.Empty)
            {
                usingLocalCenter = true;
                Vector3d temp = localCenter;
                if (UseNonRotatingFrame)
                {
                    renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBaseNonRotating * Matrix3d.Translation(-renderContext.CameraPosition);
                }
                else
                {
                    renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBase * Matrix3d.Translation(-renderContext.CameraPosition);
                }
                renderContext.View = Matrix3d.Translation(renderContext.CameraPosition) * renderContext.ViewBase;
            }

            DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;


            LineShaderNormalDates11.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
            LineShaderNormalDates11.Constants.Sky = Sky ? 1 : 0;
            LineShaderNormalDates11.Constants.ShowFarSide = ShowFarSide ? 1 : 0;
            if (TimeSeries)
            {
                LineShaderNormalDates11.Constants.Decay = (float)Decay;
            }
            else
            {
                LineShaderNormalDates11.Constants.Decay = 0;
            }

            LineShaderNormalDates11.Constants.Opacity = opacity;
            LineShaderNormalDates11.Constants.CameraPosition = new SharpDX.Vector4(Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector311, 1);
            SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            LineShaderNormalDates11.Constants.WorldViewProjection = mat;

            LineShaderNormalDates11.Use(renderContext.devContext);

            renderContext.DepthStencilMode = DepthBuffered ? DepthStencilMode.ZReadWrite : DepthStencilMode.Off;

            int index = 0;
            foreach (TimeSeriesLineVertexBuffer11 lineBuffer in lineBuffers)
            {
                renderContext.SetVertexBuffer(lineBuffer);
                renderContext.devContext.Draw(lineBuffer.Count, 0);
            }

            renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;

            if (usingLocalCenter)
            {
                renderContext.World = savedWorld;
                renderContext.View = savedView;
            }

        }

        List<TimeSeriesLineVertexBuffer11> lineBuffers = new List<TimeSeriesLineVertexBuffer11>();
        List<int> lineBufferCounts = new List<int>();

        void InitLineBuffer()
        {
            if (lineBuffers.Count == 0)
            {
                int count = linePoints.Count;

                TimeSeriesLineVertexBuffer11 lineBuffer = null;


                TimeSeriesLineVertex[] linePointList = null;
                localCenter = new Vector3d();
                if (DepthBuffered)
                {
                    // compute the local center..
                    foreach (Vector3d point in linePoints)
                    {
                        localCenter.Add(point);

                    }
                    localCenter.X /= count;
                    localCenter.Y /= count;
                    localCenter.Z /= count;
                }

                int countLeft = count;
                int index = 0;
                int counter = 0;
                Vector3d temp;

                foreach (Vector3d point in linePoints)
                {
                    if (counter >= 100000 || linePointList == null)
                    {
                        if (lineBuffer != null)
                        {
                            lineBuffer.Unlock();
                        }
                        int thisCount = Math.Min(100000, countLeft);

                        countLeft -= thisCount;
                        lineBuffer = new TimeSeriesLineVertexBuffer11(thisCount, RenderContext11.PrepDevice);

                        linePointList = (TimeSeriesLineVertex[])lineBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

                        lineBuffers.Add(lineBuffer);
                        lineBufferCounts.Add(thisCount);
                        counter = 0;
                    }


                    temp = point - localCenter;
                    linePointList[counter].Position = temp.Vector311;
                    linePointList[counter].Normal = point.Vector311;
                    linePointList[counter].Tu = (float)lineDates[index / 2].StartDate;
                    linePointList[counter].Tv = (float)lineDates[index / 2].EndDate;
                    linePointList[counter].Color = lineColors[index / 2];
                    index++;
                    counter++;
                }

                lineBuffer.Unlock();

            }
        }

        void EmptyLineBuffer()
        {
            if (lineBuffers != null)
            {
                foreach (TimeSeriesLineVertexBuffer11 lineBuffer in lineBuffers)
                {
                    lineBuffer.Dispose();
                    GC.SuppressFinalize(lineBuffer);
                }
                lineBuffers.Clear();
                lineBufferCounts.Clear();
            }

        }
    }
      
    public class SimpleLineList11
    {
        public SimpleLineList11()
        {
        }


        bool zBuffer = true;

        public bool DepthBuffered
        {
            get { return zBuffer; }
            set { zBuffer = value; }
        }

        List<Vector3d> linePoints = new List<Vector3d>();

        public void AddLine(Vector3d v1, Vector3d v2)
        {

            linePoints.Add(v1);
            linePoints.Add(v2);
            EmptyLineBuffer();

        }

        public void AddLine(Vector3 v1, Vector3 v2)
        {

            linePoints.Add(new Vector3d(v1));
            linePoints.Add(new Vector3d(v2));
            EmptyLineBuffer();

        }

        public void Clear()
        {
            linePoints.Clear();
            EmptyLineBuffer();
        }

        bool usingLocalCenter = false;
        Vector3d localCenter;
        public bool Sky = true;
        public bool aaFix = true;
        public void DrawLines(RenderContext11 renderContext, float opacity, SysColor color)
        {
            if (linePoints.Count < 2)
            {
                return;
            }

            InitLineBuffer();

          

            Matrix3d savedWorld = renderContext.World;
            Matrix3d savedView = renderContext.View;
            if (localCenter != Vector3d.Empty)
            {
                usingLocalCenter = true;
                Vector3d temp = localCenter;
                if (UseNonRotatingFrame)
                {
                    renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBaseNonRotating * Matrix3d.Translation(-renderContext.CameraPosition);
                }
                else
                {
                    renderContext.World = Matrix3d.Translation(temp) * renderContext.WorldBase * Matrix3d.Translation(-renderContext.CameraPosition);
                }
                renderContext.View = Matrix3d.Translation(renderContext.CameraPosition) * renderContext.ViewBase;
            }

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;

            SysColor col = SysColor.FromArgb((byte)(color.A * opacity), (byte)(color.R * opacity), (byte)(color.G * opacity), (byte)(color.B * opacity));


            SimpleLineShader11.Color = col;
            
            SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            SimpleLineShader11.WVPMatrix = mat;
            SimpleLineShader11.CameraPosition = Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector3;
            SimpleLineShader11.ShowFarSide = true;
            SimpleLineShader11.Sky = Sky;

            renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
            if (DepthBuffered)
            {
                renderContext.DepthStencilMode = DepthStencilMode.ZReadWrite;
            }
            else
            {
               renderContext.DepthStencilMode = DepthStencilMode.Off;
            }

            SimpleLineShader11.Use(renderContext.devContext);

            if (aaFix)
            {
                renderContext.setRasterizerState(TriangleCullMode.Off, false);
            }

            foreach (PositionVertexBuffer11 lineBuffer in lineBuffers)
            {
                renderContext.SetVertexBuffer(lineBuffer);
                renderContext.SetIndexBuffer(null);
                renderContext.devContext.Draw(lineBuffer.Count, 0);
            }

            if (aaFix)
            {
                renderContext.setRasterizerState(TriangleCullMode.Off, true);
            }

            if (usingLocalCenter)
            {
                renderContext.World = savedWorld;
                renderContext.View = savedView;
            }
        }

        List<PositionVertexBuffer11> lineBuffers = new List<PositionVertexBuffer11>();
        List<int> lineBufferCounts = new List<int>();

        void InitLineBuffer()
        {
            if (lineBuffers.Count == 0)
            {
                int count = linePoints.Count;

                PositionVertexBuffer11 lineBuffer = null;


                SharpDX.Vector3[] linePointList = null;
                localCenter = new Vector3d();
                if (DepthBuffered)
                {
                    // compute the local center..
                    foreach (Vector3d point in linePoints)
                    {
                        localCenter.Add(point);

                    }
                    localCenter.X /= count;
                    localCenter.Y /= count;
                    localCenter.Z /= count;
                }

                int countLeft = count;
                int index = 0;
                int counter = 0;
                Vector3d temp;

                foreach (Vector3d point in linePoints)
                {
                    if (counter >= 100000 || linePointList == null)
                    {
                        if (lineBuffer != null)
                        {
                            lineBuffer.Unlock();
                        }
                        int thisCount = Math.Min(100000, countLeft);

                        countLeft -= thisCount;
                        lineBuffer = new PositionVertexBuffer11(thisCount, RenderContext11.PrepDevice);

                        linePointList = (SharpDX.Vector3[])lineBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

                        lineBuffers.Add(lineBuffer);
                        lineBufferCounts.Add(thisCount);
                        counter = 0;
                    }


                    temp = point - localCenter;
                    linePointList[counter] = temp.Vector311;
                    index++;
                    counter++;
                }

                lineBuffer.Unlock();

            }
        }

        void EmptyLineBuffer()
        {
            if (lineBuffers != null)
            {
                foreach (PositionVertexBuffer11 lineBuffer in lineBuffers)
                {
                    lineBuffer.Dispose();
                    GC.SuppressFinalize(lineBuffer);
                }
                lineBuffers.Clear();
                lineBufferCounts.Clear();
            }

        }



        public bool UseNonRotatingFrame { get; set; }
    }

    
    public class TriangleList
    {
        public TriangleList()
        {
            
        }

        List<Vector3> trianglePoints = new List<Vector3>();
        List<SysColor> triangleColors = new List<SysColor>();
        List<Dates> triangleDates = new List<Dates>();

        public bool TimeSeries = false;
        public bool ShowFarSide = false;
        public bool Sky = false;
        public bool DepthBuffered = true;
        public bool WriteZbuffer = false;
        public double Decay = 0;

        public bool AutoTime = true;
        public double JNow = 0;
        bool dataToDraw = false;
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3, SysColor color, Dates date)
        {
            trianglePoints.Add(v1);
            trianglePoints.Add(v2);
            trianglePoints.Add(v3);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }
        public void AddTriangle(Vector3d v1, Vector3d v2, Vector3d v3, SysColor color, Dates date)
        {
            trianglePoints.Add(v1.Vector311);
            trianglePoints.Add(v2.Vector311);
            trianglePoints.Add(v3.Vector311);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }
        public void AddTriangle(Vector3d v1, Vector3d v2, Vector3d v3, SysColor color, Dates date, int subdivisions)
        {
            subdivisions--;

            if (subdivisions < 0)
            {
                AddTriangle(v1, v2, v3, color, date);
            }
            else
            {
                Vector3d v12;
                Vector3d v23;
                Vector3d v31;

                v12 = Vector3d.MidPointByLength(v1, v2);
                v23 = Vector3d.MidPointByLength(v2, v3);
                v31 = Vector3d.MidPointByLength(v3, v1);

                // Add 1st
                AddTriangle(v1, v12, v31, color, date, subdivisions);
                // Add 2nd
                AddTriangle(v12, v23, v31, color, date, subdivisions);
                // Add 3rd
                AddTriangle(v12, v2, v23, color, date, subdivisions);
                // Add 4th
                AddTriangle(v23, v3, v31, color, date, subdivisions);

            }
        }

        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, SysColor color, Dates date)
        {
            trianglePoints.Add(v1);
            trianglePoints.Add(v3);
            trianglePoints.Add(v2);
            trianglePoints.Add(v2);
            trianglePoints.Add(v3);
            trianglePoints.Add(v4);
            triangleColors.Add(color);
            triangleDates.Add(date);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }

        public void AddQuad(Vector3d v1, Vector3d v2, Vector3d v3, Vector3d v4, SysColor color, Dates date)
        {
            trianglePoints.Add(v1.Vector311);
            trianglePoints.Add(v3.Vector311);
            trianglePoints.Add(v2.Vector311);
            trianglePoints.Add(v2.Vector311);
            trianglePoints.Add(v3.Vector311);
            trianglePoints.Add(v4.Vector311);
            triangleColors.Add(color);
            triangleDates.Add(date);
            triangleColors.Add(color);
            triangleDates.Add(date);
            EmptyTriangleBuffer();
        }

        public void Clear()
        {

            triangleColors.Clear();
            trianglePoints.Clear();
            triangleDates.Clear();
            EmptyTriangleBuffer();

        }
        void EmptyTriangleBuffer()
        {
            if (triangleBuffers != null)
            {
                foreach (TimeSeriesLineVertexBuffer11 buf in triangleBuffers)
                {
                    buf.Dispose();
                    GC.SuppressFinalize(buf);
                }
                triangleBuffers.Clear();
                triangleBufferCounts.Clear();
                dataToDraw = false;
            }
            
        }

        List<TimeSeriesLineVertexBuffer11> triangleBuffers = new List<TimeSeriesLineVertexBuffer11>();
        List<int> triangleBufferCounts = new List<int>();

        void InitTriangleBuffer()
        {
            if (triangleBuffers.Count == 0)
            {
                int count = trianglePoints.Count;

                TimeSeriesLineVertexBuffer11 triangleBuffer = null;

                TimeSeriesLineVertex[] triPointList = null;
                int countLeft = count;
                int index = 0;
                int counter = 0;
                foreach (Vector3 point in trianglePoints)
                {
                    if (counter >= 90000 || triangleBuffer == null)
                    {
                        if (triangleBuffer != null)
                        {
                            triangleBuffer.Unlock();
                        }
                        int thisCount = Math.Min(90000, countLeft);

                        countLeft -= thisCount;
                        triangleBuffer = new TimeSeriesLineVertexBuffer11(thisCount, RenderContext11.PrepDevice);

                        triangleBuffers.Add(triangleBuffer);
                        triangleBufferCounts.Add(thisCount);
                        triPointList = (TimeSeriesLineVertex[])triangleBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
                        counter = 0;
                    }

                    triPointList[counter].Position = point;
                    triPointList[counter].Normal = point;
                    triPointList[counter].Color = triangleColors[index / 3];
                    triPointList[counter].Tu = (float)triangleDates[index / 3].StartDate;
                    triPointList[counter].Tv = (float)triangleDates[index / 3].EndDate;
                    index++;
                    counter++;
                }
                if (triangleBuffer != null)
                {
                    triangleBuffer.Unlock();
                }

                triangleColors.Clear();
                triangleDates.Clear();
                trianglePoints.Clear();

                dataToDraw = true;
            }

        }

        public enum CullMode { Clockwise, CounterClockwise, None };

        public void Draw(RenderContext11 renderContext, float opacity, CullMode cull)
        {
            renderContext.BlendMode = BlendMode.Alpha;

            if (trianglePoints.Count < 1 && !dataToDraw)
            {
                return;
            }

            InitTriangleBuffer();

            renderContext.DepthStencilMode = DepthBuffered ? (WriteZbuffer  ? DepthStencilMode.ZReadWrite : DepthStencilMode.ZReadOnly) : DepthStencilMode.Off;

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            switch (cull)
            {
                case CullMode.Clockwise:
                    renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
                    break;
                case CullMode.CounterClockwise:
                    renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);
                    break;
                case CullMode.None:
                    renderContext.setRasterizerState(TriangleCullMode.Off);
                    break;
                default:
                    break;
            }


            if (AutoTime)
            {
                DateTime baseDate = new DateTime(2010, 1, 1, 12, 00, 00);
                LineShaderNormalDates11.Constants.JNow = (float)(SpaceTimeController.JNow - SpaceTimeController.UtcToJulian(baseDate));
            }
            else
            {
                LineShaderNormalDates11.Constants.JNow = (float)JNow;
            }

            LineShaderNormalDates11.Constants.Sky = 0 ;
            LineShaderNormalDates11.Constants.ShowFarSide = ShowFarSide ? 1 : 0;
            if (TimeSeries)
            {
                LineShaderNormalDates11.Constants.Decay = (float)Decay;
            }
            else
            {
                LineShaderNormalDates11.Constants.Decay = 0;
            }
            LineShaderNormalDates11.Constants.Opacity = opacity;
            LineShaderNormalDates11.Constants.CameraPosition = new SharpDX.Vector4(Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector311, 1);

            SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            LineShaderNormalDates11.Constants.WorldViewProjection = mat;

            LineShaderNormalDates11.Use(renderContext.devContext);

            foreach (TimeSeriesLineVertexBuffer11 vertBuffer in triangleBuffers)
            {
                renderContext.SetVertexBuffer(vertBuffer);
                renderContext.devContext.Draw(vertBuffer.Count, 0);
            }
  
        }
    }
    public struct TimeSeriesLineVertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public uint color;
        public SysColor Color
        {
            get
            {
                return SysColor.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
            }
            set
            {
                color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
            }
        }   
        public float Tu;
        public float Tv;
        public TimeSeriesLineVertex(Vector3 position, Vector3 normal, float time, uint color)
        {
            Position = position;
            Normal = normal;
            Tu = time;
            Tv = 0;
            this.color = color;
        }
    }

    public enum PointScaleTypes { Linear, Power, Log, Constant, StellarMagnitude };
    public struct TimeSeriesPointVertex
    {
        public Vector3 Position;
        public float PointSize;
        public uint color;
        public SysColor Color
        {
            get
            {
                return SysColor.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
            }
            set
            {
                color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
            }
        }   
        public float Tu;
        public float Tv;
        public TimeSeriesPointVertex(Vector3 position, float size, float time, uint color)
        {
            Position = position;
            PointSize = size;
            Tu = time;
            Tv = 0;
            this.color = color;
        }
    }
}
