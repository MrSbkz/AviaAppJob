namespace AviaAppJob.Services.Contracts;

public interface IHttpClientService
{
    Task<string?> PostAsync(string endpoint, object body, string token);

    Task<string?> GetAsync(string endpoint, string token);

    Task<string?> DeleteAsync(string endpoint, string token);
}