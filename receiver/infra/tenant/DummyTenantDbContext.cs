// receiver/TenantDbContextFactory.cs
namespace receiver.infra.tenant
{
    using System.Text.Json;

    public class DummyTenantDbContext
    {
        private string tenantId;
        private readonly TenantMessageStore tenantMessageStore;

        public DummyTenantDbContext(string tenantId, TenantMessageStore tenantMessageStore)
        {
            this.tenantId = tenantId;
            this.tenantMessageStore = tenantMessageStore;
        }

        public void AddMessage(object message)
        {
            tenantMessageStore.AddMessage(tenantId, JsonSerializer.Serialize(message));
        }
    }
}
