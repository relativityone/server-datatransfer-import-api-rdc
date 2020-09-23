<#
.SYNOPSIS
This is the Import API and RDC build script.

.DESCRIPTION
This script is responsible for all build processes.

.EXAMPLE
.\build.ps1 Build
Build the solution.

.EXAMPLE
.\build.ps1 UpdatePackages
Updates the packages and disables the analyzers when debugging correctly.

.EXAMPLE
.\build.ps1 BuildVersion  -Branch '${env.BRANCH_NAME}'
Does not build, returns version name for the branch name

.EXAMPLE
.\build.ps1 Build,BuildInstallPackages
Build the solution and creates the install packages.

.EXAMPLE
.\build.ps1 Build,BuildInstallPackages -Sign
Build the solution, creates the install packages, and digitally signs all associated binaries.

.EXAMPLE
.\build.ps1 BuildUIAutomation
Builds the solution for UIAutomation.

.EXAMPLE
.\build.ps1 BuildSQLDataComparer
Builds the solution for SQLDataComparer.

.EXAMPLE
.\build.ps1 Build,UnitTests
Builds the solution and then executes all unit tests.

.EXAMPLE
.\build.ps1 UnitTests
Skips building the solution and only executes all unit tests.

.EXAMPLE
.\build.ps1 UnitTests,IntegrationTests
Skips building the solution and executes all unit and integration tests.

.EXAMPLE
.\build.ps1 TestVM,UnitTest,IntegrationTests -TestVMName "P-DV-VM-SAD3ERA"
Skips building the solution, setup the integration test parameters using the specified TestVM, and executes all unit and integration tests.

.EXAMPLE
.\build.ps1 UnitTests,IntegrationTests -TestParametersFile ".\Scripts\test-settings-e2e.json"
Skips building the solution, setup the integration test parameters using the specified JSON file, and executes all unit and integration tests.

.EXAMPLE
.\build.ps1 UnitTests,IntegrationTests,TestReports -TestEnvironment "Hopper"
Skips building the solution, setup the integration test parameters using the Hopper test environment, executes all unit and integration tests, and creates test reports within the ".\Reports" sub-folder.

.EXAMPLE
.\build.ps1 UIAutomationTests
Skips building the solution and executes all UI tests.

.EXAMPLE
.\build.ps1 LoadTests
Skips building the solution and executes all LoadTests tests.

.EXAMPLE
.\build.ps1 IntegrationTestsForMassImportImprovementsToggle
Skips building the solution, set MassImportImprovementsToggle value and execute all Integration tests.

.EXAMPLE
.\build.ps1 CodeCoverageReport -TestEnvironment "Hopper"
Skips building the solution, setup the integration test parameters using the Hopper test environment, executes a code coverage report, and creates the code coverage report within the ".\Reports" sub-folder.

.EXAMPLE
.\build.ps1 RunSqlComparerTool
Skips building the solution, run Sql Comparer Tool for previous prepared comparer input

.EXAMPLE
.\build.ps1 GetRelativityBranchesForTests -ReleasedVersionName "lanceleaf"
Get names of folders with Relativity installers in location '\\bld-pkgs\Packages\Relativity\' for release branches in specified version.

.PARAMETER Target
The target to build (e.g. Build, Rebuild).

.PARAMETER Configuration
The build configuration (e.g. Debug or Release).

.PARAMETER Version
The build version.

.PARAMETER Verbosity
The verbosity of the build log.

.PARAMETER Branch
An optional branch name. This is only required for the PublishBuildArtifacts task.

.PARAMETER BuildNumber
An optional build number. This is only used when building feature branch packages.

.PARAMETER BuildPlatform
An optional build platform. (e.g. 'Any CPU', 'x86', 'x64')

.PARAMETER BuildUrl
An optional build URL. This is only required for the versioning task.

.PARAMETER TestTimeoutInMS
Timeout for NUnit tests (in milliseconds).

.PARAMETER TestParametersFile
An optional test parameters JSON file that conforms to the standard App.Config file (e.g. Scripts\test-settings-sample.json)

.PARAMETER TestReportFolderName
An optional parameter. If specified test reports will be put in TestReports\{TestReportFolderName}\ directory

.PARAMETER TestEnvironment
An optional test environment that maps to a test parameters JSON file.

.PARAMETER TestVMName
The optional TestVM used to execute all integration tests. This is only relevant for the IntegrationTests task.

.PARAMETER ILMerge
The optional parameter to apply ILMerge configurations to the build.

.PARAMETER Sign
The optional parameter to digitally sign the appropriate artifacts for the associated task. This will not work without a signing certificate installed onto your machine.

.PARAMETER ForcePublishRdcPackage
The optional parameter that forces publishing the RDC package.

.PARAMETER Simulate
The optional parameter that simulates executing a command. This is generally reserved for debug purposes.

.PARAMETER MassImportImprovementsToggle
The optional parameter used to execute LoadTests for two MassImportImprovementsToggle values.

.PARAMETER EnableDataGrid
The optional parameter used to execute tests for two enabled or disabled DataGrid.

.PARAMETER SqlProfiling
The optional parameter used to execute tests with sql profiling turned on.

.PARAMETER SqlDataComparer
The optional parameter used to execute tests with sql data comparer tool on.

.PARAMETER TestOnWorkspaceWithNonDefaultCollation
The optional parameter used to execute tests on workspace created from template with non default collation.

.PARAMETER ReleasedVersionName
The optional parameter used in Release branches pipeline to decide which Relativity versions should be tested.
#>

#Requires -Version 5.0
#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [Parameter(Position=0)]
    [string[]]$TaskList = @("Build"),
    [ValidateSet("Build", "Rebuild")]
    [String]$Target = "Build",
    [Parameter()]
    [ValidateSet("Debug", "Release")]
    [String]$Configuration = "Release",
    [Parameter()]
    [Version]$Version = "1.0.0.0",
    [Parameter()]
    [ValidateSet("quiet", "minimal", "normal", "detailed", "diagnostic")]
    [String]$Verbosity = "quiet",
    [Parameter()]
    [String]$Branch,
    [Parameter()]
    [String]$ProgetApiKey,
    [Parameter()]
    [String]$BuildNumber = "1",
    [Parameter()]
    [String]$BuildPlatform = "Any CPU",
    [Parameter()]
    [string]$BuildUrl = "localhost",
	[Parameter()]
    [string]$TestTarget,
    [int]$TestTimeoutInMS = 300000,
    [Parameter()]
    [String]$TestParametersFile,
    [Parameter()]
    [String]$TestReportFolderName,
    [Parameter()]
    [ValidateSet("Hopper")]
    [String]$TestEnvironment,
    [Parameter()]
    [String]$TestVMName,
    [Parameter()]
    [String]$EinsteinSecret,
    [Parameter()]
    [String]$PackageTemplateRegex = "paket.template.*$",
    [Parameter()]
    [nullable[bool]]$PublishToRelease,
    [Parameter()]
    [Switch]$ILMerge,
    [Parameter()]
    [Switch]$Sign,
    [Parameter()]
    [Switch]$ForcePublishRdcPackage,    
    [Parameter()]
    [Switch]$Simulate,
	[Parameter()]
    [Switch]$MassImportImprovementsToggle,
	[Parameter()]
    [Switch]$EnableDataGrid,
	[Parameter()]
    [Switch]$SqlProfiling,
	[Parameter()]
    [Switch]$SqlDataComparer,
	[Parameter()]
	[Switch]$TestOnWorkspaceWithNonDefaultCollation,
	[Parameter()]
	[String]$ReleasedVersionName
)

$BaseDir = $PSScriptRoot
$PackagesDir = Join-Path $BaseDir "packages"
$PaketFilesDir = Join-Path $BaseDir "paket-files"
$PaketDir = Join-Path $BaseDir ".paket"
$PaketExe = Join-Path $PaketDir 'paket.exe'
$PaketBootstrapperExe = Join-Path $PaketDir 'paket.bootstrapper.exe'
Write-Verbose "The base directory resolves to $BaseDir"
Write-Verbose "Checking for Paket in the .paket sub-directory..."
if (-Not (Test-Path $PaketDir -PathType Container)) {
    New-Item -ItemType directory -Path $PaketDir
}

if ("Clean" -in $TaskList) {

    if (Test-Path $PaketExe -PathType Leaf) {
        Remove-Item $PaketExe
    }

    if (Test-Path $PaketBootstrapperExe -PathType Leaf) {
        Remove-Item $PaketBootstrapperExe
    }

    if (Test-Path $PaketFilesDir -PathType Container) {
        Remove-Item -Recurse -Force $PaketFilesDir
    }
}

if (-Not (Test-Path $PaketExe -PathType Leaf)) {
	[Net.ServicePointManager]::SecurityProtocol = ([Net.SecurityProtocolType]::Tls12)
    Invoke-WebRequest "https://relativity.jfrog.io/relativity/misc-files-local/paket.exe" -OutFile $PaketExe
}

$PaketVerbosity = if ($VerbosePreference -gt "SilentlyContinue") { "--verbose" } else { "" }
Write-Verbose "Restoring packages via paket for $MasterSolution"
& $PaketExe restore $PaketVerbosity
if ($LASTEXITCODE -ne 0) 
{
	Throw "An error occured while restoring packages."
}

if (!$Branch) {
    $Branch = git rev-parse --abbrev-ref HEAD
}

$Params = @{
    buildFile = Join-Path $BaseDir "default.ps1"
    taskList = $TaskList
    nologo = $true
    framework = "4.6.2"
    parameters = @{
        Root = $BaseDir
        PackagesDir = $PackagesDir
        TestReportFolderName = $TestReportFolderName
    }
    properties = @{
        Target = $Target
		TestTarget = $TestTarget
        Configuration = $Configuration
        Version = $Version
        Branch = $Branch
        BuildNumber = $BuildNumber
        BuildPlatform = $BuildPlatform
        BuildUrl = $BuildUrl
		EinsteinSecret = $EinsteinSecret
        Verbosity = $Verbosity
        TestTimeoutInMS = $TestTimeoutInMS
        TestParametersFile = $TestParametersFile
        TestEnvironment = $TestEnvironment
        TestVMName = $TestVMName
        ILMerge = $ILMerge
        PublishToRelease = $PublishToRelease
        Sign = $Sign
        ForcePublishRdcPackage = $ForcePublishRdcPackage
        Simulate = $Simulate
		ProgetApiKey = $ProgetApiKey
		MassImportImprovementsToggle = $MassImportImprovementsToggle
		EnableDataGrid = $EnableDataGrid
		SqlProfiling = $SqlProfiling
		SqlDataComparer = $SqlDataComparer
		TestOnWorkspaceWithNonDefaultCollation = $TestOnWorkspaceWithNonDefaultCollation
		ReleasedVersionName = $ReleasedVersionName
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