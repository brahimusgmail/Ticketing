namespace Ticketing.Tests.Unit.Auth;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit;

public class JwtTokenTests
{
    private const string _key = "CHANGE-ME-TO-A-LONG-SECRET-KEY-AT-LEAST-32-CHARS";
    private const string _issuer = "Ticketing.Api";
    private const string _audience = "Ticketing.Client";

    [Fact]
    public void Generated_token_contains_sub_and_role()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, "user-123"),
            new(ClaimTypes.Role, "Admin"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(10),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        // Act
        var decoded = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        // Assert
        Assert.Equal("user-123", decoded.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value);
        Assert.Contains(decoded.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
    }

    [Fact]
    public void Generated_token_has_correct_issuer_audience_and_expiration()
    {
        // Arrange
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(10);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: expires,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        // Act
        var decoded = new JwtSecurityTokenHandler().ReadJwtToken(jwt);

        // Assert
        Assert.Equal(_issuer, decoded.Issuer);
        Assert.Contains(_audience, decoded.Audiences);
        Assert.True(decoded.ValidTo > DateTime.UtcNow);
    }
}
