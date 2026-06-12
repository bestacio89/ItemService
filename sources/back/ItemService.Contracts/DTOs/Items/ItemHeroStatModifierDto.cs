namespace ItemService.Contracts.DTOs.Items;

/// <summary>
/// Flat additive bonuses applied to hero-level stats at match resolution.
/// Null values are not applied.
/// </summary>
public sealed record ItemHeroStatModifierDto(
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
    float? ResourceRegenerationBonus
);