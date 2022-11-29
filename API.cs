using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

/*Helpful resources: https://stackoverflow.com/questions/9620278/how-do-i-make-calls-to-a-rest-api-using-c
                     https://code-maze.com/different-ways-consume-restful-api-csharp/
*/

namespace Project300
{
    internal class API
    {
        public static void FetchAPIData()
        {
            using HttpClient client = new();

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

            //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            client.DefaultRequestHeaders.Add("User-Agent", ".NET API Reader");

            var GamesList = await ProcessDataAsync(client);

            static async Task<List<GamesList>> ProcessDataAsync(HttpClient client)
            {
                await using Stream stream = await client.GetStreamAsync("http://localhost:3000/games");
                var games = await JsonSerializer.DeserializeAsync<List<GamesList>>(stream);
                return games ?? new();
            }

        }
    }
}
