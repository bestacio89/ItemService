using System;
using System.Collections.Generic;
using System.Text;

namespace ItemService.Application.Items.Services;

public interface IItemUniquenessChecker
{
  Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
}