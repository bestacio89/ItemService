using Franz.Common.Business.Domain;

namespace ItemService.Domain.Items.Core;

/// <summary>
/// Describes the behavioural condition a hero must produce in combat
/// to activate this item's passive effect.
///
/// Design Contract:
/// - This is a pure descriptor — ItemService never evaluates it.
/// - Evaluation happens in MatchService against the EffectSequenceTracker.
/// - EffectType values are stored as integers to avoid coupling
///   ItemService to HeroService's domain.
/// - MatchService is responsible for interpreting these integers
///   against its own EffectType enum.
///
/// Examples:
/// - Gungnir: Pattern [Damage=0, CrowdControl=5, Damage=0], Window 4s
/// - Mjöllnir: ConsecutiveEffectType = 0 (Damage), ConsecutiveCount = 3
/// - Book of Thoth: DistinctEffectCount = 4, Window 6s
/// </summary>
public sealed class ItemBehaviouralCondition : ValueObject
{
  /// <summary>
  /// Ordered sequence of EffectType int values the hero must produce.
  /// Null if the condition is not pattern-based.
  /// </summary>
  public IReadOnlyList<int>? Pattern { get; }

  /// <summary>
  /// Time window in seconds within which the pattern must occur.
  /// </summary>
  public float? WindowSeconds { get; }

  /// <summary>
  /// For consecutive-type conditions (e.g. Mjöllnir).
  /// The EffectType int value that must repeat consecutively.
  /// </summary>
  public int? ConsecutiveEffectType { get; }

  /// <summary>
  /// Number of consecutive occurrences required.
  /// </summary>
  public int? ConsecutiveCount { get; }

  /// <summary>
  /// For diversity-type conditions (e.g. Book of Thoth).
  /// Number of distinct EffectTypes required within the window.
  /// </summary>
  public int? DistinctEffectCount { get; }

  /// <summary>
  /// Number of stacks required to trigger the passive.
  /// </summary>
  public int StackThreshold { get; }

  private ItemBehaviouralCondition(
      IReadOnlyList<int>? pattern,
      float? windowSeconds,
      int? consecutiveEffectType,
      int? consecutiveCount,
      int? distinctEffectCount,
      int stackThreshold)
  {
    Pattern = pattern;
    WindowSeconds = windowSeconds;
    ConsecutiveEffectType = consecutiveEffectType;
    ConsecutiveCount = consecutiveCount;
    DistinctEffectCount = distinctEffectCount;
    StackThreshold = stackThreshold;
  }

  // =========================================================
  // FACTORY METHODS
  // One per condition type — enforces valid combinations
  // =========================================================

  /// <summary>
  /// Gungnir-style: specific sequence within a time window.
  /// e.g. Damage → CrowdControl → Damage within 4 seconds
  /// </summary>
  public static ItemBehaviouralCondition ForPattern(
      IReadOnlyList<int> pattern,
      float windowSeconds,
      int stackThreshold = 1)
  {
    if (pattern is null || pattern.Count < 2)
      throw new ArgumentException("Pattern must have at least 2 effects.");

    if (windowSeconds <= 0)
      throw new ArgumentOutOfRangeException(nameof(windowSeconds));

    return new(pattern, windowSeconds, null, null, null, stackThreshold);
  }

  /// <summary>
  /// Mjöllnir-style: N consecutive uses of the same EffectType.
  /// e.g. 3 consecutive Damage effects without interruption
  /// </summary>
  public static ItemBehaviouralCondition ForConsecutive(
      int effectType,
      int consecutiveCount,
      int stackThreshold = 1)
  {
    if (consecutiveCount < 2)
      throw new ArgumentOutOfRangeException(nameof(consecutiveCount));

    return new(null, null, effectType, consecutiveCount, null, stackThreshold);
  }

  /// <summary>
  /// Book of Thoth-style: N distinct EffectTypes within a window.
  /// e.g. 4 different effect types used within 6 seconds
  /// </summary>
  public static ItemBehaviouralCondition ForDiversity(
      int distinctEffectCount,
      float windowSeconds,
      int stackThreshold = 1)
  {
    if (distinctEffectCount < 2)
      throw new ArgumentOutOfRangeException(nameof(distinctEffectCount));

    if (windowSeconds <= 0)
      throw new ArgumentOutOfRangeException(nameof(windowSeconds));

    return new(null, windowSeconds, null, null, distinctEffectCount, stackThreshold);
  }

  // =========================================================
  // VALUE OBJECT EQUALITY
  // =========================================================

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return WindowSeconds;
    yield return ConsecutiveEffectType;
    yield return ConsecutiveCount;
    yield return DistinctEffectCount;
    yield return StackThreshold;

    if (Pattern is not null)
      foreach (var p in Pattern)
        yield return p;
  }
}