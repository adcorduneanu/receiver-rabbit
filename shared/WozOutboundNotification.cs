namespace shared
{
    public record WozOutboundNotification
    {
        public string TenantId { get; init; }
        public string Content { get; init; }
    }
}
