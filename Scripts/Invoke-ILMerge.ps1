<#
.SYNOPSIS
Use ILMerge to combine all import/export assemblies into a single SDK assembly.

.DESCRIPTION
To simplify consuming and deploying the import/export API's, all assemblies contained within the master solution are combined using ILMerge.

.PARAMETER SolutionDir
The solution directory.

.PARAMETER Configuration
The current build configuration.

.EXAMPLE
.\Test-PackageUpgrade.ps1 -SolutionDir "S:\SourceCode\DataTransfer\import-api-rdc\Source\" -Configuration "Release-ILMerge"
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string]$SolutionDir,
    [Parameter(Mandatory=$True)]
    [string]$Configuration
)

if (($Configuration -ne "Debug-ILMerge") -and ($Configuration -ne "Release-ILMerge")) {
    Write-Host "Skip merging the SDK assemblies for configuration $Configuration."
    exit 0
}

$SolutionDir = $SolutionDir.Trimend('\') + '\'
Write-Host "The configuration $Configuration supports merging the SDK assemblies."
$Root = Join-Path $SolutionDir "..\"
$PackagesDir = Join-Path $Root "packages"
$IlMergeExe = Join-Path $PackagesDir "ILMerge\tools\net452\ilmerge.exe"
$BuildArtifactsDir = Join-Path $Root "Artifacts"
$BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
$SdkBinariesArtifactsDir = Join-Path $BinariesArtifactsDir "sdk"
$LogsDir = Join-Path $Root "Logs"
$LogFile = Join-Path $LogsDir "ilmerge-build.log"
$MergedSdkFile = Join-Path $SdkBinariesArtifactsDir "Relativity.Import.Export.Client.dll"

if (Test-Path $SdkBinariesArtifactsDir -PathType Container) {
    Get-ChildItem $SdkBinariesArtifactsDir -Recurse | Remove-Item -Recurse
}
else {
    New-Item -ItemType Directory -Path $SdkBinariesArtifactsDir | Out-Null
}

Write-Host "Merging SDK assemblies..."
# Note: Relativity.Import.Client.dll must be the last entry to ensure the assembly attributes are copied correctly.
& $IlMergeExe @(
    ("/log:""$LogFile"""),
    ("/targetplatform:""v4,C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.2"""),
    ("/closed"),
    ("/copyattrs"),
    ("/allowMultiple"),
    ("/xmldocs"),
    ("/out:""$MergedSdkFile"""),
    ("""$SolutionDir\Relativity.Import.Export\bin\Relativity.Import.Export.dll"""),
    ("""$SolutionDir\Relativity.Import.Export.Legacy\bin\Relativity.Import.Export.Legacy.dll"""),
    ("""$SolutionDir\Relativity.Import.Export.Services.Interfaces\bin\Relativity.Import.Export.Services.Interfaces.dll"""),
    ("""$SolutionDir\Relativity.Export.Client\bin\Relativity.Export.Client.dll"""),
    ("""$SolutionDir\Relativity.Import.Client\bin\Relativity.Import.Client.dll"""))
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully merged SDK assemblies."
    exit 0
}
else {
    Write-Error "Failed to ILMerge the SDK assembly. Check the '$LogFile' for more info."
    exit 1
}