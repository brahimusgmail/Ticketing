namespace Ticketing.Client.Server.Services.Api;

using System.Net.Http.Json;
using Ticketing.Shared.Contracts.Categories;

public sealed class CategoriesApiClient
{
    private readonly HttpClient _http;

    public CategoriesApiClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
        => await _http.GetFromJsonAsync<List<CategoryResponse>>("api/categories", ct) ?? [];

    public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest req, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync("api/categories", req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CategoryResponse>(cancellationToken: ct))!;
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, CategoryUpdateRequest req, CancellationToken ct = default)
    {
        var res = await _http.PatchAsJsonAsync($"api/categories/{id}", req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CategoryResponse>(cancellationToken: ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync($"api/categories/{id}", ct);
        res.EnsureSuccessStatusCode();
    }
}
