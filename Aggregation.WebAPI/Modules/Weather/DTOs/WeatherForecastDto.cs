
using System.Text.Json.Serialization;

namespace Aggregation.WebAPI.Modules.Weather.DTOs
{
    public class WeatherForecastDto
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("generationtime_ms")]
        public double GenerationtimeMs { get; set; }

        [JsonPropertyName("utc_offset_seconds")]
        public long UtcOffsetSeconds { get; set; }

        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }

        [JsonPropertyName("timezone_abbreviation")]
        public string? TimezoneAbbreviation { get; set; }

        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        [JsonPropertyName("current_units")]
        public CurrentUnits? CurrentUnits { get; set; }

        [JsonPropertyName("current")]
        public Current? Current { get; set; }

        //[JsonPropertyName("hourly_units")]
        //public HourlyUnits? HourlyUnits { get; set; }

        //[JsonPropertyName("hourly")]
        //public Hourly? Hourly { get; set; }
    }

    public class Current
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("interval")]
        public long Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public double Temperature2M { get; set; }
    }

    public partial class CurrentUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }

        [JsonPropertyName("interval")]
        public string? Interval { get; set; }

        [JsonPropertyName("temperature_2m")]
        public string? Temperature2M { get; set; }
    }

    //public class Hourly
    //{
    //    [JsonPropertyName("time")]
    //    public string[]? Time { get; set; }

    //    [JsonPropertyName("temperature_2m")]
    //    public double[]? Temperature2M { get; set; }
    //}

    //public class HourlyUnits
    //{
    //    [JsonPropertyName("time")]
    //    public string? Time { get; set; }

    //    [JsonPropertyName("temperature_2m")]
    //    public string? Temperature2M { get; set; }
    //}
}
