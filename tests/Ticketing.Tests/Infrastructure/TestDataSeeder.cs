namespace Ticketing.Tests.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence;

public static class TestDataSeeder
{
    public static async Task<(Guid CategoryId, Guid TicketId)> SeedCategoryAndTicketAsync(
        IServiceProvider services,
        Guid authorId,
        CancellationToken ct)
    {
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<TicketingSqlDbContext>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = "fake-hash",
        };

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = "Bug",
            Description = "Bug category",
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };


        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Initial ticket",
            CategoryId = category.Id,
            AuthorId = user.Id,
        };

        db.Users.Add(user);
        db.Categories.Add(category);
        db.Tickets.Add(ticket);

        await db.SaveChangesAsync();

        return (CategoryId: category.Id, TicketId: ticket.Id);
    }
}
