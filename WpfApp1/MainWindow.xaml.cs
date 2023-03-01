using API;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static API.Api;

namespace Odyssey
{
    public partial class MainWindow
    {
        //Contains games stored in a sane fashion
        public List<Game> MyGames = new();

        //Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient Client = new();

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
            OccupyListVar(await ProcessGamesData(Client));

            //This goes here only because it loads too early anywhere else
            DataContext = new GameViewModel(MyGames);
            GameListView.ItemsSource = MyGames;
        }

        // Allow for switching between light and dark themes
        // Must be added soon after InitializeComponent (Currently part of LoadSettings)
        private void Theming(bool dark)
        {
            // Our colour schemes are defined here, so we can easily change them

            //Dark theme colours:
            LinearGradientBrush darkLinearGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 3),
                EndPoint = new Point(1, 1)
            };
            darkLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Black, 0.1));
            darkLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Gray, 2.5));

            Color darkColour = (Color)ColorConverter.ConvertFromString("#5A5A5A");
            Color darkColourText = Colors.White;

            //Light theme colours:
            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 3),
                EndPoint = new Point(1, 1)
            };
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
            //TODO: Improve variable naming here
            foreach (var x in MainGrid.Children)
            {
                if (x is TabPanel tabPanel)
                {
                    foreach (var y in tabPanel.Children)
                    {
                        if (y is not Grid grid) continue;
                        foreach (var z in grid.Children)
                        {
                            switch (z)
                            {
                                case TextBlock textBlock:
                                    textBlock.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);
                                    break;
                                case TextBox textBox:
                                    textBox.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);
                                    break;
                                case CheckBox checkBox:
                                    checkBox.Foreground =
                                        dark
                                            ? new SolidColorBrush(darkColourText)
                                            : new SolidColorBrush(lightColourText);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        // Occupy the local list var "myGames" with the games
        // such that they are individually addressable (!!!)
        // This is only called once
        //TODO: More efficient way of doing this?
        private void OccupyListVar(List<GamesList> list)
        {
            foreach (var x in list)
            {
                MyGames.Add(new Game()
                {
                    Title = x.Title,
                    Year = x.Year,
                    Description = x.Description,
                    Image = x.Image,
                    Consoles = x.Consoles,
                    Emulator = x.Emulator
                });
            }
        }

        //Using other methods, construct a launchCommand to be ran by Process.Start
        private void StartGame(Game game)
        {
            var launchCommand = "";
            bool pickEmulatorFailed = false, findGameFailed = false;
            string msg1 = "Emulator valid.", msg2 = "Game file present";

            // If PickEmulator fails, return
            string lEmulator = PickEmulator(game); // Prevents calling the method twice
            if (lEmulator != "Invalid") launchCommand += lEmulator + " ";
            else pickEmulatorFailed = true;

            // If FindGame fails, return
            string lGame = FindGame(game).ToString(); // Prevents calling the method twice
            if (lGame != "Invalid") launchCommand += lGame;
            else findGameFailed = true;

            if (pickEmulatorFailed)
                msg1 = $"Emulator {game.Emulator} not found.";

            if (findGameFailed)
                msg2 = $"Game file not found.";

            //TODO: Uncomment Process.Start
            System.Diagnostics.Trace.WriteLine($"[INFO]: {game.Title}. Launch command \"{launchCommand}\". {msg1} {msg2}");
            //Process.Start(LaunchCommand);
        }

        // Returns the path to the correct emulator for a game OR return the emulator name for a game
        //TODO: Add support for the other emulators in our database (Also in the XAML)
        private static string PickEmulator(Game game)
        {
            return game.Emulator.ToLower() switch
            {
                "rpcs3" => Odyssey.Properties.Settings.Default.pathRPCS3,
                "xenia" => Odyssey.Properties.Settings.Default.pathXenia,
                "ppsspp" => Odyssey.Properties.Settings.Default.pathPPSSPP,
                "pcsx2" => Odyssey.Properties.Settings.Default.pathPCSX2,
                "epsxe" => Odyssey.Properties.Settings.Default.pathEPSXE,
                "snes9x" => Odyssey.Properties.Settings.Default.pathSNES9x,
                _ => "Invalid",
            };
        }

        // Checks if the game is valid and if the game path is set and returns the result of FindFile for the game
        private string FindGame(Game game)
        {
            //RPCS3 takes the folder as the game path, while the other emulators take the file
            if (game.Emulator == "RPCS3")
                return FindFolder(pathGameFolder.Text, game.Title);

            return FindFile(pathGameFolder.Text, game.Title);
        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            Settings.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Collapsed;
            Games.Visibility = Visibility.Collapsed;

            //This shouldn't really be here but it allows the user to see if the settings are valid
            //as the user opens the settings page
            VerifySettings(); 
        }

        // Verify each setting we have
        private void VerifySettings()
        {
            VerifySetting(pathRPCS3TxtBx, "RPSC3", true, "rpcs3.exe");
            VerifySetting(pathXeniaTxtBx, "Xenia", true, "xenia.exe");
            VerifySetting(pathPPSSPPTxtBx, "PPSSPP", true, "PPSSPPWindows64.exe");
            VerifySetting(pathPCSX2TxtBx, "PCSX2", true, "pcsx2.exe");
            VerifySetting(pathEPSXETxtBx, "EPSXE", true, "epsxe.exe");
            VerifySetting(pathEPSXETxtBx, "SNES9x", true, "snes9x-x64.exe");
            VerifySetting(pathGameFolder, "game folder");
        }

        // Generic method to allow for simple verification of individual settings
        //TODO: Verify that the emulator name is valid or remove the emulator parameter
        private static void VerifySetting(TextBox t, string emulator, bool executable = false, string executableName = "")
        {
            // Colours for the text boxes for case of error or no error
            Color errorColour = Color.FromArgb(80, 255, 0, 0);
            Color noColour = Color.FromArgb(00, 0, 0, 0);

            // Do some checks on the provided data, return false upon failure, else true
            bool TxtBxCheck()
            {
                // Some checks
                if (t.Text.Length < 4) return false; // No actual path should be less characters than this
                switch (executable)
                {
                    case true when !t.Text.EndsWith(".exe"):
                    // Also make sure we have the correct executable (case insensitive)
                    case true when !t.Text.ToLower().EndsWith(executableName.ToLower()):
                    // If we don't expect an executable but get one anyway
                    case false when t.Text.EndsWith(".*"):
                        return false; // If we expect an executable, make sure we get one
                    default:
                        return Path.Exists(t.Text); // If the path or executable doesn't exist
                }
            }

            // If the method above returns false for any of its checks,
            // highlight the textbox with the error colour specific at the beginning of this method
            // else apply the noColour colour (this exists to undo errorColour)
            t.Background = !TxtBxCheck() ? new SolidColorBrush(errorColour) : new SolidColorBrush(noColour);
        }

        //Save settings
        private void SaveSettings()
        {
            Odyssey.Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

            //For each textbox named "path*TxtBx", assign the text to the corresponding setting
            foreach (var g in Settings.Children)
            {
                if(g is Grid grid)
                    foreach (var t in grid.Children.OfType<TextBox>())
                    {
                        if (t.Name.StartsWith("path"))
                        {
                            if(t.Name.EndsWith("TxtBx"))
                                t.Name = t.Name.Remove(t.Name.Length - 5);

                            Odyssey.Properties.Settings.Default[t.Name] = t.Text;
                        }
                    }
            }

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

            pathRPCS3TxtBx.Text = Odyssey.Properties.Settings.Default.pathRPCS3 is null ? "Unset" : Odyssey.Properties.Settings.Default.pathRPCS3;
            pathXeniaTxtBx.Text = Odyssey.Properties.Settings.Default.pathXenia is null ? "Unset" : Odyssey.Properties.Settings.Default.pathXenia;
            pathPPSSPPTxtBx.Text = Odyssey.Properties.Settings.Default.pathPPSSPP is null ? "Unset" : Odyssey.Properties.Settings.Default.pathPPSSPP;
            pathPCSX2TxtBx.Text = Odyssey.Properties.Settings.Default.pathPCSX2 is null ? "Unset" : Odyssey.Properties.Settings.Default.pathPCSX2;
            pathEPSXETxtBx.Text = Odyssey.Properties.Settings.Default.pathEPSXE is null ? "Unset" : Odyssey.Properties.Settings.Default.pathEPSXE;
            pathSNES9xTxtBx.Text = Odyssey.Properties.Settings.Default.pathSNES9x is null ? "Unset" : Odyssey.Properties.Settings.Default.pathSNES9x;
            pathGameFolder.Text = Odyssey.Properties.Settings.Default.pathGameFolder is null ? "Unset" : Odyssey.Properties.Settings.Default.pathGameFolder;
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
                foreach (var word2 in words2)
                    // If the words match, add them to the list, the check also ignores case
                    if (string.Equals(word1, word2, StringComparison.OrdinalIgnoreCase))
                        matchingWords.Add(word1);

            // Set a max length for the strings
            var maxLength = Math.Min(words1.Length, words2.Length);

            // Calculate the likeness percentage
            var likeness = (double)matchingWords.Count / maxLength * 100;

            if(likeness > 0)
                System.Diagnostics.Trace.WriteLine($"[INFO]: Likeness: {likeness}. {str1} VS {str2}");

            return likeness;
        }

        // Generic function to search a directory for a file
        public static string FindFile(string directory, string fileName)
        {
            var directoryInfo = new DirectoryInfo(directory);
            var files = directoryInfo.GetFiles();

            //If the file name is short, reduce the expected likeness such
            //that we are more likely to get a match. (See "Halo 3")
            double expectedLikeness = fileName.Length switch
            {
                < 3 => 55,
                < 4 => 60,
                < 7 => 65,
                > 12 => 70,
                _ => 60
            };

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

            //If the file name is short, reduce the expected likeness such
            //that we are more likely to get a match. (See "Halo 3")
            double expectedLikeness = folderName.Length switch
            {
                < 4 => 20,
                < 7 => 30,
                > 12 => 80,
                _ => 60
            };

            foreach (var dir in folder)
                if (CompareStrings(dir.Name, folderName) > expectedLikeness)
                    return dir.FullName;

            return "Invalid";
        }

        private void AboutBTN_OnClick(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            About.Visibility = Visibility.Visible;
        }

        //Essentially the click even for the game covers
        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get the Game object associated with the clicked item
            // Do something with the selected game object, such as showing more details in a new window
            if ((sender as FrameworkElement)?.DataContext is Game game)
            {
                StartGame(game);
            }
        }

        private void SearchTxtBx_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TitleFilter(SearchTxtBx.Text);
        }

        //TODO: Better filtering system with more generic functions

        //Filters the listview based on the search term (title)
        private void TitleFilter(string term = "")
        {
            var filteredList = MyGames.Where(game => game.Title.ToLower().Contains(term.ToLower())).ToList();
            ApplyFilteredList(filteredList);
        }

        //Filter the listview based on the emulator
        private void EmulatorFilter(string emulator = "")
        {
            var filteredList = MyGames.Where(game => game.Emulator.ToLower().Contains(emulator.ToLower())).ToList();
            ApplyFilteredList(filteredList);
        }

        //Filter the listview based on the console
        private void ConsoleFilter(string console = "")
        {
            var filteredList = MyGames.Where(game => game.Consoles.ToLower().Contains(console.ToLower())).ToList();
            ApplyFilteredList(filteredList);
        }

        //Add the filtered list to the listview
        private void ApplyFilteredList(List<Game> filteredList)
        {
            GameListView.ItemsSource = null;
            GameListView.ItemsSource = filteredList;
            DataContext = filteredList;
        }

        //TODO: Clean this up
        private void EmulatorCbBx_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? a = EmulatorCbBx.SelectedValue.ToString();
            var b = a?.Split(':', 2);
            var c = b[1].Substring(1);
            if (c == "All")
                TitleFilter();
            else
                EmulatorFilter(c);
        }

        private void PathSNES9xTxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathSNES9xTxtBx);
        }

        private void PathEPSXETxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathEPSXETxtBx);
        }
    }
}
