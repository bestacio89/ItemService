namespace ItemService.Contracts.DTOs.Items;

/// <summary>
/// Full stat profile of an item.
/// Both modifiers are carried in full — MatchService needs both
/// to apply hero and skill layer modifications without extra round trips.
/// </summary>
public sealed record ItemBaseStatsDto(
    ItemHeroStatModifierDto? HeroModifier,
    ItemSkillStatModifierDto? SkillModifier
);