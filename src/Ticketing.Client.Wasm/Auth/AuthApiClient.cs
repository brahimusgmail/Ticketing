namespace Ticketing.Client.Wasm.Auth;

using Microsoft.AspNetCore.Components.WebAssembly.Http;
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
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh");

        httpRequest.SetBrowserRequestCredentials(
            BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(httpRequest, ct);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: ct);
    }
}
