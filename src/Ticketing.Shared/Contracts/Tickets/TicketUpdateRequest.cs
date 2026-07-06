namespace Ticketing.Shared.Contracts.Tickets;

public record TicketUpdateRequest(
    string Title,
    Guid? CategoryId);
