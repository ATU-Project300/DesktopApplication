using API;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using static API.Api;

namespace Odyssey
{
    public partial class MainWindow
    {
        //Contains games stored in a sane fashion
        public List<Game> MyGames = new();

        //Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient Client = new();

        public bool ready = false;

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
            ready = true;
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
                if (x is not TabPanel tabPanel) continue;
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
            string lGame = FindGame(game); // Prevents calling the method twice
            if (lGame != "Invalid") launchCommand += lGame;
            else findGameFailed = true;

            if (pickEmulatorFailed)
                msg1 = $"Emulator {game.Emulator} not found.";

            if (findGameFailed)
                msg2 = "Game file not found.";

            //TODO: Uncomment Process.Start
            System.Diagnostics.Trace.WriteLine($"[INFO]: {game.Title}. Launch command \"{launchCommand}\". {msg1} {msg2}");
            //Process.Start(LaunchCommand);
        }

        // Returns the path to the correct emulator for a game OR return the emulator name for a game
        private static string PickEmulator(Game game)
        {
            //Odyssey.Properties.Settings.Default[t.Name] = t.Text;
            string setting = "path" + game.Emulator;
            //verify the setting exists before returning it
            if (Properties.Settings.Default.Properties.Cast<SettingsProperty>().Any(x => x.Name == setting))
                return (string)Properties.Settings.Default[setting];
            else return "Invalid";
        }

        // Checks if the game is valid and if the game path is set and returns the result of FindFile for the game
        private string FindGame(Game game)
        {
            //RPCS3 takes the folder as the game path, while the other emulators take the file
            //TODO: Find a different way about this such that we aren't hardcoding the emulator name
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
        //TODO: Make this loop through the settings instead of hardcoding each one 
        // (This is the last place that needs to be changed!!!)
        private void VerifySettings()
        {
            VerifySetting(pathRPCS3TxtBx, "RPSC3", true, "rpcs3.exe");
            VerifySetting(pathXeniaTxtBx, "Xenia", true, "xenia.exe");
            VerifySetting(pathPPSSPPTxtBx, "PPSSPP", true, "PPSSPPWindows64.exe");
            VerifySetting(pathPCSX2TxtBx, "PCSX2", true, "pcsx2.exe");
            VerifySetting(pathEPSXETxtBx, "EPSXE", true, "epsxe.exe");
            VerifySetting(pathSNES9xTxtBx, "SNES9x", true, "snes9x-x64.exe");
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
            Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

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

                            Properties.Settings.Default[t.Name] = t.Text;
                        }
                    }
            }

            Properties.Settings.Default.Save();

        }

        private void LoadSettings()
        {
            darkModeChkBx.IsChecked = Properties.Settings.Default.DarkMode;
            Theming(darkModeChkBx.IsChecked.Value);

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

                            t.Text = Properties.Settings.Default[t.Name].ToString() is null ? "Unset" : Properties.Settings.Default[t.Name].ToString();
                        }
                    }
            }
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
                System.Diagnostics.Trace.WriteLine($"[INFO]: Likeness: {likeness}. {str1} VS {str2}.");

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
            BigFilter(sender, null);
        }

        //TODO: Better filtering system with more generic functions

        private void BigFilter(object sender, SelectionChangedEventArgs e)
        {
            if (!ready)
            {
                return;
            }

            var emulator = EmulatorCbBx.SelectedValue.ToString()?.Split(':', 2)[0];
            var year = YearCbBx.SelectedValue.ToString()?.Split(':',2)[0];
            var console = ConsoleCbBx.SelectedValue.ToString()?.Split(':',2)[0];

            System.Diagnostics.Trace.WriteLine($"[INFO]: Selected Emulator {emulator}");
            System.Diagnostics.Trace.WriteLine($"[INFO]: Selected Year {year}");
            System.Diagnostics.Trace.WriteLine($"[INFO]: Selected Console {console}");

            var filteredList = MyGames;
            bool bSearch, bEmulator, bYear, bConsole;

            //First filter by title
            if (SearchTxtBx.Text.Length > 1)
            {
                filteredList = filteredList.Where(game => game.Title.ToLower().Contains(SearchTxtBx.Text.ToLower())).ToList();
                bSearch = true;
            }
            else
                bSearch = false;

            System.Diagnostics.Trace.WriteLine($"[INFO]: filteredList {filteredList.Count}");

            //Then filter by emulator
            if (emulator != "All")
            {
                filteredList = filteredList.Where(game => game.Emulator.ToLower().Contains(emulator.ToLower())).ToList();
                bEmulator = true;
            }
            else
                bEmulator = false;

            System.Diagnostics.Trace.WriteLine($"[INFO]: filteredList {filteredList.Count}");

            //Then filter by year provided one is selected
            if (year != "All")
            {
                filteredList = filteredList.Where(game => game.Year.ToString().Equals(year)).ToList();
                bYear = true;
            }
            else
                bYear = false;

            System.Diagnostics.Trace.WriteLine($"[INFO]: filteredList {filteredList.Count}");

            //Then filter by console
            if (console != "All")
            {
                filteredList = filteredList.Where(game => game.Consoles.ToLower().Contains(console.ToLower())).ToList();
                bConsole = true;
            }
            else
                bConsole = false;

            System.Diagnostics.Trace.WriteLine($"[INFO]: filteredList {filteredList.Count}");

            if(!bSearch && !bEmulator && !bConsole && !bYear)
                ApplyFilteredList(MyGames);
            else
                ApplyFilteredList(filteredList);
        }

        //Add the filtered list to the listview
        private void ApplyFilteredList(List<Game> filteredList)
        {
            GameListView.ItemsSource = null;
            GameListView.ItemsSource = filteredList;
            DataContext = filteredList;
        }

        //TODO: Better name for this
        private void SummonFilePicker(object sender, MouseButtonEventArgs e)
        {
            FilePicker(sender as TextBox);
        }

        // Scale image on hover
        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Image ?image = sender as Image;
            if (image != null)
            {
                image.Margin = new Thickness(4,2,2,2);
                image.Width *= 1.25;
                image.Height *= 1.25;
            }
        }

        // Scale image back to normal on mouse leave
        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Image ?image = sender as Image;
            if (image != null)
            {
                image.Margin = new Thickness(12,5,5,5);
                image.Width = 100;
                image.Height = 120;
            }
        }


        // When the Emulator ComboBox loads
        private void EmulatorCbBx_OnLoaded(object sender, RoutedEventArgs e)
        {
            List<string> emuList = new List<string>(); // Create a new list of strings
            emuList.Add("All"); // Add the "All" option to the list

            // Loop through all the games and add the emulator to the list if it doesn't already exist
            foreach (var game in MyGames)
            {
                if(!emuList.Contains(game.Emulator))
                    emuList.Add(game.Emulator);
            }

            // Sort the list alphabetically
            emuList.Sort();

            // Set the ComboBox's items source to the list
            EmulatorCbBx.ItemsSource = emuList;
        }

        // When the Year ComboBox loads
        private void YearCbBx_OnLoaded(object sender, RoutedEventArgs e)
        {
            List<string> yearList = new List<string>(); // Create a new list of strings
            yearList.Add("All"); // Add the "All" option to the list

            // Loop through all the games and add the year to the list if it doesn't already exist
            foreach (var game in MyGames)
            {
                if(!yearList.Contains(game.Year.ToString()))
                    yearList.Add(game.Year.ToString());
            }

            //sort the year list in descending order excluding the "All" option
            yearList.Sort();
            yearList.Reverse();

            // Set the ComboBox's items source to the list
            YearCbBx.ItemsSource = yearList;
        }

        // When the Console ComboBox loads
        private void ConsoleCbBx_OnLoaded(object sender, RoutedEventArgs e)
        {
            List<string> consoleList = new List<string>(); // Create a new list of strings
            consoleList.Add("All"); // Add the "All" option to the list

            // Loop through all the games and add the console to the list if it doesn't already exist
            foreach (var game in MyGames)
            {
                if(!consoleList.Contains(game.Consoles))
                    consoleList.Add(game.Consoles);
            }

            // Sort the list alphabetically
            consoleList.Sort();

            // Set the ComboBox's items source to the list
            ConsoleCbBx.ItemsSource = consoleList;
        }
    }
}
