using FluentValidation;
using Franz.Common.Business.Repositories;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Commands;

public sealed class CreateItemCommandValidator
    : AbstractValidator<CreateItemCommand>
{
  private readonly IEntityRepository<Item, Guid> _repo;

  public CreateItemCommandValidator(IEntityRepository<Item, Guid> repo)
  {
    _repo = repo;

    RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(100)
        .MustAsync(BeUniqueName)
        .WithMessage("An item with this name already exists.");
  }

  private async Task<bool> BeUniqueName(string name, CancellationToken ct)
  {
    var items = await _repo.GetAllAsync(ct);
    return !items.Any(i => i.Name == name);
  }
}