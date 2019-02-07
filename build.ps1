<#
.SYNOPSIS
This is the Import API sample build script.

.DESCRIPTION
This script is responsible for all build processes.

.EXAMPLE
.\build.ps1
Build using Tasks that run by default: Build.

.EXAMPLE
.\build.ps1 -Test
Run build then unit tests.

.EXAMPLE
.\build.ps1 -SkipBuild -Test
Skips build then unit tests.

.PARAMETER TaskList
List of Psake tasks to run. For more information run: build.ps1 help.

.PARAMETER Release
Use this switch to build Relativity's code in the Release configuration.

.PARAMETER BuildType
Type of build (e.g. DEV or GOLD).

.PARAMETER AssemblyVersion
Version of assemblies produced by build.

.PARAMETER Verbosity
The verbosity of the build log.

.PARAMETER Rebuild
Runs a rebuild (i.e. clean then build).

.PARAMETER Test
Runs UnitTest task.

.PARAMETER TestTimeoutInMS
Timeout for NUnit unit tests (in milliseconds).

.PARAMETER SkipBuild
Skips building Relativity code.
#>

#Requires -Version 5.0
#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [String[]]$TaskList = @("default"),
    [Parameter()]
    [Switch]$Release,
    [Parameter()]
    [ValidateSet("DEV", "GOLD", "ALPHA", "BETA", "RC")]
    [String]$BuildType = "DEV",
    [Parameter()]
    [Version]$AssemblyVersion = "1.0.0.0",
    [Parameter()]
    [ValidateSet("local", "teambranch")]
    [String]$ServerType = "local",
    [Parameter()]
    [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")]
    [String]$Verbosity = "quiet",
    [Parameter()]
    [Switch]$Rebuild,
    [Parameter()]
    [Switch]$Test,
    [Parameter()]
    [Switch]$UnitTest,
    [Parameter()]
    [int]$TestTimeoutInMS = 90000,
    [Parameter()]
    [Alias("skip")]
    [Switch]$SkipBuild,
    [Parameter()]
    [hashtable]$Tests
)

$BaseDir = $PSScriptRoot
$BuildToolsDir = Join-Path $BaseDir "buildtools"
$PackagesDir = Join-Path $BaseDir "packages"
$PaketDir = Join-Path $BaseDir ".paket"
$PaketExe = Join-Path $PaketDir 'paket.bootstrapper.exe'
Write-Verbose "BaseDir resolves to $BaseDir"
Write-Verbose "Checking for Paket in the .paket sub-directory..."
if (-Not (Test-Path $PaketDir -PathType Container)) {
    New-Item -ItemType directory -Path $PaketDir
}

if (-Not (Test-Path $PaketExe -PathType Leaf)) {
    Invoke-WebRequest "https://github.com/fsprojects/Paket/releases/download/5.196.2/paket.exe" -OutFile $PaketExe
}

$PaketVerbosity = if ($VerbosePreference -gt "SilentlyContinue") { "--verbose" } else { "" }
Write-Verbose "Restoring packages via paket for $MasterSolution"
& $PaketExe restore $PaketVerbosity
if ($LASTEXITCODE -ne 0) 
{
	Throw "An error occured while restoring build tools."
}

if ($Test -or $UnitTest)
{
    $TempList = New-Object System.Collections.ArrayList($null)
    $TempList.AddRange($TaskList)
    $TempList.Add("UnitTest")
    $TaskList = $TempList
}

$Params = @{
    buildFile = Join-Path $BaseDir "default.ps1"
    taskList = $TaskList
    nologo = $true
    framework = "4.6.2"
    parameters = @{
        Root = $BaseDir
        BuildToolsDir = $BuildToolsDir
    }
    properties = @{
        Release = $Release
        BuildType = $BuildType
        AssemblyVersion = $AssemblyVersion
        ServerType = $ServerType
        Verbosity = $Verbosity
        Rebuild = $Rebuild
        SkipBuild = $SkipBuild
        TestTimeoutInMS = $TestTimeoutInMS
        Tests = $Tests
    }

    Verbose = $VerbosePreference
}

# Execute the build
$PSakePath = Join-Path $PackagesDir "psake\tools\psake\psake.psm1"
if (-Not (Test-Path $PSakePath -PathType Leaf)) {
    Throw "The expected PSake path '$PSakePath' doesn't exist."
}

Import-Module $PSakePath

Try
{
    Invoke-PSake @Params
}
Finally
{
    $ExitCode = 0
    If ($psake.build_success -eq $False)
    {
        $ExitCode = 1
    }

    Remove-Module PSBuildTools -Force -ErrorAction SilentlyContinue
    Remove-Module psake -Force -ErrorAction SilentlyContinue
}

Exit $ExitCode