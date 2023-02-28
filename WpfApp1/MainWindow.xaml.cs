using Microsoft.Win32;
using Odyssey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static API.API;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        //Contains games stored in a sane fashion
        public List<Game> myGames = new List<Game>();

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
            About.Visibility = Visibility.Collapsed;
        }


        // From the API, use the games data to occupy the myGames List
        public async void InitializeApiData()
        {
            OccupyListVar(await ProcessGamesData(_client));

            //This goes here only because it loads too early anywhere else
            DataContext = new GameViewModel(myGames);
        }

        // Allow for switching between light and dark themes
        // Must be added soon after InitializeComponent (Currently part of LoadSettings)
        private void Theming(bool dark)
        {
            // Our colour schemes are defined here, so we can easily change them

            //Dark theme colours:
            LinearGradientBrush darkLinearGradientBrush = new LinearGradientBrush();
            darkLinearGradientBrush.StartPoint = new Point(0, 3);
            darkLinearGradientBrush.EndPoint = new Point(1, 1);
            darkLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Black, 0.1));
            darkLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Gray, 2.5));

            Color darkColour = (Color)ColorConverter.ConvertFromString("#5A5A5A");
            Color darkColourText = Colors.White;

            //Light theme colours:
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 3);
            myLinearGradientBrush.EndPoint = new Point(1, 1);
            myLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Purple, 0.1));
            myLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Yellow, 2.5));

            Color lightColour = (Color)ColorConverter.ConvertFromString("#b06050");
            Color lightColourText = Colors.Black;

            // Below is the code that actually changes the colours
            // it loops through all the elements and changes their colours

            if (dark)
            {
                MainGrid.Background = darkLinearGradientBrush;
                LogoButtonsGrid.Background = new SolidColorBrush(darkColour);
                HomeBtn.Background = new SolidColorBrush(darkColour);
                AllGamesBtn.Background = new SolidColorBrush(darkColour);
                AboutBTN.Background = new SolidColorBrush(darkColour);
                SettingsBTN.Background = new SolidColorBrush(darkColour);
                RecentBtn.Background = new SolidColorBrush(darkColour);
            }
            else
            {
                MainGrid.Background = myLinearGradientBrush;
                LogoButtonsGrid.Background = new SolidColorBrush(lightColour);
                HomeBtn.Background = new SolidColorBrush(lightColour);
                AllGamesBtn.Background = new SolidColorBrush(lightColour);
                AboutBTN.Background = new SolidColorBrush(lightColour);
                SettingsBTN.Background = new SolidColorBrush(lightColour);
                RecentBtn.Background = new SolidColorBrush(lightColour);
            }

            //For each tabpanel, change each child items colour to white if dark mode is enabled
            foreach (var x in MainGrid.Children)
            {
                if (x is TabPanel tabPanel)
                {
                    foreach (var y in tabPanel.Children)
                    {
                        if (y is Grid grid)
                        {
                            foreach (var z in grid.Children)
                            {
                                if (z is TextBlock textBlock)
                                    textBlock.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);

                                if (z is TextBox textBox)
                                    textBox.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);
                                if (z is CheckBox checkBox)
                                    checkBox.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);

                            }
                        }
                    }
                }
            }
        }

        // Occupy the local list var "myGames" with the games
        // such that they are individually addressable (!!!)
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
            }
        }

        //Essentially the click even for the game covers
        private void StackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get the Game object associated with the clicked item
            var game = (sender as FrameworkElement)?.DataContext as Game;

            // Do something with the selected game object, such as showing more details in a new window
            if (game != null)
            {
                StartGame(game);
            }
        }

        //Using other methods, construct a launchCommand to be ran by Process.Start
        private void StartGame(Game game)
        {
            string LaunchCommand = "";

            // If PickEmulator fails, return
            if (PickEmulator(game, false) != "Invalid") LaunchCommand += PickEmulator(game, false) + " ";
            else return;

            // If FindGame fails, return
            if (FindGame(game) != "Invalid") LaunchCommand += FindGame(game);
            else return;

            //TODO: Remove this and uncomment Process.Start
            MessageBox.Show($"You clicked {game.Title}.\nLaunch command: {LaunchCommand}");
            //Process.Start(LaunchCommand);
        }

        // Returns the path to the correct emulator for a game OR return the emulator name for a game
        //TODO: Add support for the other emulators in our database (Also in the XAML)
        private string PickEmulator(Game game, bool retEmulator)
        {
            switch (game.Emulator)
            {
                case "RPCS3":
                    if (retEmulator) return "RPCS3";
                    return Odyssey.Properties.Settings.Default.pathRPCS3;
                case "Xenia":
                    if (retEmulator) return "Xenia";
                    return Odyssey.Properties.Settings.Default.pathXenia;
                case "PPSSPP":
                    if (retEmulator) return "PPSSPP";
                    return Odyssey.Properties.Settings.Default.pathPPSSPP;
                case "PCSX2":
                    if (retEmulator) return "PCSX2";
                    return Odyssey.Properties.Settings.Default.pathPCSX2;
                default:
                    return "Invalid";
            }
        }

        // Checks if the game is valid and if the game path is set and returns the result of FindFile for the game
        private string FindGame(Game game)
        {
            if (game == null) return null;

            //RPCS3 takes the folder as the game path, while the other emulators take the file
            if (PickEmulator(game, true) == "RPCS3") return FindFolder(pathGameFolder.Text, $"{game.Title}");

            //For any other emulator, return the file path
            else return FindFile(pathGameFolder.Text, game.Title);
        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Collapsed;
            Games.Visibility = Visibility.Collapsed;
        }

        //TODO: Verify settings - Check if the contents of the settings text boxes are valid as settings
        private bool VerifySettings()
        {
            // Check if the text boxes are empty
            bool TxtBxLengthCheck(TextBox t, string emulator)
            {
                if (t.Text.Length < 4)
                {
                    MessageBox.Show($"The {emulator} path is quite short. Is the path valid?");
                    return false;
                }
                return true;
            }

            if (!TxtBxLengthCheck(pathRPCS3TxtBx, "RPSC3")) return false;
            if (!TxtBxLengthCheck(pathXeniaTxtBx, "Xenia")) return false;
            if (!TxtBxLengthCheck(pathPPSSPPTxtBx, "PPSSPP")) return false;
            if (!TxtBxLengthCheck(pathPCSX2TxtBx, "PCSX2")) return false;
            if (!TxtBxLengthCheck(pathGameFolder, "game folder")) return false;

            return true;
        }

        //Save settings
        private void SaveSettings()
        {
            Odyssey.Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();
            Odyssey.Properties.Settings.Default.pathRPCS3 = pathRPCS3TxtBx.Text;
            Odyssey.Properties.Settings.Default.pathXenia = pathXeniaTxtBx.Text;
            Odyssey.Properties.Settings.Default.pathPPSSPP = pathPPSSPPTxtBx.Text;
            Odyssey.Properties.Settings.Default.pathPCSX2 = pathPCSX2TxtBx.Text;
            Odyssey.Properties.Settings.Default.pathGameFolder = pathGameFolder.Text;
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

            if (Odyssey.Properties.Settings.Default.pathPPSSPP is null)
                pathPPSSPPTxtBx.Text = "Unset";
            else
                pathPPSSPPTxtBx.Text = Odyssey.Properties.Settings.Default.pathPPSSPP;

            if (Odyssey.Properties.Settings.Default.pathPCSX2 is null)
                pathPCSX2TxtBx.Text = "Unset";
            else
                pathPCSX2TxtBx.Text = Odyssey.Properties.Settings.Default.pathPCSX2;

            if (Odyssey.Properties.Settings.Default.pathGameFolder is null)
                pathGameFolder.Text = "Unset";
            else
                pathGameFolder.Text = Odyssey.Properties.Settings.Default.pathGameFolder;
        }

        private void HomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
        }

        private void AllGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
            About.Visibility = Visibility.Collapsed;
        }

        //Settings Apply Button
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            VerifySettings();
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

        private void pathPPSSPPTxtBx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathPPSSPPTxtBx);
        }

        private void pathPCSX2TxtBx_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathPCSX2TxtBx);
        }

        // Generic function to open a file picker and store the result in a text box
        private void FilePicker(TextBox t)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == false) return;
            t.Text = ofd.FileName;
        }

        // Open a folder picker, store the resulting path in the text box
        private void PathGameFolder_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathGameFolder);
        }

        private void GameFolderPath_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //TODO: Replace this with a folder picker
            FilePicker(pathGameFolder);
        }

        // Generic function to compare two strings and return a likeness percentage
        public static double CompareStrings(string str1, string str2)
        {
            // Split the strings into words
            var words1 = str1.Split(' ', '-', '_', '.');
            var words2 = str2.Split(' ', '-', '_', '.');

            // Create a list to store the matching words
            var matchingWords = new List<string>();

            // Loop through the words and compare them
            foreach (var word1 in words1)
            {
                foreach (var word2 in words2)
                {
                    // If the words match, add them to the list, the check also ignores case
                    if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingWords.Add(word1);
                    }
                }
            }

            // Set a max length for the strings
            var maxLength = Math.Max(words1.Length, words2.Length);

            // Calculate the likeness percentage
            var likeness = (double)matchingWords.Count / maxLength * 100;

            return likeness;
        }

        // Generic function to search a directory for a file
        public static string FindFile(string directory, string fileName)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var files = directoryInfo.GetFiles();
            double expectedLikeness = 70;

            //If the file name is short, reduce the expected likeness such
            //that we are more likely to get a match. (See "Halo 3")
            if (fileName.Length < 7) expectedLikeness -= 20;

            foreach (var file in files)
            {
                if (CompareStrings(file.Name, fileName) > expectedLikeness)
                    return file.FullName;
            }

            return "Invalid";
        }

        //Generic function to search a directory for another directory
        public static string FindFolder(string directory, string folderName)
        {
            var folderInfo = new DirectoryInfo(directory);
            var folder = folderInfo.GetDirectories();
            double expectedLikeness = 90;

            //If the file name is short, reduce the expected likeness such
            //that we are more likely to get a match. (See "Halo 3")
            if (folderName.Length < 7) expectedLikeness -= 20;

            foreach (var dir in folder)
            {
                if (CompareStrings(dir.Name, folderName) > expectedLikeness)
                    return dir.FullName;
            }

            return null;
        }

        private void AboutBTN_OnClick(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            About.Visibility = Visibility.Visible;
        }
    }
}
