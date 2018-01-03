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

            if (Global.BetterMessageBoxErrorIndex == 1)
            {
                CheckGamepadMode();
            }

        }

        async void CheckGamepadMode()
        {
            betterLabel.Content = "Press 'Mode' to activate GamePad";
            MainWindow main = new MainWindow();
            main.Activate();
            while (Global.WrongMode == true)
            {
                await BetterDelay(300);
            }

            Global.BetterMessageBoxLauched = false;
            this.Close();

        }

        async Task BetterDelay(int delay)
        {
            await Task.Delay(delay);
        }

    }
}
