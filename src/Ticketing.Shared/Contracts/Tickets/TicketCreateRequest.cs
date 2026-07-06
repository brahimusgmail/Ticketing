namespace Ticketing.Shared.Contracts.Tickets;

public record TicketCreateRequest(
    string Title,
    Guid? CategoryId);
