namespace Ticketing.Application.Abstractions.Repositories;

using Ticketing.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default);

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);

    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
}
