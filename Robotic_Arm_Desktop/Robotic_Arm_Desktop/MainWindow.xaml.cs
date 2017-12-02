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

namespace Robotic_Arm_Desktop
{
    public partial class MainWindow : Window
    {
        GamepadState gamepadData;
        Movemend movemend;
        _3Dmodel model;
        Gamepad gamepad;
        XmlReadWriter xrw;

        bool AutoMode = false;
        bool gamepadConnected = false;
        bool hud = false;
        bool loadingDone = false;
        bool CapturingTemplate = false;
        bool WaitForTrigger = false;
        bool fastMode = false;
        bool connected = false;

        bool testconnection = false;

        DispatcherTimer ControllstatusTimer;
        DispatcherTimer AutoModeAnimation;

        List<string> Commands = new List<string>(); //Template command
        Stopwatch stopWatch;
        TimeSpan elapsed;

        public MainWindow()
        {
            InitializeComponent();

            if (testconnection== false)
            {
                Stats.GetPingAndTryConnection();//TODO: nech sa to overi este pri zadavani ip adresi
            }

            NetworkCom.InitCom();
            NetworkCom.VideoStrem(900, 600);

            movemend = new Movemend();
            movemend.StartAndQuitPosition();
            this.Loaded += MainWindow_Loaded; //some method need be call after window is loaded like gamepad because it need window handler 
            model = new _3Dmodel();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeGamepad();
            helix.Content = model.group;

            //Automatic Mode animation
            AutoModeAnimation = new DispatcherTimer();
            AutoModeAnimation.Tick += AutoModeAnimation_Tick ;
            AutoModeAnimation.Interval = new TimeSpan(0, 0, 0, 0, 5);

            //keyboard animation 
            ControllstatusTimer = new DispatcherTimer();
            ControllstatusTimer.Tick += ControllstatusTimer_Tick;
            ControllstatusTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);

            xrw = new XmlReadWriter();
            xrw.LoadSettings(movemend);

            //set first value to motor calibration
            MotorCalibrationDisplay();

            //load data to combobox
            LoadFilesToComboBox();


            StatTimersForStatsStuff();

            DrawDataAndUpdateModel();
            loadingDone = true;
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            NetworkCom.StopMovemend();
        }

        /*undone things*/
        private void Start(object sender, RoutedEventArgs e)
        {
            movemend.StartAndQuitPosition();
            NetworkCom.StartMovemend();
        }

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
        }

        private void Constrast_change(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
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
            movemend.keyboardenabled = !movemend.keyboardenabled;
            buttonAnimationOnOff(keyboardEnalbeButton, movemend.keyboardenabled);
        }

        private void GamepadOnOff(object sender, RoutedEventArgs e)
        {
            movemend.gamepadEnabled = !movemend.gamepadEnabled;
            buttonAnimationOnOff(GamepadEnalbeButton, movemend.gamepadEnabled);
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
                        MessageBox.Show("Gamepad Connected!");
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
                            OnOffControllStatus(); //controll status just for effect
                            gamepadData = Gamepad.GamepadProcesing(lParam);
                            movemend.AnalizeData(gamepadData);
                            gamepadConnected = true;
                            DrawDataAndUpdateModel();
                        }
                        break;

                    case 0x0219:
                        {
                            gamepadConnected = false;
                        }
                        break;
                }
                gamepadStateChange = gamepadConnected;

                /*if (movemend.wrongMode == true)
                {
                    movemend.gamepadEnabled = !movemend.gamepadEnabled; //UNDONE: toto treba cele prerobit je to cele na picu. ked sa zobrazi sprava tak nejde prepnut mod a tak
                    MessageBox.Show("Error! press mode on gamepad for a fix");
                    buttonAnimationOnOff(GamepadEnalbeButton, false);
                }*/

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
                    OnOffControllStatus(); //controll status just for effect
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

                DrawDataAndUpdateModel();
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
                availableD.Content = movemend.elbow0.maxAngle + " °";
                max.Maximum = movemend.elbow0.maxAngle;
                max.Value= movemend.elbow0.maxUseAngle;
                maxUse.Content= movemend.elbow0.maxUseAngle + " °";

                start.Maximum = movemend.elbow0.maxAngle;
                start.Value = movemend.elbow0.startfrom;
                startFrom.Content = movemend.elbow0.startfrom + " °";

            }
            else if(listBox.SelectedIndex == 1)
            {
                availableD.Content = movemend.elbow1.maxAngle + " °";
                max.Maximum = movemend.elbow1.maxAngle;
                max.Value = movemend.elbow1.maxUseAngle;
                maxUse.Content = movemend.elbow1.maxUseAngle + " °";

                start.Maximum = movemend.elbow1.maxAngle;
                start.Value = movemend.elbow1.startfrom;
                startFrom.Content = movemend.elbow1.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 2)
            {
                availableD.Content = movemend.elbow2.maxAngle + " °";
                max.Maximum = movemend.elbow2.maxAngle;
                max.Value = movemend.elbow2.maxUseAngle;
                maxUse.Content = movemend.elbow2.maxUseAngle + " °";

                start.Maximum = movemend.elbow2.maxAngle;
                start.Value = movemend.elbow2.startfrom;
                startFrom.Content = movemend.elbow2.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 3)
            {
                availableD.Content = movemend.baseMovemend.maxAngle + " °";
                max.Maximum = movemend.baseMovemend.maxAngle;
                max.Value = movemend.baseMovemend.maxUseAngle;
                maxUse.Content = movemend.baseMovemend.maxUseAngle + " °";

                start.Maximum = movemend.baseMovemend.maxAngle;
                start.Value = movemend.baseMovemend.startfrom;
                startFrom.Content = movemend.baseMovemend.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 4)
            {
                availableD.Content = movemend.griperRotation.maxAngle + " °";
                max.Maximum = movemend.griperRotation.maxAngle;
                max.Value = movemend.griperRotation.maxUseAngle;
                maxUse.Content = movemend.griperRotation.maxUseAngle + " °";

                start.Maximum = movemend.griperRotation.maxAngle;
                start.Value = movemend.griperRotation.startfrom;
                startFrom.Content = movemend.griperRotation.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 5)
            {
                availableD.Content = movemend.griper.maxAngle + " °";
                max.Maximum = movemend.griper.maxAngle;
                max.Value = movemend.griper.maxUseAngle;
                maxUse.Content = movemend.griper.maxUseAngle + " °";

                start.Maximum = movemend.griper.maxAngle;
                start.Value = movemend.griper.startfrom;
                startFrom.Content = movemend.griper.startfrom + " °";
            }
        }

        private void MaxUseSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (listBox.SelectedIndex == 0)
            {
                movemend.elbow0.maxUseAngle = Math.Round(max.Value,2);
                maxUse.Content = movemend.elbow0.maxUseAngle + " °";
            }
            else if (listBox.SelectedIndex == 1)
            {
                movemend.elbow1.maxUseAngle = Math.Round(max.Value, 2);
                maxUse.Content = movemend.elbow1.maxUseAngle + " °";
            }
            else if (listBox.SelectedIndex == 2)
            {
                movemend.elbow2.maxUseAngle = Math.Round(max.Value, 2);
                maxUse.Content = movemend.elbow2.maxUseAngle + " °";
            }
            else if (listBox.SelectedIndex == 3)
            {
                movemend.baseMovemend.maxUseAngle = Math.Round(max.Value, 2);
                maxUse.Content = movemend.baseMovemend.maxUseAngle + " °";
            }
            else if (listBox.SelectedIndex == 4)
            {
                movemend.griperRotation.maxUseAngle = Math.Round(max.Value, 2);
                maxUse.Content = movemend.griperRotation.maxUseAngle + " °";
            }
            else if (listBox.SelectedIndex == 5)
            {
                movemend.griper.maxUseAngle = Math.Round(max.Value, 2);
                maxUse.Content = movemend.griper.maxUseAngle + " °";
            }
        }

        private void StartFromSliderChange(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (listBox.SelectedIndex == 0)
            {
                movemend.elbow0.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.elbow0.startfrom + " °";

            }
            else if (listBox.SelectedIndex == 1)
            {
                movemend.elbow1.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.elbow1.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 2)
            {
                movemend.elbow2.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.elbow2.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 3)
            {
                movemend.baseMovemend.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.baseMovemend.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 4)
            {
                movemend.griperRotation.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.griperRotation.startfrom + " °";
            }
            else if (listBox.SelectedIndex == 5)
            {
                movemend.griper.startfrom = Math.Round(start.Value, 2);
                startFrom.Content = movemend.griper.startfrom + " °";
            }
        }

        private void SaveMotorsStats(object sender, RoutedEventArgs e)
        {
            xrw.UpdateFile(movemend);
        }

        /*draw position data to the app and update 3d model*/
        private void DrawDataAndUpdateModel()
        {
            this.baseRv.Content = Math.Round(movemend.baseMovemend.AngleInHz, 1);
            this.elb0v.Content = Math.Round(movemend.elbow0.AngleInHz, 1);
            this.elb1v.Content = Math.Round(movemend.elbow1.AngleInHz, 1);
            this.elb2v.Content = Math.Round(movemend.elbow2.AngleInHz, 1);
            this.grrv.Content = Math.Round(movemend.griperRotation.AngleInHz, 1);
            this.grv.Content = Math.Round(movemend.griper.AngleInHz, 1);

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
        }

        /*uppers tabs*/
        private void ExitWin(object sender, RoutedEventArgs e)
        {
            NetworkCom.SendData("7");
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
            fastmodetemplate.IsEnabled = true;

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
            string command = movemend.baseMovemend.AngleInHz + "*" + movemend.elbow0.AngleInHz + "*" + movemend.elbow1.AngleInHz + "*" 
            + movemend.elbow2.AngleInHz + "*" + movemend.griperRotation.AngleInHz + "*" + movemend.griper.AngleInHz+"*"
            +Convert.ToInt16(WaitForTrigger)+"*"+delayTexBox.Text+"*"+templateSpeed.Text + "*" + Convert.ToInt16(fastMode);

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
            fastmodetemplate.IsEnabled = false;

            fastMode = false;

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
                AutoModeAnimation.Start();
                Global.autoModeRunning = true;
                Commands = xrw.LoadCommands(templateComboBox.SelectedItem.ToString());
                AutoModeTemplate.StartTemplateAsync(Commands,movemend,model,numOfLoop);
            }
            else
            {
                MessageBox.Show("Select template");
            }
        }

        private void FastmodeChanged(object sender, RoutedEventArgs e)
        {
            fastMode = !fastMode;
        }

        private void AutoModeAnimation_Tick(object sender, EventArgs e)
        {
            DrawDataAndUpdateModel();
            if (Global.autoModeRunning == false)
            {
                AutoModeAnimation.Stop();
            }
        }

        private void StopMoving(object sender, RoutedEventArgs e)
        {
            Global.stop = true;
            numOfLoop.Text = "1";
        }

        /*Stats shit*/

        void StatTimersForStatsStuff()
        {
            DispatcherTimer PingTimer = new DispatcherTimer();
            PingTimer.Tick += PingTimer_Tick;
            PingTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            PingTimer.Start();

            DispatcherTimer DataTimer = new DispatcherTimer();
            DataTimer.Tick += DataTimer_Tick;
            DataTimer.Interval = new TimeSpan(0, 0, 0, 2);
            DataTimer.Start();

            DispatcherTimer TriggerTimer = new DispatcherTimer();
            TriggerTimer.Tick += TriggerTimer_Tick;
            TriggerTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            TriggerTimer.Start();

            DispatcherTimer UpTime = new DispatcherTimer();
            UpTime.Tick += UpTime_Tick;
            UpTime.Interval = new TimeSpan(0, 0, 1);
            UpTime.Start();

            stopWatch = new Stopwatch();
            stopWatch.Start();
        }

        private void UpTime_Tick(object sender, EventArgs e)
        {
            elapsed = stopWatch.Elapsed;
            this.uptime.Content = elapsed.ToString("hh\\:mm\\:ss");
        }

        private void TriggerTimer_Tick(object sender, EventArgs e)
        {
            NetworkCom.CheckTrigger();
            this.trigger.Content = Global.triggered;
        }

        private void DataTimer_Tick(object sender, EventArgs e)
        {
            NetworkCom.GetData();
            this.cpuusage.Content = Stats.CPUload + " %";
            this.temperature.Content = Stats.Temperature + " °C";
        }

        private void PingTimer_Tick(object sender, EventArgs e)
        {
            if (testconnection == false)
            {
                Stats.GetPingAndTryConnection();
            }
            this.latency.Content = Stats.ping;
            this.status.Content = Global.connected;
        }
    }
}
