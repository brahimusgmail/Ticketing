using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    $"Server=(localdb)\\MSSQLLocalDB;Database=Ticketing_Test_{Guid.NewGuid():N};Trusted_Connection=True;TrustServerCertificate=True;",

                ["Jwt:Issuer"] = "Ticketing.Api",
                ["Jwt:Audience"] = "Ticketing.Client",
                ["Jwt:Key"] = "CHANGE-ME-TO-A-LONG-SECRET-KEY-AT-LEAST-32-CHARS",
                ["Jwt:AccessTokenExpirationMinutes"] = "15",
                ["Jwt:RefreshTokenExpirationDays"] = "7",
            };

            config.AddInMemoryCollection(overrides);
        });
    }
}
