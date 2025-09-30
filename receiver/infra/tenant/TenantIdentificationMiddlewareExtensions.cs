namespace receiver.infra.tenant
{
    public static class TenantIdentificationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantIdentification(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantIdentificationMiddleware>();
        }
    }
}
