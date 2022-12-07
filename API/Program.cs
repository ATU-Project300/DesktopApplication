using System.Text.Json;

namespace API
{
    public class API
    {
        public static void Main()
        {
            //To be left empty
        }

        public static async Task<List<GamesList>> ProcessRepositoriesAsync(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync("http://localhost:3000/games");
            var games =
                await JsonSerializer.DeserializeAsync<List<GamesList>>(stream);
            return games ?? new();
        }

    }
}


