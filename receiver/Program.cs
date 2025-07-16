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

                            cfg.ReceiveEndpoint("outbound_notification_queue", e =>
                            {
                                e.Consumer<OutboundNotificationConsumer>(context, consumerCfg =>
                                {
                                    consumerCfg.Message<OutboundNotification>(msg =>
                                    {
                                        msg.UsePartitioner(64, ctx =>
                                        {
                                            return ctx.Headers.Get<string>("TenantId") ?? ctx.Message.TenantId;
                                        });
                                    });
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