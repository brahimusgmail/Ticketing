namespace Ticketing.Shared.Contracts.Users;

public record UserResponse(
    Guid Id,
    string Email,
    string FullName,
    List<string> Roles,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    bool IsActive);
