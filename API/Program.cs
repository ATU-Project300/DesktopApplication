using System.Text.Json;

namespace API
{
    public class Api
    {
        // TODO: Replace this once the API is hosted somewhere else
        public static string Url = "http://45.152.209.166:3000/";

        public static void Main()
        {
            //To be left empty
        }

        // TODO: Allow for both API endpoints to be accessed (/games and /emulators)
        public static async Task<List<GamesList>> ProcessGamesData(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync(Url + "games");
            var games =
                await JsonSerializer.DeserializeAsync<List<GamesList>>(stream);
            return games ?? new();
        }

        public static async Task<List<EmulatorsList>> ProcessEmulatorsData(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync(Url + "emulators");
            var emulators =
                await JsonSerializer.DeserializeAsync<List<EmulatorsList>>(stream);
            return emulators ?? new();
        }
    }
}


