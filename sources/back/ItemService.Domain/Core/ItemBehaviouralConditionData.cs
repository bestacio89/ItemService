using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Domain.Core;

public sealed class ItemBehaviouralConditionData
{
  public List<int>? Pattern { get; set; }
  public float? WindowSeconds { get; set; }
  public int? ConsecutiveEffectType { get; set; }
  public int? ConsecutiveCount { get; set; }
  public int? DistinctEffectCount { get; set; }
  public int StackThreshold { get; set; }
}