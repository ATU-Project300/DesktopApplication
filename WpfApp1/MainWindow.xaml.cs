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

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Object games;
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
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _client.DefaultRequestHeaders.Add("User-Agent", "Odyssey Desktop Client");
            var list = await ProcessRepositoriesAsync(_client);
            games = list;

            DataGrid.ItemsSource = list;

            /*
            int i = 0;
            //Testing
            foreach (var game in list)
            {
                if (i < 5)
                {
                    GenCard(game.Image, 0, game.Title.Substring(0,3));
                }
                else if (i < 10)
                {
                    GenCard(game.Image, 1, game.Title.Substring(0,3));
                }
                else if (i < 15)
                {
                    GenCard(game.Image, 2, game.Title.Substring(0,3));
                }
                else if (i < 20)
                {
                    GenCard(game.Image, 3, game.Title.Substring(0,3));
                }
                else if (i < 25)
                {
                    GenCard(game.Image, 4, game.Title.Substring(0,3));
                }
                else if (i < 30)
                {
                    GenCard(game.Image, 5, game.Title.Substring(0,3));
                }
                i++;
            }
            //MessageBox.Show(list[0].Title); //Example of an individually addressed item
            */

        }

        /*
        public void GenCard(string Uri, int HIndex, string game)
        {
            Image Img = new Image();
            Img.Source = new BitmapImage(new Uri(Uri));
            Img.MaxHeight = 120;
            Img.Height = 170;
            Img.Width = 120;
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

        */

        //Spawn settings window?
        //Hide list and other items and show settings instead?
        //TODO: Implement persistent settings from previous build
        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            //ToString for safety :)
            if (SettingsBTN.Content.ToString() == "Back")
            {
                MessageBox.Show("Exiting settings");
                SettingsBTN.Content = "Settings";
            }
            else
            {
                MessageBox.Show("Entering settings");
                SettingsBTN.Content = "Back";
            }
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

        private void DataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            switch (DataGrid.SelectedIndex)
            {
                default:
                    MessageBox.Show("Game not installed!");
                    break;
                case 0: 
                    MessageBox.Show("Game not installed!");
                    break;
                case 1: 
                    Process.Start("C:\\Games\\RPCS3\\rpcs3.exe", "E:\\Mods\\PS3\\GAMES\\BLUS30418");
                    break;
                
            }
        }
    }
}
