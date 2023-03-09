using System.Text.Json.Serialization;

namespace API;

public sealed record GamesList(

    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("image")] string Image,
    [property: JsonPropertyName("consoles")] string Console,
    [property: JsonPropertyName("emulator")] string Emulator,
    [property: JsonPropertyName("averageRating")] int Rating);