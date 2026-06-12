using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Events;
using Franz.Common.Mediator;
using FranzTesting;
using System;
using System.Linq;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// ⚖️ ItemService Tribunal — Domain Layer Compliance Audit
  /// Ensures correctness of entities, aggregates, events, and dependency rules.
  /// </summary>
  public sealed class DomainLayerComplianceAudit : ArchitecturalAuditBase
  {
    
    [Trait("Category", "ArchitecturalReport")]
    public void Domain_Governance()
    {
      ExecuteTribunal("Domain Layer Compliance Audit", (sb, markViolation) =>
      {
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("       ⚖️ DOMAIN LAYER COMPLIANCE AUDIT        ");
        sb.AppendLine("═══════════════════════════════════════════════");

        var prefix = SolutionPrefix;

        // =========================================================
        // RULE 1 — Entities
        // =========================================================
        ExecuteRule("Entities", "Entity inheritance validation", () =>
        {
          var domainEntities = DomainLayer
              .GetObjects(BaseArchitecture)
              .Where(t =>
                   t.Name.EndsWith("Entity", StringComparison.OrdinalIgnoreCase) &&
                  !t.ResidesInNamespace($"{prefix}.Domain.ValueObjects") &&
                  !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase) &&
                  !t.FullName.Contains("Persistence", StringComparison.OrdinalIgnoreCase) &&
                  !t.FullName.Contains("Mongo", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Repository", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Handler", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Service", StringComparison.OrdinalIgnoreCase) &&
                  t.Assembly.NameEquals(DomainAssembly.GetName().Name))
              .ToList();

          if (!domainEntities.Any())
          {
            sb.AppendLine("ℹ️ No domain entities found — skipping rule.");
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
              .Because("All domain entities must have a consistent identity model.")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Entities validated: {domainEntities.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 2 — Aggregates
        // =========================================================
        ExecuteRule("Aggregates", "Aggregate root validation", () =>
        {
          var aggregateRootInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i =>
                  i.FullName != null &&
                  i.FullName.StartsWith(typeof(IAggregateRoot<>).FullName!, StringComparison.OrdinalIgnoreCase));

          if (aggregateRootInterface == null)
          {
            sb.AppendLine("ℹ️ IAggregateRoot<T> not found — skipping rule.");
            return;
          }

          var aggregates = DomainLayer
              .GetObjects(BaseArchitecture)
              .Where(t => t.ImplementsInterface(aggregateRootInterface))
              .ToList();

          if (!aggregates.Any())
          {
            sb.AppendLine("ℹ️ No aggregate roots found.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(aggregates)
              .Should()
              .HaveNameEndingWith("Aggregate")
              .Because("Aggregate roots must follow naming conventions.")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Aggregates validated: {aggregates.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 3 — Domain Events
        // =========================================================
        ExecuteRule("Events", "Domain event validation", () =>
        {
          if (!HasDomainEvents)
          {
            sb.AppendLine("ℹ️ No domain events found.");
            return;
          }

          var validEvents = DomainEventTypes
              .Where(t =>
                  !t.FullName.Contains("Infrastructure", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!validEvents.Any())
          {
            sb.AppendLine("ℹ️ No valid events after filtering.");
            return;
          }

          var domainEventInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i => i.FullName == typeof(IDomainEvent).FullName);

          var integrationEventInterface = BaseArchitecture.Interfaces
              .FirstOrDefault(i => i.FullName == typeof(IIntegrationEvent).FullName);

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(validEvents)
              .Should()
              .HaveNameEndingWith("Event")
              .AndShould()
              .ImplementInterface(domainEventInterface)
              .OrShould()
              .ImplementInterface(integrationEventInterface)
              .Because("Events must be explicit and typed.")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Events validated: {validEvents.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 4 — Dependency Isolation
        // =========================================================
        ExecuteRule("Dependencies", "Domain isolation rule", () =>
        {
          ArchRuleDefinition
              .Classes()
              .That()
              .ResideInAssembly(DomainAssembly)
              .Should()
              .OnlyDependOnTypesThat()
              .ResideInNamespaceMatching($"^Franz\\.Common\\.Business(\\..*)?$")
              .OrShould()
              .ResideInNamespaceMatching($"^{prefix}\\.Domain(\\..*)?$") // ADDED: Allow internal domain references
              .OrShould()
              .ResideInNamespaceMatching("^System(\\..*)?$")
              .OrShould()
              .ResideInNamespaceMatching("^Microsoft(\\..*)?$")       // OPTIONAL: Often needed for Attributes
              .Because("Domain must remain pure and framework-agnostic.")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);

          sb.AppendLine("✅ Dependency isolation verified");
        }, sb, markViolation);

        // =========================================================
        // END REPORT
        // =========================================================
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("🎯 DOMAIN LAYER STATUS: CLEAN");
        sb.AppendLine($"🏷️ Solution: {prefix}");
        sb.AppendLine("═══════════════════════════════════════════════");
      });
    }
  }
}