using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using Franz.Common.Mediator.Results;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Persistence;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Queries;

public sealed class GetAllItemsQueryHandler
    : IQueryHandler<GetAllItemsQuery, IReadOnlyCollection<ItemDto>>
{
  private readonly IItemLookupRepository _repo;
  private readonly IFranzMapper _mapper;

  public GetAllItemsQueryHandler(
      IItemLookupRepository repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<IReadOnlyCollection<ItemDto>> Handle(
      GetAllItemsQuery request,
      CancellationToken ct)
  {
    var paged = await _repo.GetAllWithDetailsAsync(ct);

    return _mapper.Map<IReadOnlyCollection<ItemEntity?>, IReadOnlyCollection<ItemDto>>(paged);
  }
}