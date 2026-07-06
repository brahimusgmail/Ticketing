namespace Ticketing.Api.Controllers;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ticketing.Api.Auth;
using Ticketing.Application.Abstractions;
using Ticketing.Application.Abstractions.Auth;
using Ticketing.Application.Abstractions.Repositories;
using Ticketing.Domain.Entities;
using Ticketing.Shared.Contracts.Auth;
using Ticketing.Shared.Contracts.Users;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshToken;
    private readonly JwtOptions _jwtOptions;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(
            IUserRepository users,
            IOptions<JwtOptions> jwtOptions,
            ITokenService tokenService,
            IRefreshTokenRepository refreshToken)
    {
        _users = users;
        _tokenService = tokenService;
        _refreshToken = refreshToken;
        _jwtOptions = jwtOptions.Value;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(Resource.UserMustHaveEmailMessage);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(Resource.PasswordRequiredMessage);
        }

        if (request.Password.Length < 8)
        {
            return BadRequest(Resource.PasswordTooShortMessage);
        }

        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(Resource.UserMustHaveFullNameMessage);
        }

        var email = request.Email.Trim().ToLowerInvariant();
        var fullName = request.FullName.Trim();

        if (await _users.ExistsByEmailAsync(email, ct))
        {
            return Conflict(Resource.UserExistsMessage);
        }

        var user = new User
        {
            Email = email,
            FullName = fullName,
            IsActive = true,
            Roles = new List<string> { "utilisateur" },
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        var created = await _users.InsertAsync(user, ct);

        return Created(string.Empty, new UserResponse(created.Id, created.Email, created.FullName, created.Roles, created.CreatedAtUtc, created.UpdatedAtUtc, created.IsActive));
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest(Resource.UserMustHaveEmailMessage);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(Resource.PasswordRequiredMessage);
        }

        var email = request.Email.Trim().ToLowerInvariant();

        var user = await _users.GetByEmailAsync(email, ct);

        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash) || !user.IsActive)
        {
            return Unauthorized(Resource.InvalidCredentialsMessage);
        }

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            return Unauthorized(Resource.InvalidCredentialsMessage);
        }

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

        var refreshTokenEntity = new RefreshToken(
            user.Id,
            refreshTokenHash,
            DateTime.UtcNow.AddDays(7));

        await _refreshToken.AddAsync(refreshTokenEntity, ct);

        SetRefreshTokenCookie(refreshToken);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
        });
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<MeResponse>> Me(CancellationToken ct)
    {
        var userIdClaim = GetFirstClaimValue(JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userIdClaim)
            || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var email = GetFirstClaimValue(JwtRegisteredClaimNames.Email, "email", ClaimTypes.Email);

        var roles = GetRoles();

        var user = await _users.GetByIdAsync(userId, ct);

        var fullName = user?.FullName;

        return Ok(new MeResponse(
            userId,
            email,
            fullName,
            roles));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh(CancellationToken ct)
    {

        if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            return Unauthorized();
        }

        var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

        var storedRefreshToken = await _refreshToken.GetByTokenHashAsync(refreshTokenHash, ct);

        if (storedRefreshToken is null || !storedRefreshToken.IsActive)
        {
            return Unauthorized();
        }

        var user = await _users.GetByIdAsync(storedRefreshToken.UserId, ct);

        if (user is null || !user.IsActive)
        {
            return Unauthorized();
        }

        storedRefreshToken.Revoke();
        await _refreshToken.UpdateAsync(storedRefreshToken, ct);

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken(
            user.Id,
            newRefreshTokenHash,
            DateTime.UtcNow.AddDays(7));

        await _refreshToken.AddAsync(newRefreshTokenEntity, ct);

        SetRefreshTokenCookie(newRefreshToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user);

        return Ok(new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenMinutes),
        });
    }

    private string? GetFirstClaimValue(params string[] types)
    {
        foreach (var t in types)
        {
            var value = User.FindFirstValue(t);
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private string[] GetRoles()
    {
        return User.FindAll("roles").Select(c => c.Value)
            .Concat(User.FindAll("role").Select(c => c.Value))
            .Concat(User.FindAll(ClaimTypes.Role).Select(c => c.Value))
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
        });
    }
}
