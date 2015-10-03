namespace TerraViewer
{
    using System;
    using System.Drawing;

    public class SphereTest
    {
        SharpDX.Direct3D11.Buffer vertices;
        SharpDX.Direct3D11.Buffer indexBuffer;
        SharpDX.Direct3D11.VertexBufferBinding vertexBufferBinding;

        Texture11 texture;

        private SharpDX.Direct3D11.InputLayout layout;

        public void Draw(RenderContext11 renderContext)
        {
            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            renderContext.devContext.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            renderContext.devContext.InputAssembler.SetIndexBuffer(indexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);



            renderContext.SunPosition = new Vector3d(500, 500, 0.0);
            renderContext.SunlightColor = Color.White;
            renderContext.AmbientLightColor = Color.DarkGray;

            renderContext.SetupBasicEffect(BasicEffect.TextureOnly, 1.0f, Color.White);
            renderContext.MainTexture = texture;

            renderContext.PreDraw();


            if (layout == null)
            {
                layout = new SharpDX.Direct3D11.InputLayout(renderContext.Device, renderContext.Shader.InputSignature, new[]
                           {
                               new SharpDX.Direct3D11.InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float,     0, 0),
                               new SharpDX.Direct3D11.InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float,       16, 0)
                           });
                renderContext.Device.ImmediateContext.InputAssembler.InputLayout = layout;
            }


            // Draw the cube
            renderContext.devContext.DrawIndexed(triangleCount * 3, 0, 0);
        }

        const double RC = 3.1415927 / 180;
        const int subDivisionsX = 1000;
        const int subDivisionsY = 500;

        int triangleCount = subDivisionsX * subDivisionsY * 2;

        public void MakeSphere(SharpDX.Direct3D11.Device d3dDevice)
        {
            // Layout from VertexShader input signature


            texture = Texture11.FromFile(d3dDevice, "earthMap.jpg");

            var indexes = new uint[subDivisionsX * subDivisionsY * 6];
            var verts = new float[((subDivisionsX + 1) * (subDivisionsY + 1)) * 6];


            uint index;
            const double latMin = 90;
            const double latMax = -90;
            const double lngMin = -180;
            const double lngMax = 180;


            uint x1, y1;

            const double latDegrees = latMax - latMin;
            const double lngDegrees = lngMax - lngMin;

            const double textureStepX = 1.0f / subDivisionsX;
            const double textureStepY = 1.0f / subDivisionsY;
            for (y1 = 0; y1 <= subDivisionsY; y1++)
            {
                double lat;
                if (y1 != subDivisionsY)
                {
                    lat = latMax - (textureStepY * latDegrees * y1);
                }
                else
                {
                    lat = latMin;
                }

                for (x1 = 0; x1 <= subDivisionsX; x1++)
                {
                    double lng;
                    if (x1 != subDivisionsX)
                    {
                        lng = lngMin + (textureStepX * lngDegrees * x1);
                    }
                    else
                    {
                        lng = lngMax;
                    }
                    index = (y1 * (subDivisionsX + 1) + x1) * 6;

                    GeoTo3d(verts, (int)index, lat, lng);
                }
            }

            triangleCount = (subDivisionsX) * (subDivisionsY) * 2;

            for (y1 = 0; y1 < subDivisionsY; y1++)
            {
                for (x1 = 0; x1 < subDivisionsX; x1++)
                {
                    index = (y1 * subDivisionsX * 6) + 6 * x1;
                    // First triangle in quad
                    indexes[index] = (y1 * (subDivisionsX + 1) + x1);
                    indexes[index + 1] = ((y1 + 1) * (subDivisionsX + 1) + x1);
                    indexes[index + 2] = (y1 * (subDivisionsX + 1) + (x1 + 1));

                    // Second triangle in quad
                    indexes[index + 3] = (y1 * (subDivisionsX + 1) + (x1 + 1));
                    indexes[index + 4] = ((y1 + 1) * (subDivisionsX + 1) + x1);
                    indexes[index + 5] = ((y1 + 1) * (subDivisionsX + 1) + (x1 + 1));
                }
            }

            vertices = SharpDX.Direct3D11.Buffer.Create(d3dDevice, SharpDX.Direct3D11.BindFlags.VertexBuffer, verts);

            vertexBufferBinding = new SharpDX.Direct3D11.VertexBufferBinding(vertices, sizeof(float) * 6, 0);

            indexBuffer = SharpDX.Direct3D11.Buffer.Create(d3dDevice, SharpDX.Direct3D11.BindFlags.IndexBuffer, indexes);


        }

        static public void GeoTo3d(float[] data, int baseIndex, double lat, double lng)
        {
            data[baseIndex] = (float)((Math.Cos(lng * RC)) * (Math.Cos(lat * RC)));
            data[baseIndex + 1] = (float)((Math.Sin(lat * RC)));
            data[baseIndex + 2] = (float)((Math.Sin(lng * RC)) * (Math.Cos(lat * RC)));
            data[baseIndex + 3] = 1f;
            data[baseIndex + 4] = (float)((lng + 180) / 360);
            data[baseIndex + 5] = 1 - (float)((lat + 90) / 180);
        }
    }
}
