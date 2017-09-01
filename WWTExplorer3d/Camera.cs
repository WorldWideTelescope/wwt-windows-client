using System.Collections.Generic;

namespace TerraViewer
{
    public class Camera
    {
        public Camera( string manufacturer, string name, string url)
        {
            this.Manufacturer = manufacturer;
            this.Name = name;
            this.Url = url;
            Chips = new List<Imager>();
        }
        public override string ToString()
        {
            return Name;
        }
        public List<Imager> Chips;
        public string Manufacturer;
        public string Name;
        public string Url;
    }
}
