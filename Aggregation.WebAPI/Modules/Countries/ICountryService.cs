using Aggregation.WebAPI.Modules.Countries.DTOs;

namespace Aggregation.WebAPI.Modules.Countries
{
    public interface ICountryService
    {
        Task<List<CountryDto>> GetExternalCountriesAsync();
    }
}
