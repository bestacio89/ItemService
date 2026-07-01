

namespace ItemService.Domain.Core;

public static class ItemBehaviouralConditionMapper
{
  public static ItemBehaviouralCondition ToDomain(ItemBehaviouralConditionData data)
  {
    if (data.Pattern is not null)
      return ItemBehaviouralCondition.ForPattern(
        data.Pattern,
        data.WindowSeconds!.Value,
        data.StackThreshold);

    if (data.ConsecutiveEffectType.HasValue)
      return ItemBehaviouralCondition.ForConsecutive(
        data.ConsecutiveEffectType.Value,
        data.ConsecutiveCount!.Value,
        data.StackThreshold);

    if (data.DistinctEffectCount.HasValue)
      return ItemBehaviouralCondition.ForDiversity(
        data.DistinctEffectCount.Value,
        data.WindowSeconds!.Value,
        data.StackThreshold);

    throw new InvalidOperationException("Invalid condition payload");
  }

  public static ItemBehaviouralConditionData ToData(ItemBehaviouralCondition domain)
  {
    return new ItemBehaviouralConditionData
    {
      Pattern = domain.Pattern?.ToList(),
      WindowSeconds = domain.WindowSeconds,
      ConsecutiveEffectType = domain.ConsecutiveEffectType,
      ConsecutiveCount = domain.ConsecutiveCount,
      DistinctEffectCount = domain.DistinctEffectCount,
      StackThreshold = domain.StackThreshold
    };
  }
}