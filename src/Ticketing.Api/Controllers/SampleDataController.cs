using Common.SampleDataGenerator.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Shared.Contracts.AI;

[ApiController]
[Route("api/sample-data")]
public class SampleDataController : ControllerBase
{
    private readonly ISampleDataGenerator _sampleDataGenerator;

    public SampleDataController(
        ISampleDataGenerator sampleDataGenerator)
    {
        _sampleDataGenerator = sampleDataGenerator;
    }

    [HttpGet]
    public async Task<IActionResult> Generate(CancellationToken cancellationToken)
    {
        var schema = new
        {
            title = "string",
            description = "string",
            categoryName = "string",
        };

        var tickets = await _sampleDataGenerator.GenerateAsync<AiTicketSeedDto>(
            schema,
            "Generate realistic support tickets for a ticketing application",
            recordCount: 10,
            cancellationToken);

        return Ok(tickets);
    }
}
