using System.Text.Json.Serialization;

namespace Odyssey
{
    internal class Game
    {
        public string Title { get; set; }
        public int YearReleased { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string[] Consoles { get; set; }
        public Emulator Emulator { get; set; }
        public string FileLocation { get; set; } //Location of the ROM/ISO, to be added to Properties.Settings ...
    }
}
