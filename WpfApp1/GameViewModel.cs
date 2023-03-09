using System.Collections.Generic;

namespace Odyssey
{
    public class GameViewModel
    {
        //The list read by the XAML binding
        public List<Game> MyGames { get; set; }

        public GameViewModel(List<Game> myGames)
        {
            MyGames = myGames;
        }
    }
}
