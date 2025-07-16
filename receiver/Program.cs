// Receiver/Program.cs
namespace receiver
{
    using MassTransit;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using RabbitMQ.Client;

    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool sendTenantAsHeader = true;
            bool useRouterKey = true;

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<WozConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("localhost", "/", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ReceiveEndpoint("outbound-all-tenants", e =>
                            {
                                //e.ConfigureConsumeTopology = false;

                                e.Bind("shared:WozOutboundNotification", s =>
                                {
                                    s.RoutingKey = "outbound.*";
                                    //s.ExchangeType = ExchangeType.Topic;
                                });

                                e.ConfigureConsumer<WozConsumer>(context);
                            });
                            /*
                            if (useRouterKey)
                            {

                                cfg.ReceiveEndpoint("outbound-all-tenants", e =>
                                {

                                    e.Bind("outbound-topic-exchange", x =>
                                    {
                                        x.ExchangeType = "topic";
                                        x.RoutingKey = "outbound.*";
                                    });

                                    e.ConfigureConsumer<WozConsumer>(context);
                                });
                            }
                            else
                            {
                                cfg.ReceiveEndpoint("woz_outbound_queue", e =>
                                {
                                    e.Consumer<WozConsumer>(context, consumerCfg =>
                                    {
                                        consumerCfg.Message<WozOutboundNotification>(msg =>
                                        {
                                            msg.UsePartitioner(64, ctx =>
                                            {
                                                if (sendTenantAsHeader)
                                                {
                                                    return ctx.Headers.Get<string>("TenantId") ?? "unknown";
                                                }

                                                return ctx.Message.TenantId;
                                            });
                                        });
                                    });
                                });
                            }*/
                        });
                    });

                    services.AddHostedService<BusRunner>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}