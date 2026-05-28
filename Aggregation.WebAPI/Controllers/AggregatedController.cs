using Aggregation.WebAPI.DTOs;
using Aggregation.WebAPI.Modules.Countries;
using Aggregation.WebAPI.Modules.Countries.DTOs;
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
        private readonly ICountryService _countryService;
        private readonly IProductsService _productsService;
        private readonly IWeatherService _weatherService;
        private readonly ILogger<AggregatedController> _logger;

        public AggregatedController(ICountryService countryService, IProductsService productsService, IWeatherService weatherService, 
            ILogger<AggregatedController> logger)
        {
            _countryService = countryService;
            _productsService = productsService;
            _weatherService = weatherService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<AggregatedDto>> GetAggregatedData(
            [FromQuery] double? lat = null, [FromQuery] double? lon = null,
            [FromQuery] string? category = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = "asc")
        {
            double finalLat = lat ?? 37.98; // Default Latitude
            double finalLon = lon ?? 23.72; // Default Longitude

            var aggregatedData = new AggregatedDto();

            var productsTask = FetchProductsAsync(aggregatedData);
            var weatherTask = FetchWeatherForecastAsync(aggregatedData, finalLat, finalLon);
            var countriesTask = FetchCountriesAsync(aggregatedData);

            await Task.WhenAll(productsTask, weatherTask, countriesTask);

            // Filtering with Product category
            if (!string.IsNullOrWhiteSpace(category) && aggregatedData.Products != null && aggregatedData.Products.Any())
            {
                aggregatedData.Products = aggregatedData.Products
                    .Where(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Sorting with Product price, or title
            if (!string.IsNullOrWhiteSpace(sortBy) && aggregatedData.Products != null && aggregatedData.Products.Any())
            {
                bool isDescending = !string.IsNullOrWhiteSpace(sortOrder) && sortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase);

                if (sortBy.Equals("price", StringComparison.OrdinalIgnoreCase))
                {
                    aggregatedData.Products = isDescending
                        ? aggregatedData.Products.OrderByDescending(p => p.Price).ToList()
                        : aggregatedData.Products.OrderBy(p => p.Price).ToList();
                }
                else if (sortBy.Equals("title", StringComparison.OrdinalIgnoreCase))
                {
                    aggregatedData.Products = isDescending
                        ? aggregatedData.Products.OrderByDescending(p => p.Title).ToList()
                        : aggregatedData.Products.OrderBy(p => p.Title).ToList();
                }
            }

            return Ok(aggregatedData);
        }

        #region Safe data wrappers
        private async Task FetchCountriesAsync(AggregatedDto aggregatedData)
        {
            try
            {
                aggregatedData.Countries = await _countryService.GetExternalCountriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve countries data from Rest Countries API.");

                aggregatedData.Countries = new List<CountryDto>();
                aggregatedData.SystemWarnings.Add("Countries data are temporarily unavailable.");
            }
        }

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
