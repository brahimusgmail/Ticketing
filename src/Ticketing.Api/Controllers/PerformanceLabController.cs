namespace Ticketing.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Abstractions;
using Ticketing.Infrastructure.Persistence;
using Ticketing.Shared.Contracts.Tickets;
using Ticketing.Shared.Enums.Ticket;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

[ApiController]
[Route("api/performance-lab")]
public sealed class PerformanceLabController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ILogger<PerformanceLabController> _logger;

    public PerformanceLabController(
        ITicketRepository ticketRepository,
        ILogger<PerformanceLabController> logger)
    {
        _ticketRepository = ticketRepository;
        _logger = logger;
    }

    [HttpGet("tickets")]
    public async Task<ActionResult<IReadOnlyList<TicketPerformanceDto>>> GetTickets(
        [FromQuery] TicketEnums.Status status = TicketEnums.Status.New,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var tickets = await _ticketRepository.GetByStatusAsync(status, pageSize, cancellationToken);

        return Ok(tickets);
    }
}
