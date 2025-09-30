namespace receiver.infra.tenant
{
    using Serilog.Context;
    using static shared.tenant.Headers;

    public sealed class TenantIdentificationMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantIdentificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITenantContext tenantContext)
        {
            var tenantId = context.Request.Headers[TenantId].FirstOrDefault();

            if (string.IsNullOrEmpty(tenantId))
            {
                await _next(context);
                return;
            }

            tenantContext.SetTenantId(tenantId);
            using (LogContext.PushProperty(nameof(TenantId), tenantId))
            {
                await _next(context);
            }
        }
    }
}
