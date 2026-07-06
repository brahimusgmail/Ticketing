namespace Ticketing.Application.Abstractions;

using Ticketing.Domain.Entities;

public interface IUserRepository
{
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct);

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct);

    Task<User?> GetByEmailAsync(string email, CancellationToken ct);

    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);

    Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct);

    Task<User> InsertAsync(User user, CancellationToken ct);

    Task<bool> UpdateAsync(User user, CancellationToken ct);
}
