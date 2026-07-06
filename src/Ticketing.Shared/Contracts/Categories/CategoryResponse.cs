namespace Ticketing.Shared.Contracts.Categories;

public record CategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
