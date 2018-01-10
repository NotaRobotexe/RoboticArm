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
            string content = EditorNew.Text;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save a Script";
            saveFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts";
            saveFileDialog.FileName = "Script.py";
            saveFileDialog.ShowDialog();

            string path = saveFileDialog.FileName;

            if (path != "Script.py")
            {
                File.WriteAllText(path, content);
                saved = true;
            }


        }

        private void CloseWindows(object sender, RoutedEventArgs e)
        {
            if (saved == true)
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Save or delete a file first");
            }
        }

        private void Clearbox(object sender, RoutedEventArgs e)
        {
            EditorNew.Clear();
            saved = true;
        }

        private void OpenScript(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open a Script";
            openFileDialog.InitialDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\RoboticArm\\Scripts";
            openFileDialog.ShowDialog();

            string path = openFileDialog.FileName;
            string content = File.ReadAllText(path);

            if (content != null)
            {
                EditorNew.Text = content;
            }

        }
    }
}
