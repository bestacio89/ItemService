using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;

public sealed class ItemDeletionService : IItemDeletionService
{
  private readonly IEntityRepository<ItemEntity, Guid> _items;

  public ItemDeletionService(IEntityRepository<ItemEntity, Guid> items)
  {
    _items = items;
  }

  public async Task DeleteAsync(DeleteItemCommand request, CancellationToken ct)
  {
    var item = await _items.GetByIdAsync(request.ItemId, ct)
        ?? throw new InvalidOperationException($"Item {request.ItemId} not found");

    await _items.DeleteAsync(item,false);
  }
}