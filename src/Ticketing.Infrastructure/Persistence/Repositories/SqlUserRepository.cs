namespace Ticketing.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly TicketingSqlDbContext _dbContext;

    public SqlUserRepository(TicketingSqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, ct);
    }

    public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
    {
        var userIds = ids.Distinct().ToArray();

        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync(ct);
    }

    public async Task<User> InsertAsync(User user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
        await _dbContext.SaveChangesAsync(ct);

        return user;
    }

    public async Task<bool> UpdateAsync(User user, CancellationToken ct)
    {
        _dbContext.Users.Update(user);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (user is null)
        {
            return false;
        }

        _dbContext.Users.Remove(user);

        return await _dbContext.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Email == email, ct);
    }
}
