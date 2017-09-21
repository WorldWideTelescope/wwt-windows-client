using System;
using SharpDX.Direct3D11;
using System.IO;
using System.Threading.Tasks;
#if WINDOWS_UWP
#else
using SysColor = System.Drawing.Color;
#endif

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

        private ShaderResourceView resourceView = null;

        public int Id = nextID++;
        public Texture11(Texture2D t)
        {
            texture = t;
            resourceView = new ShaderResourceView(texture.Device, texture);
        }
#if WINDOWS_UWP
        public Texture11(string filename)
        {
            if (String.IsNullOrWhiteSpace(filename))
            {
                int o = 0;
            }
            var t = Task.Run(() =>
            {
                using (var bitmap = TextureLoader.LoadBitmap(RenderContext11.WicImagingFactory, filename))
                {
                   texture = TextureLoader.CreateTexture2DFromBitmap(RenderContext11.PrepDevice, bitmap);
                   resourceView = new ShaderResourceView(texture.Device, texture);
                }

            });
        }
#endif

#if WINDOWS_UWP
        public Texture11(System.IO.Stream stream)
        {
            if (stream == null)
            {
                throw new InvalidDataException("Stream can not be null");
            }
            var t = Task.Run(() =>
            {
                using (var bitmap = TextureLoader.LoadBitmap(RenderContext11.WicImagingFactory, stream))
                {
                    texture = TextureLoader.CreateTexture2DFromBitmap(RenderContext11.PrepDevice, bitmap);
                    resourceView = new ShaderResourceView(texture.Device, texture);
                }

            });
        }
#endif
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
                if (texture != null)
                {
                    return texture.Description.Width;
                }
                return 0;
            }
        }

        public int Height
        {
            get
            {
                if (texture != null)
                {
                    return texture.Description.Height;
                }
                return 0;
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
#if WINODWS_UWP
            return new Texture11(filename);
#else
            return FromFile(RenderContext11.PrepDevice, fileName);
#endif
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

        static public Texture11 FromFileImediate(Device device,string filename, LoadOptions options = LoadOptions.AssumeSRgb)
        {
#if WINDOWS_UWP
            using (var bitmap = TextureLoader.LoadBitmap(RenderContext11.WicImagingFactory, filename))
            {
                return new Texture11(TextureLoader.CreateTexture2DFromBitmap(RenderContext11.PrepDevice, bitmap));
            }
#else
            return FromFile(device, filename, options);
#endif
        }


        static public Texture11 FromFile(Device device, string fileName, LoadOptions options = LoadOptions.AssumeSRgb)
        {
#if WINDOWS_UWP
            return new Texture11(fileName);
            //using (var bitmap = TextureLoader.LoadBitmap(RenderContext11.WicImagingFactory, fileName))
            //{
            //    return new Texture11(TextureLoader.CreateTexture2DFromBitmap(RenderContext11.PrepDevice, bitmap));
            //}
#else

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
#endif
        }

        static public Texture11 FromBitmap(object bmp)
        {
            return FromBitmap(RenderContext11.PrepDevice, bmp);

        }

        static public Texture11 FromBitmap(object bmp, uint transparentColor)
        {

#if !WINDOWS_UWP
            System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)bmp;
            bitmap.MakeTransparent(SysColor.FromArgb((int) transparentColor));
            return FromBitmap(RenderContext11.PrepDevice, bitmap);
#else

            return null;
#endif
        }

        static public Texture11 FromBitmap(Device device, object bitmap)
        {
#if !WINDOWS_UWP
            System.Drawing.Bitmap bmp = (System.Drawing.Bitmap)bitmap;
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
#else
            //todo fix this 
            return null;
#endif
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
#if WINDOWS_UWP
            using (var bitmap = TextureLoader.LoadBitmap(RenderContext11.WicImagingFactory, stream))
            {
                return new Texture11(TextureLoader.CreateTexture2DFromBitmap(RenderContext11.PrepDevice, bitmap));
            }
#else
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
#endif
        }
    }

#if WINDOWS_UWP
    internal class TextureLoader
    {
        /// <summary>
        /// Loads a bitmap using WIC.
        /// </summary>
        /// <param name="deviceManager"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static SharpDX.WIC.BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, string filename)
        {
            return LoadBitmap(factory, new SharpDX.WIC.BitmapDecoder(
                factory,
                filename,
                SharpDX.WIC.DecodeOptions.CacheOnDemand));
        }

        public static SharpDX.WIC.BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, System.IO.Stream stream)
        {
            return LoadBitmap(factory, new SharpDX.WIC.BitmapDecoder(
                factory,
                stream,
                SharpDX.WIC.DecodeOptions.CacheOnDemand));
        }

        private static SharpDX.WIC.BitmapSource LoadBitmap(SharpDX.WIC.ImagingFactory2 factory, SharpDX.WIC.BitmapDecoder bitmapDecoder)
        {
            var formatConverter = new SharpDX.WIC.FormatConverter(factory);
            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                SharpDX.WIC.PixelFormat.Format32bppPRGBA,
                SharpDX.WIC.BitmapDitherType.None,
                null,
                0.0,
                SharpDX.WIC.BitmapPaletteType.Custom);
            return formatConverter;
        }

        /// <summary>
        /// Creates a <see cref="SharpDX.Direct3D11.Texture2D"/> from a WIC <see cref="SharpDX.WIC.BitmapSource"/>
        /// </summary>
        /// <param name="device">The Direct3D11 device</param>
        /// <param name="bitmapSource">The WIC bitmap source</param>
        /// <returns>A Texture2D</returns>

        //public static SharpDX.Direct3D11.Texture2D CreateTexture2DFromBitmap(SharpDX.Direct3D11.Device device, SharpDX.WIC.BitmapSource bitmapSource)
        //{
        //    int width = bitmapSource.Size.Width;
        //    int height = bitmapSource.Size.Height;
        //    int levels = CountMips(width, height);
        //    int size = CalculateMipSize(width, height);

        //    // Allocate DataStream to receive the WIC image pixels
        //    int stride = width * 4;

        //    byte[] data = new byte[width * height * 4];
        //    // Copy the content of the WIC to the buffer
        //    //bitmapSource.CopyPixels(stride, buffer);
        //    bitmapSource.CopyPixels(data, stride);

        //    var tex = new SharpDX.Direct3D11.Texture2D(device, new SharpDX.Direct3D11.Texture2DDescription()
        //    {
        //        Width = width,
        //        Height = height,
        //        ArraySize = 1,
        //        BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
        //        Usage = SharpDX.Direct3D11.ResourceUsage.Default,
        //        CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
        //        Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
        //        MipLevels = levels,
        //        OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.GenerateMipMaps,
        //        SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
        //    });

        //    //device.ImmediateContext.UpdateSubresource(data, tex);

        //    // size                    }, new SharpDX.DataRectangle(buffer.DataPointer, stride));         
        //    var view = new ShaderResourceView(RenderContext11.PrepDevice, tex);

        //    //device.ImmediateContext.GenerateMips(view);

        //    view.Dispose();
        //    return tex;

        //}

        public static unsafe SharpDX.Direct3D11.Texture2D CreateTexture2DFromBitmap(SharpDX.Direct3D11.Device device, SharpDX.WIC.BitmapSource bitmapSource)
        {
            int width = bitmapSource.Size.Width;
            int height = bitmapSource.Size.Height;
            int levels = CountMips(width, height);
            int w = width;
            int h = height;

            SharpDX.DataBox[] boxes = new SharpDX.DataBox[levels];
            SharpDX.DataStream[] ds = new SharpDX.DataStream[levels];
            byte[][] data = new byte[levels][];

            for (int i = 0; i < levels; i++)
            {
                if (i == 0)
                {
                    data[i] = new byte[w * h * 4];
                    int stride = w * 4;
                    bitmapSource.CopyPixels(data[i], stride);
                }
                else
                {
                    data[i] = new byte[w * h * 4];
                    var po = data[i];
                    var pi = (byte*)boxes[i - 1].DataPointer.ToPointer();
                    var in1 = pi;
                    var in2 = in1 + 4;
                    var in3 = pi + 8 * w;
                    var in4 = in3 + 4;
                    int index = 0;
                    for(int y = 0; y < h; y++)
                    {                     
                        for(int x = 0; x < w; x++)
                        {
                            for(int c= 0; c< 4; c++)
                            {
                                po[index] = (byte)(((int)*in1 + (int)*in2 + (int)*in3 + (int)*in4) / 4);
                                in1 += 1;
                                in2 += 1;
                                in3 += 1;
                                in4 += 1;
                                index++;
                            }
                            in1 += 4;
                            in2 += 4;
                            in3 += 4;
                            in4 += 4;
                        }

                        in1 += (w * 8);
                        in2 += (w * 8);
                        in3 += (w * 8);
                        in4 += (w * 8);
                    }
                }

                ds[i] = new SharpDX.DataStream(data[i].Length, true, true);
                ds[i].Write(data[i], 0, data[i].Length);

                boxes[i].DataPointer = ds[i].DataPointer;

                boxes[i].RowPitch = w * 4;

                if (w > 1)
                {
                    w = w / 2;
                }

                if (h > 1)
                {
                    h = h / 2;
                }
            }


            // Allocate DataStream to receive the WIC image pixels




            var tex = new SharpDX.Direct3D11.Texture2D(device, new SharpDX.Direct3D11.Texture2DDescription()
            {
                Width = width,
                Height = height,
                ArraySize = 1,
                BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                Usage = SharpDX.Direct3D11.ResourceUsage.Immutable,
                CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                MipLevels = levels,
                OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
            }, boxes);

            for (int i = 0; i < levels; i++)
            {
                ds[i].Dispose();
            }

            return tex;

        }


        //static int CalculateMipSize(int width, int height)
        //{
        //    int size = 0;

        //    int mipLevels = 1;

        //    while (height > 1 || width > 1)
        //    {
        //        size += width * height;
        //        if (height > 1)
        //        {
        //            height >>= 1;
        //        }

        //        if (width > 1)
        //        {

        //            width >>= 1;
        //        }

        //        mipLevels++;
        //    }

        //    return size;
        //}


        static int CountMips(int width, int height)
        {
            int mipLevels = 1;

            while (height > 1 || width > 1)
            {
                if (height > 1)
                {

                    height >>= 1;
                }

                if (width > 1)
                {

                    width >>= 1;
                }

                mipLevels++;
            }

            return mipLevels;
        }

        public static SharpDX.Direct3D11.Texture2D CreateTexture2DFromBitmapNoMip(SharpDX.Direct3D11.Device device, SharpDX.WIC.BitmapSource bitmapSource)
        {
            // Allocate DataStream to receive the WIC image pixels
            int stride = bitmapSource.Size.Width * 4;
            using (var buffer = new SharpDX.DataStream(bitmapSource.Size.Height * stride, true, true))
            {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                return new SharpDX.Direct3D11.Texture2D(device, new SharpDX.Direct3D11.Texture2DDescription()
                {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = SharpDX.Direct3D11.BindFlags.ShaderResource,
                    Usage = SharpDX.Direct3D11.ResourceUsage.Immutable,
                    CpuAccessFlags = SharpDX.Direct3D11.CpuAccessFlags.None,
                    Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
                    OptionFlags = SharpDX.Direct3D11.ResourceOptionFlags.None,
                    SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                }, new SharpDX.DataRectangle(buffer.DataPointer, stride));
            }
        }
    }
#endif
}
