using System;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace TerraViewer
{
    public abstract class AbstractSpriteSet : IDisposable
    {
        // We have three strategies for drawing point sprites. In order of preference:
        //    GeometryShader - minimum storage and processing time; requires D3D10+
        //    Instanced - same storage as GeometryShader, but 4x processing time; requires D3D9 feature level 3
        //    Fallback - 4x storage and processing time of GeometryShader strategy
        public enum RenderStrategy
        {
            GeometryShader,
            Instanced,
            Fallback
        }

        protected RenderStrategy renderStrategy;

        readonly int count;

        private Vector3 pointScaleFactors = new Vector3(0.0f, 0.0f, 10000.0f);

        protected IVertexBuffer11 vertexBuffer;
        protected IVertexBuffer11 fallbackVertexBuffer;
        protected GenVertexBuffer<CompatibilityPointSpriteShader.CornerVertex> spriteCornerVertexBuffer;
        protected IndexBuffer11 indexBuffer;

        private Color4 tintColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);

        public AbstractSpriteSet(Device device, Array points)
        {
            if (RenderContext11.Downlevel)
            {
                if (RenderContext11.SupportsInstancing)
                {
                    renderStrategy = RenderStrategy.Instanced;
                }
                else
                {
                    renderStrategy = RenderStrategy.Fallback;
                }
            }
            else
            {
                renderStrategy = RenderStrategy.GeometryShader;
            }
            
            count = points.Length;

            createAuxilliaryBuffers(device);
        }

        abstract protected void setupShader(RenderContext11 renderContext, Texture11 texture, float opacity);

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool Downlevel
        {
            get
            {
                return renderStrategy != RenderStrategy.GeometryShader;
            }
        }

        public bool Instanced
        {
            get
            {
                return renderStrategy == RenderStrategy.Instanced;
            }
        }

        public Vector3 PointScaleFactors
        {
            get
            {
                return pointScaleFactors;
            }

            set
            {
                pointScaleFactors = value;
            }
        }

        public Color4 TintColor
        {
            get
            {
                return tintColor;
            }

            set
            {
                tintColor = value;
            }
        }

        private void createAuxilliaryBuffers(Device device)
        {
            if (renderStrategy == RenderStrategy.Instanced)
            {
                createSpriteCornersBuffer(device);
                createSpriteCornersIndexBuffer(device);
            }
            else if (renderStrategy == RenderStrategy.Fallback)
            {
                createIndexBuffer(device, count);
            }
        }

        private void createIndexBuffer(Device device, int pointCount)
        {
            var indices = new uint[pointCount * 6];
            for (uint i = 0; i < pointCount; ++i)
            {
                indices[i * 6 + 0] = i * 4 + 0;
                indices[i * 6 + 1] = i * 4 + 1;
                indices[i * 6 + 2] = i * 4 + 2;
                indices[i * 6 + 3] = i * 4 + 0;
                indices[i * 6 + 4] = i * 4 + 2;
                indices[i * 6 + 5] = i * 4 + 3;
            }

            indexBuffer = new IndexBuffer11(device, indices);
        }

        private void createSpriteCornersBuffer(Device device)
        {
            // Create the small buffer required when using instancing for point sprites

            var corners = new CompatibilityPointSpriteShader.CornerVertex[4];
            corners[0].corner = 0x00000000;
            corners[1].corner = 0xff00ff00;
            corners[2].corner = 0xffffffff;
            corners[3].corner = 0x00ff00ff;
            spriteCornerVertexBuffer = new GenVertexBuffer<CompatibilityPointSpriteShader.CornerVertex>(device, corners);
        }

        private void createSpriteCornersIndexBuffer(Device device)
        {
            uint[] indices = { 0, 1, 2, 0, 2, 3 };
            indexBuffer = new IndexBuffer11(device, indices);
        }

        public void Draw(RenderContext11 renderContext, int count, Texture11 texture, float opacity)
        {
            setupShader(renderContext, texture, opacity);

            count = Math.Min(this.count, count);

            switch (renderStrategy)
            {
                case RenderStrategy.GeometryShader:
                    renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
                    renderContext.SetVertexBuffer(0, vertexBuffer);
                    renderContext.devContext.Draw(count, 0);
                    renderContext.Device.ImmediateContext.GeometryShader.Set(null);
                    break;

                case RenderStrategy.Instanced:
                    renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    renderContext.SetVertexBuffer(0, spriteCornerVertexBuffer);
                    renderContext.SetVertexBuffer(1, vertexBuffer);
                    renderContext.SetIndexBuffer(indexBuffer);
                    renderContext.devContext.DrawIndexedInstanced(6, count, 0, 0, 0);
                    break;

                case RenderStrategy.Fallback:
                    renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    renderContext.SetVertexBuffer(0, fallbackVertexBuffer);
                    renderContext.SetIndexBuffer(indexBuffer);
                    renderContext.devContext.DrawIndexed(count * 6, 0, 0);
                    break;
            }
        }

        public void Draw(RenderContext11 renderContext, int count, Texture11 texture, float opacity, IndexBuffer11 indexBufferIn)
        {
            setupShader(renderContext, texture, opacity);

            
            count = Math.Min(this.count, count);

            switch (renderStrategy)
            {
                case RenderStrategy.GeometryShader:
                    renderContext.devContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
                    renderContext.SetVertexBuffer(0, vertexBuffer);
                    renderContext.SetIndexBuffer(indexBufferIn);
                    renderContext.devContext.DrawIndexed(count, 0, 0);
                    renderContext.Device.ImmediateContext.GeometryShader.Set(null);
                    break;

            }
        }

        public void Dispose()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                vertexBuffer = null;
            }

            if (fallbackVertexBuffer != null)
            {
                fallbackVertexBuffer.Dispose();
                GC.SuppressFinalize(fallbackVertexBuffer);
                fallbackVertexBuffer = null;
            }

            if (spriteCornerVertexBuffer != null)
            {
                spriteCornerVertexBuffer.Dispose();
                GC.SuppressFinalize(spriteCornerVertexBuffer);
                spriteCornerVertexBuffer = null;
            }

            if (indexBuffer != null)
            {
                indexBuffer.Dispose();
                GC.SuppressFinalize(indexBuffer);
                indexBuffer = null;
            }
        }                                                 
    }


    public class PointSpriteSet : AbstractSpriteSet
    {
        public PointSpriteSet(Device device, PositionColorSize[] points) :
            base(device, points)
        {
            createVertexBuffer(device, points);
        }

        private void createVertexBuffer(Device device, PositionColorSize[] points)
        {
            if (renderStrategy == RenderStrategy.GeometryShader)
            {
                vertexBuffer = new GenVertexBuffer<PositionColorSize>(device, points);
            }
            else if (renderStrategy == RenderStrategy.Instanced)
            {
                vertexBuffer = new GenVertexBuffer<PositionColorSize>(device, points);
            }
            else
            {
                const int verticesPerPoint = 4;
                var expandedPoints = new CompatibilityPointSpriteShader.Vertex[points.Length * verticesPerPoint];

                var index = 0;
                foreach (var p in points)
                {
                    CompatibilityPointSpriteShader.Vertex xp;
                    xp.X = p.X;
                    xp.Y = p.Y;
                    xp.Z = p.Z;
                    xp.color = p.color;
                    xp.size = p.size;
                    xp.corner = 0;

                    expandedPoints[index + 0] = xp;
                    expandedPoints[index + 0].corner = 0x00000000;
                    expandedPoints[index + 1] = xp;
                    expandedPoints[index + 1].corner = 0xff00ff00;
                    expandedPoints[index + 2] = xp;
                    expandedPoints[index + 2].corner = 0xffffffff;
                    expandedPoints[index + 3] = xp;
                    expandedPoints[index + 3].corner = 0x00ff00ff;

                    index += verticesPerPoint;
                }

                fallbackVertexBuffer = new GenVertexBuffer<CompatibilityPointSpriteShader.Vertex>(device, expandedPoints);
            }
        }

        public void updateVertexBuffer(PositionColorSize[] points)
        {
            if (points == null || points.Length == 0 || points.Length > Count)
            {
                return;
            }

            if (renderStrategy == RenderStrategy.GeometryShader || renderStrategy == RenderStrategy.Instanced)
            {
                var vb = (GenVertexBuffer<PositionColorSize>) vertexBuffer;
                vb.Update(points);
            }
            else
            {
                var vb = (GenVertexBuffer<CompatibilityPointSpriteShader.Vertex>)fallbackVertexBuffer;
                var vertexData = vb.Lock(0, 0);

                if (vertexData != null)
                {
                    const int verticesPerPoint = 4;

                    var index = 0;
                    foreach (var p in points)
                    {
                        CompatibilityPointSpriteShader.Vertex xp;
                        xp.X = p.X;
                        xp.Y = p.Y;
                        xp.Z = p.Z;
                        xp.color = p.color;
                        xp.size = p.size;
                        xp.corner = 0;

                        vertexData[index + 0] = xp;
                        vertexData[index + 0].corner = 0x00000000;
                        vertexData[index + 1] = xp;
                        vertexData[index + 1].corner = 0xff00ff00;
                        vertexData[index + 2] = xp;
                        vertexData[index + 2].corner = 0xffffffff;
                        vertexData[index + 3] = xp;
                        vertexData[index + 3].corner = 0x00ff00ff;

                        index += verticesPerPoint;
                    }
                }
                vb.Unlock();
            }
        }

        public float MinPointSize = 1;

        protected override void setupShader(RenderContext11 renderContext, Texture11 texture, float opacity)
        {
            var mvp = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mvp.Transpose();

            var aspectRatio = renderContext.ViewPort.Width / renderContext.ViewPort.Height;

            var color = new Color4(TintColor.Red, TintColor.Green, TintColor.Blue, TintColor.Alpha * opacity);

            // Set up the shader
            if (renderStrategy == RenderStrategy.GeometryShader)
            {
                PointSpriteShader11.WVPMatrix = mvp;
                PointSpriteShader11.Color = color;
                PointSpriteShader11.ViewportScale = new Vector2(1.0f, aspectRatio) * 0.001f;
                PointSpriteShader11.MinPointSize = MinPointSize;
                PointSpriteShader11.PointScaleFactors = PointScaleFactors;
                PointSpriteShader11.Use(renderContext.Device.ImmediateContext);
        
            }
            else
            {
                CompatibilityPointSpriteShader.WVPMatrix = mvp;
                CompatibilityPointSpriteShader.Color = color;
                CompatibilityPointSpriteShader.ViewportScale = new Vector2(1.0f, aspectRatio) * 0.001f;
                CompatibilityPointSpriteShader.PointScaleFactors = PointScaleFactors;
                CompatibilityPointSpriteShader.MinPointSize = MinPointSize;
                var useInstancing = renderStrategy == RenderStrategy.Instanced;
                CompatibilityPointSpriteShader.Use(renderContext.Device.ImmediateContext, useInstancing);
            }

            // Set shader resources
            renderContext.Device.ImmediateContext.PixelShader.SetShaderResource(0, texture == null ? null : texture.ResourceView);
        }
    }


    public class TimeSeriesPointSpriteSet : AbstractSpriteSet
    {
        public TimeSeriesPointSpriteSet(Device device, TimeSeriesPointVertex[] points) :
            base(device, points)
        {
            createVertexBuffer(device, points);
        }

        private void createVertexBuffer(Device device, TimeSeriesPointVertex[] points)
        {
            if (renderStrategy == RenderStrategy.GeometryShader)
            {
                vertexBuffer = new GenVertexBuffer<TimeSeriesPointVertex>(device, points);
            }
            else if (renderStrategy == RenderStrategy.Instanced)
            {
                vertexBuffer = new GenVertexBuffer<TimeSeriesPointVertex>(device, points);
            }
            else
            {
                const int verticesPerPoint = 4;
                var expandedPoints = new DownlevelTimeSeriesPointSpriteShader.Vertex[points.Length * verticesPerPoint];

                var index = 0;
                foreach (var p in points)
                {
                    DownlevelTimeSeriesPointSpriteShader.Vertex xp;
                    xp.Position = p.Position;
                    xp.color = p.color;
                    xp.PointSize = p.PointSize;
                    xp.Tu = p.Tu;
                    xp.Tv = p.Tv;
                    xp.corner = 0;

                    expandedPoints[index + 0] = xp;
                    expandedPoints[index + 0].corner = 0x00000000;
                    expandedPoints[index + 1] = xp;
                    expandedPoints[index + 1].corner = 0xff00ff00;
                    expandedPoints[index + 2] = xp;
                    expandedPoints[index + 2].corner = 0xffffffff;
                    expandedPoints[index + 3] = xp;
                    expandedPoints[index + 3].corner = 0x00ff00ff;

                    index += verticesPerPoint;
                }

                fallbackVertexBuffer = new GenVertexBuffer<DownlevelTimeSeriesPointSpriteShader.Vertex>(device, expandedPoints);
            }
        }

        protected override void setupShader(RenderContext11 renderContext, Texture11 texture, float opacity)
        {
            // Let the caller handle the setup for now
        }
    }


    public class KeplerPointSpriteSet : AbstractSpriteSet
    {
        public KeplerPointSpriteSet(Device device, KeplerVertex[] points) :
            base(device, points)
        {
            createVertexBuffer(device, points);
        }

        private void createVertexBuffer(Device device, KeplerVertex[] points)
        {
            if (renderStrategy == RenderStrategy.GeometryShader)
            {
                vertexBuffer = new GenVertexBuffer<KeplerVertex>(device, points);
            }
            else if (renderStrategy == RenderStrategy.Instanced)
            {
                vertexBuffer = new GenVertexBuffer<KeplerVertex>(device, points);
            }
            else
            {
                const int verticesPerPoint = 4;
                var expandedPoints = new KeplerVertex[points.Length * verticesPerPoint];

                var index = 0;
                foreach (var p in points)
                {
                    KeplerVertex xp;
                    xp = p;
                    xp.corner = 0;

                    expandedPoints[index + 0] = xp;
                    expandedPoints[index + 0].corner = 0x00000000;
                    expandedPoints[index + 1] = xp;
                    expandedPoints[index + 1].corner = 0xff00ff00;
                    expandedPoints[index + 2] = xp;
                    expandedPoints[index + 2].corner = 0xffffffff;
                    expandedPoints[index + 3] = xp;
                    expandedPoints[index + 3].corner = 0x00ff00ff;

                    index += verticesPerPoint;
                }

                fallbackVertexBuffer = new GenVertexBuffer<KeplerVertex>(device, expandedPoints);
            }
        }

        protected override void setupShader(RenderContext11 renderContext, Texture11 texture, float opacity)
        {
            // Let the caller handle the setup for now
        }
    }

}
