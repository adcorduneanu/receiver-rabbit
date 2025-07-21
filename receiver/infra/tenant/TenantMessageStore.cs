// Receiver/TenantMessageStore.cs
namespace receiver.infra.tenant
{
    using shared;
    using System.Collections.Concurrent;

    public class TenantMessageStore
    {
        private readonly ConcurrentDictionary<string, List<string>> _messages = new();

        public void AddMessage(string tenantId, string message)
        {
            var list = _messages.GetOrAdd(tenantId, _ => new List<string>());
            lock (list)
            {
                list.Add(message);
            }
        }

        public IReadOnlyList<string> GetMessages(string tenantId)
        {
            if (_messages.TryGetValue(tenantId, out var list))
            {
                lock (list)
                {
                    return list.ToList();
                }
            }
            return Array.Empty<string>();
        }
    }
}
