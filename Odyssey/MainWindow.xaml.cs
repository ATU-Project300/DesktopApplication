using System.Windows;

namespace Odyssey
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        // On application startup
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load stored settings
            loadSettings();
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
            MessageBox.Show("Settings applied");
        }

        //Load settings on startup
        private void loadSettings()
        {
            if (!Properties.Settings.Default.DarkMode)
            {
                //Disable dark mode
                darkModeStatus.Content = "Dark mode disabled";
                darkModeChkBx.IsChecked = false;
            }
            else
            {
                //Dark mode is enabled
                darkModeStatus.Content = "Dark mode enabled";
                darkModeChkBx.IsChecked = true;
            }
        }

        //Save settings
        private void saveSettings()
        {
            //Save dark mode setting
            Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

            Properties.Settings.Default.Save();
        }
    }
}
