namespace Ticketing.Api.Endpoints;

using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure.Persistence;
using static Ticketing.Shared.Enums.Ticket.TicketEnums;

public static class AiToolsEndpoints
{
    public static IEndpointRouteBuilder MapAiToolsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai/tools");

        group.MapGet("/", GetTools);

        group.MapPost("/get-open-tickets-count", GetOpenTicketsCount);

        group.MapPost("/get-ticket-by-id", GetTicketById);

        return endpoints;
    }

    private static IResult GetTools()
    {
        var tools = new object[]
        {
            new
            {
                Name = "get_open_tickets_count",
                Description = "Returns the number of open tickets.",
                Endpoint = "POST /api/ai/tools/get-open-tickets-count",
                Parameters = new { },
            },
            new
            {
                Name = "get_ticket_by_id",
                Description = "Returns ticket details by ticket id.",
                Endpoint = "POST /api/ai/tools/get-ticket-by-id",
                Parameters = new
                {
                    Id = "guid",
                },
            },
        };

        return Results.Ok(tools);
    }

    private static async Task<IResult> GetOpenTicketsCount(
    TicketingSqlDbContext dbContext,
    CancellationToken cancellationToken)
    {
        var count = await dbContext.Tickets
            .CountAsync(ticket => ticket.Status == Status.New, cancellationToken);

        return Results.Ok(new
        {
            Count = count,
        });
    }

    private sealed record GetTicketByIdRequest(Guid Id);

    private static async Task<IResult> GetTicketById(
    GetTicketByIdRequest request,
    TicketingSqlDbContext dbContext,
    CancellationToken cancellationToken)
    {
        var ticket = await dbContext.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id == request.Id)
            .Select(ticket => new
            {
                ticket.Id,
                ticket.Title,
                ticket.Status,
                ticket.CreatedAtUtc,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (ticket is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(ticket);
    }
}
