using System.Reflection;
using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using ItemService.Persistence.Persistence.Repositories;
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

        var repository = new ItemLookupRepository(context);
        var byName = await repository.GetByNameAsync("Integration Relic");
        var byId = await repository.GetByIdWithDetailsAsync(itemId);
        var byCategory = await repository.GetByCategoryAsync(ItemCategory.Relic, page: 1, pageSize: 1);
        var byMythology = await repository.GetByMythologyAsync(ItemMythology.Norse, page: 0, pageSize: 0);
        var byClass = await repository.GetByClassAffinityAsync(ItemClass.Mage, page: 1, pageSize: 10);
        var all = await repository.GetAllWithDetailsAsync();

        Assert.Equal(itemId, byName!.Id);
        Assert.Equal(itemId, byId!.Id);
        Assert.Equal(1, byCategory.TotalCount);
        Assert.Equal(1, byCategory.Page);
        Assert.Equal(1, byCategory.PageSize);
        Assert.Equal(1, byMythology.TotalCount);
        Assert.Equal(1, byMythology.Page);
        Assert.Equal(20, byMythology.PageSize);
        Assert.Equal(1, byClass.TotalCount);
        Assert.Single(all);
        Assert.Equal(itemId, Assert.Single(all)!.Id);
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
