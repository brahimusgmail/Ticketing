namespace Ticketing.Shared.Contracts.Tickets;

public sealed record TicketAuthorResponse(
    Guid Id,
    string Email,
    string FullName,
    IReadOnlyCollection<string> Roles);
