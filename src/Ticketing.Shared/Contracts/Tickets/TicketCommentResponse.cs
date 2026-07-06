namespace Ticketing.Shared.Contracts.Tickets;

public sealed record TicketCommentResponse(
    Guid Id,
    string Message,
    Guid AuthorId,
    TicketAuthorResponse? Author,
    DateTime CreatedAtUtc);
