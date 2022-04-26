using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class CountryService : ICountryService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;
    private readonly ILogger<CountryService> _logger;

    public CountryService(
        IHttpClientService httpClientService,
        IConfiguration configuration,
        ILogger<CountryService> logger)
    {
        _httpClientService = httpClientService;
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<Country>> GetCountries(string token)
    {
        _logger.LogInformation("Getting flights");
        var result = await _httpClientService.GetAsync(_configuration.CountryListEndpoint, token);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
            return new List<Country>();

        var countries = JsonConvert.DeserializeObject<IList<Country>>(result)!;
        return countries;
    }
}