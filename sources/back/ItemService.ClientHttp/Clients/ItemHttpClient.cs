using ItemService.Client.Http.Abstractions;
using ItemService.Contracts.Commands.Items;
using ItemService.Contracts.DTOs.Items;
using ItemService.Domain.Core;
using System.Net.Http.Json;

namespace ItemService.Client.Http.Clients;

public sealed class ItemHttpClient : IItemHttpClient
{
  private readonly HttpClient _http;

  public ItemHttpClient(HttpClient http)
  {
    _http = http;
  }

  public async Task<IReadOnlyList<ItemDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<IReadOnlyList<ItemDto>>(
      $"items?page={page}&pageSize={pageSize}", ct
    ) ?? [];
  }

  public async Task<IReadOnlyList<ItemDto>> GetByCategoryAsync(ItemCategory category, int page, int pageSize, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<IReadOnlyList<ItemDto>>(
      $"items/category/{category}?page={page}&pageSize={pageSize}", ct
    ) ?? [];
  }

  public async Task<IReadOnlyList<ItemDto>> GetByClassAsync(ItemClass classAffinity, int page, int pageSize, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<IReadOnlyList<ItemDto>>(
      $"items/class/{classAffinity}?page={page}&pageSize={pageSize}", ct
    ) ?? [];
  }

  public async Task<IReadOnlyList<ItemDto>> GetByMythologyAsync(ItemMythology mythology, int page, int pageSize, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<IReadOnlyList<ItemDto>>(
      $"items/mythology/{mythology}?page={page}&pageSize={pageSize}", ct
    ) ?? [];
  }

  public async Task<ItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<ItemDto>($"items/{id}", ct);
  }

  public async Task<ItemDto?> GetByNameAsync(string name, CancellationToken ct = default)
  {
    return await _http.GetFromJsonAsync<ItemDto>($"items/by-name/{Uri.EscapeDataString(name)}", ct);
  }

  public async Task<Guid> CreateAsync(CreateItemCommand command, CancellationToken ct = default)
  {
    var response = await _http.PostAsJsonAsync("items", command, ct);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: ct);
  }

  public async Task DeleteAsync(Guid id, CancellationToken ct = default)
  {
    var response = await _http.DeleteAsync($"items/{id}", ct);
    response.EnsureSuccessStatusCode();
  }
}