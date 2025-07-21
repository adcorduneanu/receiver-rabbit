// receiver/TenantDbContextFactory.cs
namespace receiver.infra.tenant
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly ConcurrentDictionary<string, DummyTenantDbContext> _contexts = new();
        private static readonly HashSet<string> _allowedTenants = new(StringComparer.OrdinalIgnoreCase)
        {
            "test1", "test2", "test3", "test4"
        };

        private readonly ITenantContext _tenantContext;
        private readonly TenantMessageStore tenantMessageStore;

        public TenantDbContextFactory(ITenantContext tenantContext, TenantMessageStore tenantMessageStore)
        {
            _tenantContext = tenantContext;
            this.tenantMessageStore = tenantMessageStore;
        }

        public DummyTenantDbContext GetOrCreate(string? tenantId = null)
        {
            tenantId ??= _tenantContext.GetTenantId();

            if (string.IsNullOrWhiteSpace(tenantId) || !_allowedTenants.Contains(tenantId))
                throw new InvalidOperationException($"Unknown or missing tenant: '{tenantId}'");

            return _contexts.GetOrAdd(tenantId, id => new DummyTenantDbContext(id, tenantMessageStore));
        }
    }
}
