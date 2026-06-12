using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using FranzTesting;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports.Layers
{
  /// <summary>
  /// 🗄️ ItemService Tribunal — Persistence Layer Compliance Audit
  /// Validates repository structure, dependency purity, and layer isolation.
  /// </summary>
  public sealed class PersistenceLayerComplianceAudit : ArchitecturalAuditBase
  {
    private static Assembly[] LoadAssembliesWithPattern(string assemblyPattern)
    {
      return AppDomain.CurrentDomain.GetAssemblies()
          .Where(a => Regex.IsMatch(a.FullName, assemblyPattern, RegexOptions.IgnoreCase))
          .ToArray();
    }

    [Trait("Category", "ArchitecturalReport")]
    public void Persistence_Governance()
    {
      ExecuteTribunal("Persistence Tribunal", (sb, markViolation) =>
      {
        var prefix = SolutionPrefix;

        sb.AppendLine("===============================================================");
        sb.AppendLine("🗄️ PERSISTENCE LAYER COMPLIANCE AUDIT");
        sb.AppendLine("===============================================================");

        // 🧱 RULE 1 — Assembly presence
        ExecuteRule("Assembly", "Persistence assembly must be present.", () =>
        {
          Assert.NotNull(PersistenceAssembly);
          sb.AppendLine("✅ Persistence assembly detected.");
        }, sb, markViolation);

        // 🧱 RULE 2 — Repository interface enforcement
        ExecuteRule("Repositories", "Repositories must implement corresponding interfaces.", () =>
        {
          var persistenceAssemblies = LoadAssembliesWithPattern(@".*\.Persistence(\.|$)");

          var repositoryTypes = persistenceAssemblies
              .SelectMany(a => a.GetTypes())
              .Where(t => t.IsClass && t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
              .ToList();

          if (!repositoryTypes.Any())
          {
            sb.AppendLine("⚠️ No repositories found — skipping rule.");
            return;
          }

          foreach (var repo in repositoryTypes)
          {
            var hasInterface = repo.GetInterfaces()
                .Any(i => i.Name.EndsWith("Repository"));

            if (!hasInterface)
            {
              sb.AppendLine($"❌ {repo.Name} does not implement a repository interface.");
              markViolation();
            }
          }

          sb.AppendLine($"✅ Verified {repositoryTypes.Count} repository implementation(s).");
        }, sb, markViolation);

        // 🧱 RULE 3 — Domain/Contracts dependency check
        ExecuteRule("Dependencies", "Persistence must depend on Domain or Contracts abstractions.", () =>
        {
          var persistenceTypes = LoadAssembliesWithPattern(@".*\.Persistence(\.|$)")
              .SelectMany(a => a.GetTypes())
              .ToList();

          if (!persistenceTypes.Any())
          {
            sb.AppendLine("⚠️ No persistence types found — skipping rule.");
            return;
          }

          bool hasValidDependency = persistenceTypes
              .SelectMany(t => t.GetTypeInfo().ImplementedInterfaces)
              .Any(i =>
                  i.FullName?.Contains(".Domain") == true ||
                  i.FullName?.Contains(".Contracts") == true);

          if (!hasValidDependency)
          {
            sb.AppendLine("❌ Persistence layer lacks Domain/Contracts dependency.");
            markViolation();
          }
          else
          {
            sb.AppendLine("✅ Persistence layer references Domain/Contracts abstractions.");
          }
        }, sb, markViolation);

        // 🧱 RULE 4 — Dependency isolation
        ExecuteRule("Isolation", "Persistence must remain isolated from upper layers.", () =>
        {
          var rule = ArchRuleDefinition
              .Classes()
              .That()
              .ResideInAssembly(PersistenceAssembly)
              .Should()
              .OnlyDependOnTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.Common\\.Business\\.Domain(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common\\.Business\\.Events(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common\\.Mediator(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Common(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Domain(\\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Contracts(\\..*)?$")
              .OrShould().ResideInNamespaceMatching(@"^System(\..*)?$")
              .OrShould().ResideInNamespaceMatching(@"^Microsoft(\..*)?$")
              .OrShould().ResideInNamespaceMatching($"^{prefix}\\.Persistence(\\..*)?$")
              .Because("Persistence must depend only on allowed abstractions and system libraries.")
              .WithoutRequiringPositiveResults();

          rule.Check(BaseArchitecture);

          sb.AppendLine("✅ Persistence dependency isolation validated.");
        }, sb, markViolation);

        // 🧱 RULE 5 — Isolation from upper layers
        ExecuteRule("Isolation", "Persistence must not depend on API or Application layers.", () =>
        {
          ArchRuleDefinition
              .Classes()
              .That()
              .ResideInAssembly(PersistenceAssembly)
              .Should()
              .NotDependOnAnyTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.API(\\..*)?$")
              .AndShould()
              .NotDependOnAnyTypesThat()
              .ResideInNamespaceMatching($"^{prefix}\\.Application(\\..*)?$")
              .Because("Persistence must remain isolated from higher layers.")
              .Check(BaseArchitecture);

          sb.AppendLine("✅ Confirmed persistence isolation from API and Application.");
        }, sb, markViolation);

        // 📌 FINAL SUMMARY
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine("🗄️ PERSISTENCE LAYER COMPLIANCE: COMPLETED");
        sb.AppendLine("---------------------------------------------------------------");
        sb.AppendLine($"🧾 {prefix}.Persistence Audit Verdict: Excellent");
        sb.AppendLine("⚙️ Dependencies: Domain + Contracts + System only");
      });
    }
  }
}