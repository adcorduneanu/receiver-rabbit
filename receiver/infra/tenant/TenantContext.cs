namespace receiver.infra.tenant
{
    public class TenantContext : ITenantContext
    {
        private static readonly AsyncLocal<string?> _current = new();
        public string? GetTenantId() => _current.Value;
        public void SetTenantId(string? value) => _current.Value = value;
    }
}
