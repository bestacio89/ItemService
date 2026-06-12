using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace ItemService.Testing.ArchitecturalReports
{
  /// <summary>
  /// ItemService Architectural Compliance Engine (Base)
  /// -------------------------------------------------
  /// Standardized execution and forensic reporting framework.
  /// Optimized for ArchUnitNET's evaluation patterns.
  /// </summary>
  public abstract class ArchitecturalAuditBase : BaseArchitectureTest
  {
    protected static void ExecuteTribunal(string tribunalName, Action<StringBuilder, Action> run)
    {
      Console.OutputEncoding = Encoding.UTF8;

      var sb = new StringBuilder();
      int violationCount = 0;
      Action markViolation = () => violationCount++;

      Console.WriteLine();
      Console.WriteLine("===============================================================");
      Console.WriteLine($" ARCHITECTURE COMPLIANCE AUDIT — {tribunalName.ToUpper()}");
      Console.WriteLine("===============================================================");

      try
      {
        run(sb, markViolation);
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] Unhandled exception during tribunal execution: {ex.Message}");
      }

      Console.WriteLine(sb.ToString());

      if (violationCount > 0)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine($" RESULT: NON-COMPLIANT ❌ — {violationCount} violation(s) detected");
        Console.WriteLine(" ACTION: Fix architectural boundaries before proceeding.");
        Console.WriteLine("---------------------------------------------------------------");
        Console.ResetColor();

        Assert.Fail($"{tribunalName} detected {violationCount} architectural violation(s).");
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("---------------------------------------------------------------");
        Console.WriteLine(" RESULT: COMPLIANT ✅ — No architectural violations detected");
        Console.WriteLine(" STATUS: Architecture integrity intact.");
        Console.WriteLine("---------------------------------------------------------------");
        Console.ResetColor();
      }

      Console.WriteLine();
    }

    protected static void ExecuteRule(
        string context,
        string summary,
        IArchRule rule,
        StringBuilder sb,
        Action markViolation)
    {
      try
      {
        rule.Check(BaseArchitecture);
        sb.AppendLine($"[PASS] {context} — {summary}");
      }
      catch (FailedArchRuleException)
      {
        markViolation();
        sb.AppendLine($"[FAIL] {context} — {summary}");

        sb.AppendLine("   ❌ VIOLATION DETAILS:");

        var results = rule.Evaluate(BaseArchitecture);
        var failures = results.Where(r => !r.Passed).ToList();

        if (failures.Any())
        {
          foreach (var f in failures)
          {
            sb.AppendLine($"   ⚠ {f.Description}");
          }
        }
        else
        {
          sb.AppendLine("   ⚠ No detailed failure info returned by ArchUnitNET.");
        }

        if (!string.IsNullOrWhiteSpace(rule.Description))
        {
          sb.AppendLine($"   💡 Rule: {rule.Description}");
        }
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] {context} — Unexpected error: {ex.Message}");
      }
    }

    protected static void ExecuteRule(
        string context,
        string summary,
        Action manualAssertion,
        StringBuilder sb,
        Action markViolation)
    {
      try
      {
        manualAssertion();
        sb.AppendLine($"[PASS] {context} — {summary}");
      }
      catch (XunitException ex)
      {
        markViolation();
        sb.AppendLine($"[FAIL] {context} — {summary} (Assert: {ex.Message})");
      }
      catch (Exception ex)
      {
        markViolation();
        sb.AppendLine($"[ERROR] {context} — Unexpected error: {ex.Message}");
      }
    }
  }
}