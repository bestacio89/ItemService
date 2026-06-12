using ItemService.Contracts.Commands.Items;
using ItemService.Contracts.DTOs.Items;
using ItemService.Domain.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Client.Http.Abstractions;

public interface IItemHttpClient
{
  Task<IReadOnlyList<ItemDto>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);

  Task<IReadOnlyList<ItemDto>> GetByCategoryAsync(ItemCategory category, int page, int pageSize, CancellationToken ct = default);

  Task<IReadOnlyList<ItemDto>> GetByClassAsync(ItemClass classAffinity, int page, int pageSize, CancellationToken ct = default);

  Task<IReadOnlyList<ItemDto>> GetByMythologyAsync(ItemMythology mythology, int page, int pageSize, CancellationToken ct = default);

  Task<ItemDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

  Task<ItemDto?> GetByNameAsync(string name, CancellationToken ct = default);

  Task<Guid> CreateAsync(CreateItemCommand command, CancellationToken ct = default);

  Task DeleteAsync(Guid id, CancellationToken ct = default);
}