namespace receiver.infra.tenant
{
    public interface ITenantContext
    {
        string? GetTenantId();
        void SetTenantId(string? value);
    }
}
