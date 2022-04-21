namespace AviaAppJob.Models;

public class City : Location
{
    public Guid CountryId { get; set; }

    public Country Country { get; set; }
}