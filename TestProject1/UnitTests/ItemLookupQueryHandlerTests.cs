using Franz.Common.Mapping.Abstractions;
using ItemService.Application.Items.Queries;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Persistence;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Moq;
using System.Reflection;

namespace ItemService.Testing.Unit.Application;

public sealed class ItemLookupQueryHandlerTests
{
    [Fact]
    public async Task GetAll_MapsRepositoryResultsAndForwardsCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        var item = CreateItem();
        IReadOnlyCollection<ItemEntity?> entities = new ItemEntity?[] { item };
        IReadOnlyCollection<ItemDto> expected = new[]
        {
            CreateDto(item.Id, item.Name)
        };
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetAllWithDetailsAsync(cancellation.Token))
            .ReturnsAsync(entities);
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        mapper.Setup(value => value.Map<IReadOnlyCollection<ItemEntity?>, IReadOnlyCollection<ItemDto>>(entities))
            .Returns(expected);
        var handler = new GetAllItemsQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetAllItemsQuery(), cancellation.Token);

        Assert.Same(expected, result);
        repository.Verify(value => value.GetAllWithDetailsAsync(cancellation.Token), Times.Once);
        mapper.Verify(value => value.Map<IReadOnlyCollection<ItemEntity?>, IReadOnlyCollection<ItemDto>>(entities), Times.Once);
        repository.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByName_MapsTheExactRepositoryResult()
    {
        using var cancellation = new CancellationTokenSource();
        var item = CreateItem();
        var expected = CreateDto(item.Id, item.Name);
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetByNameAsync("Gungnir", cancellation.Token))
            .ReturnsAsync(item);
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        mapper.Setup(value => value.Map<ItemEntity, ItemDto>(item)).Returns(expected);
        var handler = new GetItemByNameQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetItemByNameQuery("Gungnir", cancellation.Token), cancellation.Token);

        Assert.Same(expected, result);
        repository.Verify(value => value.GetByNameAsync("Gungnir", cancellation.Token), Times.Once);
        mapper.Verify(value => value.Map<ItemEntity, ItemDto>(item), Times.Once);
        repository.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByName_ThrowsForUnknownNameWithoutMapping()
    {
        using var cancellation = new CancellationTokenSource();
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetByNameAsync("Unknown Relic", cancellation.Token))
            .ReturnsAsync((ItemEntity?)null);
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        var handler = new GetItemByNameQueryHandler(repository.Object, mapper.Object);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetItemByNameQuery("Unknown Relic", cancellation.Token), cancellation.Token));

        Assert.Contains("Unknown Relic", error.Message, StringComparison.Ordinal);
        mapper.VerifyNoOtherCalls();
        repository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByCategory_UsesRequestedFilterAndPageBeforeMapping()
    {
        var item = CreateItem();
        IReadOnlyList<ItemEntity> entities = new[] { item };
        IReadOnlyList<ItemDto> expected = new[] { CreateDto(item.Id, item.Name) };
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetByCategoryAsync(ItemCategory.Relic, 2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<ItemEntity>(entities, 11, 2, 5));
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        mapper.Setup(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities))
            .Returns(expected);
        var handler = new global::GetItemsByCategoryQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetItemsByCategoryQuery(ItemCategory.Relic, 2, 5), CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(value => value.GetByCategoryAsync(ItemCategory.Relic, 2, 5, It.IsAny<CancellationToken>()), Times.Once);
        mapper.Verify(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities), Times.Once);
        repository.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByClass_UsesRequestedFilterAndPageBeforeMapping()
    {
        var item = CreateItem();
        IReadOnlyList<ItemEntity> entities = new[] { item };
        IReadOnlyList<ItemDto> expected = new[] { CreateDto(item.Id, item.Name) };
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetByClassAffinityAsync(ItemClass.Mage, 3, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<ItemEntity>(entities, 20, 3, 7));
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        mapper.Setup(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities))
            .Returns(expected);
        var handler = new global::GetItemsByClassQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetItemsByClassQuery(ItemClass.Mage, 3, 7), CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(value => value.GetByClassAffinityAsync(ItemClass.Mage, 3, 7, It.IsAny<CancellationToken>()), Times.Once);
        mapper.Verify(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities), Times.Once);
        repository.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetByMythology_UsesRequestedFilterAndPageBeforeMapping()
    {
        var item = CreateItem();
        IReadOnlyList<ItemEntity> entities = new[] { item };
        IReadOnlyList<ItemDto> expected = new[] { CreateDto(item.Id, item.Name) };
        var repository = new Mock<IItemLookupRepository>(MockBehavior.Strict);
        repository.Setup(value => value.GetByMythologyAsync(ItemMythology.Norse, 4, 9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<ItemEntity>(entities, 42, 4, 9));
        var mapper = new Mock<IFranzMapper>(MockBehavior.Strict);
        mapper.Setup(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities))
            .Returns(expected);
        var handler = new global::GetItemsByMythologyQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetItemsByMythologyQuery(ItemMythology.Norse, 4, 9), CancellationToken.None);

        Assert.Same(expected, result);
        repository.Verify(value => value.GetByMythologyAsync(ItemMythology.Norse, 4, 9, It.IsAny<CancellationToken>()), Times.Once);
        mapper.Verify(value => value.Map<IReadOnlyList<ItemEntity>, IReadOnlyList<ItemDto>>(entities), Times.Once);
        repository.VerifyNoOtherCalls();
        mapper.VerifyNoOtherCalls();
    }

    private static ItemDto CreateDto(Guid id, string name) =>
        new(id, name, ItemCategory.Relic, ItemMythology.Norse, ItemClass.Mage, null, null!, null);

    private static ItemEntity CreateItem()
    {
        var item = (ItemEntity)Activator.CreateInstance(
            typeof(ItemEntity),
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            args: new object[] { Guid.NewGuid() },
            culture: null)!;
        item.Define("Gungnir", ItemCategory.Relic, ItemMythology.Norse,
            ItemClass.Mage, null, null!, null, "unit-test");
        return item;
    }
}
