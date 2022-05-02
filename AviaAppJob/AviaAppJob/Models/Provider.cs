namespace AviaAppJob.Models;

public class Provider
{
    public int TimeSpan { get; set; }

    public string Endpoint { get; set; }

    public string ContentType { get; set; }

    public string LoginEndpoint { get; set; }

    public string AirportListEndpoint { get; set; }

    public string CityListEndpoint { get; set; }

    public string CountryListEndpoint { get; set; }

    public string FlightEndpoint { get; set; }
    
    public string DeleteOutdatedFlightsEndpoint { get; set; }

    public int AddFlightsCount { get; set; }
}