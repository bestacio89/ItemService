using ItemService.API.Controllers;
using ItemService.Contracts.Commands.Items;
using Microsoft.AspNetCore.Mvc;
using Franz.Common.Mediator.Dispatchers;
using Moq;

namespace ItemService.Testing.Unit.Api;

public sealed class ItemControllerBehaviorTests
{
    [Fact]
    public async Task UpdateStats_ReturnsBadRequestWhenRouteAndCommandIdsDiffer()
    {
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        var controller = new ItemController(dispatcher.Object);
        var command = new UpdateItemStatsCommand(
            Guid.NewGuid(),
            // Hero modifier layer
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            // Skill modifier layer
            null, null, null,
            null, null, null,
            null, null, null);

        var result = await controller.UpdateStats(Guid.NewGuid(), command, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Route ID does not match command ID.", badRequest.Value);
        dispatcher.VerifyNoOtherCalls();
    }
}
