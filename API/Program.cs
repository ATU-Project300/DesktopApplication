﻿using System.Text.Json;

namespace API
{
    public class Api
    {
        public static string Url = "http://45.152.209.166:3000/games";

        public static void Main()
        {
            //To be left empty
        }

        public static async Task<List<GamesList>> ProcessGamesData(HttpClient client)
        {
            await using Stream stream =
                await client.GetStreamAsync(Url);
            var games =
                await JsonSerializer.DeserializeAsync<List<GamesList>>(stream);
            return games ?? new();
        }
    }
}


