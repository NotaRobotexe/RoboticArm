using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Robotic_Arm_Desktop
{
    /// <summary>
    /// Interaction logic for StartScreen.xaml
    /// </summary>
    public partial class StartScreen : Window
    {
        public StartScreen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private async void Findevice(object sender, RoutedEventArgs e)
        {
            findb.IsEnabled = false;
            loading.Visibility = Visibility.Visible;
            string output = "";
            await Task.Run(() =>
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C ping -4 raspebrrypi.local";
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            });

            findb.IsEnabled = true;
            loading.Visibility = Visibility.Hidden;

            int begin = output.IndexOf("[")+1 ;
            int end = output.IndexOf("]");

            try
            {
                string result = output.Substring(begin,end-begin);
                Global.ipaddres = result;
                ipa.Content = result;

            }
            catch (Exception)
            {
                MessageBox.Show("Device is not connected");
            }

        }

       

        private async void Next_Pressed(object sender, RoutedEventArgs e)
        {
            if (cmbox.SelectedIndex == 0){
                Global.SVK = false;
            }
            else{
                Global.SVK = true;
            }

            tothemain.IsEnabled = false;
            loading.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                System.Threading.Thread.Sleep(1000);
            });

            MainWindow main = new MainWindow();
            App.Current.MainWindow = main;
            main.Show();

            if (Global.loadingDone == true)
            {
                this.Close();
            }
            tothemain.IsEnabled = true;
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
