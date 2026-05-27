using Aggregation.WebAPI.DTOs;
using Aggregation.WebAPI.Modules.Products;
using Aggregation.WebAPI.Modules.Products.DTOs;
using Aggregation.WebAPI.Modules.Weather;
using Microsoft.AspNetCore.Mvc;

namespace Aggregation.WebAPI.Controllers
{
    [ApiController]
    [Route("api/aggregated")]
    public class AggregatedController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly IWeatherService _weatherService;
        private readonly ILogger<AggregatedController> _logger;

        public AggregatedController(IProductsService productsService, IWeatherService weatherService, ILogger<AggregatedController> logger)
        {
            _productsService = productsService;
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<AggregatedDto>> GetAggregatedData([FromQuery] double? lat, [FromQuery] double? lon)
        {
            double finalLat = lat ?? 37.98; // Default Latitude
            double finalLon = lon ?? 23.72; // Default Longitude

            var aggregatedData = new AggregatedDto();
            var productsTask = FetchProductsAsync(aggregatedData);
            var weatherTask = FetchWeatherForecastAsync(aggregatedData, finalLat, finalLon);

            await Task.WhenAll(productsTask, weatherTask);

            return Ok(aggregatedData);
        }

        #region Safe data wrappers
        private async Task FetchProductsAsync(AggregatedDto aggregatedData)
        {
            try
            {
                aggregatedData.Products = await _productsService.GetExternalProductsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve products data from Fake Store API.");

                aggregatedData.Products = new List<ProductDto>();
                aggregatedData.SystemWarnings.Add("Products data are temporarily unavailable.");
            }
        }

        private async Task FetchWeatherForecastAsync(AggregatedDto aggregatedData, double lat, double lon)
        {
            try
            {
                aggregatedData.WeatherForecast = await _weatherService.GetExternalWeatherCurrentAsync(lat, lon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve weather forecast data from Open-Meteo API.");

                aggregatedData.WeatherForecast = null;
                aggregatedData.SystemWarnings.Add("Weather forecast data are temporarily unavailable.");
            }
        }
        #endregion
    }
}
