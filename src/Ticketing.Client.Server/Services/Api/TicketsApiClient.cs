namespace Ticketing.Client.Server.Services.Api;

using System.Net;
using System.Net.Http.Json;
using Ticketing.Shared.Contracts.Tickets;

public sealed class TicketsApiClient
{
    private readonly HttpClient _api;

    public TicketsApiClient(HttpClient api) => _api = api;

    public async Task<IReadOnlyList<TicketResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _api.GetFromJsonAsync<List<TicketResponse>>("api/tickets", ct);
        return items ?? [];
    }

    public async Task<TicketResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _api.GetFromJsonAsync<TicketResponse>($"api/tickets/{id}", ct);

    public async Task<TicketResponse> CreateAsync(TicketCreateRequest req, CancellationToken ct = default)
    {
        var res = await _api.PostAsJsonAsync("api/tickets", req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<TicketResponse>(cancellationToken: ct))!;
    }

    public async Task<TicketResponse> UpdateAsync(Guid id, TicketUpdateRequest req, CancellationToken ct = default)
    {
        var res = await _api.PatchAsJsonAsync($"api/tickets/{id}", req, ct);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<TicketResponse>(cancellationToken: ct))!;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
        {
            return false;
        }

        using var resp = await _api.DeleteAsync($"api/tickets/{id}", ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        resp.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> AddCommentAsync(Guid ticketId, string message, CancellationToken ct = default)
    {
        var payload = new AddTicketCommentRequest(message);

        using var resp = await _api.PostAsJsonAsync($"api/tickets/{ticketId}/comments", payload, ct);

        if (resp.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        resp.EnsureSuccessStatusCode();
        return true;
    }

    public async Task<bool> CloseAsync(Guid id, CancellationToken ct = default)
    {
        if (id == Guid.Empty)
        {
            return false;
        }

        using var response = await _api.PatchAsync($"api/tickets/{id}/close", null, ct);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}
