// Receiver/WozConsumer.cs
using MassTransit;
using shared;

namespace receiver
{
    public class WozConsumer : IConsumer<WozOutboundNotification>
    {
        public async Task Consume(ConsumeContext<WozOutboundNotification> context)
        {
            var msg = context.Message;

            Console.WriteLine($"[START] {msg.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
            await Task.Delay(5000); // simulate work
            Console.WriteLine($"[END]   {msg.TenantId}: {msg.Content} at {DateTime.UtcNow:O}");
        }
    }
}
