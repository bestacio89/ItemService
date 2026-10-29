using ItemService.Domain.Core;

namespace ItemService.Testing.Unit.Domain;

public sealed class ItemBehaviouralConditionMapperTests
{
    [Fact]
    public void PatternCondition_RoundTripsWithoutLosingOrderOrThreshold()
    {
        var original = ItemBehaviouralCondition.ForPattern(new[] { 0, 5, 0 }, 4.5f, 2);

        var data = ItemBehaviouralConditionMapper.ToData(original);
        var restored = ItemBehaviouralConditionMapper.ToDomain(data);

        Assert.Equal(new[] { 0, 5, 0 }, data.Pattern);
        Assert.Equal(4.5f, data.WindowSeconds);
        Assert.Equal(2, data.StackThreshold);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void ConsecutiveCondition_RoundTripsWithoutLosingCount()
    {
        var original = ItemBehaviouralCondition.ForConsecutive(7, 3, 4);

        var restored = ItemBehaviouralConditionMapper.ToDomain(
            ItemBehaviouralConditionMapper.ToData(original));

        Assert.Equal(original, restored);
        Assert.Equal(7, restored.ConsecutiveEffectType);
        Assert.Equal(3, restored.ConsecutiveCount);
        Assert.Equal(4, restored.StackThreshold);
    }

    [Fact]
    public void DiversityCondition_RoundTripsWithoutLosingWindow()
    {
        var original = ItemBehaviouralCondition.ForDiversity(4, 6.25f, 2);

        var restored = ItemBehaviouralConditionMapper.ToDomain(
            ItemBehaviouralConditionMapper.ToData(original));

        Assert.Equal(original, restored);
        Assert.Equal(4, restored.DistinctEffectCount);
        Assert.Equal(6.25f, restored.WindowSeconds);
        Assert.Equal(2, restored.StackThreshold);
    }

    [Fact]
    public void ToData_CopiesPatternInsteadOfSharingMutableCollection()
    {
        var originalPattern = new List<int> { 1, 2, 3 };
        var condition = ItemBehaviouralCondition.ForPattern(originalPattern, 5);

        var data = ItemBehaviouralConditionMapper.ToData(condition);

        originalPattern[0] = 99;

        Assert.Equal(new[] { 1, 2, 3 }, data.Pattern);
    }

    [Fact]
    public void ToDomain_RejectsPayloadWithoutAConditionKind()
    {
        var data = new ItemBehaviouralConditionData { StackThreshold = 1 };

        Assert.Throws<InvalidOperationException>(() =>
            ItemBehaviouralConditionMapper.ToDomain(data));
    }

    [Fact]
    public void ToDomain_RejectsMalformedPatternPayload()
    {
        var data = new ItemBehaviouralConditionData
        {
            Pattern = new List<int> { 1 },
            WindowSeconds = 3,
            StackThreshold = 1
        };

        Assert.Throws<ArgumentException>(() => ItemBehaviouralConditionMapper.ToDomain(data));
    }

    [Fact]
    public void ToDomain_RejectsMalformedConsecutivePayload()
    {
        var data = new ItemBehaviouralConditionData
        {
            ConsecutiveEffectType = 2,
            ConsecutiveCount = 1,
            StackThreshold = 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => ItemBehaviouralConditionMapper.ToDomain(data));
    }

    [Fact]
    public void ToDomain_RejectsMalformedDiversityPayload()
    {
        var data = new ItemBehaviouralConditionData
        {
            DistinctEffectCount = 1,
            WindowSeconds = 3,
            StackThreshold = 1
        };

        Assert.Throws<ArgumentOutOfRangeException>(() => ItemBehaviouralConditionMapper.ToDomain(data));
    }
}
