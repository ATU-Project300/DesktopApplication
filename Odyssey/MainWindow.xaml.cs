using API;
using System.Threading.Tasks;
using System.Windows;

namespace Odyssey
{
    public partial class MainWindow : Window
    {
        private GameList _games = new GameList();
        public MainWindow()
        {
            InitializeComponent();
            ApiHelper.InitClient(); //Initialise API client

        }

        // Occupy the _games variable with the response of LoadGames
        private async Task LoadGames()
        {
            _games = await GameProcessor.LoadGames();
        }

        // On application startup
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load stored settings
            LoadSettings();

            //Load API game data
            LoadGames();
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Settings applied");
            LoadSettings();
        }

        //Load settings on startup
        private void LoadSettings()
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
        private void SaveSettings()
        {
            //Save dark mode setting
            Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

            Properties.Settings.Default.Save();
        }

        //Test button
        private async void loadGames_Click(object sender, RoutedEventArgs e)
        {
            LoadGames();
            int a = _games.ListOfGames.Count;  // Easy way to check if the API call worked
            //myText.Text = games.ListOfGames[5].Title.ToString(); //How individual games should be called?
        }
    }
}
