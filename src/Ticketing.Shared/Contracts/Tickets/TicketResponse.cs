namespace Ticketing.Shared.Contracts.Tickets;

using static Ticketing.Shared.Enums.Ticket.TicketEnums;

public record TicketResponse(
    Guid Id,
    string Title,
    Guid? AuthorId,
    TicketAuthorResponse? Author,
    Guid? CategoryId,
    TicketCategoryResponse? Category,
    Status Statut,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    IReadOnlyList<TicketCommentResponse> Comments);
