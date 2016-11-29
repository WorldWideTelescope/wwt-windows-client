using System;
using System.IO;
using System.Net.NetworkInformation;

namespace TerraViewer
{
    public enum ActionType { Quiz };
    public class Action
    {
        public string Id;
        public ActionType Type;
        public string Filename;
        public int Points;

        public Action() { }

        public Action(string id, ActionType type, string filename, int points)
        {
            Id = id;
            Type = type;
            Filename = filename;
            Points = points;
        }

        public override string ToString()
        {
            return Filename;
        }
 
        internal void SaveToXml(System.Xml.XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Action");
            xmlWriter.WriteAttributeString("Filename", this.Filename);
            xmlWriter.WriteAttributeString("Id", this.Id);
            xmlWriter.WriteAttributeString("Points", this.Points.ToString());
            xmlWriter.WriteAttributeString("Type", this.Type.ToString());

            xmlWriter.WriteEndElement();
        }

        public void Execute(int slide)
        {
            if (this.Filename != null)
            {
                string strPath = Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\wwt_data";
                System.IO.Directory.CreateDirectory(strPath);
                strPath += "\\" + this.Filename;
                FileStream fs = new FileStream(strPath, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(fs);
                var mac = this.GetMacAddress();
                if (sw.BaseStream.Length == 0)
                {
                    var cols = "computer,time,slide,id,points";
                    sw.WriteLine(cols);
                    sw.Flush();
                }
                else
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                }
                var line = string.Format("{0},{1},{2},{3},{4}", mac, DateTime.Now.ToString("MM/dd/yyyy h:mm tt"), slide, this.Id, this.Points);
                sw.WriteLine(line);
                sw.Flush();
                sw.Close();
            }
        }

        private string GetMacAddress()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    mac += nic.GetPhysicalAddress().ToString();
                    break;
                }
            }
            return mac;
        }

        internal static Action FromXml(System.Xml.XmlNode node)
        {
            Action newAction = new Action();
            newAction.Type = ActionType.Quiz; // default to the only type we have for now  
            if (node.Attributes["Type"] != null && Enum.IsDefined(typeof(ActionType), node.Attributes["Type"].Value))
            {
                newAction.Type = (ActionType)(Enum.Parse(typeof(ActionType), node.Attributes["Type"].Value));
            }
            newAction.Id = node.Attributes["Id"].Value;
            newAction.Filename = node.Attributes["Filename"].Value;
            newAction.Points = Convert.ToInt16(node.Attributes["Points"].Value);
            return newAction;
        }
    }
}
