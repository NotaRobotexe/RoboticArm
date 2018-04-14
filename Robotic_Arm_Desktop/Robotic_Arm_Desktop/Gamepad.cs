using System;
using System.Runtime.InteropServices;
using SharpDX.DirectInput;
using System.Threading;

namespace Robotic_Arm_Desktop
{
    class Gamepad
    {
        public static bool gamepadConnected = false;
        static Joystick joystick;
        static GamepadState NewData;


        public static void GamepadInit()
        {
            Thread thread = new Thread(new ThreadStart(FindAndSetupGamepad));
            thread.Start();
        }

        private static void FindAndSetupGamepad()
        {
            while (true)
            {
                DirectInput directInput;
                Guid joystickGuid;
                directInput = new DirectInput();
                joystickGuid = Guid.Empty;
                SetDefault();

                while (joystickGuid == Guid.Empty)
                {
                    foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
                        joystickGuid = deviceInstance.InstanceGuid;

                    if (joystickGuid == Guid.Empty)
                        foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
                            joystickGuid = deviceInstance.InstanceGuid;

                    Thread.Sleep(1000);
                }
                gamepadConnected = true;

                joystick = new Joystick(directInput, joystickGuid); //joystick was found
                joystick.Properties.BufferSize = 128;

                joystick.Acquire(); // Acquire the joystick

                ReceiveData();
                gamepadConnected = false;
            }
        }

        private static void ReceiveData()
        {
            while (true)
            {
                joystick.Poll();
                JoystickUpdate[] datas = null;

                try
                {
                    datas = joystick.GetBufferedData();
                }
                catch
                {
                    break;
                }

                if (datas.Length > 0)
                {
                    switch (datas[0].Offset)
                    {
                        case JoystickOffset.X:
                            NewData.x = datas[0].Value;
                            break;
                        case JoystickOffset.Y:
                            NewData.y = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons0:
                            NewData.button0 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons1:
                            NewData.button1 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons2:
                            NewData.button2 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons3:
                            NewData.button3 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons4:
                            NewData.button4 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons5:
                            NewData.button5 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons6:
                            NewData.button6 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons7:
                            NewData.button7 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons8:
                            NewData.button8 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons9:
                            NewData.button9 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons10:
                            NewData.button10 = datas[0].Value;
                            break;
                        case JoystickOffset.Buttons11:
                            NewData.button11 = datas[0].Value;
                            break;
                        default:
                            break;
                    }

                    Global.gamepadState = NewData;
                }
            }
        }

        private static void SetDefault()
        {
            NewData.button0 = 0;
            NewData.button1 = 0;
            NewData.button2 = 0;
            NewData.button3 = 0;
            NewData.button4 = 0;
            NewData.button5 = 0;
            NewData.button6 = 0;
            NewData.button7 = 0;
            NewData.button8 = 0;
            NewData.button9 = 0;
            NewData.button10 = 0;
            NewData.button11 = 0;
            NewData.x = 32511;
            NewData.y = 32511;
        }
    }


    public struct GamepadState
    {
        public int x;
        public int y;
        public int button0;
        public int button1;
        public int button2;
        public int button3;
        public int button4;
        public int button5;
        public int button6;
        public int button7;
        public int button8;
        public int button9;
        public int button10;
        public int button11;

    };
}
