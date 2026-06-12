using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using Franz.Common.Mediator.Handlers;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Queries;

public sealed class GetItemByIdQueryHandler
    : IQueryHandler<GetItemByIdQuery, ItemDto>
{
  private readonly IEntityRepository<ItemEntity, Guid> _repo;
  private readonly IFranzMapper _mapper;

  public GetItemByIdQueryHandler(
      IEntityRepository<ItemEntity, Guid> repo,
      IFranzMapper mapper)
  {
    _repo = repo;
    _mapper = mapper;
  }

  public async Task<ItemDto> Handle(
      GetItemByIdQuery request,
      CancellationToken ct)
  {
    var item = await _repo.GetByIdAsync(request.ItemId, ct)
        ?? throw new InvalidOperationException($"Item {request.ItemId} not found");

    return _mapper.Map<ItemEntity, ItemDto>(item);
  }
}