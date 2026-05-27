using Aggregation.WebAPI.Modules.Products.DTOs;
using Aggregation.WebAPI.Modules.Weather.DTOs;

namespace Aggregation.WebAPI.DTOs
{
    public class AggregatedDto
    {
        public List<ProductDto> Products { get; set; } = new();
        public WeatherForecastDto? WeatherForecast { get; set; }

        public List<string> SystemWarnings { get; set; } = new();
    }
}
