namespace Ticketing.Api.Endpoints;

using Microsoft.EntityFrameworkCore;
using Ticketing.Infrastructure.Persistence;
using System.Text.RegularExpressions;

public static class AiAgentEndpoints
{
    public static IEndpointRouteBuilder MapAiAgentEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/ai/agent");

        group.MapPost("/", AskAgent);

        return endpoints;
    }

    private sealed record AskAgentRequest(string Question);

    private static async Task<IResult> AskAgent(
        AskAgentRequest request,
        TicketingSqlDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var ticketId = ExtractGuid(request.Question);

        if (ticketId is not null)
        {
            var ticket = await dbContext.Tickets
                .AsNoTracking()
                .Where(ticket => ticket.Id == ticketId.Value)
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
                return Results.NotFound(new
                {
                    Answer = "Ticket not found.",
                    ToolUsed = "get_ticket_by_id",
                });
            }

            return Results.Ok(new
            {
                Answer = $"Ticket '{ticket.Title}' is currently {ticket.Status}.",
                ToolUsed = "get_ticket_by_id",
                Ticket = ticket,
            });
        }
        else
        {
            return null;
        }
    }

    private static Guid? ExtractGuid(string text)
    {
        var match = Regex.Match(
            text,
            @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}");

        if (!match.Success)
        {
            return null;
        }

        return Guid.Parse(match.Value);
    }
}
