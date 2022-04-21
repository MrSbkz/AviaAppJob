using AviaAppJob.Services.Contracts;

namespace AviaAppJob.Services;

public class AviaAppService : IAviaAppService
{
    private readonly IAuthService _authService;
    private readonly IFlightService _flightService;

    public AviaAppService(IAuthService authService, IFlightService flightService)
    {
        _authService = authService;
        _flightService = flightService;
    }

    public async Task StartAsync()
    {
        var token = await _authService.LoginAsync();
        await _flightService.AddFlightsAsync(token);
    }
}