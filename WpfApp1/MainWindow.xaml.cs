using Microsoft.Win32;
using Newtonsoft.Json;
using Odyssey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static API.API;
using Image = System.Windows.Controls.Image;

namespace WpfApp1
{
    public class GameViewModel
    {
        //The list read by the XAML binding
        public List<Game> myGames { get; set; }

        public GameViewModel(List<Game> games)
        {
            myGames = games;
        }
    }

    public partial class MainWindow : Window
    {
        public List<Game> myGames = new List<Game>(); //Contains games stored in a sane fashion

        //Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient _client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            InitializeApiData();

            // Essentially sets the default page to be the Games one.
            // Any new TabPanels should be added here and set to "Collapsed"
            Games.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
        }


        // From the API, use the games data to occupy the myGames List
        public async void InitializeApiData()
        {
            OccupyListVar(await ProcessGamesData(_client));
            DataContext = new GameViewModel(myGames);
        }

        // Allow for switching between light and dark themes
        // Must be added soon after InitializeComponent (Currently part of LoadSettings)
        private void Theming(bool dark)
        {
            if (dark)
            {
                //Change Bg Colour to black
                //Programmatically added gradient for DataGrid
                LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0, 3);
                myLinearGradientBrush.EndPoint = new Point(1, 1);
                myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Black, 0.1));
                myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Gray, 2.5));

                //Sets background to Gradient
                MainGrid.Background = myLinearGradientBrush;

                //Changes colour of all text to white so its easier to read text with dark mode
                Xeniatxtblk.Foreground = new SolidColorBrush(Colors.White);
                RPCS3txtblk.Foreground = new SolidColorBrush(Colors.White);
                pathRPCS3TxtBx.Foreground = new SolidColorBrush(Colors.White);
                pathXeniaTxtBx.Foreground = new SolidColorBrush(Colors.White);
                GFPtxtblk.Foreground = new SolidColorBrush(Colors.White);
                darkModeChkBx.Foreground = new SolidColorBrush(Colors.White);

                //Change bg colour of buttons and panel grid. Used Color Converter so that we can use Hex values opposed to Windows Default Colours
                HomeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                AllGamesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                PlayBTN.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                SettingsBTN.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                RecentBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));
                LogoButtonsGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#5A5A5A"));

            }
            else
            {
                //Programmatically added gradient for DataGrid
                LinearGradientBrush myLinearGradientBrush =
                new LinearGradientBrush();
                myLinearGradientBrush.StartPoint = new Point(0, 3);
                myLinearGradientBrush.EndPoint = new Point(1, 1);
                myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Purple, 0.1));
                myLinearGradientBrush.GradientStops.Add(
                new GradientStop(Colors.Yellow, 2.5));

                // Use the brush to paint the Grid .
                MainGrid.Background = myLinearGradientBrush;

                //Changes colour of all text to Black so its easier to read text with light mode
                Xeniatxtblk.Foreground = new SolidColorBrush(Colors.Black);
                RPCS3txtblk.Foreground = new SolidColorBrush(Colors.Black);
                pathRPCS3TxtBx.Foreground = new SolidColorBrush(Colors.Black);
                GFPtxtblk.Foreground = new SolidColorBrush(Colors.Black);
                darkModeChkBx.Foreground = new SolidColorBrush(Colors.Black);

                //Change bg colour of buttons and panel grid. Used Color Converter so that we can use Hex values opposed to Windows Default Colours
                HomeBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));
                AllGamesBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));
                PlayBTN.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));
                SettingsBTN.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));
                RecentBtn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));
                LogoButtonsGrid.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b06050"));

            }
        }

        // Occupy the local list var "myGames" with the games such that they are truly individually addressable (!!!)
        private void OccupyListVar(List<GamesList> list)
        {
            int i = 0;
            foreach (var x in list)
            {
                myGames.Add(new Game()
                {
                    Title = x.Title,
                    Year = x.Year,
                    Description = x.Description,
                    Image = x.Image,
                    Consoles = x.Consoles,
                    Emulator = x.Emulator
                });
                i++;
            }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //If the text box is modified and become empty, let all games be listed
            if (SearchTxBx.Text.Length <= 0)
            {
                InitializeApiData();
            }
            else
            {
                //TODO: Filter the list by textbox text
                //Requires replacing the hacky game cover stuff.
            }
        }
        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get the Game object associated with the clicked item
            var game = (sender as FrameworkElement)?.DataContext as Game;

            // Do something with the selected game object, such as showing more details in a new window
            if (game != null)
            {
                MessageBox.Show($"You clicked on: {game.Title}\nReleased in {game.Year} for the {game.Consoles}.");
            }
        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Visible;
        }

        //TODO: Verify settings - Check if the contents of the settings text boxes are valid as settings
        private void VerifySettings()
        {
            // Some basic checks such as if the text is shorter than 5 chars or is null etc.
        }

        //Save settings
        private void SaveSettings()
        {
            Odyssey.Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();
            Odyssey.Properties.Settings.Default.pathRPCS3 = pathRPCS3TxtBx.Text;
            Odyssey.Properties.Settings.Default.pathXenia = pathXeniaTxtBx.Text;
            Odyssey.Properties.Settings.Default.Save();
        }

        // Load settings
        private void LoadSettings()
        {
            // Check the stored settings to see find the last state of the dark mode setting
            if (!Odyssey.Properties.Settings.Default.DarkMode)
            {
                // if it's false or null, dark mode stays off
                darkModeChkBx.IsChecked = false;
                Theming(false);
            }
            else // Else it is enabled
            {
                darkModeChkBx.IsChecked = true;
                Theming(true); //Dark mode is enabled
            }

            if (Odyssey.Properties.Settings.Default.pathRPCS3 is null)
                pathRPCS3TxtBx.Text = "Unset";
            else
                pathRPCS3TxtBx.Text = Odyssey.Properties.Settings.Default.pathRPCS3;

            if (Odyssey.Properties.Settings.Default.pathXenia is null)
                pathXeniaTxtBx.Text = "Unset";
            else
                pathXeniaTxtBx.Text = Odyssey.Properties.Settings.Default.pathXenia;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
        }

        private void AllGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
        }

        //Settings Apply Button
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            LoadSettings();
        }

        // Open a file picker, store the resulting path in the text box
        private void PathRPCS3TxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathRPCS3TxtBx);
        }

        // Open a file picker, store the resulting path in the text box
        private void PathXeniaTxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathXeniaTxtBx);
        }

        /*
         Display file picker. Expected arg is the textbox which contains the property being modified.
         Done this not directly modify properties as the text box is manually modifiable and the user presses "Apply".
        */
        private void FilePicker(TextBox t)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == false) return;
            t.Text = ofd.FileName;
        }
    }
}
