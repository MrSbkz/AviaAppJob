using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;

namespace AviaAppJob.Services;

public class AirplaneService : IAirplaneService
{
    private readonly IList<string> _airplanes;

    public AirplaneService(IConfiguration configuration)
    {
        _airplanes = configuration.GetSection("Airplanes").Get<IList<string>>()!;
    }

    public string GetAirplaneName()
    {
        var rnd = new Random();
        return _airplanes[rnd.Next(0, _airplanes.Count)];
    }
}