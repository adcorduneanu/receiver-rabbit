// Receiver/BusRunner.cs
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace receiver
{
    public class BusRunner : IHostedService
    {
        private readonly IBusControl _bus;

        public BusRunner(IBusControl bus) => _bus = bus;

        public Task StartAsync(CancellationToken cancellationToken) => _bus.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken) => _bus.StopAsync(cancellationToken);
    }
}