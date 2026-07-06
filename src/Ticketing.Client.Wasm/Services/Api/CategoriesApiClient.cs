namespace Ticketing.Client.Wasm.Services.Api;

using System.Net.Http.Json;
using Ticketing.Shared.Contracts.Categories;

public sealed class CategoriesApiClient
{
    private readonly HttpClient _http;

    public CategoriesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken ct = default)
    {
        return await _http.GetFromJsonAsync<List<CategoryResponse>>("api/categories", ct) ?? [];
    }

    public async Task<CategoryResponse> CreateAsync(CategoryCreateRequest req, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync("api/categories", req, ct);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<CategoryResponse>(cancellationToken: ct))!;
    }

    public async Task<CategoryResponse> UpdateAsync(Guid id, CategoryUpdateRequest req, CancellationToken ct = default)
    {
        var response = await _http.PatchAsJsonAsync($"api/categories/{id}", req, ct);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<CategoryResponse>(cancellationToken: ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync($"api/categories/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
