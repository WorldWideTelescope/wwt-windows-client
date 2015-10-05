using System;
using System.Drawing;
using System.Windows.Forms;

namespace TerraViewer
{
    public partial class TextEditor : Form
    {
        public TextEditor()
        {
            InitializeComponent();
            SetUiStrings();
        }

        private void SetUiStrings()
        {
            Text = Language.GetLocalizedText(404, "Text Editor");
            SaveButton.Text = Language.GetLocalizedText(168, "Save");
            FontSize.ToolTipText = Language.GetLocalizedText(405, "Font Size");
            FontBold.Text = Language.GetLocalizedText(406, "Bold");
            FontBold.ToolTipText = Language.GetLocalizedText(406, "Bold");
            FontItalic.Text = Language.GetLocalizedText(407, "Italic");
            FontColor.Text = Language.GetLocalizedText(408, "Text Color");
            BackgroundColor.Text = Language.GetLocalizedText(409, "Background Preview Color");
            BackgroundStyle.Text = Language.GetLocalizedText(410, "Text Background Options");
            BackgroundStyle.ToolTipText = Language.GetLocalizedText(411, "Backgound Options");
            noBackgroundToolStripMenuItem.Text = Language.GetLocalizedText(412, "No Background");
            tightFitBackgroundToolStripMenuItem.Text = Language.GetLocalizedText(413, "Tight Fit Background");
            smallBorderToolStripMenuItem.Text = Language.GetLocalizedText(414, "Small Border");
            largerBoarderToolStripMenuItem.Text = Language.GetLocalizedText(415, "Larger Border");
        }
        static public TextObject DefaultTextobject = new TextObject("", false, false, false, 24, "Arial", Color.White, Color.Black, TextBorderStyle.None);


        private void TextEditor_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(TextObject.Text))
            {
                TextObject = DefaultTextobject;
            }

            var i = 0;
            var selectedIndex = 0;
            foreach (var oneFontFamily in FontFamily.Families)
            {
                FontName.Items.Add(oneFontFamily.Name);
                if (TextObject.FontName == oneFontFamily.Name)
                {
                    selectedIndex = i;
                }
                i++;
            }

            FontName.SelectedIndex = selectedIndex;

            FontSize.Text = TextObject.FontSize.ToString();

            FontBold.Checked = TextObject.Bold;
            FontItalic.Checked = TextObject.Italic;
            borderStyle = TextObject.BorderStyle;

            textBox.ForeColor = UiTools.RgbOnlyColor(TextObject.ForegroundColor);
            textBox.BackColor = UiTools.RgbOnlyColor(TextObject.BackgroundColor);

            TextBackgroundColor = TextObject.BackgroundColor;
            TextForegroundColor = TextObject.ForegroundColor;
            textBox.Text = TextObject.Text;

            initialized = true;
            UpdateTextCharacteristics();
        }

        public TextObject TextObject;

        TextBorderStyle borderStyle = TextBorderStyle.None;

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            float fontSize = (int)textBox.Font.Size;
            try
            {
                fontSize = Convert.ToSingle(FontSize.Text);
            }
            catch
            {
            }

            TextObject = new TextObject(textBox.Text, FontBold.Checked, FontItalic.Checked, false, fontSize, FontName.SelectedItem.ToString(), TextForegroundColor, TextBackgroundColor, borderStyle);
            DefaultTextobject = TextObject;
            DefaultTextobject.Text = "";
            Close();
        }
        bool initialized;
        private void UpdateTextCharacteristics()
        {
            if (initialized)
            {
                var style = (FontBold.Checked ? FontStyle.Bold : FontStyle.Regular) | (FontItalic.Checked ? FontStyle.Italic : FontStyle.Regular);
                float fontSize = (int)textBox.Font.Size;
                try
                {
                    fontSize = Convert.ToSingle(FontSize.Text);
                }
                catch
                {
                }

                var font = new Font((string)FontName.SelectedItem, fontSize, style);

                textBox.Font = font;

                textBox.TextAlign = HorizontalAlignment.Left;
            }
        }

        private void FontName_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();

        }

        private void FontSize_TextChanged(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }

        private void FontBold_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }

        private void FontItalic_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }

        private void FontUndeline_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }
        Color TextForegroundColor = Color.Black;

        private void FontColor_Click(object sender, EventArgs e)
        {
            var picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = TextForegroundColor;

            if (picker.ShowDialog() == DialogResult.OK)
            {
                TextForegroundColor = picker.Color;
                textBox.ForeColor = Color.FromArgb(TextForegroundColor.R, TextForegroundColor.G, TextForegroundColor.B);
            }
        }
        Color TextBackgroundColor = Color.Black;
        private void BackgroundColor_Click(object sender, EventArgs e)
        {
            var picker = new PopupColorPicker();

            picker.Location = Cursor.Position;

            picker.Color = TextBackgroundColor;
            if (picker.ShowDialog() == DialogResult.OK)
            {
                TextBackgroundColor = picker.Color;
                textBox.BackColor = Color.FromArgb(TextBackgroundColor.R, TextBackgroundColor.G, TextBackgroundColor.B);

            }

        }



        private void ToolBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void FontItalic_Click(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }

        private void FontBold_Click(object sender, EventArgs e)
        {
            UpdateTextCharacteristics();
        }

        private void noBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            borderStyle = TextBorderStyle.None;
        }

        private void tightFitBackgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            borderStyle = TextBorderStyle.Tight;

        }

        private void smallBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            borderStyle = TextBorderStyle.Small;

        }

        private void largerBoarderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            borderStyle = TextBorderStyle.Medium;
        }

        private void BackgroundStyle_Click(object sender, EventArgs e)
        {
            noBackgroundToolStripMenuItem.Checked = borderStyle == TextBorderStyle.None;
            tightFitBackgroundToolStripMenuItem.Checked = borderStyle == TextBorderStyle.Tight;
            smallBorderToolStripMenuItem.Checked = borderStyle == TextBorderStyle.Small;
            largerBoarderToolStripMenuItem.Checked = borderStyle == TextBorderStyle.Medium;
        }

        private void dateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$DATE}");
        }

        private void timeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$TIME}");

        }

        private void distanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$DIST}");

        }

        private void latitudeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$LAT}");

        }

        private void longitudeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$LNG}");

        }

        private void rAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$RA}");

        }

        private void decToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$DEC}");

        }

        private void fieldOfViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox.Paste("{$FOV}");
        }
    }
}