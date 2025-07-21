namespace receiver.extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder appBuilder, Action<IServiceCollection> configure)
        {
            configure(appBuilder.Services);

            return appBuilder;
        }
    }
}
