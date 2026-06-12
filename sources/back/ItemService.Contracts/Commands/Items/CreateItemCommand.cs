using Franz.Common.Mediator.Messages;
using ItemService.Domain.Core;

namespace ItemService.Contracts.Commands.Items;

public sealed record CreateItemCommand(
    string Name,
    ItemCategory Category,
    ItemMythology Mythology,
    ItemClass ClassAffinity,
    Guid? SignatureHeroId,

    // Hero stat layer — null if item does not modify hero stats
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

    // Skill stat layer — null if item does not modify skill stats
    float? SkillCooldownReduction,
    float? ManaCostReduction,
    float? SkillDamageBonus,
    float? HealingBonus,
    float? ShieldValueBonus,
    float? CastTimeReduction,
    float? ChannelDurationBonus,
    float? CrowdControlDurationBonus,
    float? RangeBonus,

    // Behavioural condition — null if item has no sequence condition
    IReadOnlyList<int>? ConditionPattern,
    float? ConditionWindowSeconds,
    int? ConditionConsecutiveEffectType,
    int? ConditionConsecutiveCount,
    int? ConditionDistinctEffectCount,
    int ConditionStackThreshold = 1,

    string CreatedBy = "system"
) : ICommand<Guid>;