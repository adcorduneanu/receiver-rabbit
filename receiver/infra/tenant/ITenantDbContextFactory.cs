namespace receiver.infra.tenant
{
    public interface ITenantDbContextFactory
    {
        DummyTenantDbContext GetOrCreate(string? tenantId = null);
    }
}
