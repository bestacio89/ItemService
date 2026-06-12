using System;
using System.Linq;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Franz.Common.Mediator.Handlers;
using FranzTesting;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// ⚖️ ItemService Tribunal — Application Layer Compliance Audit
  /// Enforces CQRS structure, handler naming, and dependency isolation.
  /// </summary>
  public sealed class ApplicationLayerComplianceAudit : ArchitecturalAuditBase
  {
   
    [Trait("Category", "ArchitecturalReport")]
    public void Audit_ApplicationLayer_Compliance()
    {
      ExecuteTribunal("Application Layer Compliance Audit", (sb, markViolation) =>
      {
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine("   ⚙️ APPLICATION LAYER COMPLIANCE AUDIT      ");
        sb.AppendLine("═══════════════════════════════════════════════");

        var prefix = SolutionPrefix;

        // =========================================================
        // RULE 1 — Assembly presence
        // =========================================================
        ExecuteRule("Assembly", "Application assembly must exist", () =>
        {
          Assert.NotNull(ApplicationAssembly);
          sb.AppendLine("✅ Application assembly detected");
        }, sb, markViolation);

        // =========================================================
        // RULE 2 — Command handlers
        // =========================================================
        ExecuteRule("Command Handlers", "CQRS command handler conventions", () =>
        {
          var rule = ArchRuleDefinition.Classes()
              .That()
              .AreAssignableTo(typeof(ICommandHandler<,>))
              .And().Are(ApplicationLayer)
              .Should()
              .HaveNameEndingWith("CommandHandler")
              .Because("Command handlers must follow CQRS naming conventions");

          ExecuteRule("Command Handlers", "Must implement ICommandHandler and follow naming rules",
              rule, sb, markViolation);

          sb.AppendLine("✅ Command handlers validated");
        }, sb, markViolation);

        // =========================================================
        // RULE 3 — Query handlers
        // =========================================================
        ExecuteRule("Query Handlers", "CQRS query handler conventions", () =>
        {
          var rule = ArchRuleDefinition.Classes()
              .That()
              .AreAssignableTo(typeof(IQueryHandler<,>))
              .And().Are(ApplicationLayer)
              .Should()
              .HaveNameEndingWith("QueryHandler")
              .Because("Query handlers must follow CQRS naming conventions");

          ExecuteRule("Query Handlers", "Must implement IQueryHandler and follow naming rules",
              rule, sb, markViolation);

          sb.AppendLine("✅ Query handlers validated");
        }, sb, markViolation);

        // =========================================================
        // RULE 4 — Event handlers
        // =========================================================
        ExecuteRule("Event Handlers", "Mediator event handler conventions", () =>
        {
          var rule = ArchRuleDefinition.Classes()
              .That()
              .ResideInNamespaceMatching($"^{prefix}\\.Application\\.EventHandlers(\\..*)?$")
              .Should()
              .ImplementAnyInterfacesThat().HaveFullNameContaining("IEventHandler")
              .OrShould()
              .ImplementAnyInterfacesThat().HaveFullNameContaining("INotificationHandler")
              .AndShould()
              .HaveNameEndingWith("Handler")
              .Because("Event handlers must follow mediator conventions")
              .WithoutRequiringPositiveResults();

          ExecuteRule("Event Handlers", "Must implement mediator interfaces and follow naming rules",
              rule, sb, markViolation);

          sb.AppendLine("✅ Event handlers validated");
        }, sb, markViolation);

        // =========================================================
        // RULE 5 — Dependency isolation
        // =========================================================
        ExecuteRule("Dependency Isolation", "Application layer dependency rules", () =>
        {
          var rule = ArchRuleDefinition.Types()
              .That()
              .Are(ApplicationLayer)
              .And().DoNotHaveNameMatching(".*<.*>.*")
              .Should()
              .OnlyDependOnTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.Common(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Contracts(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Domain(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Application(\\..*)?$")
              .OrShould().ResideInNamespaceMatching("^System(\\..*)?$")
              .OrShould().ResideInNamespaceMatching("^Microsoft(\\..*)?$")
              .OrShould().ResideInNamespaceMatching("^AutoMapper(\\..*)?$")
              .Because("Application layer must remain orchestrator-only and framework-light")
              .WithoutRequiringPositiveResults();

          ExecuteRule("Dependency Isolation", "Must respect allowed dependency boundaries",
              rule, sb, markViolation);

          sb.AppendLine("✅ Dependency isolation verified");
        }, sb, markViolation);

        // =========================================================
        // FINAL REPORT
        // =========================================================
        sb.AppendLine("═══════════════════════════════════════════════");
        sb.AppendLine($"🎯 {prefix}.Application Governance: CLEAN");
        sb.AppendLine("═══════════════════════════════════════════════");
      });
    }
  }
}