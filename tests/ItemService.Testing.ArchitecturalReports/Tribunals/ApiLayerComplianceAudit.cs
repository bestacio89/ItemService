using System;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using FranzTesting;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// 🚪 ItemService Tribunal — API Layer Compliance Audit
  /// Enforces controller conventions, dependency isolation, and API boundary integrity.
  /// </summary>
  public sealed class ApiLayerComplianceAudit : ArchitecturalAuditBase
  {
    [Trait("Category", "ArchitecturalReport")]
    public void Audit_ApiLayer_Compliance()
    {
      ExecuteTribunal("API Layer Compliance Audit", (sb, markViolation) =>
      {
        var prefix = SolutionPrefix;

        sb.AppendLine("===============================================================");
        sb.AppendLine("🚪 API LAYER COMPLIANCE AUDIT");
        sb.AppendLine("===============================================================");

        // 🧱 RULE 1 — Assembly presence
        ExecuteRule("Assembly Presence", "API assembly must be detected.", () =>
        {
          Assert.NotNull(ApiAssembly);
          sb.AppendLine("✅ API assembly detected.");
        }, sb, markViolation);

        // 🧱 RULE 2 — Controller conventions
        var controllerNamingRule = ArchRuleDefinition
            .Classes()
            .That()
            .ResideInNamespaceMatching($"^{prefix}\\.API\\.Controllers(\\..*)?$")
            .And()
            .HaveNameEndingWith("Controller")
            .Should()
            .BeAssignableTo(typeof(ControllerBase))                 
            .Because("Controllers must live in API.Controllers and inherit ControllerBase.");

        ExecuteRule(
            "Controller Conventions",
            "Controllers must follow naming + inheritance rules.",
            controllerNamingRule,
            sb,
            markViolation
        );

        sb.AppendLine("🧱 RULE 2 — Controller conventions validated.");

        // 🧱 RULE 3 — Dependency isolation
        var isolationRule = ArchRuleDefinition
            .Classes()
            .That()
            .ResideInNamespaceMatching($"^{prefix}\\.API\\.Controllers(\\..*)?$")
            .Should()
            .OnlyDependOnTypesThat()
            .ResideInNamespaceMatching($"^{prefix}\\.Contracts(\\..*)?$")
            .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common(\\..*)?$")
            .OrShould().ResideInNamespaceMatching($"^{prefix}\\.API(\\..*)?$")
            .OrShould().ResideInNamespaceMatching(@"^System(\\..*)?$")
            .OrShould().ResideInNamespaceMatching(@"^Microsoft(\\..*)?$")
            .Because("Controllers are entry points and must not bypass Application or Domain layers.");

        ExecuteRule(
            "Dependency Isolation",
            "Controllers must not access Domain, Application, or Persistence directly.",
            isolationRule,
            sb,
            markViolation
        );

        sb.AppendLine("🧱 RULE 3 — Dependency isolation validated.");

        // 📌 FINAL SUMMARY
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine($"🚪 {prefix}.API Audit Final Result: COMPLETED");
        sb.AppendLine("---------------------------------------------------------------");
      });
    }
  }
}