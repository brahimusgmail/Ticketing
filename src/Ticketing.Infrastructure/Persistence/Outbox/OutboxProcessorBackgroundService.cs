namespace Ticketing.Infrastructure.Persistence.Outbox;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;

    public OutboxProcessorBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessorBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessOutboxMessagesAsync(stoppingToken);

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Arrêt demandé, sortir proprement de la boucle
                break;
            }
        }
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TicketingSqlDbContext>();

        var messages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedAtUtc == null)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(10)
            .ToListAsync(stoppingToken);

        foreach (var message in messages)
        {
            try
            {
                _logger.LogInformation("Processing OutboxMessage {Id} of type {Type} with payload: {Payload}", message.Id, message.Type, message.Payload);

                // Simuler le traitement ici

                message.ProcessedAtUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                _logger.LogError(ex, "Error processing OutboxMessage {Id}", message.Id);
            }
        }

        if (messages.Count > 0)
        {
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
