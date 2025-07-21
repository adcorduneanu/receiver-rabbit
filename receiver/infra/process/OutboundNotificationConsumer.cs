namespace receiver
{
    using MassTransit;
    using receiver.infra.tenant;
    using shared;

    public sealed class OutboundNotificationConsumer : IConsumer<OutboundNotification>
    {
        private readonly ITenantContext tenantContext;
        private readonly ITenantDbContextFactory tenantDbContextFactory;

        public OutboundNotificationConsumer(ITenantContext tenantContext, ITenantDbContextFactory tenantDbContextFactory)
        {
            this.tenantContext = tenantContext;
            this.tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task Consume(ConsumeContext<OutboundNotification> context)
        {
            var tenantId = this.tenantContext.GetTenantId();
            this.tenantDbContextFactory.GetOrCreate().AddMessage(context.Message);

            var msg = context.Message;
            Console.WriteLine($"[START] tenant {tenantId}: {msg.Content} at {DateTime.UtcNow:O}");
            await Task.Delay(5000);
            Console.WriteLine($"[ END ] tenant {tenantId}: {msg.Content} at {DateTime.UtcNow:O}");
        }
    }
}
