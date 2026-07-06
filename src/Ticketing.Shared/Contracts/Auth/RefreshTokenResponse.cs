namespace Ticketing.Shared.Contracts.Auth;

public sealed class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
