using System.Text.Json;

namespace API
{
    public class API
    {
        public static string URL = "http://45.152.209.166:3000/games";

        public static void Main()
        {
            //To be left empty
        }

        public static async Task<List<GamesList>> ProcessGamesData(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync(URL);
            var games =
                await JsonSerializer.DeserializeAsync<List<GamesList>>(stream);
            return games ?? new();
        }

        //TODO: Utilise this
        public bool Online()
        {
            try
            {
                using (var client = new HttpClient())
                using (var stream = client.GetStreamAsync(URL))
                    return true;
            }
            catch
            {
                return false;
            }
        }

    }
}


