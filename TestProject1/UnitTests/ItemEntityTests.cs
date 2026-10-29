using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using System.Reflection;

namespace ItemService.Testing.Unit.Domain;

public sealed class ItemEntityTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Define_RejectsMissingName(string? name)
    {
        var item = CreateItem();

        Assert.Throws<ArgumentException>(() => item.Define(
            name!, ItemCategory.Attack, ItemMythology.Norse, ItemClass.Warrior,
            null, CreateBaseStats(), null, "test"));
    }

    [Fact]
    public void Define_PreservesTheCompleteItemDefinition()
    {
        var item = CreateItem();
        var itemId = item.Id;
        var heroId = Guid.NewGuid();
        var stats = CreateBaseStats();
        var condition = ItemBehaviouralCondition.ForConsecutive(3, 2, 1);

        item.Define("Mjolnir", ItemCategory.Attack, ItemMythology.Norse,
            ItemClass.Warrior, heroId, stats, condition, "unit-test");

        Assert.Equal(itemId, item.Id);
        Assert.Equal("Mjolnir", item.Name);
        Assert.Equal(ItemCategory.Attack, item.Category);
        Assert.Equal(ItemMythology.Norse, item.Mythology);
        Assert.Equal(ItemClass.Warrior, item.ClassAffinity);
        Assert.Equal(heroId, item.SignatureHeroId);
        Assert.Same(stats, item.BaseStats);
        Assert.Equal(condition, item.Condition);
    }

    [Fact]
    public void Define_AllowsAnItemWithoutABehaviouralCondition()
    {
        var item = CreateItem();

        item.Define("Plain Sword", ItemCategory.Attack, ItemMythology.Norse,
            ItemClass.Warrior, null, CreateBaseStats(), null, "unit-test");

        Assert.Null(item.Condition);
    }

    private static ItemEntity CreateItem() =>
        (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;

    private static ItemBaseStats CreateBaseStats() =>
        (ItemBaseStats)Activator.CreateInstance(typeof(ItemBaseStats),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;
}

public sealed class ItemBaseStatsTests
{
    [Fact]
    public void Define_RequiresAnItemId()
    {
        var stats = CreateStats();
        var hero = new ItemHeroStatModifier(1, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null);

        Assert.Throws<ArgumentException>(() => stats.Define(Guid.Empty, hero, null, "unit-test"));
    }

    [Fact]
    public void Define_RequiresAtLeastOneStatLayer()
    {
        var stats = CreateStats();

        Assert.Throws<InvalidOperationException>(() =>
            stats.Define(Guid.NewGuid(), null, null, "unit-test"));
    }

    [Fact]
    public void Define_PreservesHeroAndSkillStatLayersIndependently()
    {
        var stats = CreateStats();
        var itemId = Guid.NewGuid();
        var hero = new ItemHeroStatModifier(100, null, null, null, null, null, null, null,
            null, null, null, null, null, null, null, null);
        var skill = new ItemSkillStatModifier(null, null, 25, null, null, null, null, null, null);

        stats.Define(itemId, hero, skill, "unit-test");

        Assert.Equal(itemId, stats.ItemId);
        Assert.Same(hero, stats.HeroModifier);
        Assert.Same(skill, stats.SkillModifier);
    }

    private static ItemBaseStats CreateStats() =>
        (ItemBaseStats)Activator.CreateInstance(typeof(ItemBaseStats),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null, args: new object[] { Guid.NewGuid() }, culture: null)!;
}
