using Franz.Common.Mediator.Handlers;
using Franz.Common.Mapping.Abstractions;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Persistence;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

public sealed class GetItemsByCategoryQueryHandler
    : IQueryHandler<GetItemsByCategoryQuery, IReadOnlyList<ItemDto>>
{
  private readonly IItemLookupRepository _repo;
  private readonly IFranzMapper _mapper;

  public GetItemsByCategoryQueryHandler(
      IItemLookupRepository repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<ItemDto>> Handle(
      GetItemsByCategoryQuery request,
      CancellationToken ct)
  {
    var paged = await _repo.GetByCategoryAsync(
        request.Category,
        request.Page,
        request.PageSize,
        ct);

    return _mapper.Map<IReadOnlyList<Item>, IReadOnlyList<ItemDto>>(paged.Items);
  }
}