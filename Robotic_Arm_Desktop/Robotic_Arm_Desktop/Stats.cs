using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    class Stats
    {
        string ip;
        bool connected = false;

        public enum data
        {
            ping
        }


        public Stats(string ip)
        {
            this.ip = ip;
        }

        void GetPingAndTryConnection()
        {
            using (System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping())
            {
                //data.ping = "sadasd";
                //latency.Content = p.Send("192.167.1.69").RoundtripTime.ToString() + " ms";
            }
        }

    }
}
