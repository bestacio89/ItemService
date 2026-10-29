using System.Reflection;
using ItemService.API.Controllers;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Microsoft.AspNetCore.Mvc;

namespace ItemService.Testing.QualityGates;

/// <summary>
/// Executable contracts for the domain invariants and service boundaries that exist today.
/// These tests intentionally avoid a database, Docker daemon, broker, or external service.
/// </summary>
public sealed class DomainContractTests
{
    [Fact]
    public void PatternCondition_RequiresAtLeastTwoEffects()
    {
        Assert.Throws<ArgumentException>(() => ItemBehaviouralCondition.ForPattern(Array.Empty<int>(), 4));
        Assert.Throws<ArgumentException>(() => ItemBehaviouralCondition.ForPattern(new[] { 0 }, 4));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PatternCondition_RequiresPositiveWindow(float windowSeconds) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ItemBehaviouralCondition.ForPattern(new[] { 0, 5 }, windowSeconds));

    [Fact]
    public void PatternCondition_PreservesOrderAndWindow()
    {
        var condition = ItemBehaviouralCondition.ForPattern(new[] { 0, 5, 0 }, 4.5f, 2);
        Assert.Equal(new[] { 0, 5, 0 }, condition.Pattern);
        Assert.Equal(4.5f, condition.WindowSeconds);
        Assert.Equal(2, condition.StackThreshold);
        Assert.Null(condition.ConsecutiveCount);
        Assert.Null(condition.DistinctEffectCount);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void ConsecutiveCondition_RequiresAtLeastTwoOccurrences(int count) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ItemBehaviouralCondition.ForConsecutive(0, count));

    [Fact]
    public void ConsecutiveCondition_PreservesEffectTypeAndCount()
    {
        var condition = ItemBehaviouralCondition.ForConsecutive(5, 3);
        Assert.Equal(5, condition.ConsecutiveEffectType);
        Assert.Equal(3, condition.ConsecutiveCount);
        Assert.Null(condition.Pattern);
        Assert.Null(condition.WindowSeconds);
        Assert.Null(condition.DistinctEffectCount);
    }

    [Theory]
    [InlineData(0, 5)]
    [InlineData(1, 5)]
    [InlineData(3, 0)]
    [InlineData(3, -1)]
    public void DiversityCondition_RequiresMultipleEffectsAndPositiveWindow(int count, float windowSeconds) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ItemBehaviouralCondition.ForDiversity(count, windowSeconds));

    [Fact]
    public void DiversityCondition_PreservesCountAndWindow()
    {
        var condition = ItemBehaviouralCondition.ForDiversity(4, 6, 2);
        Assert.Equal(4, condition.DistinctEffectCount);
        Assert.Equal(6, condition.WindowSeconds);
        Assert.Equal(2, condition.StackThreshold);
        Assert.Null(condition.Pattern);
        Assert.Null(condition.ConsecutiveCount);
    }

    [Fact]
    public void EquivalentConditionsHaveValueEquality()
    {
        var first = ItemBehaviouralCondition.ForPattern(new[] { 0, 5, 0 }, 4);
        var second = ItemBehaviouralCondition.ForPattern(new[] { 0, 5, 0 }, 4);
        var differentOrder = ItemBehaviouralCondition.ForPattern(new[] { 5, 0, 0 }, 4);
        Assert.Equal(first, second);
        Assert.NotEqual(first, differentOrder);
    }

    [Fact]
    public void HeroAndSkillModifiersPreserveIndependentStatLayers()
    {
        var hero = new ItemHeroStatModifier(250, 100, 55, 12, 0.15f, 0.1f, 0.2f, 0.3f,
            25, 15, 0.05f, 0.1f, 0.2f, 10, 0.15f, 1.5f);
        var skill = new ItemSkillStatModifier(0.2f, 0.1f, 40, 20, 15, 0.05f, 0.1f, 0.25f, 5);
        Assert.Equal(250, hero.HealthBonus);
        Assert.Equal(55, hero.AttackDamageBonus);
        Assert.Equal(0.15f, hero.CooldownReductionBonus);
        Assert.Equal(40, skill.DamageBonus);
        Assert.Equal(0.2f, skill.CooldownReduction);
        Assert.Equal(5, skill.RangeBonus);
    }

    [Fact]
    public void ItemEntityExposesTheExpectedDefinitionContract()
    {
        var define = typeof(ItemEntity).GetMethod(nameof(ItemEntity.Define));
        Assert.NotNull(define);
        Assert.Equal(typeof(string), define!.GetParameters().Single(p => p.Name == "name").ParameterType);
        Assert.Contains(define.GetParameters(), p => p.Name == "baseStats");
        Assert.Contains(define.GetParameters(), p => p.Name == "createdBy");
    }

    [Fact]
    public void BehaviouralConditionIsADeclarativeDescriptorNotAnEvaluator()
    {
        var methods = typeof(ItemBehaviouralCondition).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Assert.DoesNotContain(methods, method =>
            method.Name.Contains("Evaluate", StringComparison.OrdinalIgnoreCase) ||
            method.Name.Contains("Execute", StringComparison.OrdinalIgnoreCase));
        Assert.NotNull(typeof(ItemBehaviouralCondition).GetProperty(nameof(ItemBehaviouralCondition.Pattern)));
    }
}

public sealed class ApiBoundaryContractTests
{
    [Fact]
    public void ItemControllerIsAnApiControllerWithExplicitItemsRoute()
    {
        var type = typeof(ItemController);
        Assert.True(type.IsDefined(typeof(ApiControllerAttribute), inherit: true));
        Assert.Equal("items", type.GetCustomAttribute<RouteAttribute>()?.Template);
        Assert.True(typeof(ControllerBase).IsAssignableFrom(type));
    }

    [Theory]
    [InlineData(nameof(ItemController.GetAll), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.GetById), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.GetByName), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.GetByCategory), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.GetByMythology), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.GetByClass), typeof(HttpGetAttribute))]
    [InlineData(nameof(ItemController.Create), typeof(HttpPostAttribute))]
    [InlineData(nameof(ItemController.UpdateStats), typeof(HttpPutAttribute))]
    [InlineData(nameof(ItemController.Delete), typeof(HttpDeleteAttribute))]
    public void PublicEndpointsDeclareExpectedHttpVerb(string methodName, Type attributeType)
    {
        var method = typeof(ItemController).GetMethod(methodName);
        Assert.NotNull(method);
        Assert.True(method!.IsDefined(attributeType, inherit: true), $"{methodName} must declare {attributeType.Name}.");
    }

    [Fact]
    public void UpdateStatsUsesAnIdScopedRouteAndReceivesAnId()
    {
        var method = typeof(ItemController).GetMethod(nameof(ItemController.UpdateStats));
        Assert.NotNull(method);
        Assert.Equal(typeof(Guid), method!.GetParameters()[0].ParameterType);
        Assert.Contains(method.GetCustomAttributes(typeof(HttpPutAttribute), true).Cast<HttpPutAttribute>(),
            attribute => attribute.Template == "{id:guid}/stats");
    }
}
