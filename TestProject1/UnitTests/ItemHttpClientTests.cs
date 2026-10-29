using ItemService.Client.Http.Clients;
using ItemService.Domain.Core;
using System.Net;
using System.Text;

namespace ItemService.Testing.Unit.ClientHttp;

public sealed class ItemHttpClientTests
{
    [Fact]
    public async Task GetByIdAsync_UsesTheItemsControllerRoute()
    {
        var handler = new RecordingHandler("null");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);
        var id = Guid.NewGuid();

        var result = await client.GetByIdAsync(id);

        Assert.Null(result);
        Assert.Equal($"/items/{id}", handler.RequestUri!.AbsolutePath);
    }

    [Fact]
    public async Task GetByNameAsync_UsesTheByNameRouteAndEscapesTheName()
    {
        var handler = new RecordingHandler("null");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.GetByNameAsync("Gungnir Spear");

        Assert.Null(result);
        Assert.Equal("/items/by-name/Gungnir%20Spear", handler.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetByCategoryAsync_UsesTheCategoryRouteAndPagingQuery()
    {
        var handler = new RecordingHandler("[]");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.GetByCategoryAsync(ItemCategory.Relic, 2, 10);

        Assert.Empty(result);
        Assert.Equal("/items/category/Relic?page=2&pageSize=10", handler.RequestUri!.PathAndQuery);
    }

    private static HttpClient CreateHttpClient(HttpMessageHandler handler) =>
        new(handler) { BaseAddress = new Uri("https://itemservice.test/") };

    private sealed class RecordingHandler(string responseBody) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            });
        }
    }
}
