using System;
using System.Collections.Generic;
using System.IO;
#if !WINDOWS_UWP
using System.Drawing;
#endif


namespace TerraViewer
{
    public abstract class WcsImage 
    {
        public static WcsImage FromFile(string fileName)
        {
#if !WINDOWS_UWP
            string extension = Path.GetExtension(fileName);

            switch (extension.ToLower())
            {
                case ".fits":
                case ".fit":
                case ".gz":

                    return new FitsImage(fileName);
                default:
                    return new VampWCSImageReader(fileName);
                
            }
#else
            return null;
#endif
        }

        protected string copyright;

        public string Copyright
        {
          get { return copyright; }
          set { copyright = value; }
        }
        protected string creditsUrl;

        public string CreditsUrl
        {
            get { return creditsUrl; }
            set { creditsUrl = value; }
        }

        private bool validWcs = false;

        public bool ValidWcs
        {
            get { return validWcs; }
            set { validWcs = value; }
        }
        protected List<string> keywords = new List<string>();

        public List<string> Keywords
        {
            get {
                if (keywords.Count == 0)
                {
                    keywords.Add("Image File");
                }
                return keywords;
            }
            set { keywords = value; }
        }
        protected string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        protected double scaleX;

        public double ScaleX
        {
            get { return scaleX; }
            set { scaleX = value; }
        }
        protected double scaleY;

        public double ScaleY
        {
            get { return scaleY; }
            set { scaleY = value; }
        }
        protected double centerX;

        public double CenterX
        {
            get { return centerX; }
            set { centerX = value; }
        }
        protected double centerY;

        public double CenterY
        {
            get { return centerY; }
            set { centerY = value; }
        }
        protected double rotation;

        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        protected double referenceX;

        public double ReferenceX
        {
            get { return referenceX; }
            set { referenceX = value; }
        }
        protected double referenceY;

        public double ReferenceY
        {
            get { return referenceY; }
            set { referenceY = value; }
        }
        protected double sizeX;

        public double SizeX
        {
            get { return sizeX; }
            set { sizeX = value; }
        }
        protected double sizeY;

        public double SizeY
        {
            get { return sizeY; }
            set { sizeY = value; }
        }

        protected double cd1_1;

        public double Cd1_1
        {
            get { return cd1_1; }
            set { cd1_1 = value; }
        }
        protected double cd1_2;

        public double Cd1_2
        {
            get { return cd1_2; }
            set { cd1_2 = value; }
        }
        protected double cd2_1;

        public double Cd2_1
        {
            get { return cd2_1; }
            set { cd2_1 = value; }
        }
        protected double cd2_2;

        public double Cd2_2
        {
            get { return cd2_2; }
            set { cd2_2 = value; }
        }
        protected bool hasRotation = false;
        protected bool hasSize = false;
        protected bool hasScale = false;
        protected bool hasLocation = false;
        protected bool hasPixel = false;

        public void AdjustScale(double width, double height)
        {
            //adjusts the actual height vs the reported height.
            if (width != sizeX)
            {
                scaleX *= ( sizeX / width);
                referenceX /= ( sizeX / width);
                sizeX = width;
            }

            if (height != sizeY)
            {
                scaleY *= (sizeY / height);
                referenceY /= (sizeY / height);
                sizeY = height;
            }
        }

        protected void CalculateScaleFromCD()
        {
            scaleX = Math.Sqrt(cd1_1 * cd1_1 + cd2_1 * cd2_1) * Math.Sign(cd1_1 * cd2_2 - cd1_2 * cd2_1);
            scaleY = Math.Sqrt(cd1_2 * cd1_2 + cd2_2 * cd2_2);
        }

        protected void CalculateRotationFromCD()
        {
            double sign = Math.Sign(cd1_1 * cd2_2 - cd1_2 * cd2_1);
            double rot2 = Math.Atan2((-sign * cd1_2), cd2_2);

            rotation = rot2 / Math.PI * 180;
        }
        protected string filename;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private Color color = Color.White;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }


        private bool colorCombine = false;

        public bool ColorCombine
        {
            get { return colorCombine; }
            set { colorCombine = value; }
        }


        abstract public Bitmap GetBitmap();
    }
}
