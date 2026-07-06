namespace Ticketing.Infrastructure.Persistence.Outbox;

public class OutboxMessage
{
    public Guid Id { get; set; }

    public string Type { get; set; } = default!;

    public string Payload { get; set; } = default!;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? ProcessedAtUtc { get; set; }

    public string? Error { get; set; }

    public int? RetryCount { get; set; }
}
