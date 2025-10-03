using MassTransit;
using RabbitMQ.Client;
using receiver.infra.tenant;
using shared.configs;
using shared.events;

namespace receiver.extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configure)
        {
            configure(appBuilder.Services);

            return appBuilder;
        }

        public static IServiceCollection AddMassTransitWithRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<OutboundNotificationConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureRabbitMq();

                    cfg.UseConsumeFilter(typeof(TenantDbSetupFilter<>), context);

                    cfg.Publish<OutboundNotification>(p => { p.ExchangeType = ExchangeType.Topic; });
                    cfg.Publish<ErrorLogMessage>(p => { p.ExchangeType = ExchangeType.Topic; });

                    new List<string> { "test1", "test2", "test3", "test4", "demo" }
                        .ForEach(tenantId =>
                        {
                            var routingKey = tenantId;
                            var queueName = $"{RabbitMqConfig.VirtualHost}_OutboundNotification_{tenantId}";

                            cfg.ReceiveEndpoint(queueName, e =>
                            {
                                e.ConfigureConsumeTopology = false;

                                e.PrefetchCount = 1;
                                e.ConcurrentMessageLimit = 1;

                                e.Bind<OutboundNotification>(b =>
                                {
                                    b.ExchangeType = ExchangeType.Topic;
                                    b.RoutingKey = routingKey;
                                });

                                e.ConfigureConsumer<OutboundNotificationConsumer>(context);
                            });
                        });
                });
            });

            return services;
        }
    }
}
