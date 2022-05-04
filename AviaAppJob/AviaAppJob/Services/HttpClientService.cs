using System.Net;
using System.Net.Http.Headers;
using System.Text;
using AviaAppJob.Models;
using AviaAppJob.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AviaAppJob.Services;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly Provider _configuration;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(IConfiguration configuration, ILogger<HttpClientService> logger)
    {
        _logger = logger;
        _configuration = new Provider();
        configuration.GetSection("Provider").Bind(_configuration);
        _httpClient = new HttpClient();
    }


    public async Task<string?> PostAsync(string endpoint, object body, string token)
    {
        try
        {
            SetHttpClient(token);
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8,
                _configuration.ContentType);
            var response = await _httpClient.PostAsync(_configuration.BaseUrl + endpoint, content);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning($"POST REQUEST: Failed response, endpoint {endpoint}, {response.Content}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception: {e.Message}");
            return null;
        }
    }


    public async Task<string?> DeleteAsync(string endpoint, string token)
    {
        try
        {
            SetHttpClient(token);
            var response = await _httpClient.DeleteAsync(_configuration.BaseUrl + endpoint);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning($"DELETE REQUEST: Failed response, endpoint {endpoint}, {response.Content}");
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception: {e.Message}");
            return null;
        }
    }

    public async Task<string?> GetAsync(string endpoint, string token)
    {
        try
        {
            SetHttpClient(token);
            var response = await _httpClient.GetAsync(_configuration.BaseUrl + endpoint);
            if (response.StatusCode != HttpStatusCode.OK)
                _logger.LogWarning($"GET REQUEST: Failed response, endpoint {endpoint}, {response.Content}");
            return await response.Content.ReadAsStringAsync();

        }
        catch (Exception e)
        {
            _logger.LogWarning($"Exception: {e.Message}");
            return null;
        }
    }

    private void SetHttpClient(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_configuration.ContentType));
    }
}