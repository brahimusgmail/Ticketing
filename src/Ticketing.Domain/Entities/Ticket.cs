namespace Ticketing.Domain.Entities;

using System;
using Ticketing.Domain.Exceptions;
using static Ticketing.Shared.Enums.Ticket.TicketEnums;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; } = default!;

    public Guid AuthorId { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public Guid CategoryId { get; set; }

    public Status Status { get; set; }

    public List<TicketComment> Comments { get; set; } = new();

    public DateTime? ClosedAtUtc { get; set; }

    public void AddComment(TicketComment comment)
    {
        if (Status == Status.Closed)
        {
             throw new TicketCloseException();
        }

        Comments.Add(comment);
    }

    public void Close()
    {
        if (Status == Status.Closed)
        {
            return;
        }

        Status = Status.Closed;
        ClosedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
