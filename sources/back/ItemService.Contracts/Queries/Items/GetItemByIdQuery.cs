using Franz.Common.Mediator.Messages;
using ItemService.Contracts.DTOs.Items;

namespace ItemService.Contracts.Queries.Items;

public sealed record GetItemByIdQuery(Guid ItemId) : IQuery<ItemDto>;