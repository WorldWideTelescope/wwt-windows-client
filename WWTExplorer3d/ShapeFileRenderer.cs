using System;
using System.Collections.Generic;
using System.Text;
using ShapefileTools;

using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;
using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    public class ShapeFileRenderer : Layer
    {
        PositionVertexBuffer11 shapeFileVertex;
        IndexBuffer11 shapeFileIndex;
        bool isLongIndex;
        internal ShapeFile shapefile;
        int shapeVertexCount;
        int shapeIndexCount;
        bool lines = true;
        public ShapeFileRenderer()
        {
          
        }
        ShapeFileLayerUI primaryUI;
        public override LayerUI GetPrimaryUI()
        {
            if (primaryUI == null)
            {
                primaryUI = new ShapeFileLayerUI(this);
            }

            return primaryUI;
        }

        public ShapeFileRenderer(ShapeFile initFile)
        {
            shapefile = initFile;
        }

        public ShapeFileRenderer(string path)
        {
            shapefile = new ShapeFile(path);
            shapefile.Read();
        }

        public override void LoadData(string path)
        {
            shapefile = new ShapeFile(path.Replace(".txt",".shp"));
            shapefile.Read();
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            var fName = shapefile.FileName;

            var copy = !fName.Contains(ID.ToString());

            var fileName = fc.TempDirectory + string.Format("{0}\\{1}.shp", fc.PackageID, this.ID.ToString());
            var prjFileName = fc.TempDirectory + string.Format("{0}\\{1}.prj", fc.PackageID, this.ID.ToString());
            var dbFileName = fc.TempDirectory + string.Format("{0}\\{1}.dbf", fc.PackageID, this.ID.ToString());
     
            var prjFile = fName.ToUpper().Replace(".SHP", ".PRJ");
            var dbaseFile = fName.ToUpper().Replace(".SHP", ".DBF");

            if (copy)
            {
                if (File.Exists(fName))
                {
                    File.Copy(fName, fileName);
                }

                if (File.Exists(prjFile))
                {
                    File.Copy(prjFile, prjFileName);
                }

                if (File.Exists(dbaseFile))
                {
                    File.Copy(dbaseFile, dbFileName);
                }
                shapefile.FileName = fileName;
            }

            if (File.Exists(fileName))
            {
                fc.AddFile(fileName);
            }
            if (File.Exists(prjFileName))
            {
                fc.AddFile(prjFileName);
            }
            
            // dBase file has the same name but a different (.dbf) extension. 
            if (File.Exists(dbFileName))
            {
                fc.AddFile(dbFileName);
            }
        }

        public override void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            base.SaveToXml(xmlWriter);
        }

        public bool PointInboundingBox( Coordinates target, double[] bbox)
        {
            if (bbox[0] > bbox[2])
            {
                if (bbox[0] > target.Lng && bbox[1] < target.Lat && bbox[2] < target.Lng && bbox[3] > target.Lat)
                {
                    return true;
                }

                if (bbox[0] - 360 > target.Lng && bbox[1] < target.Lat && bbox[2] - 360 < target.Lng && bbox[3] > target.Lat)
                {
                    return true;
                }         
            }
            else
            {


                if (bbox[0] < target.Lng && bbox[1] < target.Lat && bbox[2] > target.Lng && bbox[3] > target.Lat)
                {
                    return true;
                }

                if (bbox[0] - 360 < target.Lng && bbox[1] < target.Lat && bbox[2] - 360 > target.Lng && bbox[3] > target.Lat)
                {
                    return true;
                }
            }
            return false;
        }
        public override IPlace FindClosest(Coordinates target, float distance, IPlace closestPlace, bool astronomical)
        {
            var pointFound = false;
            var pointIndex = -1;
            var target3d = Coordinates.GeoTo3dDouble(target.Lat, target.Lng, 1);

            for (var i = 0; i < shapefile.Shapes.Count; i++)
            {
                if (shapefile.Shapes[i] is ComplexShape)
                {
                    var p = (ComplexShape)shapefile.Shapes[i];

                    if (PointInboundingBox(target, p.BoundingBox))
                    {
                        if (p.Attributes != null && p.Attributes.ItemArray.GetLength(0) > 0)
                        {
                            var nameCol = GetNameColumn();
                            if (nameCol == -1)
                            {
                                nameCol = 0;
                            }

                            var place = new TourPlace(p.Attributes.ItemArray[nameCol].ToString(), (p.BoundingBox[1] + p.BoundingBox[3]) / 2, (p.BoundingBox[0] + p.BoundingBox[2]) / 2, Classification.Unidentified, "", ImageSetType.Earth, -1);

                            var rowData = new Dictionary<string, string>();
                            for (var r = 0; r < p.Attributes.ItemArray.GetLength(0); r++)
                            {

                                rowData.Add(p.Attributes.Table.Columns[r].ColumnName, p.Attributes.ItemArray[r].ToString());

                            }
                            place.Tag = rowData;
                            return place;
                        }
                        return closestPlace;

                    }
                }

                else if (shapefile.Shapes[i].GetType() == typeof(ShapefileTools.Point))
                {
                    var p = (ShapefileTools.Point)shapefile.Shapes[i];

                    var point = Coordinates.GeoTo3dDouble(p.Y, p.X, 1);
                    var dist = target3d - point;

                    if (dist.Length() < distance)
                    {
                        pointFound = true;
                        pointIndex = i;
                        distance = (float)dist.Length();
                    }

                }

            }

            if (pointFound)
            {
                var p = (ShapefileTools.Point)shapefile.Shapes[pointIndex];
                if (p.Attributes.ItemArray.GetLength(0) > 0)
                {
                    var place = new TourPlace(p.Attributes.ItemArray[0].ToString(), p.Y, p.X, Classification.Unidentified, "", ImageSetType.Earth, -1);

                    var rowData = new Dictionary<string, string>();
                    for (var r = 0; r < p.Attributes.ItemArray.GetLength(0); r++)
                    {

                        rowData.Add(p.Attributes.Table.Columns[r].ColumnName, p.Attributes.ItemArray[r].ToString());

                    }
                    place.Tag = rowData;
                    return place;
                }
            }
            return closestPlace;
        }
        private static SharpDX.Direct3D11.InputLayout layout;

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (shapefile == null)
            {
                return false;
            }

            if (shapeFileVertex == null)
            {

                var vertList = new List<Vector3>();
                var indexList = new List<UInt32>();
                UInt32 firstItemIndex = 0;
                var lastItem = new Vector3();
                var firstItem = true;



                var north = true;
                double offsetX = 0;
                double offsetY = 0;
                double centralMeridian = 0;
                double mapScale = 0;
                double standardParallel = 70;

                if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                {
                    north = shapefile.FileHeader.ProjectionInfo.Name.ToLower().Contains("north");
                    standardParallel = shapefile.FileHeader.ProjectionInfo.GetParameter("standard_parallel_1");
                    centralMeridian = shapefile.FileHeader.ProjectionInfo.GetParameter("central_meridian");
                    mapScale = shapefile.FileHeader.ProjectionInfo.GetParameter("scale_factor");
                    offsetY = shapefile.FileHeader.ProjectionInfo.GetParameter("false_easting");
                    offsetX = shapefile.FileHeader.ProjectionInfo.GetParameter("false_northing");

                }




                UInt32 currentIndex = 0;
                var color = Color;
                var count = 360;
                for (var i = 0; i < shapefile.Shapes.Count; i++)
                {
                    if (shapefile.Shapes[i].GetType() == typeof(Polygon))
                    {
                        var p = (Polygon)shapefile.Shapes[i];

                        for (var z = 0; z < p.Rings.Length; z++)
                        {
                            count = (p.Rings[z].Points.Length);

                            // content from DBF
                            var dr = p.Rings[z].Attributes;

                            for (var k = 0; k < p.Rings[z].Points.Length; k++)
                            {
                                // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                                var Xcoord = p.Rings[z].Points[k].X;
                                var Ycoord = p.Rings[z].Points[k].Y;

                                if (shapefile.Projection == ShapeFile.Projections.Geo)
                                {
                                    lastItem = Coordinates.GeoTo3d(Ycoord, Xcoord);
                                }
                                else if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                                {
                                    lastItem = Coordinates.SterographicTo3d(Xcoord, Ycoord, 1, standardParallel, centralMeridian, offsetX, offsetY, mapScale, north).Vector3;
                                }

                                if (k == 0)
                                {
                                    firstItemIndex = currentIndex;
                                    firstItem = true;
                                }

                                vertList.Add(lastItem);

                                if (firstItem)
                                {
                                    firstItem = false;
                                }
                                else
                                {
                                    indexList.Add(currentIndex);
                                    currentIndex++;
                                    indexList.Add(currentIndex);
                                }
                            }

                            indexList.Add(currentIndex);
                            indexList.Add(firstItemIndex);
                            currentIndex++;

                        }
                    }
                    else if (shapefile.Shapes[i].GetType() == typeof(PolygonZ))
                    {
                        var p = (PolygonZ)shapefile.Shapes[i];

                        for (var z = 0; z < p.Rings.Length; z++)
                        {
                            count = (p.Rings[z].Points.Length);

                            // content from DBF
                            var dr = p.Rings[z].Attributes;

                            for (var k = 0; k < p.Rings[z].Points.Length; k++)
                            {
                                // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                                var Xcoord = p.Rings[z].Points[k].X;
                                var Ycoord = p.Rings[z].Points[k].Y;

                                if (shapefile.Projection == ShapeFile.Projections.Geo)
                                {
                                    lastItem = Coordinates.GeoTo3d(Ycoord, Xcoord);
                                }
                                else if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                                {
                                    lastItem = Coordinates.SterographicTo3d(Xcoord, Ycoord, 1, standardParallel, centralMeridian, offsetX, offsetY, mapScale, north).Vector3;
                                }


                                if (k == 0)
                                {
                                    firstItemIndex = currentIndex;
                                    firstItem = true;
                                }

                                vertList.Add(lastItem);

                                if (firstItem)
                                {
                                    firstItem = false;
                                }
                                else
                                {
                                    indexList.Add(currentIndex);
                                    currentIndex++;
                                    indexList.Add(currentIndex);
                                }
                            }

                            indexList.Add(currentIndex);
                            indexList.Add(firstItemIndex);
                            currentIndex++;

                        }
                    }
                    else if (shapefile.Shapes[i].GetType() == typeof(PolyLine))
                    {
                        var p = (PolyLine)shapefile.Shapes[i];
                        for (var z = 0; z < p.Lines.Length; z++)
                        {
                            count = (p.Lines[z].Points.Length);

                            firstItem = true;
                            for (var k = 0; k < p.Lines[z].Points.Length; k++)
                            {
                                // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                                var Xcoord = p.Lines[z].Points[k].X;
                                var Ycoord = p.Lines[z].Points[k].Y;
                                if (shapefile.Projection == ShapeFile.Projections.Geo)
                                {
                                    lastItem = Coordinates.GeoTo3d(Ycoord, Xcoord);
                                }
                                else if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                                {
                                    lastItem = Coordinates.SterographicTo3d(Xcoord, Ycoord, 1, standardParallel, centralMeridian, offsetX, offsetY, mapScale, north).Vector3;
                                }

                                if (k == 0)
                                {
                                    firstItemIndex = currentIndex;
                                    firstItem = true;
                                }

                                vertList.Add(lastItem);

                                if (firstItem)
                                {
                                    firstItem = false;
                                }
                                else
                                {
                                    indexList.Add(currentIndex);
                                    currentIndex++;
                                    indexList.Add(currentIndex);
                                }

                            }
                            currentIndex++;


                        }
                    }
                    else if (shapefile.Shapes[i].GetType() == typeof(PolyLineZ))
                    {
                        var p = (PolyLineZ)shapefile.Shapes[i];
                        for (var z = 0; z < p.Lines.Length; z++)
                        {
                            count = (p.Lines[z].Points.Length);
                            var points = new Vector3[(count)];

                            firstItem = true;
                            for (var k = 0; k < p.Lines[z].Points.Length; k++)
                            {
                                // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                                var Xcoord = p.Lines[z].Points[k].X;
                                var Ycoord = p.Lines[z].Points[k].Y;
                                if (shapefile.Projection == ShapeFile.Projections.Geo)
                                {
                                    lastItem = Coordinates.GeoTo3d(Ycoord, Xcoord);
                                }
                                else if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                                {
                                    lastItem = Coordinates.SterographicTo3d(Xcoord, Ycoord, 1, standardParallel, centralMeridian, offsetX, offsetY, mapScale, north).Vector3;
                                }

                                if (k == 0)
                                {
                                    firstItemIndex = currentIndex;
                                    firstItem = true;
                                }

                                vertList.Add(lastItem);

                                if (firstItem)
                                {
                                    firstItem = false;
                                }
                                else
                                {
                                    indexList.Add(currentIndex);
                                    currentIndex++;
                                    indexList.Add(currentIndex);
                                }

                            }
                            currentIndex++;


                        }
                    }
                    else if (shapefile.Shapes[i].GetType() == typeof(ShapefileTools.Point))
                    {
                        var p = (ShapefileTools.Point)shapefile.Shapes[i];

                        // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                        var Xcoord = p.X;
                        var Ycoord = p.Y;
                        if (shapefile.Projection == ShapeFile.Projections.Geo)
                        {
                            lastItem = Coordinates.GeoTo3d(Ycoord, Xcoord);
                        }
                        else if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
                        {
                            lastItem = Coordinates.SterographicTo3d(Xcoord, Ycoord, 1, standardParallel, centralMeridian, offsetX, offsetY, mapScale, north).Vector3;
                        }

                        vertList.Add(lastItem);

                        currentIndex++;
                        lines = false;
                    }

                }

                shapeVertexCount = vertList.Count;
                shapeFileVertex = new PositionVertexBuffer11(vertList.Count, RenderContext11.PrepDevice);

                var verts = (Vector3[])shapeFileVertex.Lock(0, 0); // Lock the buffer (which will return our structs)
                var indexer = 0;
                foreach (var vert in vertList)
                {
                    verts[indexer++] = vert;
                }
                shapeFileVertex.Unlock();


                shapeIndexCount = indexList.Count;

                if (lines)
                {
                    if (indexList.Count > 65500)
                    {
                        isLongIndex = true;
                        shapeFileIndex = new IndexBuffer11(typeof(UInt32), indexList.Count, RenderContext11.PrepDevice);
                    }
                    else
                    {
                        isLongIndex = false;
                        shapeFileIndex = new IndexBuffer11(typeof(short), indexList.Count, RenderContext11.PrepDevice);
                    }

                    if (isLongIndex)
                    {
                        indexer = 0;
                        var indexes = (UInt32[])shapeFileIndex.Lock();
                        foreach (var indexVal in indexList)
                        {
                            indexes[indexer++] = indexVal;
                        }
                        shapeFileIndex.Unlock();
                    }
                    else
                    {
                        indexer = 0;
                        var indexes = (short[])shapeFileIndex.Lock();
                        foreach (var indexVal in indexList)
                        {
                            indexes[indexer++] = (short)indexVal;
                        }
                        shapeFileIndex.Unlock();
                    }
                }
            }


            renderContext.DepthStencilMode = DepthStencilMode.Off;
            renderContext.BlendMode = BlendMode.Alpha;
            SimpleLineShader11.Color = Color.FromArgb((int)(opacity * 255), Color);

            var mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            SimpleLineShader11.WVPMatrix = mat;
            SimpleLineShader11.CameraPosition = Vector3d.TransformCoordinate(renderContext.CameraPosition, Matrix3d.Invert(renderContext.World)).Vector3;
            SimpleLineShader11.ShowFarSide = false;
            SimpleLineShader11.Sky = false;

            renderContext.SetVertexBuffer(shapeFileVertex);
            SimpleLineShader11.Use(renderContext.devContext);

            if (lines)
            {
                renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;
                renderContext.SetIndexBuffer(shapeFileIndex);
                renderContext.devContext.DrawIndexed(shapeFileIndex.Count, 0, 0);
            }
            else
            {
                renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.PointList;
                renderContext.devContext.Draw(shapeVertexCount, 0);
            }

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            return true;
        }

        public override bool CanCopyToClipboard()
        {

            return true;
        }

        public override void CopyToClipboard()
        {
            //todo copy binary format

            var data = ToWellKnownText();

            Clipboard.SetText(data);

        }



        public string ToWellKnownText()
        {
            if (shapefile == null)
            {
                return null;
            }
            var sb = new StringBuilder();

            // Make Header

            sb.Append("Geography");
            if (shapefile.Table != null)
            {
                foreach (DataColumn col in shapefile.Table.Columns)
                {
                    sb.Append("\t");
                    sb.Append(col.ColumnName);
                }
            }
            sb.AppendLine();

            var firstItem = true;
            var firstShape = true;


            var north = true;
            double offsetX = 0;
            double offsetY = 0;
            double centralMeridian = 0;
            double mapScale = 0;
            double standardParallel = 70;

            if (shapefile.Projection == ShapeFile.Projections.PolarStereo)
            {
                north = shapefile.FileHeader.ProjectionInfo.Name.ToLower().Contains("north");
                standardParallel = shapefile.FileHeader.ProjectionInfo.GetParameter("standard_parallel_1");
                centralMeridian = shapefile.FileHeader.ProjectionInfo.GetParameter("central_meridian");
                mapScale = shapefile.FileHeader.ProjectionInfo.GetParameter("scale_factor");
                offsetY = shapefile.FileHeader.ProjectionInfo.GetParameter("false_easting");
                offsetX = shapefile.FileHeader.ProjectionInfo.GetParameter("false_northing");

            }

            var color = Color.ToArgb();
            var count = 360;
            for (var i = 0; i < shapefile.Shapes.Count; i++)
            {
                if (shapefile.Shapes[i].GetType() == typeof(Polygon))
                {
                    firstShape = true;
                    firstItem = true;
                    sb.Append("Polygon (");
                    var p = (Polygon)shapefile.Shapes[i];

                    for (var z = 0; z < p.Rings.Length; z++)
                    {
                        if (firstShape)
                        {
                            firstShape = false;
                        }
                        else
                        {
                            sb.Append(",");
                        }
                        sb.Append("(");
                        count = (p.Rings[z].Points.Length);
                        // content from DBF
                        var dr = p.Rings[z].Attributes;
                        firstItem = true;
                        for (var k = 0; k < p.Rings[z].Points.Length; k++)
                        {
                            // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                            var Xcoord = p.Rings[z].Points[k].X;
                            var Ycoord = p.Rings[z].Points[k].Y;
                            if (firstItem)
                            {
                                firstItem = false;
                            }
                            else
                            {
                                sb.Append(",");
                            }

                          //  sb.Append(Xcoord.ToString("F5") + " " + Ycoord.ToString("F5"));
                            sb.Append(Xcoord.ToString() + " " + Ycoord.ToString());
                        }

                        sb.Append(")");
                    }
                    sb.Append(")");
                    AddTableRow(sb, shapefile.Shapes[i].Attributes);
                    sb.AppendLine();

                }
                else if (shapefile.Shapes[i].GetType() == typeof(PolyLine))
                {
                    var p = (PolyLine)shapefile.Shapes[i];
                    for (var z = 0; z < p.Lines.Length; z++)
                    {
                        firstShape = true;
                        firstItem = true;
                        sb.Append("LineString (");
                        count = (p.Lines[z].Points.Length);
                        var points = new PositionColoredTextured[(count)];

                        firstItem = true;
                        for (var k = 0; k < p.Lines[z].Points.Length; k++)
                        {
                            //if (firstShape)
                            //{
                            //    firstShape = false;
                            //}
                            //else
                            //{
                            //    sb.Append(",");
                            //}

                            // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                            var Xcoord = p.Lines[z].Points[k].X;
                            var Ycoord = p.Lines[z].Points[k].Y;
                            if (firstItem)
                            {
                                firstItem = false;
                            }
                            else
                            {
                                sb.Append(",");
                            }

                            sb.Append(Xcoord.ToString() + " " + Ycoord.ToString());

                        }
                        sb.Append(")");
                        AddTableRow(sb, shapefile.Shapes[i].Attributes);
                        sb.AppendLine();

                    }
                }
                else if (shapefile.Shapes[i].GetType() == typeof(PolyLineZ))
                {
                    var p = (PolyLineZ)shapefile.Shapes[i];
                    for (var z = 0; z < p.Lines.Length; z++)
                    {
                        firstShape = true;
                        firstItem = true;
                        sb.Append("LineString (");
                        count = (p.Lines[z].Points.Length);
                        var points = new PositionColoredTextured[(count)];

                        firstItem = true;
                        for (var k = 0; k < p.Lines[z].Points.Length; k++)
                        {
                            //if (firstShape)
                            //{
                            //    firstShape = false;
                            //}
                            //else
                            //{
                            //    sb.Append(",");
                            //}

                            // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                            var Xcoord = p.Lines[z].Points[k].X;
                            var Ycoord = p.Lines[z].Points[k].Y;
                            var Zcoord = p.Lines[z].Points[k].Z;
                            if (firstItem)
                            {
                                firstItem = false;
                            }
                            else
                            {
                                sb.Append(",");
                            }

                            sb.Append(Xcoord.ToString() + " " + Ycoord.ToString() + " " + Zcoord.ToString());

                        }
                        sb.Append(")");
                        AddTableRow(sb, shapefile.Shapes[i].Attributes);
                        sb.AppendLine();

                    }
                }
                else if (shapefile.Shapes[i].GetType() == typeof(ShapefileTools.Point))
                {
                    var p = (ShapefileTools.Point)shapefile.Shapes[i];

                    // 2D Point coordinates. 3d also supported which would add a Z. There's also an optional measure (M) that can be used.
                    var Xcoord = p.X;
                    var Ycoord = p.Y;

                    sb.Append(string.Format("Point ({0} {1})"));

                    AddTableRow(sb, shapefile.Shapes[i].Attributes);
                    sb.AppendLine();
                   

                }
            }




            return sb.ToString();
        }

        private void AddTableRow(StringBuilder sb, DataRow dataRow)
        {
            if (dataRow != null)
            {
                foreach (var col in dataRow.ItemArray)
                {
                    sb.Append("\t");
                    sb.Append(col.ToString());
                }
            }
        }

        public int GetNameColumn()
        {
            var col = -1;
            var index = 0;
            if (shapefile.Table != null)
            {
                foreach (DataColumn column in shapefile.Table.Columns)
                {
                    if (column.ColumnName.ToLower().Contains("name"))
                    {
                        col = index;
                    }
                    index++;
                }
            }
            return col;

        }

        //[LayerProperty]
        //public override Color Color
        //{
        //    get
        //    {
        //        return base.Color;
        //    }
        //    set
        //    {
        //        base.Color = value;
        //        CleanUp();
        //    }
        //}

        public override void CleanUp()
        {
            if (shapeFileIndex != null)
            {
                shapeFileIndex.Dispose();
                GC.SuppressFinalize(shapeFileIndex);
                shapeFileIndex = null;
            }
            if (shapeFileVertex != null)
            {
                shapeFileVertex.Dispose();
                GC.SuppressFinalize(shapeFileVertex);

                shapeFileVertex = null;
            }
        }
    }

    public class ShapeFileLayerUI : LayerUI
    {
        readonly ShapeFileRenderer layer;
        bool opened = true;

        public ShapeFileLayerUI(ShapeFileRenderer layer)
        {
            this.layer = layer;
        }
        IUIServicesCallbacks callbacks;

        public override void SetUICallbacks(IUIServicesCallbacks callbacks)
        {
            this.callbacks = callbacks;
        }
        public override bool HasTreeViewNodes
        {
            get
            {
                return true;
            }
        }

        public override List<LayerUITreeNode> GetTreeNodes()
        {
            var nodes = new List<LayerUITreeNode>();
            var nameColumn = layer.GetNameColumn();
            for (var i = 0; i < layer.shapefile.Shapes.Count; i++)
            {
                var type = layer.shapefile.Shapes[i].GetType();

                var node = new LayerUITreeNode();
                if (nameColumn == -1)
                {
                    node.Name = type.Name;
                }
                else
                {
                    node.Name = layer.shapefile.Shapes[i].Attributes[nameColumn].ToString();
                }

                node.Tag = layer.shapefile.Shapes[i];
                node.Checked = true;
                node.NodeSelected += new LayerUITreeNodeSelectedDelegate(node_NodeSelected);
                nodes.Add(node);
            }
            return nodes;
        }



        void node_NodeSelected(LayerUITreeNode node)
        {
            if (callbacks != null)
            {
                var shape = (Shape)node.Tag;

                if (shape.Attributes != null && shape.Attributes.ItemArray.GetLength(0) > 0)
                {

                    var rowData = new Dictionary<string, string>();
                    for (var r = 0; r < shape.Attributes.ItemArray.GetLength(0); r++)
                    {

                        rowData.Add(shape.Attributes.Table.Columns[r].ColumnName, shape.Attributes.ItemArray[r].ToString());

                    }
                    callbacks.ShowRowData(rowData);
                }
            }
        }

        public override List<LayerUIMenuItem> GetNodeContextMenu(LayerUITreeNode node)
        {
            return base.GetNodeContextMenu(node);
        }
    }
}
