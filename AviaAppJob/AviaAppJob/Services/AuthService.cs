using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class AuthService : IAuthService
{
    private readonly IHttpClientService _httpClientService;
    private readonly AuthData _authData;
    private readonly Provider _provider;

    public AuthService(IHttpClientService httpClientService, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _authData = new AuthData();
        _provider = new Provider();
        configuration.GetSection("AuthData").Bind(_authData);
        configuration.GetSection("Provider").Bind(_provider);
    }

    public async Task<string> LoginAsync()
    {
        var result = await _httpClientService.PostAsync(_provider.LoginEndpoint, _authData, null);
        return JsonConvert.DeserializeObject<LoginResponse>(result)!.Token;
    }
}