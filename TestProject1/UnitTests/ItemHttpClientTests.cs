using ItemService.Client.Http.Clients;
using ItemService.Domain.Core;
using System.Net;
using System.Text;
using System.Text.Json;
using ItemService.Contracts.Commands.Items;

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

    [Fact]
    public async Task GetAllAsync_UsesTheCollectionRoute()
    {
        var handler = new RecordingHandler("[]");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.GetAllAsync(2, 10);

        Assert.Empty(result);
        Assert.Equal("/items?page=2&pageSize=10", handler.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetByClassAsync_UsesTheClassRoute()
    {
        var handler = new RecordingHandler("[]");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.GetByClassAsync(ItemClass.Mage, 1, 5);

        Assert.Empty(result);
        Assert.Equal("/items/class/Mage?page=1&pageSize=5", handler.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task GetByMythologyAsync_UsesTheMythologyRoute()
    {
        var handler = new RecordingHandler("[]");
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.GetByMythologyAsync(ItemMythology.Norse, 3, 8);

        Assert.Empty(result);
        Assert.Equal("/items/mythology/Norse?page=3&pageSize=8", handler.RequestUri!.PathAndQuery);
    }

    [Fact]
    public async Task CreateAsync_PostsToTheCollectionAndReturnsCreatedId()
    {
        var id = Guid.NewGuid();
        var handler = new RecordingHandler(JsonSerializer.Serialize(id));
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);

        var result = await client.CreateAsync(CreateCommand());

        Assert.Equal(id, result);
        Assert.Equal(HttpMethod.Post, handler.RequestMethod);
        Assert.Equal("/items", handler.RequestUri!.AbsolutePath);
        Assert.Contains("Gungnir", handler.RequestBody, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeleteAsync_UsesTheItemResourceRoute()
    {
        var handler = new RecordingHandler(string.Empty);
        using var http = CreateHttpClient(handler);
        var client = new ItemHttpClient(http);
        var id = Guid.NewGuid();

        await client.DeleteAsync(id);

        Assert.Equal(HttpMethod.Delete, handler.RequestMethod);
        Assert.Equal($"/items/{id}", handler.RequestUri!.AbsolutePath);
    }

    private static CreateItemCommand CreateCommand() =>
        new(
            "Gungnir",
            ItemCategory.Relic,
            ItemMythology.Norse,
            ItemClass.Mage,
            null,
            100, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, 25,
            null, null, null,
            null, null, null,
            new[] { 0, 5, 0 }, 4.5f, null, null, null,
            2, "unit-test");

    private static HttpClient CreateHttpClient(HttpMessageHandler handler) =>
        new(handler) { BaseAddress = new Uri("https://itemservice.test/") };

    private sealed class RecordingHandler(string responseBody) : HttpMessageHandler
    {
        public Uri? RequestUri { get; private set; }

        public HttpMethod? RequestMethod { get; private set; }

        public string? RequestBody { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestMethod = request.Method;
            RequestBody = request.Content is null
                ? null
                : await request.Content.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }
    }
}
