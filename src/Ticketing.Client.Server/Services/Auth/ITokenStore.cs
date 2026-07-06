namespace Ticketing.Client.Server.Services.Auth;

public interface ITokenStore
{
    Task<string?> GetTokenAsync();

    Task SetTokenAsync(string token);

    Task ClearAsync();
}
