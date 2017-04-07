using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX;
using System.Drawing;
using System.IO;

namespace TerraViewer
{
    public class Texture11 : IDisposable
    {
        private static int nextID = 0;
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        private ShaderResourceView resourceView;
        public int Id = nextID++;
        public Texture11(Texture2D t)
        {
            texture = t;
            resourceView = new ShaderResourceView(texture.Device, texture);
        }

        public ShaderResourceView ResourceView
        {
            get
            {
                return resourceView;
            }
        }

        public int Width
        {
            get
            {
                return texture.Description.Width;
            }
        }

        public int Height
        {
            get
            {
                return texture.Description.Height;
            }
        }


        public void Dispose()
        {
            if (resourceView != null)
            {
                resourceView.Dispose();
                GC.SuppressFinalize(resourceView);
                resourceView = null;
            }
            if (texture != null)
            {
                texture.Dispose();
                GC.SuppressFinalize(texture);
                texture = null;
            }
        }

        static public Texture11 FromFile(string fileName)
        {
            
            
                return FromFile(RenderContext11.PrepDevice, fileName);
           
        }

        static SharpDX.DXGI.Format promoteFormatToSRGB(SharpDX.DXGI.Format format)
        {
            switch (format)
            {
                case SharpDX.DXGI.Format.R8G8B8A8_UNorm:
                    return SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb;
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm:
                    return SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb;
                case SharpDX.DXGI.Format.B8G8R8X8_UNorm:
                    return SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb;
                case SharpDX.DXGI.Format.BC1_UNorm:
                    return SharpDX.DXGI.Format.BC1_UNorm_SRgb;
                case SharpDX.DXGI.Format.BC2_UNorm:
                    return SharpDX.DXGI.Format.BC2_UNorm_SRgb;
                case SharpDX.DXGI.Format.BC3_UNorm:
                    return SharpDX.DXGI.Format.BC3_UNorm_SRgb;
                case SharpDX.DXGI.Format.BC7_UNorm:
                    return SharpDX.DXGI.Format.BC7_UNorm_SRgb;
                default:
                    return format;
            }
        }

        static bool isSRGBFormat(SharpDX.DXGI.Format format)
        {
            switch (format)
            {
                case SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:
                case SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:
                case SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb:
                case SharpDX.DXGI.Format.BC1_UNorm_SRgb:
                case SharpDX.DXGI.Format.BC2_UNorm_SRgb:
                case SharpDX.DXGI.Format.BC3_UNorm_SRgb:
                case SharpDX.DXGI.Format.BC7_UNorm_SRgb:
                    return true; ;
                default:
                    return false;
            }
        }

        [Flags]
        public enum LoadOptions
        {
            None       = 0x0,
            AssumeSRgb = 0x1,
        };

        static public Texture11 FromFile(Device device, string fileName, LoadOptions options = LoadOptions.AssumeSRgb)
        {
            try
            {
                ImageLoadInformation loadInfo = new ImageLoadInformation();
                loadInfo.BindFlags = BindFlags.ShaderResource;
                loadInfo.CpuAccessFlags = CpuAccessFlags.None;
                loadInfo.Depth = -1;
                loadInfo.Filter = FilterFlags.Box;
                loadInfo.FirstMipLevel = 0;
                loadInfo.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

                loadInfo.Height = -1;
                loadInfo.MipLevels = -1;
                loadInfo.OptionFlags = ResourceOptionFlags.None;
                loadInfo.Usage = ResourceUsage.Default;
                loadInfo.Width = -1;

                bool shouldPromoteSRgb = RenderContext11.sRGB && (options & LoadOptions.AssumeSRgb) == LoadOptions.AssumeSRgb;

                if (fileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase) ||
                    fileName.EndsWith(".jpg", StringComparison.InvariantCultureIgnoreCase) ||
                    fileName.EndsWith(".jpeg", StringComparison.InvariantCultureIgnoreCase))
                {
                    loadInfo.Filter = FilterFlags.Box;
                    if (shouldPromoteSRgb)
                    {
                        loadInfo.Format = promoteFormatToSRGB(loadInfo.Format);
                    }
                    if (isSRGBFormat(loadInfo.Format))
                    {
                        loadInfo.Filter |= FilterFlags.SRgb;
                    }
                }
                else
                {
                    // Promote image format to sRGB
                    ImageInformation? info = ImageInformation.FromFile(fileName);
                    if (info.HasValue && shouldPromoteSRgb)
                    {
                        loadInfo.Format = promoteFormatToSRGB(info.Value.Format);
                    }
                    if (isSRGBFormat(loadInfo.Format) )
                    {
                        loadInfo.Filter |= FilterFlags.SRgb;
                    }
                }

                Texture2D texture = Texture2D.FromFile<Texture2D>(device, fileName, loadInfo);

                return new Texture11(texture);
            }
            catch (Exception e)
            {
                try
                {
                    ImageLoadInformation ili = new ImageLoadInformation()
                                {
                                    BindFlags = BindFlags.ShaderResource,
                                    CpuAccessFlags = CpuAccessFlags.None,
                                    Depth = -1,
                                    Filter = FilterFlags.Box,
                                    FirstMipLevel = 0,
                                    Format = RenderContext11.DefaultTextureFormat,
                                    Height = -1,
                                    MipFilter = FilterFlags.None,
                                    MipLevels = 1,
                                    OptionFlags = ResourceOptionFlags.None,
                                    Usage = ResourceUsage.Default,
                                    Width = -1
                                };
                    if (ili.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb)
                    {
                        ili.Filter |= FilterFlags.SRgb;
                    }

                    Texture2D texture = Texture2D.FromFile<Texture2D>(device, fileName, ili);
                    return new Texture11(texture);

                }
                catch
                {
                    return null;
                }
            }
        }

        static public Texture11 FromBitmap(Bitmap bmp)
        {
            return FromBitmap(RenderContext11.PrepDevice, bmp);

        }

        static public Texture11 FromBitmap(Bitmap bmp, uint transparentColor)
        {
            bmp.MakeTransparent(System.Drawing.Color.FromArgb((int) transparentColor));
            return FromBitmap(RenderContext11.PrepDevice, bmp);
        }

        static public Texture11 FromBitmap(Device device, Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();

            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            ms.Seek(0, SeekOrigin.Begin);

            if (IsPowerOf2((uint)bmp.Width) && IsPowerOf2((uint)bmp.Height))
            {
                ImageLoadInformation loadInfo = new ImageLoadInformation();
                loadInfo.BindFlags = BindFlags.ShaderResource;
                loadInfo.CpuAccessFlags = CpuAccessFlags.None;
                loadInfo.Depth = -1;
                loadInfo.Format = RenderContext11.DefaultTextureFormat;
                loadInfo.Filter = FilterFlags.Box;
                loadInfo.FirstMipLevel = 0;
                loadInfo.Height = -1;
                loadInfo.MipFilter = FilterFlags.Linear;
                loadInfo.MipLevels = 0;
                loadInfo.OptionFlags = ResourceOptionFlags.None;
                loadInfo.Usage = ResourceUsage.Default;
                loadInfo.Width = -1;
                if (loadInfo.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb)
                {
                    loadInfo.Filter |= FilterFlags.SRgb;
                }

                Texture2D texture = Texture2D.FromStream<Texture2D>(device, ms, (int)ms.Length, loadInfo);

                ms.Dispose();
                return new Texture11(texture);
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                ImageLoadInformation ili = new ImageLoadInformation()
                {
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Depth = -1,
                    Filter = FilterFlags.Box,
                    FirstMipLevel = 0,
                    Format = RenderContext11.DefaultTextureFormat,
                    Height = -1,
                    MipFilter = FilterFlags.None,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    Usage = ResourceUsage.Default,
                    Width = -1
                };
                if (ili.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb)
                {
                    ili.Filter |= FilterFlags.SRgb;
                }

                Texture2D texture = Texture2D.FromStream<Texture2D>(device, ms, (int)ms.Length, ili);
                ms.Dispose();
                return new Texture11(texture);
            }
        }

        static bool IsPowerOf2(uint x)
        {
            return ((x & (x - 1)) == 0);
        }

        static public Texture11 FromStream(Stream stream)
        {
            return FromStream(RenderContext11.PrepDevice, stream);
        }

        public void SaveToFile(string filename)
        {
            if (SaveStream && savedStrem != null)
            {
                File.WriteAllBytes(filename, savedStrem);
            }
        }

        byte[] savedStrem = null;
        static public bool SaveStream = false;

        static public Texture11 FromStream(Device device, Stream stream)
        {
            byte[] data = null;
            if (SaveStream)
            {
                MemoryStream ms = new MemoryStream();

                stream.CopyTo(ms);
                stream.Seek(0, SeekOrigin.Begin);
                data = ms.GetBuffer();        
            }

            try
            {
                ImageLoadInformation loadInfo = new ImageLoadInformation();
                loadInfo.BindFlags = BindFlags.ShaderResource;
                loadInfo.CpuAccessFlags = CpuAccessFlags.None;
                loadInfo.Depth = -1;
                loadInfo.Format = RenderContext11.DefaultTextureFormat;
                loadInfo.Filter = FilterFlags.Box;
                loadInfo.FirstMipLevel = 0;
                loadInfo.Height = -1;
                loadInfo.MipFilter = FilterFlags.Linear;
                loadInfo.MipLevels = 0;
                loadInfo.OptionFlags = ResourceOptionFlags.None;
                loadInfo.Usage = ResourceUsage.Default;
                loadInfo.Width = -1;
                if (loadInfo.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb)
                {
                    loadInfo.Filter |= FilterFlags.SRgb;
                }

                Texture2D texture = Texture2D.FromStream<Texture2D>(device, stream, (int)stream.Length, loadInfo);

                Texture11 t11 = new Texture11(texture);
                t11.savedStrem = data;
                return t11;
            }
            catch
            {
                return null;
            }
        }
    }
}
