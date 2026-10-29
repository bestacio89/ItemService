using System.Reflection;
using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.PostgreSql;

namespace ItemService.Testing.Integration.Persistence;

public sealed class PostgreSqlPersistenceIntegrationTests
{
    [Fact]
    public async Task ItemAndStatsPersistAndReloadWithConditionAndOwnedModifiers()
    {
        await using var postgres = new PostgreSqlBuilder()
            .WithDatabase("itemservice_integration")
            .WithUsername("itemservice")
            .WithPassword("itemservice")
            .Build();

        await postgres.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(postgres.GetConnectionString())
            .Options;

        await using var context = new ApplicationDbContext(options, Mock.Of<IDispatcher>());
        await context.Database.EnsureCreatedAsync();

        var itemId = Guid.NewGuid();
        var stats = CreateStats();
        stats.Define(itemId,
            new ItemHeroStatModifier(125, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            new ItemSkillStatModifier(null, null, 30, null, null, null, null, null, null),
            "integration-test");

        var condition = ItemBehaviouralCondition.ForPattern(new[] { 0, 5, 0 }, 4.5f, 2);
        var item = CreateItem(itemId);
        item.Define("Integration Relic", ItemCategory.Relic, ItemMythology.Norse,
            ItemClass.Mage, null, stats, condition, "integration-test");

        context.Items.Add(item);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var reloaded = await context.Items
            .Include(value => value.BaseStats)
            .SingleAsync(value => value.Id == itemId);

        Assert.Equal("Integration Relic", reloaded.Name);
        Assert.Equal(condition, reloaded.Condition);
        Assert.Equal(125, reloaded.BaseStats.HeroModifier!.HealthBonus);
        Assert.Equal(30, reloaded.BaseStats.SkillModifier!.DamageBonus);
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
