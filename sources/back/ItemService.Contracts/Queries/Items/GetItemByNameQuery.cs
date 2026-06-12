using Franz.Common.Mediator.Messages;
using ItemService.Contracts.DTOs.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Contracts.Queries.Items;

public sealed record GetItemByNameQuery
  ( string Name,
  CancellationToken Cancellation): IQuery<ItemDto>;