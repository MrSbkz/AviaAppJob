using AviaAppJob.Models;

namespace AviaAppJob.Services.Contracts;

public interface IAirportService
{
    Task<IList<Airport>> GetAirportsAsync(string token);
}