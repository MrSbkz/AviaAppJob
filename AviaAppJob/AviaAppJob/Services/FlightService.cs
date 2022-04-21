using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace AviaAppJob.Services;

public class FlightService : IFlightService
{
    private readonly Provider _configuration;
    private readonly IHttpClientService _httpClientService;
    private readonly IAirportService _airportService;
    private readonly ICityService _cityService;
    private readonly ICountryService _countryService;

    public FlightService(
        IConfiguration configuration,
        IHttpClientService httpClientService,
        IAirportService airportService,
        ICityService cityService,
        ICountryService countryService)
    {
        _httpClientService = httpClientService;
        _airportService = airportService;
        _cityService = cityService;
        _countryService = countryService;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public async Task AddFlightsAsync(string token)
    {
        var airports = await GetAllAirports(token);
    }

    private async Task<IList<Airport>> GetAllAirports(string token)
    {
        var countries = await _countryService.GetCountries(token);
        var cities = new List<City>();
        foreach (var country in countries)
        {
            cities.AddRange(await _cityService.GetCitiesAsync(country.Id, token));
        }

        var airports = new List<Airport>();
        foreach (var city in cities)
        {
            airports.AddRange(await _airportService.GetAirportsAsync(city.Id, token));
        }

        return airports;
    }
}