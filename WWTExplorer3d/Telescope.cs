namespace TerraViewer
{
    public class Telescope
    {
        public Telescope(string manufacturer, string name, double focalLength, double aperture, string url, string mountType, string opticalDesign)
        {
            Manufacturer = manufacturer;
            Name = name;
            FocalLength = focalLength;
            Aperture = aperture;
            FRatio = focalLength/aperture;
            Url = url;
            MountType = mountType;
            OpticalDesign = opticalDesign;   
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
