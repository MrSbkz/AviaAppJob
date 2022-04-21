using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class CityService : ICityService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;

    public CityService(IHttpClientService httpClientService, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<City>> GetCitiesAsync(Guid countryId, string token)
    {
        var result = await _httpClientService.GetAsync(_configuration.CityListEndpoint + countryId, token);
        return JsonConvert.DeserializeObject<IList<City>>(result)!;
    }
}