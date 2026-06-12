using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;

using ArchUnitNET.xUnit;

using Franz.Common.Business.Domain;

using Franz.Common.DependencyInjection;
using Franz.Common.Mediator.Messages;
using Franz.Common.Business.Repositories;
using FranzTesting;
using Microsoft.Azure.Cosmos.Linq;
using System.Reflection;
using Interface = System.Reflection.TypeInfo;

namespace ItemService.Testing.ArchitectureTests
{
  public class ContractsArchitecture : BaseArchitectureTest
  {

    [Fact]
    public void Contracts_Assembly_Should_Exist()
    {
      Assert.NotNull(ContractsLayer); // Leverages the provider from BaseArchitectureTest
    }

   
   
    [Fact]
    public void QueryNameConventionIsCorrect()
    {
      var commandobjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (!commandobjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No Queries found in Contract layer Ã¢â‚¬â€ skipping rule.");
        return;
      }
      ArchRuleDefinition.
      Classes().That().AreAssignableTo(typeof(IQuery<>))
      .Should()
      .HaveNameEndingWith("Query")
      .Check(BaseArchitecture);
    }

    [Fact]
    public void CommandNameConventionIsCorrect()

    {
      var commandobjects = ContractsLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
        .ToList();

      if (!commandobjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No Commands found in Contract layer Ã¢â‚¬â€ skipping rule.");
        return;
      }
      ArchRuleDefinition.
      Classes().That().AreAssignableTo(typeof(ICommand<>))
       .Should()
       .HaveNameEndingWith("Command")
       .Check(BaseArchitecture);

    }

    [Fact]
    public void DtosNameConvetionIsCorrect()
    {
     
      var contractObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
             
              !t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase)
              )
          .ToList();

      if (!contractObjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No Dtos found Ã¢â‚¬â€ skipping persistence contract enforcement (virgin template).");
        return;
      }
      ArchRuleDefinition.
           Classes().That()
          .ResideInNamespace("ItemService.Contracts.DTOS.Items")
          .Should()
          .HaveNameEndingWith("Dto")
          .Because("Dtos should be properly named as such")
          .Check(BaseArchitecture);
    }

    [Fact]
    public void Infraestructure_Interfaces_Follow_Rules()
    {
      var contractObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
              t.Name.StartsWith("I") &&
              !t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
           .ToList();

      if (!contractObjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No Custom Service Intefaces found Ã¢â‚¬â€ skipping persistence contract enforcement (virgin template).");
        return;
      }
      ArchRuleDefinition.
        Classes()
        .That().ResideInNamespace("ItemService.Contracts.Infrastructure.*")
        .Should()
        .BeAssignableTo(typeof(IScopedDependency))
        .OrShould().BeAssignableTo(typeof(ISingletonDependency))
        .Because("Every infrastructureInterface requires aproper  lifetime")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture)       ;

    }

    [Fact]
    public void Persistence_Interfaces_Follow_Rules()
    {
      // Ã°Å¸Å½Â¯ Identify repository interfaces defined in the Domain (contracts or persistence abstractions)
      var contractObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
              t.Name.StartsWith("I") &&
              t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
          .ToList();

      if (!contractObjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No repository interfaces found Ã¢â‚¬â€ skipping persistence contract enforcement (virgin template).");
        return;
      }

      // Ã¢Å“â€¦ Enforce rule: all repository interfaces should conform to ItemService repository contracts
      ArchRuleDefinition
          .Classes()
          .That()
          .Are(contractObjects)
          .Should()
          .BeAssignableTo(typeof(IScopedDependency))
          .AndShould()
          .NotBeAssignableTo(typeof(IEntityRepository<,>))
          .OrShould()
          .NotBeAssignableTo(typeof(IAggregateRepository<,>))
          .AndShould()
          .HaveNameEndingWith("Repository")
                 
          .Because("Because repositories are scoped and Custom repos should not implement Generic Framework provisioned implementations")
          .WithoutRequiringPositiveResults()
          .Check(BaseArchitecture);

      Console.WriteLine($"Ã¢Å“â€¦ Verified {contractObjects.Count} repository interface(s) follow the ItemService persistence contract model.");
    }


    [Fact]
    public void CustomPersistenceInterfaces_Follow_Rules()
    {
      // Ã°Å¸Å½Â¯ Identify repository interfaces defined in the Domain (contracts or persistence abstractions)
      var contractObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
              t.Name.StartsWith("I") &&
              t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase) &&
              !t.Name.Contains("Aggregate") &&
              !t.Name.Contains("Read") &&
              !t.NameContains("Entity"))

          .ToList();

      if (!contractObjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No custom repository interfaces found Ã¢â‚¬â€ skipping persistence contract enforcement (virgin template).");
        return;
      }
      // Ã¢Å“â€¦ Enforce rule: all custom repository interfaces should inherit IScopedDependency
      ArchRuleDefinition.Classes()
          .That()
          .AreNot(typeof(IEntityRepository<,>)) // Exclude IReadRepository
          .And().AreNot(typeof(IAggregateRepository<,>)) // Exclude IAggregateRepository
          .And().HaveNameEndingWith("Repository") // Naming convention for custom repositories
          .Should()
          .ImplementAnyInterfacesThat()
          .HaveFullNameMatching(typeof(IScopedDependency).FullName!)
          // Enforce IScopedDependency
          .Because("Custom persistence interfaces should follow lifetime management using IScopedDependency")
          .Check(BaseArchitecture);
    }


    [Fact]
    public void Dtos_Must_Be_Immutable_Or_Records()
    {
      var dtoObjects = ContractsLayer
          .GetObjects(BaseArchitecture)
          .Where(t =>
              t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase) ||
              t.Namespace.FullName.Contains("DTOs", StringComparison.OrdinalIgnoreCase))
          .ToList();

      if (!dtoObjects.Any())
      {
        Console.WriteLine("Ã°Å¸Å¸Â¡ No DTOs found Ã¢â‚¬â€ skipping DTO immutability rule.");
        return;
      }

      var offenders = new List<IType>();

      foreach (var dto in dtoObjects)
      {
        var type = GetReflectionType(dto);
        if (type == null) continue;

        // Ã¢Å“â€¦ Skip records
        if (IsRecord(type))
          continue;

        // Ã°Å¸Â§Â  Check for mutable properties (setters not init-only)
        bool hasWritableProps = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Any(p =>
            {
              var setMethod = p.SetMethod;
              if (setMethod == null) return false;

              bool isInitOnly = setMethod.ReturnParameter
                  .GetRequiredCustomModifiers()
                  .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));

              return !isInitOnly;
            });

        if (hasWritableProps)
          offenders.Add(dto);
      }

      if (!offenders.Any())
      {
        Console.WriteLine("Ã¢Å“â€¦ All DTOs are immutable records or init-only Ã¢â‚¬â€ compliance confirmed.");
        return;
      }

      Console.WriteLine("Ã°Å¸Å¡Â¨ Mutable or non-record DTOs detected:");
      offenders.ForEach(o => Console.WriteLine($" - {o.FullName}"));

      var rule = ArchRuleDefinition
          .Classes()
          .That().Are(offenders)
          .Should().NotExist()
          .Because("DTOs must be immutable record types or have only init-only properties.");

      rule.Check(BaseArchitecture);
    }

    private static bool IsRecord(Type type)
    {
      return type.GetMethod("<Clone>$", BindingFlags.NonPublic | BindingFlags.Instance) != null
          || type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance) != null;
    }

    private static Type? GetReflectionType(IType archType)
    {
      // Try dynamic reflection Ã¢â‚¬â€ avoids compile-time generic binding
      var maybeTypeProp = archType.GetType().GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
      if (maybeTypeProp?.GetValue(archType) is Type systemType)
        return systemType;

      // fallback: try by name if reflection fails
      return Type.GetType(archType.FullName, throwOnError: false);
    }


  }
}




