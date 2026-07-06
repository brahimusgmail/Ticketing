namespace Ticketing.Client.Server.Services.Auth;

using System.Net.Http.Json;
using Ticketing.Shared.Contracts.Auth;

public sealed class AuthService
{
    private readonly HttpClient _api;
    private readonly ITokenStore _tokenStore;
    private readonly JwtAuthStateProvider _jwtAuthStateProvider;
    private readonly CurrentUserService _currentUserService;

    public AuthService(HttpClient api, ITokenStore tokenStore, JwtAuthStateProvider jwtAuthStateProvider, CurrentUserService currentUserService)
    {
        _api = api;
        _tokenStore = tokenStore;
        _jwtAuthStateProvider = jwtAuthStateProvider;
        _currentUserService = currentUserService;
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        var req = new LoginRequest { Email = email, Password = password };

        var resp = await _api.PostAsJsonAsync("/api/auth/login", req);

        if (!resp.IsSuccessStatusCode)
        {
            var body = await resp.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(body) ? Resource.Auth_LoginFailed : body);
        }

        var payload = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return (false, Resource.Auth_MissingToken);
        }

        await _tokenStore.SetTokenAsync(payload.AccessToken);
        _jwtAuthStateProvider.NotifyUserAuthentication(payload.AccessToken);

        await _currentUserService.RefreshAsync();

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _tokenStore.ClearAsync();
        _jwtAuthStateProvider.NotifyUserLogout();
        await _tokenStore.ClearAsync();
        _jwtAuthStateProvider.ForceNotify();
        _currentUserService.Clear();
    }

    public Task<string?> GetTokenAsync() => _tokenStore.GetTokenAsync();
}
