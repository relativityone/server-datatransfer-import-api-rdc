<#
.SYNOPSIS
This is the Import API and RDC build script.

.DESCRIPTION
This script is responsible for all build processes.

.EXAMPLE
.\build.ps1
Build the solution.

.EXAMPLE
.\build.ps1 -UnitTests
Builds the solution and then executes all unit tests.

.EXAMPLE
.\build.ps1 -SkipBuild -UnitTests
Skips building the solution and only executes all unit tests.

.EXAMPLE
.\build.ps1 -SkipBuild -UnitTests -IntegrationTests
Skips building the solution and executes all unit and integration tests.

.PARAMETER Target
The target to build (e.g. Build, Rebuild, or Clean).

.PARAMETER Configuration
Use this switch to choose the build configuration (e.g. Debug or Release).

.PARAMETER AssemblyVersion
Version of assemblies produced by the build.

.PARAMETER Verbosity
The verbosity of the build log.

.PARAMETER UnitTests
Executes all unit tests.

.PARAMETER IntegrationTests
Executes all integration tests.

.PARAMETER TestTimeoutInMS
Timeout for NUnit tests (in milliseconds).

.PARAMETER SkipBuild
Skips building the master solution.

.PARAMETER TestParametersFile
An optional test parameters JSON file that conforms to the standard App.Config file (e.g. Scripts\test-settings-sample.json)
#>

#Requires -Version 5.0
#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [ValidateSet("Build", "Rebuild", "Clean")]
    [String]$Target = "Build",
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [String]$Configuration = "Release",
    [Parameter()]
    [Version]$AssemblyVersion = "1.0.0.0",
    [Parameter()]
    [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")]
    [String]$Verbosity = "quiet",
    [Parameter()]
    [Switch]$UnitTests,
    [Parameter()]
    [Switch]$IntegrationTests,
    [Parameter()]
    [int]$TestTimeoutInMS = 90000,
    [Parameter()]
    [Alias("skip")]
    [Switch]$SkipBuild,
    [Parameter()]
    [String]$TestParametersFile
)

$BaseDir = $PSScriptRoot
$PackagesDir = Join-Path $BaseDir "packages"
$PaketDir = Join-Path $BaseDir ".paket"
$PaketExe = Join-Path $PaketDir 'paket.exe'
$PaketBootstrapperExe = Join-Path $PaketDir 'paket.bootstrapper.exe'
Write-Verbose "BaseDir resolves to $BaseDir"
Write-Verbose "Checking for Paket in the .paket sub-directory..."
if (-Not (Test-Path $PaketDir -PathType Container)) {
    New-Item -ItemType directory -Path $PaketDir
}

if (Test-Path $PaketExe -PathType Leaf) {
    Remove-Item $PaketExe
}

if (Test-Path $PaketBootstrapperExe -PathType Leaf) {
    Remove-Item $PaketBootstrapperExe
}

Invoke-WebRequest "https://github.com/fsprojects/Paket/releases/download/5.196.2/paket.exe" -OutFile $PaketExe

$PaketVerbosity = if ($VerbosePreference -gt "SilentlyContinue") { "--verbose" } else { "" }
Write-Verbose "Restoring packages via paket for $MasterSolution"
& $PaketExe restore $PaketVerbosity
if ($LASTEXITCODE -ne 0) 
{
	Throw "An error occured while restoring packages."
}

$TaskList = New-Object System.Collections.ArrayList($null)
$TaskList.Add("Build")
if ($UnitTests -or $IntegrationTests)
{
    $TaskList.Add("Test")
}

$Params = @{
    buildFile = Join-Path $BaseDir "default.ps1"
    taskList = $TaskList
    nologo = $true
    framework = "4.6.2"
    parameters = @{
        Root = $BaseDir
        PackagesDir = $PackagesDir
    }
    properties = @{
        Target = $Target
        Configuration = $Configuration
        AssemblyVersion = $AssemblyVersion        
        Verbosity = $Verbosity
        TestTimeoutInMS = $TestTimeoutInMS
        UnitTests = $UnitTests
        IntegrationTests = $IntegrationTests
        SkipBuild = $SkipBuild
        TestParametersFile = $TestParametersFile
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
	
	# Removing paket eliminates a VS paket extension conflict.
	if (Test-Path $PaketExe -PathType Leaf) {
        Remove-Item $PaketExe
    }
    
    if (Test-Path $PaketBootstrapperExe -PathType Leaf) {
        Remove-Item $PaketBootstrapperExe
    }
}

Exit $ExitCode