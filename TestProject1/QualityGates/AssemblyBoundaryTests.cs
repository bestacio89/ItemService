using ItemService.API.Controllers;
using ItemService.Application.Items.Services;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;
using ItemService.Persistence.Seeding;

namespace ItemService.Testing.QualityGates;

public sealed class AssemblyBoundaryTests
{
    [Fact]
    public void DomainDoesNotDependOnApplicationAdaptersOrApi()
    {
        var references = typeof(ItemEntity).Assembly.GetReferencedAssemblies().Select(reference => reference.Name);

        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
    }

    [Fact]
    public void ContractsDoNotDependOnRuntimeAdapters()
    {
        var references = typeof(CreateItemCommand).Assembly.GetReferencedAssemblies().Select(reference => reference.Name);

        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
    }

    [Fact]
    public void ApiUsesTheApplicationContractThroughItsControllerBoundary()
    {
        Assert.Equal("ItemService.API", typeof(ItemController).Assembly.GetName().Name);
        Assert.Equal("ItemService.Application", typeof(ItemUpdateService).Assembly.GetName().Name);
        Assert.Equal("ItemService.Persistence", typeof(ItemSeeder).Assembly.GetName().Name);
    }
}
