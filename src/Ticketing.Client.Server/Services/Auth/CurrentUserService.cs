namespace Ticketing.Client.Server.Services.Auth;

using System.Net.Http.Json;
using System.Text.Json.Serialization;

public sealed class CurrentUserService
{
    private readonly HttpClient _http;

    public CurrentUserService(HttpClient http) => _http = http;

    public event Action? OnChange;

    public CurrentUser? User { get; private set; }

    public async Task RefreshAsync(CancellationToken ct = default)
    {
        var resp = await _http.GetAsync("/api/auth/me", ct);
        if (!resp.IsSuccessStatusCode)
        {
            User = null;
            Notify();
            return;
        }

        User = await resp.Content.ReadFromJsonAsync<CurrentUser>(cancellationToken: ct);
        Notify();
    }

    public void Clear()
    {
        User = null;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();

    public sealed record CurrentUser(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("email")] string? Email,
    [property: JsonPropertyName("fullName")] string? FullName,
    [property: JsonPropertyName("roles")] string[] Roles);
}
