using System;
using System.Linq;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Events;
using Franz.Common.Mediator;
using Franz.Common.Mediator.Results;
using Franz.Common.Mediator.Errors;
using FranzTesting;
using Xunit;
using ArchUnitNET.Domain.Extensions;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// Ã¢Å¡â€“Ã¯Â¸Â ItemService Tribunal Ã¢â‚¬â€ Domain Layer Governance
  /// Validates entity inheritance, aggregate consistency, event purity,
  /// and dependency boundaries within the Domain layer (prefix-agnostic).
  /// </summary>
  public sealed class DomainLayerComplianceAudit : ArchitecturalAuditBase
  {
    [Trait("Category", "ArchitecturalReport")]
    public void Domain_Governance()
    {
      ExecuteTribunal("Domain Layer Compliance Audit", (sb, markViolation) =>
      {
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine("                 DOMAIN LAYER COMPLIANCE AUDIT                 ");
        sb.AppendLine("---------------------------------------------------------------");

        var prefix = SolutionPrefix; // Ã°Å¸â€Â¹ dynamic prefix extraction (like Persistence test)

        // RULE 1 Ã¢â‚¬â€ Entity inheritance
        ExecuteRule("Entities", "All Domain Entities must inherit Entity or Entity<T>.", () =>
        {
          var domainEntities = DomainLayer
              .GetObjects(BaseArchitecture)
              .Where(t =>
                  !t.ResidesInNamespace($"{prefix}.Domain.ValueObjects") &&
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
            sb.AppendLine("Ã°Å¸Å¸Â¡ No domain entities found Ã¢â‚¬â€ skipping Entity<> enforcement.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(domainEntities)
              .Should()
              .BeAssignableTo(typeof(Entity<>))
              .OrShould()
              .BeAssignableTo(typeof(IEntity))
              .Because("All domain entities must inherit Entity or Entity<TId> for consistent identity and lifecycle management.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {domainEntities.Count} domain entity type(s) inherit Entity or Entity<TId>.");
        }, sb, markViolation);

        // RULE 2 Ã¢â‚¬â€ Aggregate roots
        ExecuteRule("Aggregates", "Aggregate roots must inherit AggregateRoot<> and implement IAggregateRoot<T>.", () =>
        {
          var aggregateRootInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i => i.FullName != null &&
                  i.FullName.StartsWith(typeof(IAggregateRoot<>).FullName!, StringComparison.OrdinalIgnoreCase));

          if (aggregateRootInterface == null)
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ IAggregateRoot<T> interface not found Ã¢â‚¬â€ skipping aggregate validation.");
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
            sb.AppendLine("Ã°Å¸Å¸Â¡ No aggregate roots implementing IAggregateRoot<T> found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(aggregateRoots)
              .Should()
              .BeAssignableTo(typeof(AggregateRoot<>))
              .AndShould()
              .HaveNameEndingWith("Aggregate")
              .Because("Aggregate roots should implement IAggregateRoot<T> and inherit AggregateRoot<> base class.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Validated {aggregateRoots.Count} aggregate root(s) successfully.");

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

            sb.AppendLine("Ã¢Å“â€¦ Verified aggregate roots depend on domain events.");
          }
        }, sb, markViolation);

        // RULE 3 Ã¢â‚¬â€ Domain events
        ExecuteRule("Events", "Domain events must implement IDomainEvent or IIntegrationEvent and end with 'Event'.", () =>
        {
          if (!HasDomainEvents)
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No domain events found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          var domainEventInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i => i.FullName == typeof(IDomainEvent).FullName);

          var integrationEventInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i => i.FullName == typeof(IIntegrationEvent).FullName);

          var validEvents = DomainEventTypes
              .Where(t =>
                  !t.FullName.Contains("Validation", StringComparison.OrdinalIgnoreCase) &&
                  !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!validEvents.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No valid domain events found after filtering internal types.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(validEvents)
              .Should()
              .ImplementInterface(domainEventInterface)
              .OrShould()
              .ImplementInterface(integrationEventInterface)
              .AndShould()
              .HaveNameEndingWith("Event")
              .Because("All events should implement IDomainEvent or IIntegrationEvent and follow the 'SomethingHappenedEvent' naming convention.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Validated {validEvents.Count} domain event(s) successfully.");
        }, sb, markViolation);

        // RULE 4 Ã¢â‚¬â€ Domain dependency isolation (dynamic prefix)
        ExecuteRule("Dependencies", "Domain layer may depend only on Common abstractions and System libraries.", () =>
        {
          ArchRuleDefinition
              .Classes()
              .That()
              .ResideInAssembly(DomainAssembly)
              .Should()
              .OnlyDependOnTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.Common\\.Business\\.Domain(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common\\.Mediator(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Domain(\\..*)?$")
              .OrShould().ResideInNamespaceMatching(@"^System(\..*)?$")
              .Because("The Domain layer must remain pure Ã¢â‚¬â€ it may depend only on its own layer, Common abstractions, and System libraries.")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);

          sb.AppendLine("Ã¢Å“â€¦ Verified domain layer dependency purity (Common + System only).");
        }, sb, markViolation);

        // RULE 5 Ã¢â‚¬â€ Domain events purity (no infrastructure leakage)
        ExecuteRule("Purity", "Domain events must not depend on infrastructure namespaces.", () =>
        {
          if (!HasDomainEvents)
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No domain events found Ã¢â‚¬â€ skipping purity check.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(DomainEventTypes)
              .Should()
              .NotDependOnAnyTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.Infrastructure(\\..*)?$")
              .Because("Domain events must remain pure and not depend on persistence or infrastructure.")
              .Check(BaseArchitecture);

          sb.AppendLine("Ã¢Å“â€¦ Confirmed domain events do not depend on infrastructure.");
        }, sb, markViolation);

        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine(" DOMAIN LAYER COMPLIANCE: COMPLETED SUCCESSFULLY");
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine($"Ã°Å¸â€¢Å Ã¯Â¸Â  {prefix}.Domain Audit Verdict: Excellent");
        sb.AppendLine("Ã¢Å¡â„¢Ã¯Â¸Â  Dependencies: Common + System Ã¢Å“â€");
      });
    }
  }
}



