namespace Ticketing.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence;
using Ticketing.Infrastructure.Persistence.Outbox;
using Ticketing.Shared.Contracts.Tickets;
using static Ticketing.Shared.Enums.Ticket.TicketEnums;

public sealed class SqlTicketRepository : ITicketRepository
{
    private readonly TicketingSqlDbContext _dbContext;
    private readonly ILogger<SqlTicketRepository> _logger;

    public SqlTicketRepository(TicketingSqlDbContext dbContext, ILogger<SqlTicketRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Comments)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(ct);
    }

    public async Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Comments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<IEnumerable<Ticket>> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        return await _dbContext.Tickets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<Ticket> InsertAsync(Ticket ticket, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        ticket.CreatedAtUtc = now;
        ticket.UpdatedAtUtc = now;
        ticket.Status = Status.New;

        await _dbContext.Tickets.AddAsync(ticket, ct);

        await _dbContext.SaveChangesAsync(ct);

        return ticket;
    }

    public async Task<bool> UpdateAsync(Ticket ticket, CancellationToken ct)
    {
        _dbContext.Tickets.Update(ticket);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var ticket = await _dbContext.Tickets
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (ticket is null)
        {
            return false;
        }

        _dbContext.Tickets.Remove(ticket);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> AddCommentAsync(
        Guid ticketId,
        TicketComment comment,
        CancellationToken ct)
    {
        var ticketExists = await _dbContext.Tickets
        .FirstOrDefaultAsync(x => x.Id == ticketId, ct);

        if (ticketExists == null)
        {
            return false;
        }

        ticketExists.AddComment(comment);

        comment.TicketId = ticketId;

        await _dbContext.TicketComments.AddAsync(comment, ct);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> CloseAsync(Guid ticketId, CancellationToken ct)
    {
        var ticket = await _dbContext.Tickets
            .FirstOrDefaultAsync(x => x.Id == ticketId, ct);

        if (ticket is null)
        {
            return false;
        }

        // Encapsulate domain logic in the entity
        ticket.Close();

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "TicketClosed",
            Payload = System.Text.Json.JsonSerializer.Serialize(new
            {
                TicketId = ticket.Id,
                Title = ticket.Title,
                ClosedAtUtc = ticket.ClosedAtUtc,
            }),
            CreatedAtUtc = DateTime.UtcNow,
        };

        _dbContext.OutboxMessages.Add(outboxMessage);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public Task<IReadOnlyList<TicketPerformanceDto>> GetByStatusAsync(Status status, int pageSize, CancellationToken ct)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _dbContext.Tickets
            .AsNoTracking()
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAtUtc)
            .Take(pageSize)
            .Select(t => new TicketPerformanceDto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                CreatedAtUtc = t.CreatedAtUtc,
            });
        _logger.LogInformation("Generated SQL for performance lab: {Sql}", query.ToQueryString());
        return Task.FromResult<IReadOnlyList<TicketPerformanceDto>>(query.ToList());
    }
}
