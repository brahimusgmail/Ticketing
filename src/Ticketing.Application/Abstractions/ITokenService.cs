namespace Ticketing.Application.Abstractions.Auth;

using Ticketing.Domain.Entities;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
