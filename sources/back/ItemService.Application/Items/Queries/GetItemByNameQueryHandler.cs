using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Persistence;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Queries;

public sealed class GetItemByNameQueryHandler
    : IQueryHandler<GetItemByNameQuery, ItemDto>
{
  private readonly IItemLookupRepository _repo;
  private readonly IFranzMapper _mapper;

  public GetItemByNameQueryHandler(
      IItemLookupRepository repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<ItemDto> Handle(
      GetItemByNameQuery request,
      CancellationToken ct)
  {
    var item = await _repo.GetByNameAsync(request.Name, ct)
        ?? throw new InvalidOperationException($"Item '{request.Name}' not found");

    return _mapper.Map<Item, ItemDto>(item);
  }
}