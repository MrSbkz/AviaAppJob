namespace AviaAppJob.Services.Contracts;

public interface IFlightService
{
    Task AddFlightsAsync(string token);
    Task DeleteOutdatedFlightsAsync(string token);
}