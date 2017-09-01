using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace TerraViewer
{
    public partial class VOTableViewer : Form
    {
        public static string WindowText = "Microsoft WorldWide Telescope - VO Table Viewer";
        public VOTableViewer()
        {
            InitializeComponent();
            String[] typeMarkerValues = Enum.GetNames(typeof(TimeSeriesLayer.PlotTypes));
            markerTypeCombo.Items.AddRange(typeMarkerValues);
            markerTypeCombo.SelectedIndex = 0;

            SetUiStrings();

        }

        private void SetUiStrings()
        {
            raSourceLabel.Text = Language.GetLocalizedText(634, "RA Source");
            decSourceLabel.Text = Language.GetLocalizedText(635, "Dec Source");
            distanceSouceLabel.Text = Language.GetLocalizedText(636, "Distance Source");
            typeSourceLabel.Text = Language.GetLocalizedText(638, "Type Source");
            sizeSourceLabel.Text = Language.GetLocalizedText(639, "Size/Mag Source");
            save.Text = Language.GetLocalizedText(640, "Save As...");
            sync.Text = Language.GetLocalizedText(597, "Broadcast");
            label1.Text = Language.GetLocalizedText(641, "Marker Type");
            Text = Language.GetLocalizedText(642, "Microsoft WorldWide Telescope - VO Table Viewer");
        }
        public VoTableLayer layer = null;

        public VoTableLayer Layer
        {
            get { return layer; }
            set
            {
                layer = value;
                table = layer.Table;
                layer.Viewer = this;
                UpdateTable();
            }

        }

        private VoTable table;


        private void VOTableViewer_Load(object sender, EventArgs e)
        {

            UpdateTable();

            loadImage.Visible = layer.IsSiapResultSet();


            this.Owner = Earth3d.MainWindow;
        }

        public void UpdateTable()
        {
            listView1.Clear();
            listView1.Columns.Clear();
            foreach (VoColumn col in table.Columns.Values)
            {
                listView1.Columns.Add(col.Name);
            }

            listView1.VirtualListSize = table.Rows.Count;
            LoadColumnsForCombos();

            this.Text = WindowText + " : " + table.Rows.Count + " Rows";


        }
        private void LoadColumnsForCombos()
        {
            this.markerTypeCombo.SelectedIndex = (int)layer.PlotType;

            // bool star =  plotType.SelectedItem == "Star";
            VoColumn raColSelect = layer.LngColumn > -1 ? table.Column[layer.LngColumn] : null;
            VoColumn decColSelect = layer.LatColumn > -1 ? table.Column[layer.LatColumn] : null;
            VoColumn distColSelect = layer.AltColumn > -1 ? table.Column[layer.AltColumn] : null;
            VoColumn typeColSelect = layer.MarkerColumn > -1 ? table.Column[layer.MarkerColumn] : null;
            VoColumn sizeColSelect = layer.SizeColumn > -1 ? table.Column[layer.SizeColumn] : null;

            raSource.Items.Clear();
            decSource.Items.Clear();
            distanceSource.Items.Clear();
            typeSource.Items.Clear();
            sizeSource.Items.Clear();

            raSource.Items.Add("None");
            decSource.Items.Add("None");
            distanceSource.Items.Add("None");
            typeSource.Items.Add("None");
            sizeSource.Items.Add("None");

            int index = 0;
            foreach (VoColumn col in table.Columns.Values)
            {
                index = raSource.Items.Add(col);
                if (col == raColSelect)
                {
                    raSource.SelectedIndex = index;
                }

                index = decSource.Items.Add(col);

                if (col == decColSelect)
                {
                    decSource.SelectedIndex = index;
                }
                index = distanceSource.Items.Add(col);

                if (col == distColSelect)
                {
                    distanceSource.SelectedIndex = index;
                }
                index = typeSource.Items.Add(col);

                if (col == typeColSelect)
                {
                    typeSource.SelectedIndex = index;
                }

                index = sizeSource.Items.Add(col);
                if (col == sizeColSelect)
                {
                    sizeSource.SelectedIndex = index;
                }
            }
        }

        private void GetDefaultColumns()
        {

        }

        private void listView1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            VoRow row = table.Rows[e.ItemIndex];

            string[] data = new string[row.ColumnData.GetLength(0)];
            int index = 0;
            foreach (object o in row.ColumnData)
            {
                data[index++] = o.ToString();
            }
            e.Item = new ListViewItem(data);
            e.Item.Checked = row.Selected;
            e.Item.Tag = row;
        }

        public void HighlightRow(int index)
        {
            if (listView1.Items.Count > index)
            {
                listView1.Items[index].Selected = true;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                listView1.FullRowSelect = true;
                try
                {
                    VoRow row = table.Rows[listView1.SelectedIndices[0]];

                    table.SelectedRow = row;
                    layer.CleanUp();
                    double ra = Coordinates.ParseRA(row[raSource.SelectedIndex - 1].ToString(), true);
                    double dec = Coordinates.ParseDec(row[decSource.SelectedIndex - 1].ToString());
                    string id;

                    VoColumn col = table.GetColumnByUcd("meta.id");
                    if (col != null)
                    {
                        id = row[col.Name].ToString();
                    }
                    else
                    {
                        id = row[0].ToString();
                    }

                    TourPlace pl = new TourPlace(id, dec, ra, Classification.Star, Constellations.Containment.FindConstellationForPoint(ra, dec), ImageSetType.Sky, -1);
                    Earth3d.MainWindow.SetLabelText(pl, true);

                    if (table.SampId != null)
                    {
                        Earth3d.MainWindow.sampConnection.TableHighlightRow("", table.SampId, listView1.SelectedIndices[0]);
                    }
                }
                catch
                {
                }

            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                VoRow row = table.Rows[listView1.SelectedIndices[0]];
                double ra = Coordinates.ParseRA(row[raSource.SelectedIndex - 1].ToString(), true);
                double dec = Coordinates.ParseDec(row[decSource.SelectedIndex - 1].ToString());
                string id;

                VoColumn col = table.GetColumnByUcd("meta.id");
                if (col != null)
                {
                    id = row[col.Name].ToString();
                }
                else
                {
                    id = row[0].ToString();
                }

                TourPlace pl = new TourPlace(id, dec, ra, Classification.Star, Constellations.Containment.FindConstellationForPoint(ra, dec), ImageSetType.Sky, -1);
                RenderEngine.Engine.GotoTarget(pl, false, true, false);
            }
        }


        private void listView1_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                listView1.FullRowSelect = true;
                try
                {
                    VoRow row = table.Rows[e.Item.Index];

                    // double ra = Convert.ToDouble(row[GetRAColumn().Name]) / 15;
                    // double dec = Convert.ToDouble(row[GetDecColumn().Name]);
                    double ra = Coordinates.ParseRA(row[raSource.SelectedIndex - 1].ToString(), true);
                    double dec = Coordinates.ParseDec(row[decSource.SelectedIndex - 1].ToString());
                    string id;

                    VoColumn col = table.GetColumnByUcd("meta.id");
                    if (col != null)
                    {
                        id = row[col.Name].ToString();
                    }
                    else
                    {
                        id = row[0].ToString();
                    }

                    TourPlace pl = new TourPlace(id, dec, ra, Classification.Star, Constellations.Containment.FindConstellationForPoint(ra, dec), ImageSetType.Sky, -1);
                    Earth3d.MainWindow.SetLabelText(pl, true);
                }
                catch
                {
                }

            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count > 0)
            {
                try
                {
                    VoRow row = table.Rows[listView1.SelectedIndices[0]];
                    double ra = Coordinates.ParseRA(row[raSource.SelectedIndex - 1].ToString(), true);
                    double dec = Coordinates.ParseDec(row[decSource.SelectedIndex - 1].ToString().ToString());
                    string id;

                    VoColumn col = table.GetColumnByUcd("meta.id");
                    if (col != null)
                    {
                        id = row[col.Name].ToString();
                    }
                    else
                    {
                        id = row[0].ToString();
                    }

                    TourPlace pl = new TourPlace(id, dec, ra, Classification.Star, Constellations.Containment.FindConstellationForPoint(ra, dec), ImageSetType.Sky, -1);
                    RenderEngine.Engine.GotoTarget(pl, false, false, false);
                }
                catch
                {
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "VOTable|*.xml";
            saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".xml";
            saveDialog.FileName = table.LoadFilename;
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                table.Save(saveDialog.FileName);
            }
        }


        public void LabelClicked(int index)
        {
            listView1.Items[index].Selected = true;
            listView1.EnsureVisible(index);
        }

        private void sync_Click(object sender, EventArgs e)
        {

            Uri path = new Uri(table.LoadFilename);

            if (table.SampId == null)
            {
                table.SampId = "WWT:" + Math.Abs(path.ToString().GetHashCode32()).ToString();
            }
            Earth3d.MainWindow.sampConnection.LoadTable(path.ToString(), table.SampId, path.ToString());
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            VoColumn col = table.Columns[listView1.Columns[e.Column].Text];

            UiTools.ShowMessageBox("Name = " + col.Name + "; ucd=" + col.Ucd + "; type=" + col.Type.ToString());
        }


 
        private void markerTypeCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (layer != null)
            {
                layer.PlotType = (TimeSeriesLayer.PlotTypes)markerTypeCombo.SelectedIndex;
                layer.CleanUp();
            }
        }

        private void raSource_SelectionChanged(object sender, EventArgs e)
        {
            layer.LngColumn = raSource.SelectedIndex - 1;
            layer.CleanUp();
        }

        private void decSource_SelectionChanged(object sender, EventArgs e)
        {
            layer.LatColumn = decSource.SelectedIndex - 1;
            layer.CleanUp();
        }

        private void distanceSource_SelectionChanged(object sender, EventArgs e)
        {
            layer.AltColumn = distanceSource.SelectedIndex - 1;
            layer.AltUnit = AltUnits.LightYears;
            layer.AltType = TimeSeriesLayer.AltTypes.Distance;
            layer.CleanUp();
        }

        private void typeSource_SelectionChanged(object sender, EventArgs e)
        {
            layer.MarkerColumn = typeSource.SelectedIndex - 1;
            layer.CleanUp();
        }

        private void sizeSource_SelectionChanged(object sender, EventArgs e)
        {
            layer.SizeColumn = sizeSource.SelectedIndex - 1;
            layer.CleanUp();
        }

        private void VOTableViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            layer.Viewer = null;
            layer = null;
            table = null;
        }

        private void loadImage_Click(object sender, EventArgs e)
        {
            if (table.SelectedRow != null)
            {
                if (table.GetColumnByUcd("VOX:Image.AccessReference") != null)
                {
                    string colName = table.GetColumnByUcd("VOX:Image.AccessReference").Name;
                    string url = table.SelectedRow[colName].ToString();
                    Earth3d.MainWindow.DownloadFitsImage(url);
                }
            }
        }

        private void listView1_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            VoRow row = e.Item.Tag as VoRow;

            row.Selected = e.Item.Checked;
        }


    }
    //enum PlotTypes { None, Sky, Stars3D, Galaxies3D};
    //enum MarkerTypes { Circle, Cross, Asterisk, Point, Square, Triangle };
}