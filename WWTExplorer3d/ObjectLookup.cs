using System;
using TerraViewer.fr.u_strasbg.cdsws;

using System.Xml;
namespace TerraViewer
{
    class ObjectLookup
    {
        public AstroObjectResult SkyLookup(string target)
        {
            var service = new SesameService();

            var resultString = service.sesame(target, "x");
            return ParseXml(resultString);
        }


        private AstroObjectResult ParseXml(string data)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(data);

                var list = new AstroObjectResult[1];
                list[0] = new AstroObjectResult();

                XmlNode root = doc["Sesame"];
                var resolver = root.SelectSingleNode("Target/Resolver");
                foreach (XmlNode child in resolver.ChildNodes)
                {
                    switch (child.Name)
                    {
                        case "jradeg":
                            list[0].RA = Convert.ToDouble(child.InnerText) / 360 * 24;
                            break;
                        case "jdedeg":
                            list[0].Dec = Convert.ToDouble(child.InnerText);
                            break;
                        case "oname":
                            list[0].Name = child.InnerText;
                            break;
                    }
                }
                if (list[0].RA == 0 && list[0].Dec == 0)
                {
                    return null;
                }

                return list[0];
            }
            catch
            {
                return null;
            }
        }
    }

    public class AstroObjectResult
    {
        public double RA = 0.0;
        public double Dec = 0.0;
        public string Name = "";
        public double zoom = 0.0;
        public string type = "";
        public string[] aliases = null;
    }
}
