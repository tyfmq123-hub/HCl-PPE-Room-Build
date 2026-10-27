param(
    [Parameter(Mandatory = $true)]
    [string]$UnityEditor,
    [switch]$FullEditMode
)

$ErrorActionPreference = 'Stop'
$projectRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$expectedVersion = '6000.4.8f1'
if (!(Test-Path -LiteralPath $UnityEditor -PathType Leaf)) {
    throw "NOT VERIFIED: required Unity $expectedVersion is unavailable: $UnityEditor"
}
$actualVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($UnityEditor).ProductVersion
if ($actualVersion -notlike "*$expectedVersion*") {
    throw "Editor version must contain '$expectedVersion'; reported '$actualVersion'. Verify the executable."
}

$runDirectory = Join-Path $projectRoot ('.utmp/ppe-tests-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
New-Item -ItemType Directory -Path $runDirectory -Force | Out-Null
$resultPath = Join-Path $runDirectory 'editmode-results.xml'
$logPath = Join-Path $runDirectory 'editmode.log'
$arguments = @(
    '-batchmode', '-nographics', '-projectPath', $projectRoot,
    '-runTests', '-testPlatform', 'EditMode',
    '-testResults', $resultPath, '-logFile', $logPath
)
if (!$FullEditMode) {
    $arguments += @('-testFilter', 'PPEPortfolioSequenceTests;PPEGlovePairStateTests;ResponseBagPackingTests;PPEInnerGloveHandTintTests')
}
# Do not add -quit: Test Framework owns process exit after completing the run.
& $UnityEditor @arguments
$editorExitCode = $LASTEXITCODE
if (!(Test-Path -LiteralPath $resultPath)) {
    throw "NOT VERIFIED: Unity exit $editorExitCode, but no test XML. Inspect $logPath"
}
[xml]$results = Get-Content -LiteralPath $resultPath -Raw
$run = $results.'test-run'
$passed = [int]$run.passed
$failed = [int]$run.failed
$total = [int]$run.total
Write-Output "Unity exit=$editorExitCode result=$($run.result) total=$total passed=$passed failed=$failed"
Write-Output "Results: $resultPath"
Write-Output "Log: $logPath"
if ($editorExitCode -ne 0 -or $run.result -ne 'Passed' -or $total -eq 0 -or $failed -gt 0) {
    throw 'Unity validation did not pass. Review compilation and test logs before building.'
}
