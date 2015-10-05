using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class DataWizardMarkers : PropPage
    {
        public DataWizardMarkers()
        {
            InitializeComponent();
            SetUiStrings();
        }
        private void SetUiStrings()
        {
            selectMarkerLabel.Text = Language.GetLocalizedText(848, "Select a Marker");
            label5.Text = Language.GetLocalizedText(641, "Marker Type");
            label1.Text = Language.GetLocalizedText(849, "Marker Mix");
            label2.Text = Language.GetLocalizedText(850, "Marker Column");
            label3.Text = Language.GetLocalizedText(851, "Select the way you want to organize markers and their shapes. You can use one marker type for all the data or select a column to show different markers for ranges of values, or discrete values. ");
            ShowFarSide.Text = Language.GetLocalizedText(852, "Show Far Side Markers");
        }
        TimeSeriesLayer layer;

        public override void SetData(object data)
        {

            layer = data as TimeSeriesLayer;
        }

        public override bool Save()
        {
            layer.PlotType = (TimeSeriesLayer.PlotTypes)markerType.SelectedIndex;
            layer.MarkerMix = (TimeSeriesLayer.MarkerMixes)markerMix.SelectedIndex;
            return true;
        }

        private void DataWizardMarkers_Load(object sender, EventArgs e)
        {
            markerColumn.Items.Add(Language.GetLocalizedText(832, "None"));
            markerColumn.Items.AddRange(layer.Header);
            markerColumn.SelectedIndex = layer.MarkerColumn + 1; 
   
            markerType.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.PlotTypes)));
            markerType.SelectedIndex = (int)layer.PlotType;
            
            markerMix.Items.AddRange(Enum.GetNames(typeof(TimeSeriesLayer.MarkerMixes)));
            markerMix.SelectedIndex = (int)layer.MarkerMix;

            ShowFarSide.Checked = layer.ShowFarSide;

            if (layer.MarkerIndex > -1)
            {
                markerSelect.Image = PushPin.GetPushPinBitmap(layer.MarkerIndex);
            }
        }



        private void markerType_SelectionChanged(object sender, EventArgs e)
        {
            layer.PlotType = (TimeSeriesLayer.PlotTypes)markerType.SelectedIndex;

            markerSelect.Visible = ((TimeSeriesLayer.PlotTypes)markerType.SelectedIndex) == TimeSeriesLayer.PlotTypes.PushPin;
            selectMarkerLabel.Visible = markerSelect.Visible;
        }



        private void markerSelect_Click(object sender, EventArgs e)
        {

            var popup = new PushPinPickerPopup();

            popup.Location = PointToScreen(markerSelect.Location);
            popup.Top = popup.Top + 34;

            popup.ShowDialog();

            layer.MarkerIndex = popup.SelectedIndex;
            markerSelect.Image = PushPin.GetPushPinBitmap(layer.MarkerIndex);
        }

        private void listBox1_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 36;
            e.ItemWidth = 256;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var value = (DomainValue)domainList.Items[e.Index];
            e.DrawBackground();
            e.Graphics.DrawString(value.Text, UiTools.StandardRegular, UiTools.StadardTextBrush, new PointF(e.Bounds.X +2, e.Bounds.Y +8));
            PushPin.DrawAt(e.Graphics, value.MarkerIndex, e.Bounds.X + 220, e.Bounds.Y + 2);
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

        private void markerMix_SelectionChanged(object sender, EventArgs e)
        {

            if ((TimeSeriesLayer.MarkerMixes)markerMix.SelectedIndex != TimeSeriesLayer.MarkerMixes.Same_For_All && (markerColumn.SelectedIndex > 0))
            {
                layer.PlotType = TimeSeriesLayer.PlotTypes.PushPin;
                markerType.SelectedIndex = (int)layer.PlotType;
                markerType.Enabled = false;
                ShowDomainList(true);
            }
            else
            {
                markerType.Enabled = true;
                ShowDomainList(false);
            }

        }

        private void ShowDomainList(bool visible)
        {
            domainList.Visible = visible;
            domainList.Items.Clear();
            if (markerColumn.SelectedIndex > 0)
            {
                if (layer.MarkerColumn != markerColumn.SelectedIndex - 1)
                {
                    layer.MarkerColumn = markerColumn.SelectedIndex - 1;
                    var domainValues = layer.GetDomainValues(layer.MarkerColumn);
                    layer.MarkerDomainValues.Clear();
                    var index = 0;
                    foreach (var text in domainValues)
                    {
                        layer.MarkerDomainValues.Add(text, new DomainValue(text, index++));
                    }
                }
                foreach (var val in layer.MarkerDomainValues.Values)
                {
                    domainList.Items.Add(val);
                }
            }

        }

        private void markerColumn_SelectionChanged(object sender, EventArgs e)
        {
            if (layer.MarkerColumn != markerColumn.SelectedIndex - 1)
            {
                ShowDomainList(true);
            }
        }

        private void domainList_DoubleClick(object sender, EventArgs e)
        {
            var val = (DomainValue)domainList.SelectedItem;
            var popup = new PushPinPickerPopup();

            popup.Location = Cursor.Position;

            popup.SelectedIndex = val.MarkerIndex;
            popup.ShowDialog();

            val.MarkerIndex = popup.SelectedIndex;
            domainList.Refresh();
        }

        private void domainList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ShowFarSide_CheckedChanged(object sender, EventArgs e)
        {
            layer.ShowFarSide = ShowFarSide.Checked;
        }




    }
   

}
