using ItemService.Client.Http.Clients;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using System.Net;
using System.Text;

namespace ItemService.Testing.Unit.ClientHttp;

public sealed class ItemHttpClientFailureTests
{
    [Fact]
    public async Task CreateAsync_ThrowsWhenTheServiceRejectsTheRequest()
    {
        using var http = CreateHttpClient(HttpStatusCode.Conflict, string.Empty);
        var client = new ItemHttpClient(http);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.CreateAsync(CreateCommand()));
    }

    [Fact]
    public async Task DeleteAsync_ThrowsWhenTheServiceRejectsTheRequest()
    {
        using var http = CreateHttpClient(HttpStatusCode.NotFound, string.Empty);
        var client = new ItemHttpClient(http);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.DeleteAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetByIdAsync_PropagatesMalformedJsonInsteadOfReturningAnEmptyItem()
    {
        using var http = CreateHttpClient(HttpStatusCode.OK, "{not-json");
        var client = new ItemHttpClient(http);

        await Assert.ThrowsAnyAsync<System.Text.Json.JsonException>(() => client.GetByIdAsync(Guid.NewGuid()));
    }

    private static HttpClient CreateHttpClient(HttpStatusCode statusCode, string body) =>
        new(new StaticResponseHandler(statusCode, body))
        {
            BaseAddress = new Uri("https://itemservice.test/")
        };

    private static CreateItemCommand CreateCommand() =>
        new(
            "Gungnir",
            ItemCategory.Relic,
            ItemMythology.Norse,
            ItemClass.Mage,
            null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null,
            null, null, null,
            null, null, null,
            null, null, null, null, null);

    private sealed class StaticResponseHandler(HttpStatusCode statusCode, string body) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            });
    }
}
