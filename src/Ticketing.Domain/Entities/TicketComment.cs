namespace Ticketing.Domain.Entities;

public sealed class TicketComment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Message { get; set; } = default!;

    public Guid AuthorId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public Guid TicketId { get; set; }
}
