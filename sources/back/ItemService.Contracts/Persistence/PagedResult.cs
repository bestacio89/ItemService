using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Contracts.Persistence;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);