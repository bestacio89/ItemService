using Franz.Common.Mediator.Handlers;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using System.Reflection.Metadata.Ecma335;

namespace ItemService.Application.Items.Commands;

public sealed class CreateItemCommandHandler
    : ICommandHandler<CreateItemCommand, Guid>
{
  private readonly IItemCreationService _creationService;

  public  CreateItemCommandHandler(IItemCreationService creationService)
  {
    _creationService = creationService;
  }

  public async Task<Guid> Handle(
      CreateItemCommand request,
      CancellationToken cancellationToken)
  {
    var id = await _creationService.CreateAsync(request, cancellationToken);
    return id;
  }
}