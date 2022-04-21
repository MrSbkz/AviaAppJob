namespace AviaAppJob.Services.Contracts;

public interface IAuthService
{
    Task<string> LoginAsync();
}