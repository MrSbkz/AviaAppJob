using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class CityService : ICityService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;
    private readonly ILogger<CityService> _logger;

    public CityService(IHttpClientService httpClientService, IConfiguration configuration, ILogger<CityService> logger)
    {
        _httpClientService = httpClientService;
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<City>> GetCitiesAsync(Guid countryId, string token)
    {
        _logger.LogInformation($"Getting cities of country with id \'{countryId}\'");
        var result = await _httpClientService.GetAsync(_configuration.CityListEndpoint + countryId, token);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
            return new List<City>();

        var cities = JsonConvert.DeserializeObject<IList<City>>(result)!;
        return cities;
    }
}