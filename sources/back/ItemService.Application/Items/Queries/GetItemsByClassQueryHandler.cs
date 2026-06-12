using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Persistence;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

public sealed class GetItemsByClassQueryHandler
    : IQueryHandler<GetItemsByClassQuery, IReadOnlyList<ItemDto>>
{
  private readonly IItemLookupRepository _repo;
  private readonly IFranzMapper _mapper;

  public GetItemsByClassQueryHandler(
      IItemLookupRepository repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<ItemDto>> Handle(
      GetItemsByClassQuery request,
      CancellationToken ct)
  {
    var paged = await _repo.GetByClassAffinityAsync(
        request.ClassAffinity,
        request.Page,
        request.PageSize,
        ct);

    return _mapper.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(paged.Items);
  }
}