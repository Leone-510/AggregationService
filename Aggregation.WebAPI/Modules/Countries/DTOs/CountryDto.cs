using System.Text.Json.Serialization;

namespace Aggregation.WebAPI.Modules.Countries.DTOs
{
    public class CountryDto
    {
        [JsonPropertyName("name")]
        public CountryName? Name { get; set; }

        // Dictionary for currencies (XCD, GTQ, GMD, etc.)
        [JsonPropertyName("currencies")]
        public Dictionary<string, CurrencyInfo>? Currencies { get; set; }

        [JsonPropertyName("capital")]
        public List<string>? Capital { get; set; }
    }

    public class CountryName
    {
        [JsonPropertyName("common")]
        public string? Common { get; set; }

        [JsonPropertyName("official")]
        public string? Official { get; set; }

        // Dictionary for languages (eng, spa, etc.)
        [JsonPropertyName("nativeName")]
        public Dictionary<string, NativeNameInfo>? NativeName { get; set; }
    }

    public class NativeNameInfo
    {
        [JsonPropertyName("official")]
        public string? Official { get; set; }

        [JsonPropertyName("common")]
        public string? Common { get; set; }
    }

    public class CurrencyInfo
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }
}
