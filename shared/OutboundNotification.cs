namespace shared
{
    public record OutboundNotification
    {
        public string TenantId { get; init; }
        public string Content { get; init; }
    }
}
