namespace receiver
{
    public interface ITenantContext
    {
        string? GetTenantId();
        void SetTenantId(string? value);
    }

    public class TenantContext : ITenantContext
    {
        private static readonly System.Threading.AsyncLocal<string?> _current = new();
        public string? GetTenantId() => _current.Value;
        public void SetTenantId(string? value) => _current.Value = value;
    }
}
