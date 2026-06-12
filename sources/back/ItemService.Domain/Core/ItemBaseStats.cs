using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Domain.Core;

public sealed class ItemBaseStats : Entity<Guid>
{
  public Guid ItemId { get; private set; }

  // Either or both can be non-null
  // An attack item might only touch HeroStatModifier
  // A cooldown relic might only touch SkillStatModifier
  // A legendary might touch both
  public ItemHeroStatModifier? HeroModifier { get; private set; }
  public ItemSkillStatModifier? SkillModifier { get; private set; }

  private ItemBaseStats() { }

  public void Define(
      Guid itemId,
      ItemHeroStatModifier? heroModifier,
      ItemSkillStatModifier? skillModifier,
      string createdBy)
  {
    if (itemId == Guid.Empty)
      throw new ArgumentException("ItemId is required.");

    if (heroModifier is null && skillModifier is null)
      throw new InvalidOperationException(
          "An item must modify at least one stat layer.");

    ItemId = itemId;
    HeroModifier = heroModifier;
    SkillModifier = skillModifier;

    MarkCreated(createdBy);
  }

  public void Update(
  ItemHeroStatModifier? heroModifier,
  ItemSkillStatModifier? skillModifier,
  string updatedBy)
  {
    HeroModifier = heroModifier;
    SkillModifier = skillModifier;

    MarkUpdated(updatedBy);
  }
}