using System.Text.Json.Serialization;

namespace API;

public sealed record EmulatorsList(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("image")] string Image,
    [property: JsonPropertyName("executable")] string Executable);