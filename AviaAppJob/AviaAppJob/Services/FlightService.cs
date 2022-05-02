using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AviaAppJob.Services;

public class FlightService : IFlightService
{
    private readonly Provider _configuration;
    private readonly IHttpClientService _httpClientService;
    private readonly IAirportService _airportService;
    private readonly ICityService _cityService;
    private readonly ICountryService _countryService;
    private readonly ILogger<FlightService> _logger;
    private readonly Random _rnd;

    public FlightService(
        IConfiguration configuration,
        IHttpClientService httpClientService,
        IAirportService airportService,
        ICityService cityService,
        ICountryService countryService,
        ILogger<FlightService> logger)
    {
        _httpClientService = httpClientService;
        _airportService = airportService;
        _cityService = cityService;
        _countryService = countryService;
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
        _rnd = new Random();
    }

    public async Task AddFlightsAsync(string token)
    {
        var airports = await GetAllAirports(token);
        if (airports.Count >= 5)
        {
            var flightBodies = GenerateFlights(airports);
            foreach (var flightBody in flightBodies)
            {
                await _httpClientService.PostAsync(_configuration.FlightEndpoint, flightBody, token);
            }
        }
        else
        {
            _logger.LogWarning($"Airports count less than 5. Count: {airports.Count}");
        }
    }

    public async Task DeleteOutdatedFlightsAsync(string token)
    {
        _logger.LogInformation("Deleting outdated flights");
        await _httpClientService.DeleteAsync(_configuration.DeleteOutdatedFlightsEndpoint, token);
    }

    private async Task<IList<Airport>> GetAllAirports(string token)
    {
        var countries = await _countryService.GetCountries(token);
        _logger.LogInformation($"Got {countries.Count} countries");
        var cities = new List<City>();
        foreach (var country in countries)
        {
            cities.AddRange(await _cityService.GetCitiesAsync(country.Id, token));
        }
        _logger.LogInformation($"Got {cities.Count} cities");

        var airports = new List<Airport>();
        foreach (var city in cities)
        {
            airports.AddRange(await _airportService.GetAirportsAsync(city.Id, token));
        }
        _logger.LogInformation($"Got {airports.Count} airports");

        return airports;
    }

    private IList<FlightBody> GenerateFlights(IList<Airport> airports)
    {
        _logger.LogInformation("Generating flights");
        var flights = new List<FlightBody>();
        for (var i = 0; i < _configuration.AddFlightsCount; i++)
        {
            var flightBody = SetAirportIds(airports);
            flightBody.Price = _rnd.Next(55, 120);

            // Maybe it will be possible to get airplanes and flight duration from some third party service
            flightBody.Airplane = "Airbus";
            SetFlightTime(flightBody);
            flights.Add(flightBody);
        }

        _logger.LogInformation($"Generated {flights.Count} flights");

        return flights;
    }

    private FlightBody SetAirportIds(IList<Airport> airports)
    {
        var flightBody = new FlightBody();
        var airportIndex = _rnd.Next(0, airports.Count);
        var airportFrom = airports[airportIndex];
        var isAirportChosen = false;

        flightBody.AirportFromId = airportFrom.Id;
        while (!isAirportChosen)
        {
            airportIndex = _rnd.Next(0, airports.Count);
            var airportTo = airports[airportIndex];

            if (airportTo.City.CountryId == airportFrom.City.CountryId) continue;

            flightBody.AirportToId = airportTo.Id;
            isAirportChosen = true;
        }

        return flightBody;
    }

    private void SetFlightTime(FlightBody flightBody)
    {
        var departureHour = _rnd.Next(0, 21);
        var departureMinutes = _rnd.Next(0, 6) * 10;

        var departureDate = DateTime.Now.Date;
        var departureTime = new TimeSpan(departureHour, departureMinutes, 0);
        departureDate += departureTime;

        flightBody.DepartureDateTime = departureDate.AddDays(7);
        flightBody.ArrivalDateTime = flightBody.DepartureDateTime.AddHours(3);
    }
}