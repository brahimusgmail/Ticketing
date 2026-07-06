namespace Ticketing.Domain.Entities;

public sealed class RefreshToken
{
    public RefreshToken(Guid userId, string tokenHash, DateTime expiresAtUtc)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAtUtc = expiresAtUtc;
    }

    private RefreshToken()
    {
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UserId { get; private set; }

    public string TokenHash { get; private set; } = default!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? RevokedAtUtc { get; private set; }

    public string? ReplacedByTokenHash { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public bool IsRevoked => RevokedAtUtc.HasValue;

    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke()
    {
        RevokedAtUtc = DateTime.UtcNow;
    }
}
