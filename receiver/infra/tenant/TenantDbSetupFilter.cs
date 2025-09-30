namespace receiver.infra.tenant
{
    using MassTransit;
    using System.Threading.Tasks;
    using LogContext = Serilog.Context.LogContext;
    using static shared.tenant.Headers;

    public sealed class TenantDbSetupFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ITenantContext tenantContext;

        public TenantDbSetupFilter(ITenantContext tenantContext)
        {
            this.tenantContext = tenantContext;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>(TenantId);

            this.tenantContext.SetTenantId(tenantId);
            using (LogContext.PushProperty(nameof(TenantId), tenantId))
            {
                await next.Send(context);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("TenantDbSetupFilter");
        }
    }
}