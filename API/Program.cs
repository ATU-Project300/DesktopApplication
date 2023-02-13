﻿using System.Net;
using System.Text.Json;

namespace API
{
    public class API
    {
        //TODO: Utilise this
        public string URL = "https:localhost:3000/games";

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

