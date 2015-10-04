using System;
using System.Drawing.Imaging;
using SharpDX.Direct3D11;
using System.Drawing;
using System.IO;
using SharpDX.DXGI;
using Color = System.Drawing.Color;
using Device = SharpDX.Direct3D11.Device;

namespace TerraViewer
{
    public class Texture11 : IDisposable
    {
        private static int nextID;
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

        static Format promoteFormatToSRGB(Format format)
        {
            switch (format)
            {
                case Format.R8G8B8A8_UNorm:
                    return Format.R8G8B8A8_UNorm_SRgb;
                case Format.B8G8R8A8_UNorm:
                    return Format.B8G8R8A8_UNorm_SRgb;
                case Format.B8G8R8X8_UNorm:
                    return Format.B8G8R8X8_UNorm_SRgb;
                case Format.BC1_UNorm:
                    return Format.BC1_UNorm_SRgb;
                case Format.BC2_UNorm:
                    return Format.BC2_UNorm_SRgb;
                case Format.BC3_UNorm:
                    return Format.BC3_UNorm_SRgb;
                case Format.BC7_UNorm:
                    return Format.BC7_UNorm_SRgb;
                default:
                    return format;
            }
        }

        static bool isSRGBFormat(Format format)
        {
            switch (format)
            {
                case Format.R8G8B8A8_UNorm_SRgb:
                case Format.B8G8R8A8_UNorm_SRgb:
                case Format.B8G8R8X8_UNorm_SRgb:
                case Format.BC1_UNorm_SRgb:
                case Format.BC2_UNorm_SRgb:
                case Format.BC3_UNorm_SRgb:
                case Format.BC7_UNorm_SRgb:
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
                var loadInfo = new ImageLoadInformation();
                loadInfo.BindFlags = BindFlags.ShaderResource;
                loadInfo.CpuAccessFlags = CpuAccessFlags.None;
                loadInfo.Depth = -1;
                loadInfo.Filter = FilterFlags.Box;
                loadInfo.FirstMipLevel = 0;
                loadInfo.Format = Format.R8G8B8A8_UNorm;

                loadInfo.Height = -1;
                loadInfo.MipLevels = -1;
                loadInfo.OptionFlags = ResourceOptionFlags.None;
                loadInfo.Usage = ResourceUsage.Default;
                loadInfo.Width = -1;

                var shouldPromoteSRgb = RenderContext11.sRGB && (options & LoadOptions.AssumeSRgb) == LoadOptions.AssumeSRgb;

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
                    var info = ImageInformation.FromFile(fileName);
                    if (info.HasValue && shouldPromoteSRgb)
                    {
                        loadInfo.Format = promoteFormatToSRGB(info.Value.Format);
                    }
                    if (isSRGBFormat(loadInfo.Format) )
                    {
                        loadInfo.Filter |= FilterFlags.SRgb;
                    }
                }

                var texture = Texture2D.FromFile<Texture2D>(device, fileName, loadInfo);

                return new Texture11(texture);
            }
            catch (Exception e)
            {
                try
                {
                    var ili = new ImageLoadInformation()
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
                    if (ili.Format == Format.R8G8B8A8_UNorm_SRgb)
                    {
                        ili.Filter |= FilterFlags.SRgb;
                    }

                    var texture = Texture2D.FromFile<Texture2D>(device, fileName, ili);
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
            bmp.MakeTransparent(Color.FromArgb((int) transparentColor));
            return FromBitmap(RenderContext11.PrepDevice, bmp);
        }

        static public Texture11 FromBitmap(Device device, Bitmap bmp)
        {
            var ms = new MemoryStream();

            bmp.Save(ms, ImageFormat.Png);

            ms.Seek(0, SeekOrigin.Begin);

            if (IsPowerOf2((uint)bmp.Width) && IsPowerOf2((uint)bmp.Height))
            {
                var loadInfo = new ImageLoadInformation();
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
                if (loadInfo.Format == Format.R8G8B8A8_UNorm_SRgb)
                {
                    loadInfo.Filter |= FilterFlags.SRgb;
                }

                var texture = Texture2D.FromStream<Texture2D>(device, ms, (int)ms.Length, loadInfo);

                ms.Dispose();
                return new Texture11(texture);
            }
            else
            {
                ms.Seek(0, SeekOrigin.Begin);
                var ili = new ImageLoadInformation()
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
                if (ili.Format == Format.R8G8B8A8_UNorm_SRgb)
                {
                    ili.Filter |= FilterFlags.SRgb;
                }

                var texture = Texture2D.FromStream<Texture2D>(device, ms, (int)ms.Length, ili);
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

        static public Texture11 FromStream(Device device, Stream stream)
        {
            try
            {
                var loadInfo = new ImageLoadInformation();
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
                if (loadInfo.Format == Format.R8G8B8A8_UNorm_SRgb)
                {
                    loadInfo.Filter |= FilterFlags.SRgb;
                }

                var texture = Texture2D.FromStream<Texture2D>(device, stream, (int)stream.Length, loadInfo);

                return new Texture11(texture);
            }
            catch
            {
                return null;
            }
        }
    }
}
