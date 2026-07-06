namespace Ticketing.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions.Repositories;
using Ticketing.Domain.Entities;
using Ticketing.Infrastructure.Persistence;

public sealed class SqlRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly TicketingSqlDbContext _dbContext;

    public SqlRefreshTokenRepository(TicketingSqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens
            .AddAsync(refreshToken, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        _dbContext.RefreshTokens.Update(refreshToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
