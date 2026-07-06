using Ticketing.Shared.Enums.Ticket;

namespace Ticketing.Shared.Contracts.Tickets;

public sealed class TicketPerformanceDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public TicketEnums.Status Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
}
