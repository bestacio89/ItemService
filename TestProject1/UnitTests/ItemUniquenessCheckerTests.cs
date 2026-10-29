using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Services;
using ItemService.Domain.Items.Core;
using Moq;

namespace ItemService.Testing.Unit.Application;

public sealed class ItemUniquenessCheckerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExistsByNameAsync_ReturnsFalseForBlankNamesWithoutReadingRepository(string? name)
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>(MockBehavior.Strict);
        var checker = new ItemUniquenessChecker(repository.Object);

        var result = await checker.ExistsByNameAsync(name!, CancellationToken.None);

        Assert.False(result);
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ExistsByNameAsync_ReturnsTrueWhenAnExactNameExists()
    {
        var existing = CreateItem("Gungnir");
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { existing });
        var checker = new ItemUniquenessChecker(repository.Object);

        var result = await checker.ExistsByNameAsync("Gungnir", CancellationToken.None);

        Assert.True(result);
        repository.Verify(value => value.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExistsByNameAsync_ReturnsFalseWhenNameDoesNotExist()
    {
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { CreateItem("Gungnir") });
        var checker = new ItemUniquenessChecker(repository.Object);

        var result = await checker.ExistsByNameAsync("Mjolnir", CancellationToken.None);

        Assert.False(result);
    }

    private static ItemEntity CreateItem(string name)
    {
        var item = (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;

        item.Define(name, ItemService.Domain.Core.ItemCategory.Attack,
            ItemService.Domain.Core.ItemMythology.Norse,
            ItemService.Domain.Core.ItemClass.Warrior,
            null, null!, null, "unit-test");

        return item;
    }
}
