using Newtonsoft.Json;

namespace Monarchs.Dtos;

// Personally, not a great fan of JsonProperties, they add logic to a POCO, which is by definition not the goal of a POCO.
// This is a DTO that's only used for the API, making it a less of an issue, and it allows to
// add more logical property names. These are things a team can make decisions on, as long as it's consistent.
public class MonarchDto
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("nm")]
    public string? Name { get; set; }

    [JsonProperty("cty")]
    public string? City { get; set; }

    [JsonProperty("hse")]
    public string? House { get; set; }

    [JsonProperty("yrs")]
    public string? RulingYears { get; set; }
}