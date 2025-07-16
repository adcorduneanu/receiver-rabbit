namespace sender
{
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using shared;

    internal class Program
    {
        static async Task Main(string[] args)
        {

            var tenantId = args.Length > 0 ? args[0] : "default-tenant";

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.Publish<WozOutboundNotification>(p =>
                            {
                                p.ExchangeType = "topic";
                            });
                        });
                    });
                })
                .Build();

            await host.StartAsync();

            var routingKey = $"outbound.{tenantId}";

            var bus = host.Services.GetRequiredService<IBusControl>();

            for (int i = 1; i <= 1_500; i++)
            {
                await bus.Publish(new WozOutboundNotification
                {
                    TenantId = tenantId,
                    Content = $"Message {i} from {tenantId}"
                }, ctx =>
                {
                    ctx.SetRoutingKey(routingKey);
                    ctx.Headers.Set("TenantId", tenantId);
                });

                Console.WriteLine($"Published message {i} for tenant {tenantId}");
            }

            await host.StopAsync();

        }
    }
}
