using API;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static API.Api;

namespace Odyssey
{
    public partial class MainWindow
    {
        // Contains games stored in a sane fashion
        public List<Game> MyGames = new();

        public Game? SelectedGame;

        // Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient Client = new();

        // Reflect the state of the application
        public bool Ready;

        // For image scaling
        private ScaleTransform _scaleTransform = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings();
            InitializeApiData();
        }


        // From the API, use the games data to occupy the myGames List
        public async void InitializeApiData()
        {
            OccupyListVar(await ProcessGamesData(Client));

            // This goes here only because it loads too early anywhere else
            DataContext = new GameViewModel(MyGames);
            ApplyFilteredList(MyGames);
            Ready = true;
        }

        // Allow for switching between light and dark themes
        // Must be added soon after InitializeComponent (Currently part of LoadSettings)
        private void Theming(bool dark)
        {
            // Our colour schemes are defined here, so we can easily change them

            // Dark theme colours:
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

            // Light theme colours:
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
                GamesBtn.Background = new SolidColorBrush(darkColour);
                EmulatorsBtn.Background = new SolidColorBrush(darkColour);
                AboutBtn.Background = new SolidColorBrush(darkColour);
                SettingsBtn.Background = new SolidColorBrush(darkColour);
                RecentBtn.Background = new SolidColorBrush(darkColour);
            }
            else
            {
                MainGrid.Background = myLinearGradientBrush;
                LogoButtonsGrid.Background = new SolidColorBrush(lightColour);
                GamesBtn.Background = new SolidColorBrush(lightColour);
                EmulatorsBtn.Background = new SolidColorBrush(lightColour);
                AboutBtn.Background = new SolidColorBrush(lightColour);
                SettingsBtn.Background = new SolidColorBrush(lightColour);
                RecentBtn.Background = new SolidColorBrush(lightColour);
            }

            // For each tabpanel, change each child items colour to white if dark mode is enabled
            // TODO: Improve variable naming here
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
        // TODO: More efficient way of doing this?
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
                    Console = x.Console,
                    Emulator = x.Emulator,
                    Rating = x.Rating
                });
            }
        }

        // Using other methods, construct a launchCommand to be ran by Process.Start
        private void StartGame(Game game)
        {
            bool pickEmulatorFailed = false, findGameFailed = false;
            string msg1 = "Emulator valid.", msg2 = "Game file present";
            string lGame;

            // If PickEmulator fails, return
            string lEmulator = PickEmulator(game); // Prevents calling the method twice
            if (lEmulator == "Invalid")
                pickEmulatorFailed = true;

            // If FindGame fails, return
            lGame = FindGame(game); // Prevents calling the method twice
            if (lGame == "Invalid")
                findGameFailed = true;

            if (pickEmulatorFailed)
                msg1 = $"Emulator {game.Emulator} not found.";

            if (findGameFailed)
                msg2 = "Game file not found.";

            // TODO: Uncomment Process.Start
            Trace.WriteLine($"[INFO]: {game.Title}. Launch command {lEmulator} {lGame}");
            Trace.WriteLine($"[INFO]: {msg1} {msg2}");

            if (pickEmulatorFailed || findGameFailed)
            {
                if (findGameFailed)
                    MessageBox.Show($"{game.Title} file not found :( \nIs the ROM in your game folder?", "Error");
                if (pickEmulatorFailed)
                    MessageBox.Show($"Emulator {game.Emulator} not found :( \nMake sure it is installed and added in settings", "Error");

                Trace.WriteLine($"[ERROR]: {msg1} {msg2}");
            }
            else
            {
                Process.Start($"\"{lEmulator}\"", $"\"{lGame}\"");
            }
        }

        // Returns the path to the correct emulator for a game
        private static string PickEmulator(Game game)
        {
            var setting = "path" + game.Emulator;

            // Verify the setting exists before returning it
            if (Properties.Settings.Default.Properties.Cast<SettingsProperty>().Any(x => x.Name == setting))
            {
                var emu = (string)Properties.Settings.Default[setting];
                if (Path.Exists(emu))
                    return emu;
                else return "Invalid";
            }
            else return "Invalid";
        }

        // Checks if the game is valid and if the game path is set and returns the result of FindFile for the game
        private string FindGame(Game game)
        {
            // RPCS3 takes the folder as the game path, while the other emulators take the file
            // TODO: Find a different way about this such that we aren't hard coding the emulator name
            if (game.Emulator == "RPCS3")
                return FindFolder(pathGameFolder.Text, game.Title);

            return FindFile(pathGameFolder.Text, game.Title);
        }

        // Verify each setting we have
        // TODO: Make this loop through the settings instead of hard coding each one 
        // (This is the last place that needs to be changed!!!)
        private void VerifySettings()
        {
            VerifySetting(pathRPCS3TxtBx, true, "rpcs3.exe");
            VerifySetting(pathXeniaTxtBx, true, "xenia.exe");
            VerifySetting(pathPPSSPPTxtBx, true, "PPSSPPWindows64.exe");
            VerifySetting(pathPCSX2TxtBx, true, "pcsx2.exe");
            VerifySetting(pathDuckStationTxtBx, true, "duckstation-qt-x64-ReleaseLTCG.exe");
            VerifySetting(pathSNES9xTxtBx, true, "snes9x-x64.exe");
            VerifySetting(pathGameFolder, executableName: "game folder");
        }

        // Generic method to allow for simple verification of individual settings
        private static void VerifySetting(TextBox t, bool executable = false, string executableName = "")
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

        // Save settings
        private void SaveSettings()
        {
            Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();

            // For each textbox named "path*TxtBx", assign the text to the corresponding setting
            foreach (var g in Settings.Children)
            {
                if (g is Grid grid)
                    foreach (var t in grid.Children.OfType<TextBox>())
                    {
                        if (t.Name.StartsWith("path"))
                        {
                            if (t.Name.EndsWith("TxtBx"))
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

            // For each textbox named "path*TxtBx", assign the text to the corresponding setting
            foreach (var g in Settings.Children)
            {
                if (g is Grid grid)
                    foreach (var t in grid.Children.OfType<TextBox>())
                    {
                        if (t.Name.StartsWith("path"))
                        {
                            if (t.Name.EndsWith("TxtBx"))
                                t.Name = t.Name.Remove(t.Name.Length - 5);

                            t.Text = Properties.Settings.Default[t.Name].ToString() is null ? "Unset" : Properties.Settings.Default[t.Name].ToString();
                        }
                    }
            }
        }

        // Settings Apply Button
        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            VerifySettings();
            SaveSettings();
            LoadSettings();
        }

        // Generic function to open a file picker and store the result in a text box
        private void FilePicker(TextBox? t)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == false) return;
            t.Text = ofd.FileName;
        }

        // Open a folder picker, store the resulting path in the text box
        private void GameFolderPath_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // TODO: Replace this with a folder picker
            FilePicker(pathGameFolder);
        }

        // Generic function to compare two strings and return a likeness percentage
        public static double CompareStrings(string str1, string? str2)
        {
            // Split the strings into words
            var words1 = str1.Split(' ', '-', '_', '.');
            var words2 = str2?.Split(' ', '-', '_', '.');

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

            if (likeness > 0)
                Trace.WriteLine($"[INFO]: Likeness: {likeness}. {str1} VS {str2}.");

            return likeness;
        }

        // Generic function to search a directory for a file
        public static string FindFile(string? directory, string? fileName)
        {
            if (directory != null && directory.Length > 0)
            {
                var directoryInfo = new DirectoryInfo(directory);
                var files = directoryInfo.GetFiles();

                // If the file name is short, reduce the expected likeness such
                // that we are more likely to get a match. (See "Halo 3")
                if (fileName != null)
                {
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
                }
            }

            return "Invalid";
        }

        // Generic function to search a directory for another directory
        public static string FindFolder(string? directory, string? folderName)
        {
            if (directory != null && directory.Length > 0)
            {
                var folderInfo = new DirectoryInfo(directory);
                var folder = folderInfo.GetDirectories();

                // If the file name is short, reduce the expected likeness such
                // that we are more likely to get a match. (See "Halo 3")
                if (folderName != null)
                {
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
                }
            }

            return "Invalid";
        }

        // Hides all other panels which aren't the one associated with the clicked button
        private void HideOtherPanels(object sender, RoutedEventArgs e)
        {
            // Get the sender as a button, get its name excluding "Btn"
            var name = (sender as Button)?.Name.Split("Btn", 2)[0];
            
            // Loop through all the TabPanels and hide the ones that aren't associated with the button
            // and show the ones that are
            foreach (var x in MainGrid.Children)
            {
                if (x is not TabPanel tabPanel) continue;
                if (!tabPanel.Name.StartsWith(name))
                    tabPanel.Visibility = Visibility.Collapsed;
                else
                {
                    tabPanel.Visibility = Visibility.Visible;
                    // This shouldn't really be here but it allows the user to see if the settings are valid
                    // as the user opens the settings page
                    if (tabPanel.Name == "Settings")
                        VerifySettings();
                }
            }
        }


        // Essentially the click even for the game covers
        // TODO: Make this less bloated
        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Get the Game object associated with the clicked item
            // Do something with the selected game object, such as showing more details in a new window
            if ((sender as FrameworkElement)?.DataContext is Game game)
            {
                SelectedGame = game;
                DetailsView.Visibility = Visibility.Visible;
                Games.Visibility = Visibility.Collapsed;
                Settings.Visibility = Visibility.Collapsed;
                About.Visibility = Visibility.Collapsed;

                if (SelectedGame.Image != null) DetailsGameImage.Source = new BitmapImage(new Uri(SelectedGame.Image));
                Trace.WriteLine($"[INFO]: Rating {SelectedGame.Rating}");
                DetailsGameImage.Width = 250;
                DetailsGameTitle.Text = SelectedGame.Title;
                DetailsGameYear.Text = SelectedGame.Year.ToString();
                DetailsGameRating.Text = SelectedGame.Rating.ToString();
                DetailsGameConsole.Text = SelectedGame.Console;
                DetailsGameDescription.Text = SelectedGame.Description;
                DetailsGamePlayButton.Content = $"Play {SelectedGame.Title} on {SelectedGame.Emulator}";
            }
        }

        private void SearchTxtBx_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            BigFilter(sender, null);
        }

        // TODO: Better filtering system with more generic functions
        private void BigFilter(object sender, SelectionChangedEventArgs? e)
        {
            // If the function is called before the UI is ready, return
            if (!Ready)
            {
                return;
            }

            // Get the selected values from the combo boxes
            var emulator = EmulatorCbBx.SelectedValue.ToString()?.Split(':', 2)[0];
            var year = YearCbBx.SelectedValue.ToString()?.Split(':', 2)[0];
            var console = ConsoleCbBx.SelectedValue.ToString()?.Split(':', 2)[0];

            // Display the selected values in the debug console
            Trace.WriteLine($"[INFO]: Selected Emulator {emulator}");
            Trace.WriteLine($"[INFO]: Selected Year {year}");
            Trace.WriteLine($"[INFO]: Selected Console {console}");

            // Filter from the full list of games
            var filteredList = MyGames;

            // Indicate the status of each filter (if it has been applied)
            bool bSearch, bEmulator, bYear, bConsole;

            // Filter by title if the search box is not empty
            // The comments apply to the rest of the filters
            if (SearchTxtBx.Text.Length > 1)
            {
                // Use a LINQ query to filter the list
                filteredList = filteredList.Where(game => game.Title != null && game.Title.ToLower().Contains(SearchTxtBx.Text.ToLower())).ToList();
                // Mark the filter as applied
                bSearch = true;
            }
            else
                // Mark the filter as not applied
                bSearch = false;

            Trace.WriteLine($"[INFO]: filteredList stage text search {filteredList.Count}");

            // Filter by emulator
            if (emulator != "All")
            {
                filteredList = filteredList.Where(game => game.Emulator != null && game.Emulator.ToLower().Equals(emulator?.ToLower())).ToList();
                bEmulator = true;
            }
            else
                bEmulator = false;

            Trace.WriteLine($"[INFO]: filteredList stage emulator filter {filteredList.Count}");

            // Filter by year provided one is selected
            if (year != "All")
            {
                filteredList = filteredList.Where(game => game.Year.ToString().Equals(year)).ToList();
                bYear = true;
            }
            else
                bYear = false;

            Trace.WriteLine($"[INFO]: filteredList stage year filter {filteredList.Count}");

            // Filter by console
            if (console != "All")
            {
                filteredList = filteredList.Where(game => game.Console != null && game.Console.ToLower().Equals(console?.ToLower())).ToList();
                bConsole = true;
            }
            else
                bConsole = false;

            // Output the number of items in the filtered list
            Trace.WriteLine($"[INFO]: filteredList stage console filter {filteredList.Count}");

            // If no filters have been applied, display the full list
            if (!bSearch && !bEmulator && !bConsole && !bYear)
                ApplyFilteredList(MyGames);
            else
                // Else display the list which has been filtered
                ApplyFilteredList(filteredList);
        }

        // Apply a List of Games to the ListView
        private void ApplyFilteredList(List<Game> list)
        {
            GameListView.ItemsSource = null;
            GameListView.ItemsSource = list;
            DataContext = list;
        }

        // TODO: Better name for this (could remove and sacrifice the generic FilePicker function)
        private void SummonFilePicker(object sender, MouseButtonEventArgs e)
        {
            FilePicker(sender as TextBox);
        }

        // Animated scaling of image when mouse is over it
        private void UIElement_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                _scaleTransform = new ScaleTransform(1, 1);
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = _scaleTransform;

                var animation = new DoubleAnimation
                {
                    To = 1.32,
                    Duration = new Duration(TimeSpan.FromSeconds(0.25))
                };
                _scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                _scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            }
        }

        // Scale image back to normal on mouse leave (opposite of above)
        private void UIElement_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                _scaleTransform = new ScaleTransform(1.25, 1.25);
                image.RenderTransformOrigin = new Point(0.5, 0.5);
                image.RenderTransform = _scaleTransform;

                var animation = new DoubleAnimation
                {
                    To = 1,
                    Duration = new Duration(TimeSpan.FromSeconds(0.25))
                };
                _scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                _scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            }
        }

        private void OccupyFilter(object sender, RoutedEventArgs e)
        {
            // Get the name of the ComboBox from sender
            var name = (sender as ComboBox)?.Name;
            var property = name?.Substring(0, name.Length - 4);

            var list = new List<string> { "All" };

            foreach (var game in MyGames)
            {
                if (property != null)
                {
                    var a = game.GetType().GetProperty(property)?.GetValue(game);
                    if (!list.Contains(a))
                        list.Add(((string)a)!);
                }
            }

            // Sort the list alphabetically
            list.Sort();

            // Set the ComboBox's items source to the list
            ((sender as ComboBox)!).ItemsSource = list;

        }

        // Upon loading the ComboBox, occupy a list to contain every year present in the MyGames list.
        private void YearCbBx_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Create a new list of strings and add the "All" option to the list
            List<string> yearList = new List<string> { "All" };

            // Loop through all the games and add the year to the list if it doesn't already exist
            foreach (var game in MyGames)
            {
                if (!yearList.Contains(game.Year.ToString()))
                    yearList.Add(game.Year.ToString());
            }

            // Sort the year list in descending order excluding the "All" option
            yearList.Sort();
            yearList.Reverse();

            // Set the ComboBox's items source to the list
            YearCbBx.ItemsSource = yearList;
        }

        private void DetailsCloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Visible;
            About.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Collapsed;
            DetailsView.Visibility = Visibility.Collapsed;
        }

        private void DetailsGamePlayButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedGame != null) StartGame(SelectedGame);
        }

        // Disallow selecting the filters in the ComboBox as items
        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((sender as ComboBox)!).SelectedItem = null;
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ck = sender as RadioButton;
            if (ck.IsChecked.Value)
                Sort(ck?.Name.Substring(0, (ck.Name.Length - 2)));

        }

        private void Sort(string property)
        {
            // If the function is called before the UI is ready, return
            if (!Ready)
                return;

            //Get the current list items and sort them by the selected sort method
            if (GameListView.ItemsSource is not List<Game> list) return;
            list = property switch
            {
                "Title" => list.OrderBy(game => game.Title).ToList(),
                "Year" => list.OrderBy(game => game.Year).ToList(),
                "Console" => list.OrderBy(game => game.Console).ToList(),
                "Emulator" => list.OrderBy(game => game.Emulator).ToList(),
                "Rating" => list.OrderBy(game => game.Rating).ToList(),
                _ => list
            };

            // Apply the sorted list to the ListView
            ApplyFilteredList(list);

        }
    }
}
