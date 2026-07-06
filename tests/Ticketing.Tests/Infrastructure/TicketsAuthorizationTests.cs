namespace Ticketing.Tests;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ticketing.Tests.Infrastructure;
using Xunit;

public class TicketsAuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TicketsAuthorizationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Update_returns_403_when_user_is_not_owner_and_not_admin()
    {
        var ct = CancellationToken.None;

        // Arrange
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var (categoryId, ticketId) =
            await TestDataSeeder.SeedCategoryAndTicketAsync(_factory.Services, ownerId, ct);

        var client = _factory.CreateClient();

        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var token = TestAuth.CreateToken(config, otherId, "User");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            Title = "Updated title",
            CategoryId = categoryId,
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/tickets/{ticketId}", body, ct);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Update_returns_204_when_user_is_admin()
    {
        var ct = CancellationToken.None;

        // Arrange
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        var (categoryId, ticketId) =
            await TestDataSeeder.SeedCategoryAndTicketAsync(_factory.Services, ownerId, ct);

        var client = _factory.CreateClient();

        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var token = TestAuth.CreateToken(config, adminId, "Admin");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            Title = "Updated by admin",
            CategoryId = categoryId,
        };

        // Act
        var response = await client.PutAsJsonAsync($"/api/tickets/{ticketId}", body, ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_returns_403_when_user_is_not_owner_and_not_admin()
    {
        var ct = CancellationToken.None;

        // Arrange
        var ownerId = Guid.NewGuid();
        var otherId = Guid.NewGuid();

        var (_, ticketId) =
            await TestDataSeeder.SeedCategoryAndTicketAsync(_factory.Services, ownerId, ct);

        var client = _factory.CreateClient();

        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var token = TestAuth.CreateToken(config, otherId, "User");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.DeleteAsync($"/api/tickets/{ticketId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_returns_204_when_user_is_admin()
    {
        var ct = CancellationToken.None;

        // Arrange
        var ownerId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        var (_, ticketId) =
            await TestDataSeeder.SeedCategoryAndTicketAsync(_factory.Services, ownerId, ct);

        var client = _factory.CreateClient();

        var config = _factory.Services.GetRequiredService<IConfiguration>();
        var token = TestAuth.CreateToken(config, adminId, "Admin");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.DeleteAsync($"/api/tickets/{ticketId}", ct);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
