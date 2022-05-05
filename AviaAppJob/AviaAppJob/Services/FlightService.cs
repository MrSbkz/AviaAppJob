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
    private readonly IAirplaneService _airplaneService;
    private readonly ILogger<FlightService> _logger;
    private readonly Random _rnd;

    public FlightService(
        IConfiguration configuration,
        IHttpClientService httpClientService,
        IAirportService airportService,
        IAirplaneService airplaneService,
        ILogger<FlightService> logger)
    {
        _httpClientService = httpClientService;
        _airportService = airportService;
        _airplaneService = airplaneService;
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
        _rnd = new Random();
    }

    public async Task AddFlightsAsync(string token)
    {
        var airports = await _airportService.GetAirportsAsync(token);
        var countriesCount = airports.GroupBy(x => x.City.CountryId).Select(x => x.First()).Count();
        if (countriesCount >= _configuration.CountriesCountMinimum)
        {
            var flightBodies = GenerateFlights(airports);
            await _httpClientService.PostAsync(_configuration.AddFlightsEndpoint, flightBodies, token);
        }
        else
        {
            _logger.LogWarning($"Countries count less than {_configuration.CountriesCountMinimum}. Count: {countriesCount}");
        }
    }

    public async Task DeleteOutdatedFlightsAsync(string token)
    {
        _logger.LogInformation("Deleting outdated flights");
        await _httpClientService.DeleteAsync(_configuration.DeleteOutdatedFlightsEndpoint, token);
    }

    private IList<FlightBody> GenerateFlights(IList<Airport> airports)
    {
        _logger.LogInformation("Generating flights");
        var flights = new List<FlightBody>();
        for (var i = 0; i < _configuration.AddFlightsCount; i++)
        {
            var flightBody = GenerateFlightWithAirportIds(airports);
            flightBody.Price = _rnd.Next(55, 120);
            flightBody.Airplane = _airplaneService.GetAirplaneName();
            SetFlightTime(flightBody);
            flights.Add(flightBody);
        }

        _logger.LogInformation($"Generated {flights.Count} flights");

        return flights;
    }

    private FlightBody GenerateFlightWithAirportIds(IList<Airport> airports)
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
        var departureHour = _rnd.Next(3, 27);
        var departureMinutes = _rnd.Next(0, 6) * 10;

        var departureDate = DateTime.Now.Date.AddDays(_rnd.Next(1, 7));
        var departureTime = new TimeSpan(departureHour, departureMinutes, 0);
        departureDate += departureTime;

        flightBody.DepartureDateTime = departureDate;
        flightBody.ArrivalDateTime = flightBody.DepartureDateTime.AddHours(3);
    }
}