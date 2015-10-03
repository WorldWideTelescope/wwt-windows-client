using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Text.RegularExpressions;


namespace ShapefileTools
{
    public class ShapeFile
    {
        private FileHeader fileHeader;

        public FileHeader FileHeader
        {
            get { return fileHeader; }
            set { fileHeader = value; }
        }
        private string fName;

        public string FileName
        {
            get { return fName; }
            set { fName = value; }
        }
        internal Byte[] data;
        /// <summary>
        /// Reads the .shp and .dbf file. Both files have to be in the same folder. 
        /// </summary>
        /// <param name="fileName">Full path for the shapefile.</param>
        public ShapeFile(string fileName) {
            fileHeader = new FileHeader();
            fName = fileName;
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Shapefile cannot be found. Make sure the file exists, path is correct and you have access rights to the file.", fileName);
            }
            try
            {
                
                var fs = new FileStream(fileName, FileMode.Open);
                var fileLength = fs.Length;
                data = new Byte[fileLength];
                fs.Read(data, 0, (int)fileLength);
                fs.Close();
            }
            catch (Exception) {
                throw;
            }
        }
        public List<Shape> Shapes;
        public enum ShapeType
        {
            NullShape = 0,
            Point = 1,
            PolyLine = 3,
            Polygon = 5,
            MultiPoint = 8,
            PointZ = 11,
            PolyLineZ = 13,
            PolygonZ = 15,
            MultiPointZ = 18,
            PointM = 21,
            PolyLineM = 23,
            PolygonM = 25,
            MultiPointM = 28,
            MultiPatch = 31
        }

        public enum MultiPatchPartType {

            TriangleStrip = 0,
            TriangleFan = 1,
            OuterRing = 2,
            InnerRing = 3,
            FirstRing = 4,
            SubsequentRing = 5
        }


        public FileHeader Header
        {
            get { return this.fileHeader; }
        }

        // The integers that make up the data description fields in the
        // file header and record contents in the main file are in little endian
        // byte order.
        private int ReadIntLittleEndian(byte[] dataStream, int streamPosition)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                bytes[i] = dataStream[streamPosition + i];
            }
            return BitConverter.ToInt32(bytes, 0);
        }
        
        // The double-precision integers that make up the data description fields in the
        // file header and record contents in the main file are in little endian
        // byte order.
        private double ReadDoubleLittleEndian(byte[] dataStream, int streamPosition)
        {
            var bytes = new byte[8];
            for (var i = 0; i < 8; i++)
            {
                bytes[i] = dataStream[streamPosition + i];
            }
            return BitConverter.ToDouble(bytes, 0);
        }


        // The integers and double-precision floating point numbers that make
        // up the rest of the file and file management are in big endian byte
        // order.
        private int ReadIntBigEndian(byte[] dataStream, int streamPosition)
        {
            var bytes = new byte[4];
            for (var i = 0; i < 4; i++)
            {
                bytes[i] = dataStream[streamPosition + i];
            }
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        public enum Projections { Geo, PolarStereo };

        public Projections Projection = Projections.Geo;

        public void Read()
        {
            this.fileHeader = this.ReadFileHeader();

            // Spatial reference system parameters are stored in a file with the same name but a different (.prj) extension. 
            var prjFile = fName.Replace(".shp", ".prj");
            prjFile = prjFile.Replace(".SHP", ".PRJ");
            this.ReadSRSInfo(prjFile);

            // Read geometry
            Shapes = new List<Shape>();
            this.ReadShapes();

            // dBase file has the same name but a different (.dbf) extension. 
            var dbaseFile = fName.Replace(".shp", ".dbf");
            dbaseFile = dbaseFile.Replace(".SHP", ".DBF");
            this.ReadDBaseFile(dbaseFile);

            if (fileHeader.ProjectionInfo != null)
            {
                if (fileHeader.ProjectionInfo.Name.ToLower().Contains("stereographic"))
                {
                    if (fileHeader.ProjectionInfo.Name.ToLower().Contains("south"))
                    {
                        Projection = Projections.PolarStereo;
                    }
                    else
                    {
                        Projection = Projections.PolarStereo;
                    }
                }
            }

        }


        /// <summary>
        /// If a projected coordinate system, the format is:
        /// PROJCS["name",GEOGCS["name",DATUM["name", SPHEROID["name",axis,1/f]],PRIMEM["name",longitude],
        /// UNIT["name",conversion factor]],PROJECTION["name"],
        /// PARAMETER["name",value],UNIT["name",conversion factor]]
        /// The format of a geographic coordinate system is:
        /// GEOGCS["name",DATUM["name",SPHEROID["name",axis,1/f]], PRIMEM["name",longitude], UNIT["name",conversion factor]]
        /// </summary>
        /// <param name="prjFile">Full path for the prj file</param>
        private void ReadSRSInfo(string prjFile) {

            if (string.IsNullOrEmpty(prjFile))
                throw new ArgumentNullException("prjFile");

            // Prj file is optional so ignore if it doesn't exist
            if (!File.Exists(prjFile)) {
                return;
            }
             var gcs = new GeographicCoordinateSystem();
             var prj = new Projection();

             System.IO.TextReader tr = new StreamReader(prjFile);
             var prjContent = tr.ReadLine();

            var projected = new Regex("(?:PROJCS\\[\")(?<PRJName>.*)(?:\",GEOGCS\\[\")(?<CRSName>.*"+
      ")(?:\",DATUM\\[\")(?<DatumName>.*)(?:\",SPHEROID\\[\")(?<Sph"+
      "eroidName>.*)(?:\",)(?<InverseFlatteningRatio>.*)(?:,)(?<Ax"+
      "is>.*)(?:\\]\\],PRIMEM\\[\")(?<PMName>.*)(?:\",)(?<Longitude"+
      ">.*)(?:\\],UNIT\\[\")(?<UnitName>.*)(?:\",)(?<Conversion>.*)"+
      "(?:\\]\\],PROJECTION\\[\")(?<ProjectionName>.*)(?:\"\\],)(?<"+
      "Parameters>.*)(?:,UNIT\\[\")(?<ProjectionUnit>.*)(?:\",)(?<P"+
      "rjUnitConversion>.*)(?:\\]\\])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            var geo = new Regex("(?:GEOGCS\\[\")(?<CRSName>.*"+
      ")(?:\",DATUM\\[\")(?<DatumName>.*)(?:\",SPHEROID\\[\")(?<Sph"+
      "eroidName>.*)(?:\",)(?<InverseFlatteningRatio>.*)(?:,)(?<Ax"+
      "is>.*)(?:\\]\\],PRIMEM\\[\")(?<PMName>.*)(?:\",)(?<Longitude"+
      ">.*)(?:\\],UNIT\\[\")(?<UnitName>.*)(?:\",)(?<Conversion>.*)"+
      "(?:\\]\\])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
             if (projected.IsMatch(prjContent))
                            {

                                var m = projected.Match(prjContent);
                                gcs.Name = m.Groups["CRSName"].Value;
                                var rd = new RefDatum();
                                rd.Name = m.Groups["DatumName"].Value;
                                gcs.Datum = rd;
                                var rs = new RefSpheroid();
                                rs.Name = m.Groups["SpheroidName"].Value;
                                rs.InverseFlatteningRatio = double.Parse(m.Groups["InverseFlatteningRatio"].Value);
                                rs.Axis = double.Parse(m.Groups["Axis"].Value);
                                rd.Spheroid = rs;
                                var pm = new PriMem();
                                pm.Name = m.Groups["PMName"].Value;
                                pm.Longitude = double.Parse(m.Groups["Longitude"].Value);
                                gcs.PrimeMeridian = pm;
                                var cunit = new Unit();
                                cunit.Name = m.Groups["UnitName"].Value;
                                cunit.ConversionFactor = double.Parse(m.Groups["Conversion"].Value);
                                gcs.Units = cunit;

                                prj.Name = m.Groups["ProjectionName"].Value;
                                var punits = new Unit();
                                punits.Name = m.Groups["ProjectionUnit"].Value;
                                punits.ConversionFactor = double.Parse(m.Groups["PrjUnitConversion"].Value);
                                prj.Units = punits;
                                var prjParamsArray = m.Groups["Parameters"].Value.Split(new Char [] {','});
                                var  prjParams = new List<ProjectionParameter>();
                                 for (var i = 0; i < prjParamsArray.Length-1; i++)
                                 {
                                     prjParams.Add(new ProjectionParameter(prjParamsArray[i].Replace("PARAMETER[","").Replace("\"",""),double.Parse(prjParamsArray[i+1].Replace("]",""))));
                                     i++;
                                 }
                                prj.Parameters = prjParams;
                            } else if (geo.IsMatch(prjContent)){
                                var m = geo.Match(prjContent);
                                gcs.Name = m.Groups["CRSName"].Value;
                                var rd = new RefDatum();
                                rd.Name = m.Groups["DatumName"].Value;
                                gcs.Datum = rd;
                                var rs = new RefSpheroid();
                                rs.Name = m.Groups["SpheroidName"].Value;
                                rs.InverseFlatteningRatio = double.Parse(m.Groups["InverseFlatteningRatio"].Value);
                                rs.Axis = double.Parse(m.Groups["Axis"].Value);
                                var pm = new PriMem();
                                pm.Name = m.Groups["PMName"].Value;
                                pm.Longitude = double.Parse(m.Groups["Longitude"].Value);
                                gcs.PrimeMeridian = pm;
                                var cunit = new Unit();
                                cunit.Name = m.Groups["UnitName"].Value;
                                cunit.ConversionFactor = double.Parse(m.Groups["Conversion"].Value);
                                gcs.Units = cunit;


                                prj.Name = "Not projected";
                                var punits = new Unit();
                                punits.Name = "N/A";
                                punits.ConversionFactor = 0;
                                prj.Units = punits;
                                var  prjParams = new List<ProjectionParameter>();
                                prj.Parameters = prjParams;
                            }

             tr.Close();

             this.fileHeader.ProjectionInfo = prj;
             this.fileHeader.CoordinateReferenceSystem = gcs;
        }




        private void ReadPoint(Byte[] dataStream, int streamPosition)
        {
            var p = new Point();
            p.X = ReadDoubleLittleEndian(dataStream,streamPosition);
            p.Y = ReadDoubleLittleEndian(dataStream, streamPosition+8);
            Shapes.Add(p);

        }

        private void ReadPointM(Byte[] dataStream, int streamPosition)
        {
            var p = new PointM();
            p.X = ReadDoubleLittleEndian(dataStream, streamPosition);
            p.Y = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            p.M = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            Shapes.Add(p);
        }

        private void ReadPointZ(Byte[] dataStream, int streamPosition)
        {
            var p = new PointZ();
            p.X = ReadDoubleLittleEndian(dataStream, streamPosition);
            p.Y = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            p.Z = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            p.M = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            Shapes.Add(p);
        }


        private void ReadMultipoint(Byte[] dataStream, int streamPosition)
        {
            var mp = new MultiPoint();
            mp.BoundingBox = new Double[4];
            mp.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            mp.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            mp.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 32);
            mp.Points = new Point[numberOfPoints];

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].X = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16));
                mp.Points[i].Y = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16) + 8);
            }
            Shapes.Add(mp);
        }

        private void ReadMultipointM(Byte[] dataStream, int streamPosition)
        {
            var mp = new MultiPointM();
            mp.BoundingBox = new Double[4];
            mp.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            mp.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            mp.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 32);
            mp.Points = new PointM[numberOfPoints];

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].X = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16));
                mp.Points[i].Y = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16) + 8);
            }

            streamPosition = 36 + numberOfPoints * 16;

            mp.MMin = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.MMax = ReadDoubleLittleEndian(dataStream, streamPosition + 8);

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].M = ReadDoubleLittleEndian(dataStream, streamPosition + 16 + (i * 8));
            }

            Shapes.Add(mp);
        }


        private void ReadMultipointZ(Byte[] dataStream, int streamPosition)
        {
            var mp = new MultiPointZ();
            mp.BoundingBox = new Double[4];
            mp.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            mp.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            mp.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 32);
            mp.Points = new PointZ[numberOfPoints];

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].X = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16));
                mp.Points[i].Y = ReadDoubleLittleEndian(dataStream, streamPosition + 36 + (i * 16) + 8);
            }

            streamPosition = 36 + numberOfPoints * 16;

            mp.ZMin = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.ZMax = ReadDoubleLittleEndian(dataStream, streamPosition + 8);

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].Z = ReadDoubleLittleEndian(dataStream, streamPosition + 16 + (i * 8));
            }
            streamPosition = 16 + numberOfPoints * 8;


            mp.MMin = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.MMax = ReadDoubleLittleEndian(dataStream, streamPosition + 8);

            for (var i = 0; i < numberOfPoints; i++)
            {
                mp.Points[i].M = ReadDoubleLittleEndian(dataStream, streamPosition + 16 + (i * 8));
            }
            Shapes.Add(mp);
        }

        private void ReadPolyLine(Byte[] dataStream, int streamPosition)
        {
            var pl = new PolyLine();
            pl.BoundingBox = new Double[4];
            pl.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pl.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pl.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pl.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pl.Lines = new Line[numberOfParts];
            var parts = new int [numberOfParts];
            var pos = streamPosition + 40;

            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i-1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) -1 ;
            }

            parts[numberOfParts - 1] = numberOfPoints - 1;
            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var ln = new Line();
            var pts = new List<Point> ();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of line, add the last point and wrap up
                    var p = new Point();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    ln.Points = pts.ToArray();
                    pts.Clear();
                    pl.Lines[z] = ln;
                    ln = new Line();
                   // if (z < numberOfParts - 1)
                    {
                        z++;
                    }
                }
                else
                {
                    // Keep adding the points
                    var p = new Point();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }

            Shapes.Add(pl);
        }

        private void ReadPolyLineM(Byte[] dataStream, int streamPosition)
        {
            var pl = new PolyLineM();
            pl.BoundingBox = new Double[4];
            pl.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pl.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pl.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pl.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pl.Lines = new LineM[numberOfParts];
            var parts = new int[numberOfParts];
            var pos = streamPosition + 40;

            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            parts[numberOfParts - 1] = numberOfPoints - 1;
            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var ln = new LineM();
            var pts = new List<PointM>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of line, add the last point and wrap up
                    var p = new PointM();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    ln.Points = pts.ToArray();
                    pts.Clear();
                    pl.Lines[z] = ln;
                    ln = new LineM();
                    if (z < numberOfParts - 1) z++;
                }
                else
                {
                    // Keep adding the points
                    var p = new PointM();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }
            pos = pos + numberOfPoints * 16;

            pl.MMin = ReadDoubleLittleEndian(dataStream, pos);
            pl.MMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            var pointIndex = 0;
            for (var i = 0; i < pl.Lines.Length; i++)
            {
                for (var k = 0; k < pl.Lines[i].Points.Length; k++) {
                    pl.Lines[i].Points[k].M = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            Shapes.Add(pl);
        }


        private void ReadPolyLineZ(Byte[] dataStream, int streamPosition)
        {
            var pl = new PolyLineZ();
            pl.BoundingBox = new Double[4];
            pl.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pl.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pl.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pl.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pl.Lines = new LineZ[numberOfParts];
            var parts = new int[numberOfParts];
            var pos = streamPosition + 40;

            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            parts[numberOfParts - 1] = numberOfPoints - 1;
            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var ln = new LineZ();
            var pts = new List<PointZ>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of line, add the last point and wrap up
                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    ln.Points = pts.ToArray();
                    pts.Clear();
                    pl.Lines[z] = ln;
                    ln = new LineZ();
                    if (z < numberOfParts - 1) z++;
                }
                else
                {
                    // Keep adding the points
                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }
            pos = pos + numberOfPoints * 16;

            pl.ZMin = ReadDoubleLittleEndian(dataStream, pos);
            pl.ZMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            var pointIndex = 0;
            for (var i = 0; i < pl.Lines.Length; i++)
            {
                for (var k = 0; k < pl.Lines[i].Points.Length; k++)
                {
                    pl.Lines[i].Points[k].Z = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }
            pos = pos + 16 + numberOfPoints * 8;

            pl.MMin = ReadDoubleLittleEndian(dataStream, pos);
            pl.MMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            pointIndex = 0;
            for (var i = 0; i < pl.Lines.Length; i++)
            {
                for (var k = 0; k < pl.Lines[i].Points.Length; k++)
                {
                    pl.Lines[i].Points[k].M = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            Shapes.Add(pl);
        }

        private void ReadPolygon(Byte[] dataStream, int streamPosition)
        {
            var pg = new Polygon();
            pg.BoundingBox = new Double[4];
            pg.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pg.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pg.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pg.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pg.Rings = new Ring[numberOfParts];
            var parts = new int[numberOfParts];
            var pos = streamPosition + 40;

            // Convert starting indices to ending indices for the geometry. Skip first record.
            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            // Add the final record as the end of last "part".
            parts[numberOfParts - 1] = numberOfPoints - 1;

            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var rng = new Ring();
            var pts = new List<Point>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of polygon. Add the last point and wrap up.
                    var p = new Point();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    rng.Points = pts.ToArray();
                    pts.Clear();
                    pg.Rings[z] = rng;
                    rng= new Ring();
                   if (z<numberOfParts-1) z++;
                }
                else
                {
                    var p = new Point();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }
            Shapes.Add(pg);
        }

        private void ReadPolygonM(Byte[] dataStream, int streamPosition)
        {
            var pg = new PolygonM();
            pg.BoundingBox = new Double[4];
            pg.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pg.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pg.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pg.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pg.Rings = new RingM[numberOfParts];
            var parts = new int[numberOfParts];
            var pos = streamPosition + 40;

            // Convert starting indices to ending indices for the geometry. Skip first record.
            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            // Add the final record as the end of last "part".
            parts[numberOfParts - 1] = numberOfPoints - 1;

            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var rng = new RingM();
            var pts = new List<PointM>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of polygon. Add the last point and wrap up.
                    var p = new PointM();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    rng.Points = pts.ToArray();
                    pts.Clear();
                    pg.Rings[z] = rng;
                    rng = new RingM();
                    if (z < numberOfParts - 1) z++;
                }
                else
                {
                    var p = new PointM();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }

            pos = pos + numberOfPoints * 16;

            pg.MMin = ReadDoubleLittleEndian(dataStream, pos);
            pg.MMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            var pointIndex = 0;
            for (var i = 0; i < pg.Rings.Length; i++)
            {
                for (var k = 0; k < pg.Rings[i].Points.Length; k++)
                {
                    pg.Rings[i].Points[k].M = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            Shapes.Add(pg);
        }


        private void ReadPolygonZ(Byte[] dataStream, int streamPosition)
        {
            var pg = new PolygonZ();
            pg.BoundingBox = new Double[4];
            pg.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            pg.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            pg.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            pg.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            pg.Rings = new RingZ[numberOfParts];
            var parts = new int[numberOfParts];
            var pos = streamPosition + 40;

            // Convert starting indices to ending indices for the geometry. Skip first record.
            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            // Add the final record as the end of last "part".
            parts[numberOfParts - 1] = numberOfPoints - 1;

            pos = pos + 4 * numberOfParts;
            var z = 0;
            int lineEndsAt;
            var rng = new RingZ();
            var pts = new List<PointZ>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    // End of polygon. Add the last point and wrap up.
                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                    rng.Points = pts.ToArray();
                    pts.Clear();
                    pg.Rings[z] = rng;
                    rng = new RingZ();
                    if (z < numberOfParts - 1) z++;
                }
                else
                {
                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }

            pos = pos + numberOfPoints * 16;

            pg.ZMin = ReadDoubleLittleEndian(dataStream, pos);
            pg.ZMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            var pointIndex = 0;
            for (var i = 0; i < pg.Rings.Length; i++)
            {
                for (var k = 0; k < pg.Rings[i].Points.Length; k++)
                {
                    pg.Rings[i].Points[k].Z = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            pos = pos + 16 + numberOfPoints * 8;

            pg.MMin = ReadDoubleLittleEndian(dataStream, pos);
            pg.MMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            pointIndex = 0;
            for (var i = 0; i < pg.Rings.Length; i++)
            {
                for (var k = 0; k < pg.Rings[i].Points.Length; k++)
                {
                    pg.Rings[i].Points[k].M = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            Shapes.Add(pg);
        }

        private MultiPatchElement CreateMultiPatchGeometry(int partType) {

            switch (partType)
            {
                case (int)MultiPatchPartType.TriangleFan:
                    return new TriangleFan();
                case (int)MultiPatchPartType.TriangleStrip:
                    return new TriangleStrip();
                case (int)MultiPatchPartType.OuterRing:
                    return new OuterRing();
                case (int)MultiPatchPartType.InnerRing:
                    return new InnerRing();
                case (int)MultiPatchPartType.FirstRing:
                    return new UndefinedRing();
                case (int)MultiPatchPartType.SubsequentRing:
                    return new UndefinedRing();
                default:
                    {
                        var msg = String.Format(System.Globalization.CultureInfo.InvariantCulture, "PartType {0} is not supported.", (MultiPatchPartType)partType);
                        throw new Exception(msg);
                    }
            }

        }



        private void ReadMultiPatch(Byte[] dataStream, int streamPosition)
        {
            var mp = new MultiPatch();
            mp.BoundingBox = new Double[4];
            mp.BoundingBox[0] = ReadDoubleLittleEndian(dataStream, streamPosition);
            mp.BoundingBox[1] = ReadDoubleLittleEndian(dataStream, streamPosition + 8);
            mp.BoundingBox[2] = ReadDoubleLittleEndian(dataStream, streamPosition + 16);
            mp.BoundingBox[3] = ReadDoubleLittleEndian(dataStream, streamPosition + 24);
            var numberOfParts = ReadIntLittleEndian(dataStream, streamPosition + 32);
            var numberOfPoints = ReadIntLittleEndian(dataStream, streamPosition + 36);
            mp.Parts = new MultiPatchElement[numberOfParts];
            var parts = new int[numberOfParts];
            var partTypes = new int[numberOfParts];
            var pos = streamPosition + 40;

            // Convert starting indices to ending indices for the geometry. Skip first record.
            for (var i = 1; i < numberOfParts; i++)
            {
                parts[i - 1] = ReadIntLittleEndian(dataStream, pos + (i * 4)) - 1;
            }

            // Add the final record as the end of last "part".
            parts[numberOfParts - 1] = numberOfPoints - 1;

            pos = pos + 4 * numberOfParts;

            // Convert starting indices to ending indices for the geometry. Skip first record.
            for (var i = 0; i < numberOfParts; i++)
            {
                partTypes[i] = ReadIntLittleEndian(dataStream, pos + (i * 4));
            }

            pos = pos + 4 * numberOfParts;

            var z = 0;
            int lineEndsAt;
            MultiPatchElement sh;
            var pts = new List<PointZ>();
            for (var i = 0; i < numberOfPoints; i++)
            {
                lineEndsAt = parts[z];
                if (i == lineEndsAt)
                {
                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    sh = CreateMultiPatchGeometry(partTypes[z]);
                    pts.Add(p);
                    sh.Points = pts.ToArray();
                    pts.Clear();
                    if (z < numberOfParts - 1) z++;
                    sh = CreateMultiPatchGeometry(partTypes[z]);
                }
                else
                {
           

                    var p = new PointZ();
                    p.X = ReadDoubleLittleEndian(dataStream, pos + (i * 16));
                    p.Y = ReadDoubleLittleEndian(dataStream, pos + (i * 16) + 8);
                    pts.Add(p);
                }

            }

            pos = pos + numberOfPoints * 16;

            mp.ZMin = ReadDoubleLittleEndian(dataStream, pos);
            mp.ZMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            var pointIndex = 0;
            for (var i = 0; i < mp.Parts.Length; i++)
            {
                for (var k = 0; k < mp.Parts[i].Points.Length; k++)
                {
                    mp.Parts[i].Points[k].Z = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            pos = pos + 16 + numberOfPoints * 8;

            mp.MMin = ReadDoubleLittleEndian(dataStream, pos);
            mp.MMax = ReadDoubleLittleEndian(dataStream, pos + 8);
            pointIndex = 0;
            for (var i = 0; i < mp.Parts.Length; i++)
            {
                for (var k = 0; k < mp.Parts[i].Points.Length; k++)
                {
                    mp.Parts[i].Points[k].M = ReadDoubleLittleEndian(dataStream, pos + 16 + (pointIndex * 8));
                    pointIndex++;
                }
            }

            Shapes.Add(mp);
        }


        private void ReadDBaseFile(string dbfFile)
        {
            if (string.IsNullOrEmpty(dbfFile))
                throw new ArgumentNullException("dbfFile");

            // dBase file is optional so ignore if it doesn't exist
            if (!File.Exists(dbfFile))
                return;

            var fInfo = new FileInfo(dbfFile);
            var dirName = fInfo.DirectoryName;
            var fName = fInfo.Name.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
            if (fName.EndsWith(".DBF"))
                fName = fName.Substring(0, fName.Length - 4);

            if (fName.Length > 8)
            {
                if (fName.Contains(" "))
                {
                    var noSpaces = fName.Replace(" ", "");
                    if (noSpaces.Length > 8)
                        fName = noSpaces;
                }
                fName = fName.Substring(0, 6) + "~1";
            }

            var reader = new DBFReader(dirName + @"\" + fName + ".dbf");

            Table = reader.Table;
            if (Table != null && Table.Rows.Count > 0)
            {
                this.AssociateAttributes(Table);
            }

            ////string connectionString = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=" + dirName + ";Extended Properties=dBASE 5.0";
            //string connectionString = "PROVIDER=Microsoft.ACE.OLEDB.12.0;Data Source=" + dirName + ";Extended Properties=dBASE 5.0";

            //string selectQuery = "SELECT * FROM [" + fName + "#DBF];";

            //OleDbConnection connection = new OleDbConnection(connectionString);

            //OleDbCommand command = new OleDbCommand(selectQuery, connection);

            //try
            //{         
            //    connection.Open();
            //    OleDbDataAdapter dataAdapter = new OleDbDataAdapter();
            //    dataAdapter.SelectCommand = command;
            //    DataSet dataSet = new DataSet();
            //    dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture;
            //    dataAdapter.Fill(dataSet);
            //    if (dataSet.Tables.Count > 0)
            //        this.AssociateAttributes(dataSet.Tables[0]);
            //}
            //catch (OleDbException)
            //{
            //    throw;
            //}
            //finally
            //{
            //    connection.Dispose();
            //}
        }
        public DataTable Table = null;
 
        private void AssociateAttributes(DataTable table)
        {
            Table = table;
            var index = 0;
            foreach (DataRow row in table.Rows)
            {
                if (index >= this.Shapes.Count)
                    break;
                this.Shapes[index].props = row;
                ++index;
            }
        }

        private void ReadShapes()
        {

            var data = this.data;

            // Skip to the end of file header
            var pos = 100;
            while (pos < (this.fileHeader.FileLength*2))
            {
                // Record header
                var recordNumber = ReadIntBigEndian(data, pos);
                var contentLength = ReadIntBigEndian(data, pos + 4);
                // Get shape type
                // Record headers have a fixed length of 8 bytes
                var shapeType = ReadIntLittleEndian(data, pos +  8);
                // plus 4 bytes for the shape type 
                pos = pos + 12;
                switch (shapeType)
                {
                    case (int)ShapeType.NullShape:
                        break;
                    case (int)ShapeType.Point:
                        ReadPoint(data, pos);
                        break;
                    case (int)ShapeType.PointM:
                        ReadPointM(data, pos);
                        break;
                    case (int)ShapeType.PointZ:
                        ReadPointZ(data, pos);
                        break;
                    case (int)ShapeType.MultiPoint:
                        ReadMultipoint(data, pos);
                        break;
                    case (int)ShapeType.MultiPointM:
                        ReadMultipointM(data, pos);
                        break;
                    case (int)ShapeType.MultiPointZ:
                        ReadMultipointZ(data, pos);
                        break;
                    case (int)ShapeType.PolyLine:
                        ReadPolyLine(data, pos);
                        break;
                    case (int)ShapeType.PolyLineM:
                        ReadPolyLineM(data, pos);
                        break;
                    case (int)ShapeType.PolyLineZ:
                        ReadPolyLineZ(data, pos);
                        break;
                    case (int)ShapeType.Polygon:
                        ReadPolygon(data, pos);
                        break;
                    case (int)ShapeType.PolygonM:
                        ReadPolygonM(data, pos);
                        break;
                    case (int)ShapeType.PolygonZ:
                        ReadPolygonZ(data, pos);
                        break;
                    case (int)ShapeType.MultiPatch:
                        ReadMultiPatch(data, pos);
                        break;
                    default:
                        {
                            var msg = String.Format(System.Globalization.CultureInfo.InvariantCulture, "ShapeType {0} is not supported.", (ShapeType)shapeType);
                            throw new Exception(msg);
                        }
                }
                // Shape type (Int32) is already included in the content length hence -2*2
                pos = pos + (contentLength-2) * 2;
            }
        }

        private FileHeader ReadFileHeader()
        {
            var data = this.data;
            var fh = new FileHeader();
            fh.FileCode = ReadIntBigEndian(data, 0);
            fh.FileLength = ReadIntBigEndian(data, 24);
            fh.Version = ReadIntLittleEndian(data, 28);
            fh.ShapeType = ReadIntLittleEndian(data, 32);
            fh.XMin = ReadDoubleLittleEndian(data, 36);
            fh.YMin = ReadDoubleLittleEndian(data, 44);
            fh.XMax = ReadDoubleLittleEndian(data, 52);
            fh.YMax = ReadDoubleLittleEndian(data, 60);
            # region optionalContent
            // Unused, with value 0.0, if not Measured or Z type 
            fh.ZMin = ReadDoubleLittleEndian(data, 68);
            fh.ZMax = ReadDoubleLittleEndian(data, 76);
            fh.MMin = ReadDoubleLittleEndian(data, 84);
            fh.MMax = ReadDoubleLittleEndian(data, 92);
            # endregion
            if (fh.FileCode != FileHeader.shapefileDefaultCode)
            {
                throw new Exception("Unrecognized file format.");
            }

            return fh;
        }


    }
    class DBFReader
    {
        public DataTable Table = new DataTable();
        uint recordCount;
        uint recordLength;
        uint headerSize;


        readonly List<DBFField> Fields = new List<DBFField>();

        public DBFReader(string filename)
        {

            Stream s = File.OpenRead(filename);
            var br = new BinaryReader(s, Encoding.ASCII);

            br.ReadBytes(4);
            recordCount = br.ReadUInt32();
            headerSize = br.ReadUInt16();
            recordLength = br.ReadUInt16();

            br.BaseStream.Seek(32, SeekOrigin.Begin);


            var columnName = "default";
            var columnType = typeof(string);

            while (true)
            {

                if (br.PeekChar() == 13)
                {
                    break;
                }
                var field = new DBFField();

                var name = br.ReadBytes(11);
                var nameLen = 0;
                while (name[nameLen++] != 0) ;


                columnName = Encoding.ASCII.GetString(name, 0, nameLen - 1);
                field.Name = columnName;

                var fieldType = br.ReadByte();
                field.Type = Encoding.ASCII.GetString(new byte[] { fieldType }, 0, 1);

                switch (fieldType)
                {
                    case 70: // F = Floating Point
                        columnType = typeof(double);
                        break;
                    case 67: // C = Character Array
                        columnType = typeof(string);
                        break;
                    case 78: // N = Number 
                        columnType = typeof(double);
                        break;
                    case 68: // D = Date
                        columnType = typeof(DateTime);
                        break;
                    case 76: // L = Logical
                        columnType = typeof(bool);
                        break;
                    case 79: // O = Double
                        columnType = typeof(double);
                        break;
                    case 64: // @ = TimeStamp
                        columnType = typeof(DateTime);
                        break;
                    default:
                        columnType = null;
                        break;

                }

                br.ReadBytes(4);
                field.Length = br.ReadByte();
                field.DecimalCount = br.ReadByte();
                br.ReadBytes(14);

                Table.Columns.Add(new DataColumn(columnName, columnType));
                Fields.Add(field);
            }
            br.ReadBytes(1);
            for (var i = 0; i < recordCount; i++)
            {
                var row = Table.NewRow();

                var bytes = br.ReadBytes((int)recordLength);
                var data = Encoding.ASCII.GetString(bytes, 0, (int)recordLength);
                if (data[0] != 32)
                {
                    continue;
                }

                var index = 1;
                foreach (var field in Fields)
                {
                    var fieldData = data.Substring(index, field.Length);
                    index += field.Length;
                    row[field.Name] = System.DBNull.Value;
                    switch (field.Type)
                    {
                        case "F": // F = Floating Point
                            row[field.Name] = double.Parse(fieldData);
                            break;
                        case "C": // C = Character Array
                            row[field.Name] = fieldData.TrimEnd();
                            break;
                        case "N": // N = Number 
                            row[field.Name] = double.Parse(fieldData.Trim());
                            break;
                        case "D": // D = Date
                            row[field.Name] = new DateTime(int.Parse(fieldData.Substring(0, 2)), int.Parse(fieldData.Substring(2, 2)), int.Parse(fieldData.Substring(4, 2)));
                            break;
                        case "L": // L = Logical
                            row[field.Name] = bool.Parse(fieldData);
                            break;
                        case "O": // O = Double
                            row[field.Name] = double.Parse(fieldData);
                            break;
                        case "@": // @ = TimeStamp
                            columnType = typeof(DateTime);
                            break;
                        default:
                            columnType = null;
                            break;

                    }

                }
                Table.Rows.Add(row);
            }
            br.Close();
            s.Close();
            s.Dispose();
        }
    }

    internal class DBFField
    {
        public string Name;
        public string Type;
        public int Length;
        public int DecimalCount;
    }
}
