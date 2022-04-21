namespace AviaAppJob.Models;

public class Airport : Location
{
    public Guid CityId { get; set; }

    public City City { get; set; }
}