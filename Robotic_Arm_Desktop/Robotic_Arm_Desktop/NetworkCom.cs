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

    public class SendPosition
    {
        NetworkCom NetMove;
        Movemend moveData;

        struct positions
        {
            public int BaseRotation;
            public int Elb0;
            public int Elb1;
            public int Elb2;
            public int GripperRot;
            public int Gripper;
        }

        positions old;
        positions actual;

        public SendPosition( NetworkCom network, Movemend movemend )
        {
            NetMove = network;
            moveData = movemend;

            old.BaseRotation = Convert.ToInt32( Math.Round(moveData.baseMovemend.AngleInPWM));
            old.Elb0 = Convert.ToInt32(Math.Round(moveData.elbow0.AngleInPWM));
            old.Elb1 = Convert.ToInt32(Math.Round(moveData.elbow1.AngleInPWM));
            old.Elb2 = Convert.ToInt32(Math.Round(moveData.elbow2.AngleInPWM));
            old.GripperRot = Convert.ToInt32(Math.Round(moveData.griperRotation.AngleInPWM));
            old.Gripper = Convert.ToInt32(Math.Round(moveData.griper.AngleInPWM));

            actual = old;
        }

        public void AnalyzeAndSend()
        {
            actual.BaseRotation = Convert.ToInt32(Math.Round(moveData.baseMovemend.AngleInPWM));
            actual.Elb0 = Convert.ToInt32(Math.Round(moveData.elbow0.AngleInPWM));
            actual.Elb1 = Convert.ToInt32(Math.Round(moveData.elbow1.AngleInPWM));
            actual.Elb2 = Convert.ToInt32(Math.Round(moveData.elbow2.AngleInPWM));
            actual.GripperRot = Convert.ToInt32(Math.Round(moveData.griperRotation.AngleInPWM));
            actual.Gripper = Convert.ToInt32(Math.Round(moveData.griper.AngleInPWM));

            if (actual.BaseRotation != old.BaseRotation){
                NetMove.SendData("0" + actual.BaseRotation.ToString());
            }
            if (actual.Elb0 != old.Elb0){
                NetMove.SendData("1" + actual.Elb0.ToString());
            }
            if (actual.Elb1 != old.Elb1){
                NetMove.SendData("2" + actual.Elb1.ToString());
            }
            if (actual.Elb2 != old.Elb2){
                NetMove.SendData("3" + actual.Elb2.ToString());
            }
            if (actual.GripperRot != old.GripperRot){
                NetMove.SendData("4" + actual.GripperRot.ToString());
            }
            if (actual.Gripper != old.Gripper){
                NetMove.SendData("5" + actual.Gripper.ToString());
            }

            old = actual;
        }
    }

    public class ScriptNetwork
    {
        public string ip = "127.0.0.1";
    }
}
