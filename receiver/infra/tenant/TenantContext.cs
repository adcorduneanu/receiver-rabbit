namespace receiver.infra.tenant
{
    public sealed class TenantContext : ITenantContext
    {
        private static readonly AsyncLocal<string?> current = new();

        public string? GetTenantId() => current.Value;

        public void SetTenantId(string? value) => current.Value = value;
    }
}
