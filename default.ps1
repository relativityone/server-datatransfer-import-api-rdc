FormatTaskName "------- Executing Task: {0} -------"
Framework "4.6" #.NET framework version

properties {
    $LogsDir = Join-Path $Root "Logs"
    $PackagesDir = Join-Path $Root "packages"
    $PaketDir = Join-Path $Root ".paket"
    $SourceDir = Join-Path $Root "Source"
    $InstallersSolution = Join-Path $SourceDir "Installers.sln"
    $MasterSolution = Join-Path $SourceDir "Master.sln"
    $MasterILMergeSolution = Join-Path $SourceDir "Master-ILMerge.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $BuildArtifactsDir = Join-Path $Root "Artifacts"
    $BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
    $InstallersArtifactsDir = Join-Path $BuildArtifactsDir "installers"
    $PackagesArtifactsDir = Join-Path $BuildArtifactsDir "packages"
    $SdkBinariesArtifactsDir = Join-Path $BinariesArtifactsDir "sdk"
    $ScriptsDir = Join-Path $Root "Scripts"
    $BuildPackagesDir = "\\bld-pkgs\Packages\Import-Api-RDC\"
    $TestReportsDir = Join-Path $Root "TestReports"
    $CodeCoverageReportDir = Join-Path $TestReportsDir "code-coverage"        
    $DotCoverConfigFile = Join-Path $ScriptsDir "code-coverage-report.xml"
    $DotCoverReportXmlFile = Join-Path $CodeCoverageReportDir "code-coverage-report.xml"
    $IntegrationTestsReportDir = Join-Path $TestReportsDir "integration-tests"
    $IntegrationTestsResultXmlFile = Join-Path $IntegrationTestsReportDir "test-results-integration.xml"
    $UnitTestsReportDir = Join-Path $TestReportsDir "unit-tests"
    $UnitTestsResultXmlFile = Join-Path $UnitTestsReportDir "test-results-unit.xml"
    $ExtentCliExe = Join-Path $PackagesDir "extent\tools\extent.exe"
    $GitVersionExe = Join-Path $PackagesDir "GitVersion.CommandLine\tools\GitVersion.exe"
    $NunitExe = Join-Path $PackagesDir "NUnit.ConsoleRunner\tools\nunit3-console.exe"
    $DotCoverExe = Join-Path $PackagesDir "JetBrains.dotCover.CommandLineTools\tools\dotCover.exe"
    $ReportGeneratorExe = Join-Path $PackagesDir "ReportGenerator\tools\net47\ReportGenerator.exe"
    $PaketExe = Join-Path $PaketDir "paket.exe"
    $ProgetUrl = "https://proget.kcura.corp/nuget/NuGet"
    $ProgetApiKey = "03abad83-912d-4f24-ae99-03b15444eec8"

    # Installer paths
    $SignToolPath = "${Env:ProgramFiles(x86)}\Windows Kits\8.1\bin\x86\signtool.exe"
    $SignScriptPath = Join-Path $ScriptsDir "Sign.bat"

    # Properties below this line are defined in build.ps1
    $Target = $Null
    $Configuration = $Null
    $BuildPlatform = $Null
    $BuildUrl = $Null
    $Version = $Null
    $Branch = $Null
    $BuildNumber = $Null
    $Verbosity = $Null
    $TestTimeoutInMS = $Null
    $TestParametersFile = $Null
    $TestEnvironment = $Null
    $TestVMName = $Null
    $PackageTemplateRegex = $Null
    $ILMerge = $Null
    $Sign = $Null
    $SkipPublishRdcPackage = $Null
    $SkipPublishSdkPackage = $Null
    $Simulate = $Null
}

task Build -Description "Builds the source code"  {
    Initialize-Folder $LogsDir -Safe
    $SolutionFile = $MasterSolution
    $SolutionConfiguration = $Configuration
    if (!$BuildPlatform) {
        $BuildPlatform = "Any CPU"
    }

    if ($ILMerge) {
        Initialize-Folder $SdkBinariesArtifactsDir
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
        Remove-EmptyLogFile $ErrorFilePath
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

        Invoke-SignDirectoryFiles -DirectoryCandidates $directoryCandidates
    }
}

task BuildInstallPackages -Description "Builds all install packages" {
    Initialize-Folder $InstallersArtifactsDir
    Initialize-Folder $LogsDir -Safe
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
                        ("/property:SignToolPath=`"$SignToolPath`""),
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
            Remove-EmptyLogFile $ErrorFilePath
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
                    ("/property:SignToolPath=`"$SignToolPath`""),
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
        Remove-EmptyLogFile $ErrorFilePath
    }
}

task BuildPackages -Depends BuildSdkPackages,BuildRdcPackage -Description "Builds all NuGet packages" {
}

task BuildSdkPackages -Description "Builds the SDK NuGet packages" {
    Initialize-Folder $LogsDir -Safe
    Initialize-Folder $PackagesArtifactsDir -Safe
    
    $majorMinorPatchVersion = & $GitVersionExe /output json /showvariable MajorMinorPatch
    $packageVersion =  Format-NuGetPackageVersion -MajorMinorPatchVersion $majorMinorPatchVersion
    Write-Host "Package version: $packageVersion"
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
             & $PaketExe pack --template `"$packageTemplateFile`" --version $packageVersion --symbols `"$PackagesArtifactsDir`" --log-file `"$packageLogFile`" 
        } -errorMessage "There was an error creating the SDK package."
    }
}

task BuildRdcPackage -Description "Builds the RDC NuGet package" {
    Initialize-Folder $LogsDir -Safe
    Initialize-Folder $PackagesArtifactsDir -Safe

    $majorMinorPatchVersion = Get-RdcWixVersion 
    $packageVersion =  Format-NuGetPackageVersion -MajorMinorPatchVersion $majorMinorPatchVersion
    Write-Host "Package version: $packageVersion"
    Write-Host "Working directory: $PSScriptRoot"
    $packageLogFile = Join-Path $LogsDir "rdc-package-build.log"
    Write-Host "Creating the RDC package and outputting to '$PackagesArtifactsDir'."
    $packageFile = Join-Path $PaketDir "paket.template.relativity.desktop.client"
    exec {
        & $PaketExe pack --template `"$packageFile`" --version $packageVersion --symbols `"$PackagesArtifactsDir`" --log-file `"$packageLogFile`" 
    } -errorMessage "There was an error creating the RDC package."
}

task BuildVersion -Description "Retrieves the build version from GitVersion" {
    Assert ($BuildUrl -ne $null -and $BuildUrl -ne "") "BuildUrl must be provided"
    Write-Output "Importing GitVersion properties.."

    $buildVersionMajor = & $GitVersionExe /output json /showvariable Major
    $buildVersionMinor = & $GitVersionExe /output json /showvariable Minor
    $buildVersionPatch = & $GitVersionExe /output json /showvariable Patch
    $buildVersionCommitNumber = & $GitVersionExe /output json /showvariable CommitsSinceVersionSource

    Write-Output "Build Url: $BuildUrl"
    Write-Output "Version major: $buildVersionMajor"
    Write-Output "Version minor: $buildVersionMinor"
    Write-Output "Version patch: $buildVersionPatch"
    Write-Output "Version commits number: $buildVersionCommitNumber"

    $version = "$buildVersionMajor.$buildVersionMinor.$buildVersionPatch.$buildVersionCommitNumber"
    $global:BuildVersion = $version

    # So Jenkins can get the version number
    Write-Output "buildVersion=$version"
}

task Clean -Description "Clean solution" {
    Write-Output "Removing all build artifacts"
    Initialize-Folder $BuildArtifactsDir
    Initialize-Folder $LogsDir
    Initialize-Folder $TestReportsDir
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
        Initialize-Folder $TestReportsDir -Safe
        Initialize-Folder $CodeCoverageReportDir
        $targetArgument = $Null
        Write-Output "Searching for code coverage test assemblies..."

        # Exclude all RDC related test assemblies because they're empty and will cause the CLI to fail.
        $assemblies = Get-ChildItem -Path $SourceDir -Recurse -Include *Relativity*NUnit*.dll -Exclude *Desktop*.dll,*TestFramework*.dll | Where-Object { $_.FullName -Match "\\bin" }
        $assemblyCount = $assemblies.Length
        if ($assemblyCount -le 0) {
            Throw "The cover coverage report cannot be created because no NUnit test assemblies were found."
        }

        foreach ($assembly in $assemblies) {
           $targetArgument += " " + $assembly.FullName
        }

        Invoke-SetTestParameters -SkipIntegrationTests $false -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment

        # TODO: A strange issue exists attempting to use the configuration file. Using command line params for now...
        Write-Output "Running code coverage on $assemblyCount test assemblies..."
        & $DotCoverExe @(
            ("cover"),
            ("/TargetExecutable=""$NunitExe"""),
            ("/TargetArguments=""$targetArgument"""),
            ("/Output=""$DotCoverReportXmlFile"""),
            ("/ReportType=""DetailedXML"""),
            ("/Filters=""+:Relativity*;-:*NUnit*;-:*TestFramework*;-:*Controls*;"""))

        Write-Output "Generating a code coverage report..."
        & $ReportGeneratorExe @(
            ("-reports:""$DotCoverReportXmlFile"""),
            ("-targetdir:""$CodeCoverageReportDir"""),
            ("-reporttypes:Html"))
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

task IntegrationTests -Description "Run all integration tests" {
    Initialize-Folder $TestReportsDir -Safe
    Initialize-Folder $IntegrationTestsReportDir
    $SolutionFile = $MasterSolution
    if ($ILMerge) {
        $SolutionFile = $MasterILMergeSolution
    }

    $OutputFile = Join-Path $IntegrationTestsReportDir "integration-test-output.txt"
    $testCategoryFilter = "--where=`"cat==Integration`""
    Invoke-SetTestParameters -SkipIntegrationTests $false -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment
    exec { & $NunitExe $SolutionFile `
            "--labels=All" `
            "--agents=$NumberOfProcessors" `
            "--skipnontestassemblies" `
            "--timeout=$TestTimeoutInMS" `
            "--result=$IntegrationTestsResultXmlFile" `
            "--out=$OutputFile" `
            $testCategoryFilter `
    } -errorMessage "There was an error running the integration tests."
}

task IntegrationTestResults -Description "Retrieve the integration test results from the Xml file" {
    Write-TestResultsOutput $IntegrationTestsResultXmlFile
}

task PackageVersion -Description "Retrieves the package version from GitVersion" {
    $version = & $GitVersionExe /output json /showvariable NuGetVersion
    $global:PackageVersion = $version

    # So Jenkins can get the package version number
    Write-Output "packageVersion=$version"
}

task PublishBuildArtifacts -Description "Publish build artifacts" {
    Assert ($Branch -ne "") "Branch is a required argument for saving build artifacts."
    Assert ($Version -ne "") "Version is a required argument for saving build artifacts."    
    $targetDir = "$BuildPackagesDir\$Branch\$Version"
    Copy-Folder -SourceDir $LogsDir -TargetDir "$targetDir\logs"
    Copy-Folder -SourceDir $BinariesArtifactsDir -TargetDir "$targetDir\binaries"
    Copy-Folder -SourceDir $InstallersArtifactsDir -TargetDir "$targetDir\installers"
    Copy-Folder -SourceDir $PackagesArtifactsDir -TargetDir "$targetDir\packages"
    Copy-Folder -SourceDir $TestReportsDir -TargetDir "$targetDir\test-reports"
}

task PublishPackages -Description "Publishes packages to the NuGet feed" {
    $packageLogFile = Join-Path $LogsDir "package-publish.log"
    $filter = "*.nupkg"
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
    exec {
        # See this page for CLI docs: https://github.com/extent-framework/extentreports-dotnet-cli
        if (Test-Path $UnitTestsResultXmlFile -PathType Leaf) {
            & $ExtentCliExe -i "$UnitTestsResultXmlFile" -o "$UnitTestsReportDir/" -r v3html
        }

        if (Test-Path $IntegrationTestsResultXmlFile -PathType Leaf) {
            & $ExtentCliExe -i "$IntegrationTestsResultXmlFile" -o "$IntegrationTestsReportDir/" -r v3html
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
    Initialize-Folder $TestReportsDir -Safe
    Initialize-Folder $UnitTestsReportDir
    $SolutionFile = $MasterSolution
    if ($ILMerge) {
        $SolutionFile = $MasterILMergeSolution
    }

    $OutputFile = Join-Path $UnitTestsReportDir "unit-test-output.txt"
    $testCategoryFilter = "--where=`"cat!=Integration`""
    Invoke-SetTestParameters -SkipIntegrationTests $true -TestParametersFile $TestParametersFile -TestEnvironment $TestEnvironment
    exec { & $NunitExe $SolutionFile `
            "--labels=All" `
            "--agents=$NumberOfProcessors" `
            "--skipnontestassemblies" `
            "--timeout=$TestTimeoutInMS" `
            "--result=$UnitTestsResultXmlFile" `
            "--out=$OutputFile" `
            $testCategoryFilter `
    } -errorMessage "There was an error running the unit tests."
}

task UnitTestResults -Description "Retrieve the unit test results from the Xml file" {
    Write-TestResultsOutput $UnitTestsResultXmlFile
}

task UpdateAssemblyInfo -Depends UpdateSdkAssemblyInfo,UpdateRdcAssemblyInfo -Description "Update the version contained within the SDK and RDC assembly shared info source files" {
}

task UpdateSdkAssemblyInfo -Description "Update the version contained within the SDK assembly shared info source file" {
    exec { 
         & $GitVersionExe /updateassemblyinfo .\Version\AssemblySharedInfo.cs .\Version\AssemblySharedInfo.vb 
    } -errorMessage "There was an error updating the assembly info."
}

task UpdateRdcAssemblyInfo -Description "Update the version contained within the RDC assembly shared info source file" {    
    exec { 
        $majorMinorPatchVersion = Get-RdcWixVersion
        $InformationalVersion = Format-NuGetPackageVersion -MajorMinorPatchVersion $majorMinorPatchVersion
        $VersionPath = Join-Path $Root "Version"
        $ScriptPath = Join-Path $VersionPath "Update-RdcAssemblySharedInfo.ps1"
        & $ScriptPath -Version "$majorMinorPatchVersion.0" -InformationalVersion $InformationalVersion -VersionFolderPath $VersionPath
   } -errorMessage "There was an error updating the RDC assembly info."
}

Function Copy-Folder {
    param(
        [String] $SourceDir,
        [String] $TargetDir
    )

    $robocopy = "robocopy.exe"
    Write-Output "Copying the build artifacts from $SourceDir to $TargetDir"
    & $robocopy "$SourceDir" "$TargetDir" /MIR /is /R:6 /W:10 /FP /MT

    # https://ss64.com/nt/robocopy-exit.html
    if ($LASTEXITCODE -ge 8) {
        Throw "An error occured while copying the build artifacts from $SourceDir to $TargetDir. Robocopy exit code = $LASTEXITCODE"
    }
}

Function Format-NuGetPackageVersion {
    param(
        [String]$MajorMinorPatchVersion
    )

    if (!$MajorMinorPatchVersion -or $MajorMinorPatchVersion.Length -eq 0) {
        Throw "The NuGet package version cannot be formatted because the Major.Minor.Patch value is null or empty."
    }
    
    $currentBranchName = & $GitVersionExe /output json /showvariable BranchName
    if (!$currentBranchName -or $currentBranchName.Length -eq 0) {
        Throw "The NuGet package version cannot be formatted because the branch name is null or empty."
    }

    $preReleaseLabel = & $GitVersionExe /output json /showvariable PreReleaseLabel
    $commitsSinceVersionSourcePadded = & $GitVersionExe /output json /showvariable CommitsSinceVersionSourcePadded
    $formattedVersion = ""

    # All validation exception messages go here.
    $preReleaseLabelExceptionMessage = "The NuGet package version cannot be formatted for branch '$currentBranchName' because the pre-release label is null or empty."
    $commitsSinceVersionSourcePaddedExceptionMessage = "The NuGet package version cannot be formatted for branch '$currentBranchName' because the total number of commits since the last tag is null or empty."
    $buildNumberExceptionMessage = "The NuGet package version cannot be formatted for branch '$currentBranchName' because the build number is null or empty."
        
    if ($currentBranchName -eq "master") {
        $formattedVersion = $MajorMinorPatchVersion
    }
    elseif ($currentBranchName -eq "develop") {
        if (!$preReleaseLabel -or $preReleaseLabel.Length -eq 0) {
            Throw $preReleaseLabelExceptionMessage
        }

        if (!$commitsSinceVersionSourcePadded -or $commitsSinceVersionSourcePadded.Length -eq 0) {
            Throw $commitsSinceVersionSourcePaddedExceptionMessage
        }

        $formattedVersion = "$MajorMinorPatchVersion-$preReleaseLabel$commitsSinceVersionSourcePadded"
    }
    else {
        # Feature branches will always be prefixed with the JIRA ticket number.
        $jiraTicketNumber = Get-JiraTicketNumberFromBranchName
        if ($jiraTicketNumber -and $jiraTicketNumber.Length -gt 0) {
            # See https://gitversion.readthedocs.io/en/latest/more-info/version-increments/ for more details on issues with semantic versioning, NuGet, and feature branches.
            if (!$preReleaseLabel -or $preReleaseLabel.Length -eq 0) {
                Throw $preReleaseLabelExceptionMessage
            }
            
            if (!$BuildNumber -or $BuildNumber.Length -eq 0) {
                Throw $buildNumberExceptionMessage
            }

            $paddedBuildNumber = $BuildNumber.PadLeft(4, '0')
            $formattedVersion = "$MajorMinorPatchVersion-$jiraTicketNumber-$preReleaseLabel$paddedBuildNumber"
        }
        else {
            # A hotfix may not include a pre-release label.
            $formattedVersion = $MajorMinorPatchVersion
            if ($preReleaseLabel -and $preReleaseLabel.Length -gt 0) {
                if (!$commitsSinceVersionSourcePadded -or $commitsSinceVersionSourcePadded.Length -eq 0) {
                    Throw $commitsSinceVersionSourcePaddedExceptionMessage
                }

                $formattedVersion += "$MajorMinorPatchVersion-$preReleaseLabel$commitsSinceVersionSourcePadded"
            }
        }
    }

    return $formattedVersion
}

Function Get-JiraTicketNumberFromBranchName {
    # Remove the REL number to reduce the version length.
    $options = [Text.RegularExpressions.RegexOptions]::IgnoreCase
    $regexMatch = [regex]::Match($Branch, "(?<jira>REL-\d+).*", $options)
    if (!$regexMatch.Success) {
        return $null
    }

    if (!$regexMatch.Groups["jira"]) {
        return $null
    }

    $jiraTicketNumber = $regexMatch.Groups["jira"].Value
    if ($jiraTicketNumber -and $jiraTicketNumber.Length -gt 0) {
        return $jiraTicketNumber
    }

    return $null
}

Function Get-RdcWixVersion {
    $rdcVersionWixFile = Join-Path (Join-Path $SourceDir "Relativity.Desktop.Client.Setup") "Version.wxi"
    if (-Not (Test-Path $rdcVersionWixFile -PathType Leaf)) {
        Throw "The RDC version cannot be determined because the WIX RCD version source file '$rdcVersionWixFile' doesn't exist."
    }

    [xml]$xml = Get-Content $rdcVersionWixFile
    $selector = "//processing-instruction('define')[starts-with(., 'ProductVersion')]"
    $node = $xml.SelectSingleNode($selector)
    if (!$node) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' doesn't define the expected ProductVersion variable."
    }

    $options = [Text.RegularExpressions.RegexOptions]::IgnoreCase
    $regexMatch = [regex]::Match($node.InnerText, "ProductVersion[ ]*=[ ]*""(?<value>.*)\""", $options)
    if (!$regexMatch.Success) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the regular expression match failed."
    }

    if (!$regexMatch.Groups["value"]) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the value cannot be determined from the regular expresssion match."
    }

    [string]$productVersion = $regexMatch.Groups["value"].Value
    if (!$productVersion -or $productVersion.Length -le 0) {
        Throw "The RDC version cannot be determined because the WIX RDC version source file '$rdcVersionWixFile' defines the ProductionVersion variable but the version is empty."
    }

    return $productVersion.Trim()
}

Function Initialize-Folder {
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $Path,
        [Parameter()]
        [switch] $Safe
    )

    if (!$Path) {
        Throw "You must specify a non-null path to initialize a folder. Check to make sure the path value or variable passed to this method is valid."
    }

    if ((Test-Path $Path) -and $Safe) {
        Write-Host "The directory '$Path' already exists."
        Return
    }

    if (Test-Path $Path) {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
        Write-Host "Deleted the '$Path' directory."
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop | Out-Null
    Write-Host "Created the '$Path' directory."
}

Function Invoke-SignDirectoryFiles {
    param(
        [String[]] $DirectoryCandidates
    )

    $retryAttempts = 3
    $sites = @(
        "http://timestamp.comodoca.com/authenticode",
        "http://timestamp.verisign.com/scripts/timstamp.dll",
        "http://tsa.starfieldtech.com"
    )
    
    foreach ($directory in $DirectoryCandidates) {
        if (-Not (Test-Path $directory -PathType Container)) {
            Throw "The '$directory' can't be digitally signed because the directory doesn't exist. Verify the build script and project files are in agreement."
        }

        $filesToSign = Get-ChildItem -Path $directory -Recurse -Include *.dll, *.exe, *.msi | Where-Object { $_.Name -Match ".*Relativity.*|.*kCura.*" }
        $totalFilesToSign = $filesToSign.Length
        if ($totalFilesToSign -eq 0) {
            Throw "The '$directory' can't be digitally signed because there aren't any candidate files within the directory. Verify the build script and project files are in agreement."
        }

        Write-Output "Signing $totalFilesToSign total files in $directory"
        foreach ($FileToSign in $filesToSign) {
            Invoke-SignFile $FileToSign.FullName
        }
    }
}

Function Invoke-SignFile {
    param(
        [String] $File
    )

    if (!$File) {
        Throw "You must specify a non-null path to digitally sign a file. Check to make sure the path value or variable passed to this method is valid."
    }
    
    $RetryAttempts = 3
    $SignSites = @(
        "http://timestamp.comodoca.com/authenticode",
        "http://timestamp.verisign.com/scripts/timstamp.dll",
        "http://tsa.starfieldtech.com"
    )

    $Signed = $false
    & $SigntoolPath verify /pa /q $File
    $Signed = $?
    if (-not $Signed) {
        For ($i = 0; $i -lt $RetryAttempts; $i++) {
            ForEach ($Site in $SignSites) {
                Write-Host "Attempting to sign" $File "using" $Site "..."
                & $SigntoolPath sign /a /t $Site /d "Relativity" /du "http://www.kcura.com" $File
                $Signed = $?
                if ($Signed) {
                    Write-Host "Signed $File Successfully!"
                    break
                }
            }  
                    
            if ($Signed) {
                break
            }
        }
        
        if (-not $Signed) {
            Throw "Failed to sign $File. See the error above."
        }
    }
    else {
        Write-Host $File "is already signed."
    }
}

Function Invoke-SetTestParameters {
    param(
        [bool] $SkipIntegrationTests,
        [String] $TestParametersFile,
        [String] $TestEnvironment
    )

    [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", $SkipIntegrationTests, "Process")
    if ($TestParametersFile) {
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_TEST_JSON_FILE", $TestParametersFile , "Process")
    }

    if ($TestEnvironment) {
        [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_TEST_ENV", $TestEnvironment , "Process")
    }
}

Function Remove-EmptyLogFile {
    param(
        [String] $LogFile
    )

    if (!$LogFile) {
        Throw "You must specify a non-null path to remove the empty logfile. Check to make sure the path value or variable passed to this method is valid."
    }

    # Remove the error log when none exist.
    if (Test-Path $LogFile -PathType Leaf) {
        if ((Get-Item $LogFile).length -eq 0) {
            Remove-Item $LogFile
        }
    }
}

Function Write-TestResultsOutput {
    param(
        [String] $TestResultsXmlFile
    )

    if (!$TestResultsXmlFile) {
        Throw "You must specify a non-null path to retrieve the test results XML file. Check to make sure the path value or variable passed to this method is valid."
    }

    if (-Not (Test-Path $TestResultsXmlFile -PathType Leaf)) {
        Throw "The test results cannot be retrieved because the Xml tests file '$TestResultsXmlFile' doesn't exist."
    }

    $xml = [xml] (Get-Content $TestResultsXmlFile)
    $passed = $xml.'test-run'.passed
    $failed = $xml.'test-run'.failed
    $skipped = $xml.'test-run'.skipped

    # So Jenkins can get the results
    Write-Output "testResultsPassed=$passed"
    Write-Output "testResultsFailed=$failed"
    Write-Output "testResultsSkipped=$skipped"
}