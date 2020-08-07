using System;
using System.Collections.Generic;
using System.IO;
using SysColor = System.Drawing.Color;

namespace TerraViewer
{
    public class OffScreenTarget
    {
        public OffScreenTarget(float azimuth, float elevation, float fov, float aspectRatio, float[] scissor)
        {
            Azimuth = azimuth;
            Elevation = elevation;
            FOV = fov;
            AspectRatio = aspectRatio;
            Scissor = scissor;
        }

        public float Azimuth = 180;
        public float Elevation = 90;
        public float FOV = 90;
        public float AspectRatio = 1.0f;
        public float[] Scissor = new float[4];

        public List<OffScreenTargetMesh> Meshes = new List<OffScreenTargetMesh>();
    }
    public class OffScreenTargetMesh
    {
        TargetMeshType TargetMeshType = TargetMeshType.RGB;
        public int Width = 0;
        public int Height = 0;

        public IndexBuffer11 WarpIndexBuffer = null;
        public PositionColorTexturedVertexBuffer11 WarpVertexBuffer = null;

        PositionColorTexturedVertexBuffer11 VertexBuffer;
        IndexBuffer11 IndexBuffer;
        int WarpVertexCount = 0;
        public int WarpTriangleCount;

        internal static OffScreenTargetMesh FromStreamReader(StreamReader sr, int width, int height)
        {
            OffScreenTargetMesh offscreenMesh = new OffScreenTargetMesh();
            offscreenMesh.Height = height;
            offscreenMesh.Width = width;

            int meshX = width;
            int meshY = height;
            PositionColoredTextured[,] mesh = new PositionColoredTextured[meshX, meshY];

            for (int y = 0; y < meshY; y++)
            {
                for (int x = 0; x < meshX; x++)
                {
                    string buffer = sr.ReadLine();
                    string[] parts = buffer.Split(new char[] { '\t' });
                    float xx = Convert.ToSingle(parts[0]);
                    float yy = Convert.ToSingle(parts[1]);
                    float z1 = Convert.ToSingle(parts[2]);
                    float z2 = Convert.ToSingle(parts[3]);
                    if (xx== -1 || yy == -1 )
                    {
                        xx = -1;
                        yy = -1;
                    }
                    else
                    {
                        xx = xx -.5f;
                        yy = .5f - yy;
                    }
                    mesh[x, y].Position = new SharpDX.Vector4(xx, yy, .9f, 1);
                    mesh[x, y].Tu = Convert.ToSingle(parts[4]);
                    mesh[x, y].Tv = 1.0f - Convert.ToSingle(parts[5]);
                    mesh[x, y].Color = SysColor.FromArgb(255, 255, 255, 255);
                }
            }
            int warpSubX = meshX - 1;
            int warpSubY = meshY - 1;

            offscreenMesh.CleanUpWarpBuffers();
            offscreenMesh.WarpIndexBuffer = new IndexBuffer11(typeof(short), (warpSubX * warpSubY * 6), RenderContext11.PrepDevice);
            offscreenMesh.WarpVertexBuffer = new PositionColorTexturedVertexBuffer11(((warpSubX + 1) * (warpSubY + 1)), RenderContext11.PrepDevice);

            offscreenMesh.WarpVertexCount = ((warpSubX + 1) * (warpSubY + 1));

            int index = 0;

            // Create a vertex buffer 
            PositionColoredTextured[] verts = (PositionColoredTextured[])offscreenMesh.WarpVertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)
            int x1, y1;

            double textureStepX = 1.0f / warpSubX;
            double textureStepY = 1.0f / warpSubY;
            for (y1 = 0; y1 <= warpSubY; y1++)
            {
                for (x1 = 0; x1 <= warpSubX; x1++)
                {
                    index = y1 * (warpSubX + 1) + x1;
                    verts[index].Position = mesh[x1, y1].Position;
                    verts[index].Tu = mesh[x1, y1].Tu;
                    verts[index].Tv = mesh[x1, y1].Tv;
                    verts[index].Color  = mesh[x1, y1].Color;
                }
            }
            offscreenMesh.WarpVertexBuffer.Unlock();
            offscreenMesh.WarpTriangleCount = (warpSubX) * (warpSubY) * 2;
            short[] indexArray = (short[])offscreenMesh.WarpIndexBuffer.Lock();
            index = 0;
            for (y1 = 0; y1 < warpSubY; y1++)
            {
                for (x1 = 0; x1 < warpSubX; x1++)
                {
                    int i0 = y1 * meshX + x1;
                    int i1 = y1 * meshX + x1 + 1;
                    int i2 = (y1 + 1) * meshX + (x1 + 1);
                    int i3 = (y1 + 1) * meshX + x1; 

                    // First triangle in quad
                    indexArray[index] = (short)i3;
                    indexArray[index + 1] = (short)i1;
                    indexArray[index + 2] = (short)i0;

                    // Second triangle in quad
                    indexArray[index + 3] = (short)i2;
                    indexArray[index + 4] = (short)i1;
                    indexArray[index + 5] = (short)i3;

                    if (!((verts[indexArray[index]].Position.X == -1) ||
                        (verts[indexArray[index + 1]].Position.X == -1) ||
                        (verts[indexArray[index + 2]].Position.X == -1) ||
                        (verts[indexArray[index + 3]].Position.X == -1) ||
                        (verts[indexArray[index + 4]].Position.X == -1) ||
                        (verts[indexArray[index + 5]].Position.X == -1)))
                    {
                        index += 6;
                    }
                }
            }
            offscreenMesh.WarpTriangleCount = index / 3;
            offscreenMesh.WarpIndexBuffer.Unlock();

            return offscreenMesh;
        }

        private void CleanUpWarpBuffers()
        {
            if (WarpIndexBuffer != null)
            {
                WarpIndexBuffer.Dispose();
                GC.SuppressFinalize(WarpIndexBuffer);
            }

            if (WarpVertexBuffer != null)
            {
                WarpVertexBuffer.Dispose();
                GC.SuppressFinalize(WarpVertexBuffer);
            }
        }
    }

    public enum TargetMeshType { Red = 1, Green = 2, Blue = 4, RGB = 7 };

    public class RenderHead
    {
        public List<OffScreenTarget> Targets = new List<OffScreenTarget>();

        public int OutputScreenID = -1; // -1 = window output

        public int Width = 1920;
        public int Height = 1080;

        public int TargetWidth = 1024;
        public int TargetHeight = 1024;

        public RenderHead(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            float Azimuth = 0;
            float Elevation = 0;
            float Aspect = 1;
            float FOV = 0;
            float[] Scissor = new float[4];
            int MeshHeight = 0;
            int MeshWidth = 0;

            do
            {
                string line = sr.ReadLine();
                string[] parts = line.Split('=');


                switch (parts[0])
                {
                    case "Azimuth":
                        Azimuth = float.Parse(parts[1]);
                        break;
                    case "Elevation":
                        Elevation = float.Parse(parts[1]);
                        break;
                    case "FOV":
                        FOV = float.Parse(parts[1]);
                        break;
                    case "Aspect Ratio":
                        Aspect = float.Parse(parts[1]);
                        break;
                    case "Scissor":
                        {
                            string[] items = parts[1].Split(' ');
                            for (int i = 0; i < 4; i++)
                            {
                                Scissor[i] = float.Parse(items[i]);
                            }
                            break;
                        }
                    case "Mesh":
                        {
                            string[] items = parts[1].Split(' ');
                            MeshHeight = int.Parse(items[2]);
                            MeshWidth = int.Parse(items[1]);
                            OffScreenTarget target = new OffScreenTarget(Azimuth, Elevation, FOV, Aspect, Scissor);
                            Targets.Add(target);
                            target.Meshes.Add(OffScreenTargetMesh.FromStreamReader(sr, MeshWidth, MeshHeight));
                        }
                        break;
                }
                if (sr.EndOfStream)
                {
                    break;
                }
            } while (true);
        }
    }
}
