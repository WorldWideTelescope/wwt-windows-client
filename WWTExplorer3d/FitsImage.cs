using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using WwtDataUtils;
using System.IO.Compression;



namespace TerraViewer
{
    public enum ScaleTypes { Linear, Log, Power, SquareRoot, HistogramEqualization };
    public enum DataTypes { Byte, Int16, Int32, Float, Double, None };
    public class FitsImage : WcsImage
    {
        Dictionary<String, String> header = new Dictionary<string, string>();

        public FitsImage(string file)
        {
            this.Filename = file;
            Stream stream = File.OpenRead(file);

            bool gZip = IsGzip(stream);
            stream.Seek(0, SeekOrigin.Begin);
            if (gZip)
            {
                stream = new GZipStream(stream, CompressionMode.Decompress);
            }


            ParseHeader(stream);
        }
        public int[] Histogram;
        public int HistogramMaxCount;
        public int Width = 0;
        public int Height = 0;
        public int NumAxis = 0;
        public double BZero = 0;
        public int[] AxisSize;
        public object DataBuffer;
        public DataTypes DataType = DataTypes.None;
        public bool ContainsBlanks = false;
        public double BlankValue = double.MinValue;
        public double MaxVal = int.MinValue;
        public double MinVal = int.MaxValue;

        public int lastMin = 0;
        public int lastMax = 255;
        bool color = false;

        public static bool IsGzip(Stream stream)
        {
            BinaryReader br = new BinaryReader(stream);
            byte[] line = br.ReadBytes(2);

            if (line[0] == 31 && line[1] == 139)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ParseHeader(Stream stream)
        {

            BinaryReader br = new BinaryReader(stream);
            bool foundEnd = false;



            while (!foundEnd)
            {
                for (int i = 0; i < 36; i++)
                {
                    byte[] line = br.ReadBytes(80);




                    if (!foundEnd)
                    {
                        string data = new string(Encoding.UTF8.GetChars(line));

                        //string data = new string(line);
                        string keyword = data.Substring(0, 8).TrimEnd();
                        string[] values = data.Substring(10).Split(new char[] { '/' });
                        if (keyword.ToUpper() == "END")
                        {
                            foundEnd = true;
                            // Check for XTENSION
                            i++;
                            line = br.ReadBytes(80);
                            data = new string(Encoding.UTF8.GetChars(line));
                            while (String.IsNullOrWhiteSpace(data))
                            {
                                i++;
                                line = br.ReadBytes(80);
                                data = new string(Encoding.UTF8.GetChars(line));
                            }
                            keyword = data.Substring(0, 8).TrimEnd();
                            values = data.Substring(10).Split(new char[] { '/' });
                            if (keyword.ToUpper() == "XTENSION")
                            {
                                // We have additional headers
                                foundEnd = false;
                            }
                            else
                            {
                                // Rewind these 80 bytes which could be data
                                br.BaseStream.Seek(-80, SeekOrigin.Current);
                            }
                        }
                        else
                        {
                            AddKeyword(keyword, values);
                        }
                    }

                }
            }
            

            NumAxis = Convert.ToInt32(header["NAXIS"]);

            ContainsBlanks = header.ContainsKey("BLANK");

            if (ContainsBlanks)
            {
                BlankValue = Convert.ToDouble(header["BLANK"]);
            }

            if (header.ContainsKey("BZERO"))
            {
                BZero = Convert.ToDouble(header["BZERO"]);
            }

            AxisSize = new int[NumAxis];

            for (int axis = 0; axis < NumAxis; axis++)
            {
                AxisSize[axis] = Convert.ToInt32(header[string.Format("NAXIS{0}", axis + 1)]);
                BufferSize *= AxisSize[axis];
            }

            int bitsPix = Convert.ToInt32(header["BITPIX"]);


            switch (bitsPix)
            {
                case 8:
                    this.DataType = DataTypes.Byte;
                    InitDataBytes(br);
                    break;
                case 16:
                    this.DataType = DataTypes.Int16;
                    InitDataShort(br);
                    break;
                case 32:
                    this.DataType = DataTypes.Int32;
                    InitDataInt(br);
                    break;
                case -32:
                    this.DataType = DataTypes.Float;
                    InitDataFloat(br);
                    break;
                case -64:
                    this.DataType = DataTypes.Double;
                    InitDataDouble(br);
                    break;
                default:
                    this.DataType = DataTypes.None;
                    break;
            }

            if (NumAxis > 1)
            {
                if (NumAxis == 3)
                {
                    if (AxisSize[2] == 3)
                    {
                        color = true;
                    }
                }
                sizeX = Width = AxisSize[0];
                sizeY = Height = AxisSize[1];
                ComputeWcs();
                Histogram = ComputeHistogram(256, out this.HistogramMaxCount);
            }
        }



        private void AddKeyword(string keyword, string[] values)
        {
            if (keyword != "CONTINUE" && keyword != "COMMENT" && keyword != "HISTORY" && !String.IsNullOrEmpty(keyword))
            {
                try
                {
                    if (header.ContainsKey(keyword))
                    {
                        header[keyword] = values[0].Trim();
                    }
                    else
                    {
                        header.Add(keyword.ToUpper(), values[0].Trim());
                    }

                }
                catch
                {
                }
            }
        }

        private void ComputeWcs()
        {
            if (header.ContainsKey("CROTA2"))
            {
                rotation = Convert.ToDouble(header["CROTA2"].Trim());
                hasRotation = true;

            }

            if (header.ContainsKey("CDELT1"))
            {
                scaleX = Convert.ToDouble(header["CDELT1"].Trim());

                if (header.ContainsKey("CDELT2"))
                {
                    scaleY = Convert.ToDouble(header["CDELT2"].Trim());
                    hasScale = true;
                }
            }

            if (header.ContainsKey("CRPIX1"))
            {
                referenceX = Convert.ToDouble(header["CRPIX1"].Trim()) - 1;

                if (header.ContainsKey("CRPIX2"))
                {
                    referenceY = Convert.ToDouble(header["CRPIX2"].Trim()) - 1;
                    hasPixel = true;
                }
            }
            bool galactic = false;
            bool tan = false;

            if (header.ContainsKey("CTYPE1"))
            {
                if (header["CTYPE1"].Contains("GLON-"))
                {
                    galactic = true;
                    tan = true;
                }
                if (header["CTYPE2"].Contains("GLAT-"))
                {
                    galactic = true;
                    tan = true;
                }

                if (header["CTYPE1"].Contains("-TAN"))
                {
                    tan = true;
                }
                if (header["CTYPE1"].Contains("-SIN"))
                {
                    tan = true;
                }
            }

            if (!tan)
            {
                throw new System.Exception("Only TAN projected images are supported: ");
            }

            hasSize = true;

            if (header.ContainsKey("CRVAL1"))
            {
                centerX = Convert.ToDouble(header["CRVAL1"].Trim());

                if (header.ContainsKey("CRVAL2"))
                {
                    centerY = Convert.ToDouble(header["CRVAL2"].Trim());
                    hasLocation = true;
                }
            }

            if (galactic)
            {
                double[] result = Earth3d.GalactictoJ2000(centerX, centerY);
                centerX = result[0];
                centerY = result[1];
            }

            if (header.ContainsKey("CD1_1") && header.ContainsKey("CD1_2")
                && header.ContainsKey("CD2_1") && header.ContainsKey("CD2_2"))
            {
                cd1_1 = Convert.ToDouble(header["CD1_1"].Trim());
                cd1_2 = Convert.ToDouble(header["CD1_2"].Trim());
                cd2_1 = Convert.ToDouble(header["CD2_1"].Trim());
                cd2_2 = Convert.ToDouble(header["CD2_2"].Trim());
                if (!hasRotation)
                {
                    CalculateRotationFromCD();
                }
                if (!hasScale)
                {
                    CalculateScaleFromCD();
                }
                hasScale = true;
                hasRotation = true;
            }


            ValidWcs = hasScale && hasRotation && hasPixel && hasLocation;
        }

        public Bitmap GetHistogramBitmap(int max)
        {
            Bitmap bmp = new Bitmap(Histogram.Length, 150);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.FromArgb(68, 82, 105));
            Pen pen = new Pen(Color.FromArgb(127, 137, 157));
            double logMax = Math.Log(HistogramMaxCount);
            for (int i = 0; i < Histogram.Length; i++)
            {
                double height = Math.Log(Histogram[i]) / logMax;
                if (height < 0)
                {
                    height = 0;
                }


                g.DrawLine(Pens.White, new Point(i, 150), new Point(i, (int)(150 - (height * 150))));
            }
            pen.Dispose();
            g.Flush();
            g.Dispose();

            return bmp;
        }

        public int[] ComputeHistogram(int count, out int maxCount)
        {
            int[] histogram = new int[count];

            switch (DataType)
            {
                case DataTypes.Byte:
                    ComputeHistogramByte(histogram);
                    break;
                case DataTypes.Int16:
                    ComputeHistogramInt16(histogram);
                    break;
                case DataTypes.Int32:
                    ComputeHistogramInt32(histogram);
                    break;
                case DataTypes.Float:
                    ComputeHistogramFloat(histogram);
                    break;
                case DataTypes.Double:
                    ComputeHistogramDouble(histogram);
                    break;
                case DataTypes.None:
                default:
                    break;
            }
            int maxCounter = 1;
            foreach (int val in histogram)
            {
                if (val > maxCounter)
                {
                    maxCounter = val;
                }
            }
            maxCount = maxCounter;
            return histogram;
        }

        private void ComputeHistogramDouble(int[] histogram)
        {
            int buckets = histogram.Length;
            double[] buf = (double[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (double val in buf)
            {
                if (!double.IsNaN(val))
                {
                    histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
                }
            }
        }

        private void ComputeHistogramFloat(int[] histogram)
        {
            int buckets = histogram.Length;
            float[] buf = (float[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (float val in buf)
            {
                if (!float.IsNaN(val))
                {
                    histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
                }
            }
        }

        private void ComputeHistogramInt32(int[] histogram)
        {
            int buckets = histogram.Length;
            Int32[] buf = (Int32[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Int32 val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }



        private void ComputeHistogramInt16(int[] histogram)
        {
            int buckets = histogram.Length;
            short[] buf = (short[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Int16 val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }

        private void ComputeHistogramByte(int[] histogram)
        {
            int buckets = histogram.Length;
            Byte[] buf = (Byte[])DataBuffer;
            double factor = (MaxVal - MinVal) / buckets;

            foreach (Byte val in buf)
            {
                histogram[Math.Min(buckets - 1, (int)((val - MinVal) / factor))]++;
            }
        }


        int BufferSize = 1;

        private void InitDataBytes(BinaryReader br)
        {
            byte[] buffer = new byte[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = br.ReadByte();
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataShort(BinaryReader br)
        {
            short[] buffer = new Int16[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (short)((br.ReadSByte() * 256) + (short)br.ReadByte());
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataUnsignedShort(BinaryReader br)
        {
            int[] buffer = new int[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (int)((((short)br.ReadSByte() * 256) + (short)br.ReadByte()) + 32768);
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataInt(BinaryReader br)
        {
            int[] buffer = new int[BufferSize];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                buffer[i] = (br.ReadSByte() << 24) + (br.ReadSByte() << 16) + (br.ReadSByte() << 8) + br.ReadByte();
                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataFloat(BinaryReader br)
        {
            float[] buffer = new float[BufferSize];
            DataBuffer = buffer;
            Byte[] part = new Byte[4];
            for (int i = 0; i < BufferSize; i++)
            {
                part[3] = br.ReadByte();
                part[2] = br.ReadByte();
                part[1] = br.ReadByte();
                part[0] = br.ReadByte();

                buffer[i] = System.BitConverter.ToSingle(part, 0);

                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }

        private void InitDataDouble(BinaryReader br)
        {
            double[] buffer = new double[BufferSize];
            Byte[] part = new Byte[8];
            DataBuffer = buffer;
            for (int i = 0; i < BufferSize; i++)
            {
                part[7] = br.ReadByte();
                part[6] = br.ReadByte();
                part[5] = br.ReadByte();
                part[4] = br.ReadByte();
                part[3] = br.ReadByte();
                part[2] = br.ReadByte();
                part[1] = br.ReadByte();
                part[0] = br.ReadByte();
                buffer[i] = System.BitConverter.ToDouble(part, 0);

                if (MinVal > (double)buffer[i])
                {
                    MinVal = (double)buffer[i];
                }
                if (MaxVal < (double)buffer[i])
                {
                    MaxVal = (double)buffer[i];
                }
            }
        }
        public ScaleTypes lastScale = ScaleTypes.Linear;
        public double lastBitmapMin = 0;
        public double lastBitmapMax = 0;
        override public Bitmap GetBitmap()
        {
            if (lastBitmapMax == 0 && lastBitmapMin == 0)
            {
                lastBitmapMin = MinVal;
                lastBitmapMax = MaxVal;
            }


            return GetBitmap(lastBitmapMin, lastBitmapMax, lastScale);
        }

        public Bitmap GetBitmap(double min, double max, ScaleTypes scaleType)
        {
            ScaleMap scale;
            lastScale = scaleType;
            lastBitmapMin = min;
            lastBitmapMax = max;

            switch (scaleType)
            {
                default:
                case ScaleTypes.Linear:
                    scale = new ScaleLinear(min, max);
                    break;
                case ScaleTypes.Log:
                    scale = new ScaleLog(min, max);
                    break;
                case ScaleTypes.Power:
                    scale = new ScalePow(min, max);
                    break;
                case ScaleTypes.SquareRoot:
                    scale = new ScaleSqrt(min, max);
                    break;
                case ScaleTypes.HistogramEqualization:
                    scale = new HistogramEqualization(this, min, max);
                    break;
            }

            try
            {
                switch (DataType)
                {
                    case DataTypes.Byte:
                        return GetBitmapByte(min, max, scale);
                    case DataTypes.Int16:
                        return GetBitmapShort(min, max, scale);
                    case DataTypes.Int32:
                        return GetBitmapInt(min, max, scale);
                    case DataTypes.Float:
                        return GetBitmapFloat(min, max, scale);
                    case DataTypes.Double:
                        return GetBitmapDouble(min, max, scale);
                    case DataTypes.None:
                    default:
                        return new Bitmap(100, 100);
                }
            }
            catch
            {
                return new Bitmap(10, 10);
            }
        }

        private Bitmap GetBitmapByte(double min, double max, ScaleMap scale)
        {
            byte[] buf = (byte[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = new Bitmap(AxisSize[0], AxisSize[1]);
            FastBitmap fastBmp = new FastBitmap(bmp);

            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < AxisSize[1]; y++)
                {
                    int indexY = ((AxisSize[1] - 1) - y);
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < AxisSize[0]; x++)
                    {
                        if (color)
                        {
                            int datR = buf[(x + indexY * stride)];
                            int datG = buf[(x + indexY * stride) + page];
                            int datB = buf[(x + indexY * stride) + page * 2];
                            if (ContainsBlanks && (double)datR == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                int r = scale.Map(datR);
                                int g = scale.Map(datG);
                                int b = scale.Map(datB);
                                *pData++ = new PixelData(r, g, b, 255);
                            }
                        }
                        else
                        {
                            int dataValue = buf[x + indexY * stride];
                            if (ContainsBlanks && (double)dataValue == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                Byte val = scale.Map(dataValue);

                                *pData++ = new PixelData(val, val, val, 255);
                            }
                        }
                    }
                }
            }
            fastBmp.UnlockBitmap();
            return bmp;
        }

        private Bitmap GetBitmapDouble(double min, double max, ScaleMap scale)
        {
            double[] buf = (double[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = new Bitmap(AxisSize[0], AxisSize[1]);
            FastBitmap fastBmp = new FastBitmap(bmp);

            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < AxisSize[1]; y++)
                {
                    int indexY = ((AxisSize[1] - 1) - y);
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < AxisSize[0]; x++)
                    {
                        if (color)
                        {
                            double datR = buf[(x + indexY * stride)];
                            double datG = buf[(x + indexY * stride) + page];
                            double datB = buf[(x + indexY * stride) + page * 2];
                            if (ContainsBlanks && (double)datR == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                int r = scale.Map(datR);
                                int g = scale.Map(datG);
                                int b = scale.Map(datB);
                                *pData++ = new PixelData(r, g, b, 255);
                            }
                        }
                        else
                        {
                            double dataValue = buf[x + indexY * stride];
                            if (ContainsBlanks && (double)dataValue == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                Byte val = scale.Map(dataValue);
                                *pData++ = new PixelData(val, val, val, 255);
                            }
                        }
                    }
                }
            }
            fastBmp.UnlockBitmap();
            return bmp;
        }

        private Bitmap GetBitmapFloat(double min, double max, ScaleMap scale)
        {
            float[] buf = (float[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = new Bitmap(AxisSize[0], AxisSize[1]);
            FastBitmap fastBmp = new FastBitmap(bmp);

            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < AxisSize[1]; y++)
                {
                    int indexY = ((AxisSize[1] - 1) - y);
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < AxisSize[0]; x++)
                    {
                        if (color)
                        {
                            double datR = buf[(x + indexY * stride)];
                            double datG = buf[(x + indexY * stride) + page];
                            double datB = buf[(x + indexY * stride) + page * 2];
                            if (ContainsBlanks && (double)datR == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                int r = scale.Map(datR);
                                int g = scale.Map(datG);
                                int b = scale.Map(datB);
                                *pData++ = new PixelData(r, g, b, 255);
                            }
                        }
                        else
                        {
                            double dataValue = buf[x + indexY * stride];
                            if (ContainsBlanks && (double)dataValue == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                Byte val = scale.Map(dataValue);
                                *pData++ = new PixelData(val, val, val, 255);
                            }
                        }
                    }
                }
            }
            fastBmp.UnlockBitmap();
            return bmp;
        }

        private Bitmap GetBitmapInt(double min, double max, ScaleMap scale)
        {
            int[] buf = (int[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = new Bitmap(AxisSize[0], AxisSize[1]);
            FastBitmap fastBmp = new FastBitmap(bmp);

            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < AxisSize[1]; y++)
                {
                    int indexY = ((AxisSize[1] - 1) - y);
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < AxisSize[0]; x++)
                    {
                        if (color)
                        {
                            int datR = buf[(x + indexY * stride)];
                            int datG = buf[(x + indexY * stride) + page];
                            int datB = buf[(x + indexY * stride) + page * 2];
                            if (ContainsBlanks && (double)datR == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                int r = scale.Map(datR);
                                int g = scale.Map(datG);
                                int b = scale.Map(datB);
                                *pData++ = new PixelData(r, g, b, 255);
                            }
                        }
                        else
                        {
                            int dataValue = buf[x + indexY * stride];
                            if (ContainsBlanks && (double)dataValue == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                Byte val = scale.Map(dataValue);
                                *pData++ = new PixelData(val, val, val, 255);
                            }
                        }
                    }
                }
            }
            fastBmp.UnlockBitmap();

            return bmp;
        }
        public Bitmap GetBitmapShort(double min, double max, ScaleMap scale)
        {

            short[] buf = (short[])DataBuffer;
            double factor = max - min;
            int stride = AxisSize[0];
            int page = AxisSize[0] * AxisSize[1];
            Bitmap bmp = new Bitmap(AxisSize[0], AxisSize[1]);
            FastBitmap fastBmp = new FastBitmap(bmp);

            fastBmp.LockBitmap();
            unsafe
            {
                for (int y = 0; y < AxisSize[1]; y++)
                {
                    int indexY = ((AxisSize[1] - 1) - y);
                    PixelData* pData = fastBmp[0, y];
                    for (int x = 0; x < AxisSize[0]; x++)
                    {
                        if (color)
                        {
                            int datR = buf[(x + indexY * stride)];
                            int datG = buf[(x + indexY * stride) + page];
                            int datB = buf[(x + indexY * stride) + page * 2];
                            if (ContainsBlanks && (double)datR == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                int r = scale.Map(datR);
                                int g = scale.Map(datG);
                                int b = scale.Map(datB);
                                *pData++ = new PixelData(r, g, b, 255);
                            }
                        }
                        else
                        {
                            int dataValue = buf[x + indexY * stride];
                            if (ContainsBlanks && (double)dataValue == BlankValue)
                            {
                                *pData++ = new PixelData(0, 0, 0, 0);
                            }
                            else
                            {
                                Byte val = scale.Map(dataValue);
                                *pData++ = new PixelData(val, val, val, 255);
                            }
                        }

                    }
                }
            }
            fastBmp.UnlockBitmap();
            return bmp;
        }
    }


    public abstract class ScaleMap
    {
        public abstract byte Map(double val);
    }

    public class ScaleLinear : ScaleMap
    {
        double min;
        double max;
        double factor;
        double logFactor;
        public ScaleLinear(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)(val - min) / factor * 255)));
        }
    }

    public class ScaleLog : ScaleMap
    {
        double min;
        double max;
        double factor;
        double logFactor;
        public ScaleLog(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            logFactor = 255 / Math.Log(255);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Log((val - min) / factor * 255) * logFactor)));
        }
    }

    public class ScalePow : ScaleMap
    {
        double min;
        double max;
        double factor;
        double powFactor;
        public ScalePow(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            powFactor = 255 / Math.Pow(255, 2);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Pow((val - min) / factor * 255, 2) * powFactor)));
        }
    }

    public class ScaleSqrt : ScaleMap
    {
        double min;
        double max;
        double factor;
        double sqrtFactor;
        public ScaleSqrt(double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            sqrtFactor = 255 / Math.Sqrt(255);
        }

        public override byte Map(double val)
        {
            return (Byte)Math.Min(255, Math.Max(0, (int)((double)Math.Sqrt((val - min) / factor * 255) * sqrtFactor)));
        }
    }

    public class HistogramEqualization : ScaleMap
    {
        double min;
        double max;
        double factor;
        int[] Histogram;
        int maxHistogramValue = 1;
        Byte[] lookup;
        const int buckets = 10000;
        public HistogramEqualization(FitsImage image, double min, double max)
        {
            this.min = min;
            this.max = max;
            factor = max - min;
            Histogram = image.ComputeHistogram(buckets, out maxHistogramValue);
            lookup = new Byte[buckets];
            int totalCounts = image.Width * image.Height;
            int sum = 0;
            for (int i = 0; i < buckets; i++)
            {
                sum += Histogram[i];
                lookup[i] = (Byte)(Math.Min(255, ((sum * 255.0)) / totalCounts) + .5);
            }
        }

        public override byte Map(double val)
        {
            return (Byte)lookup[Math.Min(buckets - 1, Math.Max(0, (int)((double)(val - min) / factor * (buckets - 1.0))))];
        }
    }
}
