using Franz.Common.Business.Repositories;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Moq;
using System.Reflection;

namespace ItemService.Testing.Unit.Application;

public sealed class ItemDeletionServiceTests
{
    [Fact]
    public async Task DeleteAsync_DeletesTheResolvedItem()
    {
        var id = Guid.NewGuid();
        var item = CreateItem(id);
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        repository.Setup(value => value.DeleteAsync(item, false)).Returns(Task.CompletedTask);
        var service = new global::ItemDeletionService(repository.Object);

        await service.DeleteAsync(new DeleteItemCommand(id), CancellationToken.None);

        repository.Verify(value => value.DeleteAsync(item, false), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsWhenTheItemDoesNotExist()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEntity?)null);
        var service = new global::ItemDeletionService(repository.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.DeleteAsync(new DeleteItemCommand(id), CancellationToken.None));

        repository.Verify(value => value.DeleteAsync(It.IsAny<ItemEntity>(), It.IsAny<bool>()), Times.Never);
    }

    private static ItemEntity CreateItem(Guid id) =>
        (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { id }, culture: null)!;
}

public sealed class ItemUpdateServiceTests
{
    [Fact]
    public async Task UpdateStatsAsync_ReplacesHeroAndSkillModifierLayers()
    {
        var id = Guid.NewGuid();
        var item = CreateItem(id);
        var stats = CreateStats();
        stats.Define(id,
            new ItemHeroStatModifier(100, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null),
            new ItemSkillStatModifier(null, null, 25, null, null, null, null, null, null),
            "unit-test");
        item.Define("Gungnir", ItemCategory.Relic, ItemMythology.Norse,
            ItemClass.Mage, null, stats, null, "unit-test");

        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        repository.Setup(value => value.UpdateAsync(item, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new global::ItemUpdateService(repository.Object);
        var command = new UpdateItemStatsCommand(
            id,
            // Hero modifier layer
            250, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            // Skill modifier layer
            null, null, null,
            null, null, null,
            null, null, null,
            "unit-test");

        await service.UpdateStatsAsync(command, CancellationToken.None);

        Assert.Equal(250, item.BaseStats.HeroModifier!.HealthBonus);
        Assert.Null(item.BaseStats.SkillModifier);
        repository.Verify(value => value.UpdateAsync(item, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateStatsAsync_ThrowsWhenTheItemDoesNotExist()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEntity?)null);
        var service = new global::ItemUpdateService(repository.Object);
        var command = new UpdateItemStatsCommand(
            id,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null,
            null, null, null,
            null, null, null);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.UpdateStatsAsync(command, CancellationToken.None));

        repository.Verify(value => value.UpdateAsync(It.IsAny<ItemEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static ItemEntity CreateItem(Guid id) =>
        (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { id }, culture: null)!;

    private static ItemBaseStats CreateStats() =>
        (ItemBaseStats)Activator.CreateInstance(typeof(ItemBaseStats),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;
}
