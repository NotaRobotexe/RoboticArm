using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    public class NetworkCom
    {
        private Socket socket;

        public int InitCom(int port)
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
    }

    public class SendPosition
    {
        private NetworkCom NetMove;
        private Movement moveData;

        private struct positions
        {
            public int BaseRotation;
            public int Elb0;
            public int Elb1;
            public int Elb2;
            public int GripperRot;
            public int Gripper;
        }

        private positions old;
        private positions actual;

        public SendPosition(NetworkCom network, Movement movement)
        {
            NetMove = network;
            moveData = movement;

            old.BaseRotation = Convert.ToInt32(Math.Round(moveData.baseMovemend.AngleInPWM));
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

            if (actual.BaseRotation != old.BaseRotation)
            {
                NetMove.SendData("0" + actual.BaseRotation.ToString());
            }
            if (actual.Elb0 != old.Elb0)
            {
                NetMove.SendData("1" + actual.Elb0.ToString());
            }
            if (actual.Elb1 != old.Elb1)
            {
                NetMove.SendData("2" + actual.Elb1.ToString());
            }
            if (actual.Elb2 != old.Elb2)
            {
                NetMove.SendData("3" + actual.Elb2.ToString());
            }
            if (actual.GripperRot != old.GripperRot)
            {
                NetMove.SendData("4" + actual.GripperRot.ToString());
            }
            if (actual.Gripper != old.Gripper)
            {
                NetMove.SendData("5" + actual.Gripper.ToString());
            }

            old = actual;
        }
    }

    public class ScriptNetwork
    {
        public event EventHandler NewOutput;
        public event EventHandler StraightLine;
        public event EventHandler DrawTargets;

        private Movement movement;
        private Socket socket;
        public bool ScriptRunning = false;
        private bool connected = false;

        public int InitCom(string ip, Movement m)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            movement = m;
            try
            {
                socket.Connect(IPAddress.Parse(ip), 6972);
                connected = true;
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }

        private void SendData(string s)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);

            Task.Run(() =>
            {
                socket.Send(buffer);
            }
            );
        }

        private string ReceiveData()
        {
            byte[] buffer = new byte[100];

            try
            {
                socket.Receive(buffer);
                string message = Encoding.UTF8.GetString(buffer);
                return message;
            }
            catch
            {
                return "";
            }
        }

        public async void Communication()
        {

            if (connected == true)
            {

                await Task.Run(() =>
                {
                    while (ScriptRunning)
                    {
                        string msg_raw = ReceiveData();
                        if (msg_raw == ""){
                            break;
                        }
                        string msg = msg_raw.Substring(0, msg_raw.IndexOf('\0'));

                        if (msg == ""){
                            break;
                        }

                        byte ScriptMessage = Convert.ToByte(msg.Substring(0, 1));

                        switch (ScriptMessage)
                        {
                            case 1:
                                SendTriggerStatus();
                                break;

                            case 2:
                                OnStraightLine(EventArgs.Empty);
                                break;

                            case 3:
                                AutoModeTemplate.ScriptDefaultMovemend(msg, movement);
                                SendACK();
                                break;

                            case 4:
                                MovingStatus();
                                break;

                            case 5:
                                SetMovSpeed(msg);
                                break;

                            case 6:
                                SetOutput(msg);
                                OnNewOutput(EventArgs.Empty);
                                break;

                            case 7:
                                SendArmPosition();
                                break;

                            case 8:
                                Global.ScriptTargets = msg;
                                OnDrawTargets(EventArgs.Empty);
                                break;

                            default:
                                break;
                        }
                    }
                }
                );
            }
        }

        protected virtual void OnDrawTargets(EventArgs e)
        {
            EventHandler eventHandler = NewOutput;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        private void SendArmPosition()
        {
            string pos = Math.Round(movement.baseMovemend.AngleInDegree,2).ToString() + "*" + Math.Round(movement.elbow0.AngleInDegree, 2).ToString() + "*" + Math.Round(movement.elbow1.AngleInDegree, 2).ToString() +
            "*" + Math.Round(movement.elbow2.AngleInDegree, 2).ToString()+ "*"+ Math.Round(movement.griperRotation.AngleInDegree, 2).ToString() + "*" + Math.Round(movement.griper.AngleInDegree, 2).ToString();
            SendData(pos);
        }

        private void SetOutput(string msg)
        {
            string oldString = Global.ScriptOutput;
            Global.ScriptOutput = msg.Substring(1)+ "\r\n" + oldString;
        } 
            
        protected virtual void OnNewOutput(EventArgs e)
        {
            EventHandler eventHandler = NewOutput;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        protected virtual void OnStraightLine(EventArgs e)
        {
            EventHandler eventHandler = NewOutput;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

        private void SendTriggerStatus()
        {
            if (Global.triggered == true){
                SendData("1");
            }
            else{
                SendData("0");
            }
        }

        private void SetMovSpeed(string msg)
        {
            int time = Convert.ToInt32(msg);
            Global.MovingSpeed = time;
            SendACK();
        }

        public void InputMessaging(string msg)
        {
            SendData(msg);
        }

        private void MovingStatus()
        {
            if (Global.IsMoving == false)
            {
                SendData("0");
            }
            else
            {
                SendData("1");
            }
        }

        public void SendACK()
        {
            SendData("1");
        }

        public void EndCom()
        {
            socket.Close();
        }
    }

    public class RemoteNetwork
    {
        private Movement movement;
        private Socket socket;
        private bool connected = false;

        public int InitCom(string ip, Movement m)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            movement = m;
            try
            {
                socket.Connect(IPAddress.Parse(ip), 6973);
                connected = true;
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return -1;
            }
        }

        public string Acknowlage()
        {
            byte[] buffer = new byte[10];

            socket.Receive(buffer);
            string message = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(message);
            return message;
        }

        public void UploadScript(string s)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(s);

            Task.Run(() =>
            {
                socket.Send(buffer);
            }
            );
        }

        public void QuitScript()
        {
            byte[] buffer = Encoding.ASCII.GetBytes("END");

            Task.Run(() =>
            {
                socket.Send(buffer);
            }
            );
        }

        public void EndCom()
        {
            socket.Close();
        }
    }
}