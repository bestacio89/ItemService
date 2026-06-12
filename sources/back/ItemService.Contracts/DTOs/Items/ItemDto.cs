using ItemService.Domain.Core;

namespace ItemService.Contracts.DTOs.Items;

/// <summary>
/// Full item representation.
/// Carries complete stats and behavioural condition so MatchService
/// can apply modifiers and evaluate sequences in a single call.
/// </summary>
public sealed record ItemDto(
    Guid Id,
    string Name,
    ItemCategory Category,
    ItemMythology Mythology,
    ItemClass ClassAffinity,
    Guid? SignatureHeroId,
    ItemBaseStatsDto BaseStats,
    ItemBehaviouralConditionDto? Condition
);