using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AviaAppJob;

public class AppWorker : IHostedService
{
    private Timer _timer;
    private readonly ILogger<AppWorker> _logger;
    private readonly Provider _configuration;
    private readonly IAviaAppService _appService;

    public AppWorker(ILogger<AppWorker> logger, IConfiguration configuration, IAviaAppService appService)
    {
        _logger = logger;
        _appService = appService;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(AppWorker) + " is working");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(_configuration.TimeSpan));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation(nameof(AppWorker) + " is stopping");

        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        await _appService.StartAsync();
    }
}