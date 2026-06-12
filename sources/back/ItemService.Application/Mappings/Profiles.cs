using Franz.Common.Mapping.Profiles;
using ItemService.Contracts.DTOs.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Mappings.Items;

public sealed class ItemProfile : FranzMapProfile
{
  public ItemProfile()
  {
    // =========================================================
    // ItemHeroStatModifier → DTO
    // =========================================================
    CreateMap<ItemHeroStatModifier, ItemHeroStatModifierDto>()
        .ConstructUsing(src => new ItemHeroStatModifierDto(
            src.HealthBonus,
            src.ManaBonus,
            src.AttackDamageBonus,
            src.AbilityPowerBonus,
            src.AttackSpeedBonus,
            src.CastSpeedBonus,
            src.CritChanceBonus,
            src.CritDamageMultiplierBonus,
            src.ArmorBonus,
            src.MagicResistanceBonus,
            src.DamageReductionBonus,
            src.ShieldStrengthMultiplierBonus,
            src.MovementSpeedBonus,
            src.AttackRangeBonus,
            src.CooldownReductionBonus,
            src.ResourceRegenerationBonus
        ));

    // =========================================================
    // ItemSkillStatModifier → DTO
    // =========================================================
    CreateMap<ItemSkillStatModifier, ItemSkillStatModifierDto>()
        .ConstructUsing(src => new ItemSkillStatModifierDto(
            src.CooldownReduction,
            src.ManaCostReduction,
            src.DamageBonus,
            src.HealingBonus,
            src.ShieldValueBonus,
            src.CastTimeReduction,
            src.ChannelDurationBonus,
            src.CrowdControlDurationBonus,
            src.RangeBonus
        ));

    // =========================================================
    // ItemBaseStats → DTO
    // =========================================================
    CreateMap<ItemBaseStats, ItemBaseStatsDto>()
        .ConstructUsing(src => new ItemBaseStatsDto(
            src.HeroModifier == null ? null : new ItemHeroStatModifierDto(
                src.HeroModifier.HealthBonus,
                src.HeroModifier.ManaBonus,
                src.HeroModifier.AttackDamageBonus,
                src.HeroModifier.AbilityPowerBonus,
                src.HeroModifier.AttackSpeedBonus,
                src.HeroModifier.CastSpeedBonus,
                src.HeroModifier.CritChanceBonus,
                src.HeroModifier.CritDamageMultiplierBonus,
                src.HeroModifier.ArmorBonus,
                src.HeroModifier.MagicResistanceBonus,
                src.HeroModifier.DamageReductionBonus,
                src.HeroModifier.ShieldStrengthMultiplierBonus,
                src.HeroModifier.MovementSpeedBonus,
                src.HeroModifier.AttackRangeBonus,
                src.HeroModifier.CooldownReductionBonus,
                src.HeroModifier.ResourceRegenerationBonus
            ),
            src.SkillModifier == null ? null : new ItemSkillStatModifierDto(
                src.SkillModifier.CooldownReduction,
                src.SkillModifier.ManaCostReduction,
                src.SkillModifier.DamageBonus,
                src.SkillModifier.HealingBonus,
                src.SkillModifier.ShieldValueBonus,
                src.SkillModifier.CastTimeReduction,
                src.SkillModifier.ChannelDurationBonus,
                src.SkillModifier.CrowdControlDurationBonus,
                src.SkillModifier.RangeBonus
            )
        ));

    // =========================================================
    // ItemBehaviouralCondition → DTO
    // =========================================================
    CreateMap<ItemBehaviouralCondition, ItemBehaviouralConditionDto>()
        .ConstructUsing(src => new ItemBehaviouralConditionDto(
            src.Pattern,
            src.WindowSeconds,
            src.ConsecutiveEffectType,
            src.ConsecutiveCount,
            src.DistinctEffectCount,
            src.StackThreshold
        ));

    // =========================================================
    // Item → DTO
    // Franz ConstructUsing — single src, all nested inline.
    // =========================================================
    CreateMap<ItemEntity, ItemDto>()
        .ConstructUsing(src => new ItemDto(
            src.Id,
            src.Name,
            src.Category,
            src.Mythology,
            src.ClassAffinity,
            src.SignatureHeroId,
            new ItemBaseStatsDto(
                src.BaseStats.HeroModifier == null ? null : new ItemHeroStatModifierDto(
                    src.BaseStats.HeroModifier.HealthBonus,
                    src.BaseStats.HeroModifier.ManaBonus,
                    src.BaseStats.HeroModifier.AttackDamageBonus,
                    src.BaseStats.HeroModifier.AbilityPowerBonus,
                    src.BaseStats.HeroModifier.AttackSpeedBonus,
                    src.BaseStats.HeroModifier.CastSpeedBonus,
                    src.BaseStats.HeroModifier.CritChanceBonus,
                    src.BaseStats.HeroModifier.CritDamageMultiplierBonus,
                    src.BaseStats.HeroModifier.ArmorBonus,
                    src.BaseStats.HeroModifier.MagicResistanceBonus,
                    src.BaseStats.HeroModifier.DamageReductionBonus,
                    src.BaseStats.HeroModifier.ShieldStrengthMultiplierBonus,
                    src.BaseStats.HeroModifier.MovementSpeedBonus,
                    src.BaseStats.HeroModifier.AttackRangeBonus,
                    src.BaseStats.HeroModifier.CooldownReductionBonus,
                    src.BaseStats.HeroModifier.ResourceRegenerationBonus
                ),
                src.BaseStats.SkillModifier == null ? null : new ItemSkillStatModifierDto(
                    src.BaseStats.SkillModifier.CooldownReduction,
                    src.BaseStats.SkillModifier.ManaCostReduction,
                    src.BaseStats.SkillModifier.DamageBonus,
                    src.BaseStats.SkillModifier.HealingBonus,
                    src.BaseStats.SkillModifier.ShieldValueBonus,
                    src.BaseStats.SkillModifier.CastTimeReduction,
                    src.BaseStats.SkillModifier.ChannelDurationBonus,
                    src.BaseStats.SkillModifier.CrowdControlDurationBonus,
                    src.BaseStats.SkillModifier.RangeBonus
                )
            ),
            src.Condition == null ? null : new ItemBehaviouralConditionDto(
                src.Condition.Pattern,
                src.Condition.WindowSeconds,
                src.Condition.ConsecutiveEffectType,
                src.Condition.ConsecutiveCount,
                src.Condition.DistinctEffectCount,
                src.Condition.StackThreshold
            )
        ));
  }
}