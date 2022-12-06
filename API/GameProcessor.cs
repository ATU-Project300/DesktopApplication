using Odyssey;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace API
{
    public class GameProcessor
    {
        public static async Task<GameList> LoadGames()
        {
            //Await the response from the games endpoint
            //In using statement for memory management
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync("games/"))
            {
                //Write the response to console
                //TODO: Remove this once not needed
                Console.WriteLine(response);
                Console.WriteLine(response.Content);
                Console.WriteLine(response.IsSuccessStatusCode);

                //If we get a successful response, return the response content in the form of GameList (See GameList class)
                if (response.IsSuccessStatusCode)
                {
                    GameList games = await response.Content.ReadAsAsync<GameList>();
                    return games;
                }
                //Else throw exception
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}
