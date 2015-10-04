using System;
using System.Collections.Generic;
using System.Text;
using System.IO;using System.IO.Compression;
using System.Threading;
namespace TerraViewer
{
    public class ZipArchive
    {
        internal Mutex fileMutex = new Mutex();
        internal Stream Stream;

        public List<ZipEntry> Files = new List<ZipEntry>();
        public ZipArchive(Stream stream)
        {
            Stream = stream;
            Extract();
        }


        internal void Extract()
        {
            fileMutex.WaitOne();

            var br = new BinaryReader(Stream);

            var fileLen = br.BaseStream.Length;
            long start = 0;
            if (fileLen > 1024)
            {
                start = fileLen - 1024;
            }

            var dirFound = false;

            while (!dirFound)
            {
                br.BaseStream.Seek(start, SeekOrigin.Begin);
                var sig = br.ReadUInt32();
                if (sig == 0x06054b50)
                {
                    dirFound = true;
                }
                else
                {
                    start += 1;
                }
            }

            int diskNum = br.ReadUInt16();
            int diskDir = br.ReadUInt16();
            int disckCount = br.ReadUInt16();
            int totalCount = br.ReadUInt16();
            var dirSize = br.ReadUInt32();
            long dirOffset = br.ReadUInt32();

            br.BaseStream.Seek(dirOffset, SeekOrigin.Begin);

            for (var i = 0; i < totalCount; i++)
            {
                var file = new ZipEntry();

                var sig = br.ReadUInt32();
                if (sig != 0x02014b50)
                {
                    break;
                }

                br.BaseStream.Seek(6, SeekOrigin.Current);
                file.CompressionMethod = br.ReadUInt16();
                var time = br.ReadInt16();
                var date = br.ReadInt16();
                file.LastModified = ZipModTimeToDateTime(time, date);
                var crc = br.ReadUInt32();
                file.CompressedLength = br.ReadInt32();
                file.Length = br.ReadInt32();
                var fileNameLength = br.ReadUInt16();
                var extraLength =  br.ReadUInt16();
                var commentLength =  br.ReadUInt16();
                var disk = br.ReadUInt16();
                var attributes = br.ReadUInt16();
                var exAttributes = br.ReadUInt32();
                file.FileHeaderOffset = br.ReadUInt32();
                var rawFilename = br.ReadBytes(fileNameLength);

                file.Filename = Encoding.UTF8.GetString(rawFilename, 0, rawFilename.Length);
                file.FileOffset = file.FileHeaderOffset + 30 + fileNameLength + extraLength;
                br.BaseStream.Seek(extraLength, SeekOrigin.Current);
                br.BaseStream.Seek(commentLength, SeekOrigin.Current);
                file.Owner = this;
                Files.Add(file);
            }

            
            fileMutex.ReleaseMutex();
        }
        public static DateTime ZipModTimeToDateTime(Int16 time, Int16 date)
        {

            var years = 0;
            var months = 1;
            var days = 1;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            days = (date & 0x1F);
            months = ((date & 0x01E0) >> 5);
            years = 1980 + ((date & 0xFE00) >> 9);
            seconds = (time & 0x1F) * 2;
            minutes = ((time & 0x07E0) >> 5);
            hours = ((time & 0x0F800) >> 11);
            return new DateTime(years, Math.Max(1,months), Math.Max(1,days), hours, minutes, seconds);
        } 

    }
    public class ZipEntry
    {
        public override string ToString()
        {
            return Filename;
        }
        public string Filename;
        public DateTime LastModified;
        public Int32 Length;
        public Int32 CompressedLength;
        public long FileHeaderOffset;
        public long FileOffset;
        public UInt16 CompressionMethod;
        public ZipArchive Owner = null;
        public Stream GetFileStream()
        {
            try
            {
                Owner.fileMutex.WaitOne();
                var br = new BinaryReader(Owner.Stream);
                br.BaseStream.Seek(FileHeaderOffset, SeekOrigin.Begin);
                var sig = br.ReadUInt32();

                if (sig != 0x04034b50)
                {
                    return null;
                }
                br.BaseStream.Seek(FileHeaderOffset + 26, SeekOrigin.Begin);
                var fileNameLength = br.ReadUInt16();
                br.BaseStream.Seek(FileHeaderOffset + 28, SeekOrigin.Begin);
                var extraLength = br.ReadUInt16();

                br.BaseStream.Seek(FileHeaderOffset + 30 + fileNameLength + extraLength, SeekOrigin.Begin);

                var inBuffer = br.ReadBytes(CompressedLength);

                var msInput = new MemoryStream(inBuffer);

                if (CompressionMethod == 8)
                {
                    var outBuffer = new byte[Length];

                    var msOutput = new MemoryStream(outBuffer);
                    var gzs = new DeflateStream(msInput, CompressionMode.Decompress);


                    var buffer = new byte[32768];
                    while (true)
                    {
                        var read = gzs.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                        {
                            break;
                        }
                        msOutput.Write(buffer, 0, read);
                    }

                    msOutput.Seek(0, SeekOrigin.Begin);
                    return msOutput;
                }
                else
                {
                    msInput.Seek(0, SeekOrigin.Begin);
                    return msInput;
                }
            }
            finally
            {
                Owner.fileMutex.ReleaseMutex();
            }
        }
    }
    public class ZipArchiveWriter
    {
    }
}
