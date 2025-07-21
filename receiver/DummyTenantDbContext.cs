// receiver/TenantDbContextFactory.cs
namespace receiver
{
    using System.Collections.Generic;

    public class DummyTenantDbContext
    {
        public string TenantId { get; }
        public List<string> Messages { get; } = new();

        public DummyTenantDbContext(string tenantId)
        {
            TenantId = tenantId;
        }

        public void AddMessage(string message)
        {
            Messages.Add(message);
        }
    }
}
