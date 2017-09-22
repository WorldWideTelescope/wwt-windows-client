using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraViewer
{
    //todo UWP  make a real useful implimentation of this
    public class Bitmap : IDisposable
    {
        private int v1;
        private int v2;

        public Bitmap(int v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }

        public Bitmap()
        {
            
        }

        public System.IO.Stream Stream;

        public Bitmap(System.IO.Stream stream)
        {
            Stream = stream;
            HashCode = stream.GetHashCode();
        }

        public Bitmap(string filename)
        {
            this.FileName = filename;
            HashCode = filename.GetHashCode();
        }

        //todo UWP impliment a mapping to a real BITMAP 
        public int Width { get;  set; }
        public int Height { get;  set; }

        public string FileName { get; set; }
        public int HashCode { get; internal set; }

        public void Dispose()
        {
        }

        public void UnlockBits(BitmapData bitmapData)
        {
            throw new NotImplementedException();
        }

        public BitmapData LockBits(Rectangle bounds, ImageLockMode readWrite, PixelFormat format24bppRgb)
        {
            throw new NotImplementedException();
        }

        public void Save(string filename, ImageFormat jpeg)
        {
            throw new NotImplementedException();
        }

        public RectangleF GetBounds(ref GraphicsUnit unit)
        {
            unit = GraphicsUnit.Pixel;

            return new RectangleF();
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public override bool Equals(object obj)
        {
            Bitmap other = obj as Bitmap;

            if (other != null)
            {
                return HashCode == other.HashCode;
            }

            return false;
        }

       
    }

    //
    // Summary:
    //     Specifies flags that are passed to the flags parameter of the Overload:System.Drawing.Bitmap.LockBits
    //     method. The Overload:System.Drawing.Bitmap.LockBits method locks a portion of
    //     an image so that you can read or write the pixel data.
    public enum ImageLockMode
    {
        //
        // Summary:
        //     Specifies that a portion of the image is locked for reading.
        ReadOnly = 1,
        //
        // Summary:
        //     Specifies that a portion of the image is locked for writing.
        WriteOnly = 2,
        //
        // Summary:
        //     Specifies that a portion of the image is locked for reading or writing.
        ReadWrite = 3,
        //
        // Summary:
        //     Specifies that the buffer used for reading or writing pixel data is allocated
        //     by the user. If this flag is set, the flags parameter of the Overload:System.Drawing.Bitmap.LockBits
        //     method serves as an input parameter (and possibly as an output parameter). If
        //     this flag is cleared, then the flags parameter serves only as an output parameter.
        UserInputBuffer = 4
    }

    //
    // Summary:
    //     Specifies the unit of measure for the given data.
    public enum GraphicsUnit
    {
        //
        // Summary:
        //     Specifies the world coordinate system unit as the unit of measure.
        World = 0,
        //
        // Summary:
        //     Specifies the unit of measure of the display device. Typically pixels for video
        //     displays, and 1/100 inch for printers.
        Display = 1,
        //
        // Summary:
        //     Specifies a device pixel as the unit of measure.
        Pixel = 2,
        //
        // Summary:
        //     Specifies a printer's point (1/72 inch) as the unit of measure.
        Point = 3,
        //
        // Summary:
        //     Specifies the inch as the unit of measure.
        Inch = 4,
        //
        // Summary:
        //     Specifies the document unit (1/300 inch) as the unit of measure.
        Document = 5,
        //
        // Summary:
        //     Specifies the millimeter as the unit of measure.
        Millimeter = 6
    }

    //
    // Summary:
    //     Specifies the format of the color data for each pixel in the image.
    public enum PixelFormat
    {
        //
        // Summary:
        //     The pixel format is undefined.
        Undefined = 0,
        //
        // Summary:
        //     No pixel format is specified.
        DontCare = 0,
        //
        // Summary:
        //     The maximum value for this enumeration.
        Max = 15,
        //
        // Summary:
        //     The pixel data contains color-indexed values, which means the values are an index
        //     to colors in the system color table, as opposed to individual color values.
        Indexed = 65536,
        //
        // Summary:
        //     The pixel data contains GDI colors.
        Gdi = 131072,
        //
        // Summary:
        //     Specifies that the format is 16 bits per pixel; 5 bits each are used for the
        //     red, green, and blue components. The remaining bit is not used.
        Format16bppRgb555 = 135173,
        //
        // Summary:
        //     Specifies that the format is 16 bits per pixel; 5 bits are used for the red component,
        //     6 bits are used for the green component, and 5 bits are used for the blue component.
        Format16bppRgb565 = 135174,
        //
        // Summary:
        //     Specifies that the format is 24 bits per pixel; 8 bits each are used for the
        //     red, green, and blue components.
        Format24bppRgb = 137224,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     red, green, and blue components. The remaining 8 bits are not used.
        Format32bppRgb = 139273,
        //
        // Summary:
        //     Specifies that the pixel format is 1 bit per pixel and that it uses indexed color.
        //     The color table therefore has two colors in it.
        Format1bppIndexed = 196865,
        //
        // Summary:
        //     Specifies that the format is 4 bits per pixel, indexed.
        Format4bppIndexed = 197634,
        //
        // Summary:
        //     Specifies that the format is 8 bits per pixel, indexed. The color table therefore
        //     has 256 colors in it.
        Format8bppIndexed = 198659,
        //
        // Summary:
        //     The pixel data contains alpha values that are not premultiplied.
        Alpha = 262144,
        //
        // Summary:
        //     The pixel format is 16 bits per pixel. The color information specifies 32,768
        //     shades of color, of which 5 bits are red, 5 bits are green, 5 bits are blue,
        //     and 1 bit is alpha.
        Format16bppArgb1555 = 397319,
        //
        // Summary:
        //     The pixel format contains premultiplied alpha values.
        PAlpha = 524288,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     alpha, red, green, and blue components. The red, green, and blue components are
        //     premultiplied, according to the alpha component.
        Format32bppPArgb = 925707,
        //
        // Summary:
        //     Reserved.
        Extended = 1048576,
        //
        // Summary:
        //     The pixel format is 16 bits per pixel. The color information specifies 65536
        //     shades of gray.
        Format16bppGrayScale = 1052676,
        //
        // Summary:
        //     Specifies that the format is 48 bits per pixel; 16 bits each are used for the
        //     red, green, and blue components.
        Format48bppRgb = 1060876,
        //
        // Summary:
        //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
        //     alpha, red, green, and blue components. The red, green, and blue components are
        //     premultiplied according to the alpha component.
        Format64bppPArgb = 1851406,
        //
        // Summary:
        //     The default pixel format of 32 bits per pixel. The format specifies 24-bit color
        //     depth and an 8-bit alpha channel.
        Canonical = 2097152,
        //
        // Summary:
        //     Specifies that the format is 32 bits per pixel; 8 bits each are used for the
        //     alpha, red, green, and blue components.
        Format32bppArgb = 2498570,
        //
        // Summary:
        //     Specifies that the format is 64 bits per pixel; 16 bits each are used for the
        //     alpha, red, green, and blue components.
        Format64bppArgb = 3424269
    }

    //
    // Summary:
    //     Specifies the file format of the image. Not inheritable.
    public sealed class ImageFormat
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Imaging.ImageFormat class by
        //     using the specified System.Guid structure.
        //
        // Parameters:
        //   guid:
        //     The System.Guid structure that specifies a particular image format.
        public ImageFormat(Guid guid)
        {

        }

        //
        // Summary:
        //     Gets the format of a bitmap in memory.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the format of a bitmap
        //     in memory.
        public static ImageFormat MemoryBmp { get; }
        //
        // Summary:
        //     Gets the bitmap (BMP) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the bitmap image
        //     format.
        public static ImageFormat Bmp { get; }
        //
        // Summary:
        //     Gets the enhanced metafile (EMF) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the enhanced metafile
        //     image format.
        public static ImageFormat Emf { get; }
        //
        // Summary:
        //     Gets the Windows metafile (WMF) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the Windows metafile
        //     image format.
        public static ImageFormat Wmf { get; }
        //
        // Summary:
        //     Gets the Graphics Interchange Format (GIF) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the GIF image format.
        public static ImageFormat Gif { get; }
        //
        // Summary:
        //     Gets the Joint Photographic Experts Group (JPEG) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the JPEG image format.
        public static ImageFormat Jpeg { get; }
        //
        // Summary:
        //     Gets the W3C Portable Network Graphics (PNG) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the PNG image format.
        public static ImageFormat Png { get; }
        //
        // Summary:
        //     Gets the Tagged Image File Format (TIFF) image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the TIFF image format.
        public static ImageFormat Tiff { get; }
        //
        // Summary:
        //     Gets the Exchangeable Image File (Exif) format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the Exif format.
        public static ImageFormat Exif { get; }
        //
        // Summary:
        //     Gets the Windows icon image format.
        //
        // Returns:
        //     An System.Drawing.Imaging.ImageFormat object that indicates the Windows icon
        //     image format.
        public static ImageFormat Icon { get; }
        //
        // Summary:
        //     Gets a System.Guid structure that represents this System.Drawing.Imaging.ImageFormat
        //     object.
        //
        // Returns:
        //     A System.Guid structure that represents this System.Drawing.Imaging.ImageFormat
        //     object.
        public Guid Guid { get; }

        //
        // Summary:
        //     Returns a value that indicates whether the specified object is an System.Drawing.Imaging.ImageFormat
        //     object that is equivalent to this System.Drawing.Imaging.ImageFormat object.
        //
        // Parameters:
        //   o:
        //     The object to test.
        //
        // Returns:
        //     true if o is an System.Drawing.Imaging.ImageFormat object that is equivalent
        //     to this System.Drawing.Imaging.ImageFormat object; otherwise, false.
        public override bool Equals(object o)
        {
            ImageFormat imf = (ImageFormat)o;

            return imf.Guid == Guid;
        }
        //
        // Summary:
        //     Returns a hash code value that represents this object.
        //
        // Returns:
        //     A hash code that represents this object.
        public override int GetHashCode()
        {
            return Guid.GetHashCode();
        }
        //
        // Summary:
        //     Converts this System.Drawing.Imaging.ImageFormat object to a human-readable string.
        //
        // Returns:
        //     A string that represents this System.Drawing.Imaging.ImageFormat object.
        public override string ToString()
        {
            return Guid.ToString();
        }
    }

    //
    // Summary:
    //     Specifies the attributes of a bitmap image. The System.Drawing.Imaging.BitmapData
    //     class is used by the Overload:System.Drawing.Bitmap.LockBits and System.Drawing.Bitmap.UnlockBits(System.Drawing.Imaging.BitmapData)
    //     methods of the System.Drawing.Bitmap class. Not inheritable.
    public class BitmapData
    {
        //
        // Summary:
        //     Initializes a new instance of the System.Drawing.Imaging.BitmapData class.
        public BitmapData()
        {

        }

        //
        // Summary:
        //     Gets or sets the pixel width of the System.Drawing.Bitmap object. This can also
        //     be thought of as the number of pixels in one scan line.
        //
        // Returns:
        //     The pixel width of the System.Drawing.Bitmap object.
        public int Width { get; set; }
        //
        // Summary:
        //     Gets or sets the pixel height of the System.Drawing.Bitmap object. Also sometimes
        //     referred to as the number of scan lines.
        //
        // Returns:
        //     The pixel height of the System.Drawing.Bitmap object.
        public int Height { get; set; }
        //
        // Summary:
        //     Gets or sets the stride width (also called scan width) of the System.Drawing.Bitmap
        //     object.
        //
        // Returns:
        //     The stride width, in bytes, of the System.Drawing.Bitmap object.
        public int Stride { get; set; }
        //
        // Summary:
        //     Gets or sets the format of the pixel information in the System.Drawing.Bitmap
        //     object that returned this System.Drawing.Imaging.BitmapData object.
        //
        // Returns:
        //     A System.Drawing.Imaging.PixelFormat that specifies the format of the pixel information
        //     in the associated System.Drawing.Bitmap object.
        public PixelFormat PixelFormat { get; set; }
        //
        // Summary:
        //     Gets or sets the address of the first pixel data in the bitmap. This can also
        //     be thought of as the first scan line in the bitmap.
        //
        // Returns:
        //     The address of the first pixel data in the bitmap.
        public IntPtr Scan0 { get; set; }
        //
        // Summary:
        //     Reserved. Do not use.
        //
        // Returns:
        //     Reserved. Do not use.
        public int Reserved { get; set; }
    }

    public class Icon : IDisposable
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public string FileName { get; set; }
        public void Dispose()
        {
        }
    }

    public class ResourceManager
    {
        private string v;

        public ResourceManager(string v)
        {
            this.v = v;
            Init();
        }
        public void Init()
        { 
            string xmlFilename = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "UwpRenderEngine\\Resources\\Resources.resx");
            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.Load(xmlFilename);
            LoadResources(doc);
        }
        Dictionary<string, string> resourceMap = new Dictionary<string, string>();

        private void LoadResources(Windows.Data.Xml.Dom.XmlDocument xml)
        {
            XmlNode nodes = xml.GetChildByName("root");

            foreach (XmlNode item in nodes.ChildNodes)
            {
                

                if ((string)item.LocalName == "data")
                {
                    string name = item.Attributes["name"].InnerText;
                    XmlNode valChild = item["value"];

                    string val = valChild.InnerText;

                    resourceMap[name] = val;
                }
            }

            int count = resourceMap.Count;
        }

        public object GetObject(string name, System.Globalization.CultureInfo culture)
        {
            if (resourceMap.ContainsKey(name))
            {
                string val = resourceMap[name];
                if (val.Contains("System.Drawing.Bitmap"))
                {
                    string path = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "UwpRenderEngine");
                    string[] parts = val.Split(new char[] { ';' });
                    return new Bitmap(parts[0].Replace("..", path));
                }
                return val;
            }
            else
            {
                return null;
            }
        }

        public string GetString(string name, CultureInfo resourceCulture)
        {
            if (resourceMap.ContainsKey(name))
            {
                string val = resourceMap[name];

                return val;
            }
            else
            {
                return null;
            }
        }
    }
}
