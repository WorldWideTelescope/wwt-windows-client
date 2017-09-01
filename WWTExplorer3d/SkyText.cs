using System;
using System.Collections.Generic;
#if WINDOWS_UWP
using Color = Windows.UI.Color;
#else
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;
using PointF = System.Drawing.PointF;
using SizeF = System.Drawing.SizeF;
using System.Drawing;
using System.Xml;
#endif


namespace TerraViewer
{

    public class Text3dBatch : IDisposable
    {
        public int Height = 128;
        public Text3dBatch()
        {
        }
        
        public Text3dBatch(int height)
        {
            Height = (int)(height*3f);
        }

        public Text3dBatch(GlyphCache glyphCache)
        {
            Height = glyphCache.Height;

        }

        public List<Text3d> Items = new List<Text3d>();

        public void Add(Text3d newItem)
        {
            Items.Add(newItem);
        }
        int glyphVersion = -1;


        public void Draw(RenderContext11 renderContext, float opacity, Color color)
        {
            if (glyphCache == null || glyphCache.Version > glyphVersion)
            {
                PrepareBatch();
            }


            Color col = Color.FromArgb((byte)(color.A * opacity), (byte)(color.R * opacity), (byte)(color.G * opacity), (byte)(color.B * opacity));


            SimpleGeometryShader11.Color = col;

            SharpDX.Matrix mat = (renderContext.World * renderContext.View * renderContext.Projection).Matrix11;
            mat.Transpose();

            SimpleGeometryShader11.WVPMatrix = mat;


            renderContext.setRasterizerState(TriangleCullMode.CullCounterClockwise);

            BlendMode bm = renderContext.BlendMode;
            DepthStencilMode dm = renderContext.DepthStencilMode;
            //renderContext.DepthStencilMode = DepthStencilMode.ZReadOnly;
            renderContext.BlendMode = BlendMode.Alpha;

            SimpleGeometryShader11.Use(renderContext.devContext);

            renderContext.MainTexture = glyphCache.Texture;
            // Debug to get Glyhph textures and caches
    //        SharpDX.Direct3D11.Texture2D.ToFile(renderContext.devContext, glyphCache.Texture.Texture, SharpDX.Direct3D11.ImageFileFormat.Png, "c:\\temp\\texture2.png");

            renderContext.devContext.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
 
            renderContext.SetVertexBuffer(vertexBuffer);

            renderContext.devContext.Draw(vertexBuffer.Count, 0);

            renderContext.DepthStencilMode = dm;

            renderContext.BlendMode = bm;

            //dump cache

  //          glyphCache.SaveToXML("c:\\temp\\glyphCache.xml");

        }
 
        GlyphCache glyphCache;

        TextObject TextObject = new TextObject();
        PositionColorTexturedVertexBuffer11 vertexBuffer;
        public void PrepareBatch()
        {
            if (glyphCache == null)
            {
                glyphCache = GlyphCache.GetCache(Height);
            }
            // Add All Glyphs

            foreach (Text3d t3d in Items)
            {
                foreach (char c in t3d.Text)
                {
                    glyphCache.AddGlyph(c);
                }
            }

            // Calculate Metrics

            TextObject.Text = "";
            TextObject.FontSize = (float)Height*.50f;

            // Create Index Buffers

            List<PositionColoredTextured> verts = new List<PositionColoredTextured>();
            foreach (Text3d t3d in Items)
            {
                String text = t3d.Text;
                float left = 0;
                float top = 0;
                float fntAdjust = TextObject.FontSize / 128f;
                float factor = .6666f;
                float width = 0;
                float height = 0;
                for (int i = 0; i < text.Length; i++)
                {
                    GlyphItem item = glyphCache.GetGlyphItem(text[i]);
                    if (item != null)
                    {
                        width += (float)(item.Extents.Width);
                        height = Math.Max(item.Extents.Height, height);
                    }
                }

                Vector2d size = new Vector2d(width, height);

                t3d.width = size.X * (float)t3d.scale * factor * fntAdjust;
                t3d.height = size.Y * (float)t3d.scale * factor * fntAdjust;


                int charsLeft = text.Length;

                for (int i = 0; i < charsLeft; i++)
                {
                    GlyphItem item = glyphCache.GetGlyphItem(text[i]);
                    if (item != null)
                    {
                        RectangleF position = new RectangleF(left * (float)t3d.scale * factor, 0 * (float)t3d.scale * factor, item.Extents.Width * fntAdjust * (float)t3d.scale * factor, item.Extents.Height * fntAdjust * (float)t3d.scale * factor);
                        left += (float)(item.Extents.Width * fntAdjust);

                        //System.Diagnostics.Debug.WriteLine((position.Width/position1.Width).ToString() + ", " + (position.Height / position1.Height).ToString());

                        t3d.AddGlyphPoints(verts, item.Size, position, item.UVRect);
                    }
                }
            }



            vertCount = verts.Count;
            vertexBuffer = new PositionColorTexturedVertexBuffer11(vertCount, RenderContext11.PrepDevice);

            PositionColoredTextured[] vertBuf = (PositionColoredTextured[])vertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

            for (int i = 0; i < vertCount; i++)
            {
                vertBuf[i] = verts[i];
            }

            vertexBuffer.Unlock();

            glyphVersion = glyphCache.Version;
        }

        //public void PrepareBatchOld()
        //{
        //    if (glyphCache == null)
        //    {
        //        glyphCache = GlyphCache.GetCache(Height);
        //    }
        //    // Add All Glyphs

        //    foreach (Text3d t3d in Items)
        //    {
        //        foreach (char c in t3d.Text)
        //        {
        //            glyphCache.AddGlyph(c);
        //        }
        //    }

        //    // Calculate Metrics

        //    TextObject.Text = "";
        //    TextObject.FontSize = (float)Height * .50f;

        //    System.Drawing.Font font = TextObject.Font;
        //    StringFormat sf = new StringFormat();
        //    sf.Alignment = StringAlignment.Near;

        //    Bitmap bmp = new Bitmap(20, 20);
        //    Graphics g = Graphics.FromImage(bmp);
        //    // Create Index Buffers

        //    List<PositionColoredTextured> verts = new List<PositionColoredTextured>();
        //    foreach (Text3d t3d in Items)
        //    {
        //        float fntAdjust = font.Size / 128f;
        //        String text = t3d.Text;
        //        SizeF size = g.MeasureString(text, font);

        //        float factor = .6666f;
        //        t3d.width = size.Width * (float)t3d.scale * factor;
        //        t3d.height = size.Height * (float)t3d.scale * factor;
        //        float left = 0;

        //        int charsLeft = text.Length;
        //        int index = 0;
        //        // SetMeasurableCharacterRanges has a limit of 32 items per call;
        //        while (charsLeft > 0)
        //        {
        //            int charsNow = Math.Min(32, charsLeft);
        //            charsLeft -= charsNow;

        //            CharacterRange[] ranges = new CharacterRange[charsNow];
        //            for (int i = 0; i < charsNow; i++)
        //            {
        //                ranges[i] = new CharacterRange(i + index, 1);
        //            }

        //            sf.SetMeasurableCharacterRanges(ranges);

        //            Region[] reg = g.MeasureCharacterRanges(text, font, new RectangleF(new PointF(0, 0), size), sf);




        //            for (int i = 0; i < (charsNow); i++)
        //            {
        //                GlyphItem item = glyphCache.GetGlyphItem(text[i + index]);
        //                RectangleF rectf = reg[i].GetBounds(g);
        //                RectangleF position = new RectangleF(rectf.Left * (float)t3d.scale * factor, rectf.Top * (float)t3d.scale * factor, rectf.Width * (float)t3d.scale * factor, rectf.Height * (float)t3d.scale * factor);

        //                position = new RectangleF(left * (float)t3d.scale * factor, 0 * (float)t3d.scale * factor, item.Extents.Width * fntAdjust * (float)t3d.scale * factor, item.Extents.Height * fntAdjust * (float)t3d.scale * factor);
        //                left += item.Extents.Width * fntAdjust;
        //                t3d.AddGlyphPoints(verts, item.Size, position, item.UVRect);
        //            }

        //            index += charsNow;
        //        }
        //    }

        //    g.Dispose();
        //    GC.SuppressFinalize(g);
        //    bmp.Dispose();
        //    font.Dispose();

        //    vertCount = verts.Count;
        //    vertexBuffer = new PositionColorTexturedVertexBuffer11(vertCount, RenderContext11.PrepDevice);

        //    PositionColoredTextured[] vertBuf = (PositionColoredTextured[])vertexBuffer.Lock(0, 0); // Lock the buffer (which will return our structs)

        //    for (int i = 0; i < vertCount; i++)
        //    {
        //        vertBuf[i] = verts[i];
        //    }

        //    vertexBuffer.Unlock();

        //    glyphVersion = glyphCache.Version;
        //}

        int vertCount = 0;

        public void CleanUp()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                vertexBuffer = null;
            }
            if (glyphCache != null)
            {
                glyphCache.CleanUp();
                glyphCache = null;
            }
           
            Items.Clear();
        }

        

        #region IDisposable Members

        public void Dispose()
        {
            if (vertexBuffer != null)
            {
                vertexBuffer.Dispose();
                GC.SuppressFinalize(vertexBuffer);
                vertexBuffer = null;
            }

            if (glyphCache != null)
            {
                glyphCache.CleanUp();
                glyphCache = null;
            }
        }

        #endregion

    }


    public class GlyphItem
    {
        public GlyphItem()
        {
        }

        public GlyphItem(char glyph)
        {
            Glyph = glyph;
            UVRect = new RectangleF();
            Size = new SizeF();
            ReferenceCount = 1;
        }

        public GlyphItem(char glyph, RectangleF uv, SizeF size)
        {
            Glyph = glyph;
            UVRect = uv;
            Size = size;
            ReferenceCount = 1;
        }

        public void AddRef()
        {
            ReferenceCount++;
        }

        public void Release()
        {
            ReferenceCount--;
        }
#if !WINDOWS_UWP
        public void SaveToXml(XmlWriter xmlWriter)
        {

            xmlWriter.WriteStartElement("GlyphItem");
            xmlWriter.WriteAttributeString("Glyph", new String(new char[] {Glyph}));
            xmlWriter.WriteAttributeString("UVTop", UVRect.Top.ToString());
            xmlWriter.WriteAttributeString("UVLeft", UVRect.Left.ToString());
            xmlWriter.WriteAttributeString("UVWidth", UVRect.Width.ToString());
            xmlWriter.WriteAttributeString("UVHeight", UVRect.Height.ToString());
            xmlWriter.WriteAttributeString("SizeWidth", Size.Width.ToString());
            xmlWriter.WriteAttributeString("SizeHeight", Size.Height.ToString());
            xmlWriter.WriteAttributeString("ExtentsWidth", Extents.Width.ToString());
            xmlWriter.WriteAttributeString("ExtentsHeight", Extents.Height.ToString());
            xmlWriter.WriteEndElement();
        }
#endif
        internal static GlyphItem FromXML(XmlNode node)
        {
            char glyph = node.Attributes.GetNamedItem("Glyph").Value[0];

            GlyphItem item = new GlyphItem(glyph);
            item.UVRect = new  RectangleF(
                float.Parse(node.Attributes.GetNamedItem("UVLeft").Value),
                float.Parse(node.Attributes.GetNamedItem("UVTop").Value),
                float.Parse(node.Attributes.GetNamedItem("UVWidth").Value),
                float.Parse(node.Attributes.GetNamedItem("UVHeight").Value)
                );

            item.Size = new SizeF(
                float.Parse(node.Attributes.GetNamedItem("SizeWidth").Value),
                float.Parse(node.Attributes.GetNamedItem("SizeHeight").Value));

            item.Extents = new SizeF(
              float.Parse(node.Attributes.GetNamedItem("ExtentsWidth").Value),
              float.Parse(node.Attributes.GetNamedItem("ExtentsHeight").Value));

            return item;
        }

        public char Glyph;
        public RectangleF UVRect;
        public SizeF Size;
        public SizeF Extents;
        public int ReferenceCount = 0;
        
    }


    public class GlyphCache : IDisposable
    {
        static Dictionary<int, GlyphCache> caches = new Dictionary<int, GlyphCache>();

        static public GlyphCache GetCache(int height)
        {
            if (!caches.ContainsKey(height))
            {
                caches[height] = new GlyphCache(height);
            }
            return caches[height];
        }

        static public void CleanUpAll()
        {
            foreach(GlyphCache cache in caches.Values)
            {
                cache.CleanUp();
            }

            caches.Clear();
        }

        Texture11 texture;

        int cellHeight = 128;

        public int Height
        {
            get
            {
                return cellHeight;
            }
        }
        int gridSize = 8;

        public GlyphCache()
        {

        }

        private GlyphCache(int height)
        {
            cellHeight = height;
#if WINDOWS_UWP
            Init();
        }

        public async void Init()
        {

            string testureFilename = System.IO.Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "Assets/glyphs1.png");
            texture = Texture11.FromFile(testureFilename);
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            var file = await folder.GetFileAsync("glyphs1.xml");
            var doc = await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file);
            LoadXmlGlyph(doc);
        }

        private void LoadXmlGlyph(Windows.Data.Xml.Dom.XmlDocument xml)
        {
            XmlNode nodes = xml.GetChildByName("GlyphItems");

            foreach (XmlNode glyphItem in nodes.ChildNodes)
            {
                if ((string)glyphItem.LocalName == "GlyphItem")
                {
                    GlyphItem item = GlyphItem.FromXML(glyphItem);

                    GlyphItems[item.Glyph] = item;
                    allGlyphs = allGlyphs + item.Glyph;
                }
            }
            Ready = true;
#endif
        }

        public Texture11 Texture
        {
            get
            {
                if (dirty)
                {
                    CalculateGlyphDetails();
                }

                if (textureDirty)
                {
                    MakeTexture();
                }

                return texture;
            }
        }

        private void MakeTexture()
        {
            CalcOrMake(true);
        }

        Dictionary<char, GlyphItem> GlyphItems = new Dictionary<char, GlyphItem>();

        public GlyphItem GetGlyphItem(char glyph)
        {
            if (dirty)
            {
                CalculateGlyphDetails();
            }
            return GlyphItems[glyph];
        }

        public TextObject TextObject = new TextObject();

        private void CalculateGlyphDetails()
        {
            CalcOrMake(false);
        }

        private void CalcOrMake(bool makeTexture)
        {
#if !WINDOWS_UWP
            gridSize = 1;

            while ((gridSize * gridSize) < GlyphItems.Count)
            {
                gridSize *= 2;
            }

            int cellSize = 2;

            while (cellSize < cellHeight)
            {
                cellSize *= 2;
            }
            cellHeight = cellSize;

            int textureSize = cellHeight * gridSize;

            TextObject.Text = "";
            TextObject.FontSize = (float)cellHeight * .50f;


            System.Drawing.Font font = TextObject.Font;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;

            Bitmap bmp;
            if (makeTexture)
            {
                bmp = new Bitmap(textureSize, textureSize);
            }
            else
            {
                bmp = new Bitmap(20, 20);
            }

            Graphics g = Graphics.FromImage(bmp);

            int count = 0;

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;


            CharacterRange[] ranges = new CharacterRange[1];
            ranges[0] = new CharacterRange(0, 1);

            sf.SetMeasurableCharacterRanges(ranges);



            foreach (GlyphItem item in GlyphItems.Values)
            {
                int x = (int)(count % gridSize) * cellHeight;
                int y = (int)(count / gridSize) * cellHeight;
                string text = new string(item.Glyph, 1);
                item.Size = g.MeasureString(text, font);
                Region[] reg = g.MeasureCharacterRanges(text, font, new RectangleF(new PointF(0, 0), item.Size), sf);
                RectangleF rectf = reg[0].GetBounds(g);
                item.Extents = new SizeF(rectf.Width, rectf.Height);

                if (item.Extents.Width == 0)
                {
                    item.Extents = item.Size;
                }

                float div = textureSize;
                item.UVRect = new RectangleF(x / div, y / div, item.Size.Width / div, item.Size.Height / div);
                item.UVRect = new RectangleF((x + rectf.X) / div, (y + rectf.Y) / div, rectf.Width / div, rectf.Height / div);
                if (makeTexture)
                {
                    g.DrawString(text, font, Brushes.White, x, y, sf);
                }
                count++;
            }

            g.Dispose();
            GC.SuppressFinalize(g);
            if (makeTexture)
            {
                if (texture != null)
                {
                    texture.Dispose();
                    GC.SuppressFinalize(texture);
                    texture = null;
                }
                texture = Texture11.FromBitmap(RenderContext11.PrepDevice, bmp);
                textureDirty = false;
             }
            else
            {
                textureDirty = true;
            }
            bmp.Dispose();
            GC.SuppressFinalize(bmp);
            dirty = false;
#endif
        }



        bool dirty = true;
        bool textureDirty = true;
        int version = 0;

        public int Version
        {
            get { return version; }
            set { version = value; }
        }

        static string allGlyphs = "";

        public void AddGlyph(char glyph)
        {
            if (!GlyphItems.ContainsKey(glyph))
            {
                GlyphItem item = new GlyphItem(glyph);
                GlyphItems.Add(glyph, item);
                dirty = true;
                textureDirty = true;
                version++;
                allGlyphs = allGlyphs + new string(glyph,1);
            }
            else
            {
                GlyphItems[glyph].AddRef();
            }
        }

#if !WINDOWS_UWP
        public void SaveToXML(string filename)
        {

            XmlWriter xmlWriter = XmlWriter.Create(filename);
            xmlWriter.WriteStartElement("GlyphItems");
            foreach (GlyphItem item in GlyphItems.Values)
            {
                item.SaveToXml(xmlWriter);
            }
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }
#endif
        public bool Ready = false;

      

        public void CleanUp()
        {
            if (texture != null)
            {
                texture.Dispose();
            }
            dirty = true;
            texture = null;
        }



#region IDisposable Members

        public void Dispose()
        {
            CleanUp();
        }

#endregion
     
        public bool Dirty
        {
            get
            {
                return dirty;
            }
            set
            {
                dirty = value;
            }
        }
    }

    public class Text3d 
    {
        public Text3d()
        {

        }


        public Text3d(Vector3d center, Vector3d up, string text, float fontsize, double scale)
        {
            Text = text;
            this.up = up;
            this.center = center;
            this.scale = scale;
            if (fontsize < 0)
            {
                sky = false;
            }
        }

        public double Rotation = 0;
        public double Tilt = 0;
        public double Bank = 0;
        Matrix3d rtbMat;
        bool matInit = false;

        public Color Color = SystemColors.White;
        public bool sky = true;
        public Vector3d center;
        public Vector3d up;
        public double scale;
        public float Opacity = 1.0f;
        public string Text = "";

        public double width = 1;
        public double height = 1;

        public enum Alignment
        {
            Center,
            Left
        }

        public Alignment alignment = Alignment.Center;

        public void AddGlyphPoints(List<PositionColoredTextured> pointList, SizeF size, RectangleF position, RectangleF uv)
        {
            PositionColoredTextured[] points = new PositionColoredTextured[6];

            Vector3d left = Vector3d.Cross(center, up);
            Vector3d right = Vector3d.Cross(up, center);

            left.Normalize();
            right.Normalize();
            up.Normalize();

            Vector3d upTan = Vector3d.Cross(center, right);

            upTan.Normalize();

            if (alignment == Alignment.Center)
            {
                left.Multiply(width - position.Left * 2);
                right.Multiply(width - ((width * 2) - position.Right * 2));
            }
            else if (alignment == Alignment.Left)
            {
                left.Multiply(-position.Left * 2);
                right.Multiply(position.Right * 2);
            }

            Vector3d top = upTan;
            Vector3d bottom = -upTan;
            top.Multiply(height-position.Top*2);
            bottom.Multiply(height-((height*2)-position.Bottom*2));
            Vector3d ul = center;
            ul.Add(top);
            if (sky)
            {
                ul.Add(left);
            }
            else
            {
                ul.Subtract(left);
            }
            Vector3d ur = center;
            ur.Add(top);
            if (sky)
            {
                ur.Add(right);
            }
            else
            {
                ur.Subtract(right);
            }
            Vector3d ll = center;
            if (sky)
            {
                ll.Add(left);
            }
            else
            {
                ll.Subtract(left);
            }

            ll.Add(bottom);

            Vector3d lr = center;
            if (sky)
            {
                lr.Add(right);
            }
            else
            {
                lr.Subtract(right);
            }
            lr.Add(bottom);

            points[0].Pos3d = ul;
            points[0].Tu = uv.Left;
            points[0].Tv = uv.Top;
            points[0].Color = Color;

            points[2].Tu = uv.Left;
            points[2].Tv = uv.Bottom;
            points[2].Pos3d = ll;
            points[2].Color = Color;
        
            points[1].Tu = uv.Right;
            points[1].Tv = uv.Top;
            points[1].Pos3d = ur;
            points[1].Color = Color;
      
            points[3].Tu = uv.Right;
            points[3].Tv = uv.Bottom;
            points[3].Pos3d = lr;
            points[3].Color = Color;

            points[5].Tu = uv.Right;
            points[5].Tv = uv.Top;
            points[5].Pos3d = ur;
            points[5].Color = Color;

            points[4].Tu = uv.Left;
            points[4].Tv = uv.Bottom;
            points[4].Pos3d = ll;
            points[4].Color = Color;

            if (Rotation != 0 || Tilt != 0 || Bank != 0)
            {
                if (!matInit)
                {
                    Matrix3d lookAt = Matrix3d.LookAtLH(center, new Vector3d(0, 0, 0), up);
                    Matrix3d lookAtInv = lookAt;
                    lookAtInv.Invert();

                    rtbMat = lookAt * Matrix3d.RotationZ(-Rotation / 180 * Math.PI) * Matrix3d.RotationX(-Tilt / 180 * Math.PI) * Matrix3d.RotationY(-Bank / 180 * Math.PI) * lookAtInv;
                    //todo make this true after debug
                    matInit = false;
                }
                for (int i = 0; i < 6; i++)
                {
                    Vector3d pos = points[i].Pos3d;
                    pos.TransformCoordinate(rtbMat);
                    points[i].Pos3d = pos;
                }
            }

            pointList.AddRange(points);
        }

    }

}
