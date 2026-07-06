namespace Ticketing.Api.Setup;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;

public sealed class DemoDataSeeder
{
    private readonly IUserRepository _users;
    private readonly PasswordHasher<User> _passwordHasher = new();
    private readonly DemoUserOptions _options;

    public DemoDataSeeder(
        IUserRepository users,
        IOptions<DemoUserOptions> options)
    {
        _users = users;
        _options = options.Value;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Email))
        {
            return;
        }

        var existing = await _users.GetByEmailAsync(_options.Email, ct);

        if (existing is not null)
        {
            return;
        }

        var user = new User
        {
            Email = _options.Email,
            FullName = _options.FullName,
            Roles = new List<string> { "Admin" },
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            IsActive = true,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, _options.Password);

        await _users.InsertAsync(user, ct);
    }
}
