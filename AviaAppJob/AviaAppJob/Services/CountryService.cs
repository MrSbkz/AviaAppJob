using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class CountryService : ICountryService
{
    private readonly IHttpClientService _httpClientService;
    private readonly Provider _configuration;

    public CountryService(IHttpClientService httpClientService, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task<IList<Country>> GetCountries(string token)
    {
        var result = await _httpClientService.GetAsync(_configuration.CountryListEndpoint, token);
        return JsonConvert.DeserializeObject<IList<Country>>(result)!;
    }
}