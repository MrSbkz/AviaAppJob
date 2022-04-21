namespace AviaAppJob.Models;

public class FlightBody
{
    public Guid AirportFromId { get; set; }

    public Guid AirportToId { get; set; }

    public DateTime DepartureDateTime { get; set; }

    public DateTime ArrivalDateTime { get; set; }

    public decimal Price { get; set; }

    public string Airplane { get; set; }
}