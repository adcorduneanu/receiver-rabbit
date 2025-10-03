namespace sender
{
    using System;
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using RabbitMQ.Client;
    using shared.configs;
    using shared.events;
    using static shared.tenant.Headers;

    internal sealed class Program
    {
        static async Task Main(string[] args)
        {

            var tenantId = args.Length > 0 ? args[0] : "default-tenant";
            var numberOfMessages = args.Length > 1 ? int.Parse(args[1]) : 5;

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.ConfigureRabbitMq();

                            cfg.Publish<OutboundNotification>(p => { p.ExchangeType = ExchangeType.Topic; });
                        });
                    });
                })
                .Build();

            await host.StartAsync();
            var routingKey = tenantId;

            var bus = host.Services.GetRequiredService<IBusControl>();

            for (int i = 1; i <= numberOfMessages; i++)
            {
                await bus.Publish(new OutboundNotification
                {
                    TenantId = tenantId,
                    Content = $"Message {i} from {tenantId}"
                }, ctx =>
                {
                    ctx.SetRoutingKey(routingKey);
                    ctx.Headers.Set(TenantId, tenantId);
                });

                Console.WriteLine($"Published message {i} for tenant {tenantId}");
            }

            await host.StopAsync();
        }
    }
}
