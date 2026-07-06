namespace Ticketing.Tests.Unit.Auth;

using Microsoft.AspNetCore.Identity;
using Ticketing.Domain.Entities;
using Xunit;

public class PasswordHasherTests
{
    private readonly PasswordHasher<User> _hasher = new();

    [Fact]
    public void HashPassword_then_Verify_returns_Success()
    {
        // Arrange
        var user = new User();
        var password = "StrongPassword123";

        // Act
        var hash = _hasher.HashPassword(user, password);
        var result = _hasher.VerifyHashedPassword(user, hash, password);

        // Assert
        Assert.Equal(PasswordVerificationResult.Success, result);
    }

    [Fact]
    public void Verify_with_wrong_password_returns_Failed()
    {
        // Arrange
        var user = new User();
        var password = "StrongPassword123";
        var wrongPassword = "WrongPassword";

        var hash = _hasher.HashPassword(user, password);

        // Act
        var result = _hasher.VerifyHashedPassword(user, hash, wrongPassword);

        // Assert
        Assert.Equal(PasswordVerificationResult.Failed, result);
    }
}
