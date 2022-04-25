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
    private readonly Random _rnd;

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
        _rnd = new Random();
    }

    public async Task AddFlightsAsync(string token)
    {
        var airports = await GetAllAirports(token);
        var flightBodies = GetFlightBodies(airports);
        foreach (var flightBody in flightBodies)
        {
            await _httpClientService.PostAsync(_configuration.FlightEndpoint, flightBody, token);
        }
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

    private IList<FlightBody> GetFlightBodies(IList<Airport> airports)
    {
        var flights = new List<FlightBody>();
        for (int i = 0; i < _configuration.AddFlightsCount; i++)
        {
            var flightBody = SetAirportIds(airports);
            flightBody.Price = _rnd.Next(55, 120);

            // Maybe it will be possible to get airplanes and flight duration from some third party service
            flightBody.Airplane = "Airbus";
            SetFlightTime(flightBody);
            flights.Add(flightBody);
        }

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