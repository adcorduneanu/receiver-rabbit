// Receiver/OutboundNotificationConsumer.cs
using MassTransit;
using receiver.infra.tenant;
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

            var tenantDbContext = _factory.GetOrCreate();
            tenantDbContext.AddMessage(msg);

            Console.WriteLine($"[START] tenant {msg.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
            await Task.Delay(5000);
            Console.WriteLine($"[ END ] tenant {msg.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
        }
    }
}
