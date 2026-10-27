param(
    [string]$RepositoryRoot = (Join-Path $PSScriptRoot '..\..\..'),
    [Parameter(Mandatory = $true)]
    [string]$ReferenceUnityEditorData
)

# Supplemental static checks only. This script does not start Unity, import the
# project, run NUnit, or assert that the current project compiles in Unity.
$ErrorActionPreference = 'Stop'
$RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
$outputDirectory = Join-Path $RepositoryRoot 'Temp\PortfolioValidation'
New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
$utf8 = [System.Text.UTF8Encoding]::new($false)
$sdkLine = @(& dotnet --list-sdks)[-1]
if ($sdkLine -notmatch '^([^ ]+) \[(.+)\]$') { throw 'No local .NET SDK found.' }
$sdkDirectory = Join-Path $Matches[2] $Matches[1]
$dotnetDirectory = Split-Path (Split-Path $sdkDirectory -Parent) -Parent
$compiler = Join-Path $sdkDirectory 'Roslyn\bincore\csc.dll'
$roslynDirectory = Split-Path $compiler -Parent
$referencePack = Get-ChildItem (Join-Path $dotnetDirectory 'packs\Microsoft.NETCore.App.Ref') -Directory |
    Sort-Object { [version]$_.Name } | Select-Object -Last 1
$targetFramework = 'net' + ([version]$referencePack.Name).Major + '.0'
$frameworkReferences = Join-Path $referencePack.FullName ('ref\' + $targetFramework)

$parserSource = @'
using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

public static class PpeSyntaxCheck
{
    public static int Main(string[] args)
    {
        var files = Directory.GetFiles(Path.Combine(args[0], "Assets", "1.JSR"), "*.cs", SearchOption.AllDirectories);
        Array.Sort(files, StringComparer.Ordinal);
        var errors = 0;
        foreach (var editor in new[] { false, true })
        {
            var options = new CSharpParseOptions(LanguageVersion.Latest,
                preprocessorSymbols: editor ? new[] { "UNITY_EDITOR", "UNITY_INCLUDE_TESTS" } : Array.Empty<string>());
            var configurationErrors = 0;
            foreach (var file in files)
            {
                var tree = CSharpSyntaxTree.ParseText(File.ReadAllText(file), options, file);
                foreach (var diagnostic in tree.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error))
                {
                    Console.WriteLine(diagnostic);
                    configurationErrors++;
                }
            }
            Console.WriteLine($"SYNTAX {(editor ? "Editor" : "Player")}: {files.Length} files, {configurationErrors} errors");
            errors += configurationErrors;
        }
        Console.WriteLine("Syntax parsing does not resolve APIs, run tests, or compile the Unity project.");
        return errors == 0 ? 0 : 1;
    }
}
'@
$parserPath = Join-Path $outputDirectory 'PpeSyntaxCheck.cs'
[System.IO.File]::WriteAllText($parserPath, $parserSource, $utf8)
$parserArguments = @('/nologo', '/target:exe', '/langversion:latest', '/nostdlib+',
    ('/out:"' + (Join-Path $outputDirectory 'PpeSyntaxCheck.dll') + '"'))
$parserArguments += Get-ChildItem $frameworkReferences -Filter '*.dll' | ForEach-Object { '/reference:"' + $_.FullName + '"' }
$parserArguments += @('Microsoft.CodeAnalysis.dll', 'Microsoft.CodeAnalysis.CSharp.dll') |
    ForEach-Object { '/reference:"' + (Join-Path $roslynDirectory $_) + '"' }
$parserArguments += '"' + $parserPath + '"'
$parserResponse = Join-Path $outputDirectory 'syntax.rsp'
[System.IO.File]::WriteAllLines($parserResponse, $parserArguments, $utf8)
& dotnet $compiler ('@' + $parserResponse)
if ($LASTEXITCODE -ne 0) { throw 'Could not compile the local syntax parser.' }
foreach ($assembly in @('Microsoft.CodeAnalysis.dll', 'Microsoft.CodeAnalysis.CSharp.dll')) {
    Copy-Item -LiteralPath (Join-Path $roslynDirectory $assembly) -Destination $outputDirectory -Force
}
$runtimeConfiguration = @{ runtimeOptions = @{ tfm = $targetFramework; framework = @{
    name = 'Microsoft.NETCore.App'; version = $referencePack.Name
} } } | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText((Join-Path $outputDirectory 'PpeSyntaxCheck.runtimeconfig.json'), $runtimeConfiguration, $utf8)
$syntaxOutput = & dotnet (Join-Path $outputDirectory 'PpeSyntaxCheck.dll') $RepositoryRoot 2>&1
$syntaxExit = $LASTEXITCODE
[System.IO.File]::WriteAllLines((Join-Path $outputDirectory 'syntax.log'), @($syntaxOutput | ForEach-Object { $_.ToString() }), $utf8)
$syntaxOutput | Write-Output

# API/syntax compilation against the shipped 2026-09-10 demo assemblies is a
# separate compatibility probe. Its dependencies are older than the 09-18 source;
# a diagnostic can therefore identify a baseline API mismatch, not a new bug.
$semanticArguments = @('/nologo', '/target:library', '/langversion:latest', '/define:UNITY_EDITOR',
    '/nostdlib+', '/nowarn:0436', ('/out:"' + (Join-Path $outputDirectory 'PpeModifiedStatic.dll') + '"'))
$semanticArguments += Get-ChildItem (Join-Path $RepositoryRoot 'Runtime\Win64\HClTrainingPPEDemo_Data\Managed') -Filter '*.dll' |
    ForEach-Object { '/reference:"' + $_.FullName + '"' }
$nunit = Join-Path $ReferenceUnityEditorData 'Resources\PackageManager\BuiltInPackages\com.unity.ext.nunit\net40\unity-custom\nunit.framework.dll'
if (!(Test-Path -LiteralPath $nunit)) { throw 'Local NUnit reference is missing; provide -ReferenceUnityEditorData.' }
$semanticArguments += '/reference:"' + $nunit + '"'
$semanticArguments += '/reference:"' + (Join-Path $ReferenceUnityEditorData 'Managed\UnityEditor.dll') + '"'
# Compile current PPE runtime dependencies together to avoid artificial duplicate
# type identity conflicts with the old Assembly-CSharp snapshot. App/shared/package
# implementations still come from the old demo and remain an explicit limitation.
$sources = @(Get-ChildItem (Join-Path $RepositoryRoot 'Assets\1.JSR\Scripts') -Filter '*.cs' -Recurse |
    ForEach-Object { $_.FullName })
$sources += @(
    'Assets\1.JSR\Editor\PPEGlovePairStateTests.cs',
    'Assets\1.JSR\Editor\PPEPortfolioSequenceTests.cs',
    'Assets\1.JSR\Editor\ResponseBagPackingTests.cs'
) | ForEach-Object { Join-Path $RepositoryRoot $_ }
$semanticArguments += $sources | ForEach-Object { '"' + $_ + '"' }
$semanticResponse = Join-Path $outputDirectory 'modified-static.rsp'
[System.IO.File]::WriteAllLines($semanticResponse, $semanticArguments, $utf8)
$semanticOutput = & dotnet $compiler ('@' + $semanticResponse) 2>&1
$semanticExit = $LASTEXITCODE
[System.IO.File]::WriteAllLines((Join-Path $outputDirectory 'modified-static.log'), @($semanticOutput | ForEach-Object { $_.ToString() }), $utf8)
$semanticOutput | Write-Output
$summary = @(
    'SUPPLEMENTAL STATIC CHECKS; UNITY/NUNIT EXECUTION NOT PERFORMED',
    ('SDK: ' + $sdkLine),
    ('NUnit reference: ' + $nunit),
    ('Syntax exit code: ' + $syntaxExit),
    ('09-10 assembly compatibility probe exit code: ' + $semanticExit),
    ('Reproduce: powershell -ExecutionPolicy Bypass -File docs/portfolio/tools/Validate-PpeCSharp.ps1 -ReferenceUnityEditorData "' + $ReferenceUnityEditorData + '"'),
    ('Compiler response files/logs: ' + $outputDirectory)
)
[System.IO.File]::WriteAllLines((Join-Path $outputDirectory 'summary.log'), $summary, $utf8)
$summary | Write-Output
$validationDirectory = Join-Path $PSScriptRoot '..\validation'
New-Item -ItemType Directory -Path $validationDirectory -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $outputDirectory 'syntax.log') -Destination (Join-Path $validationDirectory 'csharp-syntax.txt') -Force
Copy-Item -LiteralPath (Join-Path $outputDirectory 'modified-static.log') -Destination (Join-Path $validationDirectory 'csharp-compatibility.txt') -Force
Copy-Item -LiteralPath (Join-Path $outputDirectory 'summary.log') -Destination (Join-Path $validationDirectory 'csharp-summary.txt') -Force
$staticReport = @{
    checkedAtUtc = [DateTime]::UtcNow.ToString('o')
    classification = 'SUPPLEMENTAL STATIC CHECKS ONLY'
    sdk = $sdkLine
    syntax = @{
        fileCountPerConfiguration = @(Get-ChildItem (Join-Path $RepositoryRoot 'Assets\1.JSR') -Filter '*.cs' -Recurse).Count
        configurations = @('Player: no editor symbols', 'Editor: UNITY_EDITOR; UNITY_INCLUDE_TESTS')
        exitCode = $syntaxExit
        errorCount = @($syntaxOutput | Select-String 'error CS').Count
        log = 'csharp-syntax.txt'
    }
    compatibilityCompilation = @{
        sourceCount = $sources.Count
        sourceScope = 'All current Assets/1.JSR/Scripts sources and the three new PPE test fixtures'
        referenceBuildDate = '2026-09-10'
        baselineSourceSnapshotDate = '2026-09-18'
        referenceUnityEditorData = $ReferenceUnityEditorData
        referenceUnityEditorVersion = (Split-Path (Split-Path (Split-Path $ReferenceUnityEditorData -Parent) -Parent) -Leaf) + ' (reference assemblies only; Editor not started)'
        nunitReference = $nunit
        exitCode = $semanticExit
        errorCount = @($semanticOutput | Select-String 'error CS').Count
        warningCount = @($semanticOutput | Select-String 'warning CS').Count
        suppressedDiagnostic = 'CS0436: source type takes precedence over the older Assembly-CSharp type'
        log = 'csharp-compatibility.txt'
    }
    unityProjectCompilation = 'NOT VERIFIED; exact Unity 6000.4.8f1 was not available'
    editModeExecution = 'NOT RUN'
    playModeExecution = 'NOT RUN'
    headsetExecution = 'NOT RUN'
    limitation = 'Syntax and API compatibility checks do not import Unity assets, resolve current packages, execute NUnit, validate serialization, or establish scene/device behavior.'
    reproduction = 'powershell -ExecutionPolicy Bypass -File docs/portfolio/tools/Validate-PpeCSharp.ps1'
}
[System.IO.File]::WriteAllText((Join-Path $validationDirectory 'csharp-static.json'), ($staticReport | ConvertTo-Json -Depth 6), $utf8)
if ($syntaxExit -ne 0 -or $semanticExit -ne 0) { exit 1 }
