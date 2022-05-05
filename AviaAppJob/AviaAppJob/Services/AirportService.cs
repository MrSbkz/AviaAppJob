using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class AirportService : IAirportService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;
    private readonly ILogger<AirportService> _logger;

    public AirportService(
        IHttpClientService httpClientService,
        IConfiguration configuration,
        ILogger<AirportService> logger)
    {
        _httpClientService = httpClientService;
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<Airport>> GetAirportsAsync(string token)
    {
        _logger.LogInformation($"Getting airports");
        var result = await _httpClientService.GetAsync(_configuration.AirportListEndpoint, token);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
            return new List<Airport>();

        var airports = JsonConvert.DeserializeObject<IList<Airport>>(result)!;
        return airports;
    }
}