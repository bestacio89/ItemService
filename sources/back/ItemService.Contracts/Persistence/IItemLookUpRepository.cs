using Franz.Common.DependencyInjection;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Contracts.Persistence;

public interface IItemLookupRepository : IScopedDependency
{
  Task<PagedResult<Item>> GetByCategoryAsync(
      ItemCategory category,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<PagedResult<Item>> GetByMythologyAsync(
      ItemMythology mythology,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<PagedResult<Item>> GetByClassAffinityAsync(
      ItemClass classAffinity,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<Item?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

}