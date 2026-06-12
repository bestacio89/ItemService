using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Client.Http.Models;


public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount
);
