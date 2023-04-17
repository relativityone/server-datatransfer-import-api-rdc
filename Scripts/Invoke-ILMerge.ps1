<#
.SYNOPSIS
Use ILMerge to combine all import/export assemblies into a single SDK assembly.

.DESCRIPTION
To simplify consuming and deploying the import/export API's, all assemblies contained within the master solution are combined using ILMerge.

.PARAMETER SolutionDir
The solution directory.

.EXAMPLE
.\Test-PackageUpgrade.ps1 -SolutionDir "S:\SourceCode\DataTransfer\import-api-rdc\Source\"
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$SolutionDir
)

$SolutionDir = $SolutionDir.Trimend('\') + '\'
$Root = Join-Path $SolutionDir "..\"
$PackagesDir = Join-Path $Root "buildtools"
$IlMergeExe = Join-Path $PackagesDir "ILMerge\tools\net452\ilmerge.exe"
$BuildArtifactsDir = Join-Path $Root "Artifacts"
$BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
$SdkBinariesArtifactsDir = Join-Path $BinariesArtifactsDir "sdk"
$LogsDir = Join-Path $Root "Logs"
$LogFile = Join-Path $LogsDir "ilmerge-build.log"
$MergedSdkFile = Join-Path $SdkBinariesArtifactsDir "Relativity.DataExchange.Client.SDK.dll"

if (Test-Path $SdkBinariesArtifactsDir -PathType Container) {
    Get-ChildItem $SdkBinariesArtifactsDir -Recurse | Remove-Item -Recurse
}
else {
    New-Item -ItemType Directory -Path $SdkBinariesArtifactsDir | Out-Null
}

Write-Host "Merging SDK assemblies..."
# Note: Relativity.DataExchange.Import.dll must be the last entry to ensure the assembly attributes are copied correctly.
& $IlMergeExe @(
    ("/log:""$LogFile"""),
    ("/targetplatform:""v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"""),
    ("/closed"),
    ("/copyattrs"),
    ("/allowMultiple"),
    ("/xmldocs"),
    ("/out:""$MergedSdkFile"""),
    ("""$SolutionDir\Relativity.DataExchange.Core\bin\Relativity.DataExchange.Core.dll"""),
    ("""$SolutionDir\Relativity.DataExchange.Legacy\bin\Relativity.DataExchange.Legacy.dll"""),
    ("""$SolutionDir\Relativity.DataExchange.Export\bin\Relativity.DataExchange.Export.dll"""),
    ("""$SolutionDir\Relativity.DataExchange.Import\bin\Relativity.DataExchange.Import.dll"""))
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully merged SDK assemblies."
    exit 0
}
else {
    Write-Error "Failed to ILMerge the SDK assembly. Check the '$LogFile' for more info."
    exit 1
}