namespace Receiver.IntegrationTests
{
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.Hosting;

    public class ReceiverWebAppFactory : WebApplicationFactory<receiver.Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
            });

            return base.CreateHost(builder);
        }
    }
}
