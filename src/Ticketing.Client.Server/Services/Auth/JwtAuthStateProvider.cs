namespace Ticketing.Client.Server.Services.Auth;

using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

public sealed class JwtAuthStateProvider : AuthenticationStateProvider
{
    // "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    private static readonly string _roleClaimTypeUri = ClaimTypes.Role;

    private readonly ITokenStore _tokenStore;

    public JwtAuthStateProvider(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anon();
        }

        return BuildAuthState(token);
    }

    public void NotifyUserAuthentication(string token)
        => NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthState(token)));

    public void NotifyUserLogout()
        => NotifyAuthenticationStateChanged(Task.FromResult(Anon()));

    public void ForceNotify()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static AuthenticationState BuildAuthState(string token)
    {
        var claims = ParseClaims(token);

        // Use ClaimTypes.Role so Blazor role-based authorization works correctly.
        var identity = new ClaimsIdentity(
            claims,
            authenticationType: "jwt",
            nameType: "name",
            roleType: ClaimTypes.Role);

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private static AuthenticationState Anon()
        => new(new ClaimsPrincipal(new ClaimsIdentity()));

    private static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var parts = jwt.Split('.');
        if (parts.Length != 3)
        {
            return Array.Empty<Claim>();
        }

        var jsonBytes = DecodeBase64(parts[1]);

        var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
        if (dict is null)
        {
            return Array.Empty<Claim>();
        }

        var claims = new List<Claim>();

        foreach (var (key, value) in dict)
        {
            // roles: "role" / "roles" / ou ClaimTypes.Role (URI)
            if (key is "role" or "roles" || string.Equals(key, _roleClaimTypeUri, StringComparison.OrdinalIgnoreCase))
            {
                if (value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var r in value.EnumerateArray())
                    {
                        var role = r.GetString();
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }
                    }

                    continue;
                }

                if (value.ValueKind == JsonValueKind.String)
                {
                    var role = value.GetString();
                    if (!string.IsNullOrWhiteSpace(role))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    continue;
                }

                // si autre kind, on ignore proprement
                continue;
            }

            claims.Add(new Claim(key, value.ToString()));
        }

        return claims;
    }

    private static byte[] DecodeBase64(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}
