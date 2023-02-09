using Microsoft.Win32;
using Newtonsoft.Json;
using Odyssey;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static API.API;
using Image = System.Windows.Controls.Image;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Object games; // TODO: what was this for again?
        public List<Game> myGames = new List<Game>(); // Contains games stored in a sane fashion

        //String emulatorPath, gameFolderPath;
        //Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient _client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            InitializeApi();
            Games.Visibility = Visibility.Visible;
            Settings.Visibility = Visibility.Collapsed;
        }

        public async void InitializeApi()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Odyssey Desktop Client");

            var list = await ProcessRepositoriesAsync(_client);
            //games = list;
            // Outputs all games from the list to a JSON file in the working directory
            //TestMethod(list);

            OccupyListVar(await ProcessRepositoriesAsync(_client));

            //DataGrid.ItemsSource = list;

            //Programmatically added gradient for DataGrid
            LinearGradientBrush myLinearGradientBrush =
            new LinearGradientBrush();
            myLinearGradientBrush.StartPoint = new Point(0, 3);
            myLinearGradientBrush.EndPoint = new Point(1, 1);
            myLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Purple, 0.1));
            myLinearGradientBrush.GradientStops.Add(
            new GradientStop(Colors.Yellow, 2.5));

            int i = 0;
            //Testing
            foreach (var game in list)
            {
                if (i < 5)
                {
                    GenCard(game.Image, 0, game.Title);
                }
                else if (i < 1)
                {
                    GenCard(game.Image, 1, game.Title);
                }
                else if (i < 15)
                {
                    GenCard(game.Image, 2, game.Title);
                }
                else if (i < 20)
                {
                    GenCard(game.Image, 3, game.Title);
                }
                else if (i < 25)
                {
                    GenCard(game.Image, 4, game.Title);
                }
                else if (i < 30)
                {
                    GenCard(game.Image, 5, game.Title);
                }
                else if (i < 35)
                {
                    GenCard(game.Image, 5, game.Title);
                }
                else if (i < 40)
                {
                    GenCard(game.Image, 5, game.Title);
                }
                else if (i < 45)
                {
                    GenCard(game.Image, 5, game.Title);
                }
                i++;
            }

            //MessageBox.Show(list[0].Title); //Example of an individually addressed item
        }

        // On application startup
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Load stored settings
            LoadSettings();
        }

        private void TestMethod(List<GamesList> list)
        {
            string jsondata = JsonConvert.SerializeObject(list, Formatting.Indented);

            StreamWriter sw = new StreamWriter("games.json");

            sw.WriteLine(jsondata);
            sw.Close();
            Console.WriteLine(jsondata);
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

        public void GenCard(string Uri, int HIndex, string game)
        {

            // Create a new image which is sourced from the Uri provided
            Image Img = new Image();
            Img.Source = new BitmapImage(new Uri(Uri));

            // Set image dimensions
            Img.MaxHeight = 120;
            Img.Height = 170;
            Img.Width = 120;
            Img.ToolTip = game;

            // Add margin to bottom of each image
            Img.Margin = new Thickness(0, 0, 0, 5);

            switch (HIndex)
            {
                default:
                    break;
                case 0:
                    HorizGameStackPanel0.Children.Add(Img);
                    break;
                case 1:
                    HorizGameStackPanel1.Children.Add(Img);
                    break;
                case 2:
                    HorizGameStackPanel2.Children.Add(Img);
                    break;
                case 3:
                    HorizGameStackPanel3.Children.Add(Img);
                    break;
                case 4:
                    HorizGameStackPanel4.Children.Add(Img);
                    break;
                case 5:
                    HorizGameStackPanel5.Children.Add(Img);
                    break;
            }
        }

        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            //If the text box is modified and become empty, let all games be listed
            if (SearchTxBx.Text.Length <= 0)
            {
                InitializeApi();
            }
            else
            {
                //TODO: Filter the list by textbox text
            }
        }

        //private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    StartGame(DataGrid.SelectedIndex);
        //}

        //private void StartGame(int gameIndex)
        //{
        //    switch (DataGrid.SelectedIndex)
        //    {
        //        default:
        //            MessageBox.Show("Game not installed!");
        //            break;
        //        case 0: 
        //            MessageBox.Show("Game not installed!");
        //            break;
        //        case 1: 
        //            Process.Start("C:\\Games\\RPCS3\\rpcs3.exe", "E:\\Mods\\PS3\\GAMES\\BLUS30418");
        //            break;

        //    }

        //}

        private void StartGame(Game g)
        {

        }

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            Games.Visibility = Visibility.Collapsed;
            Settings.Visibility = Visibility.Visible;
        }

        //TODO: Verify settings

        //Save settings
        private void SaveSettings()
        {
            Odyssey.Properties.Settings.Default.DarkMode = darkModeChkBx.IsChecked.GetValueOrDefault();
            Odyssey.Properties.Settings.Default.pathRPCS3 = pathRPCS3TxtBx.Text;
            Odyssey.Properties.Settings.Default.Save();
        }

        // Load settings on startup
        private void LoadSettings()
        {
            // Check the stored settings to see find the last state of the dark mode setting
            if (!Odyssey.Properties.Settings.Default.DarkMode) // if it's false or null, dark mode stays off
                darkModeChkBx.IsChecked = false; //Disable dark mode
            else // Else it is enabled
                darkModeChkBx.IsChecked = true; //Dark mode is enabled

            if (Odyssey.Properties.Settings.Default.pathRPCS3 is null)
                pathRPCS3TxtBx.Text = "Unset";
            else
                pathRPCS3TxtBx.Text = Odyssey.Properties.Settings.Default.pathRPCS3;
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
            MessageBox.Show("Settings applied");

            if (darkModeChkBx.IsChecked == true)
            {
                //Change Bg Colour to black
            }

            /*
            if (pathRPCS3TxtBx != null)
            {
                emulatorPath = pathRPCS3TxtBx.Text;
            }

            if (GameFolderPath != null)
            {
                gameFolderPath = GameFolderPath.Text;
            }
            */
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
