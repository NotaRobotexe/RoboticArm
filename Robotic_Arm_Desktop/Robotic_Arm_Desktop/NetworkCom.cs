using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Robotic_Arm_Desktop
{
    public class NetworkCom
    {
        private Socket socket;

        public int InitCom( int port)
        {
            if (Global.connected == true)
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(IPAddress.Parse(Global.ipaddres), port);
                    return 0;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return -1;
                }
            }
            return -1;
        }

        public void SendData(string s)
        {
            if (Global.connected == true)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);

                Task.Run(() =>
                {
                    socket.Send(buffer);
                }
                );
            }
        }

        public async Task<string> ReceiveData()
        {
            if (Global.connected == true)
            {
                byte[] buffer = new byte[100];

                return await Task.Run(() =>
                {
                    socket.Receive(buffer);
                    string message = Encoding.UTF8.GetString(buffer);
                    return message;
                }
                );
            }
            return "";
        }

        /*

        public static async void CheckTrigger()
        {
            SendData("2");

            string trigger = await ReceiveData();
            if (!(trigger.Any(char.IsDigit)))
            {
                if (trigger == "false")
                {
                    Global.triggered = false;
                }
                else
                {
                    Global.triggered = true;
                }
            }

        }

        public static void StartMovemend()
        {
            SendData("3");
        }

        public static void StopMovemend()
        {
            SendData("4");
        }

        public static async void GetData()
        {
            SendData("5");
            string data = await ReceiveData();
            Stats.getData(data);
        }

        public static void Move(String position, String mode)
        {
            SendData("6" + mode + position);
        }

        public static void SetFanSpeed()
        {
            SendData("8");
        }*/
    }

    public static class Move
    {

    }
}
