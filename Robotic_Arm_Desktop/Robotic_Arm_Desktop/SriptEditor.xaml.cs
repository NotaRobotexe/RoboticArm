using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SriptEditor.xaml
    /// </summary>
    public partial class SriptEditor : Window
    {
        public SriptEditor()
        {
            InitializeComponent();
        }

        bool saved = false;

        private void SaveScript(object sender, RoutedEventArgs e)
        {
            string content = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd).Text;
            MessageBox.Show(content);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save a Script";
            saveFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts";
            saveFileDialog.FileName = "Script.ArmScript";
            saveFileDialog.ShowDialog();

            string path = saveFileDialog.FileName;

            File.WriteAllText(path, content);

        }

        private void CloseWindows(object sender, RoutedEventArgs e)
        {
            if (saved == true)
            {
                this.Close();
            }
            else
            {
            }
        }

        private void Clearbox(object sender, RoutedEventArgs e)
        {
            Editor.Document.Blocks.Clear();
        }

        private void OpenScript(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string path = openFileDialog.FileName;
            MessageBox.Show(path);
        }
    }
}
