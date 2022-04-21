using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class AirportService : IAirportService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;

    public AirportService(IHttpClientService httpClientService, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<Airport>> GetAirportsAsync(Guid cityId, string token)
    {
        var result = await _httpClientService.GetAsync(_configuration.AirportListEndpoint + cityId, token);
        return JsonConvert.DeserializeObject<IList<Airport>>(result)!;
    }
}