namespace Ticketing.Client.Server.Services.Auth;

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

public sealed class SessionTokenStore : ITokenStore
{
    private const string _key = "access_token";
    private readonly ProtectedSessionStorage _storage;

    private string? _cachedToken;

    public SessionTokenStore(ProtectedSessionStorage storage)
        => _storage = storage;

    public Task<string?> GetTokenAsync()
    {
        // Pas de JS, donc le handler peut lire sans problème
        return Task.FromResult(_cachedToken);
    }

    // Utilisé uniquement au bootstrap (quand JS OK)
    public async Task LoadFromBrowserAsync()
    {
        try
        {
            var result = await _storage.GetAsync<string>(_key);
            _cachedToken = result.Success ? result.Value : null;
        }
        catch
        {
            _cachedToken = null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        _cachedToken = token;
        await _storage.SetAsync(_key, token);
    }

    public async Task ClearAsync()
    {
        _cachedToken = null;
        await _storage.DeleteAsync(_key);
    }
}
