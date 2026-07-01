using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Domain.Core;

public sealed record ItemHeroStatModifier(
    // CORE SURVIVABILITY
    float? HealthBonus,
    float? ManaBonus,

    // DAMAGE PROFILE
    float? AttackDamageBonus,
    float? AbilityPowerBonus,

    // TEMPO / COMBAT FLOW
    float? AttackSpeedBonus,
    float? CastSpeedBonus,

    // CRITICAL PROFILE
    float? CritChanceBonus,
    float? CritDamageMultiplierBonus,

    // DEFENSIVE PROFILE
    float? ArmorBonus,
    float? MagicResistanceBonus, 
    float? DamageReductionBonus,
    float? ShieldStrengthMultiplierBonus,

    // POSITIONING
    float? MovementSpeedBonus,
    float? AttackRangeBonus,

    // RESOURCE
    float? CooldownReductionBonus,
    float? ResourceRegenerationBonus
);