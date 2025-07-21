namespace receiver.infra.tenant
{
    using System.Collections.Concurrent;

    public sealed class TenantMessageStore
    {
        private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _messages = new();

        public void AddMessage(string tenantId, string message)
        {
            var list = _messages.GetOrAdd(tenantId, _ => new ConcurrentBag<string>());

            list.Add(message);
        }

        public IReadOnlyList<string> GetMessages(string tenantId)
        {
            if (_messages.TryGetValue(tenantId, out var list))
            {
                return list.ToList();
            }

            return Array.Empty<string>();
        }
    }
}
