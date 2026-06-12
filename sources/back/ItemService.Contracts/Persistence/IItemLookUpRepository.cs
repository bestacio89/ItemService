using Franz.Common.DependencyInjection;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Contracts.Persistence;

public interface IItemLookupRepository : IScopedDependency
{
  Task<PagedResult<ItemEntity>> GetByCategoryAsync(
      ItemCategory category,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<PagedResult<ItemEntity>> GetByMythologyAsync(
      ItemMythology mythology,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<PagedResult<ItemEntity>> GetByClassAffinityAsync(
      ItemClass classAffinity,
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

  Task<ItemEntity?> GetByNameAsync(
      string name,
      CancellationToken cancellationToken = default);

}