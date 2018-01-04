using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robotic_Arm_Desktop
{
    class XmlReadWriter
    {
        XmlDocument doc; 
        
        // movement settings

        private void CreateFile(Movemend movemend)
        {
            using (XmlWriter writer = XmlWriter.Create("MotorCalibration.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Motors");

                writer.WriteStartElement("Elbow0");
                writer.WriteElementString("MaxUse", movemend.elbow0.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow0.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow1");
                writer.WriteElementString("MaxUse", movemend.elbow1.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow1.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow2");
                writer.WriteElementString("MaxUse", movemend.elbow2.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow2.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Base");
                writer.WriteElementString("MaxUse", movemend.baseMovemend.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.baseMovemend.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper0");
                writer.WriteElementString("MaxUse", movemend.griperRotation.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.griperRotation.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper1");
                writer.WriteElementString("MaxUse", movemend.griper.EndAt.ToString());
                writer.WriteElementString("StartFrom", movemend.griper.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void UpdateFile(Movemend movemend)
        {
            XmlNode node = doc.SelectSingleNode("Motors/Elbow0/MaxUse");
            node.InnerText = movemend.elbow0.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            node.InnerText = movemend.elbow0.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            node.InnerText = movemend.elbow1.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            node.InnerText = movemend.elbow1.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            node.InnerText = movemend.elbow2.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            node.InnerText = movemend.elbow2.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            node.InnerText = movemend.baseMovemend.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            node.InnerText = movemend.baseMovemend.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            node.InnerText = movemend.griperRotation.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            node.InnerText = movemend.griperRotation.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            node.InnerText = movemend.griper.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Gripper1/StartFrom");
            node.InnerText = movemend.griper.startfrom.ToString();

            doc.Save("MotorCalibration.xml");
        }

        public void LoadSettings(Movemend movemend)
        {
            doc = new XmlDocument();

            try
            {
                doc.Load("MotorCalibration.xml");
                ReadData(movemend);
            }
            catch (Exception)
            {
                CreateFile(movemend);
                ReadData(movemend);
            }

        }

        private void ReadData(Movemend movemend)
        {
            XmlNode node = doc.SelectSingleNode("Motors/Elbow0/MaxUse");
            movemend.elbow0.EndAt = Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            movemend.elbow0.startfrom= Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            movemend.elbow1.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            movemend.elbow1.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            movemend.elbow2.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            movemend.elbow2.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            movemend.baseMovemend.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            movemend.baseMovemend.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            movemend.griperRotation.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            movemend.griperRotation.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            movemend.griper.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper1/StartFrom");
            movemend.griper.startfrom=Convert.ToDouble(node.InnerText);
        }

        //template mode

        public void CreateFileTemplate(string name,List<string> list)
        {
            using (XmlWriter writer = XmlWriter.Create("templates/"+name+".xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Template");

                foreach (var command in list)
                {
                    Console.WriteLine(command);
                    writer.WriteElementString("instruction",command);
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public List<string> LoadCommands(string path)
        {
            XDocument xml = XDocument.Load("templates/" + path);

            List<string> Commands = xml.Root.Elements("instruction").Select(element => element.Value).ToList();

            return Commands;
        }

    }
}
