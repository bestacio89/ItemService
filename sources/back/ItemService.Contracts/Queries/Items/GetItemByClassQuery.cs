using Franz.Common.Mediator.Messages;
using ItemService.Contracts.DTOs.Items;
using ItemService.Domain.Core;

namespace ItemService.Contracts.Queries.Items;

public sealed record GetItemsByClassQuery(
    ItemClass ClassAffinity,
    int Page = 1,
    int PageSize = 20
) : IQuery<IReadOnlyList<ItemDto>>;