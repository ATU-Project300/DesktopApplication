namespace Odyssey
{
    public class Game
    {
        //Data structure for individual games as per the API response. Not case sensitive.
        public string Title { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string[] Consoles { get; set; }
        public string Emulator { get; set; }
    }
}
