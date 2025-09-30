namespace receiver.infra.logging;

using MassTransit;
using Serilog.Core;
using Serilog.Events;
using shared.events;
using static shared.tenant.Headers;

public sealed class MassTransitErrorSink : IBatchedLogEventSink
{
    private IBus _bus;
    private readonly IServiceProvider serviceProvider;

    public MassTransitErrorSink(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public async Task EmitBatchAsync(IReadOnlyCollection<LogEvent> batch)
    {
        _bus ??= serviceProvider.GetRequiredService<IBus>();

        List<Task> tasks = new();
        foreach (var logEvent in batch)
        {
            var errorMessage = new ErrorLogMessage
            {
                Message = logEvent.MessageTemplate.Text,
                Exception = logEvent.Exception?.ToString(),
                Timestamp = logEvent.Timestamp.UtcDateTime,
                TenantId = logEvent.Properties.TryGetValue(TenantId, out var tenantIdValue) ? tenantIdValue?.ToString()?.Trim('"') : null
            };

            tasks.Add(_bus.Publish(errorMessage, ctx =>
            {
                if (!string.IsNullOrEmpty(errorMessage.TenantId))
                {
                    ctx.Headers.Set(TenantId, errorMessage.TenantId);
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}