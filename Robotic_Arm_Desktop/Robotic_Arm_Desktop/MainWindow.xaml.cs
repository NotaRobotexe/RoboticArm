﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace Robotic_Arm_Desktop
{
    //TODO: Safety control - taktiez netreba moc
    //TODO: fixnut ostatne buggy
    //TODO: responzivnost - nepotrebne fullHD staci
    //TODO: Spracovanie obrazu a vsetky tie blbosti - hot hot hot hot
    //TODO: spiest sa ako babovka #1 #2 #3
    //TODO: Prestat pridavat TODO

    public partial class MainWindow : Window
    {
        GamepadState gamepadData;
        Movement movement;
        _3Dmodel model;
        Gamepad gamepad;
        XmlReadWriter xrw;
        VideoStream stream;
        BruteForceMovement ForceMovement;
        RemoteNetwork remoteNetwork;

        string ScriptPath = "";

        bool AutoMode = false;
        bool gamepadConnected = false;
        bool hud = false;
        bool loadingDone = false;
        bool CapturingTemplate = false;
        bool WaitForTrigger = false;
        int framerate = 0;

        DispatcherTimer ControllstatusTimer;
        DispatcherTimer FrameRateCounter;
        DispatcherTimer IK_timer;

        List<string> Commands = new List<string>(); //Template command
        Stopwatch stopWatch;
        TimeSpan elapsed;

        SendPosition send_pos;
        NetworkCom netCom;
        NetworkCom netData;
        NetworkCom netMove;
        NetworkCom netTrigger;
        NetworkCom netFan;

        ScriptNetwork scriptCom;
        Process pythone;
        InverseKinematic inverse;

        public MainWindow()
        {

            Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts");
            InitializeComponent();

            if (Global.DebugMode== false)
            {
                Stats.GetPingAndTryConnection();//TODO: nech sa to overi este pri zadavani ip adresi
            }

            movement = new Movement();

            xrw = new XmlReadWriter();
            xrw.LoadSettings(movement);

            this.Loaded += MainWindow_Loaded; //some method need be call after window is loaded like gamepad because it need window handler 
            model = new _3Dmodel();


            InitCom();
            netCom.SendData("1");
            GetCPUandTemp();
            GetTrigger();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //keyboard animation 
            ControllstatusTimer = new DispatcherTimer();
            ControllstatusTimer.Tick += ControllstatusTimer_Tick;
            ControllstatusTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            
            Arm.PositonChange += Arm_PositonChange;
            movement.IncrementationChange += Movemend_IncrementationChange;

            InitializeGamepad();
            helix.Content = model.group;
            HelixViewport3D.Camera.Position = new Point3D(0,75,85);
            HelixViewport3D.Camera.LookDirection = new Vector3D(0, -1, -1);

            //set first value to motor calibration
            MotorCalibrationDisplay();

            //load data to combobox
            LoadFilesToComboBox();

            StatTimersForStatsStuff();

            SetFirstPositionOfModel();
            send_pos = new SendPosition(netMove, movement);

            //start video stream

            stream = new VideoStream();
            SetStreamSettings();
            VideoStream.NewFrame += Stream_NewFrame;

            //framerateCounter
            FrameRateCounter = new DispatcherTimer();
            FrameRateCounter.Tick += FrameRateCounter_Tick;
            FrameRateCounter.Interval = new TimeSpan(0, 0, 1);
            FrameRateCounter.Start();

            buttonAnimationOnOff(HUDbutton, false);

            ForceMovement = new BruteForceMovement(movement, model);

            loadingDone = true;
        }

        /*undone things*/

        private void EnableAutoMode_Click(object sender, RoutedEventArgs e)
        {
            AutoMode = !AutoMode;

            if (AutoMode == true)
            {

                manualModeStatus.Content = "Disabled";
                ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(172, 33, 33));
            }
            else
            {
                manualModeStatus.Content = "Enabled";
                ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(37, 37, 38));
            }

            movement.keyboardenabled = !movement.keyboardenabled;
            buttonAnimationOnOff(keyboardEnalbeButton, movement.keyboardenabled);
            if (gamepad.gamepadConnected == true )
            {
                movement.gamepadEnabled = !movement.gamepadEnabled;
                buttonAnimationOnOff(GamepadEnalbeButton, movement.gamepadEnabled);
            }
        }

        /*video stream and other thing with image control*/
        private async void SetStreamSettings()
        {
            if (loadingDone==true){
                stream.ProcesEnd();
                stream = new VideoStream();
            }

            Global.StreamWidth = Convert.ToInt32(widthL.Text);
            Global.StreamHight = Convert.ToInt32(highL.Text);

            string StreammSetting = "v4l2-ctl --set-ctrl=color_effects="+ Math.Round(ColSlid.Value) +" --set-ctrl=contrast="+ Math.Round(Contrast.Value)+" --set-ctrl=brightness="+ Math.Round(brighness.Value);
            string Slenght = StreammSetting.Length.ToString();
            string _width, _high;

            if (widthL.Text.Length<4){
                _width = "0" + widthL.Text;
            }
            else{
                _width = widthL.Text;
            }
            if (highL.Text.Length < 4){
                _high = "0" + highL.Text;
            }
            else{
                _high = highL.Text;
            }

            string res = _width + _high;
            netCom.SendData("2"+res+ Slenght + StreammSetting);
            await NonBlockSleep();
            stream.Procesinit();
        }

        public async Task NonBlockSleep()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(2000);
            });
        } 

        private void Stream_NewFrame(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
            {
                ViewFrame.Stretch = Stretch.Fill;
                framerate++;
                ViewFrame.Source = Global.Frame;
            });
        }

        private void StreamSetting_Click(object sender, RoutedEventArgs e)
        {
            SetStreamSettings();
        }

        private void FrameRateCounter_Tick(object sender, EventArgs e)
        {
            FrameLab.Content = framerate.ToString() + " fps";
            framerate = 0;
        }
        private void HUDclick(object sender, RoutedEventArgs e)
        {
            hud = !hud;
            buttonAnimationOnOff(HUDbutton, hud);

            if (hud == true)
            {
                hudImage.Visibility = Visibility.Visible;
                hudimage1.Visibility = Visibility.Visible;
            }
            else
            {
                hudImage.Visibility = Visibility.Hidden;
                hudimage1.Visibility = Visibility.Hidden;
            }

        }
        
        /*controls buttons*/
        private async void ExitWin(object sender, RoutedEventArgs e)
        {
            FrameRateCounter.Stop();
            stream.ProcesEnd();
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
            netCom.SendData("7");
            Application.Current.Shutdown();
        }

        private void MinWin(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            List<string> instructionsRaw = Positions.RecoveryPos.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            movement.baseMovemend.Update(instructions[0],1);
            movement.elbow0.Update(instructions[1], 1);
            movement.elbow1.Update(instructions[2], 1);
            movement.elbow2.Update(instructions[3], 1);
            movement.griperRotation.Update(instructions[4], 1);
            movement.griper.Update(instructions[5], 1);

            DrawDataAndUpdateModel();
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.SteadyPos, movement);
            latency.Content = "test";
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
            latency.Content = "test2";

        }

        private async void TurnOffPressed(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
        }

        /*manual mode stuff here*/
        private void OnOffControllStatus() //ManualModeStatusEllipse turning on off
        {
            if (ControllstatusTimer.IsEnabled == false)
            {
                ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(0, 153, 0));

                
                ControllstatusTimer.Start();
            }
        }

        private void ControllstatusTimer_Tick(object sender, EventArgs e)
        {
            ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(37, 37, 38));
            ControllstatusTimer.Stop();
        }

        private void KeyboardOnOff(object sender, RoutedEventArgs e)
        {
            if (AutoMode == false)
            {
                movement.keyboardenabled = !movement.keyboardenabled;
                buttonAnimationOnOff(keyboardEnalbeButton, movement.keyboardenabled);
            }
        }

        private void GamepadOnOff(object sender, RoutedEventArgs e)
        {
            if (gamepad.gamepadConnected == true && AutoMode == false)
            {
                movement.gamepadEnabled = !movement.gamepadEnabled;
                buttonAnimationOnOff(GamepadEnalbeButton, movement.gamepadEnabled);
            }
        }

        void buttonAnimationOnOff(Button button,bool on)
        {
            if(on == true)
            {
                button.Foreground = new SolidColorBrush(Color.FromRgb(241, 241, 241));
                button.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
                button.Content = "Enabled";
            }
            else
            {
                button.Content = "Disabled";
                button.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                button.BorderBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            }
        }

        public bool gamepadStateChange //what to do if gamepad is plugged / unplugged
        {
            set
            {
                if (gamepad.gamepadConnected != value)
                {
                    gamepad.gamepadConnected = value;

                    if (gamepad.gamepadConnected == false)
                    {
                        gamepadLabel.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                        GamepadEnalbeButton.Content = "Disabled";
                        GamepadEnalbeButton.Foreground = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                        GamepadEnalbeButton.BorderBrush = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                    }
                    else
                    {
                        gamepadLabel.Foreground = new SolidColorBrush(Color.FromRgb(241, 241, 241));
                        GamepadEnalbeButton.Content = "Enabled";
                        GamepadEnalbeButton.Foreground = new SolidColorBrush(Color.FromRgb(241, 241, 241));
                        GamepadEnalbeButton.BorderBrush = new SolidColorBrush(Colors.Gainsboro);
                    }
                }
            }
        }

        public static object GC { get; internal set; }

        /*gamepad and keyboard basic input*/
        private void InitializeGamepad()
        {
            var mainHandle = new WindowInteropHelper(this).Handle;

            gamepad = new Gamepad(mainHandle);

            HwndSource source = HwndSource.FromHwnd(mainHandle);
            source.AddHook(new HwndSourceHook(WndProc));
            
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) //gamepad input
        {
            if (movement.gamepadEnabled == true)
            {
                switch (msg)
                {
                    case 0x00ff:
                        {
                            gamepadData = Gamepad.GamepadProcesing(lParam);
                            movement.AnalizeData(gamepadData);
                            gamepadConnected = true;
                        }
                        break;

                    case 0x0219:
                        {
                            gamepadConnected = false;
                        }
                        break;
                }
                gamepadStateChange = gamepadConnected;

                if (Global.WrongMode == true)
                {
                    if (Global.BetterMessageBoxLauched == false)
                    {
                        Global.BetterMessageBoxLauched = true;
                        Global.BetterMessageBoxErrorIndex = 1;
                        BetterPopUpBox BetterMessageBox = new BetterPopUpBox();
                        BetterMessageBox.Show();
                    }
                }

            }
            return IntPtr.Zero;
        }

        private void KeyboardEvent(object sender, KeyEventArgs e)
        {
            if (movement.keyboardenabled == true)
            {
                if (e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3)
                {
                    movement.AnalizeData(e.Key);
                }
                else if (e.Key == Key.D1 && movement.keyboardMovingArm != 0)
                {
                    movement.keyboardMovingArm = 0;
                }
                else if (e.Key == Key.D2 && movement.keyboardMovingArm != 1)
                {
                    movement.keyboardMovingArm = 1;
                }
                else if (e.Key == Key.D3 && movement.keyboardMovingArm != 2)
                {
                    movement.keyboardMovingArm = 2;
                }
            }
        }

        /*Motors calibration and settings */
        int OpenListbox;

        private void ListboxChange(object sender, SelectionChangedEventArgs e)
        {
            if (loadingDone==true)
            {
                MotorCalibrationDisplay();
            }
        }

        private void MotorCalibrationDisplay()
        {
            if (listBox.SelectedIndex == 0)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value= movement.elbow0.EndAt; 
                maxUse.Content=  Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.elbow0.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.elbow0.SpeedBoost.ToString();

                OpenListbox = 0;
            }
            else if(listBox.SelectedIndex == 1)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movement.elbow1.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.elbow1.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.elbow1.SpeedBoost.ToString();

                OpenListbox = 1;
            }
            else if (listBox.SelectedIndex == 2)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movement.elbow2.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.elbow2.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.elbow2.SpeedBoost.ToString();

                OpenListbox = 2;
            }
            else if (listBox.SelectedIndex == 3)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movement.baseMovemend.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.baseMovemend.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.baseMovemend.SpeedBoost.ToString();

                OpenListbox = 3;
            }
            else if (listBox.SelectedIndex == 4)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movement.griperRotation.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.griperRotation.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value), 2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.griperRotation.SpeedBoost.ToString();

                OpenListbox = 4;
            }
            else if (listBox.SelectedIndex == 5)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movement.griper.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movement.griper.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value), 2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";

                speedR.Text = movement.griper.SpeedBoost.ToString();

                OpenListbox = 5;
            }
        }

        private void MaxUseSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingDone == true)
            {
                if (listBox.SelectedIndex == 0)
                {
                    movement.elbow0.EndAt = Math.Round(max.Value,2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";
                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 1)
                {
                    movement.elbow1.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 2)
                {
                    movement.elbow2.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 3)
                {
                    movement.baseMovemend.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 4)
                {
                    movement.griperRotation.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 5)
                {
                    movement.griper.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }

            }
        }

        private void StartFromSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingDone == true)
            {
                if (listBox.SelectedIndex == 0)
                {
                    movement.elbow0.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 1)
                {
                    movement.elbow1.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 2)
                {
                    movement.elbow2.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 3)
                {
                    movement.baseMovemend.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 4)
                {
                    movement.griperRotation.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 5)
                {
                    movement.griper.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible,2).ToString() + " °";
                }
            }
        }

        private void SaveMotorsStats(object sender, RoutedEventArgs e)
        {
            switch (OpenListbox)
            {
                case 0:
                    movement.elbow0.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
                case 1:
                    movement.elbow1.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
                case 2:
                    movement.elbow2.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
                case 3:
                    movement.baseMovemend.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
                case 4:
                    movement.griperRotation.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
                case 5:
                    movement.griper.SpeedBoost = Convert.ToDouble(speedR.Text);
                    break;
            }

            xrw.UpdateFile(movement);
        }

        /*draw position data to the app, update 3d model and send data to arm*/

        private void Arm_PositonChange(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
             {
                 DrawDataAndUpdateModel();
            });

            if (loadingDone == true)
            {
                send_pos.AnalyzeAndSend();
            }
        }

        public void DrawDataAndUpdateModel()
        {
            
            this.baseRa.Content = Math.Round(movement.baseMovemend.AngleInDegree, 2) + " °";
            this.elb0a.Content = Math.Round(movement.elbow0.AngleInDegree, 2) + " °";
            this.elb1a.Content = Math.Round(movement.elbow1.AngleInDegree, 2) + " °";
            this.elb2a.Content = Math.Round(movement.elbow2.AngleInDegree, 2) + " °";
            this.grra.Content = Math.Round(movement.griperRotation.AngleInDegree, 2) + " °";
            this.gr.Content = Math.Round(movement.griper.AngleInDegree, 2) + " °";

            model.UpdateModel(movement);

            if (hud == true)
            {
                RotateTransform rotateTransform = new RotateTransform(movement.griperRotation.AngleInDegree-90);
                hudImage.RenderTransform = rotateTransform;
            }

            if (CapturingTemplate == true)
            {
                modeposition0.Content = baseRa.Content + " " + elb0a.Content + " " + elb1a.Content + " " + elb2a.Content + " " + grra.Content;
            }

            OnOffControllStatus(); //controll status just for effect
        }

        public void SetFirstPositionOfModel()
        {
            List<string> instructionsRaw = Positions.OffPos.Split('*').ToList();
            List<double> instructions = new List<double>();

            for (int i = 0; i < 6; i++)
            {
                instructions.Add(Convert.ToDouble(instructionsRaw[i]));
            }

            movement.baseMovemend.Update(instructions[0], 1);
            movement.elbow0.Update(instructions[1], 1);
            movement.elbow1.Update(instructions[2], 1);
            movement.elbow2.Update(instructions[3], 1);
            movement.griperRotation.Update(instructions[4], 1);
            movement.griper.Update(instructions[5], 1);

            DrawDataAndUpdateModel();
        }

        private void Movemend_IncrementationChange(object sender, EventArgs e)
        {
            this.incLabel.Content = Math.Round(movement.valueCount,3);
        }

        /*bug fixes*/
        bool ValueBeforeTiping1, ValueBeforeTiping2;

        private void DisableMovemendWhenWriting(object sender, RoutedEventArgs e)
        {
            ValueBeforeTiping1 = movement.keyboardenabled;
            ValueBeforeTiping2 = movement.gamepadEnabled;

            movement.keyboardenabled = false;
            movement.gamepadEnabled = false;
        }

        private void EnableMovementdAfterWriting(object sender, RoutedEventArgs e)
        {

            movement.keyboardenabled = ValueBeforeTiping1;
            movement.gamepadEnabled = ValueBeforeTiping2;
        }


        /*template mode*/

        private void CreateNewTemplate(object sender, RoutedEventArgs e)
        {
            modestart0.IsEnabled = false;
            modestop0.IsEnabled = false;
            modecapture0.IsEnabled = true;
            modesave0.IsEnabled = true;
            delayTexBox.IsEnabled = true;
            triggerButton.IsEnabled = true;
            templateSpeed.IsEnabled = true;

            if (tempname.Text != "Template name" && tempname.Text != "")
            {
                Commands.Clear();
                CapturingTemplate = true;
            }
            else
            {
                MessageBox.Show("Incorrect file name");
            }
        }

        private void CapturePressed(object sender, RoutedEventArgs e)
        {
            string command = movement.baseMovemend.AngleInPWM + "*" + movement.elbow0.AngleInPWM + "*" + movement.elbow1.AngleInPWM + "*"
            + movement.elbow2.AngleInPWM + "*" + movement.griperRotation.AngleInPWM + "*" + movement.griper.AngleInPWM + "*"
            + Convert.ToInt16(WaitForTrigger) + "*" + delayTexBox.Text + "*" + templateSpeed.Text;

            Commands.Add(command);
            WaitForTrigger = false;
        }

        private void TrigerState(object sender, RoutedEventArgs e)
        {
            WaitForTrigger = !WaitForTrigger;
        }

        private void SaveTemplate(object sender, RoutedEventArgs e)
        {
            xrw.CreateFileTemplate(tempname.Text,Commands);

            modestart0.IsEnabled = true;
            modestop0.IsEnabled = true;
            modecapture0.IsEnabled = false;
            modesave0.IsEnabled = false;
            delayTexBox.IsEnabled = false;
            triggerButton.IsEnabled = false;
            templateSpeed.IsEnabled = false;

            Commands.Clear();
            LoadFilesToComboBox();
        }

        private void LoadFilesToComboBox()
        {
            List<string> ComboBoxData = new List<string>();
            ComboBoxData = Directory.GetFiles("templates","*.xml").Select(System.IO.Path.GetFileName).ToList();
            templateComboBox.ItemsSource = ComboBoxData;
        }

        private void StartTemplate(object sender, RoutedEventArgs e) 
        {
            Global.stop = false;

            if (templateComboBox.SelectedItem != null)
            {
                Global.autoModeRunning = true;
                Commands = xrw.LoadCommands(templateComboBox.SelectedItem.ToString());
                AutoModeTemplate.StartTemplateAsync(Commands,movement,numOfLoop);
            }
            else
            {
                MessageBox.Show("Select template");
            }
        }

        private void StopMoving(object sender, RoutedEventArgs e)
        {
            Global.stop = true;
            numOfLoop.Text = "1";
        }


        /*comunication and stats*/

        void StatTimersForStatsStuff()
        {
            DispatcherTimer PingTimer = new DispatcherTimer();
            PingTimer.Tick += PingTimer_Tick;
            PingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            PingTimer.Start();

            DispatcherTimer UpTime = new DispatcherTimer();
            UpTime.Tick += UpTime_Tick;
            UpTime.Interval = new TimeSpan(0, 0, 1);
            UpTime.Start();

            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        void InitCom()
        {
            netCom = new NetworkCom();
            netData = new NetworkCom();
            netMove = new NetworkCom(); 
            netTrigger = new NetworkCom() ;
            netFan = new NetworkCom();

            netCom.InitCom(6969);
            netData.InitCom(6968);
            netMove.InitCom(6967);
            netTrigger.InitCom(6966);
            netFan.InitCom(6965);
        }

        async void GetCPUandTemp()
        {
            while (Global.connected == true)
            {
                string data = await netData.ReceiveData();
                Stats.getData(data);

                this.cpuusage.Content = Stats.CPUload + " %";
                this.temperature.Content = Stats.Temperature + " °C";
            }
        }

        async void GetTrigger()
        {
            while (Global.connected == true)
            {
                string trigger = await netTrigger.ReceiveData();
                if (trigger == "false")
                {
                    Global.triggered = false;
                }
                else
                {
                    Global.triggered = true;
                }
                this.trigger.Content = Global.triggered;
            }
        }

        private void UpTime_Tick(object sender, EventArgs e)
        {
            elapsed = stopWatch.Elapsed;
            this.uptime.Content = elapsed.ToString("hh\\:mm\\:ss");
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            if (Global.DebugMode == false)
            {
                Stats.GetPingAndTryConnection();
            }
            this.latency.Content = Stats.ping;
            this.status.Content = Global.connected;
        }

        private void NewFanSpeed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingDone == true)
            {
                int speed = (int)Math.Round(fanSlider.Value);
                netFan.SendData(speed.ToString());
            }
        }

        /*SCRIPT editor*/

        private void OpenScriptEditor(object sender, RoutedEventArgs e)
        {
            SriptEditor Editor = new SriptEditor();
            Editor.Show();
        }

        private void LoadScript(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open a Script";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts";
            openFileDialog.ShowDialog();

            ScriptPath = openFileDialog.FileName;

            if (ScriptPath != "")
            {
                string name = openFileDialog.SafeFileName;
                string name_ = name.Substring(0,name.IndexOf("."));
                scriptName.Content = name_; 
            
                startScript.IsEnabled = true;
            }

        }

        private void RunScript(object sender, RoutedEventArgs e)
        {
            if (ScriptPath != "")
            {

                if (Global.RemoteExc == false)
                {
                    pythone = new Process();
                    pythone.StartInfo.FileName = ScriptPath;
                    pythone.StartInfo.UseShellExecute = true;
                    pythone.StartInfo.RedirectStandardOutput = false;
                    pythone.StartInfo.CreateNoWindow = true;
                    pythone.EnableRaisingEvents = true;
                    pythone.Start();
                    pythone.Exited += Pythone_Exited;


                    Global.ScriptEnabled = true;
                    scriptCom = new ScriptNetwork();
                    scriptCom.InitCom("127.0.0.1",movement);
                    scriptCom.Communication();
                    scriptCom.ScriptRunning = true;
                }
                else //start script on remote computer
                {
                    remoteNetwork = new RemoteNetwork();
                    string ip = targetIp.Text;
                    remoteNetwork.InitCom(ip, movement);

                    string script = File.ReadAllText(ScriptPath);

                    if (script != null)
                    {
                        remoteNetwork.UploadScript(script);
                        string ack = remoteNetwork.Acknowlage();

                        if (ack.Substring(0,1) == "k")
                        {
                            Global.ScriptEnabled = true;
                            scriptCom = new ScriptNetwork();
                            scriptCom.InitCom(ip, movement);
                            scriptCom.Communication();
                            scriptCom.ScriptRunning = true;
                        }
                    }
                }

            }
        }

        private void InputSend_Click(object sender, RoutedEventArgs e)
        {
            if (input_.Text != "")
            {
                scriptCom.InputMsg = input_.Text;
                input_.Text = "";
            }
        }

        private void Pythone_Exited(object sender, EventArgs e)
        {
            scriptCom.EndCom();
        }

        private void ScriptStop_Click(object sender, RoutedEventArgs e)
        {
            if (Global.RemoteExc == true)
            {
                remoteNetwork.QuitScript();
            }
            else
            {
                Global.ScriptEnabled = false;
                pythone.Close();
            }

        }

        private void RemoteExecution_Change(object sender, RoutedEventArgs e)
        {
            Global.RemoteExc = !Global.RemoteExc;
        }

        /*INVERSE kinematic         Not what I wanned  but it is really cool and can be use later*/

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            inverse = new InverseKinematic(movement, model);
            Global.InverseKinematicMovement = true;

            IK_timer = new DispatcherTimer();
            IK_timer.Tick += IK_timer_Tick;
            IK_timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            IK_timer.Start();

        }

        private void IK_timer_Tick(object sender, EventArgs e)
        {
            /*if (inverse.MidpointReached==true)
            {
                inverse.MidpointReached = false;
                inverse.RealoadTarger();
            }*/

            //inverse.InverseKinematics();
            if (Global.InverseKinematicMovement == true)
            {
                //inverse.RealoadTarger();
                inverse.InverseKinematics();
            }
            else
            {
                IK_timer.Stop();
            }
        }

        /*Brute Force "Fake inverse kinematic"*/

        /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            ForceMovement.InitBRM();
            MessageBox.Show("end");
            
        }*/
    }
}

