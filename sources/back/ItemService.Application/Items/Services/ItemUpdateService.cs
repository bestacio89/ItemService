using Franz.Common.Business.Repositories;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

public sealed class ItemUpdateService : IItemUpdateService
{
  private readonly IEntityRepository<ItemEntity, Guid> _items;

  public ItemUpdateService(IEntityRepository<ItemEntity, Guid> items)
  {
    _items = items;
  }

  public async Task UpdateStatsAsync(UpdateItemStatsCommand request, CancellationToken ct)
  {
    var item = await _items.GetByIdAsync(request.ItemId, ct)
        ?? throw new InvalidOperationException($"Item {request.ItemId} not found");

    item.BaseStats.Update(
        BuildHeroModifier(request),
        BuildSkillModifier(request),
        request.UpdatedBy);

    await _items.UpdateAsync(item, ct);
  }

  // =========================
  // PURE TRANSFORMATION ONLY
  // =========================

  private static ItemHeroStatModifier? BuildHeroModifier(UpdateItemStatsCommand r)
  {
    var hasAny =
        r.HealthBonus.HasValue ||
        r.ManaBonus.HasValue ||
        r.AttackDamageBonus.HasValue ||
        r.AbilityPowerBonus.HasValue ||
        r.AttackSpeedBonus.HasValue ||
        r.CastSpeedBonus.HasValue ||
        r.CritChanceBonus.HasValue ||
        r.CritDamageMultiplierBonus.HasValue ||
        r.ArmorBonus.HasValue ||
        r.MagicResistanceBonus.HasValue ||
        r.DamageReductionBonus.HasValue ||
        r.ShieldStrengthMultiplierBonus.HasValue ||
        r.MovementSpeedBonus.HasValue ||
        r.AttackRangeBonus.HasValue ||
        r.CooldownReductionBonus.HasValue ||
        r.ResourceRegenerationBonus.HasValue;

    if (!hasAny)
      return null;

    return new ItemHeroStatModifier(
        r.HealthBonus,
        r.ManaBonus,
        r.AttackDamageBonus,
        r.AbilityPowerBonus,
        r.AttackSpeedBonus,
        r.CastSpeedBonus,
        r.CritChanceBonus,
        r.CritDamageMultiplierBonus,
        r.ArmorBonus,
        r.MagicResistanceBonus,
        r.DamageReductionBonus,
        r.ShieldStrengthMultiplierBonus,
        r.MovementSpeedBonus,
        r.AttackRangeBonus,
        r.CooldownReductionBonus,
        r.ResourceRegenerationBonus);
  }

  private static ItemSkillStatModifier? BuildSkillModifier(UpdateItemStatsCommand r)
  {
    var hasAny =
        r.SkillCooldownReduction.HasValue ||
        r.ManaCostReduction.HasValue ||
        r.SkillDamageBonus.HasValue ||
        r.HealingBonus.HasValue ||
        r.ShieldValueBonus.HasValue ||
        r.CastTimeReduction.HasValue ||
        r.ChannelDurationBonus.HasValue ||
        r.CrowdControlDurationBonus.HasValue ||
        r.RangeBonus.HasValue;

    if (!hasAny)
      return null;

    return new ItemSkillStatModifier(
        r.SkillCooldownReduction,
        r.ManaCostReduction,
        r.SkillDamageBonus,
        r.HealingBonus,
        r.ShieldValueBonus,
        r.CastTimeReduction,
        r.ChannelDurationBonus,
        r.CrowdControlDurationBonus,
        r.RangeBonus);
  }
}