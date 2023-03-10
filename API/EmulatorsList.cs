using System.Text.Json.Serialization;

namespace API;

public sealed record EmulatorsList(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("uri")] int Uri,
    [property: JsonPropertyName("image")] string Image);