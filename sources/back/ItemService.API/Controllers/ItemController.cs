using Franz.Common.Mediator.Dispatchers;
using ItemService.Contracts.Commands.Items;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace ItemService.API.Controllers;

[ApiController]
[Route("items")]
public sealed class ItemController : ControllerBase
{
  private readonly IDispatcher _dispatcher;

  public ItemController(IDispatcher dispatcher)
  {
    _dispatcher = dispatcher;
  }

  // =========================================================
  // GET ALL (paged default)
  // =========================================================
  [HttpGet]
  public async Task<IActionResult> GetAll(
        CancellationToken ct = default)
  {
    var result = await _dispatcher.SendAsync(
        new GetAllItemsQuery());

    return Ok(result);
  }

  // =========================================================
  // GET BY ID
  // =========================================================
  [HttpGet("{id:guid}")]
  public async Task<IActionResult> GetById(
      Guid id,
      CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(
        new GetItemByIdQuery(id));

    return result is null ? NotFound() : Ok(result);
  }

  // =========================================================
  // GET BY NAME (unique lookup)
  // =========================================================
  [HttpGet("by-name/{name}")]
  public async Task<IActionResult> GetByName(
      string name,
      CancellationToken ct)
  {
    var result = await _dispatcher.SendAsync(
        new GetItemByNameQuery(name, ct));

    return result is null ? NotFound() : Ok(result);
  }

  // =========================================================
  // GET BY CATEGORY
  // =========================================================
  [HttpGet("category/{category}")]
  public async Task<IActionResult> GetByCategory(
      ItemCategory category,
      int page = 1,
      int pageSize = 20,
      CancellationToken ct = default)
  {
    var result = await _dispatcher.SendAsync(
        new GetItemsByCategoryQuery(category, page, pageSize));

    return Ok(result);
  }

  // =========================================================
  // GET BY MYTHOLOGY
  // =========================================================
  [HttpGet("mythology/{mythology}")]
  public async Task<IActionResult> GetByMythology(
      ItemMythology mythology,
      int page = 1,
      int pageSize = 20,
      CancellationToken ct = default)
  {
    var result = await _dispatcher.SendAsync(
        new GetItemsByMythologyQuery(mythology, page, pageSize));

    return Ok(result);
  }

  // =========================================================
  // GET BY CLASS AFFINITY
  // =========================================================
  [HttpGet("class/{classAffinity}")]
  public async Task<IActionResult> GetByClass(
      ItemClass classAffinity,
      int page = 1,
      int pageSize = 20,
      CancellationToken ct = default)
  {
    var result = await _dispatcher.SendAsync(
        new GetItemsByClassQuery(classAffinity, page, pageSize));

    return Ok(result);
  }

  // =========================================================
  // CREATE ITEM
  // =========================================================
  [HttpPost]
  public async Task<IActionResult> Create(
      [FromBody] CreateItemCommand command,
      CancellationToken ct)
  {
    var id = await _dispatcher.SendAsync(command);

    return CreatedAtAction(
        nameof(GetById),
        new { id },
        id);
  }

  // =========================================================
  // UPDATE ITEM STATS
  // =========================================================
  [HttpPut("{id:guid}/stats")]
  public async Task<IActionResult> UpdateStats(
      Guid id,
      [FromBody] UpdateItemStatsCommand command,
      CancellationToken ct)
  {
    if (id != command.ItemId)
      return BadRequest("Route ID does not match command ID.");

    await _dispatcher.SendAsync(command);

    return NoContent();
  }

  // =========================================================
  // DELETE ITEM
  // =========================================================
  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(
      Guid id,
      CancellationToken ct)
  {
    await _dispatcher.SendAsync(new DeleteItemCommand(id));

    return NoContent();
  }
}