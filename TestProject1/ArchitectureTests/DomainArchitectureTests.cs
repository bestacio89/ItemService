using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Events;
using Franz.Common.Mediator;
using Franz.Common.Mediator.Results;
using Franz.Common.Mediator.Errors;
namespace ItemService.Testing.ArchitectureTests;

/// <summary>
/// Validates the structural integrity of the Domain layer in ItemService-based projects.
/// </summary>
public class DomainArchitectureTests : BaseArchitectureTest
{
  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  // Ã°Å¸Â§Â± ENTITY RULES
  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
 
  [Fact]
  public void DomainEntities_ShouldInherit_FromEntityOrEntityOfT()
  {
    // Ã°Å¸Â§Â± Identify all domain entities that are NOT value objects or infrastructure
    var domainEntities = DomainLayer
        .GetObjects(BaseArchitecture)
        .Where(t =>
            t.NameEndsWith("Entity", StringComparison.OrdinalIgnoreCase) &&
            !t.ResidesInNamespace("ItemService.Domain.ValueObjects") && // Ã°Å¸Å¡Â« exclude ValueObjects
            !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase) &&
            !t.FullName.Contains("Persistence", StringComparison.OrdinalIgnoreCase) &&
            !t.FullName.Contains("Mongo", StringComparison.OrdinalIgnoreCase) &&
            !t.Name.Contains("Repository", StringComparison.OrdinalIgnoreCase) &&
            !t.Name.Contains("Context", StringComparison.OrdinalIgnoreCase) &&
            !t.Name.Contains("Handler", StringComparison.OrdinalIgnoreCase) &&
            !t.Name.Contains("Validator", StringComparison.OrdinalIgnoreCase) &&
            !t.Name.Contains("Service", StringComparison.OrdinalIgnoreCase) &&
            t.Assembly.NameEquals(DomainAssembly.GetName().Name))
        .ToList();

    if (!domainEntities.Any())
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No domain entities found Ã¢â‚¬â€ skipping Entity<> enforcement.");
      return;
    }

    // Ã¢Å“â€¦ Enforce that all domain entities inherit Entity or Entity<T>
    ArchRuleDefinition
        .Classes()
        .That()
        .Are(domainEntities)
        .Should()
        .BeAssignableTo(typeof(Entity<>))
        .OrShould()
        .BeAssignableTo(typeof(IEntity))
        .Because("All domain entities must inherit Entity or Entity<TId> for consistent identity, equality, and lifecycle management.")
        .Check(BaseArchitecture);

    Console.WriteLine($"Ã¢Å“â€¦ Verified {domainEntities.Count} domain entity type(s) inherit Entity or Entity<TId>.");
  }

  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  // Ã¢Å¡â„¢Ã¯Â¸Â AGGREGATE ROOT RULES
  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  [Fact]
  public void AggregateRoots_AreSetupCorrectly()
  {
    ReportArchitectureContext();

    var aggregateRootInterface = BaseArchitecture.Interfaces
        .FirstOrDefault(i => i.FullName != null &&
                             i.FullName.StartsWith(typeof(IAggregateRoot<>).FullName!,
                             StringComparison.OrdinalIgnoreCase));

    if (aggregateRootInterface == null)
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ IAggregateRoot<TEvent> interface not found Ã¢â‚¬â€ skipping aggregate root validation.");
      return;
    }

    var domainEventInterface = BaseArchitecture.Interfaces
        .FirstOrDefault(i => i.FullName == typeof(IDomainEvent).FullName);

    var aggregateRoots = DomainLayer
        .GetObjects(BaseArchitecture)
        .Where(t => t.ImplementsInterface(aggregateRootInterface))
        .ToList();

    if (!aggregateRoots.Any())
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No aggregate roots implementing IAggregateRoot<TEvent> found Ã¢â‚¬â€ skipping aggregate rule.");
      return;
    }

    // Ã¢Å“â€¦ Structural rule
    ArchRuleDefinition
        .Classes()
        .That()
        .Are(aggregateRoots)
        .Should()
        .BeAssignableTo(typeof(AggregateRoot<>))
        .AndShould()
        .HaveNameEndingWith("Aggregate")
        .Because("Aggregate roots should implement IAggregateRoot<TEvent> and inherit from AggregateRoot<> base class.")
        .Check(BaseArchitecture);

    Console.WriteLine($"Ã¢Å“â€¦ Validated {aggregateRoots.Count} aggregate root(s) successfully.");

    // Ã¢Å“â€¦ Optional: Ensure aggregates depend on domain events
    if (domainEventInterface != null)
    {
      ArchRuleDefinition
          .Classes()
          .That()
          .Are(aggregateRoots)
          .Should()
          .DependOnAnyTypesThat()
          .ImplementInterface(domainEventInterface)
          .Because("Aggregate roots should be capable of raising domain events.")
          .Check(BaseArchitecture);

      Console.WriteLine("Ã¢Å“â€¦ Verified aggregate roots depend on domain events.");
    }
    else
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ IDomainEvent interface not found Ã¢â‚¬â€ skipping dependency validation.");
    }
  }

  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  // Ã°Å¸â€œÂ¢ DOMAIN EVENT RULES
  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  [Fact]
  public void Events_AreSetupCorrectly()
  {
    ReportArchitectureContext();

    var aggregateRootInterface = BaseArchitecture.Interfaces
        .FirstOrDefault(i => i.FullName != null &&
                             i.FullName.StartsWith(typeof(IAggregateRoot<>).FullName!,
                             StringComparison.OrdinalIgnoreCase));

    var hasAggregates = DomainLayer
        .GetObjects(BaseArchitecture)
        .Any(t => aggregateRootInterface != null && t.ImplementsInterface(aggregateRootInterface));

    if (!hasAggregates)
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No aggregates found Ã¢â‚¬â€ skipping event validation test.");
      return;
    }

    if (!HasDomainEvents)
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No domain events found Ã¢â‚¬â€ skipping event validation test.");
      return;
    }

    var domainEventInterface = BaseArchitecture.Interfaces
        .FirstOrDefault(i => i.FullName == typeof(IDomainEvent).FullName);

    var integrationEventInterface = BaseArchitecture.Interfaces
        .FirstOrDefault(i => i.FullName == typeof(IIntegrationEvent).FullName);

    var validDomainEvents = DomainEventTypes
        .Where(t => !t.FullName.Contains("Validation", StringComparison.OrdinalIgnoreCase)
                 && !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (!validDomainEvents.Any())
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No valid domain events found after filtering internal types Ã¢â‚¬â€ skipping.");
      return;
    }

    ArchRuleDefinition
        .Classes()
        .That()
        .Are(validDomainEvents)
        .Should()
        .ImplementInterface(domainEventInterface)
        .OrShould()
        .ImplementInterface(integrationEventInterface)
        .AndShould()
        .HaveNameEndingWith("Event")
        .Because("All events should implement IDomainEvent or IIntegrationEvent and follow the 'SomethingHappenedEvent' naming convention.")
        .Check(BaseArchitecture);

    Console.WriteLine($"Ã¢Å“â€¦ Validated {validDomainEvents.Count} domain event(s) successfully.");
  }

  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  // Ã°Å¸Â§Â­ DOMAIN DEPENDENCY RULES
  // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
  [Fact]
  public void DomainAssemblyDependencies_AreCorrect()
  {
    var domainobjects = DomainLayer.GetObjects(BaseArchitecture)
        .Where(t => t.Name.EndsWith("Event") || t.Name.EndsWith("AggregateRoot") || t.GetType().Namespace == "*.Domain.Entities")
        .ToList();

    if (!domainobjects.Any())
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No CommandHandlers found in Application layer Ã¢â‚¬â€ skipping rule.");
      return;
    }
    ArchRuleDefinition
        .Classes()
        .That()
        .ResideInAssembly(DomainAssembly)
        .Should()
        .OnlyDependOnTypesThat()
        // Self references (other domain types)
        .ResideInNamespaceMatching("*.Domain")

        // ItemService base abstractions
        .OrShould().ResideInAssembly(typeof(Entity<>).Assembly.GetName().Name)          // Franz.Common.Business
        .OrShould().ResideInAssembly(typeof(ValueObject).Assembly.GetName().Name)       // Franz.Common.Business.Domain
        .OrShould().ResideInAssembly(typeof(Result).Assembly.GetName().Name)            // Franz.Common.Mediator
        .OrShould().ResideInAssembly(typeof(Error).Assembly.GetName().Name)             // Franz.Common.Errors

        // System namespaces
        .OrShould().ResideInNamespace("System")
        .OrShould().ResideInNamespace("System.Collections")
        .OrShould().ResideInNamespace("System.Collections.Generic")
        .OrShould().ResideInNamespace("System.Linq")
        .OrShould().ResideInNamespace("System.Runtime.CompilerServices")

        .Because("The Domain layer may depend on itself, ItemService domain abstractions, and system libraries only.")
        .WithoutRequiringPositiveResults()
        .Check(BaseArchitecture);

    Console.WriteLine("Ã¢Å“â€¦ Verified domain dependency isolation (self + ItemService + System).");
  }


  [Fact]
  public void DomainEvents_ShouldNotDependOnInfrastructure()
  {
    if (!HasDomainEvents)
    {
      Console.WriteLine("Ã°Å¸Å¸Â¡ No domain events found Ã¢â‚¬â€ skipping infrastructure dependency rule.");
      return;
    }

    ArchRuleDefinition
        .Classes()
        .That()
        .Are(DomainEventTypes)
        .Should()
        .NotDependOnAnyTypesThat()
        .ResideInNamespace("ItemService.Infrastructure")
        .Because("Domain events must be pure domain concepts without infrastructure dependencies.")
        .Check(BaseArchitecture);

    Console.WriteLine("Ã¢Å“â€¦ Confirmed domain events do not depend on infrastructure.");
  }
}




