using Franz.Common.Business.Repositories;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Services;

public interface IItemDeletionService
{
  Task DeleteAsync(DeleteItemCommand request, CancellationToken ct);
}