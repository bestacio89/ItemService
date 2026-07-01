using Franz.Common.Business.Domain.Factories;
using Franz.Common.Business.Repositories;
using Franz.Common.Mediator.Pipelines.Core;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Persistence.Seeding;

public sealed class ItemSeeder : ISeeder
{
  public int Order => 1;

  private readonly IEntityFactory<Guid, ItemEntity> _itemFactory;
  private readonly IEntityFactory<Guid, ItemBaseStats> _statsFactory;

  private readonly IEntityRepository<ItemEntity, Guid> _items;
  private readonly IUnitOfWork _uow;

  public ItemSeeder(
      IEntityFactory<Guid, ItemEntity> itemFactory,
      IEntityFactory<Guid, ItemBaseStats> statsFactory,
      IEntityRepository<ItemEntity, Guid> items,
      IUnitOfWork uow)
  {
    _itemFactory = itemFactory;
    _statsFactory = statsFactory;
    _items = items;
    _uow = uow;
  }

  public async Task SeedAsync(CancellationToken ct)
  {
    if ((await _items.GetAllAsync(ct)).Any())
      return;

    const string system = "seed-system";

    // =========================================================
    // 1. Mjöllnir
    // =========================================================
    await CreateItem(
        "Mjöllnir",
        ItemCategory.Attack,
        ItemMythology.Norse,
        ItemClass.Warrior,
        new ItemHeroStatModifier(
            250, null,
            55, null,
            0.15f, null,
            0.10f, null,
            25, null, null, null,
            null, null,
            null, null),
        new ItemSkillStatModifier(
            null, null,
            40, null, null,
            null, null,
            0.20f,
            null),
        ItemBehaviouralCondition.ForConsecutive(0, 3),
        system,
        ct);

    // =========================================================
    // 2. Gungnir
    // =========================================================
    await CreateItem(
        "Gungnir",
        ItemCategory.Attack,
        ItemMythology.Norse,
        ItemClass.Ranger,
        new ItemHeroStatModifier(
            120, null,
            65, null,
            0.20f, null,
            0.15f, 0.20f,
            null, null, null, null,
            null, 15,
            null, null),
        new ItemSkillStatModifier(
            null, null,
            35, null, null,
            null, null,
            null,
            null),
        ItemBehaviouralCondition.ForPattern(
            new[] { 0, 5, 0 }, 4f),
        system,
        ct);

    // =========================================================
    // 3. Book of Thoth
    // =========================================================
    await CreateItem(
        "Book of Thoth",
        ItemCategory.Magic,
        ItemMythology.Egyptian,
        ItemClass.Mage,
        new ItemHeroStatModifier(
            null, 350,
            null, 85,
            null, null,
            null, null,
            null, null, null, null,
            null, null,
            0.20f, 1.5f),
        new ItemSkillStatModifier(
            0.15f, null,
            null, null, null,
            null, null,
            null,
            null),
        ItemBehaviouralCondition.ForDiversity(4, 6f),
        system,
        ct);

    // =========================================================
    // 4. Lion’s Hide
    // =========================================================
    await CreateItem(
        "Lion’s Hide",
        ItemCategory.Defence,
        ItemMythology.Greek,
        ItemClass.Tank,
        new ItemHeroStatModifier(
            400, null,
            null, null,
            null, null,
            null, null,
            40, 30, 0.05f, 0.10f,
            null, null,
            null, null),
        null,
        null,
        system,
        ct);

    // =========================================================
    // 5. Hydra Fang
    // =========================================================
    await CreateItem(
        "Hydra Fang",
        ItemCategory.Attack,
        ItemMythology.Greek,
        ItemClass.Assassin,
        new ItemHeroStatModifier(
            150, null,
            70, null,
            0.25f, null,
            0.20f, null,
            null, null, null, null,
            0.05f, null,
            null, null),
        new ItemSkillStatModifier(
            null, null,
            45, null, null,
            null, null,
            null,
            null),
        ItemBehaviouralCondition.ForConsecutive(0, 2),
        system,
        ct);

    // =========================================================
    // 6. Atlas Guard
    // =========================================================
    await CreateItem(
        "Atlas Guard",
        ItemCategory.Defence,
        ItemMythology.Greek,
        ItemClass.Tank,
        new ItemHeroStatModifier(
            500, null,
            null, null,
            null, null,
            null, null,
            60, 50, 0.10f, 0.20f,
            null, null,
            null, null),
        null,
        null,
        system,
        ct);

    // =========================================================
    // 7. Hermes Boots
    // =========================================================
    await CreateItem(
        "Hermes Boots",
        ItemCategory.Movement,
        ItemMythology.Greek,
        ItemClass.Support,
        new ItemHeroStatModifier(
            null, null,
            null, null,
            null, null,
            null, null,
            null, null, null, null,
            0.35f, null,
            null, null),
        null,
        null,
        system,
        ct);

    // =========================================================
    // 8. Anubis Codex
    // =========================================================
    await CreateItem(
        "Anubis Codex",
        ItemCategory.Magic,
        ItemMythology.Egyptian,
        ItemClass.Mage,
        new ItemHeroStatModifier(
            null, 300,
            null, 90,
            null, null,
            null, null,
            null, null, null, null,
            null, null,
            0.25f, 1.2f),
        skillMod: null,
        ItemBehaviouralCondition.ForDiversity(3, 5f),
        system,
        ct);

    // =========================================================
    // 9. Titan Core
    // =========================================================
    await CreateItem(
        "Titan Core",
        ItemCategory.Defence,
        ItemMythology.Greek,
        ItemClass.Tank,
        new ItemHeroStatModifier(
            600, null,
            40, null,
            null, null,
            null, null,
            80, 70, 0.15f, 0.25f,
            null, null,
            null, null),
        null,
        null,
        system,
        ct);

    // =========================================================
    // 10. Eclipse Blade
    // =========================================================
    await CreateItem(
        "Eclipse Blade",
        ItemCategory.Attack,
        ItemMythology.Norse,
        ItemClass.Assassin,
        new ItemHeroStatModifier(
            200, null,
            80, null,
            0.30f, null,
            0.25f, 0.30f,
            null, null, null, null,
            0.10f, 10,
            null, null),
        new ItemSkillStatModifier(
            null, null,
            50, null, null,
            null, null,
            null,
            null),
        ItemBehaviouralCondition.ForPattern(
            new[] { 0, 0, 5 }, 3.5f),
        system,
        ct);

    await _uow.CommitAsync(ct);
  }

  // =========================================================
  // FACTORY METHOD
  // =========================================================
  private async Task CreateItem(
      string name,
      ItemCategory category,
      ItemMythology mythology,
      ItemClass classAffinity,
      ItemHeroStatModifier? heroMod,
      ItemSkillStatModifier? skillMod,
      ItemBehaviouralCondition? condition,
      string system,
      CancellationToken ct)
  {
    var item = _itemFactory.Create();
    var stats = _statsFactory.Create();

    stats.Define(
        item.Id,
        heroMod,
        skillMod,
        system);

    item.Define(
        name,
        category,
        mythology,
        classAffinity,
        signatureHeroId: null,
        baseStats: stats,
        condition: condition,
        createdBy: system);

    await _items.AddAsync(item, ct);
    await _uow.CommitAsync(ct);

  }
}