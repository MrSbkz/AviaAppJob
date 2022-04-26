using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientService _httpClientService;
    private readonly AuthData _authData;
    private readonly Provider _provider;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientService httpClientService, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _httpClientService = httpClientService;
        _logger = logger;
        _authData = new AuthData();
        _provider = new Provider();
        configuration.GetSection("AuthData").Bind(_authData);
        configuration.GetSection("Provider").Bind(_provider);
    }

    public async Task<string> LoginAsync()
    {
        _logger.LogInformation("");
        var result = await _httpClientService.PostAsync(_provider.LoginEndpoint, _authData, null);
        if (string.IsNullOrEmpty(result) || string.IsNullOrWhiteSpace(result))
        {
            _logger.LogWarning("Authorization failed");
            return string.Empty;
        }

        return JsonConvert.DeserializeObject<LoginResponse>(result)!.Token;
    }
}