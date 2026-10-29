using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Commands;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Moq;

namespace ItemService.Testing.Unit.Application;

public sealed class CreateItemCommandValidatorTests
{
    [Fact]
    public async Task ValidateAsync_AcceptsAUniqueNonEmptyNameWithinTheLimit()
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ItemEntity>());
        var validator = new CreateItemCommandValidator(repository.Object);

        var result = await validator.ValidateAsync(CreateCommand("Gungnir"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_RejectsDuplicateName()
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { CreateItem("Gungnir") });
        var validator = new CreateItemCommandValidator(repository.Object);

        var result = await validator.ValidateAsync(CreateCommand("Gungnir"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateItemCommand.Name));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ValidateAsync_RejectsBlankName(string name)
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ItemEntity>());
        var validator = new CreateItemCommandValidator(repository.Object);

        var result = await validator.ValidateAsync(CreateCommand(name));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateItemCommand.Name));
    }

    [Fact]
    public async Task ValidateAsync_RejectsNameLongerThanOneHundredCharacters()
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<ItemEntity>());
        var validator = new CreateItemCommandValidator(repository.Object);

        var result = await validator.ValidateAsync(CreateCommand(new string('x', 101)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(CreateItemCommand.Name));
    }

    private static CreateItemCommand CreateCommand(string name) =>
        new(
            name,
            ItemCategory.Attack,
            ItemMythology.Norse,
            ItemClass.Warrior,
            null,
            // Hero modifier layer
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            // Skill modifier layer
            null, null, null,
            null, null, null,
            null, null, null,
            // Behavioural condition
            null, null, null, null, null);

    private static ItemEntity CreateItem(string name)
    {
        var item = (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;

        item.Define(name, ItemCategory.Attack, ItemMythology.Norse,
            ItemClass.Warrior, null, null!, null, "unit-test");

        return item;
    }
}
