using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace TerraViewer
{
    class ImageSetLayer : Layer
    {
        IImageSet imageSet = null;

        ScaleTypes lastScale = ScaleTypes.Linear;
        double    min = 0;
        double    max = 0;

        public IImageSet ImageSet
        {
            get { return imageSet; }
            set { imageSet = value; }
        }
        string extension = "txt";
        public ImageSetLayer()
        {
           
        }

        public ImageSetLayer(IImageSet set)
        {
            imageSet = set;
        }

        bool overrideDefaultLayer = false;
        public bool OverrideDefaultLayer
        {
            get { return overrideDefaultLayer; }
            set { overrideDefaultLayer = value; }
        }

        public FitsImage FitsImage
        {
            get
            {
                if (imageSet.WcsImage == null || !(imageSet.WcsImage is FitsImage))
                {
                    return null;
                }
                return imageSet.WcsImage as FitsImage;
            }
        }

        public override void InitializeFromXml(System.Xml.XmlNode node)
        {
            XmlNode imageSetNode = node["ImageSet"];

            ImageSetHelper ish = ImageSetHelper.FromXMLNode(imageSetNode);

            if (!String.IsNullOrEmpty(ish.Url) && Earth3d.ReplacementImageSets.ContainsKey(ish.Url))
            {
                imageSet = Earth3d.ReplacementImageSets[ish.Url];
            }
            else
            {
                imageSet = ish;
            }

            if (node.Attributes["Extension"] != null)
            {
                extension = node.Attributes["Extension"].Value;
            }

            if (node.Attributes["ScaleType"] != null)
            {
                lastScale = (ScaleTypes) Enum.Parse(typeof(ScaleTypes), node.Attributes["ScaleType"].Value);
            }

            if (node.Attributes["MinValue"] != null)
            {
                min = double.Parse(node.Attributes["MinValue"].Value);
            }

            if (node.Attributes["MaxValue"] != null)
            {
                max = double.Parse(node.Attributes["MaxValue"].Value);
            }

            if (node.Attributes["OverrideDefault"] != null)
            {
                overrideDefaultLayer = bool.Parse(node.Attributes["OverrideDefault"].Value );
            }

        }

        public override bool Draw(RenderContext11 renderContext, float opacity, bool flat)
        {
            if (!flat)
            {
                renderContext.setRasterizerState(TriangleCullMode.CullClockwise);
            }
            renderContext.WorldBase = renderContext.World ;
            renderContext.ViewBase = renderContext.View;
            Earth3d.MainWindow.MakeFrustum();
            renderContext.MakeFrustum();
            Earth3d.MainWindow.PaintLayerFullTint11(imageSet, this.Opacity * opacity * 100, Color);

     
            return true;

        }

        public override void WriteLayerProperties(System.Xml.XmlTextWriter xmlWriter)
        {
            if (imageSet.WcsImage != null)
            {
                xmlWriter.WriteAttributeString("Extension", Path.GetExtension(imageSet.WcsImage.Filename));
            }

            if (imageSet.WcsImage is FitsImage)
            {
                FitsImage fi = imageSet.WcsImage as FitsImage;
                xmlWriter.WriteAttributeString("ScaleType", fi.lastScale.ToString());
                xmlWriter.WriteAttributeString("MinValue", fi.lastBitmapMin.ToString());
                xmlWriter.WriteAttributeString("MaxValue", fi.lastBitmapMax.ToString());
               
            }

            xmlWriter.WriteAttributeString("OverrideDefault", overrideDefaultLayer.ToString());
            
            ImageSetHelper.SaveToXml(xmlWriter, imageSet, "");
            base.WriteLayerProperties(xmlWriter);
        }

        public override void CleanUp()
        {
            base.CleanUp();
        }

        public override void AddFilesToCabinet(FileCabinet fc)
        {
            if (imageSet.WcsImage != null)
            {
                string fName = imageSet.WcsImage.Filename;

                bool copy = !fName.Contains(ID.ToString());
                string extension = Path.GetExtension(fName);

                string fileName = fc.TempDirectory + string.Format("{0}\\{1}{2}", fc.PackageID, this.ID.ToString(), extension);
                string path = fName.Substring(0, fName.LastIndexOf('\\') + 1);

                if (fName != fileName)
                {
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
        }

        public override string[] GetParamNames()
        {
            return base.GetParamNames();
        }

        public override double[] GetParams()
        {
            return base.GetParams();
        }

        public override void SetParams(double[] paramList)
        {
            base.SetParams(paramList);
        }

        public override void LoadData(string path)
        {
            string filename = path.Replace(".txt",  extension);
             if (File.Exists(filename))
             {
                 imageSet.WcsImage = WcsImage.FromFile(filename);
                 if (min != 0 && max != 0)
                 {
                     FitsImage fi = imageSet.WcsImage as FitsImage;
                     fi.lastBitmapMax = max;
                     fi.lastBitmapMin = min;
                     fi.lastScale = lastScale;
                 }
             }
        }
    }
}
