namespace ItemService.Contracts.DTOs.Items;

/// <summary>
/// Flat additive bonuses applied to skill-level stats at match resolution.
/// Applied across all skills in the hero's kit.
/// Null values are not applied.
/// </summary>
public sealed record ItemSkillStatModifierDto(
    float? CooldownReduction,
    float? ManaCostReduction,
    float? DamageBonus,
    float? HealingBonus,
    float? ShieldValueBonus,
    float? CastTimeReduction,
    float? ChannelDurationBonus,
    float? CrowdControlDurationBonus,
    float? RangeBonus
);