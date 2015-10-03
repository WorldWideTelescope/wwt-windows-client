using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardColorMap : PropPage
    {
        public DataWizardColorMap()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            this.ColorMapTypeLabel.Text = Language.GetLocalizedText(834, "Color Map Type");
            this.label1.Text = Language.GetLocalizedText(835, "Color Map Column");
            this.label2.Text = Language.GetLocalizedText(833, "A color map allows you to vary the color of a marker based on the value of a a selected column using colors for discrete domain values, selecting colors to represent ranges, or using gradients to map a range of colors smoothly to a range of values.");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.ColorMap = (TimeSeriesLayer.ColorMaps)ColorMapType.SelectedIndex ;
            layer.ColorMapColumn = ColorMapColumn.SelectedIndex - 1;
            return true;

        }

        private void DataWizardColorMap_Load(object sender, EventArgs e)
        {
            ColorMapColumn.Items.Add(Language.GetLocalizedText(832, "None"));
            ColorMapColumn.Items.AddRange(layer.Header);
            ColorMapColumn.SelectedIndex = layer.ColorMapColumn + 1;

            ColorMapType.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.ColorMaps)));
            ColorMapType.SelectedIndex = (int)layer.ColorMap;
        }

        private void ColorMapColumn_SelectionChanged(object sender, EventArgs e)
        {
            ShowDomainList(ColorMapColumn.SelectedIndex > 0);

        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 36;
            e.ItemWidth = 256;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                var value = (DomainValue)domainList.Items[e.Index];
                e.DrawBackground();
                e.Graphics.DrawString(value.Text, UiTools.StandardRegular, UiTools.StadardTextBrush, new PointF(e.Bounds.X + 2, e.Bounds.Y + 8));
                Brush backGround = new SolidBrush(Color.FromArgb(value.MarkerIndex));
                e.Graphics.FillRectangle(backGround, new Rectangle(e.Bounds.X + 220, e.Bounds.Y + 2, 34, 34));
                backGround.Dispose();
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    var rect = e.Bounds;
                    rect.Inflate(-1, -1);
                    e.Graphics.DrawRectangle(Pens.Yellow, rect);
                }
                if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
                {
                    e.DrawFocusRectangle();
                }
            }
        }

       

        private void ShowDomainList(bool visible)
        {
            domainList.Visible = visible;
            domainList.Items.Clear();
            if (ColorMapType.SelectedIndex > 0)
            {
                if (layer.ColorMapColumn != ColorMapColumn.SelectedIndex - 1)
                {
                    layer.ColorMapColumn = ColorMapColumn.SelectedIndex - 1;
                    layer.MakeColorDomainValues();
                }
                foreach (var val in layer.ColorDomainValues.Values)
                {
                    domainList.Items.Add(val);
                }
            }

        }

       

        private void ColorMapType_SelectionChanged(object sender, EventArgs e)
        {
            layer.ColorMap = (TimeSeriesLayer.ColorMaps)ColorMapType.SelectedIndex;
            if ((TimeSeriesLayer.ColorMaps)ColorMapType.SelectedIndex != TimeSeriesLayer.ColorMaps.Same_For_All )
            {
                ColorMapColumn.Enabled = true;
                ShowDomainList(ColorMapColumn.SelectedIndex > 0);
            }
            else
            {
                ColorMapColumn.Enabled = false;
                ShowDomainList(false);
            }

        }

        private void domainList_SelectedIndexChanged(object sender, EventArgs e)
        {

            this.Refresh();
        }

        private void domainList_DoubleClick(object sender, EventArgs e)
        {
            var val = (DomainValue)domainList.SelectedItem;

            var picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = Color.FromArgb(val.MarkerIndex);

            if (picker.ShowDialog() == DialogResult.OK)
            {
                val.MarkerIndex = picker.Color.ToArgb();
            }

            domainList.Refresh();
        }
    }
}
