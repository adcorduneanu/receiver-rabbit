namespace receiver
{
    using MassTransit;
    using System.Threading.Tasks;

    public class TenantDbSetupFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ITenantContext _tenantContext;

        public TenantDbSetupFilter(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            var tenantId = context.Headers.Get<string>("TenantId");
            _tenantContext.SetTenantId(tenantId);
            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("TenantDbSetupFilter");
        }
    }
}