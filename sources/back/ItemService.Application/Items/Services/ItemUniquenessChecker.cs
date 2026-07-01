using Franz.Common.Business.Repositories;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Services;

public sealed class ItemUniquenessChecker : IItemUniquenessChecker
{
  private readonly IEntityRepository<ItemEntity, Guid> _items;

  public ItemUniquenessChecker(IEntityRepository<ItemEntity, Guid> items)
  {
    _items = items;
  }

  public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
  {
    if (string.IsNullOrWhiteSpace(name))
      return false;

    var all = await _items.GetAllAsync(ct);

    return all.Any(x => x.Name == name);
  }
}