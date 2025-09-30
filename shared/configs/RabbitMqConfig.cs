using MassTransit;

namespace shared.configs
{
    public static class RabbitMqConfig
    {
        public const string Host = "localhost";
        public const int Port = 5672;
        public const string UserName = "guest";
        public const string Password = "guest";
        public const string VirtualHost = "D_A";

        public static string ManagementApiUrl => $"http://{Host}:15672/api";

        public static void ConfigureRabbitMq(this IRabbitMqBusFactoryConfigurator cfg)
        {
            cfg.Host(Host, VirtualHost, h =>
            {
                h.Username(UserName);
                h.Password(Password);
            });
        }
    }
}
