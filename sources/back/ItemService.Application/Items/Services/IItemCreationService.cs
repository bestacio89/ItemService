using Franz.Common.DependencyInjection;
using ItemService.Contracts.Commands.Items;

namespace ItemService.Application.Items.Services;

public interface IItemCreationService
{
  Task<Guid> CreateAsync(CreateItemCommand request, CancellationToken cancellationToken);
}