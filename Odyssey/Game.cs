using System.Text.Json.Serialization;

namespace Odyssey
{
    internal class Game
    {
        [property: JsonPropertyName("title")] public string Title { get; set; }
        [property: JsonPropertyName("year")] public int YearReleased { get; set; }
        [property: JsonPropertyName("description")] public string Description { get; set; }
        [property: JsonPropertyName("image")] public string Image { get; set; }
        [property: JsonPropertyName("console")] public string[] Consoles { get; set; }
        [property: JsonPropertyName("emulator")] public Emulator Emulator { get; set; }
        public string FileLocation { get; set; } //Location of the ROM/ISO, add this to the local INI file mentioned in MainWindow.xaml.cs
    }
}
