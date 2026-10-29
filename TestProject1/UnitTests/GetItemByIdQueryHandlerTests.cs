using Franz.Common.Business.Repositories;
using Franz.Common.Mapping.Abstractions;
using ItemService.Application.Items.Queries;
using ItemService.Contracts.DTOs.Items;
using ItemService.Contracts.Queries.Items;
using ItemService.Domain.Core;
using ItemService.Domain.Items.Core;
using Moq;

namespace ItemService.Testing.Unit.Application;

public sealed class GetItemByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_MapsTheItemReturnedByTheRepository()
    {
        var id = Guid.NewGuid();
        var item = CreateItem(id);
        var dto = new ItemDto(id, "Gungnir", ItemCategory.Relic, ItemMythology.Norse,
            ItemClass.Mage, null, null!, null);
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        var mapper = new Mock<IFranzMapper>();
        mapper.Setup(value => value.Map<ItemEntity, ItemDto>(item)).Returns(dto);
        var handler = new GetItemByIdQueryHandler(repository.Object, mapper.Object);

        var result = await handler.Handle(new GetItemByIdQuery(id), CancellationToken.None);

        Assert.Same(dto, result);
        repository.Verify(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        mapper.Verify(value => value.Map<ItemEntity, ItemDto>(item), Times.Once);
    }

    [Fact]
    public async Task Handle_ThrowsWhenTheItemDoesNotExist()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<IEntityRepository<ItemEntity, Guid>>();
        repository.Setup(value => value.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ItemEntity?)null);
        var mapper = new Mock<IFranzMapper>();
        var handler = new GetItemByIdQueryHandler(repository.Object, mapper.Object);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(new GetItemByIdQuery(id), CancellationToken.None));

        Assert.Contains(id.ToString(), error.Message, StringComparison.Ordinal);
        mapper.VerifyNoOtherCalls();
    }

    private static ItemEntity CreateItem(Guid id) =>
        (ItemEntity)Activator.CreateInstance(typeof(ItemEntity),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            binder: null, args: new object[] { id }, culture: null)!;
}
