using ItemService.API.Controllers;
using ItemService.Application.Items.Services;
using ItemService.Client.Http.Clients;
using ItemService.Contracts.Commands.Items;
using ItemService.Domain.Items.Core;
using ItemService.Persistence;
using ItemService.Persistence.Seeding;

namespace ItemService.Testing.QualityGates;

public sealed class AssemblyBoundaryTests
{
    [Fact]
    public void DomainDoesNotDependOnApplicationAdaptersOrApi()
    {
        var references = ReferencedAssemblyNames(typeof(ItemEntity).Assembly);

        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
        Assert.DoesNotContain("ItemService.Client.Http", references);
    }

    [Fact]
    public void ContractsDoNotDependOnRuntimeAdapters()
    {
        var references = ReferencedAssemblyNames(typeof(CreateItemCommand).Assembly);

        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
        Assert.DoesNotContain("ItemService.Client.Http", references);
    }

    [Fact]
    public void ApplicationDependsOnContractsAndDomainNotDeliveryOrPersistenceAdapters()
    {
        var references = ReferencedAssemblyNames(typeof(ItemUpdateService).Assembly);

        Assert.Contains("ItemService.Contracts", references);
        Assert.Contains("ItemService.Domain", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
        Assert.DoesNotContain("ItemService.Client.Http", references);
    }

    [Fact]
    public void PersistenceDoesNotDependOnDeliveryOrApplicationLayers()
    {
        var references = ReferencedAssemblyNames(typeof(ApplicationDbContext).Assembly);

        Assert.Contains("ItemService.Contracts", references);
        Assert.Contains("ItemService.Domain", references);
        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
        Assert.DoesNotContain("ItemService.Client.Http", references);
    }

    [Fact]
    public void TypedHttpClientDoesNotDependOnServerRuntimeAdapters()
    {
        var references = ReferencedAssemblyNames(typeof(ItemHttpClient).Assembly);

        Assert.Contains("ItemService.Contracts", references);
        Assert.DoesNotContain("ItemService.Application", references);
        Assert.DoesNotContain("ItemService.Persistence", references);
        Assert.DoesNotContain("ItemService.API", references);
        Assert.DoesNotContain("ItemService.Consumer", references);
    }

    [Fact]
    public void ApiCompositionRootReferencesApplicationAndPersistence()
    {
        var references = ReferencedAssemblyNames(typeof(ItemController).Assembly);

        Assert.Contains("ItemService.Application", references);
        Assert.Contains("ItemService.Persistence", references);
        Assert.Contains("ItemService.Contracts", references);
        Assert.Equal("ItemService.Persistence", typeof(ItemSeeder).Assembly.GetName().Name);
    }

    private static string?[] ReferencedAssemblyNames(System.Reflection.Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(reference => reference.Name).ToArray();
}
