namespace Ticketing.Client.Server.Services.Auth;

using System.Net.Http.Json;
using Ticketing.Shared.Contracts.Auth;

public sealed class AuthApiClient
{
    private readonly HttpClient _httpClient;

    public AuthApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<RefreshTokenResponse?> RefreshAsync(CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsync("api/auth/refresh", content: null, ct);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: ct);
    }
}
