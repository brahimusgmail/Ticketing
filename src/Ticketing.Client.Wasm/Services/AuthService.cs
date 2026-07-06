namespace Ticketing.Client.Wasm.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Ticketing.Client.Wasm.Auth;
using Ticketing.Shared.Contracts.Auth;

public sealed class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtAuthStateProvider _authStateProvider;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _authStateProvider = (JwtAuthStateProvider)authStateProvider;
    }

    public async Task<bool> LoginAsync(
    string email,
    string password,
    CancellationToken ct = default)
    {
        var request = new LoginRequest
        {
            Email = email,
            Password = password,
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/login");

        httpRequest.SetBrowserRequestCredentials(
            BrowserRequestCredentials.Include);

        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest, ct);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>(
            cancellationToken: ct);

        if (payload is null || string.IsNullOrWhiteSpace(payload.AccessToken))
        {
            return false;
        }

        await _authStateProvider.MarkUserAsAuthenticatedAsync(payload.AccessToken);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", payload.AccessToken);

        return true;
    }

    public async Task LogoutAsync()
    {
        _httpClient.DefaultRequestHeaders.Authorization = null;
        await _authStateProvider.MarkUserAsLoggedOutAsync();
    }
}
