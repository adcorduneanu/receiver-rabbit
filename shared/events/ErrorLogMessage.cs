namespace shared.events
{
    public record ErrorLogMessage
    {
        public string Message { get; init; }

        public string Exception { get; init; }

        public DateTime Timestamp { get; init; }

        public string TenantId { get; init; }
    }
}