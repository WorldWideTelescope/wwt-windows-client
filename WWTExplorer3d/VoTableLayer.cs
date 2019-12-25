using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.IO;
using System.Reflection;
using System.Xml;

using Vector3 = SharpDX.Vector3;

namespace TerraViewer
{
    public class VoTableLayer : TimeSeriesLayer
    {
        public VOTableViewer Viewer = null;
        public VoTableLayer()
        {
            this.table = null;
            this.filename = "";

            PlotType = PlotTypes.Circle;
        }
        public VoTableLayer(VoTable table)
        {
            this.table = table;
            this.filename = table.LoadFilename;
            LngColumn = table.GetRAColumn().Index;
            LatColumn = table.GetDecColumn().Index;
            PlotType = PlotTypes.Circle;
        }
        public override void AddFilesToCabinet(FileCabinet fc)
        {
            string fName = filename;

            bool copy = true;

            string fileName = fc.TempDirectory + string.Format("{0}\\{1}.txt", fc.PackageID, this.ID.ToString());
            string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);
            string path2 = fileName.Substring(0, fileName.LastIndexOf('\\') + 1);

            if (copy)
            {
                if (!Directory.Exists(path2))
                {
                    Directory.CreateDirectory(path2);
                }
                if (File.Exists(fName) && !File.Exists(fileName))
                {
                    File.Copy(fName, fileName);
                }
            }

            if (File.Exists(fileName))
            {
                fc.AddFile(fileName);
            }
        }
        string filename = "";
        public override void LoadData(string path)
        {
            filename = path;
            table = new VoTable(path);
            LngColumn = table.GetRAColumn().Index;
            LatColumn = table.GetDecColumn().Index;
        }

        public override bool CanCopyToClipboard()
        {
            return true;
        }

        public override void CopyToClipboard()
        {
            System.Windows.Clipboard.SetText(table.ToString());
        }

        public override IPlace FindClosest(Coordinates target, float distance, IPlace defaultPlace, bool astronomical)
        {
            Vector3d searchPoint = Coordinates.GeoTo3dDouble(target.Lat, target.Lng);

            Vector3d dist;
            if (defaultPlace != null)
            {
                Vector3d testPoint = Coordinates.RADecTo3d(defaultPlace.RA, -defaultPlace.Dec, -1.0);
                dist = searchPoint - testPoint;
                distance = (float)dist.Length();
            }

            int closestItem = -1;
            int index = 0;
            foreach (Vector3 point in positions)
            {
                dist = searchPoint - new Vector3d(point);
                if (dist.Length() < distance)
                {
                    distance = (float)dist.Length();
                    closestItem = index;
                }
                index++;
            }

            if (closestItem == -1)
            {
                return defaultPlace;
            }

            Coordinates pnt = Coordinates.CartesianToSpherical2(positions[closestItem]);

            string name = table.Rows[closestItem].ColumnData[this.nameColumn].ToString();
            if (nameColumn == startDateColumn || nameColumn == endDateColumn)
            {
                name = SpreadSheetLayer.ParseDate(name).ToString("u");
            }

            if (String.IsNullOrEmpty(name))
            {
                name = string.Format("RA={0}, Dec={1}", Coordinates.FormatHMS(pnt.RA), Coordinates.FormatDMS(pnt.Dec));
            }
            TourPlace place = new TourPlace(name, pnt.Lat, pnt.RA, Classification.Unidentified, "", ImageSetType.Sky, -1);

            Dictionary<String, String> rowData = new Dictionary<string, string>();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string colValue = table.Rows[closestItem][i].ToString();
                if (i == startDateColumn || i == endDateColumn)
                {
                    colValue = SpreadSheetLayer.ParseDate(colValue).ToString("u");
                }

                if (!rowData.ContainsKey(table.Column[i].Name) && !string.IsNullOrEmpty(table.Column[i].Name))
                {
                    rowData.Add(table.Column[i].Name, colValue);
                }
                else
                {
                    rowData.Add("Column" + i.ToString(), colValue);
                }
            }
            place.Tag = rowData;
            if (Viewer != null)
            {
                Viewer.LabelClicked(closestItem);
            }
            return place;
        }

        protected override bool PrepVertexBuffer(float opacity)
        {
            VoColumn col = table.GetColumnByUcd("meta.id");
            if (col == null)
            {
                col = table.Column[0];
            }

            if (shapeFileVertex == null)
            {
                bool siapSet = IsSiapResultSet();

                if (lineList2d == null)
                {
                    lineList2d = new LineList();
                }
                lineList2d.Clear();

                VoColumn stcsCol = table.GetColumnByUcd("phys.area;obs.field");

                if (stcsCol == null && table.Columns.ContainsKey("regionSTCS"))
                {
                    stcsCol = table.Columns["regionSTCS"];
                }

                if (PlotType == PlotTypes.Gaussian)
                {
                    MarkerScale = MarkerScales.World;
                }
                else
                {
                    MarkerScale = MarkerScales.Screen;
                }

                List<TimeSeriesPointVertex> vertList = new List<TimeSeriesPointVertex>();
                List<UInt32> indexList = new List<UInt32>();
                TimeSeriesPointVertex lastItem = new TimeSeriesPointVertex();
                positions.Clear();
                UInt32 currentIndex = 0;
                Color color = Color.FromArgb((int)(opacity * (float)Color.A), Color);

                pointScaleType = PointScaleTypes.StellarMagnitude;

                foreach (VoRow row in table.Rows)
                {
                    try
                    {
                        if (lngColumn > -1 && latColumn > -1)
                        {
                            double Xcoord = Coordinates.ParseRA(row[this.LngColumn].ToString(), true) * 15 + 180;
                            double Ycoord = Coordinates.ParseDec(row[this.LatColumn].ToString());
                            lastItem.Position = Coordinates.GeoTo3dDouble(Ycoord, Xcoord).Vector311;
                            positions.Add(lastItem.Position);
                            lastItem.Color = color;
                            if (sizeColumn > -1)
                            {

                                try
                                {
                                    if (MarkerScale == MarkerScales.Screen)
                                    {
                                        lastItem.PointSize = 20f;
                                    }
                                    else
                                    {
                                        switch (pointScaleType)
                                        {
                                            case PointScaleTypes.Linear:
                                                lastItem.PointSize = Convert.ToSingle(row[sizeColumn]);
                                                break;
                                            case PointScaleTypes.Log:
                                                lastItem.PointSize = (float)Math.Log(Convert.ToSingle(row[sizeColumn]));
                                                break;
                                            case PointScaleTypes.Power:
                                                lastItem.PointSize = (float)Math.Pow(2, Convert.ToSingle(row[sizeColumn]));
                                                break;
                                            case PointScaleTypes.StellarMagnitude:
                                                {
                                                    double size = Convert.ToSingle(row[sizeColumn]);
                                                    lastItem.PointSize = (float)(40 / Math.Pow(1.6, size)) * 10;
                                                }
                                                break;
                                            case PointScaleTypes.Constant:
                                                lastItem.PointSize = 1;
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                catch
                                {
                                    lastItem.PointSize = .01f;
                                }
                            }
                            else
                            {
                                if (MarkerScale == MarkerScales.Screen)
                                {
                                    lastItem.PointSize = 20;
                                }
                                else
                                {
                                    lastItem.PointSize = (float)Math.Pow(2, 1) * 100;
                                }
                            }


                            if (startDateColumn > -1)
                            {
                                DateTime dateTime = DateTime.Parse(row[startDateColumn].ToString());
                                lastItem.Tu = (float)SpaceTimeController.UtcToJulian(dateTime);
                                lastItem.Tv = 0;
                            }


                            vertList.Add(lastItem);
                            currentIndex++;
                        }


                        if (siapSet && stcsCol!= null)
                        {
                            AddSiapStcRow(stcsCol.Name, row, row == table.SelectedRow);
                        }
                    }

                    catch
                    {
                    }
                    lines = false;
                }

                if (siapSet && stcsCol != null)
                {
                    AddSiapStcRow(stcsCol.Name, table.SelectedRow, true);
                }
    

                shapeVertexCount = vertList.Count;
                if (shapeVertexCount == 0)
                {
                    shapeVertexCount = 1;
                }
                shapeFileVertex = new TimeSeriesPointSpriteSet(RenderContext11.PrepDevice, vertList.ToArray());
            }
            return true;
        }

        private void AddSiapStcRow(string stcsColName, VoRow row, bool selected)
        {
            string stcs = row[stcsColName].ToString().Replace("  ", " ");
            Color col = Color.FromArgb(120, 255, 255, 255);

            if (selected)
            {
                col = Color.Yellow;
            }

            if (stcs.StartsWith("Polygon J2000"))
            {
                string[] parts = stcs.Split(new char[] { ' ' });

                int len = parts.Length;
                int index = 0;
                while (index < len)
                {
                    if (parts[index] == "Polygon")
                    {
                        index += 2;
                        Vector3d lastPoint = new Vector3d();
                        Vector3d firstPoint = new Vector3d();
                        bool start = true;
                        for (int i = index; i < len; i += 2)
                        {
                            if (parts[i] == "Polygon")
                            {
                                start = true;
                                break;
                            }
                            else
                            {
                                double Xcoord = Coordinates.ParseRA(parts[i], true) * 15 + 180;
                                double Ycoord = Coordinates.ParseDec(parts[i + 1]);
         

                                Vector3d pnt = Coordinates.GeoTo3dDouble(Ycoord, Xcoord);

                                if (!start)
                                {
                                    lineList2d.AddLine(lastPoint, pnt, col, new Dates());
                                }
                                else
                                {
                                    firstPoint = pnt;
                                    start = false;
                                }
                                lastPoint = pnt;
                            }
                            index += 2;
                        }
                        if (len > 4)
                        {
                            lineList2d.AddLine(firstPoint, lastPoint, col, new Dates());
                        }

                        

                    }
                }
            }
        }

        public bool IsSiapResultSet()
        {
            return table.GetColumnByUcd("vox:image.title") != null && table.GetColumnByUcd("VOX:Image.AccessReference") != null;
 
        }

        public override string[] Header
        {
            get
            {
                string[] header = new string[table.Columns.Count];
                int index = 0;
                foreach (VoColumn col in table.Column)
                {
                    header[index++] = col.Name;
                }

                return header;
            }

        }


        private VoTable table;

        public VoTable Table
        {
            get { return table; }
            set { table = value; }
        }

    }
 
}
