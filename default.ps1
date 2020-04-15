﻿FormatTaskName "------- Executing Task: {0} -------"
Framework "4.6" #.NET framework version

properties {
    $LogsDir = Join-Path $Root "Logs"
    $PackagesDir = Join-Path $Root "packages"
    $PaketDir = Join-Path $Root ".paket"
    $SourceDir = Join-Path $Root "Source"
    $InstallersSolution = Join-Path $SourceDir "Installers.sln"
    $MasterSolution = Join-Path $SourceDir "Master.sln"
    $MasterILMergeSolution = Join-Path $SourceDir "Master-ILMerge.sln"
    $UIAutomationSolution = Join-Path $SourceDir "UIAutomation.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $BuildArtifactsDir = Join-Path $Root "Artifacts"
    $BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
    $InstallersArtifactsDir = Join-Path $BuildArtifactsDir "installers"
    $PackagesArtifactsDir = Join-Path $BuildArtifactsDir "packages"
    $SdkBinariesArtifactsDir = Join-Path $BinariesArtifactsDir "sdk"
    $ScriptsDir = Join-Path $Root "Scripts"
    $BuildPackagesDir = "\\bld-pkgs\Packages\Import-Api-RDC\"
    $BuildPackagesDirGold = "\\bld-pkgs\Release\Import-Api-RDC\"
#----------- testreports ------------	
    $TestReportsDir = Join-Path $Root "TestReports" | Join-Path -ChildPath $TestReportFolderName

    $CodeCoverageReportDir = Join-Path $TestReportsDir "code-coverage"        
    $DotCoverReportXmlFile = Join-Path $CodeCoverageReportDir "code-coverage-report.xml"
	
	$UnitTestsReportDir = Join-Path $TestReportsDir "unit-tests"
    $UnitTestsResultXmlFile = Join-Path $UnitTestsReportDir "test-results-unit.xml"
    $UnitTestsOutputFile = Join-Path $UnitTestsReportDir "unit-test-output.txt"

    $IntegrationTestsReportDir = Join-Path $TestReportsDir "integration-tests"
	$IntegrationTestsResultXmlFile = Join-Path $IntegrationTestsReportDir "test-results-integration.xml"
    $IntegrationTestsOutputFile =  Join-Path $IntegrationTestsReportDir "integration-test-output.txt"

    $UIAutomationTestsReportDir = Join-Path $TestReportsDir "ui-automation-tests"
    $UIAutomationTestsResultXmlFile = Join-Path $UIAutomationTestsReportDir "test-results-ui-automation.xml"
    $UIAutomationTestsOutputFile = Join-Path $UIAutomationTestsReportDir "ui-automation-test-output.txt"
#----------- end testreports ------------	
    $ExtentCliExe = Join-Path $PackagesDir "extent\tools\extent.exe"
    $NunitExe = Join-Path $PackagesDir "NUnit.ConsoleRunner\tools\nunit3-console.exe"
    $DotCoverExe = Join-Path $PackagesDir "JetBrains.dotCover.CommandLineTools\tools\dotCover.exe"
    $ReportGeneratorExe = Join-Path $PackagesDir "ReportGenerator\tools\net47\ReportGenerator.exe"
    $ReportUnitExe = Join-Path $PackagesDir "ReportUnit\tools\ReportUnit.exe"
    $PaketExe = Join-Path $PaketDir "paket.exe"
    $ProgetUrl = "https://proget.kcura.corp/nuget/NuGet"

    # Installer paths
    $SignScriptPath = Join-Path $ScriptsDir "Sign.bat"

    # Properties below this line are defined in build.ps1
    $Target = $Null
    $Configuration = $Null
    $BuildPlatform = $Null
    $BuildUrl = $Null
    $EinsteinSecret = $Null
    $Version = $Null
    $Branch = $Null
    $BuildNumber = $Null
    $Verbosity = $Null
	$TestTarget = $Null
    $TestTimeoutInMS = $Null
    $TestParametersFile = $Null
    $TestEnvironment = $Null
    $TestVMName = $Null
    $PackageTemplateRegex = $Null
    $ILMerge = $Null
    $PublishToRelease = $Null
    $Sign = $Null
    $SkipPublishRdcPackage = $Null
    $SkipPublishSdkPackage = $Null
    $Simulate = $Null
    $ProgetApiKey = $Null
}

$code = @"
using System;
namespace Paths
{
	public class UriScheme
	{
		public static string AddHttpsIfMissing(string s)
		{
			var x = new UriBuilder(s);
			x.Scheme = Uri.UriSchemeHttps;
			x.Port = -1;
			return x.Uri.ToString().TrimEnd('/');	
		}
		public static string GetHost(string s)
		{
			var x = new UriBuilder(s);
			return x.Host;	
		}
	}
}
"@
 
Add-Type -TypeDefinition $code -Language CSharp

Import-Module ".\DevelopmentScripts\branches.psm1"
Import-Module ".\DevelopmentScripts\testing.psm1"
Import-Module ".\DevelopmentScripts\folders.psm1"
Import-Module ".\DevelopmentScripts\versioning.psm1"
Import-Module ".\DevelopmentScripts\external.psm1"

task Build -Description "Builds the source code"  {
    folders\Initialize-Folder $LogsDir -Safe
    $SolutionFile = $MasterSolution
    $SolutionConfiguration = $Configuration
    if (!$BuildPlatform) {
        $BuildPlatform = "Any CPU"
    }

    if ($ILMerge) {
        folders\Initialize-Folder $SdkBinariesArtifactsDir
        $SolutionFile = $MasterILMergeSolution
        $SolutionConfiguration = "$SolutionConfiguration-ILMerge"
    }

    Write-Output "Solution: $SolutionFile"
    Write-Output "Configuration: $SolutionConfiguration"
    Write-Output "Build platform: $BuildPlatform"
    Write-Output "Target: $Target"
    Write-Output "Verbosity: $Verbosity"
    $lwrConfiguration = $SolutionConfiguration.ToLower()
    $LogFilePath = Join-Path $LogsDir "master-buildsummary-$lwrConfiguration.log"
    $ErrorFilePath = Join-Path $LogsDir "master-builderrors-$lwrConfiguration.log"

    try
    {
        exec {            
            msbuild @(($SolutionFile),
                    ("-t:$Target"),
                    ("-v:$Verbosity"),
                    ("-p:Platform=$BuildPlatform"),
                    ("-p:Configuration=$SolutionConfiguration"),
                    ("-p:BuildProjectReferences=true"),
                    ("-p:CopyArtifacts=true"),
                    ("-clp:Summary"),
                    ("-nodeReuse:false"),
                    ("-nologo"),
                    ("-maxcpucount"),
                    ("-flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity"),
                    ("-flp2:errorsonly;LogFile=`"$ErrorFilePath`""))
        } -errorMessage "There was an error building the master solution."
    }
    finally {
        testing\Remove-EmptyLogFile $ErrorFilePath
    }

    if ($Sign) {
        # To reduce spending a significant amount of time signing unnecessary files, limit the candidate folders.
        $directoryCandidates =  @(
            # The RDC binaries contained within the project must be signed to ensure harvesting includes digitally signed binaries.
            (Join-Path (Join-Path $SourceDir "Relativity.Desktop.Client.Legacy") "bin"),
            (Join-Path $BinariesArtifactsDir "Relativity.Desktop.Client.Legacy"),
            (Join-Path $BinariesArtifactsDir "Relativity.DataExchange.Import"),
            (Join-Path $BinariesArtifactsDir "sdk")
        )

        external\Invoke-SignDirectoryFiles -DirectoryCandidates $directoryCandidates
    }
}

task BuildInstallPackages -Description "Builds all install packages" {
    folders\Initialize-Folder $InstallersArtifactsDir
    folders\Initialize-Folder $LogsDir -Safe
    Write-Output "Building all RDC MSI and bootstrapper packages..."
    
    # Note: Digitally signing MSI/bootstrapper relies on MSBuild targets and a special sign script.
    $BuildPlatforms = @("x86", "x64")
    foreach ($platform in $BuildPlatforms) {
        Write-Output "Solution: $InstallersSolution"
        Write-Output "Configuration: $Configuration"
        Write-Output "Build platform: $platform"
        Write-Output "Target: $Target"
        Write-Output "Verbosity: $Verbosity"
        $LogFilePath = Join-Path $LogsDir "installers-buildsummary-$platform.log"
        $ErrorFilePath = Join-Path $LogsDir "installers-builderrors-$platform.log"
        
        try {
            exec { 
                # Note: BuildProjectReferences must be disabled to prevent overwriting digitally signed binaries!
                Write-Output "Building the $platform RDC MSI package..."
                msbuild @(($InstallersSolution),
                        ("-t:$Target"),
                        ("-v:$Verbosity"),
                        ("-p:Platform=$platform"),
                        ("-p:Configuration=$Configuration"),
                        ("-p:BuildProjectReferences=false"),
                        ("-p:CopyArtifacts=true"),
                        ("/property:SignOutput=$Sign"),
                        ("/property:SignToolPath=`"$global:SignToolPath`""),
                        ("/property:SignScriptPath=`"$SignScriptPath`""),
                        ("-clp:Summary"),
                        ("-nodeReuse:false"),
                        ("-nologo"),
                        ("-maxcpucount"),
                        ("-flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity"),
                        ("-flp2:errorsonly;LogFile=`"$ErrorFilePath`""))
                Write-Output "Successfully built the $platform RDC MSI package."
            } -errorMessage "There was an error building the RDC MSI."
        }
        finally {
            testing\Remove-EmptyLogFile $ErrorFilePath
        }
    }

    try {
        Write-Output "Solution: $InstallersSolution"
        Write-Output "Configuration: $Configuration-bootstrapper"
        Write-Output "Target: $Target"
        Write-Output "Verbosity: $Verbosity"
        $LogFilePath = Join-Path $LogsDir "bootstrapper-buildsummary-$platform.log"
        $ErrorFilePath = Join-Path $LogsDir "bootstrapper-builderrors-$platform.log"
        exec { 
            # Note: BuildProjectReferences must be disabled to prevent overwriting digitally signed binaries!
            Write-Output "Building the RDC bootstrapper package..."
            msbuild @(($InstallersSolution),
                    ("-t:$Target"),
                    ("-v:$Verbosity"),
                    ("-p:Platform=$platform"),
                    ("-p:Configuration=$Configuration-bootstrapper"),
                    ("-p:BuildProjectReferences=false"),
                    ("-p:CopyArtifacts=true"),
                    ("/property:SignOutput=$Sign"),
                    ("/property:SignToolPath=`"$global:SignToolPath`""),
                    ("/property:SignScriptPath=`"$SignScriptPath`""),
                    ("-clp:Summary"),
                    ("-nodeReuse:false"),
                    ("-nologo"),
                    ("-maxcpucount"),
                    ("-flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity"),
                    ("-flp2:errorsonly;LogFile=`"$ErrorFilePath`""))
            Write-Output "Successfully built the RDC bootstrapper package."
        } -errorMessage "There was an error building the RDC bootstrapper."

        Write-Output "Successfuly built all RDC MSI and bootstrapper packages."
    }
    finally {
        testing\Remove-EmptyLogFile $ErrorFilePath
    }
}

task BuildPackages -Depends BuildRdcPackage,BuildSdkPackages -Description "Builds all NuGet packages" {
}

task BuildSdkPackages -Description "Builds the SDK NuGet packages" {
    folders\Initialize-Folder $LogsDir -Safe
    folders\Initialize-Folder $PackagesArtifactsDir -Safe
    $version = versioning\Get-ReleaseVersion "$Branch"

    Write-Host "Package version: $version"
    Write-Host "Working directory: $PSScriptRoot"
    $packageLogFile = Join-Path $LogsDir "package-build.log"

    # Add any new package templates to the array.
    $packageTemplateFileNames = @("paket.template.relativity.dataexchange.client.sdk")
    foreach ($packageTemplateFileName in $packageTemplateFileNames) {
        $packageTemplateFile = Join-Path $PaketDir $packageTemplateFileName
        if (-Not (Test-Path $packageTemplateFile -PathType Leaf)) {
            Throw "The package cannot be created from template file '$packageTemplateFile' because it doesn't exist."
        }
        
        Write-Host "Creating package for template '$packageTemplateFile' and outputting to '$PackagesArtifactsDir'."
        exec {
             & $PaketExe pack --template `"$packageTemplateFile`" --version $version --symbols `"$PackagesArtifactsDir`" --log-file `"$packageLogFile`" 
        } -errorMessage "There was an error creating the SDK NUGet package."
    }
}

task BuildUIAutomation -Description "Builds the source code for UI Automation"  {
    folders\Initialize-Folder $LogsDir -Safe
    $SolutionFile = $UIAutomationSolution
    $SolutionConfiguration = $Configuration
    if (!$BuildPlatform) {
        $BuildPlatform = "Any CPU"
    }


    Write-Output "Solution: $SolutionFile"
    Write-Output "Configuration: $SolutionConfiguration"
    Write-Output "Build platform: $BuildPlatform"
    Write-Output "Target: $Target"
    Write-Output "Verbosity: $Verbosity"
    $lwrConfiguration = $SolutionConfiguration.ToLower()
    $LogFilePath = Join-Path $LogsDir "UIAutomation-buildsummary-$lwrConfiguration.log"
    $ErrorFilePath = Join-Path $LogsDir "UIAutomation-builderrors-$lwrConfiguration.log"

    try
    {
        exec {            
            msbuild @(($SolutionFile),
                    ("-t:$Target"),
                    ("-v:$Verbosity"),
                    ("-p:Platform=$BuildPlatform"),
                    ("-p:Configuration=$SolutionConfiguration"),
                    ("-p:BuildProjectReferences=true"),
                    ("-p:CopyArtifacts=true"),
                    ("-clp:Summary"),
                    ("-nodeReuse:false"),
                    ("-nologo"),
                    ("-maxcpucount"),
                    ("-flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity"),
                    ("-flp2:errorsonly;LogFile=`"$ErrorFilePath`""))
        } -errorMessage "There was an error building the master solution."
    }
    finally {
        testing\Remove-EmptyLogFile $ErrorFilePath
    }
}

task CheckSdkDependencies -Description "Checks if the references in ..\.paket\paket.template.relativity.dataexchange.client.sdk can be found in ..\paket.dependencies"{
    exec { 
        & "$ScriptsDir\Check-Sdk-Template.ps1" -SolutionDir $Root
    } -errorMessage "References in ..\.paket\paket.template.relativity.dataexchange.client.sdk are not equal to ..\paket.dependencies."
}

task CheckFolderAccess -Description "Checks if we can write to the destination path"{
    $testpath = Join-Path -Path $BuildPackagesDir -ChildPath "empty.txt"
	Try { 
		Set-Content $testpath ''
		Remove-Item $testpath
	}
	Catch { 
		Write-Warning "Unable to write to output file $testpath" 
		throw
	}
	$testpath = Join-Path -Path $BuildPackagesDirGold -ChildPath "Releases\empty.txt"
	Try { 
		Set-Content $testpath ''
		Remove-Item $testpath
	}
	Catch { 
		Write-Warning "Unable to write to output file $testpath" 
		throw
	}
}

task BuildRdcPackage -Description "Builds the RDC NuGet package" {
    folders\Initialize-Folder $LogsDir -Safe
    folders\Initialize-Folder $PackagesArtifactsDir -Safe
    $rdcVersionWixFile = Join-Path (Join-Path $SourceDir "Relativity.Desktop.Client.Setup") "Version.wxi"
    $packageVersion = versioning\Get-RdcVersion -rdcVersionWixFile $rdcVersionWixFile -branch $Branch
    Write-Host "Package version: $packageVersion"
    Write-Host "Working directory: $PSScriptRoot"
    $packageLogFile = Join-Path $LogsDir "rdc-package-build.log"
    Write-Host "Creating the RDC package and outputting to '$PackagesArtifactsDir'."
    $packageFile = Join-Path $PaketDir "paket.template.relativity.desktop.client"
    exec {
        & $PaketExe pack --template `"$packageFile`" --version $packageVersion --symbols `"$PackagesArtifactsDir`" --log-file `"$packageLogFile`" 
    } -errorMessage "There was an error creating the RDC NuGet package."
}

task BuildVersion -Description "Retrieves the build version from powershell" {
    Assert ($BuildUrl -ne $null -and $BuildUrl -ne "") "BuildUrl must be provided"
    Write-Host "Importing powershell properties.."

    $majorMinorIncrease = versioning\Get-ReleaseVersion "$Branch" -omitPostFix
    
    Write-Output "Build Url: $BuildUrl"
    $maxVersionLength = 50
    $localBuildVersion = $majorMinorIncrease
    if ($localBuildVersion.Length -gt $maxVersionLength) {
        Throw "The version length exceeds the maximum of $maxVersionLength characters and suggests a serious GIT or powershell issue."
    }

    $global:BuildVersion = $localBuildVersion

    # So Jenkins can get the version number
    Write-Output "buildVersion=$localBuildVersion"
}

task Clean -Description "Clean solution" {
    Write-Output "Removing all build artifacts"
    folders\Initialize-Folder $BuildArtifactsDir
    folders\Initialize-Folder $LogsDir
    folders\Initialize-Folder $TestReportsDir
    Write-Output "Running Clean target on $MasterSolution"
    exec { 
        msbuild $MasterSolution `
            "-t:Clean" `
            "-verbosity:$Verbosity" `
            "-p:Configuration=$Configuration" `
            "-nologo" `
    } -errorMessage "There was an error cleaning the master solution."

    exec { 
        msbuild $InstallersSolution `
            "-t:Clean" `
            "-verbosity:$Verbosity" `
            "-p:Configuration=$Configuration" `
            "-nologo" `
    } -errorMessage "There was an error cleaning the installer solution."
}

task CodeCoverageReport -Description "Create a code coverage report" {
    exec {
		folders\Initialize-Folder $LogsDir -Safe
        folders\Initialize-Folder $CodeCoverageReportDir

        folders\Initialize-Folder $UnitTestsReportDir
        folders\Initialize-Folder $IntegrationTestsReportDir

        ###
        # Step 1: We have x assemblies
        #     Output: x snapshos for dotcover
        #             x xml results from nunit
        #             x txt output from nunit
        # Step 2: merge x snapshots from dotcover
        #     Output: MergedCoverageSnapshots.dcvr
        # Step 3: Convert MergedCoverageSnapshots.dcvr - > CoverageReport.html (detailed report with classes and methods)
        # Step 4: Convert MergedCoverageSnapshots.dcvr - > .xml - > index.html (summary report for coverage)
        # Step 5: Convert x xml reports from nunit info one .html report (one of last task in Tridentfile)
        ###

        Write-Output "Searching for code coverage test assemblies..."

        ### 0. Get tests list to execute
        $UnitTests = Get-ChildItem -Path $SourceDir -Recurse -Include *Relativity*NUnit.dll -Exclude *Desktop*.dll,*TestFramework*.dll | Where-Object { $_.FullName -Match "\\*NUnit\\bin" }        
        $IntegrationTests = Get-ChildItem -Path $SourceDir -Recurse -Include *Relativity*NUnit.Integration.dll -Exclude *Desktop*.dll,*TestFramework*.dll | Where-Object { $_.FullName -Match "\\*NUnit.Integration\\bin" }        
        $LoadTests = Get-ChildItem -Path $SourceDir -Recurse -Include *Relativity*NUnit.LoadTests.dll -Exclude *Desktop*.dll,*TestFramework*.dll | Where-Object { $_.FullName -Match "\\*NUnit.LoadTests\\bin" }        

        $assemblies = @()
        $assemblies += $UnitTests
        $assemblies += $IntegrationTests
        $assemblies += $LoadTests

        $assemblyCount = $assemblies.Length
        if ($assemblyCount -le 0) {
            Throw "The cover coverage report cannot be created because no NUnit test assemblies were found."
        }

        testing\Invoke-SetTestParameters -SkipIntegrationTests $false -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment
         
        ### 1. Execute each Unit tests project separately, 
        #      Store results from NUnit execution  (used to summary of test execution in 'Write-TestResultsOutput') 
        #      Store snapshots from dotcover        (used to create merged report from all assembies execution)
        Write-Host "=======================================================================================================" 
        Write-Host " STEP 1: Execute tests, create NUnit and coverage reports" -ForegroundColor Yellow
        Write-Host "======================================================================================================="

        foreach ($assembly in $assemblies){
            [string]$fileName  = ($assembly.Name).Replace("Relativity.DataExchange.", "")
            [string]$ReportDir = ""

            if ($fileName.Contains("Integration")) { $ReportDir = $IntegrationTestsReportDir } else { $ReportDir = $UnitTestsReportDir}

            Write-Host
            Write-Host " Execute tests for '$assembly'"
            Write-Host " Details in file: '\\code-coverage\dotcover-execute-$fileName.txt'"
            
            & $DotCoverExe @(
                        ("cover"),
                        ("/TargetExecutable=""$NunitExe"""),
                        ("/Output=""$CodeCoverageReportDir\$fileName.dcvr"""),
                        ("/TargetArguments=""$assembly"" --result=""$ReportDir\$fileName.xml"" --output=""$ReportDir\$fileName.txt"""),
                        ("/Filters=""+:Relativity*;-:*NUnit*;-:*TestFramework*;-:*Controls*;""")) | Out-File "$CodeCoverageReportDir\dotcover-execute-$fileName.txt"
        }

        ### 2. Merge all dotcover snapshot logs into one report
        Write-Host "=======================================================================================================" 
        Write-Host " STEP 2: Merge dotcover snapshots" -ForegroundColor Yellow
        Write-Host "=======================================================================================================" 
        
        $CoverageSnapshots = Get-ChildItem -Path $CodeCoverageReportDir -Filter "*.dcvr" -Force     
        [string]$CoverageSnapshotList = ""
        foreach ($snapshot in $CoverageSnapshots){ $CoverageSnapshotList += $snapshot.FullName + ";" }

        & $DotCoverExe @(
                        ("m"),
                        ("/Source=$CoverageSnapshotList"),
                        ("/Output=""$CodeCoverageReportDir\MergedCoverageSnapshots.dcvr"""))

        # Remove part snapshots
        foreach ($snapshot in $CoverageSnapshots){ Remove-Item -LiteralPath $snapshot.FullName -Force }

        # 3. Convert merged coverage snapshots into html(detailed html showing each methos individually)
        Write-Host "=======================================================================================================" 
        Write-Host " STEP 3: Convert merged coverage snapshots into html(detailed html showing each methos individually)" -ForegroundColor Yellow
        Write-Host "=======================================================================================================" 
        & $DotCoverExe @(
                        ("r"),
                        ("/Source=""$CodeCoverageReportDir\MergedCoverageSnapshots.dcvr"""),
                        ("/Output=""$CodeCoverageReportDir\CoverageReport.html"""),
                        ("/ReportType=html"))

        ### 4. Convert merged coverage snapshots into html(overview for classes)
        Write-Host "=======================================================================================================" 
        Write-Host " STEP 4: Convert merged coverage snapshots into html(overview for classes)" -ForegroundColor Yellow
        Write-Host "=======================================================================================================" 
        
        # Convert to xml report
        & $DotCoverExe @(
                        ("r"),
                        ("/Source=""$CodeCoverageReportDir\MergedCoverageSnapshots.dcvr"""),
                        ("/Output=""$DotCoverReportXmlFile"""),
                        ("/ReportType=DetailedXML"))
        
        Write-Host "Details in file: \\code-coverage\dotcover-generate-html-report.txt" 

        # Convert to html report (index.html)
        & $ReportGeneratorExe @(
            ("-reports:""$DotCoverReportXmlFile"""),
            ("-targetdir:""$CodeCoverageReportDir"""),
            ("-reporttypes:Html")) | Out-File "$CodeCoverageReportDir\dotcover-generate-html-report.txt"

        # Remove not used file
        Remove-Item -LiteralPath "$CodeCoverageReportDir\MergedCoverageSnapshots.dcvr" -Force

        ### 5. Convert NUnit reports from tests to html: It is done in Trident file after all tests in 'TestReports' task

    } -errorMessage "There was an error creating a code coverage report."
}

task ExtendedCodeAnalysis -Description "Perform extended code analysis checks." {
    exec { 
        & "$ScriptsDir\Invoke-ExtendedCodeAnalysis.ps1" -SolutionFile $MasterSolution
    } -errorMessage "There was an error running the extended code analysis checks."
}

task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

task IntegrationTestsNightly -Description "Run all integration tests for the nightly pipeline" {
	folders\Initialize-Folder $TestReportsDir -Safe
    folders\Initialize-Folder $IntegrationTestsReportDir
	Testing\Invoke-IntegrationTests -TestCategoryFilter	"--where=`"cat==Integration and cat!=NotInCompatibility`""
}

task IntegrationTests -Description "Run all integration tests" {
    folders\Initialize-Folder $TestReportsDir -Safe
    folders\Initialize-Folder $IntegrationTestsReportDir
	Testing\Invoke-IntegrationTests -TestCategoryFilter	"--where=`"cat==Integration`""
}

task UIAutomationTests -Description "Runs all UI tests" {
    folders\Initialize-Folder $TestReportsDir -Safe
    folders\Initialize-Folder $UIAutomationTestsReportDir

    exec { & $NunitExe $testDllPath `
        "--labels=All" `
        "--agents=$NumberOfProcessors" `
        "--skipnontestassemblies" `
        "--timeout=$TestTimeoutInMS" `
        "--result=$UIAutomationTestsResultXmlFile" `
        "--out=$UIAutomationTestsOutputFile"
    } -errorMessage "There was an error running the UI tests."
}

task PackageVersion -Description "Retrieves the package version from powershell" {

    $localPackageVersion = versioning\Get-ReleaseVersion "$Branch"

    $maxVersionLength = 255
    if ($localPackageVersion.Length -gt $maxVersionLength) {
        Throw "The version length exceeds the maximum of $maxVersionLength characters and suggests a serious GIT or powershell issue."
    }

    $global:PackageVersion = $localPackageVersion

    # So Jenkins can get the package version number
    Write-Output "$localPackageVersion"
}

task PublishBuildArtifacts -Description "Publish build artifacts" {
    Assert ($Branch -ne "") "Branch is a required argument for saving build artifacts."
    Assert ($Version -ne "") "Version is a required argument for saving build artifacts."
    Assert ($PublishToRelease -ne $Null) "Determination of the type of build, gold or not, is required for saving the build artifacts."
    $targetDir = if ($PublishToRelease) { Join-Path $BuildPackagesDirGold "\Releases\$Version" } else { Join-Path $BuildPackagesDir "\$Branch\$Version" }  
    folders\Copy-Folder -SourceDir $LogsDir -TargetDir "$targetDir\logs"
    folders\Copy-Folder -SourceDir $BinariesArtifactsDir -TargetDir "$targetDir\binaries"
    folders\Copy-Folder -SourceDir $InstallersArtifactsDir -TargetDir "$targetDir\installers"
    folders\Copy-Folder -SourceDir $PackagesArtifactsDir -TargetDir "$targetDir\packages"
    folders\Copy-Folder -SourceDir $TestReportsDir -TargetDir "$targetDir\test-reports"
}

task PublishPackages -Description "Publishes packages to the NuGet feed" {
    $packageLogFile = Join-Path $LogsDir "package-publish.log"
    $filter = "*.nupkg"
	if([string]::IsNullOrWhiteSpace($ProgetApiKey))
	{
		Throw "The api key of the proget feed is not provided. This is required for this step (push to the nuget feed)"
	}
	
    if ($SkipPublishRdcPackage -and $SkipPublishSdkPackage) {
        Write-Host "Skip publishing the the SDK and REC packages."
        return
    }
    
    if ($SkipPublishRdcPackage) {
        $filter = "Relativity.DataExchange.Client.SDK*.nupkg"
        Write-Host "Pushing just the SDK .nupkg files contained within '$PaketDir' to '$ProgetUrl'."
    }
    elseif ($SkipPublishSdkPackage) {
        $filter = "*Relativity.Desktop.Client*.nupkg"
        Write-Host "Pushing just the RDC .nupkg files contained within '$PaketDir' to '$ProgetUrl'."
    }
    else {
        Write-Host "Pushing all SDK and RDC .nupkg files contained within '$PaketDir' to '$ProgetUrl'."
    }

    $path = Join-Path $PackagesArtifactsDir "*.*"
    foreach ($file in Get-ChildItem $path -Include $filter) {
        $packageFile = $file.FullName
        exec { 
            if (!$Simulate) {
                & $PaketExe push `"$packageFile`" --url `"$ProgetUrl`" --api-key `"$ProgetApiKey`" --verbose --log-file `"$packageLogFile`" 
            }
            else {
                Write-Host "Simulated pushing '$packageFile' to the '$ProgetUrl' NuGet feed."
            }
        } -errorMessage "There was an error pushing the packages."
    }
}

task SemanticVersions -Depends BuildVersion, PackageVersion -Description "Calculate and retrieve the semantic build and package versions" {
}

task TestReports -Description "Create the test reports" {

    folders\Initialize-Folder $UnitTestsReportDir -Safe
    folders\Initialize-Folder $IntegrationTestsReportDir -Safe
    exec {
        # See this page for CLI docs: https://github.com/extent-framework/extentreports-dotnet-cli
        if (Test-Path $UnitTestsResultXmlFile -PathType Leaf) {
            & $ExtentCliExe -i "$UnitTestsResultXmlFile" -o "$UnitTestsReportDir/" -r v3html
        }

        if (Test-Path $IntegrationTestsResultXmlFile -PathType Leaf) {
            & $ExtentCliExe -i "$IntegrationTestsResultXmlFile" -o "$IntegrationTestsReportDir/" -r v3html
        }

        # Convert reports using ReportUnit(only if exist files not converted above)
        $filesToConvert = Get-ChildItem -LiteralPath $UnitTestsReportDir -Include "*.xml" -Exclude $UnitTestsResultXmlFile -Force
        if ($filesToConvert) {
            Write-Host "Convert Unit tests results to html"
            Write-Host "Details in file: '\\unit-tests\create-html-report-from-unit-tests.txt'"

            & $ReportUnitExe @(
                        ("$UnitTestsReportDir"), 
                        ("$UnitTestsReportDir")) | Out-File "$UnitTestsReportDir\create-html-report-from-unit-tests.txt"
        }
        
        $filesToConvert = Get-ChildItem -LiteralPath $IntegrationTestsReportDir -Include "*.xml" -Exclude $IntegrationTestsResultXmlFile -Force
        if ($filesToConvert) {
            Write-Host "Convert Unit tests results to html"
            Write-Host "Details in file: '\\integration-tests\create-html-report-from-integration-tests.txt'"

            & $ReportUnitExe @(
                    ("$IntegrationTestsReportDir"), 
                    ("$IntegrationTestsReportDir")) | Out-File "$IntegrationTestsReportDir\create-html-report-from-integration-tests.txt"
        }

    } -errorMessage "There was an error creating the test reports."
}

task TestVMSetup -Description "Setup the test parameters for TestVM" {
    try {
        $testVM = $null
        if ($TestVMName) {
            $testVM = (Get-TestVm) | Where-Object { $_.BoxName -eq $TestVMName } | Select-Object
            if (-Not $testVM) {
                Throw "This operation cannot be performed because the TestVM $TestVMName doesn't exist."
            }
        }
        else {
            $testVM = (Get-TestVm) | Select-Object -First 1
            if (-Not $testVM) {
                Throw "This operation cannot be performed because there must be at least 1 TestVM available."
            }
        }

        $hostname = $testVM.BoxName 
        If ((Get-Content (Join-Path $testVM.Directory box.json) | ConvertFrom-Json).parameters.joinDomain.value -eq 0) { 
            $hostname = "$($testVM.BoxName).kcura.corp"
        }

        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYURL", "https://$hostname", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYRESTURL", "https://$hostname/relativity.rest/api", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYSERVICESURL", "https://$hostname/relativity.services", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYWEBAPIURL", "https://$hostname/relativitywebapi", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYUSERNAME", "relativity.admin@kcura.com", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_RELATIVITYPASSWORD", "Test1234!", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPASPERAMODETESTS", "true", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPDIRECTMODETESTS", "false", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "false", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLDROPWORKSPACEDATABASE", "true", "Process")
		[Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLCAPTUREPROFILING", "false", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLINSTANCENAME", "$hostname\\EDDSINSTANCE001", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLADMINUSERNAME", "sa", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SQLADMINPASSWORD", "P@ssw0rd@1", "Process")
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_WORKSPACETEMPLATE", "Relativity Starter Template", "Process")
        Write-Host "The test environment is setup with the $hostname TestVM."
    }
    catch {
        $errorMessage = $_.Exception.Message
        Write-Error "Failed to setup the TestVM for integration tests. Error: $errorMessage"
        throw
    }
}

task UnitTests -Description "Run all unit tests" {
    folders\Initialize-Folder $TestReportsDir -Safe
    folders\Initialize-Folder $UnitTestsReportDir

    $SolutionFile = $MasterSolution
    if ($ILMerge) {
        $SolutionFile = $MasterILMergeSolution
    }

    $testCategoryFilter = "--where=`"cat!=Integration`""
    testing\Invoke-SetTestParameters -SkipIntegrationTests $true -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment
    exec { & $NunitExe $SolutionFile `
            "--labels=All" `
            "--agents=$NumberOfProcessors" `
            "--skipnontestassemblies" `
            "--timeout=$TestTimeoutInMS" `
            "--result=$UnitTestsResultXmlFile" `
            "--out=$UnitTestsOutputFile" `
            $testCategoryFilter `
    } -errorMessage "There was an error running the unit tests."
}

task IntegrationTestResults -Description "Retrieve the integration test results from the Xml file" {
    testing\Write-TestResultsOutput $IntegrationTestsReportDir
}

task UnitTestResults -Description "Retrieve the unit test results from the Xml file" {
    testing\Write-TestResultsOutput $UnitTestsReportDir
}

task ReplaceTestVariables -Description "Replace test variables in file" {
$pathToFile = ".\Source\Relativity.DataExchange.TestFramework\Resources\test-parameters-hopper.json"
if ($TestParametersFile) {
    $pathToFile = $TestParametersFile
}
     $replaceTarget = [Paths.UriScheme]::AddHttpsIfMissing($TestTarget)
     $sqlserveraddress = [Paths.UriScheme]::GetHost($TestTarget)
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_sql_instance_name>',$sqlserveraddress) | Set-Content -Path $pathToFile
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_sql_user_name>','eddsdbo') | Set-Content -Path $pathToFile
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_sql_password>','P@ssw0rd@1') | Set-Content -Path $pathToFile
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_relativity_password>','Test1234!') | Set-Content -Path $pathToFile
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_relativity_user_name>','relativity.admin@kcura.com') | Set-Content -Path $pathToFile
    ((Get-Content -path $pathToFile -Raw) -replace '<replaced_in_build_target_to_test>', $replaceTarget) | Set-Content -Path $pathToFile
	Write-Host (Get-Content -path $pathToFile -Raw)
}

task PostReleasePageOnEinstein -Description "Post the releae page on Einstein"{
    $localSdkVersion = versioning\Get-ReleaseVersion "$Branch"
    $rdcVersionWixFile = Join-Path (Join-Path $SourceDir "Relativity.Desktop.Client.Setup") "Version.wxi"
	$localRdcVersion = versioning\Get-RdcVersion -rdcVersionWixFile $rdcVersionWixFile -branch $Branch
	$localPathToRdcExe = "$InstallersArtifactsDir\Relativity.Desktop.Client.Setup.exe"
    exec { 
        & $ScriptsDir\Invoke-EinsteinUpdate.ps1 -Secret $EinsteinSecret -SdkVersion $localSdkVersion -RdcVersion "$localRdcVersion" -branch "$Branch" -BuildPackagesDir "$BuildPackagesDir" -BuildPackagesDirGold "$BuildPackagesDirGold" -PathToLocalRdcExe "$localPathToRdcExe" -publishToRelease $PublishToRelease
    } -errorMessage "Failed to publish the release page on einstein."

}

task UpdateAssemblyInfo -Depends UpdateSdkAssemblyInfo,UpdateRdcAssemblyInfo -Description "Update the version contained within the SDK and RDC assembly shared info source files" {
}

task UpdateSdkAssemblyInfo -Description "Update the version contained within the SDK assembly shared info source file" {
    $version = versioning\Get-ReleaseVersion "$Branch" -omitPostFix
    versioning\Update-AssemblyInfo "$version.0"
}

task UpdateRdcAssemblyInfo -Description "Update the version contained within the RDC assembly shared info source file" {    
    exec { 
        $rdcVersionWixFile = Join-Path (Join-Path $SourceDir "Relativity.Desktop.Client.Setup") "Version.wxi"
        $majorMinorPatchVersion = versioning\Get-RdcWixVersion -rdcVersionWixFile $rdcVersionWixFile
        $postFix = versioning\Get-ReleaseVersion "$Branch" -postFixOnly
        $InformationalVersion = "$majorMinorPatchVersion$postFix"
        $VersionPath = Join-Path $Root "Version"
        $ScriptPath = Join-Path $VersionPath "Update-RdcAssemblySharedInfo.ps1"
        & $ScriptPath -Version "$majorMinorPatchVersion.0" -InformationalVersion $InformationalVersion -VersionFolderPath $VersionPath
   } -errorMessage "There was an error updating the RDC assembly info."
}

task RemoveRedundantTestOutputFiles -Description "Remove redundant test output files that should not be added to Trident logs" {
    # Remove .xml files only if .html reports exist
    
    [array]$UnitTestsList,$IntegrationTestsList = @()

    if(Test-Path -LiteralPath $UnitTestsReportDir) {
        $UnitTestsList = Get-ChildItem -LiteralPath $UnitTestsReportDir -Filter "*.xml" -Recurse -Force  
    }

    if(Test-Path -LiteralPath $IntegrationTestsReportDir) {
        $IntegrationTestsList = Get-ChildItem -LiteralPath $IntegrationTestsReportDir -Filter "*.xml" -Recurse -Force
    }

    $filesList = $UnitTestsList + $IntegrationTestsList

    if($filesList) {
        foreach($file in $filesList) {
            if(Test-Path -Path ($file.FullName).Replace(".xml", ".html") -PathType Leaf) { Remove-Item -LiteralPath $file.FullName -Force }
        }
    } 
}

task CreateTemplateTestParametersFile -Description "Create template of test parameters file" {
    if (-Not $TestParametersFile) {
        Throw "You need to specify path to new test parameters file (including file name and extension)"
    }
    $pathToTemplateFile = ".\Scripts\test-parameters-template.json"
    Copy-Item $pathToTemplateFile $TestParametersFile
}