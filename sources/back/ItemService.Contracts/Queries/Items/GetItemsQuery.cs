using Franz.Common.Mediator.Messages;
using ItemService.Contracts.DTOs.Items;

namespace ItemService.Contracts.Queries.Items;

public sealed record GetAllItemsQuery : IQuery<IReadOnlyCollection<ItemDto>>;