namespace Ticketing.Infrastructure.Persistence.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Payload)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedAtUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);

        builder.Property(x => x.RetryCount)
            .HasDefaultValue(0);

        builder.HasIndex(x => x.ProcessedAtUtc);
    }
}
