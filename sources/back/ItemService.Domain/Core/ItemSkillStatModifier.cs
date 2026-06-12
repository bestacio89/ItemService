using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Domain.Core;

// Modifies skill-level stats (cooldown, damage, range etc.)
public sealed record ItemSkillStatModifier(
    // CORE COSTS
    float? CooldownReduction,
    float? ManaCostReduction,

    // BASE OUTPUT
    float? DamageBonus,
    float? HealingBonus,
    float? ShieldValueBonus,

    // CASTING TEMPO
    float? CastTimeReduction,
    float? ChannelDurationBonus,

    // UTILITY
    float? CrowdControlDurationBonus,
    float? RangeBonus
);
