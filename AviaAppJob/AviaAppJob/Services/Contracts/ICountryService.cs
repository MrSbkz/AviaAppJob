using AviaAppJob.Models;

namespace AviaAppJob.Services.Contracts;

public interface ICountryService
{
    Task<IList<Country>> GetCountries(string token);
}