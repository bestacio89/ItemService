using Franz.Common.Mediator.Messages;

namespace ItemService.Contracts.Commands.Items;

/// <summary>
/// Directly updates an item's stats.
/// No versioning — changes take effect immediately for all future matches.
/// </summary>
public sealed record UpdateItemStatsCommand(
    Guid ItemId,

    // Hero stat layer
    float? HealthBonus,
    float? ManaBonus,
    float? AttackDamageBonus,
    float? AbilityPowerBonus,
    float? AttackSpeedBonus,
    float? CastSpeedBonus,
    float? CritChanceBonus,
    float? CritDamageMultiplierBonus,
    float? ArmorBonus,
    float? MagicResistanceBonus,
    float? DamageReductionBonus,
    float? ShieldStrengthMultiplierBonus,
    float? MovementSpeedBonus,
    float? AttackRangeBonus,
    float? CooldownReductionBonus,
    float? ResourceRegenerationBonus,

    // Skill stat layer
    float? SkillCooldownReduction,
    float? ManaCostReduction,
    float? SkillDamageBonus,
    float? HealingBonus,
    float? ShieldValueBonus,
    float? CastTimeReduction,
    float? ChannelDurationBonus,
    float? CrowdControlDurationBonus,
    float? RangeBonus,

    string UpdatedBy = "system"
) : ICommand;