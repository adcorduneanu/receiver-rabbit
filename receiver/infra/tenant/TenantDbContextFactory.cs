namespace receiver.infra.tenant
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public sealed class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ConcurrentDictionary<string, DummyTenantDbContext> contexts = new();
        private static readonly HashSet<string> allowedTenants = new(StringComparer.OrdinalIgnoreCase)
        {
            "test1", "test2", "test3", "test4"
        };

        private readonly ITenantContext tenantContext;
        private readonly TenantMessageStore tenantMessageStore;

        public TenantDbContextFactory(ITenantContext tenantContext, TenantMessageStore tenantMessageStore)
        {
            this.tenantContext = tenantContext;
            this.tenantMessageStore = tenantMessageStore;
        }

        public DummyTenantDbContext GetOrCreate(string? tenantId = null)
        {
            tenantId ??= this.tenantContext.GetTenantId();

            if (string.IsNullOrWhiteSpace(tenantId) || !allowedTenants.Contains(tenantId))
                throw new InvalidOperationException($"Unknown or missing tenant: '{tenantId}'");

            return this.contexts.GetOrAdd(tenantId, id => new DummyTenantDbContext(id, this.tenantMessageStore));
        }
    }
}
