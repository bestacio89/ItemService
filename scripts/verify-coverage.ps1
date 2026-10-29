param(
    [Parameter(Mandatory = $true)]
    [string] $CoverageRoot,

    [Parameter(Mandatory = $false)]
    [double] $MinimumLineRate = 0.25
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $CoverageRoot -PathType Container)) {
    throw "Coverage directory '$CoverageRoot' does not exist."
}

$reports = Get-ChildItem -LiteralPath $CoverageRoot -Recurse -File -Filter "coverage.cobertura.xml"
$candidates = foreach ($report in $reports) {
    [xml] $xml = Get-Content -LiteralPath $report.FullName
    $clientClass = $xml.SelectSingleNode("//class[@name='ItemService.Client.Http.Clients.ItemHttpClient']")

    if ($null -eq $clientClass) {
        continue
    }

    $coveredClientLines = @($clientClass.SelectNodes("./lines/line") |
        Where-Object { [int] $_.GetAttribute("hits") -gt 0 }).Count

    # The main application test run executes this production type. The
    # architecture-report test assembly may reference it without executing it.
    if ($coveredClientLines -gt 0) {
        [pscustomobject] @{
            Path = $report.FullName
            LineRate = [double] $xml.DocumentElement.GetAttribute("line-rate")
        }
    }
}

$selected = $candidates | Sort-Object LineRate -Descending | Select-Object -First 1

if ($null -eq $selected) {
    throw "No executable application coverage report was found. Ensure the main test suite collects Cobertura coverage."
}

$percentage = [math]::Round($selected.LineRate * 100, 2)
$minimumPercentage = [math]::Round($MinimumLineRate * 100, 2)
Write-Host "Application line coverage: $percentage% (minimum: $minimumPercentage%)."
Write-Host "Coverage report: $($selected.Path)"

if ($selected.LineRate -lt $MinimumLineRate) {
    throw "Application line coverage $percentage% is below the required $minimumPercentage%."
}
