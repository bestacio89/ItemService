using ItemService.Contracts.Persistence;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Microsoft.EntityFrameworkCore;

namespace ItemService.Persistence.Persistence.Repositories;

public sealed class ItemLookupRepository : IItemLookupRepository
{
  private readonly ApplicationDbContext _context;

  public ItemLookupRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  // =========================================================
  // CATEGORY
  // =========================================================
  public async Task<PagedResult<ItemEntity>> GetByCategoryAsync(
      ItemCategory category,
      int page,
      int pageSize,
      CancellationToken ct = default)
  {
    var query = _context.Items
        .AsNoTracking()
        .Include(x => x.BaseStats)
        .Where(x => x.Category == category);

    return await ToPaged(query, page, pageSize, ct);
  }

  // =========================================================
  // MYTHOLOGY
  // =========================================================
  public async Task<PagedResult<ItemEntity>> GetByMythologyAsync(
      ItemMythology mythology,
      int page,
      int pageSize,
      CancellationToken ct = default)
  {
    var query = _context.Items
        .AsNoTracking()
        .Include(x => x.BaseStats)
        .Where(x => x.Mythology == mythology);

    return await ToPaged(query, page, pageSize, ct);
  }

  // =========================================================
  // CLASS AFFINITY
  // =========================================================
  public async Task<PagedResult<ItemEntity>> GetByClassAffinityAsync(
      ItemClass classAffinity,
      int page,
      int pageSize,
      CancellationToken ct = default)
  {
    var query = _context.Items
        .AsNoTracking()
        .Include(x => x.BaseStats)
        .Where(x => x.ClassAffinity == classAffinity);

    return await ToPaged(query, page, pageSize, ct);
  }

  // =========================================================
  // NAME (IDENTITY LOOKUP)
  // =========================================================
  public async Task<ItemEntity?> GetByNameAsync(
      string name,
      CancellationToken ct = default)
  {
    return await _context.Items
        .Include(x => x.BaseStats)
        .AsNoTracking()

        .FirstOrDefaultAsync(x => x.Name == name, ct);
  }

  public async Task<ItemEntity?> GetByIdWithDetailsAsync(
     Guid id,
     CancellationToken ct = default)
  {
    return await _context.Items
        .Include(x => x.BaseStats)
        .AsNoTracking()

        .FirstOrDefaultAsync(x => x.Id == id, ct);
  }

  // =========================================================
  // PAGINATION CORE
  // =========================================================
  private static async Task<PagedResult<ItemEntity>> ToPaged(
      IQueryable<ItemEntity> query,
      int page,
      int pageSize,
      CancellationToken ct)
  {
    if (page <= 0) page = 1;
    if (pageSize <= 0) pageSize = 20;

    var total = await query.CountAsync(ct);

    var items = await query
        .OrderBy(x => x.Name)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(ct);

    return new PagedResult<ItemEntity>(
        items,
        total,
        page,
        pageSize);
  }

  public async Task<IReadOnlyCollection<ItemEntity?>> GetAllWithDetailsAsync(
         CancellationToken ct = default)
  {
    var query = await _context.Items
        .AsNoTracking()
        .Include(x => x.BaseStats)
        .ToListAsync();
        

    return query;
  }
}