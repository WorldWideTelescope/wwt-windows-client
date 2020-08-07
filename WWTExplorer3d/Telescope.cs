namespace TerraViewer
{
    public class Telescope
    {
        public Telescope(string manufacturer, string name, double focalLength, double aperture, string url, string mountType, string opticalDesign)
        {
            this.Manufacturer = manufacturer;
            this.Name = name;
            this.FocalLength = focalLength;
            this.Aperture = aperture;
            this.FRatio = focalLength/aperture;
            this.Url = url;
            this.MountType = mountType;
            this.OpticalDesign = opticalDesign;   
        }

        public override string ToString()
        {
            return Name;
        }

        public string Manufacturer;
        public string Name;
        public double FocalLength;
        public double Aperture;
        public double FRatio;
        public string Url;
        public string MountType;
        public string OpticalDesign;
    }
}
