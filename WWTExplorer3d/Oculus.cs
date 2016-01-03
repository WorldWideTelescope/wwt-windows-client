using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    class Oculus
    {
    }

    public enum RenderAPIType
    {
        /// <summary>
        /// No API
        /// </summary>
        None = 0,

        /// <summary>
        /// OpenGL
        /// </summary>
        OpenGL = 1,

        /// <summary>
        /// OpenGL ES
        /// </summary>
        Android_GLES = 2,

        /// <summary>
        /// DirectX 11.
        /// </summary>
        D3D11 = 5,

        /// <summary>
        /// Count of enumerated elements.
        /// </summary>
        Count = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Sizei
    {
        public Sizei(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width,
                    Height;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct TextureHeader
    {
        /// <summary>
        /// The API type to which this texture belongs.
        /// </summary>
        public RenderAPIType API;

        /// <remarks>
        /// Size of this texture in pixels.
        /// </remarks>
        public Sizei TextureSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3D11TextureData
    {
        /// <summary>
        /// General device settings.
        /// </summary>
        public TextureHeader Header;

        /// <summary>
        /// The D3D11 texture containing the undistorted eye image.
        /// </summary>
        /// <remarks>
        /// ID3D11Texture2D
        /// </remarks>
        public IntPtr Texture;

        /// <summary>
        /// The D3D11 shader resource view for this texture.
        /// </summary>
        /// <remarks>
        /// ID3D11ShaderResourceView
        /// </remarks>
        public IntPtr ShaderResourceView;
    };


    public struct Texture
    {
        /// <summary>
        /// API-independent header.
        /// </summary>
        public TextureHeader Header;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public IntPtr[] PlatformData;
    }

    public class SwapTextureSetD3D
    {
        private SharpOVR.SwapTextureSet SwapTextureSet;
        public SwapTextureSetD3D(IntPtr swapTextureSetPtr)

        {
            
            SwapTextureSet = (SharpOVR.SwapTextureSet)Marshal.PtrToStructure(swapTextureSetPtr, typeof(SharpOVR.SwapTextureSet));
            
            // Allocate the list of managed textures.
            Textures = new D3D11TextureData[SwapTextureSet.TextureCount];

            // The size of the OVR.D3D11.D3D11TextureData is defined as a C++ union between the 
            // OVR.Texture and the OVR.D3D11.D3D11TextureData type. As the OVR.Texture is larger
            // than the size of the OVR.D3D11.D3D11TextureData, the size of the OVR.Texture is 
            // used to determine the unmanaged size of the OVR.D3D11.D3D11TextureData.
            int textureSize = Marshal.SizeOf(typeof(Texture));

            for (int textureIndex = 0; textureIndex < SwapTextureSet.TextureCount; textureIndex++)
            {
                // Copy the contents of the unmanaged texture to the list of managed textures.
                Textures[textureIndex] = (D3D11TextureData)Marshal.PtrToStructure(SwapTextureSet.Textures + textureSize * textureIndex, typeof(D3D11TextureData));
            }
        }

        /// <summary>
        /// Array of textures.
        /// </summary>
        public IList<D3D11TextureData> Textures
        {
            get;
            private set;
        }
    }
}
