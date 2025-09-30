namespace receiver
{
    using MassTransit;
    using receiver.infra.tenant;
    using shared.events;

    public sealed class OutboundNotificationConsumer : IConsumer<OutboundNotification>
    {
        private readonly ILogger logger;
        private readonly ITenantContext tenantContext;
        private readonly ITenantDbContextFactory tenantDbContextFactory;

        public OutboundNotificationConsumer(ILogger<OutboundNotificationConsumer> logger, ITenantContext tenantContext, ITenantDbContextFactory tenantDbContextFactory)
        {
            this.logger = logger;
            this.tenantContext = tenantContext;
            this.tenantDbContextFactory = tenantDbContextFactory;
        }

        public async Task Consume(ConsumeContext<OutboundNotification> context)
        {
            var tenantId = this.tenantContext.GetTenantId();
            this.tenantDbContextFactory.GetOrCreate().AddMessage(context.Message);

            var msg = context.Message;
            logger.LogInformation($"[START] tenant {tenantId}: {msg.Content} at {DateTime.UtcNow:O}");
            await Task.Delay(5000);
            logger.LogInformation($"[ END ] tenant {tenantId}: {msg.Content} at {DateTime.UtcNow:O}");
        }
    }
}
