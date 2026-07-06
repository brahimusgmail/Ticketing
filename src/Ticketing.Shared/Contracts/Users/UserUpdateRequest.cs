namespace Ticketing.Shared.Contracts.Users;

public record UserUpdateRequest(
    string FullName,
    List<string> Roles,
    bool IsActive);
