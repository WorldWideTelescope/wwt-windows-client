using OculusWrap;
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    public class EyeTexture : IDisposable
    {
        #region IDisposable Members
        /// <summary>
        /// Dispose contained fields.
        /// </summary>
        public void Dispose()
        {
            if (SwapTextureSet != null)
            {
                SwapTextureSet.Dispose();
                SwapTextureSet = null;
            }

            if (Textures != null)
            {
                foreach (Texture2D texture in Textures)
                    texture.Dispose();

                Textures = null;
            }

            if (RenderTargetViews != null)
            {
                foreach (RenderTargetView renderTargetView in RenderTargetViews)
                    renderTargetView.Dispose();

                RenderTargetViews = null;
            }

            if (DepthBuffer != null)
            {
                DepthBuffer.Dispose();
                DepthBuffer = null;
            }

            if (DepthStencilView != null)
            {
                DepthStencilView.Dispose();
                DepthStencilView = null;
            }
        }
        #endregion

        public Texture2DDescription Texture2DDescription;
        public TextureSwapChain SwapTextureSet;
        public Texture2D[] Textures;
        public RenderTargetView[] RenderTargetViews;
        public Texture2DDescription DepthBufferDescription;
        public Texture2D DepthBuffer;
        public Viewport Viewport;
        public DepthStencilView DepthStencilView;
        public OVRTypes.FovPort FieldOfView;
        public OVRTypes.Sizei TextureSize;
        public OVRTypes.Recti ViewportSize;
        public OVRTypes.EyeRenderDesc RenderDescription;
        public OVRTypes.Vector3f HmdToEyeViewOffset;
    }
}
