namespace receiver
{
    using MassTransit;

    public sealed class BusRunner : IHostedService
    {
        private readonly IBusControl bus;

        public BusRunner(IBusControl bus)
        {
            this.bus = bus;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return this.bus.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return this.bus.StopAsync(cancellationToken);
        }
    }
}