namespace ItemService.Contracts.DTOs.Items;

/// <summary>
/// DTO mirror of ItemBehaviouralCondition.
/// Consumed by MatchService to evaluate sequences against EffectSequenceTracker.
/// EffectType values are carried as integers — MatchService maps them to its own EffectType enum.
/// </summary>
public sealed record ItemBehaviouralConditionDto(

    // Pattern-based (Gungnir-style)
    IReadOnlyList<int>? Pattern,
    float? WindowSeconds,

    // Consecutive-based (Mjöllnir-style)
    int? ConsecutiveEffectType,
    int? ConsecutiveCount,

    // Diversity-based (Book of Thoth-style)
    int? DistinctEffectCount,

    int StackThreshold
);