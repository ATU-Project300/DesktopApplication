namespace Odyssey
{
    public partial class MainWindow
    {
        // TODO: Consistent variable and function naming (excluding cases as necessary)

        // Contains games stored in a sane fashion
        public List<Game> MyGames = new();

        public List<Emulator> MyEmulators = new();

        public static string OdysseyPath;

        // The current game selected for DetailsView
        // Variable exists so that we can access it from other methods
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
            CreateOdysseyFolder();
            Trace.WriteLine(OdysseyPath + "\\");
        }

        // Create a folder named Odyssey in APPDATA to store emulators
        private void CreateOdysseyFolder()
        {
            OdysseyPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Odyssey";
            if (!Directory.Exists(OdysseyPath))
            {
                Directory.CreateDirectory(OdysseyPath);
            }
        }


        // From the API, use the games data to occupy the myGames List
        public async void InitializeApiData()
        {
            OccupyListVarGames(await ProcessGamesData(Client));
            OccupyListVarEmulators(await ProcessEmulatorsData(Client));

            // This goes here only because it loads too early anywhere else
            DataContext = new GameViewModel(MyGames);
            ApplyFilteredList(MyGames);
            Ready = true;
        }

        // Occupy the local list var "myGames" with the games
        // such that they are individually addressable (!!!)
        // This is only called once
        // TODO: More efficient way of doing this?
        private void OccupyListVarGames(List<GamesList> list)
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

        private void OccupyListVarEmulators(List<EmulatorsList> list)
        {
            foreach (var x in list)
            {
                MyEmulators.Add(new Emulator()
                {
                    Name = x.Name,
                    Image = x.Image,
                    Exectuable = x.Executable,
                    Uri = x.Uri
                });
            }
        }

        // Allow for switching between light and dark themes
        // Must be added soon after InitializeComponent (Currently part of LoadSettings)
        private void Theming(bool dark)
        {
            // Initialise these variables so that they can be used in the foreach loops
            // Colours are given values as needed in the if/else instead of creating many 
            // variables holding light and dark variants.
            Color colour = new Color();
            Color textColour = new Color();
            Image logoImage = (Image)FindName("LogoImage");
            Image closeImage = (Image)FindName("GamesBtnReturn");
            ImageBrush sortBrushO = (ImageBrush)FindName("SortImageBrushO");
            ImageBrush sortBrushB = (ImageBrush)FindName("SortImageBrushB");
            ImageBrush filterBrushO = (ImageBrush)FindName("FilterImageBrushO");
            ImageBrush filterBrushB = (ImageBrush)FindName("FilterImageBrushB");

            LinearGradientBrush myLinearGradientBrush = new LinearGradientBrush
            {
                StartPoint = new Point(0, 3),
                EndPoint = new Point(1, 1)
            };

            if (dark)
            {
                // Dark theme colours:
                myLinearGradientBrush.GradientStops.Add(
                    new GradientStop(Colors.Black, 0.1));
                myLinearGradientBrush.GradientStops.Add(
                    new GradientStop(Colors.White, 8.5));

                colour = (Color)ColorConverter.ConvertFromString("#222222");
                textColour = Colors.White;

                // TODO: Clean these up
                // https://learn.microsoft.com/en-us/dotnet/desktop/wpf/app-development/pack-uris-in-wpf?redirectedfrom=MSDN&view=netframeworkdesktop-4.8
                logoImage.Source =
                    new BitmapImage(new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Logo.png"));
                sortBrushO.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Sort.png"));
                sortBrushB.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Sort.png"));
                filterBrushO.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Filter.png"));
                filterBrushB.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Filter.png"));
                closeImage.Source = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Dark/Close.png"));
            }
            else
            {
                // Light theme colours:
                myLinearGradientBrush.GradientStops.Add(
                    new GradientStop(Colors.Purple, 0.1));
                myLinearGradientBrush.GradientStops.Add(
                    new GradientStop(Colors.Yellow, 2.5));

                colour = (Color)ColorConverter.ConvertFromString("#b06050");
                textColour = Colors.Black;

                // TODO: Clean these up
                logoImage.Source =
                    new BitmapImage(new Uri("pack://application:,,,/odyssey;component/Resources/Light/Logo.png"));
                sortBrushO.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Light/Sort.png"));
                sortBrushB.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Light/Sort.png"));
                filterBrushO.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Light/Filter.png"));
                filterBrushB.ImageSource = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Light/Filter.png"));
                closeImage.Source = new BitmapImage(uriSource: new Uri("pack://application:,,,/odyssey;component/Resources/Light/Close.png"));
            }

            MainGrid.Background = myLinearGradientBrush;
            LogoButtonsGrid.Background = new SolidColorBrush(colour);

            // For each of the buttons on the left side of the window, change their colour
            foreach (var x in LogoButtonsGrid.Children)
            {
                if (x is not StackPanel sp) continue;
                foreach (var y in sp.Children)
                {
                    if (y is not System.Windows.Controls.Button b) continue;
                    b.Background = new SolidColorBrush(colour);
                }
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
                                textBlock.Foreground = new SolidColorBrush(textColour);
                                break;
                            case TextBox textBox:
                                textBox.Foreground = new SolidColorBrush(textColour);
                                break;
                            case CheckBox checkBox:
                                checkBox.Foreground = new SolidColorBrush(textColour);
                                break;
                        }
                    }
                }
            }
        }


        // Using other methods, construct a launchCommand to be ran by Process.Start
        private void StartGame(Game game)
        {
            bool pickEmulatorFailed = false, findGameFailed = false, verifyPathSettingFailed = false;
            string msg1 = "Emulator valid.", msg2 = "Game file present";
            var lGame = "Invalid";

            string lEmulator = PickEmulator(game); // Prevents calling the method twice

            if (lEmulator == "Invalid")
                pickEmulatorFailed = true;
            
            if (VerifySetting(pathGameFolderTxtBx, false))
                    lGame = FindGame(game); // Prevents calling the method twice
            else
            {
                verifyPathSettingFailed = true;
                lGame = "";
            }

            if (lGame == "Invalid")
                findGameFailed = true;

            if (pickEmulatorFailed)
                msg1 = $"Emulator {game.Emulator} not found.";

            if (findGameFailed)
                msg2 = "Game file not found.";

            Trace.WriteLine($"[INFO]: {game.Title}. Launch command {lEmulator} {lGame}");
            Trace.WriteLine($"[INFO]: {msg1} {msg2}");

            if (pickEmulatorFailed || findGameFailed)
            {
                string errorMessage = "";
                if(verifyPathSettingFailed)
                    errorMessage += "ERROR 1: Game folder path is invalid. Please check your settings.\n";
                if (findGameFailed)
                    errorMessage += $"ERROR 2: ROM/ISO file for {game.Title} was not found, is it in your game folder?\n";
                if (pickEmulatorFailed)
                    errorMessage += $"ERROR 3: Emulator {game.Emulator} was not found, you can download it from the Emulators panel.";

                MessageBox.Show(errorMessage);

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
            // The setting name is the emulator name + "path".
            // Example "pathXenia"
            var setting = "path" + game.Emulator;

            // Verify the setting exists before returning it
            if (Properties.Settings.Default.Properties.Cast<SettingsProperty>().All(x => x.Name != setting))
                return "Invalid";
            var emu = (string)Properties.Settings.Default[setting];
            return Path.Exists(emu) ? emu : "Invalid";

        }

        // Checks if the game is valid and if the game path is set and returns the result of FindFile for the game
        private string FindGame(Game game)
        {
            // RPCS3 takes the folder as the game path, while the other emulators take the file
            // TODO: Find a different way about this such that we aren't hard coding the emulator name
            if (game.Emulator == "RPCS3")
                return FindFolder(pathGameFolderTxtBx.Text, game.Title);

            return FindFile(pathGameFolderTxtBx.Text, game.Title);
        }

        // Verify each setting we have
        private void VerifySettings()
        {
            // For each TextBox named "path*"
            foreach (var g in Settings.Children)
            {
                if (g is not Grid grid) continue;
                foreach (var t in grid.Children.OfType<TextBox>())
                {
                    if (!t.Name.StartsWith("path")) continue;
                    // Get the emulator name from the TextBox name, then find the emulator in the MyEmulators list using the name
                    var emulatorName = t.Name.Remove(0, 4);
                    var emulator = MyEmulators.Find(x => x.Name == emulatorName);

                    VerifySetting(t, !t.Name.Contains("GameFolder"), emulator?.Exectuable);
                }
            }


        }

        // Generic method to allow for simple verification of individual settings
        private static bool VerifySetting(TextBox t, bool executable = false, string executableName = "")
        {
            // Colours for the text boxes for case of error or no error
            Color errorColour = Color.FromArgb(80, 255, 0, 0);
            Color noColour = Color.FromArgb(00, 0, 0, 0);

            // Do some checks on the provided data, return false upon failure, else true
            bool TxtBxCheck()
            {
                // Some checks
                if (t.Text.Length < 2) return false; // No actual path should be less characters than this (C:\)
                if (!Path.Exists(t.Text)) return false; // If the path doesn't exist
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
            return TxtBxCheck();
        }

        // TODO: Make this and VerifySettings not overlap so much
        // Verify a single setting when it is changed
        private void VerifySettingOnChange(object? sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            var t = sender as TextBox;

            if (t.Name.Contains("GameFolder"))
                VerifySetting(t, false);
            else
            {
                var emulatorName = t.Name.Remove(0, 4);
                var emulator = MyEmulators.Find(x => x.Name == emulatorName);

                if (emulator?.Exectuable != null)
                    VerifySetting(t, !t.Name.Contains("GameFolder"), emulator.Exectuable);
            }


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

                            t.Text = Properties.Settings.Default[t.Name].ToString() is null
                                ? "Unset"
                                : Properties.Settings.Default[t.Name].ToString();
                        }
                    }
            }
        }
        private void ApplySettings()
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
            if (t != null) t.Text = ofd.FileName;
        }

        private void FolderPicker(TextBox? t)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "C:\\";
            var result = fbd.ShowDialog();
            if (result.ToString() == "Cancel") return;
            if (t != null) t.Text = fbd.SelectedPath;
        }

        // Hides all other panels which aren't the one associated with the clicked button
        private void HideOtherPanels(object sender, RoutedEventArgs e)
        {
            // Get the sender as a button, get its name excluding "Btn"
            var name = (sender as Button)?.Name.Split("Btn", 2)[0];

            // If our button name is null, assume function was actually called from an Image
            if (name is null)
                name = (sender as Image)?.Name.Split("Btn", 2)[0];


            // Loop through all the TabPanels and hide the ones that aren't associated with the button
            // and show the ones that are
            foreach (var x in MainGrid.Children)
            {
                if (x is not TabPanel tabPanel) continue;
                if (name != null && !tabPanel.Name.StartsWith(name))
                    tabPanel.Visibility = Visibility.Collapsed;
                else
                {
                    tabPanel.Visibility = Visibility.Visible;
                    // TODO: Better solution for this
                    // This shouldn't really be here but it allows the user to see if the settings are valid
                    // as the user opens the settings page
                    // If they are switching to a page other than the settings page, apply the settings (workaround to remove apply button)
                    if (tabPanel.Name == "Settings")
                        VerifySettings();
                    else
                    {
                        ApplySettings();
                    }
                }
            }
        }

        // Essentially the click even for the game covers
        private void Image_Click(object sender, MouseButtonEventArgs e)
        {
            // Get the Game object associated with the clicked item
            // Do something with the selected game object, such as showing more details in a new window
            if ((sender as FrameworkElement)?.DataContext is Game game)
            {
                HideOtherPanels(sender, e);
                GenerateDetailsView(game);
            }
        }

        // Fill the DetailsView panel with the correct details for the selected game
        private void GenerateDetailsView(Game game)
        {
            if (game == null) return;
            SelectedGame = null;
            SelectedGame = game;

            if (SelectedGame.Image != null) DetailsGameImage.Source = new BitmapImage(new Uri(SelectedGame.Image));
            Trace.WriteLine($"[INFO]: Rating {SelectedGame.Rating}");
            DetailsGameImage.Width = 250;
            DetailsGameTitleTxtBlk.Text = SelectedGame.Title;
            DetailsGameYearTxtBlk.Text = SelectedGame.Year.ToString();
            DetailsGameConsoleTxtBlk.Text = SelectedGame.Console;
            DetailsGameDescriptionTxtBlk.Text = SelectedGame.Description;
            DetailsGamePlayButton.Content = $"Play {SelectedGame.Title} on {SelectedGame.Emulator}";

            // Create a variable to access the grid here
            Grid ratingGrid = (Grid)FindName("DetailsGameRatingBlk");

            //Remove any stars already present in the grid
            ratingGrid.Children.Clear();

            // Show the unrated text if the game has no rating, else hide it
            if (game.Rating == 0)
            {
                // Show the text and return as we don't need to draw any stars
                DetailsUnrated.SetValue(VisibilityProperty, Visibility.Visible);
                return;
            }
            else
            {
                DetailsUnrated.SetValue(VisibilityProperty, Visibility.Collapsed);
            }

            // For the value of game.Rating, draw that number of stars.png in the DetailsGameRatingImgBlk
            var i = 1;
            var j = game.Rating;
            while (j > 0)
            {
                var img = new Image();
                img.Source = new BitmapImage(new Uri("pack://application:,,,/odyssey;component/Resources/Star.png"));
                img.Width = 20;
                img.Height = 20;
                img.SetValue(Grid.ColumnProperty, i);
                DetailsGameRatingBlk.Children.Add(img);
                j--;
                i++;
            }

        }

        // Handles clicking any of the buttons in the Emulator Management panel
        private void EmulatorManagementBtn_Click(object sender, RoutedEventArgs e)
        {
            // Example of name: "RunXenia"
            var name = (sender as Button)?.Name.Split("Btn", 2)[0];
            if (name == null) return;

            if (name.Contains("Download"))
            {
                name = name.Split("Download", 2)[1];
                DownloadEmulator(name);
            }
            else if (name.Contains("Run"))
            {
                name = name.Split("Run", 2)[1];
                EmulatorManagementRun(name);
            }
            else if (name.Contains("Delete"))
            {
                name = name.Split("Delete", 2)[1];
                DeleteEmulator(name);
            }
        }

        // Delete and emulator by name
        // TODO: Investigate SNES9x "Valid.Ext" file causing an exception
        private void DeleteEmulator(string? name)
        {
            if(name == null) return;
            var folder = FindFolder(OdysseyPath, name);
            var file = FindFile(OdysseyPath, name);
            if (!Path.Exists(folder)) return;
            try
            {
                // Recursively delete the emulators directory
                Directory.Delete(folder, true);
                // Delete any file that matches the emulator title (Like the archives)
                File.Delete(file);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"[ERROR]: {e}");
            }
        }

        // Determines some things to download an emulator
        // TODO: Make this take an Emulator as a parameter instead of a string
        private void DownloadEmulator(string name)
        {
            foreach (var emu in MyEmulators.Where(emu => emu.Name == name))
            {
                var output = emu.Name;

                if (emu.Uri.Contains(".zip"))
                    output += ".zip";
                else if (emu.Uri.Contains(".7z"))
                    output += ".7z";
                else if (emu.Uri.Contains(".rar"))
                    output += ".rar";

                if (Path.Exists(OdysseyPath + "\\" + output))
                    File.Delete(OdysseyPath + "\\" + output);

                InstallEmulator(emu, output);
            }
        }

        // Run an emulator from the Emulator Management panel
        // TODO: Make this take an Emulator as a parameter instead of a string
        private void EmulatorManagementRun(string name)
        {
            var setting = "path" + name;

            // Verify the setting exists
            if (Properties.Settings.Default.Properties.Cast<SettingsProperty>().Any(x => x.Name == setting))
            {
                var emu = (string)Properties.Settings.Default[setting];
                if (Path.Exists(emu))
                    Process.Start(emu);
                else
                    MessageBox.Show("Emulator not found :(");
            }
        }

        // Run the BigFilter when the text box contents have changed
        private void SearchTxtBx_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // This cannot be used as the text changed event directly due to it passing TextChangedEventsArgs
            // which is not expected by this function
            // TODO: Make it compatible (without breaking it for the other uses)
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
                filteredList = filteredList.Where(game =>
                    game.Title != null && game.Title.ToLower().Contains(SearchTxtBx.Text.ToLower())).ToList();
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
                filteredList = filteredList.Where(game =>
                    game.Emulator != null && game.Emulator.ToLower().Equals(emulator?.ToLower())).ToList();
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
                filteredList = filteredList
                    .Where(game => game.Console != null && game.Console.ToLower().Equals(console?.ToLower())).ToList();
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
        private void SummonPicker(object sender, MouseButtonEventArgs e)
        {
            var x = sender as TextBox;
            if (x.Name.Contains("Folder")) FolderPicker(x);
            else FilePicker(x);
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
                if (property == null) continue;
                var a = game.GetType().GetProperty(property)?.GetValue(game);
                if (!list.Contains(a))
                    list.Add(((string)a)!);
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

        // Starts the game selected in DetailsView
        // TODO: Remove this
        private void DetailsGamePlayButton_Click(object sender, RoutedEventArgs e)
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

        // Downloads an emulator, extracts it and then adds the emulator path to settings
        // TODO: Split into multiple functions, this is a mess
        public void InstallEmulator(Emulator? emu, string? output)
        {
            if (emu == null || output == null) return;
            var outputPath = OdysseyPath + "\\" + output;

            using (WebClient wc = new WebClient())
            {
                //Download from URL to location
                wc.DownloadFileAsync(new Uri(emu.Uri), outputPath);
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(WcDownloadProgressChanged);

                // For each update in the downloads progress, do this
                void WcDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
                {
                    DownloadProgressBar.Value = e.ProgressPercentage;

                    if (e.ProgressPercentage % 5 == 1)
                        Trace.WriteLine($"[INFO]: Download progress of {output} {e.ProgressPercentage}%");

                    if (e.ProgressPercentage == 100)
                    {
                        void Save(string extension)
                        {
                            var settingName = "path" + emu.Name;

                            var path = FindFile(FindFolder(OdysseyPath, emu.Name), emu.Exectuable);

                            if (!path.EndsWith(".exe")) return;
                            // Put the found executable path into settings
                            Properties.Settings.Default[settingName] = path;

                            // Load each of the settings
                            LoadSettings();

                            // Save the settings
                            SaveSettings();

                        }

                        wc.Dispose(); // Dispose of the web client
                        Trace.WriteLine($"[INFO]: {output} downloaded from {emu.Uri}");

                        if (output.EndsWith(".7z"))
                            if (ExtractArchive(outputPath, OdysseyPath + "\\" + output.Split(".7z", 2)[0]))
                            {
                                Trace.WriteLine("[INFO]: Adding to settings");
                                Save(".7z");
                            }

                        if (output.EndsWith(".zip"))
                        {
                            if (ExtractArchive(outputPath, OdysseyPath + "\\" + output.Split(".zip", 2)[0]))
                            {
                                Trace.WriteLine("[INFO]: Adding to settings");
                                Save(".zip");
                            }
                        }

                    }

                }

                // If the web client could not download the zip, this code executes
                wc.Dispose(); // Dispose of the web client
            }
        }

        // thanks https://stackoverflow.com/questions/7994477/extract-7zip-in-c-sharp-code
        public bool ExtractArchive(string sourceArchive, string destination)
        {
            // TODO: This shouldn't be hard coded
            // Add edge case for pcsx2 having a folder in the zip already
            if (sourceArchive.ToLower().Contains("pcsx2"))
            {
                destination = OdysseyPath + "\\";
            }

            string zPath = "7za.exe"; //add to proj and set CopyToOuputDir
            try
            {
                Trace.WriteLine($"[INFO]: Extracting {sourceArchive} to {destination}");
                ProcessStartInfo pro = new ProcessStartInfo();
                pro.WindowStyle = ProcessWindowStyle.Hidden;
                pro.FileName = zPath;
                pro.Arguments = string.Format("x \"{0}\" -y -o\"{1}\"", sourceArchive, destination);
                Process x = Process.Start(pro);
                x.WaitForExit();
                return true;
            }
            catch (System.Exception Ex)
            {
                //handle error
                return false;
            }
        }

        private void DarkModeQuickToggle(object sender, RoutedEventArgs e)
        {
            var darkModeToggle = darkModeChkBx.IsChecked.GetValueOrDefault();
            Theming(darkModeToggle);
            Properties.Settings.Default.DarkMode = darkModeToggle;
            Properties.Settings.Default.Save();
        }
    }
}
