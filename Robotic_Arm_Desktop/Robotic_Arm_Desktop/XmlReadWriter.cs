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

        private void CreateFile(Movement movement)
        {
            using (XmlWriter writer = XmlWriter.Create("MotorCalibration.xml"))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Motors");

                writer.WriteStartElement("Elbow0");
                writer.WriteElementString("MaxUse", movement.elbow0.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.elbow0.startfrom.ToString());
                writer.WriteElementString("Speed", movement.elbow0.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow1");
                writer.WriteElementString("MaxUse", movement.elbow1.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.elbow1.startfrom.ToString());
                writer.WriteElementString("Speed", movement.elbow1.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Elbow2");
                writer.WriteElementString("MaxUse", movement.elbow2.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.elbow2.startfrom.ToString());
                writer.WriteElementString("Speed", movement.elbow2.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Base");
                writer.WriteElementString("MaxUse", movement.baseMovemend.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.baseMovemend.startfrom.ToString());
                writer.WriteElementString("Speed", movement.baseMovemend.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper0");
                writer.WriteElementString("MaxUse", movement.griperRotation.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.griperRotation.startfrom.ToString());
                writer.WriteElementString("Speed", movement.griperRotation.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteStartElement("Gripper1");
                writer.WriteElementString("MaxUse", movement.griper.EndAt.ToString());
                writer.WriteElementString("StartFrom", movement.griper.startfrom.ToString());
                writer.WriteElementString("Speed", movement.griper.SpeedBoost.ToString());
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public void UpdateFile(Movement movement)
        {
            XmlNode node = doc.SelectSingleNode("Motors/Elbow0/MaxUse");
            node.InnerText = movement.elbow0.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            node.InnerText = movement.elbow0.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Elbow0/Speed");
            node.InnerText = movement.elbow0.SpeedBoost.ToString();

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            node.InnerText = movement.elbow1.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            node.InnerText = movement.elbow1.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Elbow1/Speed");
            node.InnerText = movement.elbow1.SpeedBoost.ToString();

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            node.InnerText = movement.elbow2.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            node.InnerText = movement.elbow2.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Elbow2/Speed");
            node.InnerText = movement.elbow2.SpeedBoost.ToString();

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            node.InnerText = movement.baseMovemend.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            node.InnerText = movement.baseMovemend.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Base/Speed");
            node.InnerText = movement.baseMovemend.SpeedBoost.ToString();

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            node.InnerText = movement.griperRotation.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            node.InnerText = movement.griperRotation.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Gripper0/Speed");
            node.InnerText = movement.griperRotation.SpeedBoost.ToString();

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            node.InnerText = movement.griper.EndAt.ToString();
            node = doc.SelectSingleNode("Motors/Gripper1/StartFrom");
            node.InnerText = movement.griper.startfrom.ToString();
            node = doc.SelectSingleNode("Motors/Gripper1/Speed");
            node.InnerText = movement.griper.SpeedBoost.ToString();

            doc.Save("MotorCalibration.xml");
        }

        public void LoadSettings(Movement movement)
        {
            doc = new XmlDocument();

            try
            {
                doc.Load("MotorCalibration.xml");
                ReadData(movement);
            }
            catch (Exception)
            {
                CreateFile(movement);
                ReadData(movement);
            }

        }

        private void ReadData(Movement movement)
        {
            XmlNode node = doc.SelectSingleNode("Motors/Elbow0/MaxUse");
            movement.elbow0.EndAt = Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow0/StartFrom");
            movement.elbow0.startfrom= Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow0/Speed");
            movement.elbow0.SpeedBoost = Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow1/MaxUse");
            movement.elbow1.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow1/StartFrom");
            movement.elbow1.startfrom=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow1/Speed");
            movement.elbow1.SpeedBoost = Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Elbow2/MaxUse");
            movement.elbow2.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow2/StartFrom");
            movement.elbow2.startfrom=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Elbow2/Speed");
            movement.elbow2.SpeedBoost = Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Base/MaxUse");
            movement.baseMovemend.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Base/StartFrom");
            movement.baseMovemend.startfrom=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Base/Speed");
            movement.baseMovemend.SpeedBoost = Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper0/MaxUse");
            movement.griperRotation.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper0/StartFrom");
            movement.griperRotation.startfrom=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper0/Speed");
            movement.griperRotation.SpeedBoost = Convert.ToDouble(node.InnerText);

            node = doc.SelectSingleNode("Motors/Gripper1/MaxUse");
            movement.griper.EndAt=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper1/StartFrom");
            movement.griper.startfrom=Convert.ToDouble(node.InnerText);
            node = doc.SelectSingleNode("Motors/Gripper1/Speed");
            movement.griper.SpeedBoost = Convert.ToDouble(node.InnerText);
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
