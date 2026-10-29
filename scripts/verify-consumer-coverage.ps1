param(
    [Parameter(Mandatory = $true)]
    [string] $CoverageRoot,

    [Parameter(Mandatory = $false)]
    [double] $MinimumLineRate = 0.80
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $CoverageRoot -PathType Container)) {
    throw "Coverage directory '$CoverageRoot' does not exist."
}

$reports = Get-ChildItem -LiteralPath $CoverageRoot -Recurse -File -Filter "coverage.cobertura.xml"
$candidates = foreach ($report in $reports) {
    [xml] $xml = Get-Content -LiteralPath $report.FullName
    $processor = $xml.SelectSingleNode('//class[starts-with(@name, "ItemService.Consumer.Processing.GenericMessageProcessor")]')

    if ($null -eq $processor) {
        continue
    }

    $lines = @($processor.SelectNodes("./lines/line"))
    $validLines = $lines.Count
    $coveredLines = @($lines | Where-Object { [int] $_.GetAttribute("hits") -gt 0 }).Count

    if ($validLines -gt 0) {
        [pscustomobject] @{
            Path = $report.FullName
            CoveredLines = $coveredLines
            ValidLines = $validLines
            LineRate = $coveredLines / $validLines
        }
    }
}

$selected = $candidates | Sort-Object LineRate -Descending | Select-Object -First 1

if ($null -eq $selected) {
    throw "No executable GenericMessageProcessor coverage was found in '$CoverageRoot'."
}

$percentage = [math]::Round($selected.LineRate * 100, 2)
$minimumPercentage = [math]::Round($MinimumLineRate * 100, 2)
Write-Host "Consumer processor line coverage: $percentage% ($($selected.CoveredLines)/$($selected.ValidLines); minimum: $minimumPercentage%)."
Write-Host "Coverage report: $($selected.Path)"

if ($selected.LineRate -lt $MinimumLineRate) {
    throw "Consumer processor line coverage $percentage% is below the required $minimumPercentage%."
}
