namespace Ticketing.Api.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public string Key { get; set; } = default!;

    public int AccessTokenMinutes { get; set; } = 60;
}
