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

namespace Odyssey
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        String emulatorPath, gameFolderPath;
        public SettingsWindow()
        {
            InitializeComponent();
        }

        //Save settings
        private void SaveSettings()
        {
            //Save dark mode setting
            Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

            Properties.Settings.Default.Save();
        }

        //Load settings on startup
        private void LoadSettings()
        {
            if (!Properties.Settings.Default.DarkMode)
            {
                //Disable dark mode
                
                darkModeChkBx.IsChecked = false;
            }
            else
            {
                //Dark mode is enabled
                
                darkModeChkBx.IsChecked = true;
            }
        }

        // On application startup
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load stored settings
            LoadSettings();
    
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Settings applied");

            if (darkModeChkBx.IsChecked == true)
            {
                //Change Bg Colour to black
            }

            if (EmulatorFilePath != null)
            {
                emulatorPath = EmulatorFilePath.Text;
            }

            if (GameFolderPath!= null)
            {
                gameFolderPath = GameFolderPath.Text;
            }
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
