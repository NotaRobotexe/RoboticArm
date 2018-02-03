using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private async void Next_Pressed(object sender, RoutedEventArgs e)
        {
            tothemain.IsEnabled = false;
            Global.ipaddres = IpAddres.Text;
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
