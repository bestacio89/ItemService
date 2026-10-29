using ItemService.Application.Items.Commands;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Core;
using Moq;

namespace ItemService.Testing.Unit.Application;

public sealed class ItemCommandHandlerTests
{
    [Fact]
    public async Task CreateHandler_ForwardsCommandAndCancellationAndReturnsCreatedId()
    {
        using var cancellation = new CancellationTokenSource();
        var command = CreateCommand();
        var expectedId = Guid.NewGuid();
        var service = new Mock<IItemCreationService>(MockBehavior.Strict);
        service.Setup(value => value.CreateAsync(command, cancellation.Token)).ReturnsAsync(expectedId);
        var handler = new CreateItemCommandHandler(service.Object);

        var result = await handler.Handle(command, cancellation.Token);

        Assert.Equal(expectedId, result);
        service.Verify(value => value.CreateAsync(command, cancellation.Token), Times.Once);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateHandler_PropagatesCreationFailure()
    {
        using var cancellation = new CancellationTokenSource();
        var command = CreateCommand();
        var service = new Mock<IItemCreationService>(MockBehavior.Strict);
        service.Setup(value => value.CreateAsync(command, cancellation.Token))
            .ThrowsAsync(new InvalidOperationException("duplicate item"));
        var handler = new CreateItemCommandHandler(service.Object);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.Handle(command, cancellation.Token));

        Assert.Equal("duplicate item", error.Message);
        service.Verify(value => value.CreateAsync(command, cancellation.Token), Times.Once);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateStatsHandler_ForwardsCommandAndCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var command = new UpdateItemStatsCommand(
            Guid.NewGuid(),
            100, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null, null,
            null, null, null,
            null, null, null,
            null, null, null,
            "unit-test");
        var service = new Mock<IItemUpdateService>(MockBehavior.Strict);
        service.Setup(value => value.UpdateStatsAsync(command, cancellation.Token)).Returns(Task.CompletedTask);
        var handler = new UpdateItemStatsCommandHandler(service.Object);

        await handler.Handle(command, cancellation.Token);

        service.Verify(value => value.UpdateStatsAsync(command, cancellation.Token), Times.Once);
        service.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteHandler_ForwardsCommandAndCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var command = new DeleteItemCommand(Guid.NewGuid());
        var service = new Mock<IItemDeletionService>(MockBehavior.Strict);
        service.Setup(value => value.DeleteAsync(command, cancellation.Token)).Returns(Task.CompletedTask);
        var handler = new global::DeleteItemCommandHandler(service.Object);

        await handler.Handle(command, cancellation.Token);

        service.Verify(value => value.DeleteAsync(command, cancellation.Token), Times.Once);
        service.VerifyNoOtherCalls();
    }

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
