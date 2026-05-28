using Aggregation.WebAPI.Controllers;
using Aggregation.WebAPI.DTOs;
using Aggregation.WebAPI.Modules.Countries;
using Aggregation.WebAPI.Modules.Countries.DTOs;
using Aggregation.WebAPI.Modules.Products;
using Aggregation.WebAPI.Modules.Products.DTOs;
using Aggregation.WebAPI.Modules.Weather;
using Aggregation.WebAPI.Modules.Weather.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Aggregation.Tests
{
    public class AggregatedControllerTest
    {
        private readonly Mock<ICountryService> _countriesServiceMock;
        private readonly Mock<IProductsService> _productsServiceMock;
        private readonly Mock<IWeatherService> _weatherServiceMock;
        private readonly Mock<ILogger<AggregatedController>> _loggerMock;
        private readonly AggregatedController _controller;

        public AggregatedControllerTest()
        {
            _countriesServiceMock = new Mock<ICountryService>();
            _productsServiceMock = new Mock<IProductsService>();
            _weatherServiceMock = new Mock<IWeatherService>();
            _loggerMock = new Mock<ILogger<AggregatedController>>();

            _controller = new AggregatedController(_countriesServiceMock.Object, _productsServiceMock.Object, 
                _weatherServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAggregatedData_Returns_200OK_With_All_Data_When_Services_Succeed()
        {
            var testProducts = new List<ProductDto> { new ProductDto { Id = 1, Title = "Test Product" } };
            var testWeather = new WeatherForecastDto { Latitude = 37.98, Longitude = 23.72 };
            var testCountries = new List<CountryDto>
            {
                new CountryDto
                {
                    Name = new CountryName
                    {
                        Common = "CommonName",
                        Official = "OfficialName",
                        NativeName = new Dictionary<string, NativeNameInfo> { { "KEY", new NativeNameInfo { Official = "Official", Common = "Common" } } }
                    },
                    Currencies = new Dictionary<string, CurrencyInfo>
                    {
                        { "XCD", new CurrencyInfo { Name = "Eastern Caribbean dollar", Symbol = "$" } }
                    },
                    Capital = new List<string> { "Capital" }
                }
            };

            // Mocks return test data
            _productsServiceMock.Setup(s => s.GetExternalProductsAsync()).ReturnsAsync(testProducts);
            _weatherServiceMock.Setup(s => s.GetExternalWeatherCurrentAsync(It.IsAny<double>(), It.IsAny<double>())).ReturnsAsync(testWeather);
            _countriesServiceMock.Setup(c => c.GetExternalCountriesAsync()).ReturnsAsync(testCountries);

            // Act for Controller method
            var result = await _controller.GetAggregatedData(37.98, 23.72);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var aggregated = Assert.IsType<AggregatedDto>(okResult.Value);

            Assert.NotNull(aggregated.Products);
            Assert.Single(aggregated.Products);          // Confirmation for Product
            Assert.Equal("Test Product", aggregated.Products[0].Title);
            Assert.NotNull(aggregated.WeatherForecast);
            Assert.NotNull(aggregated.Countries);
            Assert.Empty(aggregated.SystemWarnings);     // Should not have any warnings
        }

        [Fact]
        public async Task GetAggregatedData_Handles_Partial_Failure_When_Weather_Service_Throws_Exception()
        {
            var testProducts = new List<ProductDto> { new ProductDto { Id = 1, Title = "Test Product" } };
            _productsServiceMock.Setup(s => s.GetExternalProductsAsync()).ReturnsAsync(testProducts);

            var testCountries = new List<CountryDto>
            {
                new CountryDto
                {
                    Name = new CountryName
                    {
                        Common = "CommonName",
                        Official = "OfficialName",
                        NativeName = new Dictionary<string, NativeNameInfo> { { "KEY", new NativeNameInfo { Official = "Official", Common = "Common" } } }
                    },
                    Currencies = new Dictionary<string, CurrencyInfo>
                    {
                        { "XCD", new CurrencyInfo { Name = "Eastern Caribbean dollar", Symbol = "$" } }
                    },
                    Capital = new List<string> { "Capital" }
                }
            };
            _countriesServiceMock.Setup(c => c.GetExternalCountriesAsync()).ReturnsAsync(testCountries);

            _weatherServiceMock.Setup(s => s.GetExternalWeatherCurrentAsync(It.IsAny<double>(), It.IsAny<double>()))
                .ThrowsAsync(new HttpRequestException("API Offline"));

            var result = await _controller.GetAggregatedData(37.98, 23.72);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var aggregated = Assert.IsType<AggregatedDto>(okResult.Value);

            // API should return 200 OK for Products and Countries, but NULL Weather and WARNING!
            Assert.NotNull(aggregated.Countries);
            Assert.NotNull(aggregated.Products);
            Assert.Single(aggregated.Products);
            Assert.Null(aggregated.WeatherForecast);    // Weather failed, so it is null
            Assert.Single(aggregated.SystemWarnings);   // Should contain warning message
            Assert.Contains("Weather", aggregated.SystemWarnings[0]);
        }
    }
}