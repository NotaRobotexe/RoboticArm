using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    static class  Stats
    {
        static public string ping = "";
        static public string CPUload = "";
        static public string Temperature = "";

        static public void GetPingAndTryConnection()
        {
            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                try
                {
                    ping = p.Send(Global.ipaddres).RoundtripTime.ToString() + " ms";
                    Global.connected = true;
                }
                catch (Exception)
                {
                    Global.connected = false;
                }
            }
        }

        static public void getData(string data)
        {
            if (Global.connected == true)
            {
                data = data.Substring(0, data.IndexOf('\0')-1);
                if (!(data.Any(char.IsLetter)))
                {
                    string[] tempAndLoad = data.Split('*');
                    Temperature = tempAndLoad[0];
                    CPUload = tempAndLoad[1];
                }
            }
        }

    }
}
