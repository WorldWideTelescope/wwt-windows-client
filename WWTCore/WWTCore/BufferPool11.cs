using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Windows;
using System.Diagnostics;

using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using SharpDX.D3DCompiler;
using System.IO;
//using System.Drawing.Imaging;
using System.Threading;


namespace TerraViewer
{
    public class BufferPool11
    {
        static Dictionary<int, Buffers> IndexPools = new Dictionary<int, Buffers>();
        static Dictionary<int, Buffers> VertexX2Pools = new Dictionary<int, Buffers>();
        static Dictionary<int, Buffers> LockX2Pools = new Dictionary<int, Buffers>();
        static Dictionary<int, Buffers> Vector3dPools = new Dictionary<int, Buffers>();
        static Dictionary<int, Buffers> IndexU32Pools = new Dictionary<int, Buffers>();
        static Dictionary<int, Buffers> IndexU16Pools = new Dictionary<int, Buffers>();
        static Buffers TriangleListPool = new Buffers(1);
        static Buffers PositionTextureListPool = new Buffers(1);
        
        static Mutex BufferMutex = new Mutex();

        public static void DisposeBuffers()
        {
            foreach (Buffers pool in IndexPools.Values)
            {
                foreach (object buf in pool.Entries)
                {
                    IndexBuffer11 indexBuf = buf as IndexBuffer11;
                    indexBuf.Dispose();
                    GC.SuppressFinalize(indexBuf);
                }
            }
            IndexPools.Clear();

            foreach (Buffers pool in VertexX2Pools.Values)
            {
                foreach (object buf in pool.Entries)
                {
                    VertexBuffer11 vertexBuf = buf as VertexBuffer11;
                    vertexBuf.Dispose();
                    GC.SuppressFinalize(vertexBuf);
                }
            }
            Vector3dPools.Clear();

            // no need to dispose since these are just managed buffers
            LockX2Pools.Clear();
            Vector3dPools.Clear();
            IndexU32Pools.Clear();
            IndexU32Pools.Clear();
            TriangleListPool.Entries.Clear();

            foreach (Texture11 texture in TexturePool256)
            {
                texture.Dispose();
                GC.SuppressFinalize(texture);
            }
            TexturePool256.Clear();
        }

        public static List<Triangle> GetTriagleList()
        {
            try
            { 
                BufferMutex.WaitOne();
                if (TriangleListPool.Entries.Count > 0)
                {
                    return (List<Triangle>)TriangleListPool.Entries.Pop();
                }

                return new List<Triangle>(128);
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnTriangleList( List<Triangle> triList)
        {
            if (triList == null)
            {
                return;
            }

            triList.Clear();
            BufferMutex.WaitOne();
            TriangleListPool.Entries.Push(triList);
            BufferMutex.ReleaseMutex();

        }

        public static List<PositionTexture> GetPositionTextureList()
        {
            try
            { 
                BufferMutex.WaitOne();
                if (PositionTextureListPool.Entries.Count > 0)
                {
                    return (List<PositionTexture>)PositionTextureListPool.Entries.Pop();
                }

                return new List<PositionTexture>(525);
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnPositionTextureList(List<PositionTexture> posTextList)
        {
            if (posTextList == null)
            {
                return;
            }
            posTextList.Clear();
            BufferMutex.WaitOne();
            PositionTextureListPool.Entries.Push(posTextList);
            BufferMutex.ReleaseMutex();

        }

        public static IndexBuffer11 GetShortIndexBuffer(int count)
        {
            //todo11 this needs to be made thread safe..
            //if (!IndexPools.ContainsKey(count))
            //{
            //    IndexPools.Add(count, new Buffers(count));
            //}

            //if (IndexPools[count].Entries.Count > 0)
            //{
            //    return (IndexBuffer11)IndexPools[count].Entries.Pop();
            //}

            return new IndexBuffer11(typeof(short), count, RenderContext11.PrepDevice);
        }

        public static void ReturnShortIndexBuffer(IndexBuffer11 buff)
        {
            if (buff == null)
            {
                return;
            }
            //old code to dispose buffers
            buff.Dispose();
            GC.SuppressFinalize(buff);

            //todo11 reenable buffer pooling
            //int count = buff.Count;
            //if (!IndexPools.ContainsKey(count))
            //{
            //    IndexPools.Add(count, new Buffers(count));
            //}

            //IndexPools[count].Entries.Push(buff);
        }

        public static VertexBuffer11 GetPNTX2VertexBuffer(int count)
        {
            //BufferMutex.WaitOne();
            //todo11 this needs to be made thread safe..
            //if (!VertexX2Pools.ContainsKey(count))
            //{
            //    VertexX2Pools.Add(count, new Buffers(count));
            //}

            //if (VertexX2Pools[count].Entries.Count > 0)
            //{
            //    return (VertexBuffer11)VertexX2Pools[count].Entries.Pop();
            //}
            //BufferMutex.ReleaseMutex();

            return new VertexBuffer11(typeof(PositionNormalTexturedX2), count, RenderContext11.PrepDevice);
        }


       


        public static PositionNormalTexturedX2 sizeTest;
        public static void ReturnPNTX2VertexBuffer(VertexBuffer11 buff)
        {
            if (buff == null)
            {
                return;
            }
            buff.Dispose();
            GC.SuppressFinalize(buff);

           // BufferMutex.WaitOne();
            //todo11 reimplement buffer pooling
            //int count = buff.Count;
            //if (!VertexX2Pools.ContainsKey(count))
            //{
            //    VertexX2Pools.Add(count, new Buffers(count));
            //}

            //VertexX2Pools[count].Entries.Push(buff);
            //BufferMutex.ReleaseMutex();
        }

        public static PositionNormalTexturedX2[] GetLockX2Buffer(int count)
        {

            BufferMutex.WaitOne();

            try
            {
                if (count < 2048)
                {
                    if (!LockX2Pools.ContainsKey(count))
                    {
                        LockX2Pools.Add(count, new Buffers(count));
                    }

                    if (LockX2Pools[count].Entries.Count > 0)
                    {
                        return (PositionNormalTexturedX2[])LockX2Pools[count].Entries.Pop();
                    }
                }
                return new PositionNormalTexturedX2[count];
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnLockX2Buffer(PositionNormalTexturedX2[] buff)
        {
            if (buff == null)
            {
                return;
            }
            BufferMutex.WaitOne();
            try
            {
                int count = buff.Length;
                if (count < 2048)
                {
                    if (!LockX2Pools.ContainsKey(count))
                    {
                        LockX2Pools.Add(count, new Buffers(count));
                    }

                    LockX2Pools[count].Entries.Push(buff);
                }
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }
        

        public static Vector3d[] GetVector3dBuffer(int count)
        {

            BufferMutex.WaitOne();

            try
            {
                if (count < 2048)
                {
                    if (!Vector3dPools.ContainsKey(count))
                    {
                        Vector3dPools.Add(count, new Buffers(count));
                    }

                    if (Vector3dPools[count].Entries.Count > 0)
                    {
                        return (Vector3d[])Vector3dPools[count].Entries.Pop();
                    }
                }
                return new Vector3d[count];
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnVector3dBuffer(Vector3d[] buff)
        {
            if (buff == null)
            {
                return;
            }
            BufferMutex.WaitOne();
            int count = buff.Length;
            if (count < 2048)
            {
                if (!Vector3dPools.ContainsKey(count))
                {
                    Vector3dPools.Add(count, new Buffers(count));
                }

                Vector3dPools[count].Entries.Push(buff);
            }
            BufferMutex.ReleaseMutex();
        }



        public static UInt32[] GetUInt32Buffer(int count)
        {

            BufferMutex.WaitOne();

            try
            {
                if (count < 2048)
                {
                    if (!IndexU32Pools.ContainsKey(count))
                    {
                        IndexU32Pools.Add(count, new Buffers(count));
                    }

                    if (IndexU32Pools[count].Entries.Count > 0)
                    {
                        return (UInt32[])IndexU32Pools[count].Entries.Pop();
                    }
                }
                return new UInt32[count];
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnUInt32Buffer(UInt32[] buff)
        {
            if (buff == null)
            {
                return;
            }
            BufferMutex.WaitOne();
            int count = buff.Length;
            if (count < 2048)
            {
                if (!IndexU32Pools.ContainsKey(count))
                {
                    IndexU32Pools.Add(count, new Buffers(count));
                }

                IndexU32Pools[count].Entries.Push(buff);
            }
            BufferMutex.ReleaseMutex();
        }



        public static UInt16[] GetUInt16Buffer(int count)
        {

            BufferMutex.WaitOne();

            try
            {
                if (count < 2048)
                {
                    if (!IndexU16Pools.ContainsKey(count))
                    {
                        IndexU16Pools.Add(count, new Buffers(count));
                    }

                    if (IndexU16Pools[count].Entries.Count > 0)
                    {
                        return (UInt16[])IndexU16Pools[count].Entries.Pop();
                    }
                }
                return new UInt16[count];
            }
            finally
            {
                BufferMutex.ReleaseMutex();
            }
        }

        public static void ReturnUInt16Buffer(UInt16[] buff)
        {
            if (buff == null)
            {
                return;
            }
            BufferMutex.WaitOne();
            int count = buff.Length;
            if (count < 2048)
            {
                if (!IndexU16Pools.ContainsKey(count))
                {
                    IndexU16Pools.Add(count, new Buffers(count));
                }

                IndexU16Pools[count].Entries.Push(buff);
            }
            BufferMutex.ReleaseMutex();
        }

        static Stack<Texture11> TexturePool256 = new Stack<Texture11>();

        public static void ReturnTexture(Texture11 texture)
        {
            if (texture == null)
            {
                return;
            }
            texture.Dispose();
            GC.SuppressFinalize(texture);
            
            //todo11 reimplement pooling of textures
            //SurfaceDescription sd = texture.GetLevelDescription(0);

            //if (sd.Height != 256 || sd.Width != 256)
            //{
            //    texture.Dispose();
            //    GC.SuppressFinalize(texture);
            //}
            //else
            //{
            //BufferMutex.WaitOne();
            //    TexturePool256.Push(texture);
            //BufferMutex.ReleaseMutex();
            //}
        }

        public static Texture11 GetTexture(string filename)
        {
            return Texture11.FromFile(RenderContext11.PrepDevice, filename);
        }

        public static Texture11 GetTexture(Stream stream)
        {
            return Texture11.FromStream(RenderContext11.PrepDevice, stream);
        }

        public static Texture11 GetTexture(Bitmap bitmap)
        {
            return Texture11.FromBitmap(RenderContext11.PrepDevice, bitmap);

            //todo11 reenable the pooling of textures
            //if (bitmap.Width != 256 || bitmap.Height != 256 | !(bitmap.PixelFormat == PixelFormat.Format32bppArgb | bitmap.PixelFormat == PixelFormat.Format24bppRgb))
            //{
            //    return UiTools.LoadTextureFromBmp(Tile.prepDevice, bitmap);
            //}

            //Texture11 texture = null;
            //if (TexturePool256.Count > 0)
            //{
            //    texture = TexturePool256.Pop();
            //}
            //else
            //{
            //    texture = new Texture11(Tile.prepDevice, 256, 256, 0, Usage.AutoGenerateMipMap, Format.A8R8G8B8, Pool.Managed);

            //}

            //if (bitmap.PixelFormat == PixelFormat.Format32bppArgb | bitmap.PixelFormat == PixelFormat.Format24bppRgb)
            //{
            //    BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            //    int pitch;
            //    GraphicsStream textureData = texture.LockRectangle(0, LockFlags.Discard, out pitch);
            //    unsafe
            //    {
            //        uint* texturePointer = (uint*)textureData.InternalDataPointer;
            //        for (int y = 0; y < bitmap.Height; y++)
            //        {
            //            uint* bitmapLinePointer = (uint*)bitmapData.Scan0 + y * (bitmapData.Stride / sizeof(int));
            //            uint* textureLinePointer = texturePointer + y * (pitch / sizeof(int));
            //            int length = bitmap.Width;

            //            while (--length >= 0)
            //            {
            //                *textureLinePointer++ = *bitmapLinePointer++;
            //            }

            //        }
            //    }

            //    bitmap.UnlockBits(bitmapData);
            //    texture.UnlockRectangle(0);
            //    //   texture.GenerateMipSubLevels();
            //}

            //return texture;
        }



    }

    public class Buffers
    {
        public Buffers(int count)
        {
            Count = count;
        }
        public int Count;
        public Stack<object> Entries = new Stack<object>();

    }
}
