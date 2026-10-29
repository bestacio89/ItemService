using Franz.Common.Mediator.Dispatchers;
using ItemService.API.Controllers;
using ItemService.Contracts.Commands.Items;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Core;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ItemService.Testing.Unit.Api;

public sealed class ItemControllerEndpointTests
{
    [Fact]
    public async Task GetById_ReturnsOkWithTheDispatchedItem()
    {
        var id = Guid.NewGuid();
        var dto = CreateDto(id);
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(It.IsAny<GetItemByIdQuery>())).ReturnsAsync(dto);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.GetById(id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(dto, ok.Value);
        dispatcher.Verify(value => value.SendAsync(It.Is<GetItemByIdQuery>(query => query.ItemId == id)), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundWhenTheDispatchedItemIsMissing()
    {
        var id = Guid.NewGuid();
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(It.IsAny<GetItemByIdQuery>())).ReturnsAsync((ItemDto?)null);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.GetById(id, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
        dispatcher.Verify(value => value.SendAsync(It.Is<GetItemByIdQuery>(query => query.ItemId == id)), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByName_ReturnsNotFoundWhenTheDispatchedItemIsMissing()
    {
        using var cancellation = new CancellationTokenSource();
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(It.IsAny<GetItemByNameQuery>())).ReturnsAsync((ItemDto?)null);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.GetByName("Unknown Relic", cancellation.Token);

        Assert.IsType<NotFoundResult>(result);
        dispatcher.Verify(value => value.SendAsync(It.Is<GetItemByNameQuery>(query =>
            query.Name == "Unknown Relic" && query.Cancellation == cancellation.Token)), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtTheItemResource()
    {
        var command = CreateCommand();
        var id = Guid.NewGuid();
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(command)).ReturnsAsync(id);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.Create(command, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(ItemController.GetById), created.ActionName);
        Assert.Equal(id, created.RouteValues!["id"]);
        Assert.Equal(id, created.Value);
        dispatcher.Verify(value => value.SendAsync(command), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateStats_ReturnsNoContentAfterSuccessfulDispatch()
    {
        var id = Guid.NewGuid();
        var command = new UpdateItemStatsCommand(
            id,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null,
            null, null, null,
            null, null, null);
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(command)).Returns(Task.CompletedTask);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.UpdateStats(id, command, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        dispatcher.Verify(value => value.SendAsync(command), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Delete_ReturnsNoContentAfterSuccessfulDispatch()
    {
        var id = Guid.NewGuid();
        var dispatcher = new Mock<IDispatcher>(MockBehavior.Strict);
        dispatcher.Setup(value => value.SendAsync(It.IsAny<DeleteItemCommand>())).Returns(Task.CompletedTask);
        var controller = new ItemController(dispatcher.Object);

        var result = await controller.Delete(id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        dispatcher.Verify(value => value.SendAsync(It.Is<DeleteItemCommand>(command => command.ItemId == id)), Times.Once);
        dispatcher.VerifyNoOtherCalls();
    }

    private static ItemDto CreateDto(Guid id) =>
        new(id, "Gungnir", ItemCategory.Relic, ItemMythology.Norse, ItemClass.Mage, null, null!, null);

    private static CreateItemCommand CreateCommand() =>
        new(
            "Gungnir",
            ItemCategory.Relic,
            ItemMythology.Norse,
            ItemClass.Mage,
            null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null,
            null, null, null,
            null, null, null,
            null, null, null, null, null);
}
