using Franz.Common.Business.Repositories;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;
using ItemService.Domain.Core;

namespace ItemService.Application.Items.Services;

public interface IItemUpdateService
{
  Task UpdateStatsAsync(UpdateItemStatsCommand request, CancellationToken ct);
}