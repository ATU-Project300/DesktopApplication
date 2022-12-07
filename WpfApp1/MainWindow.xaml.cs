using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using static API.API;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
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

            //Testing
            //ListBox.ItemsSource = list; // Trying list box, doesn't seem like a good solution
            DataGrid.ItemsSource = list;
            //MessageBox.Show(list[0].Title); //Example of an individually addressed item

        }

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
    }
}
