namespace Ticketing.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Email { get; set; } = default!;

    public string FullName { get; set; } = default!;

    public string? PasswordHash { get; set; }

    public List<string> Roles { get; set; } = new();

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public bool IsActive { get; set; }
}
