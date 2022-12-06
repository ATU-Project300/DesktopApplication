using System.Collections.Generic;

namespace Odyssey
{
    public class GameList
    {
        //API response contains a list of items which match the type "Game" (See Game class)
        public List<Game> ListOfGames { get; set; }
    }
}
