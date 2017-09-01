namespace TerraViewer
{
    public class Imager
    {
        public Imager()
        {
        }

        public Imager(int Id, string Type, double Width, double Height, double HorizontalPixels, double VerticalPixels, double CenterX, double CenterY, double Rotation, string Filter)
        {
            this.Id = Id;
            this.Type = Type;
            this.Width = Width;
            this.Height = Height;
            this.HorizontalPixels = HorizontalPixels;
            this.VerticalPixels = VerticalPixels;
            this.CenterX = CenterX;
            this.CenterY = CenterY;
            this.Rotation = Rotation;
            this.Filter = Filter;
        }

        /// <summary>
        /// Identifier for Chip
        /// </summary>
        public int Id;
        /// <summary>
        /// Imager technology: Film, CCD, CMOS
        /// </summary>
        public string Type;
        /// <summary>
        /// Width in mm
        /// </summary> 
        public double Width;
        /// <summary>
        /// Height in mm
        /// </summary>
        public double Height;
        /// <summary>
        /// Width in Pixels
        /// </summary>
        public double HorizontalPixels;
        /// <summary>
        /// Height in Pixels
        /// </summary>
        public double VerticalPixels;
        /// <summary>
        /// Offset from optical axis in mm North is +
        /// </summary>
        public double CenterX;
        /// <summary>
        /// Offset from optical axis in mm East is +
        /// </summary>
        public double CenterY;
        /// <summary>
        /// Rotatin Clockwise zero is north.
        /// </summary>
        public double Rotation;
        /// <summary>
        /// Perminent filter. None, Bayer, H-Aplha, etc
        /// </summary>
        public string Filter;
    }
}
