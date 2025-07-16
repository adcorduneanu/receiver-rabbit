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
                        });
                    });
                })
                .Build();

            await host.StartAsync();

            var routingKey = $"outbound.{tenantId}";

            var bus = host.Services.GetRequiredService<IBusControl>();
            var sendEndpoint = await bus.GetSendEndpoint(new Uri($"queue:outbound_notification_queue"));
            for (int i = 1; i <= 1_500; i++)
            {


                await sendEndpoint.Send(new OutboundNotification
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
