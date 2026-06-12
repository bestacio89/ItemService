using Franz.Common.Mediator.Handlers;
using ItemService.Contracts.Commands.Items;
using ItemService.Application.Items.Services;

namespace ItemService.Application.Items.Commands;

public sealed class UpdateItemStatsCommandHandler
    : ICommandHandler<UpdateItemStatsCommand>
{
  private readonly IItemUpdateService _service;

  public UpdateItemStatsCommandHandler(IItemUpdateService service)
  {
    _service = service;
  }

  public Task Handle(UpdateItemStatsCommand request, CancellationToken ct)
      => _service.UpdateStatsAsync(request, ct);
}