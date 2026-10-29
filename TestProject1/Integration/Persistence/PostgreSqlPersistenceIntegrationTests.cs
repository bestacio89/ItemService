using System.Reflection;
using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using ItemService.Persistence.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Npgsql;
using Testcontainers.PostgreSql;

namespace ItemService.Testing.Integration.Persistence;

public sealed class PostgreSqlPersistenceIntegrationTests
{
    [Fact]
    public async Task ItemAndStatsPersistAndReloadWithConditionAndOwnedModifiers()
    {
        await using var postgres = await StartPostgresAsync();
        await using var context = CreateContext(postgres.GetConnectionString());
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

    [Fact]
    public async Task DatabaseRejectsDuplicateItemNames()
    {
        await using var postgres = await StartPostgresAsync();
        await using var context = CreateContext(postgres.GetConnectionString());
        await context.Database.EnsureCreatedAsync();

        context.Items.Add(CreatePersistableItem("Duplicate Relic"));
        await context.SaveChangesAsync();

        context.Items.Add(CreatePersistableItem("Duplicate Relic"));

        var error = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());

        Assert.IsType<PostgresException>(error.InnerException);
        Assert.Equal(PostgresErrorCodes.UniqueViolation, ((PostgresException)error.InnerException!).SqlState);
    }

    [Fact]
    public async Task DeletingAnItemCascadesToItsRequiredStatsRow()
    {
        await using var postgres = await StartPostgresAsync();
        await using var context = CreateContext(postgres.GetConnectionString());
        await context.Database.EnsureCreatedAsync();

        var item = CreatePersistableItem("Cascade Relic");
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var statsId = item.BaseStats.Id;
        Assert.Equal(1, await context.Set<ItemBaseStats>().CountAsync());

        context.Items.Remove(item);
        await context.SaveChangesAsync();

        Assert.False(await context.Items.AnyAsync(value => value.Id == item.Id));
        Assert.False(await context.Set<ItemBaseStats>().AnyAsync(value => value.Id == statsId));
    }

    private static async Task<PostgreSqlContainer> StartPostgresAsync()
    {
        var postgres = new PostgreSqlBuilder()
            .WithDatabase("itemservice_integration")
            .WithUsername("itemservice")
            .WithPassword("itemservice")
            .Build();

        await postgres.StartAsync();
        return postgres;
    }

    private static ApplicationDbContext CreateContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new ApplicationDbContext(options, Mock.Of<IDispatcher>());
    }

    private static ItemEntity CreatePersistableItem(string name)
    {
        var id = Guid.NewGuid();
        var stats = CreateStats();
        stats.Define(id,
            new ItemHeroStatModifier(1, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            null,
            "integration-test");
        var item = CreateItem(id);
        item.Define(name, ItemCategory.Relic, ItemMythology.Norse,
            ItemClass.Mage, null, stats, null, "integration-test");
        return item;
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
