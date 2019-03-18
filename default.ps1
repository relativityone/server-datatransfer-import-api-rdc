FormatTaskName "------- Executing Task: {0} -------"
Framework "4.6" #.NET framework version

properties {
    $LogsDir = Join-Path $Root "Logs"
    $PackagesDir = Join-Path $Root "packages"
    $PaketDir = Join-Path $Root ".paket"
    $MasterSolution = Join-Path $Root "Source/Relativity.ImportAPI-RDC.sln"
    $NumberOfProcessors = (Get-ChildItem env:"NUMBER_OF_PROCESSORS").Value
    $BuildArtifactsDir = Join-Path $Root "Artifacts"
    $BinariesArtifactsDir = Join-Path $BuildArtifactsDir "binaries"
    $PackagesArtifactsDir = Join-Path $BuildArtifactsDir "packages"
    $ScriptsDir = Join-Path $Root "Scripts"
    $BuildPackagesDir = "\\bld-pkgs\Packages\Import-Api-RDC\"
    $TestResultsDir = Join-Path $Root "TestResults"
    $UnitTestResultsXmlFile = Join-Path $TestResultsDir "unit-test-results.xml"
    $IntegrationTestResultsXmlFile = Join-Path $TestResultsDir "integration-test-results.xml"
    $ExtentCliExe  = Join-Path $PackagesDir "extent\tools\extent.exe"
    $GitVersionExe = Join-Path $PackagesDir "GitVersion.CommandLine\tools\GitVersion.exe"
    $NunitExe = Join-Path $PackagesDir "NUnit.ConsoleRunner\tools\nunit3-console.exe"
    $PaketExe = Join-Path $PaketDir "paket.exe"
    $ProgetUrl = "https://proget.kcura.corp/nuget/NuGet"
    $ProgetApiKey = "03abad83-912d-4f24-ae99-03b15444eec8"

    # Properties below this line are defined in build.ps1
    $Target = $Null
    $Configuration = $Null
    $BuildPlatform = $Null
    $BuildUrl = $Null
    $Version = $Null
    $PackageVersion = $Null
    $Branch = $Null
    $Verbosity = $Null
    $TestTimeoutInMS = $Null
    $TestParametersFile = $Null
    $TestVMName = $Null
}

task Build -Description "Builds the source code" -Depends UpdateAssemblyInfo, CompileMasterSolution {
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
    Initialize-Folder $TestResultsDir
    Write-Output "Running Clean target on $MasterSolution"
    exec { msbuild $MasterSolution `
        "/t:Clean" `
        "/verbosity:$Verbosity" `
        "/p:Configuration=$Configuration" `
        "/nologo" `
    }
}

task CompileMasterSolution -Description "Compile the solution" {

    if (!$BuildPlatform) {
        $BuildPlatform = "Any CPU"
    }

    Write-Output "Solution: $MasterSolution"
    Write-Output "Configuration: $Configuration"
    Write-Output "Build platform: $BuildPlatform"
    Write-Output "Verbosity: $Verbosity"

    Initialize-Folder $LogsDir -Safe
    $LogFilePath = Join-Path $LogsDir "buildsummary.log"
    $ErrorFilePath = Join-Path $LogsDir "builderrors.log"

	# Always force binaries to get copied when building via build script.
    exec { msbuild $MasterSolution `
        "/t:$Target" `
        "/verbosity:$Verbosity" `
        "/p:Platform=$BuildPlatform" `
        "/p:Configuration=$Configuration" `
        "/p:CopyArtifacts=true" `
        "/clp:Summary"`
        "/nodeReuse:false" `
        "/nologo" `
        "/maxcpucount" `
        "/flp1:LogFile=`"$LogFilePath`";Verbosity=$Verbosity" `
        "/flp2:errorsonly;LogFile=`"$ErrorFilePath`""
    } -errorMessage "There was an error building the master solution."

    # Remove the error log when none exist.
    if (Test-Path $ErrorFilePath -PathType Leaf) {
        if ((Get-Item $ErrorFilePath).length -eq 0) {
            Remove-Item $ErrorFilePath
        }
    }
}

task DigitallySign -Description "Digitally sign all binaries"   {
    $retryAttempts = 3
    $sites = @("http://timestamp.comodoca.com/authenticode",
               "http://timestamp.verisign.com/scripts/timstamp.dll",
               "http://tsa.starfieldtech.com")
    $signtool = [System.IO.Path]::Combine(${env:ProgramFiles(x86)}, "Microsoft SDKs", "Windows", "v7.1A", "Bin", "signtool.exe")
    
    # To reduce spending 5 minutes blindly signing unnecessary files, limit to just the RDC/IAPI and use directory/file counts to verify.
    $folderNameCandidates = @("Relativity.Desktop.Client.Legacy", "Relativity.Import.Client")
    foreach($folder in $folderNameCandidates)
    {
        $directory = Join-Path $BinariesArtifactsDir $folder
        if (-Not (Test-Path $directory -PathType Container)) {
            Throw "The '$directory' can't be digitally signed because the directory doesn't exist. Verify the build script and project files are in agreement."
        }

        $filesToSign = Get-ChildItem -Path $directory -Recurse -Include *.dll,*.exe,*.msi | Where-Object { $_.Name -Match ".*Relativity.*|.*kCura.*" }
        $totalFilesToSign = $filesToSign.Length
        if ($totalFilesToSign -eq 0)
        {
            Throw "The '$directory' can't be digitally signed because there aren't any candidate files within the directory. Verify the build script and project files are in agreement."
        }

        Write-Output "Signing $totalFilesToSign total assemblies in $directory"
        foreach($fileToSign in $filesToSign)
        {
            $file = $fileToSign.FullName
            & $signtool verify /pa /q $file
            $signed = $?

            if (-not $signed) {

                For ($i =0; $i -lt $retryAttempts; $i++) {
                    ForEach ($site in $sites){
                        Write-Host "Attempting to sign" $file "using" $site "..."
                        & $signtool sign /a /t $site /d "Relativity" /du "http://www.kcura.com" $file
                        $signed = $?                    
                        if ($signed) {
                            Write-Host "Signed" $file "Successfully!"
                            break
                        }
                    }  
                    
                    if ($signed) {
                        break
                    }
                }
        
                if (-not $signed) {
                    Throw "Failed to sign the dlls. See the error above."
                }
            }
            else {
                Write-Host $file "is already signed!"
            }
        }
    }
}

task ExtendedCodeAnalysis -Description "Perform extended code analysis checks." {
    & "$ScriptsDir\Invoke-ExtendedCodeAnalysis.ps1" -SolutionFile $MasterSolution
}

task GenerateTestReport -Description "Generate a merged test report" {
    # This will generate index.html within the test results directory.
    exec { & $ExtentCliExe -d $TestResultsDir -o $TestResultsDir -r v3html --merge } -errorMessage "There was an error generating the test report."
}

task Help -Alias ? -Description "Display task information" {
    WriteDocumentation
}

task IntegrationTests -Description "Run all integration tests" {
    Initialize-Folder $TestResultsDir -Safe
    $OutputFile = Join-Path $TestResultsDir "integration-test-output.txt"
    $testCategoryFilter = "--where=`"cat==Integration`""
    [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "false", "Process")
    Invoke-SetTestParametersByFile $TestParametersFile
    exec { & $NunitExe $MasterSolution `
        "--labels=All" `
        "--domain=Multiple" `
        "--process=Multiple" `
        "--agents=$NumberOfProcessors" `
        "--skipnontestassemblies" `
        "--timeout=$TestTimeoutInMS" `
        "--result=$IntegrationTestResultsXmlFile" `
        "--out=$OutputFile" `
        $testCategoryFilter `
    } -errorMessage "There was an error running the integration tests."
}

task IntegrationTestsResults -Description "Retrieve the integration test results from the Xml file" {
    Write-TestResults $IntegrationTestResultsXmlFile
}

task PackageVersion -Description "Retrieves the package version from GitVersion" {
    $version = & $GitVersionExe /output json /showvariable NuGetVersion
    $global:PackageVersion = $version

    # So Jenkins can get the package version number
    Write-Output "packageVersion=$version"
}

task PublishBuildArtifacts -Description "Publish build artifacts"  {
    Assert ($Branch -ne "") "Branch is a required argument for saving build artifacts."
    Assert ($Version -ne "") "Version is a required argument for saving build artifacts."    
    $targetDir = "$BuildPackagesDir\$Branch\$Version"
    Copy-Folder -SourceDir $LogsDir -TargetDir "$targetDir\logs"
    Copy-Folder -SourceDir $BinariesArtifactsDir -TargetDir "$targetDir\binaries"
    Copy-Folder -SourceDir $PackagesArtifactsDir -TargetDir "$targetDir\packages"
    Copy-Folder -SourceDir $TestResultsDir -TargetDir "$targetDir\test-results"
}

task PublishPackages -Depends PublishPackagesInternal

task PublishPackagesInternal -Description "Pushes package to NuGet feed" {
    Assert ($Branch -ne "") "Branch is a required argument for publishing packages"
    Assert ($PackageVersion -ne "") "PackageVersion is a required argument for publishing packages"

    Initialize-Folder $LogsDir -Safe
    Initialize-Folder $PackagesArtifactsDir -Safe
    $preReleaseLabel = & $GitVersionExe /output json /showvariable PreReleaseLabel
    Write-Host "Branch name: $Branch"
    Write-Host "Pre-release label: $preReleaseLabel"
    Write-Host "Working directory: $PSScriptRoot"
    if (($Branch -ne "master" -and (-not $Branch -like "hotfix-*")) -and [string]::IsNullOrWhiteSpace($preReleaseLabel))
    {
        Write-Warning "PPP: Current branch '$Branch' has version that appears to be a release version and is not master. Packing and publishing will not occur. Exiting..."
        exit 0
    }

    $packageLogFile = Join-Path $LogsDir "package-build.log"
    Write-Host "Creating packages for all package templates contained within '$PaketDir' matching '$templateRegex' with version '$PackageVersion' and outputting to '$PackagesArtifactsDir'."
    foreach ($file in Get-ChildItem $PaketDir)
    {
        if (!($file.Name -match [regex]"paket.template.*$"))
        {
            continue
        }

        $templateFile = $file.FullName
        Write-Host "Creating package for template '$templateFile' and outputting to '$PackagesArtifactsDir'."
        exec { & $PaketExe pack --template `"$templateFile`" --version $PackageVersion --symbols `"$PackagesArtifactsDir`" --log-file `"$packageLogFile`" } -errorMessage "There was an error creating the package."
    }

    Write-Host "Pushing all .nupkg files contained within '$PaketDir' to '$ProgetUrl'."
    foreach($file in Get-ChildItem $PackagesArtifactsDir)
    {
        if ($file.Extension -ne '.nupkg')
        {
            continue
        }

        $packageFile = $file.FullName
        exec { & $PaketExe push `"$packageFile`" --url `"$ProgetUrl`" --api-key `"$ProgetApiKey`" --verbose --log-file `"$packageLogFile`" } -errorMessage "There was an error pushing the packages."
    }
}

task SemanticVersions -Depends BuildVersion, PackageVersion -Description "Calculate and retrieve the semantic build and package versions" {
}

task TestVMSetup -Description "Setup the test parameters for TestVM" {
    try
    {
        $testVM = $null
        if ($TestVMName) {
            $testVM = (Get-Testvm) | Where-Object { $_.BoxName -eq $TestVMName } | Select-Object
            if (-Not $testVM) {
                Throw "This operation cannot be performed because the TestVM $TestVMName doesn't exist."
            }
        }
        else {
            $testVM = (Get-Testvm) | Select-Object -First 1
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
    catch
    {
        $errorMessage = $_.Exception.Message
        Write-Error "Failed to setup the TestVM for integration tests. Error: $errorMessage"
        throw
    }
}

task UnitTests -Description "Run all unit tests" {
    Initialize-Folder $TestResultsDir -Safe
    $OutputFile = Join-Path $TestResultsDir "unit-test-output.txt"
    $testCategoryFilter = "--where=`"cat!=Integration`""
    [Environment]::SetEnvironmentVariable("IAPI_INTEGRATION_SKIPINTEGRATIONTESTS", "true", "Process")
    Invoke-SetTestParametersByFile $TestParametersFile
    exec { & $NunitExe $MasterSolution `
        "--labels=All" `
        "--domain=Multiple" `
        "--process=Multiple" `
        "--agents=$NumberOfProcessors" `
        "--skipnontestassemblies" `
        "--timeout=$TestTimeoutInMS" `
        "--result=$UnitTestResultsXmlFile" `
        "--out=$OutputFile" `
        $testCategoryFilter `
    }
}

task UnitTestResults -Description "Retrieve the unit test results from the Xml file" {
    Write-TestResults $UnitTestResultsXmlFile
}

task UpdateAssemblyInfo -Precondition { $Version -ne "1.0.0.0" } -Description "Update the AssemblyInfo files in \Version\" {
    $VersionPath = Join-Path $Root "Version"
    $ScriptPath = Join-Path $VersionPath "Update-AssemblyInfo.ps1"
    exec { & $ScriptPath -Version $Version -VersionFolderPath $VersionPath }
}

Function Initialize-Folder {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [String] $Path,
        [Parameter()]
        [switch] $Safe
    )

    if ((Test-Path $Path) -and $Safe)
    {
        Write-Host "The directory '$Path' already exists."
        Return
    }

    if (Test-Path $Path)
    {
        Remove-Item -Recurse -Force $Path -ErrorAction Stop
        Write-Host "Deleted the '$Path' directory."
    }

    New-Item -Type Directory $Path -Force -ErrorAction Stop | Out-Null
    Write-Host "Created the '$Path' directory."
}

Function Copy-Folder {
    param(
        [String] $SourceDir,
        [String] $TargetDir
    )

    $robocopy = "robocopy.exe"
    Write-Output "Copying the build artifacts from $SourceDir to $TargetDir"
    & $robocopy "$SourceDir" "$TargetDir" /MIR /is
    if ($LASTEXITCODE -ne 1) 
    {
	    Throw "An error occured while copying the build artifacts from $SourceDir to $TargetDir"
    }
}

Function Invoke-SetTestParametersByFile {
    param(
        [String] $TestParametersFile
    )

    if ($TestParametersFile) {
        if (-Not (Test-Path $TestParametersFile -PathType Leaf)) {
            Throw "The test parameters file '$TestParametersFile' was specified but doesn't exist."
        }

        $json = Get-Content -Raw -Path $TestParametersFile | ConvertFrom-Json
        foreach ($property in $json.PSObject.Properties) {
            $name = $property.Name
            $value = $property.Value

            # Ensure the parameters are in env var format.
            if (-Not ($name.StartsWith("IAPI_INTEGRATION_"))) {
                $name = "IAPI_INTEGRATION_" + $name.ToUpper()
            }

            [Environment]::SetEnvironmentVariable($name, $value , "Process")
        }
    }
}

Function Write-TestResults {
    param(
        [String] $TestResultsXmlFile
    )

    if (-Not (Test-Path $TestResultsXmlFile -PathType Leaf)) {
        Throw "The test results cannot be retrieved because the Xml tests file '$TestResultsXmlFile' doesn't exist."
    }

    $xml = [xml] (Get-Content $TestResultsXmlFile)
    $passed = $xml.'test-run'.passed
    $failed = $xml.'test-run'.failed
    $skipped = $xml.'test-run'.skipped

    # So Jenkins can get the results
    Write-Host "testResultsPassed: $passed"
    Write-Host "testResultsFailed: $failed"
    Write-Host "testResultsSkipped: $skipped"
}