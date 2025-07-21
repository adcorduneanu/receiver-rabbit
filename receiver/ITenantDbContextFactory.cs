// receiver/ITenantDbContextFactory.cs
namespace receiver
{
    public interface ITenantDbContextFactory
    {
        DummyTenantDbContext GetOrCreate(string? tenantId = null);
    }
}
