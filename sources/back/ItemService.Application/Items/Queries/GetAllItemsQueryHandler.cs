using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Queries;

public sealed class GetAllItemsQueryHandler
    : IQueryHandler<GetAllItemsQuery, IReadOnlyList<ItemDto>>
{
  private readonly IEntityRepository<Item, Guid> _repo;
  private readonly IFranzMapper _mapper;

  public GetAllItemsQueryHandler(
      IEntityRepository<Item, Guid> repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<IReadOnlyList<ItemDto>> Handle(
      GetAllItemsQuery request,
      CancellationToken ct)
  {
    var items = await _repo.GetAllAsync(ct);

    return items
        .Select(i => _mapper.Map<Item, ItemDto>(i))
        .ToList();
  }
}