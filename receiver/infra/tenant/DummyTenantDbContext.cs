namespace receiver.infra.tenant
{
    using System.Text.Json;

    public sealed class DummyTenantDbContext
    {
        private readonly string tenantId;
        private readonly TenantMessageStore tenantMessageStore;

        public DummyTenantDbContext(string tenantId, TenantMessageStore tenantMessageStore)
        {
            this.tenantId = tenantId;
            this.tenantMessageStore = tenantMessageStore;
        }

        public void AddMessage(object message)
        {
            this.tenantMessageStore.AddMessage(this.tenantId, JsonSerializer.Serialize(message));
        }
    }
}
