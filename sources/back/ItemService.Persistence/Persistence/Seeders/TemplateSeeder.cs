using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Franz.Common.Data;

namespace ItemService.Persistence;

public sealed class DatabaseSeeder
{
  private readonly IEnumerable<ISeeder> _seeders;

  public DatabaseSeeder(IEnumerable<ISeeder> seeders)
  {
    _seeders = seeders;
  }

  public async Task RunAsync(CancellationToken ct)
  {
    foreach (var seeder in _seeders.OrderBy(x => x.Order))
    {
      await seeder.SeedAsync(ct);
    }
  }
}


