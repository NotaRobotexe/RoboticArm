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
    //TODO: implementovat video stream
    //TODO: HUD, contrast ostatne
    //TODO: Safety control - taktiez netreba moc
    //TODO: fixnut ostatne buggy
    //TODO: responzivnost - nepotrebne fullHD staci
    //TODO: SCRIPT module - hot hot hot hot
    //TODO: Spracovanie obrazu a vsetky tie blbosti - hot hot hot hot
    //TODO: spiest sa ako babovka #1 #2 #3
    //TODO: Prestat pridavat TODO

    public partial class MainWindow : Window
    {
        GamepadState gamepadData;
        Movemend movemend;
        _3Dmodel model;
        Gamepad gamepad;
        XmlReadWriter xrw;

        string ScriptPath = "";

        bool AutoMode = false;
        bool gamepadConnected = false;
        bool hud = false;
        bool loadingDone = false;
        bool CapturingTemplate = false;
        bool WaitForTrigger = false;


        DispatcherTimer ControllstatusTimer;

        List<string> Commands = new List<string>(); //Template command
        Stopwatch stopWatch;
        TimeSpan elapsed;

        NetworkCom netCom;
        NetworkCom netData;
        NetworkCom netMove;
        NetworkCom netTrigger;
        NetworkCom netFan;

        public MainWindow()
        {

            Directory.CreateDirectory(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts");
            InitializeComponent();

            if (Global.DebugMode== false)
            {
                Stats.GetPingAndTryConnection();//TODO: nech sa to overi este pri zadavani ip adresi
            }

            movemend = new Movemend();
            Arm.PositonChange += Arm_PositonChange;

            xrw = new XmlReadWriter();
            xrw.LoadSettings(movemend);

            this.Loaded += MainWindow_Loaded; //some method need be call after window is loaded like gamepad because it need window handler 
            model = new _3Dmodel();


            InitCom();
            netCom.SendData("1");
            GetCPUandTemp();
            GetTrigger();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGamepad();
            helix.Content = model.group;

            //keyboard animation 
            ControllstatusTimer = new DispatcherTimer();
            ControllstatusTimer.Tick += ControllstatusTimer_Tick;
            ControllstatusTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            //set first value to motor calibration
            MotorCalibrationDisplay();

            //load data to combobox
            LoadFilesToComboBox();

            StatTimersForStatsStuff();

            SetFirstPositionOfModel();
            loadingDone = true;
        }

        /*undone things*/

        private void HUDclick(object sender, RoutedEventArgs e)
        {
            /*if (hud == false)
            {
                HUDbutton.Content = "Enabled";
                HUDbutton.Background = 
                hud = true;
            }
            else
            {
                HUDbutton.Content = "Disabled";// "#FF2E2E2E"
                HUDbutton.Background = Brushes.;
                hud = false;
            }*/
        }

        private void Brighness_change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }

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

            movemend.keyboardenabled = !movemend.keyboardenabled;
            buttonAnimationOnOff(keyboardEnalbeButton, movemend.keyboardenabled);
            if (gamepad.gamepadConnected == true )
            {
                movemend.gamepadEnabled = !movemend.gamepadEnabled;
                buttonAnimationOnOff(GamepadEnalbeButton, movemend.gamepadEnabled);
            }
        }

        private void Constrast_change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
        
        /*buttons*/
        private void Recovery_Click(object sender, RoutedEventArgs e)
        {
            List<string> instructionsRaw = Positions.RecoveryPos.Split('*').ToList();
            List<double> instructions = new List<double>();

            foreach (var item in instructionsRaw)
            {
                instructions.Add(Convert.ToDouble(item));
            }

            movemend.baseMovemend.Update(instructions[0],1);
            movemend.elbow0.Update(instructions[1], 1);
            movemend.elbow1.Update(instructions[2], 1);
            movemend.elbow2.Update(instructions[3], 1);
            movemend.griperRotation.Update(instructions[4], 1);
            movemend.griper.Update(instructions[5], 1);

            DrawDataAndUpdateModel();
        }

        private async void Start(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.SteadyPos, movemend);
            latency.Content = "test";
        }

        private async void Stop(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movemend);
            latency.Content = "test2";

        }

        private async void TurnOffPressed(object sender, RoutedEventArgs e)
        {
            await AutoModeTemplate.AnimationFromTemplate(Positions.OffPos, movemend);
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
                movemend.keyboardenabled = !movemend.keyboardenabled;
                buttonAnimationOnOff(keyboardEnalbeButton, movemend.keyboardenabled);
            }
        }

        private void GamepadOnOff(object sender, RoutedEventArgs e)
        {
            if (gamepad.gamepadConnected == true && AutoMode == false)
            {
                movemend.gamepadEnabled = !movemend.gamepadEnabled;
                buttonAnimationOnOff(GamepadEnalbeButton, movemend.gamepadEnabled);
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
        private void InitializeGamepad()
        {
            var mainHandle = new WindowInteropHelper(this).Handle;

            gamepad = new Gamepad(mainHandle);

            HwndSource source = HwndSource.FromHwnd(mainHandle);
            source.AddHook(new HwndSourceHook(WndProc));
            
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) //gamepad input
        {
            if (movemend.gamepadEnabled == true)
            {
                switch (msg)
                {
                    case 0x00ff:
                        {
                            gamepadData = Gamepad.GamepadProcesing(lParam);
                            movemend.AnalizeData(gamepadData);
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
            if (movemend.keyboardenabled == true)
            {
                if (e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3)
                {
                    movemend.AnalizeData(e.Key);
                }
                else if (e.Key == Key.D1 && movemend.keyboardMovingArm != 0)
                {
                    movemend.keyboardMovingArm = 0;
                }
                else if (e.Key == Key.D2 && movemend.keyboardMovingArm != 1)
                {
                    movemend.keyboardMovingArm = 1;
                }
                else if (e.Key == Key.D3 && movemend.keyboardMovingArm != 2)
                {
                    movemend.keyboardMovingArm = 2;
                }
            }
        }

        /*Motors calibration and settings */
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
                max.Value= movemend.elbow0.EndAt; 
                maxUse.Content=  Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.elbow0.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
            else if(listBox.SelectedIndex == 1)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movemend.elbow1.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.elbow1.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
            else if (listBox.SelectedIndex == 2)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movemend.elbow2.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.elbow2.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
            else if (listBox.SelectedIndex == 3)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movemend.baseMovemend.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.baseMovemend.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
            else if (listBox.SelectedIndex == 4)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movemend.griperRotation.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.griperRotation.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value), 2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
            else if (listBox.SelectedIndex == 5)
            {
                max.Maximum = Arm.max_Pwm;
                max.Minimum = Arm.min_Pwm;
                max.Value = movemend.griper.EndAt;
                maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                start.Maximum = Arm.max_Pwm; ;
                start.Minimum = Arm.min_Pwm; ;
                start.Value = movemend.griper.startfrom;
                startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value), 2) + " °";

                double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                availableD.Content = Math.Round(avalible, 2).ToString() + " °";
            }
        }

        private void MaxUseSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (loadingDone == true)
            {
                if (listBox.SelectedIndex == 0)
                {
                    movemend.elbow0.EndAt = Math.Round(max.Value,2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";
                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 1)
                {
                    movemend.elbow1.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 2)
                {
                    movemend.elbow2.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 3)
                {
                    movemend.baseMovemend.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 4)
                {
                    movemend.griperRotation.EndAt = Math.Round(max.Value, 2);
                    maxUse.Content= Math.Round(Arm.PwmToDegree(max.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 5)
                {
                    movemend.griper.EndAt = Math.Round(max.Value, 2);
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
                    movemend.elbow0.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 1)
                {
                    movemend.elbow1.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 2)
                {
                    movemend.elbow2.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 3)
                {
                    movemend.baseMovemend.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 4)
                {
                    movemend.griperRotation.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible, 2).ToString() + " °";
                }
                else if (listBox.SelectedIndex == 5)
                {
                    movemend.griper.startfrom = Math.Round(start.Value, 2);
                    startFrom.Content = "+ " + Math.Round(Arm.PwmToDegree(start.Value),2) + " °";

                    double avalible = 180 - Arm.PwmToDegree(start.Value) - (180 - Arm.PwmToDegree(max.Value));
                    availableD.Content = Math.Round(avalible,2).ToString() + " °";
                }
            }
        }

        private void SaveMotorsStats(object sender, RoutedEventArgs e)
        {
            xrw.UpdateFile(movemend);
        }

        /*draw position data to the app and update 3d model*/

        private void Arm_PositonChange(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
            () =>
             {
                 DrawDataAndUpdateModel();
            });
        }

        public void DrawDataAndUpdateModel()
        {
            this.baseRa.Content = Math.Round(movemend.baseMovemend.AngleInDegree, 2) + " °";
            this.elb0a.Content = Math.Round(movemend.elbow0.AngleInDegree, 2) + " °";
            this.elb1a.Content = Math.Round(movemend.elbow1.AngleInDegree, 2) + " °";
            this.elb2a.Content = Math.Round(movemend.elbow2.AngleInDegree, 2) + " °";
            this.grra.Content = Math.Round(movemend.griperRotation.AngleInDegree, 2) + " °";
            this.gr.Content = Math.Round(movemend.griper.AngleInDegree, 2) + " °";

            this.incLabel.Content = Math.Round(movemend.valueCount,3);

            model.UpdateModel(movemend);

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

            movemend.baseMovemend.Update(instructions[0], 1);
            movemend.elbow0.Update(instructions[1], 1);
            movemend.elbow1.Update(instructions[2], 1);
            movemend.elbow2.Update(instructions[3], 1);
            movemend.griperRotation.Update(instructions[4], 1);
            movemend.griper.Update(instructions[5], 1);

            DrawDataAndUpdateModel();
        }

        /*uppers tabs*/
        private void ExitWin(object sender, RoutedEventArgs e)
        {
            netCom.SendData("7");
            Application.Current.Shutdown();
        }

        private void MinWin(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
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
            string command = movemend.baseMovemend.AngleInPWM + "*" + movemend.elbow0.AngleInPWM + "*" + movemend.elbow1.AngleInPWM + "*"
            + movemend.elbow2.AngleInPWM + "*" + movemend.griperRotation.AngleInPWM + "*" + movemend.griper.AngleInPWM + "*"
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

        private void StartTemplate(object sender, RoutedEventArgs e) //TODO: nech to zacne na nejakej basic pozicii;
        {
            Global.stop = false;

            if (templateComboBox.SelectedItem != null)
            {
                Global.autoModeRunning = true;
                Commands = xrw.LoadCommands(templateComboBox.SelectedItem.ToString());
                AutoModeTemplate.StartTemplateAsync(Commands,movemend,numOfLoop);
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
            Process pythone = new Process();
            pythone.StartInfo.FileName = ScriptPath;
            pythone.StartInfo.UseShellExecute = false;
            pythone.StartInfo.RedirectStandardOutput = true;
            pythone.StartInfo.CreateNoWindow = true;
            pythone.Start();
        }

        bool ValueBeforeTiping1, ValueBeforeTiping2;

        private void DisableMovemendWhenWriting(object sender, RoutedEventArgs e)
        {
            ValueBeforeTiping1 = movemend.keyboardenabled;
            ValueBeforeTiping2 = movemend.gamepadEnabled;

            movemend.keyboardenabled = false;
            movemend.gamepadEnabled = false;
        }


        private void EnableMovementdAfterWriting(object sender, RoutedEventArgs e)
        {

            movemend.keyboardenabled = ValueBeforeTiping1;
            movemend.gamepadEnabled = ValueBeforeTiping2;
        }
    }
}
