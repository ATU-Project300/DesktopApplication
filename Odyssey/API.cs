using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Odyssey
{
    internal class API
    {
        /*
        private static List<Game> Games = new List<Game>();

        public async Task<List<Game>> FetchData()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

           /List<Game> GamesList =  ProcessDataAsync(client);

            foreach (var game in GamesList)
            {
                Games.Add(game);
            }

            static async Task<List<Game>> ProcessDataAsync(HttpClient client)
            {
                using Stream stream = await client.GetStreamAsync("http://localhost:3000/games");
                var games = await JsonSerializer.DeserializeAsync<List<Game>>(stream);
                return games;
            }
            //FetchAPIDataAsync();
            return Games;
        }
        */

    }
}
