using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Application.Items.Services;

public sealed class ItemCreationService : IItemCreationService
{
  private readonly IEntityFactory<Guid, ItemEntity> _itemFactory;
  private readonly IEntityFactory<Guid, ItemBaseStats> _statsFactory;

  private readonly IEntityRepository<ItemEntity, Guid> _items;
  private readonly IEntityRepository<ItemBaseStats, Guid> _stats;

  private readonly IItemUniquenessChecker _uniquenessChecker;

  public ItemCreationService(
      IEntityFactory<Guid, ItemEntity> itemFactory,
      IEntityFactory<Guid, ItemBaseStats> statsFactory,
      IEntityRepository<ItemEntity, Guid> items,
      IEntityRepository<ItemBaseStats, Guid> stats,
      IItemUniquenessChecker uniquenessChecker)
  {
    _itemFactory = itemFactory;
    _statsFactory = statsFactory;
    _items = items;
    _stats = stats;
    _uniquenessChecker = uniquenessChecker;
  }

  public async Task<Guid> CreateAsync(CreateItemCommand request, CancellationToken ct)
  {
    // =========================================================
    // DOMAIN INVARIANT CHECK (Name uniqueness)
    // =========================================================
    if (await _uniquenessChecker.ExistsByNameAsync(request.Name, ct))
      throw new InvalidOperationException(
          $"An item with name '{request.Name}' already exists.");

    var item = _itemFactory.Create();

    var heroModifier = BuildHeroModifier(request);
    var skillModifier = BuildSkillModifier(request);
    var condition = BuildCondition(request);

    var baseStats = _statsFactory.Create();

    baseStats.Define(
        item.Id,
        heroModifier,
        skillModifier,
        request.CreatedBy);

    item.Define(
        request.Name,
        request.Category,
        request.Mythology,
        request.ClassAffinity,
        request.SignatureHeroId,
        baseStats,
        condition,
        request.CreatedBy);

    await _items.AddAsync(item, ct);
    await _stats.AddAsync(baseStats, ct);

    return item.Id;
  }

  // =========================================================
  // DOMAIN ASSEMBLY (NOT MAPPING)
  // =========================================================

  private static ItemHeroStatModifier? BuildHeroModifier(CreateItemCommand r)
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

    if (!hasAny) return null;

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

  private static ItemSkillStatModifier? BuildSkillModifier(CreateItemCommand r)
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

    if (!hasAny) return null;

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

  private static ItemBehaviouralCondition? BuildCondition(CreateItemCommand r)
  {
    if (r.ConditionPattern?.Count > 1)
      return ItemBehaviouralCondition.ForPattern(
          r.ConditionPattern,
          r.ConditionWindowSeconds ?? 3f,
          r.ConditionStackThreshold);

    if (r.ConditionConsecutiveEffectType.HasValue &&
        r.ConditionConsecutiveCount.HasValue)
      return ItemBehaviouralCondition.ForConsecutive(
          r.ConditionConsecutiveEffectType.Value,
          r.ConditionConsecutiveCount.Value,
          r.ConditionStackThreshold);

    if (r.ConditionDistinctEffectCount.HasValue)
      return ItemBehaviouralCondition.ForDiversity(
          r.ConditionDistinctEffectCount.Value,
          r.ConditionWindowSeconds ?? 3f,
          r.ConditionStackThreshold);

    return null;
  }
}