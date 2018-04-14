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
    /// Interaction logic for BetterPopUpBox.xaml
    /// </summary>
    public partial class BetterPopUpBox : Window
    {
        public BetterPopUpBox()
        {
            InitializeComponent();

            if (Global.BetterMessageBoxErrorIndex == 2)
            {
                GlobalException();
            }
            else if (Global.BetterMessageBoxErrorIndex == 3)
            {
                RecoveryMode();
            }

        }

        void RecoveryMode()
        {
            betterLabel.Content = "Recovery mode is now activated.";
            shBut.Visibility = Visibility.Visible;
        }

        private void GlobalException()
        {
            betterLabel.FontSize = 15;
            betterLabel.Content = "Something went wrong! &#xA; Restart program and reconnect robotic arm.";
            shBut.Visibility = Visibility.Visible;

        }

        async Task BetterDelay(int delay)
        {
            await Task.Delay(delay);
        }

        private void shBut_Click(object sender, RoutedEventArgs e)
        {
            if (Global.BetterMessageBoxErrorIndex ==2){
                Application.Current.Shutdown();
            }
            else if (Global.BetterMessageBoxErrorIndex == 3){
                shBut.Visibility = Visibility.Hidden;
                Global.Recovery = true;
                this.Close();
            }

        }
    }
}
