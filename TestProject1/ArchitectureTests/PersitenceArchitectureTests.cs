using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ItemService.Testing.ArchitectureTests;
public class PersistenceArchitectureTests : BaseArchitectureTest
{
  private Assembly[] LoadAssembliesWithPattern(string assemblyPattern)
  {
    var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
    var matchingAssemblies = loadedAssemblies
        .Where(assembly => Regex.IsMatch(assembly.FullName, assemblyPattern))
        .ToArray();

    return matchingAssemblies;
  }

  [Fact]
  public void PersistenceAssemblyExistence()
  {
    Assert.NotNull(PersistenceAssembly);
  }

  [Fact]
  public void PersistenceAssemblyDependencies()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      var dependencies = type.GetTypeInfo().ImplementedInterfaces;
      Assert.True(dependencies.Any(dep => dep.FullName.EndsWith("Domain.dll") || dep.FullName.EndsWith("Contracts.dll")));
    }
  }

  [Fact]
  public void PersistenceLayerDependencies_AreCorrect()
  {
    ReportArchitectureContext();
    var repositories = PersistenceLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Repository"))
        .ToList();

    if (!repositories.Any())
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No CommandHandlers found in Application layer Ã¢â‚¬â€ skipping rule.");
      return;
    }

    ArchRuleDefinition
        .Classes()
        .That()
        .ResideInAssembly(PersistenceAssembly)
        .Should()
        .OnlyDependOnTypesThat()
        // Ã¢Å“â€¦ Allow dependencies on ItemService core domain abstractions
        .ResideInNamespaceMatching("Franz.Common.Business.Domain")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Business.Events")

        // Ã¢Å“â€¦ Allow dependencies on ItemService persistence and repository tooling
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Repositories")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Extensions")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Configuration")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Behaviors")
        .OrShould().ResideInNamespaceMatching("Franz.Common.EntityFramework.Properties")
        .OrShould().ResideInNamespaceMatching("Franz.Common.MongoDB")
        .OrShould().ResideInNamespaceMatching("Franz.Common.MongoDB.Config") 

        // Ã¢Å“â€¦ Allow DI, Mediator, and core utilities
        .OrShould().ResideInNamespaceMatching("Microsoft.Extensions.DependencyInjection")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Mediator")
        .OrShould().ResideInNamespaceMatching("Franz.Common.Errors")

        // Ã¢Å“â€¦ Allow standard BCL namespaces
        .OrShould().ResideInNamespaceMatching("System")
        .OrShould().ResideInNamespaceMatching("System.Collections")
        .OrShould().ResideInNamespaceMatching("System.Collections.Generic")
        .OrShould().ResideInNamespaceMatching("System.Linq")
        .OrShould().ResideInNamespaceMatching("System.Threading")
        .OrShould().ResideInNamespaceMatching("System.Threading.Tasks")
        .OrShould().ResideInNamespaceMatching("System.Runtime.CompilerServices")

        // Ã¢Å“â€¦ Allow itself (internal persistence classes)
        .OrShould().ResideInNamespaceMatching("ItemService.Persistence")

        .Because("The Persistence layer should depend only on ItemService persistence abstractions, core business types, and system libraries.")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);

    Console.WriteLine("Ã¢Å“â€¦ Verified persistence dependency isolation (ItemService + System + self).");
  }




  [Fact]
  public void RepositoryImplementationsExist()
  {
    var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence\..*")
        .SelectMany(assembly => assembly.GetTypes());

    foreach (var type in persistenceTypes)
    {
      if (type.Name.EndsWith("Repository") && type.IsClass)
      {
        // Check for implementation of a repository interface
        var repositoryInterface = type.GetInterfaces()
            .FirstOrDefault(interf => interf.Name.EndsWith("Repository"));

        Assert.NotNull(repositoryInterface);
      }
    }
  }
}


