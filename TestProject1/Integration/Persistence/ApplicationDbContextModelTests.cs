using Franz.Common.Mediator.Dispatchers;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ItemService.Testing.Integration.Persistence;

public sealed class ApplicationDbContextModelTests
{
    [Fact]
    public void ItemName_IsRequiredAndLimitedToTheDomainContract()
    {
        using var context = CreateContext();
        var item = context.Model.FindEntityType(typeof(ItemEntity))!;
        var name = item.FindProperty(nameof(ItemEntity.Name))!;

        Assert.True(name.IsNullable is false);
        Assert.Equal(150, name.GetMaxLength());
    }

    [Fact]
    public void ItemName_HasAUniqueIndexToEnforceIdentityAtTheDatabaseBoundary()
    {
        using var context = CreateContext();
        var item = context.Model.FindEntityType(typeof(ItemEntity))!;
        var index = Assert.Single(item.GetIndexes(), candidate =>
            candidate.Properties.Any(property => property.Name == nameof(ItemEntity.Name)));

        Assert.True(index.IsUnique);
    }

    [Fact]
    public void ItemAndBaseStats_HaveRequiredOneToOneCascadeRelationship()
    {
        using var context = CreateContext();
        var item = context.Model.FindEntityType(typeof(ItemEntity))!;
        var navigation = item.FindNavigation(nameof(ItemEntity.BaseStats))!;

        Assert.True(navigation.ForeignKey.IsRequired);
        Assert.Equal(DeleteBehavior.Cascade, navigation.ForeignKey.DeleteBehavior);
        Assert.Equal(typeof(ItemService.Domain.Core.ItemBaseStats),
            navigation.ForeignKey.DeclaringEntityType.ClrType);
    }

    [Fact]
    public void BaseStats_ItemIdHasUniqueIndex()
    {
        using var context = CreateContext();
        var stats = context.Model.FindEntityType(typeof(ItemService.Domain.Core.ItemBaseStats))!;
        var index = Assert.Single(stats.GetIndexes(), candidate =>
            candidate.Properties.Any(property => property.Name == "ItemId"));

        Assert.True(index.IsUnique);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql("Host=localhost;Database=itemservice_model_tests;Username=test;Password=test")
            .Options;

        return new ApplicationDbContext(options, Mock.Of<IDispatcher>());
    }
}
