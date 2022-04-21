using AviaAppJob.Models;

namespace AviaAppJob.Services.Contracts;

public interface ICityService
{
    Task<IList<City>> GetCitiesAsync(Guid countryId, string token);
}