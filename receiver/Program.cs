namespace receiver
{
    using Azure.Monitor.OpenTelemetry.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Trace;
    using receiver.extensions;
    using receiver.infra.logging;
    using receiver.infra.tenant;
    using Serilog;
    using Serilog.Events;
    using shared.configs;
    using static shared.tenant.Headers;

    internal static class Program
    {
        static async Task Main(string[] args)
        {
            await DropAndCreateRabbitMqVHostAndPermissions();

            var appBuilder = WebApplication.CreateBuilder();

            var azureMonitorConnectionString = appBuilder.Configuration["ApplicationInsightsConfig:ConnectionString"];

            appBuilder.Services.AddSingleton<MassTransitErrorSink>();

            appBuilder.Services.AddSingleton<TenantMessageStore>();
            appBuilder.Services.AddScoped<ITenantContext, TenantContext>();
            appBuilder.Services.AddScoped<ITenantDbContextFactory, TenantDbContextFactory>();
            appBuilder.Services.AddScoped<OutboundNotificationConsumer>();

            appBuilder.Services.AddMassTransitWithRabbitMq();

            appBuilder.Services.AddOpenTelemetry()
                        .WithTracing(tracer => tracer
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddSource("MassTransit")
                        )
                        .WithMetrics(metrics => metrics
                            .AddAspNetCoreInstrumentation()
                            .AddHttpClientInstrumentation()
                            .AddMeter("MassTransit"))
                        .UseAzureMonitor(o => { o.ConnectionString = azureMonitorConnectionString; });

            appBuilder.Logging.ClearProviders();

            appBuilder.Host.UseSerilog((context, services, loggerConfiguration) =>
                       {
                           loggerConfiguration
                               .Enrich.FromLogContext()
                               .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [TenantId:{TenantId}] {Message:lj}{NewLine}{Exception}")
                               .WriteTo.Sink(new MassTransitErrorSink(services), new Serilog.Configuration.BatchingOptions { BatchSizeLimit = 10, BufferingTimeLimit = TimeSpan.FromSeconds(1) }, LogEventLevel.Error);
                       }, writeToProviders: true);

            //appBuilder.Services.AddHostedService<BusRunner>();

            var app = appBuilder.Build();

            app.UseTenantIdentification();

            app.MapGet("/messages/", ([FromHeader(Name = TenantId)] string tenantId, TenantMessageStore store) =>
            {
                var messages = store.GetMessages(tenantId);
                return Results.Ok(messages);
            });

            await app.RunAsync();
        }

        private static async Task DropAndCreateRabbitMqVHostAndPermissions()
        {
            using var client = new HttpClient();
            var byteArray = System.Text.Encoding.ASCII.GetBytes($"{RabbitMqConfig.UserName}:{RabbitMqConfig.Password}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            var _ = await client.DeleteAsync($"{RabbitMqConfig.ManagementApiUrl}/vhosts/{RabbitMqConfig.VirtualHost}");

            var createResponse = await client.PutAsync($"{RabbitMqConfig.ManagementApiUrl}/vhosts/{RabbitMqConfig.VirtualHost}", null);
            if (!createResponse.IsSuccessStatusCode && createResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Failed to create vhost: {createResponse.StatusCode}");
            }

            var permissionsJson = "{\"configure\":\".*\",\"write\":\".*\",\"read\":\".*\"}";
            var content = new StringContent(permissionsJson, System.Text.Encoding.UTF8, "application/json");
            var permResponse = await client.PutAsync($"{RabbitMqConfig.ManagementApiUrl}/permissions/{RabbitMqConfig.VirtualHost}/{RabbitMqConfig.UserName}", content);
            if (!permResponse.IsSuccessStatusCode && permResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Failed to set permissions: {permResponse.StatusCode}");
            }

            var topicPermissionsJson = $"{{\"vhost\":\"{RabbitMqConfig.VirtualHost}\",\"username\":\"{RabbitMqConfig.UserName}\",\"exchange\":\"\",\"write\":\".*\",\"read\":\".*\"}}";
            var topicContent = new StringContent(topicPermissionsJson, System.Text.Encoding.UTF8, "application/json");
            var topicPermResponse = await client.PutAsync($"{RabbitMqConfig.ManagementApiUrl}/topic-permissions/{RabbitMqConfig.VirtualHost}/{RabbitMqConfig.UserName}", topicContent);
            if (!topicPermResponse.IsSuccessStatusCode && topicPermResponse.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine($"Failed to set topic permissions: {topicPermResponse.StatusCode}");
            }

            Console.WriteLine("RabbitMQ vhost and permissions setup completed.");
            await Task.Delay(5000);
        }
    }
}