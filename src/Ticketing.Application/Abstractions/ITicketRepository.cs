namespace Ticketing.Application.Abstractions;

using Ticketing.Domain.Entities;
using Ticketing.Shared.Contracts.Tickets;
using Ticketing.Shared.Enums.Ticket;

public interface ITicketRepository
{
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken ct);

    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IEnumerable<Ticket>> GetPagedAsync(int page, int pageSize, CancellationToken ct);

    Task<Ticket> InsertAsync(Ticket ticket, CancellationToken ct);

    Task<bool> UpdateAsync(Ticket ticket, CancellationToken ct);

    Task<bool> AddCommentAsync(Guid ticketId, TicketComment comment, CancellationToken ct);

    Task<bool> CloseAsync(Guid ticketId, CancellationToken ct);

    Task<IReadOnlyList<TicketPerformanceDto>> GetByStatusAsync(TicketEnums.Status status, int pageSize, CancellationToken ct);
}
