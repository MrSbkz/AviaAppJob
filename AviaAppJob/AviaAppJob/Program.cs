using AviaAppJob.Services;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AviaAppJob;

public static class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<AppWorker>();
                services.AddScoped<IAviaAppService, AviaAppService>();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<IFlightService, FlightService>();
                services.AddScoped<IAirportService, AirportService>();
                services.AddScoped<ICityService, CityService>();
                services.AddScoped<ICountryService, CountryService>();
                services.AddScoped<IAirplaneService, AirplaneService>();

                services.AddScoped<IHttpClientService, HttpClientService>();
            });
}