// Receiver/OutboundNotificationConsumer.cs
using MassTransit;
using shared;

namespace receiver
{
    public class OutboundNotificationConsumer : IConsumer<OutboundNotification>
    {
        private readonly ITenantDbContextFactory _factory;

        public OutboundNotificationConsumer(ITenantDbContextFactory factory)
        {
            _factory = factory;
        }

        public async Task Consume(ConsumeContext<OutboundNotification> context)
        {
            var msg = context.Message;
            // Now resolves using the current tenant from the accessor if not provided
            var tenantDbContext = _factory.GetOrCreate();
            Console.WriteLine($"[START] msg tenant {msg.TenantId} / db tenant {tenantDbContext.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
            await Task.Delay(5000); // simulate work
            Console.WriteLine($"[END] msg tenant {msg.TenantId} / db tenant {tenantDbContext.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
        }
    }
}
