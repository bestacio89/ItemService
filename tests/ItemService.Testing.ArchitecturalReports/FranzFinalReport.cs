using ItemService.Testing.ArchitecturalReports.Layers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace ItemService.Testing.ArchitecturalReports
{
  public sealed class ArchitectureComplianceReport
  {
    private record ReportEntry(string Tribunal, TimeSpan Duration, bool Passed, string Message);

    [Trait("Category", "ArchitecturalReport")]
    [Fact(DisplayName = "Architecture Compliance Report — Unified Compliance Summary")]
    public void Generate_Compliance_Summary()
    {
      Console.OutputEncoding = Encoding.UTF8;

      var results = new List<ReportEntry>();
      var report = new StringBuilder();

      report.AppendLine("===============================================================");
      report.AppendLine("            ITEMSERVICE ARCHITECTURAL SUPREME COURT            ");
      report.AppendLine("===============================================================");
      report.AppendLine($"Session Started: {DateTime.Now:G}");
      report.AppendLine();

      // Execute tribunals in order
      results.Add(RunAudit<DomainLayerComplianceAudit>("Domain Layer"));
      results.Add(RunAudit<ApplicationLayerComplianceAudit>("Application Layer"));
      results.Add(RunAudit<PersistenceLayerComplianceAudit>("Persistence Layer"));
      results.Add(RunAudit<ApiLayerComplianceAudit>("API Layer"));
      results.Add(RunAudit<ContractsLayerComplianceAudit>("Contracts Layer"));

      // =========================
      // TRIBUNAL LOG TABLE
      // =========================
      report.AppendLine("---------------------------------------------------------------");
      report.AppendLine("                        TRIBUNAL LOGS                          ");
      report.AppendLine("---------------------------------------------------------------");
      report.AppendLine("Layer                    | Result   | Duration  | Notes");
      report.AppendLine("-------------------------|----------|-----------|----------------");

      foreach (var r in results)
      {
        var result = r.Passed ? "PASS ✔" : "FAIL ✖";
        report.AppendLine($"{r.Tribunal,-24} | {result,-8} | {r.Duration.TotalSeconds,7:F2}s | {r.Message}");
      }

      // =========================
      // FINAL VERDICT ENGINE
      // =========================
      int total = results.Count;
      int failed = results.Count(r => !r.Passed);
      double failureRate = (double)failed / total;

      report.AppendLine("---------------------------------------------------------------");
      report.AppendLine("                      FINAL JUDGMENT                           ");
      report.AppendLine("---------------------------------------------------------------");

      var (verdict, message) = GetVerdict(failureRate);

      report.AppendLine($"STATUS: {verdict}");
      report.AppendLine($"DECREE: \"{message}\"");
      report.AppendLine("===============================================================");

      Console.WriteLine(report.ToString());

      Assert.True(failed == 0, $"Architectural Tribunal failed: {failed} layer(s) non-compliant.");
    }

    private static (string Verdict, string Message) GetVerdict(double failureRate)
    {
      return failureRate switch
      {
        0.0 => ("COMPLIANT (GOD-TIER)", "Absolute purity. Architecture is fully aligned."),
        <= 0.2 => ("MINOR INFRACTIONS", "Mostly clean, but minor structural drift detected."),
        <= 0.5 => ("NEEDS IMPROVEMENT", "Architecture is weakening under coupling pressure."),
        <= 0.8 => ("ARCHITECTURAL DEBT", "Severe layering violations detected. Refactor recommended."),
        _ => ("CRITICAL FAILURE", "System integrity compromised. Major redesign required.")
      };
    }

    private static ReportEntry RunAudit<T>(string title)
    {
      var stopwatch = Stopwatch.StartNew();

      try
      {
        var instance = Activator.CreateInstance(typeof(T));

        var method = typeof(T)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m =>
                m.Name.Contains("Governance", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Audit", StringComparison.OrdinalIgnoreCase) ||
                m.Name.Contains("Tribunal", StringComparison.OrdinalIgnoreCase));

        if (method == null)
          return new ReportEntry(title, stopwatch.Elapsed, true, "No audit method found");

        method.Invoke(instance, null);

        stopwatch.Stop();
        return new ReportEntry(title, stopwatch.Elapsed, true, "OK");
      }
      catch (Exception ex)
      {
        stopwatch.Stop();

        var realError = ex.InnerException?.Message ?? ex.Message;

        return new ReportEntry(title, stopwatch.Elapsed, false, realError);
      }
    }
  }
}