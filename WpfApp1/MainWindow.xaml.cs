using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using static API.API;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;
using System.Windows.Media;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Win32;
using Odyssey;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Object games;
        String emulatorPath, gameFolderPath;
        //Static client because it is thread safe and we don't need more than one
        private static readonly HttpClient _client = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
            InitializeApi();
        }

        public async void InitializeApi()
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Odyssey Desktop Client");
            var list = await ProcessRepositoriesAsync(_client);
            games = list;

            // Outputs all games from the list to a JSON file in the working directory
            TestMethod(list);

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

            // Use the brush to paint the datagrid .
            //DataGrid.Background = myLinearGradientBrush;
            
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
            //foreach (var item in list)
            //{
            //    Console.WriteLine(item.Title);
            //}

            string jsondata = JsonConvert.SerializeObject(list, Formatting.Indented);

            StreamWriter sw = new StreamWriter("games.json");

            sw.WriteLine(jsondata);
            sw.Close();
            Console.WriteLine(jsondata);
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

        private void SettingsBTN_Click(object sender, RoutedEventArgs e)
        {
            //Hides the Stack panels of games - using collapsed so that they still arent taking up whitespace
            HorizGameStackPanel0.Visibility = Visibility.Collapsed;
            HorizGameStackPanel1.Visibility = Visibility.Collapsed;
            HorizGameStackPanel2.Visibility = Visibility.Collapsed;
            HorizGameStackPanel3.Visibility = Visibility.Collapsed;
            HorizGameStackPanel4.Visibility = Visibility.Collapsed;
            HorizGameStackPanel5.Visibility = Visibility.Collapsed;

            //Allows the settings to appear for the end user
            applyBtn.Visibility = Visibility.Visible;
            darkModeChkBx.Visibility = Visibility.Visible;

            RPCS3txtblk.Visibility = Visibility.Visible;
            pathRPCS3TxtBx.Visibility = Visibility.Visible;

            Xeniatxtblk.Visibility = Visibility.Visible;
            pathXeniaTxtBx.Visibility = Visibility.Visible;

            GFPtxtblk.Visibility = Visibility.Visible;
            GameFolderPath.Visibility = Visibility.Visible;
        }

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
            if(HorizGameStackPanel0.Visibility == Visibility.Collapsed)
            {
                //Brings back the game cover art if they are currently closed due to being in another tab
                HorizGameStackPanel0.Visibility = Visibility.Visible;
                HorizGameStackPanel1.Visibility = Visibility.Visible;
                HorizGameStackPanel2.Visibility = Visibility.Visible;
                HorizGameStackPanel3.Visibility = Visibility.Visible;
                HorizGameStackPanel4.Visibility = Visibility.Visible;
                HorizGameStackPanel5.Visibility = Visibility.Visible;
            }

            //If apply button is visible, then it is safe to say the settings window is opened.
            //This will change all settings to Collapsed - Collapsed prevents leftover whitespace from the objects.
            if(applyBtn.Visibility== Visibility.Visible)
            {
                //Allows the settings to appear for the end user
                applyBtn.Visibility = Visibility.Collapsed;
                darkModeChkBx.Visibility = Visibility.Collapsed;

                Xeniatxtblk.Visibility = Visibility.Visible;
                pathXeniaTxtBx.Visibility = Visibility.Visible;

                RPCS3txtblk.Visibility = Visibility.Collapsed;
                pathRPCS3TxtBx.Visibility = Visibility.Collapsed;

                GFPtxtblk.Visibility = Visibility.Collapsed;
                GameFolderPath.Visibility = Visibility.Collapsed;
            }
        }

        private void AllGamesBtn_Click(object sender, RoutedEventArgs e)
        {
            if (HorizGameStackPanel0.Visibility == Visibility.Collapsed)
            {
                //Brings back the game cover art if they are currently closed due to being in another tab
                HorizGameStackPanel0.Visibility = Visibility.Visible;
                HorizGameStackPanel1.Visibility = Visibility.Visible;
                HorizGameStackPanel2.Visibility = Visibility.Visible;
                HorizGameStackPanel3.Visibility = Visibility.Visible;
                HorizGameStackPanel4.Visibility = Visibility.Visible;
                HorizGameStackPanel5.Visibility = Visibility.Visible;
            }

            //If apply button is visible, then it is safe to say the settings window is opened.
            //This will change all settings to Collapsed - Collapsed prevents leftover whitespace from the objects.
            if (applyBtn.Visibility == Visibility.Visible)
            {
                //Allows the settings to appear for the end user
                applyBtn.Visibility = Visibility.Collapsed;
                darkModeChkBx.Visibility = Visibility.Collapsed;

                RPCS3txtblk.Visibility = Visibility.Collapsed;
                pathRPCS3TxtBx.Visibility = Visibility.Collapsed;

                Xeniatxtblk.Visibility = Visibility.Collapsed;
                pathXeniaTxtBx.Visibility = Visibility.Collapsed;

                GFPtxtblk.Visibility = Visibility.Collapsed;
                GameFolderPath.Visibility = Visibility.Collapsed;
            }
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

        private void PathRPCS3TxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathRPCS3TxtBx);
        }

        private void PathXeniaTxtBx_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilePicker(pathXeniaTxtBx);
        }

        private void FilePicker(TextBox t)
        {
            var ofd = new OpenFileDialog();
            var result = ofd.ShowDialog();
            if (result == false) return;
            t.Text = ofd.FileName;
        }
    }
}
