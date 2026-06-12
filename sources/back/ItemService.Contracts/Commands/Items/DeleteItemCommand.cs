using Franz.Common.Mediator.Messages;

namespace ItemService.Contracts.Commands.Items;

public sealed record DeleteItemCommand(
    Guid ItemId,
    string DeletedBy = "system"
) : ICommand;