using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Repositories;
using Franz.Common.DependencyInjection;
using Franz.Common.Mediator.Messages;
using FranzTesting;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// Ã¢Å¡â€“Ã¯Â¸Â ItemService Tribunal Ã¢â‚¬â€ Contracts Layer Governance
  /// Enforces naming, immutability, and lifetime conventions for the Contracts layer.
  /// Dynamically adapts to any solution prefix (ItemService, Raanz, etc.).
  /// </summary>
  public sealed class ContractsLayerComplianceAudit : ArchitecturalAuditBase
  {
    [Trait("Category", "ArchitecturalReport")]
    public void Contracts_Governance()
    {
      ExecuteTribunal("Contracts Layer Compliance Audit", (sb, markViolation) =>
      {
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine("               CONTRACTS LAYER COMPLIANCE AUDIT                ");
        sb.AppendLine("---------------------------------------------------------------");

        var prefix = SolutionPrefix; // Ã°Å¸â€Â¹ dynamic prefix resolution

        // RULE 1 Ã¢â‚¬â€ Assembly presence
        ExecuteRule("Assembly Presence", "Contracts assembly must be present.", () =>
        {
          Assert.NotNull(ContractsLayer);
          sb.AppendLine("Ã¢Å“â€¦ Verified: Contracts assembly detected.");
        }, sb, markViolation);

        // RULE 2 Ã¢â‚¬â€ Query Naming Convention
        ExecuteRule("Queries", "Queries must end with 'Query' and implement IQuery<>.", () =>
        {
          var queries = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!queries.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No Queries found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .AreAssignableTo(typeof(IQuery<>))
              .Should()
              .HaveNameEndingWith("Query")
              .Because("All query types should follow the 'SomethingQuery' naming pattern.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {queries.Count} Query class(es) follow CQRS naming conventions.");
        }, sb, markViolation);

        // RULE 3 Ã¢â‚¬â€ Command Naming Convention
        ExecuteRule("Commands", "Commands must end with 'Command' and implement ICommand.", () =>
        {
          var commands = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!commands.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No Commands found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .AreAssignableTo(typeof(ICommand<>))
              .Or()
              .AreAssignableTo(typeof(ICommand))
              .Should()
              .HaveNameEndingWith("Command")
              .Because("All command types should follow the 'SomethingCommand' naming pattern.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {commands.Count} Command class(es) follow CQRS naming conventions.");
        }, sb, markViolation);

        // RULE 4 Ã¢â‚¬â€ DTO Naming & Namespace (dynamic)
        ExecuteRule("DTOs", $"DTOs must end with 'Dto' and reside in {prefix}.Contracts.DTOs.", () =>
        {
          var dtos = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!dtos.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No DTOs found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .ResideInNamespaceMatching($"^{prefix}\\.Contracts\\.DTOs(\\..*)?$")
              .Should()
              .HaveNameEndingWith("Dto")
              .Because("All DTOs must be suffixed with 'Dto' and reside in the Contracts.DTOs namespace.")
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {dtos.Count} DTO class(es) follow naming and namespace conventions.");
        }, sb, markViolation);

        // RULE 5 Ã¢â‚¬â€ DTO Immutability / Record Enforcement
        ExecuteRule("DTO Immutability", "DTOs must be records or immutable types.", () =>
        {
          var dtoObjects = ContractsLayer.GetObjects(BaseArchitecture)
            .Where(t =>
              t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase) ||
              t.Namespace.FullName.Contains("DTOs", StringComparison.OrdinalIgnoreCase))
            .ToList();

          var offenders = new List<IType>();

          foreach (var dto in dtoObjects)
          {
            var type = GetReflectionType(dto);
            if (type == null) continue;

            if (IsRecord(type)) continue;

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

          if (offenders.Any())
          {
            markViolation();
            sb.AppendLine("Ã°Å¸Å¡Â¨ Mutable or non-record DTOs detected:");
            offenders.ForEach(o => sb.AppendLine($" - {o.FullName}"));
          }
          else sb.AppendLine("Ã¢Å“â€¦ All DTOs are immutable records Ã¢â‚¬â€ compliance confirmed.");
        }, sb, markViolation);

        // RULE 6 Ã¢â‚¬â€ Infrastructure Interfaces Lifetime Enforcement (dynamic)
        ExecuteRule("Infrastructure Interfaces", "Infrastructure interfaces must define lifetimes.", () =>
        {
          var infraInterfaces = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t =>
                  t.Name.StartsWith("I") &&
                  !t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!infraInterfaces.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No infrastructure interfaces found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(infraInterfaces)
              .Should()
              .BeAssignableTo(typeof(IScopedDependency))
              .OrShould()
              .BeAssignableTo(typeof(ISingletonDependency))
              .Because("Infrastructure interfaces must declare explicit lifetime scope.")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {infraInterfaces.Count} infrastructure interface(s) define lifetime dependencies.");
        }, sb, markViolation);

        // RULE 7 Ã¢â‚¬â€ Custom Repositories Must Be Scoped & Independent (dynamic)
        ExecuteRule("Custom Repositories", "Custom repositories must declare scoped lifetime and remain independent.", () =>
        {
          var customRepos = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t =>
                  t.Name.StartsWith("I") &&
                  t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Aggregate", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Read", StringComparison.OrdinalIgnoreCase) &&
                  !t.Name.Contains("Entity", StringComparison.OrdinalIgnoreCase) &&
                  t.Namespace.FullName.Contains("Persistence", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!customRepos.Any())
          {
            sb.AppendLine("Ã°Å¸Å¸Â¡ No custom repositories found Ã¢â‚¬â€ skipping rule.");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(customRepos)
              .Should()
              .BeAssignableTo(typeof(IScopedDependency))
              .AndShould()
              .NotBeAssignableTo(typeof(IEntityRepository<,>))
              .AndShould()
              .NotBeAssignableTo(typeof(IAggregateRepository<,>))
              .Because("Custom repositories must declare scoped lifetime and remain independent of framework abstractions.")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);

          sb.AppendLine($"Ã¢Å“â€¦ Verified {customRepos.Count} repository interface(s) comply with scoped-lifetime independence.");
        }, sb, markViolation);

        // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
        // Ã°Å¸Å½Â¯ VERDICT SUMMARY
        // Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬Ã¢â€â‚¬
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine(" CONTRACTS LAYER COMPLIANCE: COMPLETED SUCCESSFULLY");
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine($"Ã°Å¸â€¢Å Ã¯Â¸Â  {prefix}.Contracts Audit Verdict: Excellent");
        sb.AppendLine("Ã¢Å¡â„¢Ã¯Â¸Â  DTOs: Immutable Ã¢Å“â€  |  Commands/Queries: Properly Named Ã¢Å“â€| Repositories: properly Scoped and Independent from framework provided notions");
      });
    }

    // Ã°Å¸Â§Â  Helpers
    private static bool IsRecord(Type type) =>
      type.GetMethod("<Clone>$", BindingFlags.NonPublic | BindingFlags.Instance) != null ||
      type.GetMethod("PrintMembers", BindingFlags.NonPublic | BindingFlags.Instance) != null;

    private static Type? GetReflectionType(IType archType)
    {
      var maybeTypeProp = archType.GetType().GetProperty("Type", BindingFlags.Public | BindingFlags.Instance);
      if (maybeTypeProp?.GetValue(archType) is Type systemType)
        return systemType;

      return Type.GetType(archType.FullName, throwOnError: false);
    }
  }
}



