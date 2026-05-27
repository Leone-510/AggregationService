using Aggregation.WebAPI.Modules.Weather.DTOs;

namespace Aggregation.WebAPI.Modules.Weather
{
    public interface IWeatherService
    {
        Task<WeatherForecastDto?> GetExternalWeatherCurrentAsync(double lat, double lon);
    }
}
