using System;
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
    //TODO: bug pri vypinani
    //todo : este skusit cloud

    public partial class MainWindow : Window
    {
        GamepadState gamepadData ,gamepadDataOld;
        Movement movement;
        _3Dmodel model;
        Gamepad gamepad;
        XmlReadWriter xrw;
        VideoStream stream;
        RemoteNetwork remoteNetwork;

        string ScriptPath = "";

        bool GamepadSlowDown = false;
        bool AutoMode = false;
        bool gamepadConnected = false;
        bool hud = false;
        bool WaitForTrigger = false;
        int framerate = 0;
        int connectionStatus = -1;
        string[,] targets;

        DispatcherTimer ControllstatusTimer;
        DispatcherTimer FrameRateCounter;
        DispatcherTimer IK_timer;
        Timer GamepadSlower;

        List<string> Commands = new List<string>(); //Template command
        Stopwatch stopWatch;
        TimeSpan elapsed;

        SendPosition send_pos;
        NetworkCom netCom;
        NetworkCom netData;
        NetworkCom netMove;
        NetworkCom netTrigger;
        NetworkCom netFan;

        Ellipse[] TargetImg;
        TextBlock[] TargetName;

        Process restream;
        ScriptNetwork scriptCom;
        Process pythone;
        InverseKinematic inverse;

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException; //global exception
            Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts");
            InitializeComponent();

            if (Global.DebugMode== false)
            {
                Stats.GetPingAndTryConnection();
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

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) //Global exception if something goes wrong 
        {
            Global.BetterMessageBoxLauched = true;
            Global.BetterMessageBoxErrorIndex = 2;
            BetterPopUpBox BetterMessageBox = new BetterPopUpBox();
            BetterMessageBox.Show();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ControllstatusTimer = new DispatcherTimer();
            ControllstatusTimer.Tick += ControllstatusTimer_Tick;
            ControllstatusTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            Arm.PositonChange += Arm_PositonChange;
            movement.IncrementationChange += Movemend_IncrementationChange;

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
            this.incLabel.Content = Math.Round(movement.valueCount, 3);

            int mainID = Process.GetCurrentProcess().Id;
            new Thread(() =>
            {
                Process p = Process.GetProcessById(mainID);
                IntPtr windowHandle = p.MainWindowHandle;
                InitializeGamepad(windowHandle);
            }).Start();


            int speed = (int)Math.Round(fanSlider.Value); //start fan after loading
            netFan.SendData(speed.ToString());

            GamepadSlower = new Timer(TimerCallback, null, 0, 30);
            Global.streamratioX = (float)ViewFrame.Width / (float)Global.StreamWidth;
            Global.streamratioY = (float)ViewFrame.Height / (float)Global.StreamHight;

            Global.loadingDone = true;
        }

        /*video stream and other thing with image control*/
        private async void SetStreamSettings()
        {
            loading.Visibility = Visibility.Visible;
            if (Global.loadingDone ==true){
                stream.ProcesEnd();
                stream = new VideoStream();
            }

            Global.StreamWidth = Convert.ToInt32(widthL.Text);
            Global.StreamHight = Convert.ToInt32(highL.Text);
            Global.streamratioX =  (float)ViewFrame.Width/ (float)Global.StreamWidth;
            Global.streamratioY = (float)ViewFrame.Height / (float)Global.StreamHight;


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
            await NonBlockSleep(2000);
            stream.Procesinit();
            loading.Visibility = Visibility.Hidden;
        }

        public async Task NonBlockSleep(int time)
        {
            await Task.Run(() =>
            {
                Thread.Sleep(time);
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
        private async void ExitWin(object sender, RoutedEventArgs e) //nezabudni znovu zapnuti vypinanie
        {
            try
            {
                await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
                netCom.SendData("7");
                FrameRateCounter.Stop();
                stream.ProcesEnd();
                Application.Current.Shutdown();
            }
            catch (Exception)
            {
                Application.Current.Shutdown();
            }
        }

        private void MinWin(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Recovery == false)
            {
                Global.BetterMessageBoxLauched = true;
                Global.BetterMessageBoxErrorIndex = 3;
                BetterPopUpBox BetterMessageBox = new BetterPopUpBox();
                BetterMessageBox.Show();
                Global.BetterMessageBoxLauched = false;
            }

            if (Global.Recovery == true)
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
                Global.Recovery = false;
            }
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.SteadyPos, movement);
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
        }

        private async void TurnOffPressed(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movement);
            netCom.SendData("70");
            this.Close();
        }

        /*manual mode stuff here*/
        private void OnOffControllStatus() //ManualModeStatusEllipse turning on off
        {
            if (ControllstatusTimer != null)
            {
                if (ControllstatusTimer.IsEnabled == false)
                {
                    ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(117, 255, 67));
                    ControllstatusTimer.Start();
                }
            }
        }

        private void ControllstatusTimer_Tick(object sender, EventArgs e)
        {
            ManualModeStatusEllipse.Fill = new SolidColorBrush(Color.FromRgb(30, 190, 247));
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


        /*gamepad and keyboard basic input*/
        private void InitializeGamepad(IntPtr windowHandle)
        {
            gamepad = new Gamepad(windowHandle);

            HwndSource source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(new HwndSourceHook(WndProc));

        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) //gamepad input
        {
            if (movement.gamepadEnabled == true && GamepadSlowDown == false)
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

        private void TimerCallback(object state)
        {
            GamepadSlowDown = !GamepadSlowDown;
        }

        /*Motors calibration and settings */

        private void ListboxChange(object sender, SelectionChangedEventArgs e)
        {
            if (Global.loadingDone ==true)
            {
                MotorCalibrationDisplay();
            }
        }

        private void MotorCalibrationDisplay()
        {
            if (listBox.SelectedIndex == 0 && max != null)
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
            }
        }

        private void MaxUseSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Global.loadingDone == true)
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
            if (Global.loadingDone == true)
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

            if (Global.loadingDone == true)
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

            if (tempname.Text != "Template name" && tempname.Text != ""){
                Commands.Clear();
            }
            else{
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
                OnOffControllStatus();
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
                trigger = trigger.Substring(0, trigger.IndexOf('\0'));
                if (trigger == "false")
                {
                    Global.triggered = false;
                }
                else
                {
                    Global.triggered = true;
                }
                this.trigger.Content = Global.triggered;
                OnOffControllStatus();
            }
        }

        private void UpTime_Tick(object sender, EventArgs e) //and some cleaning of targets which have no connection with this but i just need some timer so why create new... pls end my suffering 
        {
            elapsed = stopWatch.Elapsed;
            this.uptime.Content = elapsed.ToString("hh\\:mm\\:ss");

            if (TargetImg != null && TargetImg.Length > 0 && Cnv.Children.Contains(TargetImg[0])){
                RemoveTarget();
            }
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
            if (Global.connected == true && netFan != null)
            {
                OnOffControllStatus();
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

        private async void RunScript(object sender, RoutedEventArgs e)
        {
            Global.ScriptOutput = "";
            sciptoutput.Text = "";

            if (ScriptPath != "")
            {

                if (Global.RemoteExc == false)
                {
                    pythone = new Process();
                    pythone.StartInfo.FileName = "python.exe";
                    pythone.StartInfo.Arguments = "\""+ScriptPath+ "\"";
                    pythone.StartInfo.UseShellExecute = true;
                    pythone.StartInfo.RedirectStandardOutput = false;
                    pythone.StartInfo.CreateNoWindow = true;
                    pythone.EnableRaisingEvents = true;
                    pythone.Start();
                    pythone.Exited += Pythone_Exited;

                    await NonBlockSleep(1000);

                    Global.ScriptEnabled = true;
                    scriptCom = new ScriptNetwork();
                    scriptCom.InitCom("127.0.0.1",movement);
                    scriptCom.ScriptRunning = true;
                    scriptCom.Communication();
                    scriptCom.NewOutput += ScriptCom_NewOutput;
                    scriptCom.StraightLine += ScriptCom_StraightLine;
                    scriptCom.DrawTargets += ScriptCom_DrawTargets;
                }
                else //start script on remote computer
                {
                    string ip = "";

                    if (connectionStatus == -1)
                    {
                        remoteNetwork = new RemoteNetwork();
                        ip = targetIp.Text;
                        connectionStatus = remoteNetwork.InitCom(ip, movement);

                        if (connectionStatus == 0){
                            remotestatus.Content = "Online";
                        }
                        else{
                            remotestatus.Content = "Offline";
                        }
                    }

                    LauchRestream();

                    string script = File.ReadAllText(ScriptPath);

                    if (script != null && connectionStatus == 0)
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

        private void LauchRestream()
        {
            restream = new Process();
            restream.StartInfo.FileName = Global.FfmpegPath;
            restream.StartInfo.Arguments = @"-i rtsp://" + Global.ipaddres + ":8554/unicast -preset ultrafast -c:v copy -vcodec libx264 -tune zerolatency -f h264 udp://" + targetIp.Text+ ":8555";
            restream.StartInfo.UseShellExecute = false;
            restream.StartInfo.RedirectStandardOutput = false;
            restream.StartInfo.CreateNoWindow = true;
            restream.EnableRaisingEvents = true;
            restream.Exited += Restream_Exited;
            restream.Start();
        }

        private void Restream_Exited(object sender, EventArgs e)
        {
            remotestatus.Content = "Stream failed";
        }

        private void KillRestream()
        {
            try
            {
                restream.Kill();
                restream.Close();
            }
            catch (Exception)
            {
            }
        }

        private void ScriptCom_DrawTargets(object sender, EventArgs e)
        {
            string[] objects = Global.ScriptTargets.Split('|');
            targets = new string[objects.Length - 1, 3];

            for (int i = 0; i < objects.Length-1; i++)
            {
                string[] parameters = objects[i].Split('*');
                for (int a = 0; a < 3; a++)
                {
                    targets[i, a] = parameters[a];
                }
            }

            RemoveTarget();
            DrawTargets();
        }

        private void DrawTargets()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                TargetImg = new Ellipse[targets.Length/3];
                TargetName = new TextBlock[targets.Length/3];

                

                for (int i = 0; i < targets.Length/3; i++)
                {
                    int posx = 815 + (int)((float)Convert.ToInt32(targets[i, 0]) * Global.streamratioX);
                    int posy = 20 + (int)(Convert.ToInt32(targets[i, 1]) * Global.streamratioY);

                    TargetImg[i] = new Ellipse();
                    TargetImg[i].Fill = Brushes.Red;
                    TargetImg[i].Width = 30;
                    TargetImg[i].Height = 30;
                    TargetImg[i].StrokeThickness = 6;

                    Cnv.Children.Add(TargetImg[i]);
                    Canvas.SetLeft(TargetImg[i],posx);
                    Canvas.SetTop(TargetImg[i], posy);

                    TargetName[i] = new TextBlock();
                    TargetName[i].Text = targets[i, 2];
                    TargetName[i].FontSize = 25;
                    TargetName[i].Foreground = Brushes.Red;

                    Cnv.Children.Add(TargetName[i]);
                    Canvas.SetLeft(TargetName[i], posx+15);
                    Canvas.SetTop(TargetName[i], posy+15);
                }
            });
        }

        private void RemoveTarget()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Cnv.Children.Clear();
            });
        }

        private void InputSend_Click(object sender, RoutedEventArgs e)
        {
            if (input_.Text != "" && scriptCom != null)
            {
                scriptCom.InputMessaging(input_.Text);
                input_.Text = "";
            }
        }

        private void Pythone_Exited(object sender, EventArgs e)
        {
            if (scriptCom != null)
            {
                scriptCom.EndCom();
            }
        }

        private void ScriptStop_Click(object sender, RoutedEventArgs e)
        {
            if (Global.RemoteExc == true)
            {
                scriptCom.EndCom();
                remoteNetwork.QuitScript();
            }
            else
            {
                scriptCom.EndCom();
                Global.ScriptEnabled = false;
                try
                {
                    pythone.Kill();
                    pythone.Close();
                }
                catch (Exception)
                {
                }

            }
            Global.InverseKinematicMovement = false ;

        }

        private void RemoteExecution_Change(object sender, RoutedEventArgs e)
        {
            Global.RemoteExc = !Global.RemoteExc;

            if (Global.RemoteExc==true)
            {
                remotestatus.Content = "Active";
                buttonchange.Content = "Enabled";
                LauchRestream();
            }
            else
            {
                KillRestream();
                buttonchange.Content = "Disabled";
                remotestatus.Content = "Non-Active";
            }
        }

        private void ScriptCom_NewOutput(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
            {
                sciptoutput.Text = Global.ScriptOutput;
            });
            scriptCom.SendACK();
        }

        /*INVERSE kinematic - straight line movement*/

        private void ScriptCom_StraightLine(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
            {
                inverse = new InverseKinematic(movement, model);
                Global.InverseKinematicMovement = true;

                IK_timer = new DispatcherTimer();
                IK_timer.Tick += IK_timer_Tick; 
                IK_timer.Interval = new TimeSpan(0, 0, 0, 0, 0);
                IK_timer.Start();

                scriptCom.SendACK();
            });
        }


        private void IK_timer_Tick(object sender, EventArgs e)
        {
            if (Global.InverseKinematicMovement == true)
            {
                inverse.InverseKinematics();
            }
            else
            {
                IK_timer.Stop();
            }
        }
    }
}

