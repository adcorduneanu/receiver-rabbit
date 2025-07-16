// Receiver/Program.cs
namespace receiver
{
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using shared;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<OutboundNotificationConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.Publish<OutboundNotification>(p => { p.ExchangeType = "topic"; });

                            new List<string> { "test1", "test2", "test3", "test4", "demo" }
                                .ForEach(tenantId =>
                                {
                                    var routingKey = $"outbound.{tenantId}";

                                    cfg.ReceiveEndpoint(routingKey, e =>
                                    {
                                        e.ConfigureConsumeTopology = false;

                                        e.PrefetchCount = 1;
                                        e.ConcurrentMessageLimit = 1;

                                        e.Bind("shared:OutboundNotification", b =>
                                        {
                                            b.ExchangeType = "topic";
                                            b.RoutingKey = routingKey;
                                        });

                                        e.ConfigureConsumer<OutboundNotificationConsumer>(context);
                                    });
                                });
                        });
                    });

                    services.AddHostedService<BusRunner>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}