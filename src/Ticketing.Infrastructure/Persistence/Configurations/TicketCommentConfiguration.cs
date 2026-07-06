namespace Ticketing.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Domain.Entities;

public sealed class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("TicketComments");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.HasOne<Ticket>()
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TicketId);

        builder.HasIndex(x => x.AuthorId);

        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
