using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Runtime.InteropServices;

namespace TerraViewer
{
    class DX11Tools
    {
    }

    public class VertexBuffer11 : IDisposable, IVertexBuffer11
    {
    
        int vertCount = 0;

        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public VertexBuffer11(Type typeVertexType, int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
            
        }

        PositionNormalTexturedX2[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = BufferPool11.GetLockX2Buffer(vertCount);

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as PositionNormalTexturedX2[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((PositionNormalTexturedX2[])vertexArray)[0]), 0);
            }
            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                //Vector3d[] points = new Vector3d[vertCount];
                Vector3d[] points = BufferPool11.GetVector3dBuffer(vertCount);


                foreach (PositionNormalTexturedX2 vert in vertexArray)
                {
                    points[index++] = vert.Position;
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
                BufferPool11.ReturnVector3dBuffer(points);
            }

            BufferPool11.ReturnLockX2Buffer(vertexArray);

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }

    public class GenVertexBuffer<T> : IVertexBuffer11 where T : struct
    {
        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }

        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public GenVertexBuffer(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
        }

        public GenVertexBuffer(Device device, T[] vertexArray)
        {
            this.device = device;
            vertCount = vertexArray.Length;
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as T[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((T[])vertexArray)[0]), 0);
            }
        }

        T[] vertexArray = null;
        public T[] Lock(int x, int y)
        {
            vertexArray = new T[vertCount];

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as T[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((T[])vertexArray)[0]), 0);
            }

            // Compute the sphere is asked
            // todo11 Make all vertex types implement an interface for getting the position so that
            //        we can make the bounding sphere calculation work generically
            /*
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (T vert in vertexArray)
                {
                    points[index++] = vert.Position;
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }
            */

            vertexArray = null;
        }

        public void Update(T[] newVertexData)
        {
            if (vertices != null)
            {
                device.ImmediateContext.UpdateSubresource(newVertexData, vertices);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }


    public class PositionTexturedVertexBuffer11 : IDisposable, IVertexBuffer11
    {

        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public PositionTexturedVertexBuffer11( int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;

        }

        PositionTextured[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new PositionTextured[vertCount];

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as PositionTextured[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((PositionTextured[])vertexArray)[0]), 0);
            }
            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (PositionTextured vert in vertexArray)
                {
                    points[index++] = vert.Position;
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }


    public class PositionColorSizeVertexBuffer11 : IDisposable, IVertexBuffer11
    {
        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public PositionColorSizeVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
        }

        PositionColorSize[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new PositionColorSize[vertCount];
            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as PositionColorSize[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((PositionColorSize[])vertexArray)[0]), 0);
            }

            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (PositionColorSize vert in vertexArray)
                {
                    points[index++] = new Vector3d(vert.X, vert.Y, vert.Z);
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }
    public class TimeSeriesLineVertexBuffer11 : IDisposable, IVertexBuffer11
    {
        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public TimeSeriesLineVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
        }

        TimeSeriesLineVertex[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new TimeSeriesLineVertex[vertCount];
            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as TimeSeriesLineVertex[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((TimeSeriesLineVertex[])vertexArray)[0]), 0);
            }

            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (TimeSeriesLineVertex vert in vertexArray)
                {
                    points[index++] = new Vector3d(vert.Position);
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }

    public class TimeSeriesPointVertexBuffer11 : IDisposable, IVertexBuffer11
    {
        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public TimeSeriesPointVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
        }

        TimeSeriesPointVertex[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new TimeSeriesPointVertex[vertCount];
            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as TimeSeriesPointVertex[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((TimeSeriesPointVertex[])vertexArray)[0]), 0);
            }

            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (TimeSeriesPointVertex vert in vertexArray)
                {
                    points[index++] = new Vector3d(vert.Position);
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }
  
     public class KeplerVertexBuffer11 : IDisposable, IVertexBuffer11
    {
        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public KeplerVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;
        }

        KeplerVertex[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new KeplerVertex[vertCount];
            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as KeplerVertex[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((KeplerVertex[])vertexArray)[0]), 0);
            }

            

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }
  
    
    public class TansformedPositionTexturedVertexBuffer11 : IDisposable, IVertexBuffer11
    {


        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public TansformedPositionTexturedVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;

        }

        TansformedPositionTextured[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new TansformedPositionTextured[vertCount];

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as TansformedPositionTextured[],0,ResourceUsage.Dynamic,CpuAccessFlags.Write,ResourceOptionFlags.None);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((TansformedPositionTextured[])vertexArray)[0]), 0);
            }
            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }
    public class PositionColorTexturedVertexBuffer11 : IDisposable, IVertexBuffer11
    {

        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }

        int vertCount = 0;
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

   
        public PositionColorTexturedVertexBuffer11(int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;


        }

        PositionColoredTextured[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new PositionColoredTextured[vertCount];

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray as PositionColoredTextured[]);
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(((PositionColoredTextured[])vertexArray)[0]), 0);
            }
            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }


    public interface IVertexBuffer11 : IDisposable
    {
        VertexBufferBinding VertexBufferBinding
        {
            get;
        }

    }

    public class PositionVertexBuffer11 : IDisposable, IVertexBuffer11
    {
        int vertCount = 0;

        public int Count
        {
            get { return vertCount; }
            set { vertCount = value; }
        }
        Device device = null;

        Buffer vertices = null;

        private VertexBufferBinding vertexBufferBinding;

        public VertexBufferBinding VertexBufferBinding
        {
            get { return vertexBufferBinding; }
            set { vertexBufferBinding = value; }
        }
        public Vector3d SphereCenter;
        public double SphereRadius;
        public bool ComputeSphereOnUnlock = false;

        public PositionVertexBuffer11( int numVerts, Device device)
        {
            vertCount = numVerts;
            this.device = device;

        }

        Vector3[] vertexArray = null;
        public object Lock(int x, int y)
        {
            vertexArray = new Vector3[vertCount];

            return vertexArray;
        }

        public void Unlock()
        {
            if (vertCount > 0 && vertexArray != null)
            {
                vertices = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertexArray );
                VertexBufferBinding = new VertexBufferBinding(vertices, System.Runtime.InteropServices.Marshal.SizeOf(vertexArray[0]), 0);
            }
            // Compute the sphere is asked
            if (ComputeSphereOnUnlock)
            {
                int index = 0;
                Vector3d[] points = new Vector3d[vertCount];
                foreach (Vector3 vert in vertexArray)
                {
                    points[index++] = new Vector3d(vert);
                }
                ConvexHull.FindEnclosingSphere(points, out SphereCenter, out SphereRadius);
            }

            vertexArray = null;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (vertices != null)
            {
                vertices.Dispose();
                GC.SuppressFinalize(vertices);
                vertices = null;
                vertexBufferBinding.Buffer = null;
            }
        }

        #endregion
    }

    public class IndexBuffer11 : IDisposable
    {
        public Buffer IndexBuffer;

        public IndexBuffer11 (Device device, uint[] indexes)
        {
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indexes);
            indexCount = indexes.Length;
            format = Format.R32_UInt;
            this.device = device;
        }

        public IndexBuffer11 (Device device, short[] indexes)
        {
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indexes);
            indexCount = indexes.Length;
            format = Format.R16_UInt;
            this.device = device;
        }

        public int Count
        {
            get { return indexCount; }

        }

        public Format format = Format.R32_UInt;
        int indexCount = 0;
        Device device = null;
        public IndexBuffer11(Type typeIndexType, int numberIndices, Device device)
        {
            if (typeIndexType == typeof(short))
            {
                format = Format.R16_UInt;
            }
            else
            {
                format = Format.R32_UInt;
            }

            indexCount = numberIndices;
            this.device = device;
        }

        object indexArray;
        
        public object Lock()
        {
            if (format == Format.R32_UInt)
            {
                indexArray = BufferPool11.GetUInt32Buffer(indexCount);
            }
            else
            {
                indexArray = BufferPool11.GetUInt16Buffer(indexCount);
            }
            return indexArray;
        }

        public void Unlock()
        {
            if (format == Format.R32_UInt)
            {
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indexArray as uint[]);
                BufferPool11.ReturnUInt32Buffer(indexArray as UInt32[]);
            }
            else
            {
                IndexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.IndexBuffer, indexArray as ushort[]);
                BufferPool11.ReturnUInt16Buffer(indexArray as UInt16[]);
            } 
            indexArray = null;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (IndexBuffer != null)
            {
                IndexBuffer.Dispose();
                GC.SuppressFinalize(IndexBuffer);
                IndexBuffer = null;
            }
        }

        #endregion
    }

    public struct PositionTextured
    {
        // Summary:
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv;


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionTextured(Vector3d pos, Vector3d nor, float u, float v)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;
            Tu = u;
            Tv = v;

        }




        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionTextured(float xvalue, float yvalue, float zvalue, float u, float v)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;

            Tu = u;
            Tv = v;

        }

        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3d Position
        {
            get
            {
                return new Vector3d(X, Y, Z);
            }
            set
            {
                X = (float)value.X;
                Y = (float)value.Y;
                Z = (float)value.Z;
            }
        }


        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, U={3}, V={4}",
                X, Y, Z, Tu, Tv
                );
        }
    }


    public struct PositionColorSize
    {
        // Summary:
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;

        public uint color;
        public float size;

        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        public PositionColorSize(Vector3d pos, SharpDX.Color color, float size)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;  
            this.color = (uint)(color.ToBgra());
            this.size = size;
        }

        public PositionColorSize(Vector3d pos, System.Drawing.Color color, float size)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;
            this.color = 0;
            this.size = size;
            Color = color;  
        }

        public void Save(System.IO.BinaryWriter bw)
        {
            bw.Write(X);
            bw.Write(Y);
            bw.Write(Z);
            bw.Write(color);
            bw.Write(size);
        }

        public static PositionColorSize Load(System.IO.BinaryReader br)
        {
            PositionColorSize point = new PositionColorSize();

            point.X =  br.ReadSingle();
            point.Y =  br.ReadSingle();
            point.Z =  br.ReadSingle();
            point.color = br.ReadUInt32();
            point.size = br.ReadSingle();
            return point;
        }

        public Vector3 Position
        {
            get
            {
                return new Vector3(X, Y, Z);
            }

            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
            }
            set
            {
                color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
            }
        }


    }


    public struct PositionColoredTextured
    {
        // Summary:
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float W;

        private uint color;

        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb((byte)(color>>24), (byte)color, (byte)(color >>8), (byte)(color >> 16));
            }
            set
            {
                color = (uint) (((uint)value.A) << 24) | (((uint)value.B) << 16) |  (((uint)value.G) <<8) |  (((uint)value.R) );
            }
        }   
        
       //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv;



        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionColoredTextured(Vector4 pos, System.Drawing.Color color, float u, float v)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            W = pos.W;
            Tu = u;
            Tv = v;
            this.color = 0;
            Color = color;
        }

        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionColoredTextured(float xvalue, float yvalue, float zvalue, System.Drawing.Color color, float wvalue, float u, float v)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
            W = wvalue;
            Tu = u;
            Tv = v;
            this.color = 0;
            Color = color;

        }

        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector4 Position
        {
            get
            {
                return new Vector4(X, Y, Z, W);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                W = value.W;
            }
        }

        public Vector3 Pos3
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                W = 1;
            }
        }

        public Vector3d Pos3d
        {
            get
            {
                return new Vector3d(X, Y, Z);
            }
            set
            {
                X = (float)value.X;
                Y = (float)value.Y;
                Z = (float)value.Z;
                W = 1;
            }
        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, W={3}, U={4}, V={5}",
                X, Y, Z, W, Tu, Tv
                );
        }
    }

    public struct TansformedPositionTextured
    {
        // Summary:
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;
             //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float W;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv;


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public TansformedPositionTextured(Vector4 pos, float u, float v)
        {
            X = pos.X;
            Y = pos.Y;
            Z = pos.Z;
            W = pos.W;
            Tu = u;
            Tv = v;

        }




        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public TansformedPositionTextured(float xvalue, float yvalue, float zvalue, float wvalue,float u, float v)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
            W = wvalue;
            Tu = u;
            Tv = v;

        }

        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector4 Position
        {
            get
            {
                return new Vector4(X, Y, Z,W);
            }
            set
            {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
                W = value.W;
            }
        }


        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, W={3}, U={4}, V={5}",
                X, Y, Z, W, Tu, Tv
                );
        }
    }

    public class RenderTargetTexture
    {
        public RenderTargetView renderView;
        public Texture11 RenderTexture;
 
        public int Width;
        public int Height;
        public RenderTargetTexture(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            RenderTexture = new Texture11(new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
            {
                Format = RenderContext11.DefaultColorFormat,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(RenderContext11.MultiSampleCount, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }));

            renderView = new RenderTargetView(RenderContext11.PrepDevice, RenderTexture.Texture);
        }

        public RenderTargetTexture(int width, int height, Format format)
        {
            this.Width = width;
            this.Height = height;
            RenderTexture = new Texture11(new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
            {
                Format = format,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(RenderContext11.MultiSampleCount, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }));

            renderView = new RenderTargetView(RenderContext11.PrepDevice, RenderTexture.Texture);
        }

        public RenderTargetTexture(int width, int height, int sampleCount)
        {
            this.Width = width;
            this.Height = height;
            RenderTexture = new Texture11(new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
            {
                Format = RenderContext11.DefaultColorFormat,
                ArraySize = 1,
                MipLevels = 0,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(sampleCount, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.GenerateMipMaps
            }));

            renderView = new RenderTargetView(RenderContext11.PrepDevice, RenderTexture.Texture);
        }


        public void Dispose()
        {
            if (renderView != null)
            {
                renderView.Dispose();
                GC.SuppressFinalize(renderView);
                renderView = null;
            }

            if (RenderTexture != null)
            {
                RenderTexture.Dispose();

                GC.SuppressFinalize(RenderTexture);
                RenderTexture = null; 
            }
        }
    }

    public class DepthBuffer : IDisposable
    {
        public DepthStencilView DepthView;
        public Texture2D DepthTexture;
        public int Width;
        public int Height;
        public DepthBuffer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            DepthTexture = new Texture2D(RenderContext11.PrepDevice, new Texture2DDescription()
            {
                Format = RenderContext11.DefaultDepthStencilFormat,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(RenderContext11.MultiSampleCount, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            DepthView = new DepthStencilView(RenderContext11.PrepDevice, DepthTexture);
        }

        public void Dispose()
        {
            if (DepthView != null)
            {
                DepthView.Dispose();
                GC.SuppressFinalize(DepthView);
                DepthView = null;
            }

            if (DepthTexture != null)
            {
                DepthTexture.Dispose();      
                GC.SuppressFinalize(DepthTexture);
                DepthTexture = null;
            }
        }
    }

    public struct PositionNormalTextured
    {
        // Summary:
        //     Retrieves the Microsoft.DirectX.Direct3D.VertexFormats for the current custom
        //     vertex.
        public float X;
        //
        // Summary:
        //     Retrieves or sets the y component of the position.
        public float Y;
        //
        // Summary:
        //     Retrieves or sets the z component of the position.
        public float Z;
        // Summary:
        //     Retrieves or sets the nx component of the vertex normal.
        public float Nx;
        //
        // Summary:
        //     Retrieves or sets the ny component of the vertex normal.
        public float Ny;
        //
        // Summary:
        //     Retrieves or sets the nz component of the vertex normal.
        public float Nz;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
        public float Tu;
        //
        // Summary:
        //     Retrieves or sets the v component of the texture coordinate.
        public float Tv;
        //
        // Summary:
        //     Retrieves or sets the u component of the texture coordinate.
    
        //
        // Summary:
        //     Retrieves or sets the x component of the position.


        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   pos:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex position.
        //
        //   nor:
        //     A Microsoft.DirectX.Vector3 object that contains the vertex normal data.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionNormalTextured(Vector3d pos, Vector3d nor, float u, float v)
        {
            X = (float)pos.X;
            Y = (float)pos.Y;
            Z = (float)pos.Z;
            Nx = (float)nor.X;
            Ny = (float)nor.Y;
            Nz = (float)nor.Z;
            Tu = u;
            Tv = v;

        }

        public PositionNormalTextured(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            X = position.X;
            Y = position.Y;
            Z = position.Z;
            Nx = normal.X;
            Ny = normal.Y;
            Nz = normal.Z;
            Tu = texCoord.X;
            Tv = texCoord.Y;
        }

        //
        // Summary:
        //     Initializes a new instance of the PositionNormalTexturedX2
        //     class.
        //
        // Parameters:
        //   xvalue:
        //     Floating-point value that represents the x coordinate of the position.
        //
        //   yvalue:
        //     Floating-point value that represents the y coordinate of the position.
        //
        //   zvalue:
        //     Floating-point value that represents the z coordinate of the position.
        //
        //   nxvalue:
        //     Floating-point value that represents the nx coordinate of the vertex normal.
        //
        //   nyvalue:
        //     Floating-point value that represents the ny coordinate of the vertex normal.
        //
        //   nzvalue:
        //     Floating-point value that represents the nz coordinate of the vertex normal.
        //
        //   u:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        //
        //   v:
        //     Floating-point value that represents the PositionNormalTexturedX2.#ctor()
        //     component of the texture coordinate.
        public PositionNormalTextured(float xvalue, float yvalue, float zvalue, float nxvalue, float nyvalue, float nzvalue, float u, float v)
        {
            X = xvalue;
            Y = yvalue;
            Z = zvalue;
            Nx = nxvalue;
            Ny = nyvalue;
            Nz = nzvalue;
            Tu = u;
            Tv = v;

        }

        // Summary:
        //     Retrieves or sets the vertex normal data.
        public Vector3 Normal
        {
            get
            {
                return new Vector3(Nx, Ny, Nz);
            }
            set
            {
                Nx = (float)value.X;
                Ny = (float)value.Y;
                Nz = (float)value.Z;
            }
        }
        //
        // Summary:
        //     Retrieves or sets the vertex position.
        public Vector3 Position
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = (float)value.X;
                Y = (float)value.Y;
                Z = (float)value.Z;
            }
        }

        public Vector3 Pos
        {
            get
            {
                return new Vector3(X, Y, Z);
            }
            set
            {
                X = (float)value.X;
                Y = (float)value.Y;
                Z = (float)value.Z;
            }
        }

        // Summary:
        //     Obtains a string representation of the current instance.
        //
        // Returns:
        //     String that represents the object.
        public override string ToString()
        {
            return string.Format(
                "X={0}, Y={1}, Z={2}, Nx={3}, Ny={4}, Nz={5}, U={6}, V={7}, U1={8}, U2={9}",
                X, Y, Z, Nx, Ny, Nz, Tu, Tv
                );
        }
    }

    public struct KeplerVertex
    {

        public Vector3 ABC;
        public Vector3 abc;
        public float PointSize;
        public uint color;

        public float w;
        public float e;
        public float n;
        public float T;
        public float a;
        public float z;
        public float orbitPos;
        public float orbits;
        public uint corner;

        static double sine = .0;
        static double cose = 1;
        static double degrad = Math.PI / 180;


        public System.Drawing.Color Color
        {
            get
            {
                return System.Drawing.Color.FromArgb((byte)(color >> 24), (byte)color, (byte)(color >> 8), (byte)(color >> 16));
            }
            set
            {
                color = (uint)(((uint)value.A) << 24) | (((uint)value.B) << 16) | (((uint)value.G) << 8) | (((uint)value.R));
            }
        }

        public static int baseDate = (int)SpaceTimeController.UtcToJulian(DateTime.Now);
        public void Fill(CAAEllipticalObjectElements ee)
        {
            double F = Math.Cos(ee.omega * degrad);
            double sinOmega = Math.Sin(ee.omega * degrad);
            double cosi = Math.Cos(ee.i * degrad);
            double sini = Math.Sin(ee.i * degrad);
            double G = sinOmega * cose;
            double H = sinOmega * sine;
            double P = -sinOmega * cosi;
            double Q = (F * cosi * cose) - (sini * sine);
            double R = (F * cosi * sine) + (sini * cose);

            double checkA = (F * F) + (G * G) + (H * H);// Should be 1.0
            double checkB = (P * P) + (Q * Q) + (R * R); // should be 1.0 as well

            ABC.X = (float)Math.Atan2(F, P);
            ABC.Y = (float)Math.Atan2(G, Q);
            ABC.Z = (float)Math.Atan2(H, R);

            abc.X = (float)Math.Sqrt((F * F) + (P * P));
            abc.Y = (float)Math.Sqrt((G * G) + (Q * Q));
            abc.Z = (float)Math.Sqrt((H * H) + (R * R));

            PointSize = .1f;
            if (ee.a < 2.5)
            {
                Color = System.Drawing.Color.White;
            }
            else if (ee.a < 2.83)
            {
                Color = System.Drawing.Color.Red;
            }
            else if (ee.a < 2.96)
            {
                Color = System.Drawing.Color.Green;
            }
            else if (ee.a < 3.3)
            {
                Color = System.Drawing.Color.Magenta;
            }
            else if (ee.a < 5)
            {
                Color = System.Drawing.Color.Cyan;
            }
            else if (ee.a < 10)
            {
                Color = System.Drawing.Color.Yellow;
                PointSize = .9f;
            }
            else
            {
                Color = System.Drawing.Color.White;
                PointSize = 8f;
            }
            w = (float)ee.w;
            e = (float)ee.e;
            if (ee.n == 0)
            {
                n = (float)((0.9856076686 / (ee.a * Math.Sqrt(ee.a))));
            }
            else
            {
                n = (float)ee.n;
            }
            T = (float)(ee.T - baseDate);
            a = (float)ee.a;
            z = 0;

            orbitPos = 0;
            orbits = 0;

        }
    }
}
