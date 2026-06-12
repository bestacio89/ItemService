using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;

public sealed class ItemDeletionService : IItemDeletionService
{
  private readonly IEntityRepository<Item, Guid> _items;

  public ItemDeletionService(IEntityRepository<Item, Guid> items)
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