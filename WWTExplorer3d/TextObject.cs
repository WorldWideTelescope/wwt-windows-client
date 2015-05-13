using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TerraViewer
{
    public enum TextBorderStyle { None, Tight, Small, Medium, Large };
    public struct TextObject
    {
        
        public TextObject(string text, bool bold, bool italic, bool underline, float fontSize, string fontName, Color forgroundColor, Color backgroundColor , TextBorderStyle borderStyle)
        {

            Text = text;
            Bold = bold;
            Italic = italic;
            Underline = underline;
            FontSize = fontSize;
            FontName = fontName;
            ForegroundColor = forgroundColor;
            BackgroundColor = backgroundColor;
            BorderStyle = borderStyle;
        }

        public string Text;
        public bool Bold;
        public bool Italic;
        public bool Underline;
        public float FontSize;
        public string FontName;
        public Color ForegroundColor;
        public Color BackgroundColor;
        public TextBorderStyle BorderStyle;

        public Font Font
        {
            get
            {
                FontStyle style = (Bold ? FontStyle.Bold : FontStyle.Regular) | (Italic ? FontStyle.Italic : FontStyle.Regular );

                return new Font(FontName, FontSize, style);


            }
        }



        public override string ToString()
        {
            return Text;
        }

        internal void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TextObject");
            xmlWriter.WriteAttributeString("Bold", Bold.ToString());
            xmlWriter.WriteAttributeString("Italic", Italic.ToString());
            xmlWriter.WriteAttributeString("Underline", Underline.ToString());
            xmlWriter.WriteAttributeString("FontSize", FontSize.ToString());
            xmlWriter.WriteAttributeString("FontName", FontName);
            xmlWriter.WriteAttributeString("ForgroundColor", SavedColor.Save(ForegroundColor));
            xmlWriter.WriteAttributeString("BackgroundColor", SavedColor.Save(BackgroundColor));
            xmlWriter.WriteAttributeString("BorderStyle", BorderStyle.ToString());

            xmlWriter.WriteString(this.Text);
            xmlWriter.WriteEndElement();
        }

        internal static TextObject FromXml(System.Xml.XmlNode node)
        {
            TextObject newTextObject = new TextObject();
            newTextObject.Text = node.InnerText;
            newTextObject.BorderStyle = TextBorderStyle.None;
            newTextObject.Bold = Convert.ToBoolean(node.Attributes["Bold"].Value);
            newTextObject.Italic = Convert.ToBoolean(node.Attributes["Italic"].Value);
            newTextObject.Underline = Convert.ToBoolean(node.Attributes["Underline"].Value);
            newTextObject.FontSize = Convert.ToSingle(node.Attributes["FontSize"].Value);
            newTextObject.FontName = node.Attributes["FontName"].Value;
            newTextObject.ForegroundColor = SavedColor.Load(node.Attributes["ForgroundColor"].Value);
            newTextObject.BackgroundColor = SavedColor.Load(node.Attributes["BackgroundColor"].Value);
            if (node.Attributes["BorderStyle"] != null)
            {
                newTextObject.BorderStyle = (TextBorderStyle)(Enum.Parse(typeof(TextBorderStyle), node.Attributes["BorderStyle"].Value));
            }
            return newTextObject;
        }
    }
}
