namespace Ticketing.Shared.Contracts.Users;

public record UserCreateRequest(
    string Email,
    string FullName,
    List<string> Roles);
