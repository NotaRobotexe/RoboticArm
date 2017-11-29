using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Robotic_Arm_Desktop
{
    class XmlReadWriter
    {
        XmlDocument doc;    //for motor call

        private void CreateFile(Movemend movemend)
        {
            using (XmlWriter writer = XmlWriter.Create("MotorCalibration.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Motors");

                writer.WriteStartElement("Elbow0");
                writer.WriteElementString("Angle", movemend.elbow0.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.elbow0.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow0.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow1");
                writer.WriteElementString("Angle", movemend.elbow1.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.elbow1.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow1.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow2");
                writer.WriteElementString("Angle", movemend.elbow2.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.elbow2.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.elbow2.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Base");
                writer.WriteElementString("Angle", movemend.baseMovemend.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.baseMovemend.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.baseMovemend.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper0");
                writer.WriteElementString("Angle", movemend.griperRotation.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.griperRotation.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.griperRotation.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper1");
                writer.WriteElementString("Angle", movemend.griper.maxAngle.ToString());
                writer.WriteElementString("MaxUse", movemend.griper.maxUseAngle.ToString());
                writer.WriteElementString("StartFrom", movemend.griper.startfrom.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void UpdateFile(Movemend movemend)
        {
            XmlNode node = doc.SelectSingleNode("Motors/Elbow0/MaxUse");
            node.InnerText = movemend.elbow0.maxUseAngle.ToString();
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            node.InnerText = movemend.elbow0.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            node.InnerText = movemend.elbow1.maxUseAngle.ToString();
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            node.InnerText = movemend.elbow1.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            node.InnerText = movemend.elbow2.maxUseAngle.ToString();
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            node.InnerText = movemend.elbow2.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            node.InnerText = movemend.baseMovemend.maxUseAngle.ToString();
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            node.InnerText = movemend.baseMovemend.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            node.InnerText = movemend.griperRotation.maxUseAngle.ToString();
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            node.InnerText = movemend.griperRotation.startfrom.ToString();

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            node.InnerText = movemend.griper.maxUseAngle.ToString();
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
            movemend.elbow0.maxUseAngle = Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            movemend.elbow0.startfrom= Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            movemend.elbow1.maxUseAngle=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            movemend.elbow1.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            movemend.elbow2.maxUseAngle=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            movemend.elbow2.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            movemend.baseMovemend.maxUseAngle=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            movemend.baseMovemend.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            movemend.griperRotation.maxUseAngle=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            movemend.griperRotation.startfrom=Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            movemend.griper.maxUseAngle=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper1/StartFrom");
            movemend.griper.startfrom=Convert.ToDouble(node.InnerText);
        }

        //for template mode

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
