using System.Net.Http.Headers;
using System.Text;
using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly Provider _configuration;

    public HttpClientService(IConfiguration configuration)
    {
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
        _httpClient = new HttpClient();
    }


    public async Task<string> PostAsync(string endpoint, object body, string token)
    {
        SetHttpClient(token);
        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, _configuration.ContentType);
        var response = await _httpClient.PostAsync(_configuration.Endpoint + endpoint, content);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetAsync(string endpoint, string token)
    {
        SetHttpClient(token);
        var response = await _httpClient.GetAsync(_configuration.Endpoint + endpoint);
        return await response.Content.ReadAsStringAsync();
    }

    private void SetHttpClient(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_configuration.ContentType));
    }
}