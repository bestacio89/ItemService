using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Business.Domain;
using Franz.Common.Business.Repositories;
using Franz.Common.DependencyInjection;
using Franz.Common.Mediator.Messages;
using FranzTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// ⚖️ ItemService Tribunal — Contracts Layer Governance
  /// Ensures correctness, immutability, naming conventions, and DI discipline.
  /// </summary>
  public sealed class ContractsLayerComplianceAudit : ArchitecturalAuditBase
  {
    
    [Trait("Category", "ArchitecturalReport")]
    public void Contracts_Governance()
    {
      ExecuteTribunal("Contracts Layer Compliance Audit", (sb, markViolation) =>
      {
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("     📦 CONTRACTS LAYER COMPLIANCE AUDIT       ");
        sb.AppendLine("═══════════════════════════════════════════════");

        var prefix = SolutionPrefix;

        // =========================================================
        // RULE 1 — Assembly presence
        // =========================================================
        ExecuteRule("Assembly", "Contracts assembly must exist", () =>
        {
          Assert.NotNull(ContractsLayer);
          sb.AppendLine("✅ Contracts assembly detected");
        }, sb, markViolation);

        // =========================================================
        // RULE 2 — Queries
        // =========================================================
        ExecuteRule("Queries", "Query naming and CQRS compliance", () =>
        {
          var queries = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Query", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!queries.Any())
          {
            sb.AppendLine("ℹ️ No queries found");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .AreAssignableTo(typeof(IQuery<>))
              .Should()
              .HaveNameEndingWith("Query")
              .Because("Queries must follow CQRS naming conventions")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Queries validated: {queries.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 3 — Commands
        // =========================================================
        ExecuteRule("Commands", "Command naming and CQRS compliance", () =>
        {
          var commands = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Command", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!commands.Any())
          {
            sb.AppendLine("ℹ️ No commands found");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .AreAssignableTo(typeof(ICommand))
              .Or().AreAssignableTo(typeof(ICommand<>))
              .Should()
              .HaveNameEndingWith("Command")
              .Because("Commands must follow CQRS naming conventions")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Commands validated: {commands.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 4 — DTO naming & namespace
        // =========================================================
        ExecuteRule("DTOs", $"DTOs must follow {prefix}.Contracts.DTOs conventions", () =>
        {
          var dtos = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t => t.Name.EndsWith("Dto", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!dtos.Any())
          {
            sb.AppendLine("ℹ️ No DTOs found");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .ResideInNamespaceMatching($"^{prefix}\\.Contracts\\.DTOs(\\..*)?$")
              .Should()
              .HaveNameEndingWith("Dto")
              .Because("DTOs must be clearly separated and named consistently")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ DTOs validated: {dtos.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 5 — DTO immutability
        // =========================================================
        ExecuteRule("DTO Immutability", "DTOs must be immutable (records or init-only)", () =>
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

            var mutableProps = type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.SetMethod != null && !IsInitOnly(p));

            if (mutableProps)
              offenders.Add(dto);
          }

          if (offenders.Any())
          {
            markViolation();
            sb.AppendLine("🚨 Mutable DTOs detected:");
            foreach (var o in offenders)
              sb.AppendLine($" - {o.FullName}");
          }
          else
          {
            sb.AppendLine("✅ All DTOs are immutable");
          }

        }, sb, markViolation);

        // =========================================================
        // RULE 6 — Infrastructure interfaces lifetimes
        // =========================================================
        ExecuteRule("DI Lifetimes", "Interfaces must declare lifecycle", () =>
        {
          var infraInterfaces = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t =>
                  t.Name.StartsWith("I") &&
                  !t.Name.EndsWith("Command") &&
                  !t.Name.EndsWith("Query") &&
                  !t.Name.EndsWith("Dto")&&
                  !t.Name.EndsWith("Repository"))
              .ToList();

          if (!infraInterfaces.Any())
          {
            sb.AppendLine("ℹ️ No infrastructure interfaces found");
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
              .Because("Interfaces must explicitly define lifetime")
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ DI lifetimes validated: {infraInterfaces.Count}");
        }, sb, markViolation);

        // =========================================================
        // RULE 7 — Repositories
        // =========================================================
        ExecuteRule("Repositories", "Repository contracts must be clean and scoped", () =>
        {
          var repos = ContractsLayer.GetObjects(BaseArchitecture)
              .Where(t =>
                  t.Name.StartsWith("I") &&
                  t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!repos.Any())
          {
            sb.AppendLine("ℹ️ No repositories found");
            return;
          }

          ArchRuleDefinition
              .Classes()
              .That()
              .Are(repos)
              .Should()
              .BeAssignableTo(typeof(IScopedDependency))
              .Because("Repositories must be scoped dependencies")
              .WithoutRequiringPositiveResults()
              .Check(BaseArchitecture);

          sb.AppendLine($"✅ Repositories validated: {repos.Count}");
        }, sb, markViolation);

        // =========================================================
        // FINAL VERDICT
        // =========================================================
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("🎯 CONTRACTS LAYER STATUS: CLEAN");
        sb.AppendLine($"🏷️ Solution: {prefix}");
        sb.AppendLine("═══════════════════════════════════════════════");
      });
    }

    // =============================================================
    // Helpers
    // =============================================================
    private static bool IsRecord(Type type) =>
      type.GetMethod("<Clone>$", BindingFlags.NonPublic | BindingFlags.Instance) != null;

    private static bool IsInitOnly(PropertyInfo p)
    {
      var setMethod = p.SetMethod;
      return setMethod != null &&
             setMethod.ReturnParameter
               .GetRequiredCustomModifiers()
               .Contains(typeof(System.Runtime.CompilerServices.IsExternalInit));
    }

    private static Type? GetReflectionType(IType archType)
    {
      var prop = archType.GetType().GetProperty("Type");
      return prop?.GetValue(archType) as Type;
    }
  }
}