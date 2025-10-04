namespace Receiver.IntegrationTests
{
    using System.Net.Http.Json;
    using receiver.infra.tenant;
    using Microsoft.Extensions.DependencyInjection;
    using shared.events;
    using Xunit;
    using Shouldly;

    public sealed class MessagesEndpointTests : IClassFixture<ReceiverWebAppFactory>
    {
        private readonly ReceiverWebAppFactory _factory;

        public MessagesEndpointTests(ReceiverWebAppFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetMessages_ReturnsEmpty_ThenReflectsAddedMessage()
        {
            var client = _factory.CreateClient();
            var tenantId = "test1";
            client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId);

            var initial = await client.GetFromJsonAsync<List<OutboundNotification>>("/messages/");
            initial.ShouldNotBeNull();
            initial!.ShouldBeEmpty();

            using var scope = _factory.Services.CreateScope();
            var store = scope.ServiceProvider.GetRequiredService<TenantMessageStore>();
            store.AddMessage(tenantId, "hello");

            var after = await client.GetFromJsonAsync<List<OutboundNotification>>("/messages/");
            after.ShouldNotBeNull();
            after!.Count.ShouldBe(1);
            after![0].Content.ShouldBe("hello");
        }
    }
}
