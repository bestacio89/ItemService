using Franz.Common.Mediator.Handlers;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;

public sealed class DeleteItemCommandHandler
    : ICommandHandler<DeleteItemCommand>
{
  private readonly IItemDeletionService _service;

  public DeleteItemCommandHandler(IItemDeletionService service)
  {
    _service = service;
  }

  public Task Handle(DeleteItemCommand request, CancellationToken ct)
      => _service.DeleteAsync(request, ct);
}