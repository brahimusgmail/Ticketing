namespace Ticketing.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence.Outbox;

public class TicketingSqlDbContext : DbContext
{
    public TicketingSqlDbContext(DbContextOptions<TicketingSqlDbContext> options)
    : base(options)
    {
    }

    public DbSet<Category> Categories => this.Set<Category>();

    public DbSet<Ticket> Tickets => this.Set<Ticket>();

    public DbSet<User> Users => this.Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<TicketComment> TicketComments => Set<TicketComment>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TicketingSqlDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
