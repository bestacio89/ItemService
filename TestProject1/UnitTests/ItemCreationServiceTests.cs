using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Moq;
using System.Reflection;

namespace ItemService.Testing.Unit.Application;

public sealed class ItemCreationServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsDuplicateNameBeforeCreatingEntities()
    {
        var itemFactory = new Mock<IEntityFactory<Guid, ItemEntity>>(MockBehavior.Strict);
        var statsFactory = new Mock<IEntityFactory<Guid, ItemBaseStats>>(MockBehavior.Strict);
        var items = new Mock<IEntityRepository<ItemEntity, Guid>>(MockBehavior.Strict);
        var stats = new Mock<IEntityRepository<ItemBaseStats, Guid>>(MockBehavior.Strict);
        var uniqueness = new Mock<IItemUniquenessChecker>();
        uniqueness.Setup(value => value.ExistsByNameAsync("Gungnir", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        var service = new ItemCreationService(itemFactory.Object, statsFactory.Object,
            items.Object, stats.Object, uniqueness.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CreateAsync(CreateCommand("Gungnir"), CancellationToken.None));

        itemFactory.VerifyNoOtherCalls();
        statsFactory.VerifyNoOtherCalls();
        items.VerifyNoOtherCalls();
        stats.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAsync_BuildsAndPersistsBothStatLayersAndCondition()
    {
        var item = CreateItem();
        var baseStats = CreateStats();
        var itemFactory = new Mock<IEntityFactory<Guid, ItemEntity>>();
        itemFactory.Setup(value => value.Create()).Returns(item);
        var statsFactory = new Mock<IEntityFactory<Guid, ItemBaseStats>>();
        statsFactory.Setup(value => value.Create()).Returns(baseStats);
        var items = new Mock<IEntityRepository<ItemEntity, Guid>>();
        items.Setup(value => value.AddAsync(item, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var stats = new Mock<IEntityRepository<ItemBaseStats, Guid>>();
        stats.Setup(value => value.AddAsync(baseStats, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var uniqueness = new Mock<IItemUniquenessChecker>();
        uniqueness.Setup(value => value.ExistsByNameAsync("Gungnir", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var service = new ItemCreationService(itemFactory.Object, statsFactory.Object,
            items.Object, stats.Object, uniqueness.Object);

        var id = await service.CreateAsync(CreateCommand("Gungnir"), CancellationToken.None);

        Assert.Equal(item.Id, id);
        Assert.Equal("Gungnir", item.Name);
        Assert.Equal(100, item.BaseStats.HeroModifier!.HealthBonus);
        Assert.Equal(25, item.BaseStats.SkillModifier!.DamageBonus);
        Assert.Equal(new[] { 0, 5, 0 }, item.Condition!.Pattern);
        Assert.Equal(4.5f, item.Condition.WindowSeconds);
        Assert.Equal(2, item.Condition.StackThreshold);
        items.Verify(value => value.AddAsync(item, It.IsAny<CancellationToken>()), Times.Once);
        stats.Verify(value => value.AddAsync(baseStats, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateItemCommand CreateCommand(string name) =>
        new(
            name,
            ItemCategory.Relic,
            ItemMythology.Norse,
            ItemClass.Mage,
            null,
            // Hero modifier layer
            100, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            // Skill modifier layer
            null, null, 25,
            null, null, null,
            null, null, null,
            // Behavioural condition
            new[] { 0, 5, 0 }, 4.5f, null, null, null,
            2, "unit-test");

    private static ItemEntity CreateItem() =>
        (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;

    private static ItemBaseStats CreateStats() =>
        (ItemBaseStats)Activator.CreateInstance(typeof(ItemBaseStats),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;
}
