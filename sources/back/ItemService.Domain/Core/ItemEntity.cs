using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;

namespace ItemService.Domain.Items.Core;

public sealed class ItemEntity : Entity<Guid>
{
  public string Name { get; private set; } = null!;

  public ItemCategory Category { get; private set; }

  public ItemMythology Mythology { get; private set; }

  public ItemClass ClassAffinity { get; private set; }

  public Guid? SignatureHeroId { get; private set; }

  public ItemBaseStats BaseStats { get; private set; } = null!;

  public ItemBehaviouralCondition? Condition { get; private set; }

  protected ItemEntity(Guid id) : base(id) { }
 

  public void Define(
      string name,
      ItemCategory category,
      ItemMythology mythology,
      ItemClass classAffinity,
      Guid? signatureHeroId,
      ItemBaseStats baseStats,
      ItemBehaviouralCondition? condition,
      string createdBy)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException("Item name is required.");

    Name = name;
    Category = category;
    Mythology = mythology;
    ClassAffinity = classAffinity;
    SignatureHeroId = signatureHeroId;
    BaseStats = baseStats;
    Condition = condition;

    MarkCreated(createdBy);
  }


}